using ErpToolkit.Models;
using static ErpToolkit.Helpers.Db.DogManager;

//----------------------------------------------------------------------------------
// Ordinamento topologico
//----------------------------------------------------------------------------------


//la struttura dei metadati (tabelle/campi/Xref) la costruisci una volta con DogManager (DogTable/DogField) e la salvi;
//poi, per ogni operazione, hai due cache: dbCache_LETTO(prima) e dbCache_DA_INSERIRE-AGGIORNARE (dopo) e da queste due vuoi:
//  -ricavare le effettive modifiche (add/update/delete),
//  -costruire le dipendenze solo sulle FK effettivamente coinvolte (ignorare FK null o FK non cambiati),
//  -ordinare le singole operazioni record-by-record evitando falsi ciclo quando il ciclo esiste solo a livello di schema ma non è attivato dai cambiamenti,
//  -se c'è un vero ciclo operativo (es. due nuovi record che si riferiscono a vicenda), offrire una strategia automatica per romperlo (es. inserire prima con FK=null e poi fare update) — solo se il campo FK è nullable,

//-----------------------
//Cosa fornisce il codice
//-----------------------
//  **BuildTypeDependencyGraph() — costruisce e ritorna il grafo type → parent types (serializzabile). Lo puoi chiamare una volta e salvare su disco.
//  **PlanChangesAndGenerateSql(...) — prende dbBefore e dbAfter(i tuoi due dbCache), il grafo dei tipi(carcato o calcolato), opzioni(se verificare orfani nel DB, se provare a rompere i cicli automaticamente), e restituisce:
//      -la sequenza ordinata di operazioni(record-by-record),
//      -il testo SQL completo(parametrizzato) generato usando DogManagerInt.sqlMantain(stessa tecnica usata nel tuo MntList),
//      -errori / warning rilevati(p.es.FK riferite a record che non esistono e non vengono creati),
//      -info su eventuale breaking automatico di cicli.



namespace ErpToolkit.Helpers.Db
{
    public static class DogManagerTopologicalSort
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerTopologicalSort()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************


        //================================================================================
        // COSTRUZIONE E SERIALIZZAZIONE DEL GRAFO
        //================================================================================
        public static Dictionary<System.Type, List<System.Type>> BuildTypeDependencyGraph(DogManager dogMng)
        {
            var graph = new Dictionary<System.Type, List<System.Type>>();

            foreach (var table in dogMng.tables.Values)
            {
                foreach (var field in table.fields.Where(f => f.optXREF))
                {
                    if (field.XrefObj == null) continue;

                    System.Type parent = field.XrefObj.table.tableTpy;
                    System.Type child = table.tableTpy;

                    if (!graph.ContainsKey(child))
                        graph[child] = new List<System.Type>();

                    if (!graph[child].Contains(parent))
                        graph[child].Add(parent);
                }
            }

            return graph;
        }

        //public static void SaveTypeGraphToFile(string path, Dictionary<System.Type, List<System.Type>> graph)
        //{
        //    var json = System.Text.Json.JsonSerializer.Serialize(graph, new System.Text.Json.JsonSerializerOptions
        //    {
        //        WriteIndented = true
        //    });
        //    File.WriteAllText(path, json);
        //}

        //public static Dictionary<System.Type, List<System.Type>> LoadTypeGraphFromFile(string path)
        //{
        //    var json = File.ReadAllText(path);
        //    return System.Text.Json.JsonSerializer.Deserialize<Dictionary<System.Type, List<System.Type>>>(json);
        //}

        //================================================================================
        // ORDINAMENTO TOPOLIGICO DELLE MODIFICHE
        //================================================================================
        public static List<ModelErp> PlanSortedChanges(
            DogManager dogMng,
            List<ModelErp> tabModels,   // Add/Update/Delete richiesti
            DogCache dogCache,          // stato DB
            Dictionary<System.Type, List<System.Type>> typeGraph, // grafo dipendenze
            bool checkDbForOrphans = true,
            bool tryBreakCyclesByNullingFK = true
        )
        {
            var toApply = new List<ModelErp>();

            foreach (var model in tabModels)
            {
                switch (model.action ?? ' ')
                {
                    case 'A': // Add
                        if (!ExistsInCache(dogCache, model))
                            toApply.Add(model);
                        break;

                    case 'M': // Update
                        object icode = model.getIcode();
                        if (UtilHelper.IsNullOrEmptyObject(icode))
                        {
                            throw new InvalidOperationException($"PlanSortedChanges: Icode non definito in Mantain {model.GetType().FullName}");
                        }
                        if (dogCache.dbCache.TryGetValue(model.GetType(), out var dict) &&
                            dict.TryGetValue(model.getIcode(), out var oldModel))
                        {
                            var diffModel = CreateDiffModel(dogMng, oldModel, model);
                            if (diffModel != null)
                                toApply.Add(diffModel);
                        }
                        break;

                    case 'D': // Delete
                        if (ExistsInCache(dogCache, model))
                            toApply.Add(model);
                        break;
                }
            }

            // --- Ordina i record in base alle dipendenze ---
            return OrderByTopologicalSort(toApply, dogMng, dogCache);
        }

        //================================================================================
        // SUPPORT FUNCTIONS
        //================================================================================
        private static bool ExistsInCache(DogCache cache, ModelErp model)
        {
            return cache.dbCache.TryGetValue(model.GetType(), out var dict) &&
                   dict.ContainsKey(model.getIcode());
        }

