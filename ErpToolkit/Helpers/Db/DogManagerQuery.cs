using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Text;
using System.Collections;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.Db.DogManager;
using ErpToolkit.Models;
using System.Threading.Tasks.Dataflow;


namespace ErpToolkit.Helpers.Db
{
    public static class DogManagerQuery
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerQuery()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************


        private const bool IS_NULLABLE_ID = false; //=true se posso inserire NULL in tutti gli identifificatori univoci (serve per velocizzare gli indici)
        private const bool IS_NULLABLE_NUM = false; //=true se posso inserire NULL sui valori numerici
        private const bool IS_NULLABLE_INDEX = false; //=true se posso definire indici univoci con campi NULL


        //==========================================================================================================
        //==========================================================================================================

        // AUTOCOMPLETE
        //---------------

        //xxx//internal static List<Choice> AutocompleteGetAll<T>(DogManager dogMng, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new() { return Autocomplete_Int<T>(dogMng, "GetAll", extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords); }
        //xxx//internal static List<Choice> AutocompleteGetSelect<T>(DogManager dogMng, string term, bool caseInsensitive = true, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new() { return Autocomplete_Int<T>(dogMng, "GetSelect", term: term, caseInsensitive: caseInsensitive, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords); }
        //xxx//internal static List<Choice> AutocompletePreLoad<T>(DogManager dogMng, List<string> values, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new() { return Autocomplete_Int<T>(dogMng, "PreLoad", values: values, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords); }
        //xxx//internal static List<Choice> Autocomplete_Int<T>(DogManager dogMng, DogTable tab, string tpy, string? term = null, bool caseInsensitive = true, List<string>? values = null, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new()
        internal static List<Choice> Autocomplete_Int(DogManager dogMng, DogTable tab, string tpy, string? term = null, bool caseInsensitive = true, List<string>? values = null, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) 
        {
            //check init
            if (tpy == null) throw new Exception($"Autocomplete_Int: tpy == null.");
            //T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
            //if (objModel == null) throw new Exception($"Autocomplete_Int: objModel [{typeof(T)}] == null.");
            ////////////////////////////T objModel = new T(); // create an instance of that type
            ////////////////////////////string labelHtml = objModel.LabelHtml;  // = objModel.LABEL_HTML;
            ////////////////////////////if (String.IsNullOrWhiteSpace(labelHtml)) throw new Exception($"Autocomplete_Int: labelHtml [{typeof(T)}]  vuota.");
            if (dogMng == null) throw new Exception($"Autocomplete_Int: dogMng == null.");

            //xxx//if (!dogMng.tabTypes.ContainsKey(typeof(T))) throw new Exception($"Autocomplete_Int: Classe {typeof(T)} non trovata.");
            //xxx//DogTable tab = dogMng.tabTypes[typeof(T)];
            
            if (tab == null) throw new Exception($"Autocomplete_Int: tab == null.");
            if (tab.fldIcode == null) throw new Exception($"Autocomplete_Int: tab.fldIcode [{tab.tableTpy.FullName}] == null.");

            ////////////////////////////// Trova tutte le occorrenze di {NomeVariabile}
            ////////////////////////////var matches = Regex.Matches(labelHtml, @"\{([^\}]+)\}");
            ////////////////////////////List<string> variabili = new List<string>();
            ////////////////////////////foreach (Match match in matches)
            ////////////////////////////{
            ////////////////////////////    // match.Groups[1] contiene il nome della variabile senza le graffe
            ////////////////////////////    string varInterpolata = match.Groups[1].Value;
            ////////////////////////////    // Sostituisce tutti i caratteri NON validi con '+'
            ////////////////////////////    // I validi sono: lettere, cifre, underscore (_)
            ////////////////////////////    // Il simbolo @ è considerato non valido in questo contesto
            ////////////////////////////    variabili.Add(Regex.Replace(varInterpolata, @"[^a-zA-Z0-9_]", "+"));
            ////////////////////////////}
            ////////////////////////////string varsHtml = string.Join("+", variabili);

            // Estraggo la lista delle proprietà della classe T presenti nelle variabili interpolate ie: {NomeVariabile}
            var fieldNames = new HashSet<string>(StringComparer.Ordinal);
            //////////////////////////foreach (var fld in tab.fields) if (labelHtml.Contains(fld.fieldName)) fieldNames.Add(fld.fieldName);
            foreach (var fld in tab.fields) { if (fld.optLABEL) fieldNames.Add(fld.fieldName); }


            // Costruisci la SELECT solo con i campi rilevati
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            StringBuilder sb = new StringBuilder($@"SELECT ");
            sb.Append(string.Join(", ", fieldNames.Select(f => $@"{dogMng.tabProperties[f].SqlFieldName} as {f}")));
            if (!fieldNames.Contains(tab.fldIcode.fieldName))
            {
                if (fieldNames.Count() > 0) sb.Append($@",");  
                sb.Append($@" {tab.fldIcode.SqlFieldName} as {tab.fldIcode.fieldName} ");  // devo aggiungere anche icode, se non presente
            }
            if (tab.fldDeleted == null) { sb.Append($@" FROM {tab.SqlTableName} WHERE 1=1 "); }
            else { sb.Append($@" FROM {tab.SqlTableName} WHERE {tab.fldDeleted.SqlFieldName} = {DogManager.addParam("N", ref parameters)} "); }

            if (tpy == "GetAll")
            {
                ;  // do nothing
            }
            else if (tpy == "GetSelect")
            {
                if (term == null) throw new Exception($"Autocomplete_Int: term == null.");
                //filtro per term
                if (fieldNames.Count() > 0)
                {
                    //string searchFields = string.Join(" + ", fieldNames.Select(f => $@"{dogMng.tabProperties[f].SqlFieldName} + {DogManager.addParam(" - ", ref parameters)}"));
                    //if (caseInsensitive) sb.Append($@" AND upper({searchFields}) LIKE {DogManager.addParam("%" + term.ToUpper() + "%", ref parameters)}");
                    //else sb.Append($@" AND {searchFields} LIKE {DogManager.addParam("%" + term + "%", ref parameters)}");



                    caseInsensitive = false; // forzo case sensitive per migliorare performance su DBMS che non indicizzano ricerche case insensitive



                    bool serchInMiddle = false;
                    string startWith = "";
                    if (serchInMiddle) startWith = "%";
                    if (caseInsensitive)
                    {
                        string searchUpperFields = string.Join(" OR ", fieldNames.Select(f => $@"upper({dogMng.tabProperties[f].SqlFieldName}) LIKE {DogManager.addParam(startWith + term.ToUpper() + "%", ref parameters)} "));
                        sb.Append($@" AND ({searchUpperFields})");
                    }
                    else
                    {
                        string searchFields = string.Join(" OR ", fieldNames.Select(f => $@"{dogMng.tabProperties[f].SqlFieldName} LIKE {DogManager.addParam(startWith + term + "%", ref parameters)} "));
                        sb.Append($@" AND ({searchFields})");
                    }


                }
            }
            else if (tpy == "PreLoad")
            {
                if (values == null) throw new Exception($"Autocomplete_Int: values == null.");
                List<string> filteredValues = values.Where(v => v != null).ToList(); // Filtra i valori null
                if (filteredValues.Count() > 0)
                {
                    //filtro per list values
                    string searchCodes = string.Join(",", DogManager.addListParam(filteredValues.ToList<object>(), ref parameters));
                    sb.Append($@" AND {tab.fldIcode.SqlFieldName} in ({searchCodes})");
                }
                else
                {
                    // Nessun valore valido, quindi nessun risultato
                    sb.Append($@" AND {DogManager.addParam("1", ref parameters)}={DogManager.addParam("0", ref parameters)}"); // Condizione sempre falsa
                }
            }
            else
            {
                throw new Exception($"Autocomplete_Int: wrong tpy {tpy}.");
            }
            //filtro per extra Where
            if (!String.IsNullOrWhiteSpace(extraWhere)) sb.Append($@" AND {extraWhere}");

            // Esegui la query
            //xxx//var listModel = dogMng.ExecuteQuery<T>(sb.ToString(), parameters, transactionId, maxRecords);
            Dictionary<object, ModelErp> listModel = dogMng.ExecuteQuery(null, tab.tableTpy, sb.ToString(), parameters, transactionId, maxRecords);

            // Applica la formattazione dinamica
            //xxx//var result = listModel.Select(p => { return new Choice { value = p.getIcode().ToString(), label = p.ToHtml() }; }).ToList<Choice>();
            var result = listModel.Select(p => { return new Choice { value = p.Value?.getIcode()?.ToString() ?? "", label = p.Value?.ToHtml() ?? "" }; }).ToList<Choice>();
            return result;
        }

        //private static List<Choice> Autocomplete_Int<T>(DogManager dogMng, string tpy, string? term = null, bool caseInsensitive = true, List<string>? values = null, string? extraWhere = null) where T : ModelErp
        //{
        //    if (tpy == null) throw new Exception($"Autocomplete_Int: tpy == null.");

