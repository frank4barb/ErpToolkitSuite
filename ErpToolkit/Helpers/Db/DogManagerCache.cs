using System.Data;
using System.Reflection;
using System.Collections;
using static ErpToolkit.Helpers.Db.DogManager;
using ErpToolkit.Models;
using System.Text;
using Microsoft.Extensions.Options;
using K4os.Hash.xxHash;
using System.Data.Odbc;

namespace ErpToolkit.Helpers.Db
{
    public static class DogManagerCache
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerCache()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************

        //genera la query SQL per l'estrazione di una lista di oggetti, in base al parametro "objModel.ViewQueryFromWhere()"
        internal static string sqlListEx(DogManager dogMng, DogTable tab, ref IDictionary<string, object> parameters, ModelErp selModel, DogField fldXref, List<object> lstRowId, List<string> lstFmt, bool isXdata, string options = "")
        {
            string sql = "";
            if (selModel == null && lstRowId == null) { throw new ArgumentNullException(nameof(selModel) + " - " + nameof(lstRowId)); }
            if (isXdata) {
                if (tab.tabXdata == null) throw new Exception($"sqlListEx: tab [{tab.tableTpy.FullName}] Xdata == null.");
                if (selModel != null) { throw new ArgumentException("sqlListEx must be null when isXdata is true."); }
            }
            ModelErp objModel = (ModelErp)Activator.CreateInstance(tab.tableTpy); // create an instance of that type

            StringBuilder sb = new StringBuilder();
            string sqlSelect = (isXdata) ? DogManagerQuery.sqlSelectXdataEx(tab.tabXdata, ref parameters) : DogManagerQuery.sqlSelectEx(tab, ref parameters);
            string sqlFromWhere = (isXdata) ? objModel.ViewQueryXdataFromWhere() : objModel.ViewQueryFromWhere();
            if (string.IsNullOrEmpty(sqlFromWhere))
            {
                sb.Append(sqlSelect)
                    .Append(DogManagerQuery.sqlFromEx((isXdata) ? tab.tabXdata : tab, ref parameters));
                if (selModel == null && fldXref == null) sb.Append(DogManagerQuery.sqlWhereListIcodeEx(tab, lstRowId, isXdata, ref parameters, options: options));  //lista icode
                else if (selModel == null) sb.Append(DogManagerQuery.sqlWhereListXrefEx(tab, fldXref, lstRowId, lstFmt, isXdata, ref parameters, options: options));  //lista icode
                else sb.Append(DogManagerQuery.sqlWhereSelection(dogMng, selModel, ref parameters, options: options));  //filtro parametri
                sql = sb.ToString();
            }
            else
            {
                sb.Append("SELECT * FROM ( \n")
                    .Append(sqlSelect)
                    .Append(sqlFromWhere)
                    .Append(") AS subquery \n");
                if (selModel == null && fldXref == null) sb.Append(DogManagerQuery.sqlWhereListIcodeEx(tab, lstRowId, isXdata, ref parameters, options: "[UsePropertyNameField] " + options));  //lista icode
                if (selModel == null) sb.Append(DogManagerQuery.sqlWhereListXrefEx(tab, fldXref, lstRowId, lstFmt, isXdata, ref parameters, options: "[UsePropertyNameField] " + options));  //lista icode
                else sb.Append(DogManagerQuery.sqlWhereSelection(dogMng, selModel, ref parameters, options: "[UsePropertyNameField] " + options)); // Componi il filtro dinamicamente
                sql = DogManagerQuery.replaceSqlTextWithPlaceholders(sb.ToString(), ref parameters);  // elimino le stringhe esplicite dalla query
            }
            return sql;
        }


        ////***************************************************************************************************************************************************
        ////*** Gestione CACHE
        ////***************************************************************************************************************************************************

        // Negli accessi ad DB è previsto l'uso di una cache interna di tipo: Dictionary<System.Type, Dictionary<object, ModelErp>> dbCache
        // dove: System.Type è il tipo di struttura del modello, object è la chiave univoca del record, e ModelErp è l'istanza del modello con i contenuti dl DB
        //
        // (1) ad ogni query i contenuti della dbCache vengono integrati/aggiornati (se esiste aggiorno, altrimenti aggiungo)
        // (2) per il calcolo dei riferimenti esterni (campi XrefObj) accedo al DB solo se l'informazione non è già contenuta nella dbCache
        // (3) ad ogni accesso al DB integro la dbCache con i contenuti estratti

