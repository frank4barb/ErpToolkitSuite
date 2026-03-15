using System.Data;
using System.Reflection;
using System.Collections;
using static ErpToolkit.Helpers.Db.DogManager;
using ErpToolkit.Models;
using static Google.Protobuf.Reflection.UninterpretedOption.Types;
using System.ComponentModel;
using System;


namespace ErpToolkit.Helpers.Db
{
    public static class DogManagerClone
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerClone()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************


        //*************
        // TRUNCATE CLONE
        //*************

        internal static ModelErp? TruncateCloneModelErp(DogManager dogMng, ModelErp? source, int maxDepth, int depth = 0, char? action = null, string? options=null)
        {
            ModelErp clone = null; string place = "init";
            if (maxDepth - depth <= 0 || source == null) return null;

            var typeSource = source.GetType();


            if (dogMng == null) throw new Exception($"TruncateCloneModelErp: dogMng == null.");
            if (!dogMng.tabTypes.ContainsKey(typeSource)) throw new Exception($"TruncateCloneModelErp: Classe {typeSource} non trovata.");
            DogTable tab = dogMng.tabTypes[typeSource];
            if (tab == null) throw new Exception($"TruncateCloneModelErp: tab [{typeSource}] == null.");
            if (tab.fldIcode == null) throw new Exception($"TruncateCloneModelErp: tab.fldIcode [{typeSource}] == null.");



            try
            {
                clone = (ModelErp)Activator.CreateInstance(typeSource)!;
                clone.depth = depth; // Imposta la profondità del clone
                if (action != null) clone.action = action;
            }
            catch (TargetInvocationException ex)
            {
                throw new TargetInvocationException($"DogManager: Errore interno CacheFillNull TruncateCloneModelErp CreateInstance[{typeSource.FullName}, {place}.{depth}]: " + ex.InnerException?.Message, ex);
            }


            // Copia i campi interni
            clone.depth = depth; //profondità di inclusione dell'oggetto (esternamente può essere solo letto)
            clone.action = (action != null) ? action : source.action;   // [R]ead, [A]dd, [M]odify, [D]elete // proprietà necessarie per la mantain e list del record
            clone.options = source.options;
            clone.vars = new Dictionary<string, string>(source.vars); // Copia il dizionario vars
            clone.jsonOriginal = source.jsonOriginal; //variabile che consente di caricare la stringa json originale ricevuta dal client (utile per confrontare i valori cambiati)


            //copia valore dei singoli campi
            foreach (var fld in tab.fields)
            {
                object? sourceValue = fld.GetValue(source);
                if (UtilHelper.IsNullOrEmptyObject(sourceValue))
                {
                    fld.SetValue(clone, null);
                    if (fld.optXREF) fld.SetObjValue(clone, null); ;
                    continue;
                }  // Salta se il valore della chiave è null o stringa vuota
                //---
                fld.CopyValue(clone, sourceValue);
                if (fld.optXREF)
                {
                    object? sourceObjValue = fld.GetObjValue(source);
                    object? cloneSourceObjValue = (object?)TruncateCloneModelErp(dogMng, (ModelErp)sourceObjValue, maxDepth, depth + 1, action: action);
                    fld.SetObjValue(clone, cloneSourceObjValue);
                }
            }

            //copia valore delle tabelle referenziate
            if (maxDepth > depth + 1)  //evito di clonare le tabelle referenziate se ho già raggiunto la profondità massima
            {
                foreach (var xrefFld in tab.XrefFromFld)  //XrefFromFld: lista dei campi che referenziano questa tabella
                {
                    if (source.xrefFrom.ContainsKey(xrefFld.fieldName))
                    {
                        // check if List<T>
                        if (xrefFld.optXREFlist && xrefFld.fieldXrefListTyp != null)    //se è definita la proprietà List<..xrefFld.table.tableTpy..> Xref..xrefFld.fieldName.. nel modello tab.tableTpy
                        {
                            var sourceList = ((IList)source.xrefFrom[xrefFld.fieldName]).Cast<ModelErp>().ToList();
                            var cloneList = (IList)Activator.CreateInstance(xrefFld.fieldXrefListTyp);

                            //-----------------
                            // inserisco primo record vuoto per fare add su tabella in grafica cshtml
                            if (options != null && options.Contains("inserisco_primo_record_vuoto_per_fare_add_su_tabella_in_grafica_cshtml"))
                            {
                                var cloneElem = Activator.CreateInstance(xrefFld.table.tableTpy);
                                ((ModelErp)cloneElem).action = 'X';
                                cloneList.Add(cloneElem);
                            }
                            //-----------------

                            foreach (var item in sourceList)
                            {
                                var clonedItem = TruncateCloneModelErp(dogMng, (ModelErp)item, maxDepth, depth + 1, action: action);
                                cloneList.Add(clonedItem);
                            }
                            xrefFld.SetListXrefValue(clone, cloneList); //var prop = clone.GetType().GetProperty("Xref" + xrefFld.fieldName); //prop.SetValue(clone, cloneList);
                        }
                        // check if Dictionary<object,T>
                        if (xrefFld.optXREFdict && xrefFld.fieldXrefDictTyp != null)    //se è definita la proprietà Dictionary<string,..xrefFld.table.tableTpy..> Xref..xrefFld.fieldName.. nel modello tab.tableTpy
                        {
                            var sourceList = ((IList)source.xrefFrom[xrefFld.fieldName]).Cast<ModelErp>().ToList();
                            if (sourceList == null) { xrefFld.SetDictXrefValue(clone, null); }
                            else
                            {
                                var cloneDict = (System.Collections.IDictionary)Activator.CreateInstance(xrefFld.fieldXrefDictTyp);
                                foreach (var item in sourceList)
                                {
                                    var clonedItem = TruncateCloneModelErp(dogMng, (ModelErp)item, maxDepth, depth + 1, action: action);
                                    string? icode = item.getIcode()?.ToString(); // Recupera l'icode dell'item originale
                                    if (icode != null) cloneDict[icode] = clonedItem; // sovrascrive in caso di chiave duplicata
                                }
                                xrefFld.SetDictXrefValue(clone, cloneDict);
                            }

                        }

                    }
                }
            }

            return clone;
        }