        //    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        //    string labelHtml = objModel.LabelHtml;  // = objModel.LABEL_HTML;

        //    if (!dogMng.tabTypes.ContainsKey(typeof(T))) throw new Exception($"Classe {typeof(T)} non trovata.");
        //    DogTable tab = dogMng.tabTypes[typeof(T)];
        //    string[] knownFields = tab.fields.Select(d => d.fieldName).ToArray();  //array alle proprietà della classe T

        //    // Estrai il contenuto tra parentesi graffe
        //    var matches = Regex.Matches(labelHtml, @"\{(.*?)\}");
        //    var fieldNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        //    foreach (Match match in matches)
        //    {
        //        string inside = match.Groups[1].Value; // es: "HttpUtility.HtmlEncode(Pr1Icode)"
        //        foreach (string field in knownFields)
        //        {
        //            // cerca il nome del campo come parola intera nel contenuto
        //            if (Regex.IsMatch(inside, $@"\b{Regex.Escape(field)}\b"))
        //                fieldNames.Add(field);
        //        }
        //    }
        //    if (fieldNames.Count == 0) throw new Exception($"Autocomplete_Int: Nessun campo valido trovato nel formato.");

        //    // Costruisci la SELECT solo con i campi rilevati
        //    IDictionary<string, object> parameters = new Dictionary<string, object>();

        //    StringBuilder sb = new StringBuilder($@"SELECT ");
        //    if (!fieldNames.Contains(tab.fldIcode.fieldName)) sb.Append($@"{tab.fldIcode.SqlFieldName} as {tab.fldIcode.fieldName}, ");  // devo aggiungere anche icode, se non presente
        //    string selectFields = string.Join(", ", fieldNames.Select(f => $@"{dogMng.tabProperties[f].SqlFieldName} as {f}"));
        //    sb.Append(selectFields).Append($@" FROM {tab.SqlTableName} WHERE {tab.fldDeleted.SqlFieldName} = {DogManager.addParam("N", ref parameters)} ");

        //    if (tpy == "GetAll")
        //    {
        //        ;  // do nothing
        //    }
        //    else if (tpy == "GetSelect")
        //    {
        //        if (term == null) throw new Exception($"Autocomplete_Int: term == null.");
        //        //filtro per term
        //        string searchFields = string.Join(" + ", fieldNames.Select(f => $@"{dogMng.tabProperties[f].SqlFieldName} + {DogManager.addParam(" - ", ref parameters)}"));
        //        if (caseInsensitive) sb.Append($@" AND upper({searchFields}) LIKE {DogManager.addParam("%" + term.ToUpper() + "%", ref parameters)}");
        //        else sb.Append($@" AND searchFields LIKE {DogManager.addParam("%" + term + "%", ref parameters)}");
        //    }
        //    else if (tpy == "PreLoad")
        //    {
        //        if (values == null) throw new Exception($"Autocomplete_Int: values == null.");
        //        if (values.Count() > 0)
        //        {
        //            //filtro per list values
        //            string searchCodes = string.Join(", ", DogManager.addListParam(values.ToList<object>(), ref parameters));
        //            sb.Append($@" AND {tab.fldIcode.SqlFieldName} in ({DogManager.addParam("%" + term + "%", ref parameters)})");
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception($"Autocomplete_Int: wrong tpy {tpy}.");
        //    }
        //    //filtro per extra Where
        //    if (!String.IsNullOrWhiteSpace(extraWhere)) sb.Append($@" AND {extraWhere}");

        //    // Esegui la query
        //    var listModel = dogMng.ExecuteQuery<T>(sb.ToString(), parameters);

        //    // Applica la formattazione dinamica
        //    var result = listModel.Select(p => { return new Choice { value = p.getIcode().ToString(), label = p.ToHtml() }; }).ToList<Choice>();
        //    return result;
        //}


        //******************************************************************************************************************
        //******************************************************************************************************************


        //==========================================================================================================
        //==========================================================================================================

        // SQL SELECT
        //---------------

        ////crea SELECT per l'oggetto del modello 'objModel'
        //internal static string sqlSelect(DogManager dogMng, ModelErp objModel, ref IDictionary<string, object> parameters)
        //{
        //    StringBuilder sb = new StringBuilder("select ");
        //    if (objModel == null) { throw new ArgumentNullException(nameof(objModel)); }
        //    if (dogMng == null) throw new Exception($"sqlSelect: dogMng == null.");
        //    if (!dogMng.tabTypes.ContainsKey(objModel.GetType())) throw new Exception($"sqlSelect: Classe {objModel.GetType().FullName} non trovata.");
        //    DogTable tab = dogMng.tabTypes[objModel.GetType()];
        //    if (tab == null) throw new Exception($"sqlSelect: tab [{objModel.GetType().FullName}] == null.");
        //    //ciclo sui campi della tabella
        //    foreach (var fld in tab.fields )
        //    {
        //        string propertyName = fld.fieldName;
        //        var sqlFieldNameExt = fld.SqlFieldName?.Trim() ?? "";
        //        if (sqlFieldNameExt != "") { sb.AppendLine($" {sqlFieldNameExt} as {propertyName},"); }
        //    }
        //    // terminatore di select
        //    sb.AppendLine($" 0 as ErpTerm ");
        //    return sb.ToString();
        //}
        ////crea FROM per l'oggetto del modello 'objModel'
        //internal static string sqlFrom(DogManager dogMng, ModelErp objModel, ref IDictionary<string, object> parameters)
        //{
        //    if (objModel == null) { throw new ArgumentNullException(nameof(objModel)); }
        //    if (dogMng == null) throw new Exception($"sqlFrom: dogMng == null.");
        //    if (!dogMng.tabTypes.ContainsKey(objModel.GetType())) throw new Exception($"sqlFrom: Classe {objModel.GetType().FullName} non trovata.");
        //    DogTable tab = dogMng.tabTypes[objModel.GetType()];
        //    if (tab == null) throw new Exception($"sqlFrom: tab [{objModel.GetType().FullName}] == null.");
        //    //recupero nome tabella
        //    var sqlTableNameExt = tab.SqlTableNameExt?.Trim() ?? "";
        //    return $"from {sqlTableNameExt} \n";
        //}

        ////crea WHERE per l'oggetto del modello 'objModel' in base all'icode
        //internal static string sqlWhereIcode(DogManager dogMng, ModelErp objModel, object icode, ref IDictionary<string, object> parameters, string options = "")
        //{
        //    return sqlWhereListIcode(dogMng, objModel, new List<object>() { icode }, ref parameters);
        //}
        //internal static string sqlWhereListIcode(DogManager dogMng, ModelErp objModel, List<object> rowIdList, ref IDictionary<string, object> parameters, string options = "")
        //{
        //    return sqlWhereListXref(dogMng, objModel, null, rowIdList, ref parameters, options);
        //}
        //internal static string sqlWhereListXref(DogManager dogMng, ModelErp objModel, DogField fldXref, List<object> rowIdList, ref IDictionary<string, object> parameters, string options = "")
        //{
        //    var sqlRowName = "";
        //    if (rowIdList == null) { throw new ArgumentNullException("DogManagerInt.sqlWhereListXref: Null " + nameof(rowIdList)); }
        //    //if (rowIdList.Count() == 0) { throw new ArgumentNullException("DogManagerInt.sqlWhereListXref: Empty " + nameof(rowIdList)); }
        //    if (fldXref == null)  // uso icode della tabella
        //    {
        //        if (objModel == null) { throw new ArgumentNullException("Null " + nameof(objModel)); }
        //        if (dogMng == null) throw new Exception($"sqlWhereListXref: dogMng == null.");
        //        if (!dogMng.tabTypes.ContainsKey(objModel.GetType())) throw new Exception($"sqlWhereListXref: Classe {objModel.GetType().FullName} non trovata.");
        //        DogTable tab = dogMng.tabTypes[objModel.GetType()];
        //        if (tab == null) throw new Exception($"sqlWhereListXref: tab [{objModel.GetType().FullName}] == null.");
        //        sqlRowName = tab.fldIcode?.SqlFieldName?.Trim() ?? "";
        //        if (options.Contains("[UsePropertyNameField]")) sqlRowName = tab.fldIcode?.fieldName?.Trim() ?? "";
        //    }
        //    else
        //    {
        //        sqlRowName = fldXref.SqlFieldName;
        //        if (options.Contains("[UsePropertyNameField]")) sqlRowName = fldXref.fieldName;
        //    }
        //    if (rowIdList.Count() == 0) return $"where 1=0 "; //RESTITUISCO LISTA VUOTA
        //    return $"where {sqlRowName} in ({string.Join(", ", DogManager.addListParam(rowIdList, ref parameters))}) ";
        //}