        //Inizializza la Cache per un nuovo servizio DogManager (eg: List, Mnt, ecc.)
        internal static void CacheFuncInit(DogManager dogMng, ref DogCache dogCache, string serviceName, char serviceAction, System.Type serviceTabType, string options = "")
        {
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            // Inizializza la cache per il nuovo Servizio
            dogCache.ServiceName = serviceName;  // Nome del servizio (es: "List", "Mnt", ecc.)
            dogCache.ServiceAction = serviceAction;  // Azione del servizio (es: 'L' per List, 'M' per Mnt, ecc.)
            dogCache.ServiceTabType = serviceTabType;  // Tipo della tabella del servizio (es: typeof(T) per List<T>, ecc.)
            dogCache.InitReadID();  // Inizializza l'ID della lista per il servizio
            dogCache.InitMntID();  // Inizializza l'ID di manutenzione per il servizio

        }

        // Integra in Cache il dizionario "outDict". return: la lista "List<T>" del dizionario inserito/aggiornato 
        internal static List<object> CacheAddDict(DogManager dogMng, ref DogCache dogCache, System.Type objType, Dictionary<object, ModelErp> outDict, string options = "")
        {
            // Recupera il dizionario finale esistente o inizializza uno nuovo
            if (!dogCache.dbCache.TryGetValue(objType, out var dictFinale)) { dictFinale = new Dictionary<object, ModelErp>(); }

            //Hai Un dizionario esistente (Dictionary<object, ModelErp> dictFinale) e Un dizionario sorgente (Dictionary<object, List<ModelErp>> dizionario)
            //Vuoi usare le chiavi di dizionario come riferimento: (1) Se la chiave esiste in dizionarioFinale, aggiorni il valore (2) Se non esiste, la aggiungi

            // Unisci i dati: aggiorna o aggiungi
            foreach (var kv in outDict) { if (UtilHelper.IsNullOrEmptyObject(kv.Key) == false && kv.Value != null) { kv.Value.addDogCache(ref dogCache); dictFinale[kv.Key] = kv.Value; } }

            // Salva nella cache
            dogCache.dbCache[objType] = dictFinale;

            // Inserisci nella cache i riferimenti agli oggetti referenziati (XrefObj) se non sono già presenti
            // Integra in Cache l'elenco degli oggetti icodeXref referenziati nella lista "outList". Ad ogni Chiave Icode viene associato un Valore null. 
            if (options.Contains("[PLAIN]") == false)  //if (options.Contains("[DecodeLabels]"))
            {
                //Hai Un dizionario esistente (Dictionary<object, ModelErp> dictFinale) e Un dizionario sorgente (Dictionary<object, List<ModelErp>> dizionario)
                //Vuoi usare le chiavi di dizionario come riferimento: (1) Se la chiave esiste in dizionarioFinale, salti (2) Se non esiste, la aggiungi la chiave Icode con un valore null (questo sarà valorizzato successivamente)

                // Unisci i dati: aggiungi solo se non esiste, per tutti i riferimenti a campi Xref presenti nel modello
                if (dogMng.tabTypes.ContainsKey(objType))
                {
                    DogTable tab = dogMng.tabTypes[objType];

                    //carica gli Icode degli oggetti referenziati
                    foreach (var fld in tab.fields)
                    {
                        try
                        {
                            var xrefObj = fld?.XrefObj;
                            if (xrefObj == null) continue; //per applicare la condizione la proprietà deve avere un attributo [ErpDogField(..)]
                            string propertyName = fld.fieldName; // Get property name and value
                            System.Type xrefObjType = fld.XrefObj.table.tableTpy;

                            foreach (var el in outDict.Values)
                            {
                                if (el == null) continue; // Salta l'elemento della lista è null
                                object? icodeXref = objType.GetProperty(propertyName).GetValue(el);
                                if (UtilHelper.IsNullOrEmptyObject(icodeXref) == true) continue; // Salta se il valore della chiave è null
                                if (!dogCache.dbCache.ContainsKey(xrefObjType)) { dogCache.dbCache.Add(xrefObjType, new Dictionary<object, ModelErp>()); }
                                if (dogCache.dbCache[xrefObjType].ContainsKey(icodeXref)) continue; // Salta se la chiave è già presente nel dizionario
                                dogCache.dbCache[xrefObjType].Add(icodeXref, null); // Aggiungi la chiave con valore null
                            }
                        }
                        catch (Exception ex) { }  //skip exceptions
                    }
                }
            }

            // Ritorna la lista finale delle chiavi dei soli valori estratti
            return outDict.Keys.ToList<object>(); //return dictFinale.Keys.ToList<object>(); 
        }