        private static ModelErp? CreateDiffModel(DogManager dogMng, ModelErp oldModel, ModelErp newModel)
        {
            var table = dogMng.getTable(newModel.GetType());
            bool hasDiff = false;

            var diff = (ModelErp)dogMng.CopyModelErp(newModel);   //^^//var diff = (ModelErp)newModel.Copy();
            diff.resetMnt();

            foreach (var field in table.fields)
            {
                if (field.optSYS) continue; //non confronto i campi di sistema   //if (!field.canUpdate) continue;

                var oldVal = field.GetValue(oldModel);
                var newVal = field.GetValue(newModel);

                if (Equals(oldVal, newVal))
                {
                    field.SetValue(diff, null);
                }
                else
                {
                    hasDiff = true;
                }
            }

            return hasDiff ? diff : null;
        }

        private static List<ModelErp> OrderByTopologicalSort(
            List<ModelErp> models,
            DogManager dogMng,
            DogCache cache,
            int maxPasses = 1000)
        {
            var ordered = models.ToList();
            int n = ordered.Count;

            bool swapped;
            int passes = 0;

            do
            {
                swapped = false;
                passes++;

                for (int i = 0; i < n - 1; i++)
                {
                    var a = ordered[i];
                    var b = ordered[i + 1];

                    if (MustComeAfter(a, b, dogMng, cache, ordered))
                    {
                        // scambia
                        ordered[i] = b;
                        ordered[i + 1] = a;
                        swapped = true;
                    }
                }

                if (passes > maxPasses)
                    throw new InvalidOperationException("Ordinamento non riuscito: possibile ciclo irrisolvibile.");
            }
            while (swapped);

            return ordered;
        }

        private static bool MustComeAfter(ModelErp a, ModelErp b, DogManager dogMng, DogCache cache, List<ModelErp> models)
        {
            var tableA = dogMng.getTable(a.GetType());
            var tableB = dogMng.getTable(b.GetType());

            // --- Caso Add/Update ---
            if (a.action is 'A' or 'M')
            {
                foreach (var fk in tableA.fields.Where(f => f.optXREF && f.XrefObj != null))
                {
                    var val = fk.GetValue(a);
                    if (val == null) continue;

                    if (b.getIcode().Equals(val) && b.GetType() == fk.XrefObj.table.tableTpy)
                    {
                        Console.WriteLine($"[ORDER] {a.GetType().Name}/{a.getIcode()} dipende da {b.GetType().Name}/{b.getIcode()} (FK {fk.fieldName})");
                        return true; // a deve venire dopo b
                    }
                }
            }

            // --- Caso Delete ---
            if (a.action == 'D')
            {
                foreach (var fk in tableB.fields.Where(f => f.optXREF && f.XrefObj?.table.tableTpy == a.GetType()))
                {
                    var val = fk.GetValue(b);
                    if (val != null && val.Equals(a.getIcode()))
                    {
                        Console.WriteLine($"[DELETE] {a.GetType().Name}/{a.getIcode()} deve venire dopo {b.GetType().Name}/{b.getIcode()} (b referenzia a)");
                        return false;
                    }
                }
            }

            // --- Caso Add vs Add con ciclo reciproco ---
            if (a.action == 'A' && b.action == 'A')
            {
                foreach (var fkA in tableA.fields.Where(f => f.optXREF && f.XrefObj?.table.tableTpy == b.GetType()))
                {
                    var valA = fkA.GetValue(a);
                    if (valA == null || !valA.Equals(b.getIcode())) continue;

                    foreach (var fkB in tableB.fields.Where(f => f.optXREF && f.XrefObj?.table.tableTpy == a.GetType()))
                    {
                        var valB = fkB.GetValue(b);
                        if (valB == null || !valB.Equals(a.getIcode())) continue;

                        // ⚠️ ciclo reciproco trovato!
                        if (IsNullableField(fkA))
                        {
                            Console.WriteLine($"[CYCLE] Trovato ciclo reciproco tra {a.GetType().Name}/{a.getIcode()} e {b.GetType().Name}/{b.getIcode()}.");
                            Console.WriteLine($"[CYCLE] Spezzo: imposto {a.GetType().Name}.{fkA.fieldName}=NULL e creo un Update successivo.");

                            // 1) spezza il ciclo: metto FK di a a null
                            fkA.SetValue(a, null);

                            // 2) creo update posteriore che ripristina la FK
                            var update = CreateUpdateFromAdd(dogMng, a, fkA, valA);

                            // 3) aggiungo l'update subito dopo b
                            var index = models.IndexOf(b);
                            models.Insert(index + 1, update);

                            Console.WriteLine($"[CYCLE] Creato update {update.GetType().Name}/{update.getIcode()} per ripristinare la FK {fkA.fieldName}={valA}");

                            return true; // forza swap
                        }
                        else
                        {
                            Console.WriteLine($"[ERROR] Ciclo reciproco rilevato ma la FK {fkA.fieldName} non è nullable → impossibile spezzare automaticamente.");
                        }
                    }
                }
            }

            return false;
        }
        private static ModelErp CreateUpdateFromAdd(DogManager dogMng, ModelErp addModel, DogField fk, object fkValue)
        {
            var update = (ModelErp)dogMng.CloneModelErp(addModel);    //^^// var update = (ModelErp)addModel.Clone();
            update.action = 'M';
            fk.SetValue(update, fkValue);
            return update;
        }

        private static bool IsNullableField(DogField fld)
        {
            var ft = fld.fieldTyp;
            if (!ft.IsValueType) return true; // reference type
            if (Nullable.GetUnderlyingType(ft) != null) return true;
            return true; // string, object ecc.
        }




    }
}