        //**************************************************************************************************
        //**************************************************************************************************
        //sqlEx con supporto a campi xdata e formattazione dinamica


        internal static string sqlSelectXdataEx(DogTable tab, ref IDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder("select ");
            //lista campi Xdata
            //sb.AppendLine($" {_testSqlFiedNmame(tab.fldIcode.SqlFieldName)} as Icode,"); sb.AppendLine($" {tab.fldDeleted.SqlFieldName} as Deleted,"); sb.AppendLine($" {tab.fldTimestamp.SqlFieldName} as Timestamp,");
            //sb.AppendLine($" {tab.fldCdate.SqlFieldName} as Cdate,"); sb.AppendLine($" {tab.fldCtime.SqlFieldName} as Ctime,"); sb.AppendLine($" {tab.fldCagent.SqlFieldName} as Cagent,"); sb.AppendLine($" {tab.fldCunit.SqlFieldName} as Cunit,");
            //sb.AppendLine($" {tab.fldMdate.SqlFieldName} as Mdate,"); sb.AppendLine($" {tab.fldMtime.SqlFieldName} as Mtime,"); sb.AppendLine($" {tab.fldMagent.SqlFieldName} as Magent,"); sb.AppendLine($" {tab.fldMunit.SqlFieldName} as Munit,");
            //sb.AppendLine($" {tab.fldHome.SqlFieldName} as Home,"); sb.AppendLine($" {tab.fldVersion.SqlFieldName} as Version,"); sb.AppendLine($" {tab.fldInactive.SqlFieldName} as Inactive,"); sb.AppendLine($" {tab.fldExtatt.SqlFieldName} as Extatt,");
            //sb.AppendLine($" {tab.fldMref.SqlFieldName} as Mref,");
            //sb.AppendLine($" {tab.fldSeq.SqlFieldName} as Seq,");
            //sb.AppendLine($" {tab.fldDescr.SqlFieldName} as Descr,");
            //sb.AppendLine($" {tab.fldFmt.SqlFieldName} as Fmt,");
            //sb.AppendLine($" {tab.fldXdurl.SqlFieldName} as Xdurl,");
            //sb.AppendLine($" {tab.fldXdatum.SqlFieldName} as Xdatum,");
            sb.AppendLine($@" 
            	{_testSqlFiedNmame(tab.fldIcode.SqlFieldName)} as Icode, {_testSqlFiedNmame(tab.fldDeleted.SqlFieldName)} as Deleted, {_testSqlFiedNmame(tab.fldTimestamp.SqlFieldName)} as Timestamp,
            	{_testSqlFiedNmame(tab.fldCdate.SqlFieldName)} as Cdate, {_testSqlFiedNmame(tab.fldCtime.SqlFieldName)} as Ctime, {_testSqlFiedNmame(tab.fldCagent.SqlFieldName)} as Cagent, {_testSqlFiedNmame(tab.fldCunit.SqlFieldName)} as Cunit,
            	{_testSqlFiedNmame(tab.fldMdate.SqlFieldName)} as Mdate, {_testSqlFiedNmame(tab.fldMtime.SqlFieldName)} as Mtime, {_testSqlFiedNmame(tab.fldMagent.SqlFieldName)} as Magent, {_testSqlFiedNmame(tab.fldMunit.SqlFieldName)} as Munit,
            	{_testSqlFiedNmame(tab.fldHome.SqlFieldName)} as Home, {_testSqlFiedNmame(tab.fldVersion.SqlFieldName)} as Version, {_testSqlFiedNmame(tab.fldInactive.SqlFieldName)} as Inactive, {_testSqlFiedNmame(tab.fldExtatt.SqlFieldName)} as Extatt,
            	{_testSqlFiedNmame(tab.fldMref.SqlFieldName)} as Mref,
            	{_testSqlFiedNmame(tab.fldSeq.SqlFieldName)} as Seq,
            	{_testSqlFiedNmame(tab.fldDescr.SqlFieldName)} as Descr,
            	{_testSqlFiedNmame(tab.fldFmt.SqlFieldName)} as Fmt,
            	{_testSqlFiedNmame(tab.fldXdurl.SqlFieldName)} as Xdurl,
            	{_testSqlFiedNmame(tab.fldXdatum.SqlFieldName)} as Xdatum,
                        ");
            // terminatore di select
            sb.AppendLine($" 0 as ErpTerm ");
            return sb.ToString();
        }
        private static string _testSqlFiedNmame(string sqlFieldName) => (sqlFieldName != "") ? sqlFieldName : "NULL";

        //crea SELECT per l'oggetto del modello 'objModel'
        internal static string sqlSelectEx(DogTable tab, ref IDictionary<string, object> parameters)
        {
            StringBuilder sb = new StringBuilder("select ");
            //ciclo sui campi della tabella
            foreach (var fld in tab.fields)
            {
                string propertyName = fld.fieldName;
                var sqlFieldNameExt = fld.SqlFieldName?.Trim() ?? "";
                if (sqlFieldNameExt != "") { sb.AppendLine($" {sqlFieldNameExt} as {propertyName},"); }
            }
            // terminatore di select
            sb.AppendLine($" 0 as ErpTerm ");
            return sb.ToString();
        }
        //crea FROM per l'oggetto del modello 'objModel'
        internal static string sqlFromEx(DogTable tab, ref IDictionary<string, object> parameters)
        {
            //recupero nome tabella
            var sqlTableNameExt = tab.SqlTableNameExt?.Trim() ?? "";
            return $"from {sqlTableNameExt} \n";
        }
        //crea WHERE per l'oggetto del modello 'objModel' in base all'oggetto di selezione 'selModel'
        internal static string sqlWhereSelection(DogManager dogMng, ModelErp selModel, ref IDictionary<string, object> parameters, string options = "")
        {
            StringBuilder sb = new StringBuilder("where ");
            // init
            int numCond = 0, numPreCond = 0;
            if (selModel == null) { throw new ArgumentNullException(nameof(selModel)); }
            if (dogMng == null) throw new Exception($"sqlWhereSelection: dogMng == null.");
            if (!dogMng.selTypes.ContainsKey(selModel.GetType())) throw new Exception($"sqlWhereSelection: Classe {selModel.GetType()} non trovata.");
            DogTable sel = dogMng.selTypes[selModel.GetType()];
            if (sel == null) throw new Exception($"sqlWhereSelection: sel [{selModel.GetType()}] == null.");
            //ciclo sui campi di selezione
            foreach (var selFld in sel.fields)
            {
                string propertyName = selFld.fieldName;
                var sqlFieldNameExt = selFld.SqlFieldName?.Trim() ?? "";
                if (sqlFieldNameExt == "") { throw new Exception($"sqlWhereSelection: sqlFieldNameExt is empty."); }
                object propertyValue = selFld.GetValue((ModelErp)selModel);    //property.GetValue(selModel);
                //---
                if (propertyValue == null) continue;    //>>>> NESSUNA CONDIZIONE APPLICATA <<<<
                //---
                if (options.Contains("[UsePropertyNameField]"))
                {
                    sqlFieldNameExt = (propertyName + "    ").Substring(3).Trim(); // uso il campo delle struttura in caso where per VISTE del tipo: "SELECT * FROM ( ??select?? ) ) AS subquery ??where??
                }
                //---
                if (propertyValue is string str)
                {
                    if (selFld.optUID) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(str.TrimEnd(), ref parameters)} and ");   //sb.AppendLine($" {sqlFieldNameExt} = '{str.TrimEnd()}' and ");
                    else if (selFld.optXID) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(str.TrimEnd(), ref parameters)} and "); //sb.AppendLine($" {sqlFieldNameExt} = '{str.TrimEnd()}' and ");
                    else sb.AppendLine($" {sqlFieldNameExt} LIKE {DogManager.addParam($"%{str.TrimEnd()}%", ref parameters)} and "); //sb.AppendLine($" {sqlFieldNameExt} LIKE '%{str}%' and ");
                }
                else if (propertyValue is DateTime dattim)  // DateOnly.FromDateTime()
                {
                    if (selFld.optDATE) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(DateOnly.FromDateTime(dattim), ref parameters)} and ");  //sb.AppendLine($" {sqlFieldNameExt} = '{dattim.ToString(DogManager.DB_FORMAT_DATE)}' and ");
                    else if (selFld.optTIME) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(TimeOnly.FromDateTime(dattim), ref parameters)} and ");  //sb.AppendLine($" {sqlFieldNameExt} = '{dattim.ToString(DogManager.DB_FORMAT_TIME)}' and ");
                    else if (selFld.optDATETIME) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(dattim, ref parameters)} and ");  //sb.AppendLine($" {sqlFieldNameExt} = '{dattim.ToString(DogManager.DB_FORMAT_DATETIME)}' and ");
                    else throw new ErpException($"{propertyName}: DateTime fa riferimento ad un campo non data ora");
                }
                else if (propertyValue is DateOnly dat)
                {
                    if (selFld.optDATE) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(dat, ref parameters)} and ");  //sb.AppendLine($" {sqlFieldNameExt} = '{dat.ToString(DogManager.DB_FORMAT_DATE)}' and ");
                }
                else if (propertyValue is TimeOnly tim)
                {
                    if (selFld.optTIME) sb.AppendLine($" {sqlFieldNameExt} = {DogManager.addParam(tim, ref parameters)} and ");  //sb.AppendLine($" {sqlFieldNameExt} = '{tim.ToString(DogManager.DB_FORMAT_TIME)}' and ");
                }
                //>>>TC>>>// else if (propertyValue is List<string> strList) sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", DogManager.addListParam(strList.Select(str => str.TrimEnd()).ToList<object>(), ref parameters))).AppendLine($") and");  //sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", strList.Select(str => $"'{str.Trim()}'"))).AppendLine($") and");
                else if (propertyValue is List<string> strList_all)
                {
                    List<string> strList = strList_all.Where(item => item != null && !(item is string str && string.IsNullOrWhiteSpace(str))).ToList();  // elimina elementi null e strighe vuote
                    //---
                    if (strList.Count() == 0) continue;    //>>>> NESSUNA CONDIZIONE APPLICATA <<<<
                    //---
                    // in javascrip considero qualsiasi valore come stringa.
                    // se la chiave univoca è un numero, devo converitre la stringa in numero prima di passarla al DBMS
                    //if (fld.optBIGINT) sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", DogManager.addListParam(strList.Select(str => (System.Int64)long.Parse(str.TrimEnd()))).ToList<object>(), ref parameters))).AppendLine($") and");
                    if (selFld.optBIGINT)
                    {
                        sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", DogManager.addListParam(strList.Select(s => (object)long.Parse(s)).ToList<object>(), ref parameters))).AppendLine($") and");
                    }
                    else
                    {
                        sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", DogManager.addListParam(strList.Select(str => str.TrimEnd()).ToList<object>(), ref parameters))).AppendLine($") and");
                    }
                }
                //<<<TC<<<
                else if (propertyValue is List<long> lngList_all)
                {
                    List<long> lngList = lngList_all.Where(item => item != null).ToList();  // elimina elementi null 
                    //---
                    if (lngList.Count() == 0) continue;    //>>>> NESSUNA CONDIZIONE APPLICATA <<<<
                    //---
                    sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", DogManager.addListParam(lngList.Select(u => (object)u).ToList(), ref parameters))).AppendLine($") and");  //sb.Append($" {sqlFieldNameExt} in (").Append(string.Join(", ", lngList)).AppendLine($") and");
                }
                else if (propertyValue is DateRange dateRng)
                {
                    if (dateRng.StartDate == default && dateRng.EndDate == default) continue;    //>>>> NESSUNA CONDIZIONE APPLICATA <<<<   (entrambe le date sono null)
                    //---
                    if (dateRng.StartDate == default)
                    {
                        if (selFld.optDATE) sb.AppendLine($" {sqlFieldNameExt} <= {DogManager.addParam(DateOnly.FromDateTime(dateRng.EndDate), ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} <= '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else if (selFld.optTIME) sb.AppendLine($" {sqlFieldNameExt} <= {DogManager.addParam(TimeOnly.FromDateTime(dateRng.EndDate), ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} <= '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else if (selFld.optDATETIME) sb.AppendLine($" {sqlFieldNameExt} <= {DogManager.addParam(dateRng.EndDate, ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} <= '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else throw new ErpException($"{propertyName}: DateRange fa riferimento ad un campo non data ora (1)");
                    }
                    else if (dateRng.EndDate == default)
                    {
                        if (selFld.optDATE) sb.AppendLine($" {sqlFieldNameExt} >= {DogManager.addParam(DateOnly.FromDateTime(dateRng.StartDate), ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} >= '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else if (selFld.optTIME) sb.AppendLine($" {sqlFieldNameExt} >= {DogManager.addParam(TimeOnly.FromDateTime(dateRng.StartDate), ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} >= '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else if (selFld.optDATETIME) sb.AppendLine($" {sqlFieldNameExt} >= {DogManager.addParam(dateRng.StartDate, ref parameters)} and");  //sb.AppendLine($" {sqlFieldNameExt} >= '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and");
                        else throw new ErpException($"{propertyName}: DateRange fa riferimento ad un campo non data ora (2)");
                    }
                    else
                    {
                        if (selFld.optDATE) sb.AppendLine($" ({sqlFieldNameExt} BETWEEN {DogManager.addParam(DateOnly.FromDateTime(dateRng.StartDate), ref parameters)} and {DogManager.addParam(DateOnly.FromDateTime(dateRng.EndDate), ref parameters)}) and");  //sb.AppendLine($" ({sqlFieldNameExt} BETWEEN '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}') and");
                        else if (selFld.optTIME) sb.AppendLine($" ({sqlFieldNameExt} BETWEEN {DogManager.addParam(TimeOnly.FromDateTime(dateRng.StartDate), ref parameters)} and {DogManager.addParam(TimeOnly.FromDateTime(dateRng.EndDate), ref parameters)}) and");  //sb.AppendLine($" ({sqlFieldNameExt} BETWEEN '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}') and");
                        else if (selFld.optDATETIME) sb.AppendLine($" ({sqlFieldNameExt} BETWEEN {DogManager.addParam(dateRng.StartDate, ref parameters)} and {DogManager.addParam(dateRng.EndDate, ref parameters)}) and");  //sb.AppendLine($" ({sqlFieldNameExt} BETWEEN '{dateRng.StartDate.ToString(DogManager.DB_FORMAT_DATE)}' and '{dateRng.EndDate.ToString(DogManager.DB_FORMAT_DATE)}') and");
                        else throw new ErpException($"{propertyName}: DateRange fa riferimento ad un campo non data ora (3)");
                    }
                }
                else continue;
                numCond++; //condizione applicata correttamente
            }
            // Verifica condizioni
            if (numCond == 0) throw new ErpException("Nessuna condizione inserita");
            // terminatore di where
            string SqlTableProperties = sel.SqlTableProperties?.Trim() ?? "";
            if (SqlTableProperties.Contains("[NoSysFields]") == false && options.Contains("[DELETED=Y]") == false)
            {  // escludo filtro XX__DELETED='N' se il campo non è previsto per la Tabella
                string delField = sel.tabSelection?.fldDeleted?.SqlFieldName?.Trim() ?? "";
                if (delField != "") sb.AppendLine($" {delField} = {DogManager.addParam("N", ref parameters)} ");      //__DELETED
            }
            else
            {
                sb.AppendLine($" 1=1 ");
            }
            return sb.ToString();
        }

        //crea WHERE per l'oggetto del modello 'objModel' in base all'icode
        internal static string sqlWhereListIcodeEx(DogTable tab, List<object> rowIdList, bool isXdata, ref IDictionary<string, object> parameters, string options = "")
        {
            return sqlWhereListXrefEx(tab, null, rowIdList, null, isXdata, ref parameters, options);
        }
        internal static string sqlWhereListXrefEx(DogTable tab, DogField fldXref, List<object> rowIdList, List<string> lstFmt, bool isXdata, ref IDictionary<string, object> parameters, string options = "")
        {
            string sqlRowName = "", sqlDelField = ""; string sqlFmt = "", sqlDeleted = "";
            if (rowIdList == null) { throw new ArgumentNullException("DogManagerInt.sqlWhereListXref: Null " + nameof(rowIdList)); }
            if (fldXref == null)  // uso icode della tabella
            {
                if(isXdata)
                {
                    if (tab.tabXdata == null) throw new Exception($"sqlWhereListXref: tab [{tab.tableTpy.FullName}] Xdata == null.");
                    sqlRowName = tab.tabXdata.fldIcode?.SqlFieldName?.Trim() ?? ""; sqlDelField = tab.tabXdata.fldDeleted?.SqlFieldName?.Trim() ?? "";
                    if (options.Contains("[UsePropertyNameField]")) { sqlRowName = tab.tabXdata.fldIcode?.fieldName?.Trim() ?? ""; sqlDelField = tab.tabXdata.fldDeleted?.fieldName?.Trim() ?? ""; }
                }
                else
                {
                    sqlRowName = tab.fldIcode?.SqlFieldName?.Trim() ?? ""; sqlDelField = tab.fldDeleted?.SqlFieldName?.Trim() ?? "";
                    if (options.Contains("[UsePropertyNameField]")) { sqlRowName = tab.fldIcode?.fieldName?.Trim() ?? ""; sqlDelField = tab.fldDeleted?.fieldName?.Trim() ?? ""; }
                }
            }
            else
            {
                if (isXdata)
                {
                    if (tab.tabXdata == null) throw new Exception($"sqlWhereListXref: tab [{tab.tableTpy.FullName}] Xdata is null.");
                    sqlRowName = fldXref.SqlFieldName; sqlDelField = tab.tabXdata.fldDeleted?.SqlFieldName?.Trim() ?? "";
                    var sqlFmtRowName = tab.tabXdata.fldFmt?.SqlFieldName?.Trim() ?? "";  //var sqlFmtRowName = tab.tabXdata.fldGetFirstByOption("[FMT]")?.SqlFieldName?.Trim() ?? "";
                    if (options.Contains("[UsePropertyNameField]"))
                    {
                        sqlRowName = fldXref.fieldName; sqlDelField = tab.tabXdata.fldDeleted?.fieldName?.Trim() ?? "";
                        sqlFmtRowName = tab.tabXdata.fldFmt?.fieldName?.Trim() ?? "";     //sqlFmtRowName = tab.tabXdata.fldGetFirstByOption("[FMT]")?.fieldName?.Trim() ?? "";
                    }
                    sqlFmt = (isXdata && lstFmt != null && lstFmt.Count() > 0) ? $"and {sqlFmtRowName} in ({string.Join(", ", DogManager.addListParam(rowIdList, ref parameters))}) " : "";
                }
                else
                {
                    sqlRowName = fldXref.SqlFieldName; sqlDelField = tab.fldDeleted?.SqlFieldName?.Trim() ?? "";
                    if (options.Contains("[UsePropertyNameField]")) { sqlRowName = fldXref.fieldName; sqlDelField = tab.fldDeleted?.fieldName?.Trim() ?? ""; }
                 }
            }
            //---
            if (rowIdList.Count() == 0) return $"where 1=0 "; //RESTITUISCO LISTA VUOTA

            if (tab.SqlTableProperties.Contains("[NoSysFields]") == false && options.Contains("[DELETED=Y]") == false)
            {  // escludo filtro XX__DELETED='N' se il campo non è previsto per la Tabella
                if (sqlDelField != "") sqlDeleted = $"and {sqlDelField} = {DogManager.addParam("N", ref parameters)} ";      //__DELETED
            }

            return $"where {sqlRowName} in ({string.Join(", ", DogManager.addListParam(rowIdList, ref parameters))}) {sqlFmt} {sqlDeleted}";
        }



        //==========================================================================================================
        //==========================================================================================================

        // SQL MANTAIN
        //---------------

        //crea INSERT, UPDATE, DELETE(logico) per l'oggetto del modello 'tabModel' (SOLO PER TABELLE)
        internal static string sqlMantain(DogManager dogMng, ModelErp tabModel, ref IDictionary<string, object> parameters, ref List<DogResult> results, string options = "")
        {
            if (dogMng == null) throw new Exception($"sqlMantain: dogMng == null.");
            if (!dogMng.tabTypes.ContainsKey(tabModel.GetType())) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} non trovata.");
            DogTable tab = dogMng.tabTypes[tabModel.GetType()];
            if (tab == null) throw new Exception($"sqlMantain: tab [{tabModel.GetType()}] == null.");
            return _sqlMantain(dogMng, tab, (ModelDog)tabModel, ref parameters, ref results, options);
        }
        internal static string sqlMantainXdata(DogManager dogMng, DogTable tab, ModelXdata tabModel, ref IDictionary<string, object> parameters, ref List<DogResult> results, string options = "")
        {
            if (dogMng == null) throw new Exception($"sqlMantainXdata: dogMng == null.");
            if (tab == null) throw new Exception($"sqlMantainXdata: tab [{tabModel.GetType()}] == null.");
            return _sqlMantain(dogMng, tab, (ModelDog)tabModel, ref parameters, ref results, options);
        }
        private static string _sqlMantain(DogManager dogMng, DogTable tab, ModelDog tabModel, ref IDictionary<string, object> parameters, ref List<DogResult> results, string options = "")
        {
            StringBuilder sb = new StringBuilder(), sbValues = new StringBuilder();
            int numParam = 0;
            // init
            if (tabModel == null) { throw new ArgumentNullException(nameof(tabModel)); }

            //campi di sistema
            if (tab.fldIcode == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldIcode undefined.");
            if (tab.fldDeleted == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldDeleted undefined.");
            if (tab.fldTimestamp == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldTimestamp undefined.");
            if (tab.fldHome == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldHome undefined.");
            //--
            if (tab.fldCdate == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldCdate undefined.");
            if (tab.fldCtime == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldCtime undefined.");
            if (tab.fldCagent == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldCagent undefined.");
            if (tab.fldCunit == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldCunit undefined.");
            if (tab.fldMdate == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldMdate undefined.");
            if (tab.fldMtime == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldMtime undefined.");
            if (tab.fldMagent == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldMagent undefined.");
            if (tab.fldMunit == null) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} fldMunit undefined.");

            string dateNow = DateTime.Now.ToString(DogManager.DB_FORMAT_DATE), timeNow = DateTime.Now.ToString(DogManager.DB_FORMAT_TIME);
            string agent = ErpContext.Instance.UserId, unit = ErpContext.Instance.UnitId;

            string _db_cdate = dateNow, _db_ctime = timeNow, _db_cagent = agent, _db_cunit = unit;
            string _db_mdate = dateNow, _db_mtime = timeNow, _db_magent = agent, _db_munit = unit;

            if (options.Contains("*noSys*"))
            {
                _db_cdate = tab.fldCdate?.GetValue(tabModel)?.ToString() ?? dateNow;
                _db_ctime = tab.fldCtime?.GetValue(tabModel)?.ToString() ?? timeNow;
                _db_cagent = tab.fldCagent?.GetValue(tabModel)?.ToString() ?? agent;
                _db_cunit = tab.fldCunit?.GetValue(tabModel)?.ToString() ?? unit;
                _db_mdate = tab.fldMdate?.GetValue(tabModel)?.ToString() ?? dateNow;
                _db_mtime = tab.fldMtime?.GetValue(tabModel)?.ToString() ?? timeNow;
                _db_magent = tab.fldMagent?.GetValue(tabModel)?.ToString() ?? agent;
                _db_munit = tab.fldMunit?.GetValue(tabModel)?.ToString() ?? unit;
            }

            //gestione action
            char? action = tabModel.action;  //può assumere solo A[dd], M[odify], D[elete], R[ead]
            if (action == null || "AMD".Contains((char)action) == false) throw new Exception($"sqlMantain: Classe {tabModel.GetType()} wrong action[{action}].");
            if (action == 'A') { sb.AppendLine($"insert into {tab.SqlTableNameExt} ("); } else { sb.AppendLine($"update {tab.SqlTableNameExt} set "); }  //            if (action == 'A') { sb.AppendLine($"insert into {tab.SqlTableNameExt} ("); sbValues.AppendLine("("); } else { sb.AppendLine($"update {tab.SqlTableNameExt} set "); }

            if (action != 'D')
            {
                foreach (var fld in tab.fields)
                {
                    string propertyName = fld.fieldName; // Get property name and value
                    object? propertyValue = fld.GetValue(tabModel);

                    //fill propertyObject
                    object? propertyObject = null;
                    if (action == 'A' && propertyValue == null)
                    {
                        // esiste una condizione
                        var sqlFieldNameExt = fld.SqlFieldName?.Trim() ?? "";
                        if (sqlFieldNameExt != "")
                        {
                            //escludo campi di sistema
                            if (fld.optSYS) continue;

                            //DEFAULT VALUES
                            if (fld.fieldTyp == typeof(System.String))
                            {
                                if (fld.optUID || fld.optXID || fld.optXREF)
                                {
                                    if (IS_NULLABLE_ID) { propertyObject = DBNull.Value; } else { propertyObject = DogManager.DB_STRING_EMPTY; }
                                }
                                else if (fld.DefaultValue != null) { propertyObject = (string)fld.DefaultValue; } //{ propertyObject = Convert.ChangeType(fld.DefaultValue, fld.fieldTyp); }
                                else { propertyObject = DogManager.DB_STRING_EMPTY; }
                            }
                            else if (fld.fieldTyp == typeof(System.DateOnly?)) 
                            {
                                if (fld.DefaultValue != null) { propertyObject = (string)fld.DefaultValue; }    //{ propertyObject = DateOnly.Parse((string)fld.DefaultValue); }
                                else { propertyObject = DogManager.DB_DATE_MIN; }
                            }
                            else if (fld.fieldTyp == typeof(System.TimeOnly?)) 
                            {
                                if (fld.DefaultValue != null) { propertyObject = (string)fld.DefaultValue; }    //{ propertyObject = DateOnly.Parse((string)fld.DefaultValue); }
                                else { propertyObject = DogManager.DB_TIME_EMPTY; }
                            }
                            else if (fld.fieldTyp == typeof(System.DateTime?)) 
                            {
                                if (fld.DefaultValue != null) { propertyObject = (string)fld.DefaultValue; }
                                else { propertyObject = DogManager.DB_DATETIME_EMPTY; }
                            }
                            else if (fld.fieldTyp == typeof(System.Int16?)) //short
                            {
                                if (fld.DefaultValue != null) { propertyObject = System.Int16.Parse((string)fld.DefaultValue); }
                                else { if (IS_NULLABLE_ID) { propertyObject = DBNull.Value; } else { propertyObject = DogManager.DB_SHORT_EMPTY; } }
                            }  
                            else if (fld.fieldTyp == typeof(System.Int64?)) //long
                            {
                                if (fld.DefaultValue != null) { propertyObject = System.Int64.Parse((string)fld.DefaultValue); }
                                else { if (IS_NULLABLE_ID) { propertyObject = DBNull.Value; } else { propertyObject = DogManager.DB_LONG_EMPTY; } }
                            }  
                            else if (fld.fieldTyp == typeof(System.Double?)) //double
                            {
                                if (fld.DefaultValue != null) { propertyObject = System.Double.Parse((string)fld.DefaultValue); }
                                else { if (IS_NULLABLE_ID) { propertyObject = DBNull.Value; } else { propertyObject = DogManager.DB_DOUBLE_EMPTY; } }
                            }  
                            else if (fld.fieldTyp == typeof(System.Byte[])) //byte[]   ???????????????????????
                            {
                                //xx//propertyObject = DBNull.Value;

                                //gestione Xdatum

                                //verifico se è un campo xdatum (stream) e in caso positivo lo converto in stream
                                if (fld.optXDATUM && tab.tableTpy == typeof(ModelXdata))
                                {
                                    propertyObject = ((ModelXdata)tabModel)._streamXdatum;
                                }
                                if (propertyObject == null) { propertyObject = new byte[0]; }

                            }
                            else continue;  //  <<<<<<<<<<<<<<<<<<<<<< SALTO I CAMPI NULL


                            //INSERISCO I VALORI DI DEFAULT
                            sb.AppendLine($"{sqlFieldNameExt}, "); sbValues.AppendLine($"{DogManager.addParam(propertyObject, ref parameters)}, ");

                        }

                    }
                    else    
                    {
                        if (propertyValue == null) continue;  //  <<<<<<<<<<<<<<<<<<<<<< SALTO I CAMPI NULL

                        // esiste una condizione
                        var sqlFieldNameExt = fld.SqlFieldName?.Trim() ?? "";
                        if (sqlFieldNameExt != "")
                        {
                            //escludo campi di sistema
                            if (fld.optSYS) continue;

                            //gestione tipo campi
                            if (propertyValue is string str)
                            {
                                if (String.IsNullOrEmpty(str) && (fld.optUID || fld.optXID || fld.optXREF))
                                {
                                    if (IS_NULLABLE_ID) { propertyObject = DBNull.Value; } else { propertyObject = DogManager.DB_STRING_EMPTY; }
                                }
                                else propertyObject = (string)str.TrimEnd();
                            }
                            else if (propertyValue is DateTime dattim)  // DateOnly.FromDateTime()
                            {
                                if (fld.optDATE) propertyObject = DateOnly.FromDateTime(dattim);
                                else if (fld.optTIME) propertyObject = TimeOnly.FromDateTime(dattim);
                                else if (fld.optDATETIME) propertyObject = (DateTime)dattim;
                                else throw new ErpException($"{propertyName}: DateTime fa riferimento ad un campo non data ora");
                            }
                            else if (propertyValue is DateOnly dat)
                            {
                                if (fld.optDATE) propertyObject = (DateOnly)dat;
                                else throw new ErpException($"{propertyName}: DateOnly fa riferimento ad un campo non data");
                            }
                            else if (propertyValue is TimeOnly tim)
                            {
                                if (fld.optTIME) propertyObject = (TimeOnly)tim;
                                else throw new ErpException($"{propertyName}: TimeOnly fa riferimento ad un campo non ora");
                            }
                            else if (propertyValue is byte[] bty)
                            {
                                propertyObject = (byte[])bty;
                            }
                            else if (propertyValue is short shr)
                            {
                                if (shr == DogManager.DB_SHORT_EMPTY) continue;  //  <<<<<<<<<<<<<<<<<<<<<< SALTO I CAMPI NULL
                                propertyObject = (short)shr;
                            }
                            else if (propertyValue is long lng)
                            {
                                if (lng == DogManager.DB_LONG_EMPTY) continue;  //  <<<<<<<<<<<<<<<<<<<<<< SALTO I CAMPI NULL
                                propertyObject = (long)lng;
                            }
                            else if (propertyValue is double dbl)
                            {
                                if (dbl == DogManager.DB_DOUBLE_EMPTY) continue;  //  <<<<<<<<<<<<<<<<<<<<<< SALTO I CAMPI NULL
                                propertyObject = (double)dbl;
                            }
                            else throw new ErpException($"{propertyName}: {propertyValue.GetType().Name} non è un tipo consentito"); ;

                        }

                        ////gestione Xdatum
                        //if (fld.optXDATUM && propertyObject==null && tabModel is ModelXdata xd) 
                        //{
                        //    propertyObject = (Stream?)xd._streamXdatum;
                        //}



                        // Costruzione SQL
                        if (action == 'A') { sb.AppendLine($"{sqlFieldNameExt}, "); sbValues.AppendLine($"{DogManager.addParam(propertyObject, ref parameters)}, "); }
                        else if (action == 'M') { sb.AppendLine($"{sqlFieldNameExt} = {DogManager.addParam(propertyObject, ref parameters)}, "); }
                        else throw new ArgumentOutOfRangeException(nameof(action));
                        numParam++; //condizione applicata correttamente
                    }
                }
            }
            else
            {
                //DELETED
                sb.AppendLine($"{tab.fldDeleted.SqlFieldName} = {DogManager.addParam("Y", ref parameters)}, ");
                numParam = 1; 
                if (IS_NULLABLE_INDEX && IS_NULLABLE_ID)
                {  //se cancello il record elimino i campi chiave per evitare problemi di integrita referenziale
                    foreach (var fld in tab.fields)
                    {
                        string propertyName = fld.fieldName; // Get property name and value
                        var sqlFieldNameExt = fld.SqlFieldName?.Trim() ?? "";
                        if (sqlFieldNameExt != "")
                        {
                            if (fld.optUID || fld.optXID || fld.optXREF) { sb.AppendLine($"{sqlFieldNameExt} = {DogManager.addParam(DBNull.Value, ref parameters)}, "); }
                        }
                    }
                }
            }
            // Verifica condizioni: con *allowTouch* posso eseguire insert/update anche senza campi valorizzati, altrimenti è necessario almeno un campo da inserire/modificare
            if (numParam == 0)
            {
                if (options.Contains("*allowTouch*") == false) throw new ErpException("Nessun parametro inserito");
            }
            //gestione icode e timestamp
            object? icode = tab.fldIcode?.GetValue(tabModel) ?? null;
            byte[]? oldTimestamp = (byte[]?)(tab.fldTimestamp?.GetValue(tabModel) ?? null);
            byte[] newTimestamp = DatabaseManager.GenerateTimestamp();

            // terminatore di insert update
            if (action == 'A')
            {
                //-----------------------------------------------------------
                //-- SE L'ICODE NON E' FORNITO LO DEVO GENERARE
                //-----------------------------------------------------------
                if (UtilHelper.IsNullOrEmptyObject(icode)) { icode = dogMng.GenerateIcode(); tab.fldIcode?.SetValue(tabModel, icode); }
                //-----------------------------------------------------------

                //--
                if (dogMng.DatabaseType != DbTyp.SqlServer && dogMng.DatabaseType != DbTyp.Sybase)
                {
                    sb.AppendLine($"{tab.fldTimestamp.SqlFieldName}, ");
                    sbValues.AppendLine($"{DogManager.addParam(newTimestamp, ref parameters)}, ");
                }
                sb.AppendLine($"{tab.fldIcode.SqlFieldName}, {tab.fldDeleted.SqlFieldName}, {tab.fldHome.SqlFieldName}, ");
                sbValues.AppendLine($"{DogManager.addParam(icode, ref parameters)}, {DogManager.addParam("N", ref parameters)}, {DogManager.addParam(dogMng.DbHome, ref parameters)}, ");
                //---
                sb.AppendLine($"{tab.fldCdate.SqlFieldName}, {tab.fldCtime.SqlFieldName}, {tab.fldCagent.SqlFieldName}, {tab.fldCunit.SqlFieldName}, ");
                sbValues.AppendLine($"{DogManager.addParam(_db_cdate, ref parameters)}, {DogManager.addParam(_db_ctime, ref parameters)}, {DogManager.addParam(_db_cagent, ref parameters)}, {DogManager.addParam(_db_cunit, ref parameters)}, ");
                //--
                sb.AppendLine($"{tab.fldMdate.SqlFieldName}, {tab.fldMtime.SqlFieldName}, {tab.fldMagent.SqlFieldName}, {tab.fldMunit.SqlFieldName}");
                sbValues.AppendLine($"{DogManager.addParam(_db_mdate, ref parameters)}, {DogManager.addParam(_db_mtime, ref parameters)}, {DogManager.addParam(_db_magent, ref parameters)}, {DogManager.addParam(_db_munit, ref parameters)}");
                //---
                sb.AppendLine(") values (").Append(sbValues.ToString()).Append(") ");
            }
            else  // modify or delete
            {
                //-----------------------------------------------------------
                //-- CONTROLLO ICODE TIMESTAMP
                //-----------------------------------------------------------
                if (UtilHelper.IsNullOrEmptyObject(icode)) throw new ErpException($"Icode vuoto per action [{action}]");
                if (oldTimestamp == null) throw new ErpException($"Timestamp vuoto per action [{action}] icode [{icode}]");
                //-----------------------------------------------------------


                if (dogMng.DatabaseType != DbTyp.SqlServer && dogMng.DatabaseType != DbTyp.Sybase)
                {
                    sb.AppendLine($"{tab.fldTimestamp.SqlFieldName} = {DogManager.addParam(newTimestamp, ref parameters)}, ");
                }
                //--
                sb.AppendLine($"{tab.fldMdate.SqlFieldName} = {DogManager.addParam(_db_mdate, ref parameters)}, {tab.fldMtime.SqlFieldName} = {DogManager.addParam(_db_mtime, ref parameters)}, ");
                sb.AppendLine($"{tab.fldMagent.SqlFieldName} = {DogManager.addParam(_db_magent, ref parameters)}, {tab.fldMunit.SqlFieldName} = {DogManager.addParam(_db_munit, ref parameters)}");
                //--
                sb.AppendLine($" where {tab.fldIcode.SqlFieldName} = {DogManager.addParam(icode, ref parameters)} and {tab.fldDeleted.SqlFieldName} = {DogManager.addParam("N", ref parameters)}");
                if (options.Contains("*noTms*") == false) sb.Append($" and {tab.fldTimestamp.SqlFieldName} = {DogManager.addParam(oldTimestamp, ref parameters)}");
            }

            //result
            results.Add(new DogResult(tabModel.GetType(), (char)action, icode, newTimestamp));
            return sb.ToString();
        }

        //crea select per rileggere icode e timestamp dei record presenti nell'elenco result
        //  ...serve quando il timestamp viene generato da DB e non in fase di insert/update (SqlServer/Sybase)
        internal static string sqlSelectIcodeTimestamp(DogManager dogMng, List<DogResult> results, ref IDictionary<string, object> parameters, string options = "")
        {
            //divido per tabelle
            IDictionary<System.Type, List<object>> tabList = new Dictionary<System.Type, List<object>>();
            foreach (DogResult result in results) 
            {
                if (!tabList.ContainsKey(result.TabType)) tabList.Add(result.TabType, new List<object>());
                tabList[result.TabType].Add(result.Icode); 
            }
            //scrivo query
            StringBuilder sb = new StringBuilder(); 
            foreach (System.Type tpy in tabList.Keys) {
                DogManager.DogTable tab = dogMng.tabTypes[tpy];
                if (sb.Length != 0) sb.AppendLine(" union ");
                //sb.Append($"select {tab.SqlPrefix}_ICODE as ICODE, {tab.SqlPrefix}_TIMESTAMP as TIMESTAMP from {tab.SqlTableName} where {tab.SqlPrefix}_ICODE in (")
                //    .Append(string.Join(", ", DogManager.addListParam(tabList[tpy].Select(obj => UtilHelper.TrimEndObject(obj)).ToList<object>(), ref parameters)))
                //    .AppendLine(") "); ;
                sb.Append($"select {tab.fldIcode.SqlFieldName} as ICODE, {tab.fldTimestamp.SqlFieldName} as TIMESTAMP from {tab.SqlTableName} where {tab.fldIcode.SqlFieldName} in (")
                    .Append(string.Join(", ", DogManager.addListParam(tabList[tpy].Select(obj => UtilHelper.TrimEndObject(obj)).ToList<object>(), ref parameters)))
                    .AppendLine(") "); ;
            }
            return sb.ToString();
        }

        //******************************************************************************************************************
        //******************************************************************************************************************

        internal static object? getPropertyValue(DogManager dogMng, ModelErp obj_selModel, string propName)
        {
            if (obj_selModel == null) { throw new ArgumentNullException(nameof(obj_selModel)); }
            if (dogMng == null) throw new Exception($"getPropertyValue: dogMng == null.");
            DogTable tab = null;
            if (dogMng.tabTypes.ContainsKey(obj_selModel.GetType())) tab = dogMng.tabTypes[obj_selModel.GetType()];
            else if (dogMng.selTypes.ContainsKey(obj_selModel.GetType())) tab = dogMng.selTypes[obj_selModel.GetType()];
            else throw new Exception($"getPropertyValue: Classe {obj_selModel.GetType()} non trovata in tab e sel.");
            if (tab == null) throw new Exception($"getPropertyValue: tab [{obj_selModel.GetType()}] == null.");
            foreach (var fld in tab.fields)
            {
                string propertyName = fld.fieldName; // Get property name and value
                object propertyValue = fld.GetValue((ModelErp)obj_selModel); 
                if (propertyValue == null) continue;
                // >>> verifica List
                if (typeof(IEnumerable<object>).IsAssignableFrom(propertyValue.GetType()))
                {
                    IEnumerable<object> ienum = (IEnumerable<object>)propertyValue;
                    List<object> list = ienum.Where(item => item != null && !(item is string str && string.IsNullOrWhiteSpace(str))).ToList();  // elimina elementi null e strighe vuote
                    if (list.Count() == 0) continue;
                    if (list[0] is string) propertyValue = (List<string>)list.Select(i => i.ToString()).ToList();
                    else if (list[0] is sbyte || list[0] is byte || list[0] is short || list[0] is ushort || list[0] is int || list[0] is uint
                         || list[0] is long || list[0] is ulong) propertyValue = (List<long>)list.Select(i => Convert.ToInt64(i)).ToList();
                    else throw new ErpException("Tipo Lista non supportato (solo stinga e intero)");
                }
                //---
                if (propertyValue is string str)
                {
                    if (propName == propertyName) return str;
                }
                else if (propertyValue is List<string> strList)
                {
                    for (int i = 0; i < strList.Count; i++)
                        try
                        {
                            if (propName == propertyName + "[" + i.ToString() + "]") return strList[i];
                        }
                        catch (Exception ex) { }  //skip exceptions
                }
                else if (propertyValue is List<long> lngList)
                {
                    for (int i = 0; i < lngList.Count; i++)
                        try
                        {
                            if (propName == propertyName + "[" + i.ToString() + "]") return lngList[i];
                        }
                        catch (Exception ex) { }  //skip exceptions
                }
                else if (propertyValue is DateRange dateRng)
                {
                    if (propName == propertyName + ".StartDate" && dateRng.StartDate != default) return dateRng.StartDate;
                    if (propName == propertyName + ".EndDate" && dateRng.EndDate != default) return dateRng.EndDate;
                }
                else continue;
            }
            return null;
        }

        //Custom Model Binder
        internal static bool setPropertyValue(DogManager dogMng, ModelErp obj_selModel, string propName, string? propValue)
        {
            if (obj_selModel == null) { throw new ArgumentNullException(nameof(obj_selModel)); }
            if (propName == null) { throw new ArgumentNullException(nameof(propName)); }

            if (obj_selModel == null) { throw new ArgumentNullException(nameof(obj_selModel)); }
            if (dogMng == null) throw new Exception($"setPropertyValue: dogMng == null.");
            DogTable tab = null;
            if (dogMng.tabTypes.ContainsKey(obj_selModel.GetType())) tab = dogMng.tabTypes[obj_selModel.GetType()];
            else if (dogMng.selTypes.ContainsKey(obj_selModel.GetType())) tab = dogMng.selTypes[obj_selModel.GetType()];
            else throw new Exception($"setPropertyValue: Classe {obj_selModel.GetType()} non trovata in tab e sel.");
            if (tab == null) throw new Exception($"setPropertyValue: tab [{obj_selModel.GetType()}] == null.");
            try { 
                foreach (var fld in tab.fields)
                {
                    string propertyName = fld.fieldName; // Get property name and value
                    string attribXref = fld.XrefObj?.fieldName ?? ""; 
                    object? propertyValue = fld.GetValue((ModelErp)obj_selModel);
                    if (propName.StartsWith(propertyName + "[") || propName.StartsWith(attribXref + "["))
                    {
                        if (propValue != null)
                        {
                            System.Type argumentType = propertyValue.GetType().GenericTypeArguments[0];
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(argumentType);
                            object propValueObj = typeConverter.ConvertFromString(propValue);
                            ((IList)propertyValue).Add(propValueObj);
                            fld.SetValue((ModelErp)obj_selModel, propertyValue); //^^//property.SetValue(selModel, propertyValue);
                            return true;  
                        }
                    }
                    else if (propName == propertyName + ".StartDate" || propName == propertyName + ".EndDate")
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(DateTime));
                        object propValueObj = typeConverter.ConvertFromString(propValue);
                        //---
                        if (propValueObj == null) propValueObj = default;
                        if (propName == propertyName + ".StartDate") ((DateRange)propertyValue).StartDate = (DateTime)propValueObj;
                        if (propName == propertyName + ".EndDate") ((DateRange)propertyValue).EndDate = (DateTime)propValueObj;
                        fld.SetValue((ModelErp)obj_selModel, (DateRange)propertyValue); //^^//property.SetValue(selModel, (DateRange)propertyValue); 
                        return true;
                    }
                    else if (propName == propertyName || propName == attribXref)
                    {
                        if (typeof(IEnumerable<object>).IsAssignableFrom(propertyValue.GetType()))
                        {
                            if (propValue != null)
                            {
                                System.Type argumentType = propertyValue.GetType().GenericTypeArguments[0];
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(argumentType);
                                object propValueObj = typeConverter.ConvertFromString(propValue);
                                ((IList)propertyValue).Add(propValueObj);
                                fld.SetValue((ModelErp)obj_selModel, propertyValue); //^^//property.SetValue(selModel, propertyValue); 
                                return true;  //((ICollection<string>)propertyValue).Add(propValue); property.SetValue(selModel, propertyValue);
                            }
                        }
                        else
                        {
                            if (propValue != null)
                            {
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyValue.GetType());
                                object propValueObj = typeConverter.ConvertFromString(propValue);
                                fld.SetValue((ModelErp)obj_selModel, propertyValue); //^^//property.SetValue(selModel, propValueObj); 
                                return true;
                            }
                            else
                            {
                                fld.SetValue((ModelErp)obj_selModel, null); //^^//property.SetValue(selModel, null); 
                                return true;
                            }
                        }
                    }
                    else continue;
                }
            }
            catch (Exception ex) { }  //skip exceptions
            return false;
        }


        // stesse funzioni ma senza DogManager

        internal static object? getPropertyValue_static(ModelErp selModel, string propName)
        {
            if (selModel == null) { throw new ArgumentNullException(nameof(selModel)); }
            //ciclo sulle proprietà
            System.Type type = selModel.GetType(); PropertyInfo[] properties = type.GetProperties();
            foreach (var property in properties)
            {
                try
                {
                    string propertyName = property.Name; // Get property name and value
                    object propertyValue = property.GetValue(selModel); //sb.AppendLine($"Property: {propertyName}, Value: {propertyValue}");
                    if (propertyValue == null) continue;
                    // >>> verifica List
                    if (typeof(IEnumerable<object>).IsAssignableFrom(propertyValue.GetType()))
                    {
                        IEnumerable<object> ienum = (IEnumerable<object>)propertyValue;
                        List<object> list = ienum.Where(item => item != null && !(item is string str && string.IsNullOrWhiteSpace(str))).ToList();  // elimina elementi null e strighe vuote
                        if (list.Count() == 0) continue;
                        if (list[0] is string) propertyValue = (List<string>)list.Select(i => i.ToString()).ToList();
                        else if (list[0] is sbyte || list[0] is byte || list[0] is short || list[0] is ushort || list[0] is int || list[0] is uint
                             || list[0] is long || list[0] is ulong) propertyValue = (List<long>)list.Select(i => Convert.ToInt64(i)).ToList();
                        else throw new ErpException("Tipo Lista non supportato (solo stinga e intero)");
                    }
                    //---
                    if (propertyValue is string str)
                    {
                        if (propName == propertyName) return str;
                    }
                    else if (propertyValue is List<string> strList)
                    {
                        for (int i = 0; i < strList.Count; i++)
                            try
                            {
                                if (propName == propertyName + "[" + i.ToString() + "]") return strList[i];
                            }
                            catch (Exception ex) { }  //skip exceptions
                    }
                    else if (propertyValue is List<long> lngList)
                    {
                        for (int i = 0; i < lngList.Count; i++)
                            try
                            {
                                if (propName == propertyName + "[" + i.ToString() + "]") return lngList[i];
                            }
                            catch (Exception ex) { }  //skip exceptions
                    }
                    else if (propertyValue is DateRange dateRng)
                    {
                        if (propName == propertyName + ".StartDate" && dateRng.StartDate != default) return dateRng.StartDate;
                        if (propName == propertyName + ".EndDate" && dateRng.EndDate != default) return dateRng.EndDate;
                    }
                    else continue;
                }
                catch (Exception ex) { }  //skip exceptions
            }
            return null;
        }

        //Custom Model Binder
        internal static bool setPropertyValue_static(ModelErp selModel, string propName, string? propValue)
        {
            if (selModel == null) { throw new ArgumentNullException(nameof(selModel)); }
            if (propName == null) { throw new ArgumentNullException(nameof(propName)); }
            try
            {
                //ciclo sulle proprietà
                System.Type type = selModel.GetType(); PropertyInfo[] properties = type.GetProperties();
                foreach (var property in properties)
                {
                    string propertyName = property.Name; // Get property name and value
                    string attribXref = ((ErpDogFieldAttribute)(property.GetCustomAttribute(typeof(ErpDogFieldAttribute))))?.Xref ?? "";
                    object? propertyValue = property.GetValue(selModel); //sb.AppendLine($"Property: {propertyName}, Value: {propertyValue}");
                    if (propName.StartsWith(propertyName + "[") || propName.StartsWith(attribXref + "["))
                    {
                        if (propValue != null)
                        {
                            System.Type argumentType = propertyValue.GetType().GenericTypeArguments[0];
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(argumentType);
                            object propValueObj = typeConverter.ConvertFromString(propValue);
                            ((IList)propertyValue).Add(propValueObj); property.SetValue(selModel, propertyValue); return true;  //((ICollection<string>)propertyValue).Add(propValue); property.SetValue(selModel, propertyValue);
                        }
                    }
                    else if (propName == propertyName + ".StartDate" || propName == propertyName + ".EndDate")
                    {
                        TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(DateTime));
                        object propValueObj = typeConverter.ConvertFromString(propValue);
                        //---
                        if (propValueObj == null) propValueObj = default;
                        if (propName == propertyName + ".StartDate") ((DateRange)propertyValue).StartDate = (DateTime)propValueObj;
                        if (propName == propertyName + ".EndDate") ((DateRange)propertyValue).EndDate = (DateTime)propValueObj;
                        property.SetValue(selModel, (DateRange)propertyValue); return true;
                    }
                    else if (propName == propertyName || propName == attribXref)
                    {
                        if (typeof(IEnumerable<object>).IsAssignableFrom(propertyValue.GetType()))
                        {
                            if (propValue != null)
                            {
                                System.Type argumentType = propertyValue.GetType().GenericTypeArguments[0];
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(argumentType);
                                object propValueObj = typeConverter.ConvertFromString(propValue);
                                ((IList)propertyValue).Add(propValueObj); property.SetValue(selModel, propertyValue); return true;  //((ICollection<string>)propertyValue).Add(propValue); property.SetValue(selModel, propertyValue);
                            }
                        }
                        else
                        {
                            if (propValue != null)
                            {
                                TypeConverter typeConverter = TypeDescriptor.GetConverter(propertyValue.GetType());
                                object propValueObj = typeConverter.ConvertFromString(propValue);
                                property.SetValue(selModel, propValueObj); return true;
                            }
                            else
                            {
                                property.SetValue(selModel, null); return true;
                            }
                        }
                    }
                    else continue;
                }
            }
            catch (Exception ex) { }  //skip exceptions
            return false;
        }



        //*********************************************************************************************************

        internal static string replaceSqlTextWithPlaceholders(string sql, ref IDictionary<string, object> parameters)  // converte, se trova parametri (eg: 'Y') nel formato richiesto dal DogManager (eg: {DogManager.addParam(""Y"", ref parameters)})
        {
            StringBuilder sb = new StringBuilder(); StringBuilder text = new StringBuilder(); 
            char[] s = sql.ToCharArray();

            bool InStringa = false;
            for (int i = 0; i < s.Length; i++)
            {
                //sono fuori dalla stringa
                if (InStringa == false)
                {
                    if (s[i] == '\'') InStringa = true;
                    else sb.Append(s[i]);
                }
                //sono dentro la stringa
                else if (s[i] == '\'')
                {
                    if (i+1 < s.Length && s[i+1] == '\'')
                    {
                        text.Append("''"); i++;
                    }
                    else
                    {
                        sb.Append(DogManager.addParam(text.ToString().Replace("''","'"), ref parameters)); text = new StringBuilder();
                        InStringa = false;
                    }
                }
                else text.Append(s[i]);
            }
            return sb.ToString();
        }



    }
}