        // Integra in Cache tutti i riferimenti a Chiave Icode con Valore null.
        // A fine processo effettua l'abbinamento dei riferimenti Xref per tutti i record presenti nella Cache
        internal static List<T> CacheFillNull<T>(DogManager dogMng, ref DogCache dogCache, List<object> mainObjKeyList, bool fillXdata, List<string> fmtList, string? transactionId, int maxRecords, string options = "") where T : ModelErp
        {
            T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
            return CacheFillNull(dogMng, ref dogCache, objModel.GetType(), mainObjKeyList, fillXdata, fmtList, transactionId, maxRecords, options: options).OfType<T>().ToList(); //  OfType<T>() : filtra e fa cast solo se possibile (cioè solo se tipo T, atrimenti scarta la struttura);
        }
        internal static List<ModelErp> CacheFillNull(DogManager dogMng, ref DogCache dogCache, System.Type mainObjType, List<object> mainObjKeyList, bool fillXdata, List<string> fmtList, string? transactionId, int maxRecords, string options = "")
        {
            int recursiveCicle = 0; bool mustAddRecursiveObj = false;
            do
            {
                //----------------------------------------------------------
                recursiveCicle++; mustAddRecursiveObj = false;
                if (recursiveCicle > 100) { throw new IndexOutOfRangeException(nameof(recursiveCicle)); }
                // ----------------------------------------------------------
                //riempi i valori degli oggetti con Valore null presenti in Cache 
                foreach (var objType in dogCache.dbCache.Keys)
                {
                    DogTable tab = dogMng._getDogTableException(objType, "CacheFillNull");  // verifico che esista la tabella per quel tipo di oggetto, altrimenti è un errore di configurazione grave e fermo tutto con un'eccezione
                    //ModelErp obj = (ModelErp)Activator.CreateInstance(objType); // create an instance of that type
                    IDictionary<string, object> objParameters = new Dictionary<string, object>();
                    List<object> nullKeyList = dogCache.dbCache[objType].Where(kvp => kvp.Value == null).Select(kvp => kvp.Key).ToList<object>(); // lista delle chiavi con valore null
                    if (nullKeyList.Count() == 0) continue; // Se non ci sono chiavi con valore null, salto il ciclo


                    //string objSql = sqlList(dogMng, obj, ref objParameters, null, null, nullKeyList, options: options);
                    string objSql = sqlListEx(dogMng, tab, ref objParameters, null, null, nullKeyList, null, false, options: options);

                    //dogCache.dbCache[objType] = this.ExecuteQuery(dogCache.dbCache[objType], objType, objSql, objParameters, "[PLAIN] " + options); // non ricorsivo ?????
                    //Dictionary<object, ModelErp> outDict = this.ExecuteQuery(dogCache.dbCache[objType], objType, objSql, objParameters, options);


                    //!!!//Dictionary<object, ModelErp> outDict = dogMng.ExecuteQuery(null, objType, objSql, objParameters, transactionId, maxRecords, options: options);
                    Dictionary<object, ModelErp> outDict = dogMng.ExecuteQueryEx(null, objType, objSql, objParameters, fillXdata, fmtList, transactionId, maxRecords, options: options);
                    //!!!//

                    foreach (var kv in outDict) { if (UtilHelper.IsNullOrEmptyObject(kv.Key) == false && kv.Value != null) { kv.Value.addDogCache(ref dogCache); dogCache.dbCache[objType][kv.Key] = kv.Value; } } //aggiorna o aggiungi alla cache
                }
                // ----------------------------------------------------------
                //riassegno a tutti i record della Cache il riferimento agli oggetti referenziati (ie: per tutte le tabelle presenti nella cache
                Dictionary<System.Type, Dictionary<object, ModelErp>> appIcodeList = new Dictionary<System.Type, Dictionary<object, ModelErp>>();
                foreach (var objType in dogCache.dbCache.Keys)
                {
                    if (dogMng.tabTypes.ContainsKey(objType))
                    {
                        List<ModelErp> outList = dogCache.dbCache[objType].Values.OfType<ModelErp>().ToList(); // prendo la lista dei record della tabella corrente
                        DogTable tab = dogMng.tabTypes[objType];
                        foreach (var fld in tab.fields)
                        {
                            try
                            {
                                var xrefObj = fld?.XrefObj;
                                if (xrefObj == null) continue; //per applicare la condizione la proprietà deve avere un attributo [ErpDogField(..)]
                                string propertyName = fld.fieldName; // Get property name and value
                                System.Type xrefObjType = fld.XrefObj.table.tableTpy;

                                if (!dogCache.dbCache.ContainsKey(xrefObjType) && options.Contains("[RECURSIVE]") == false) continue; // Se non è presente la tabella collegata, salto il ciclo (non aggiorno i riferimenti)

                                foreach (ModelErp rec in outList) // Assegno il record della tabella collegata ad ogni riga della tabella principale (campo + "Obj")
                                {
                                    try
                                    {
                                        if (rec == null) continue; // Salta l'elemento della lista se inserito come null
                                        object? icode = rec.GetType().GetProperty(propertyName).GetValue(rec); // Get the value of the property
                                        if (UtilHelper.IsNullOrEmptyObject(icode)) continue; // Salta se il valore della chiave è null o stringa vuota

                                        //verifico se devo aggiungere l'oggetto ricorsivo alla cache
                                        if (options.Contains("[PLAIN]") == false && options.Contains("[RECURSIVE]") == true)
                                        {
                                            if (!dogCache.dbCache.ContainsKey(xrefObjType) || !dogCache.dbCache[xrefObjType].ContainsKey(icode))
                                            {
                                                // devo aggiungere l'oggetto ricorsivo alla cache se non già presente
                                                if (!appIcodeList.ContainsKey(xrefObjType)) appIcodeList[xrefObjType] = new Dictionary<object, ModelErp>();
                                                if (!appIcodeList[xrefObjType].ContainsKey(icode))
                                                {
                                                    mustAddRecursiveObj = true; // devo aggiungere l'oggetto ricorsivo alla cache
                                                    appIcodeList[xrefObjType].Add(icode, null); // Aggiungo la chiave con valore null
                                                }

                                            }
                                            if (mustAddRecursiveObj) continue; // Se devo aggiungere l'oggetto ricorsivo, salto il ciclo (non aggiorno i riferimenti)
                                        }

                                        // Aggiorno riferimenti
                                        rec.GetType().GetProperty(propertyName + "Obj").SetValue(rec, dogCache.dbCache[xrefObjType][icode]);



                                        //!!! Inserisce vars FieldLabel per il campo !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                                        string label = dogCache.dbCache[xrefObjType][icode].ToHtml();
                                        rec.vars[$"@{propertyName}-FieldLabel"] = label;
                                        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                                    }
                                    catch (Exception ex) { }  //skip exceptions (salta se la chiave non è valorizzata 
                                }
                            }
                            catch (Exception ex) { }  //skip exceptions
                        }
                    }
                }
                // Aggiungo le nuove chiavi da cercare, con il valore null
                if (mustAddRecursiveObj)
                {
                    foreach (var xrefObjType in appIcodeList.Keys)
                    {
                        if (!dogCache.dbCache.ContainsKey(xrefObjType)) dogCache.dbCache[xrefObjType] = new Dictionary<object, ModelErp>();
                        foreach (var icode in appIcodeList[xrefObjType].Keys)
                        {
                            if (!dogCache.dbCache[xrefObjType].ContainsKey(icode)) dogCache.dbCache[xrefObjType].Add(icode, null); // Aggiungo la chiave con valore null
                        }
                    }
                }
                //----------------------------------------------------------
            } while (mustAddRecursiveObj); // Ciclo per aggiungere oggetti ricorsivi se necessario

            // ----------------------------------------------------------
            // ricostruisco tutti i riferimenti all'oggetto referenziati nelle tabelle esterne (per le regole impostate in ruleXrefFrom)
            if (dogCache.RuleXrefFrom.Count > 0)
            {
                foreach (var xrefFromPropertyName in dogCache.RuleXrefFrom)
                {
                    DogField fld = dogMng.tabProperties[xrefFromPropertyName];
                    System.Type objModelType = fld?.XrefObj?.table?.tableTpy; // modello tabella da aggiornare  
                    if (objModelType == null) continue;
                    if (!dogCache.dbCache.ContainsKey(objModelType)) continue;
                    System.Type xrefFromType = fld?.table?.tableTpy;  // modello tabella referenziata
                    if (xrefFromType == null) continue;
                    if (!dogCache.dbCache.ContainsKey(xrefFromType)) continue;
                    //carico i riferimenti per ogni record della lista
                    Dictionary<object, ModelErp> outDictFrom = dogCache.dbCache[xrefFromType];
                    foreach (var key in dogCache.dbCache[objModelType].Keys)
                    {
                        var el = dogCache.dbCache[objModelType][key]; if (el == null) continue;
                        object icode = el.getIcode(); if (icode == null) continue;
                        el.xrefFrom[xrefFromPropertyName] = outDictFrom.Values.ToList<ModelErp>().Where(item => item != null &&                             // esclude null
                                                                            item.GetType() == xrefFromType &&                                               // filtro sul tipo esatto
                                                                            xrefFromType.GetProperty(xrefFromPropertyName) != null &&                       // controlla che la proprietà esista
                                                                            Equals(xrefFromType.GetProperty(xrefFromPropertyName).GetValue(item), icode))   // confronta valori
                                                                        .ToList<ModelErp>();
                    }
                }
            }
            // ----------------------------------------------------------
            // ricostruisco tutti i riferimenti ai dati estesi ....to do....
            //...
            //...
            //...
            // ----------------------------------------------------------
            // Ritorna la lista finale dei soli valori estratti
            if (mainObjType == null || mainObjKeyList == null) return null;
            //if (typeof(ModelErp).IsAssignableFrom(mainObjType) == false) return null; //check parametri //????? non è necessario vedere se applicare questa condizione

            Dictionary<object, ModelErp> mainObjDict = dogCache.dbCache[mainObjType];
            return mainObjKeyList.Select(k => mainObjDict[k]).ToList();     // restituisce la lista di valori T corrispondenti a una lista di chiavi List<object> da un Dictionary<object, T> 
                                                                            // se anche una sola chiave non è presente, otterrai una KeyNotFoundException.
        }


        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        ////////// ----------------------------------------------------------
        ////////// ricostruisco tutti i riferimenti ai dati estesi ....to do....
        //////////...
        //////////...
        //////////...
        ////////// ----------------------------------------------------------
        ////////// Ritorna la lista finale dei soli valori estratti
        ////////if (mainObjType == null || mainObjKeyList == null) return null;
        //////////if (typeof(ModelErp).IsAssignableFrom(mainObjType) == false) return null; //check parametri //????? non è necessario vedere se applicare questa condizione