        //*************
        // COPY & CLONE
        //*************


        //Usa un dizionario visited per prevenire cicli(molto importante con relazioni bidirezionali).
        //Copia in profondità(deepXref= true) solo se lo chiedi, altrimenti mantiene i riferimenti.
        //Gestisce:
        //Dictionary<string, string>
        //Dictionary<string, ModelErp>
        //List<ModelErp>
        //Dictionary<string, List<ModelErp>>
        //Oggetti ModelErp singoli
        //Tutto il resto viene copiato come shallow copy.
        //internal static ModelErp? CloneModelErp(DogManager dogMng, ModelErp? source, bool deepXref = false, Dictionary<ModelErp, ModelErp>? visited = null, 
        //                                        List<ModelErp>? updated = null, int depth = 0, string namePath = "")
        internal static ModelErp? CloneModelErp(DogManager dogMng, ModelErp? source, bool deepXref = false, Dictionary<ModelErp, ModelErp>? visited = null,
                                                Dictionary<ModelErp, List<string>>? updated = null, int depth = 0, string namePath = "")
        {
            visited ??= new Dictionary<ModelErp, ModelErp>();
            //---
            string indent = new string(' ', depth * 2);
            namePath = (string.IsNullOrWhiteSpace(namePath)) ? "" : $"{namePath.Trim()}.";
            Console.WriteLine($"{indent}CloneModelErp: [{source?.GetType().Name ?? "null"}] action={source?.action ?? ' '} deepXref={deepXref} namePath={namePath}");
            if (source == null) return null;    // in caso di null scrivo solo il log e ritorno null
            var typeSource = source.GetType();
            if (dogMng == null) throw new Exception($"CloneModelErp: dogMng == null.");
            if (!dogMng.tabTypes.ContainsKey(typeSource)) throw new Exception($"CloneModelErp: Classe {typeSource} non trovata.");
            DogTable tab = dogMng.tabTypes[typeSource];
            if (tab == null) throw new Exception($"CloneModelErp: tab [{typeSource}] == null.");
            //----
            if (visited.ContainsKey(source))
            {
                if (updated != null && updated.ContainsKey(source)) updated[source].Add(namePath);
                Console.WriteLine($"{indent}-> Già visitato: ritorno clone esistente");
                return visited[source]; // previene cicli
            }

            if (updated != null && source.action != null && "AMD".Contains((char)source.action))
            {
                Console.WriteLine($"{indent}-> Aggiungo a updated (TruncateClone)");
                updated.Add(TruncateCloneModelErp(dogMng, source, 1), new List<string>() { namePath }); //updated.Add(TruncateCloneModelErp(dogMng, source, 1));  //updated.Add(source.TruncateClone(1));
            }

            // Crea istanza del tipo concreto
            var clone = (ModelErp)Activator.CreateInstance(source.GetType())!;
            visited[source] = clone;

            //copia valore dei singoli campi
            foreach (var fld in tab.fields)
            {
                object? sourceValue = fld.GetValue(source); Console.WriteLine($"{indent}- Prop {fld.fieldName}: {(sourceValue == null ? "null" : sourceValue.GetType().Name)}");
                if (UtilHelper.IsNullOrEmptyObject(sourceValue))
                {
                    fld.SetValue(clone, null);
                    if (fld.optXREF) fld.SetObjValue(clone, null); ;
                    continue;   // Salta se il valore della chiave è null o stringa vuota
                }  
                //---
                fld.CopyValue(clone, sourceValue);
                if (fld.optXREF)
                {
                    ModelErp? sourceObjValue = (ModelErp?)fld.GetObjValue(source); //dump generato da CloneModelErp(..)
                    object? cloneSourceObjValue = (object?) (deepXref ? CloneModelErp(dogMng, sourceObjValue, deepXref, visited, updated, depth + 1, $"{namePath}{fld.fieldName}") : sourceObjValue);
                    fld.SetObjValue(clone, cloneSourceObjValue);
                }
            }
            //copia valore delle tabelle referenziate
            foreach (var xrefFld in tab.XrefFromFld)  //XrefFromFld: lista dei campi che referenziano questa tabella
            {
                // check if List<T>
                if (xrefFld.optXREFlist && xrefFld.fieldXrefListTyp != null)    //se è definita la proprietà List<..xrefFld.table.tableTpy..> Xref..xrefFld.fieldName.. nel modello tab.tableTpy
                {
                    //var sourceList = ((IList)source.xrefFrom[xrefFld.fieldName]).Cast<ModelErp>().ToList();
                    var sourceList = ((IList)xrefFld.GetListXrefValue(source))?.Cast<ModelErp>().ToList() ?? null; // ((IList)source.xrefFrom[xrefFld.fieldName]).Cast<ModelErp>().ToList();

                    if (sourceList == null)
                    {
                        Console.WriteLine($"{indent}  -> Clone List<{xrefFld.table.tableTpy}> {xrefFld.fieldName} is null)");
                        continue;
                    }
                    Console.WriteLine($"{indent}  -> Clone List<{xrefFld.table.tableTpy}> {xrefFld.fieldName} ({sourceList.Count} items)");

                    var cloneList = (IList)Activator.CreateInstance(xrefFld.fieldXrefListTyp);
                    ////////-----------------
                    //////// inserisco primo record vuoto per fare Add su tabella in grafica cshtml
                    //////var cloneElem = Activator.CreateInstance(xrefFld.table.tableTpy);
                    //////((ModelErp)cloneElem).action = 'D';
                    //////cloneList.Add(cloneElem);
                    ////////-----------------
                    int nRow = 0;
                    foreach (var item in sourceList)
                    {
                        var clonedItem = CloneModelErp(dogMng, item, deepXref, visited, updated, depth + 1, $"{namePath}Xref{xrefFld.fieldName}[{nRow++}]");
                        cloneList.Add(clonedItem);
                    }
                    xrefFld.SetListXrefValue(clone, cloneList); //var prop = clone.GetType().GetProperty("Xref" + xrefFld.fieldName); //prop.SetValue(clone, cloneList);
                }
                // check if Dictionary<string,T>
                if (xrefFld.optXREFdict && xrefFld.fieldXrefDictTyp != null)    //se è definita la proprietà Dictionary<string,..xrefFld.table.tableTpy..> Xref..xrefFld.fieldName.. nel modello tab.tableTpy
                {
                    System.Collections.IDictionary sourceDict = (System.Collections.IDictionary)xrefFld.GetDictXrefValue(source);
                    //var sourceList = ((IList)xrefFld.GetListXrefValue(source))?.Cast<ModelErp>().ToList() ?? null;
                    if (sourceDict == null)
                    {
                        Console.WriteLine($"{indent}  -> Clone Dictionary<string,{xrefFld.table.tableTpy}> {xrefFld.fieldName} is null)");
                        continue;
                    }
                    Console.WriteLine($"{indent}  -> Clone Dictionary<string,{xrefFld.table.tableTpy}> {xrefFld.fieldName} ({sourceDict.Count} items)");
                    //-----
                    var cloneDict = (System.Collections.IDictionary)Activator.CreateInstance(xrefFld.fieldXrefDictTyp);
                    foreach (DictionaryEntry kvp in sourceDict)
                    {
                        var key = (string)kvp.Key;
                        if (string.IsNullOrEmpty(key)) continue; // Salta se la chiave è null o stringa vuota
                        var item = (ModelErp)kvp.Value;
                        var clonedItem = CloneModelErp(dogMng, item, deepXref, visited, updated, depth + 1, $"{namePath}Xref{xrefFld.fieldName}[{key}]");
                        string? icode = (item != null) ? item?.getIcode()?.ToString() : key; // Recupera l'icode dell'item originale
                        if (icode != null) cloneDict[icode] = clonedItem; // sovrascrive in caso di chiave duplicata
                    }
                    xrefFld.SetDictXrefValue(clone, cloneDict);
                }
            }
            //copia dizionario interno xrefFrom
            if (source.xrefFrom != null)
            {
                clone.xrefFrom = new Dictionary<string, List<ModelErp>>();
                foreach (var kvp in source.xrefFrom)
                {
                    var listType = kvp.Value.GetType(); // es. List<MyDerivedModelErp>
                    var clonedList = (IList)Activator.CreateInstance(listType)!;

                    int nRow = 0;
                    foreach (var item in kvp.Value)
                    {
                        var clonedItem = deepXref ? CloneModelErp(dogMng, item, deepXref, visited, updated, depth + 1, $"{namePath}xrefFrom.{kvp.Key}[{nRow++}]") : item;
                        clonedList.Add(clonedItem);
                    }

                    clone.xrefFrom[kvp.Key] = clonedList.Cast<ModelErp>().ToList();
                }
            }
            // Copia anche i campi interni
            if (!deepXref)
            {
                DogCache? sourceDogCache = (DogCache)source.GetType()?.BaseType?.GetField("_dogCache", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(source) ?? null;
                if (sourceDogCache != null) clone.addDogCache(ref sourceDogCache);
                clone.depth = source.depth;
            }
            clone.jsonOriginal = source.jsonOriginal;
            clone.action = source.action;
            clone.options = source.options;
            if(source.vars != null) clone.vars = new Dictionary<string, string>(source.vars);

            return clone;
        }



    }
}