        ////////Dictionary<object, ModelErp> mainObjDict = dogCache.dbCache[mainObjType];
        //////////return mainObjKeyList.Select(k => mainObjDict[k]).ToList();     // restituisce la lista di valori T corrispondenti a una lista di chiavi List<object> da un Dictionary<object, T> 
        //////////                                                                // se anche una sola chiave non è presente, otterrai una KeyNotFoundException.


        //////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //////////!!!!!!! RESTITUISCO UN CLONE DELL'OGGETTO ModelErp TRONCATO AL SECONDO LIVELLO (depth=2)                    !!!!!!
        //////////!!!!!!! in questo modo eventuali modifiche effettuate sulla lista restituita non hanno impatto sulla CACHE  !!!!!!
        //////////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        //////////return mainObjKeyList.Select(k => mainObjDict[k].TruncateClone(DogManager.DOG_MAX_OBJ_DEPTH, action: 'R')).ToList();     // restituisce un clone troncato la lista di valori T corrispondenti a una lista di chiavi List<object> da un Dictionary<object, T> 
        //////////                                                                                                                         // se anche una sola chiave non è presente, otterrai una KeyNotFoundException.



        ////////return mainObjKeyList.Select(k => DogManagerClone.Test_TruncateCloneModelErp(dogMng, mainObjDict[k], DogManager.DOG_MAX_OBJ_DEPTH, action: 'R')).ToList();
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!








    }
}
