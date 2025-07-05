using Amazon.Runtime.Internal.Transform;
using Amazon.SecurityToken.Model;
using ErpToolkit.Models;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Microsoft.CodeAnalysis;
using MongoDB.Driver;
using MySql.Data.MySqlClient.X.XDevAPI.Common;
using Org.BouncyCastle.Crypto.Parameters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static ErpToolkit.Helpers.Db.DatabaseFactory;
using static ErpToolkit.Helpers.ErpError;


namespace ErpToolkit.Helpers.Db
{
    //------------------- 
    //Data Object Gateway
    //-------------------
    // Funzioni di gestione accesso al Database, con il supporto del Data Model 
    public class DogManager
    {
        //formati interni
        internal const string DB_FORMAT_DATE = "yyyy/MM/dd"; //formato stringa di memorizzazione della data nel DB
        internal const string DB_FORMAT_TIME = "HH:mm:ss"; //formato stringa di memorizzazione dell'ora nel DB
        internal const string DB_FORMAT_DATETIME = "yyyy/MM/dd HH:mm:ss"; //formato stringa di memorizzazione di data e ora nel DB

        internal const string DB_DATE_MAX = "9999/99/99"; //futuro
        internal const string DB_DATE_MIN = "    /  /  "; //passato
        internal const string DB_TIME_EMPTY = "  :  :  "; //vuoto
        internal const string DB_DATETIME_EMPTY = "    /  /     :  :  "; //vuoto
        internal const string DB_STRING_EMPTY = " "; //vuoto
        internal const short DB_SHORT_EMPTY = (short)(-32768); //vuoto
        internal const long DB_LONG_EMPTY = (long)(-2147483647 - 1); //vuoto
        internal const double DB_DOUBLE_EMPTY = (double)(-2147483648.0000); //vuoto
        //---

        //***************************************************************************************************************************************************
        //*** STRUTTURE STATICHE
        //***************************************************************************************************************************************************


        //voce del menù
        public class MenuItem
        {
            public string Name { get; } = "";
            public string Controller { get; } = "";
            public string Action { get; } = "";
            public MenuItem(string name, string controller, string action) { this.Name = name; this.Controller = controller; this.Action = action; }
        }
        //pagina del percorso
        public class Page
        {
            public string pageName { get; set; } = "";
            //public List<DefaultFieldValue> defaultFields { get; set; } = new List<DefaultFieldValue>();
            public Dictionary<string, string?> defaultFields { get; set; } = new Dictionary<string, string?>();
            public Page(string name) { pageName = name; }
            public Page AddDefault(string name, string? value) { defaultFields[name] = value; return this; }
        }

        //attributi di visibilità del campo
        public class FieldAttr
        {
            public char Readonly { get; set; } = 'N';
            public char Visible { get; set; } = 'Y';
            public void setAttr(string attr)
            {
                char[] a = attr.ToCharArray();
                for (int i = 0; i < a.Length; i++)
                {
                    switch (i)
                    {
                        case 0: Readonly = a[i]; break; // 0) ReadOnly
                        case 1: Visible = a[i]; break; // 1) Visible
                    }
                }
            }
            public string getAttr()
            {
                return (new string(new char[] { Readonly, Visible }));
            }
            public FieldAttr(string attr) { setAttr(attr); }
            public FieldAttr(bool readOnly, bool visible) { if (readOnly) Readonly = 'Y'; if (!visible) Visible = 'N'; }
            public static string strAttr(bool readOnly, bool visible) { return new FieldAttr(readOnly, visible).getAttr(); }
        }


        public class DogResult
        {
            public System.Type TabType { get; set; } = null;
            public char Action { get; set; } = ' ';
            public object Icode { get; set; } = null;
            public byte[]? Timestamp { get; set; } = null;
            public DogResult(System.Type tabType, char action, object icode, byte[]? timestamp)
            {
                TabType = tabType; Action = action; Icode = icode; Timestamp = timestamp;
            }
        }

        public class DogCache
        {
            public string ServiceName { get; set; } = "";
            public char ServiceAction { get; set; } = ' ';
            public System.Type ServiceTabType { get; set; } = null;

            private int serviceReadID = -1; 
            internal int GetReadID() { return ++serviceReadID; }
            internal void InitReadID() { serviceReadID = -1; }

            private int serviceMntID = -1;
            internal int GetMntID() { return ++serviceMntID; }
            internal void InitMntID() { serviceMntID = -1; }

            //-----

            public List<string> ruleXrefFrom { get; } = new List<string>();
            public void AddRuleXrefFrom(List<string> newRuleXrefFrom) { this.ruleXrefFrom.Union<string>(newRuleXrefFrom); } //integra valori passati a quelli presenti. esclude duplicati in base alla funzione Equal. E' case sensitive.

            public Dictionary<System.Type, Dictionary<object, ModelErp>> dbCache { get; } = new Dictionary<System.Type, Dictionary<object, ModelErp>>();


            public DogCache()
            {
            }
        }


        //gestione objects
        public static void checkTableObj(object tabModel) { if("TAB" != (tabModel.GetType().GetField("CATEG", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString()?.Trim() ?? "")) throw new ArgumentException(nameof(tabModel)); }


        //gestione properties
        public static object? getPropertyValue(object selModel, string propName) { return DogManagerInt.getPropertyValue(selModel, propName); }
        public static bool setPropertyValue(object selModel, string propName, string? propValue) { return DogManagerInt.setPropertyValue(selModel, propName, propValue);  }


        //gestione parameters
        public static string addParam(object value, ref IDictionary<string, object> parameters) { string parName = $"PARM{parameters.Count}"; parameters.Add(parName, value); return $"@{parName}"; }
        public static List<string> addListParam(List<object> values, ref IDictionary<string, object> parameters) { List<string> cond = new List<string>(); foreach (var value in values) { string parName = $"PARM{parameters.Count}"; parameters.Add(parName, value); cond.Add($"@{parName}"); } return cond; }



        //***************************************************************************************************************************************************
        //*** INIZIO CLASSE
        //***************************************************************************************************************************************************


        private string _modelName; // = "SIO";
        private string _modelMode; // = "";  //indica come interpretare il Modello. Se _modelMode == "FREE" allora il modello non prevede i campi standard _deleted _timestamp, ecc. e non gestisce le date come stringhe
        private DbTyp _databaseType; // = SqlServer;
        private string _connectionStringName; // = "#connectionString_SQLSLocal";
        private string _dbRoot; // = "IU01";
        private string _dbHome; // = "sio_PROD";
        private NLog.ILogger _logger;


        public DbTyp DatabaseType { get { return this._databaseType; } }
        public string DbHome { get { return this._dbHome; } }

        private DatabaseManager _getDbMg() { return ErpContext.Instance.DbFactory.GetDatabase(_databaseType, _connectionStringName); }


        // Proprietà configurabili
        public int PageSize { get { return _getDbMg().PageSize; } set { _getDbMg().PageSize = value; } }  
        public int MaxRetries { get { return _getDbMg().MaxRetries; } set { _getDbMg().MaxRetries = value; } }
        public int DelayBetweenRetriesMs { get { return _getDbMg().DelayBetweenRetriesMs; } set { _getDbMg().DelayBetweenRetriesMs = value; } }
        public int TimeoutSeconds { get { return _getDbMg().TimeoutSeconds; } set { _getDbMg().TimeoutSeconds = value; } }
        public int TransactionTimeoutSeconds { get { return _getDbMg().TransactionTimeoutSeconds; } set { _getDbMg().TransactionTimeoutSeconds = value; } }
        public int MaxRecords { get { return _getDbMg().MaxRecords; } set { _getDbMg().MaxRecords = value; } }
        public bool EnableTrace { get { return _getDbMg().EnableTrace; } set { _getDbMg().EnableTrace = value; } }
        public int MaxFileLengthBytes { get { return _getDbMg().MaxFileLengthBytes; } set { _getDbMg().MaxFileLengthBytes = value; } }


        //***************************************************************************************************************************************************
        //*** INIT
        //***************************************************************************************************************************************************

        public readonly Dictionary<string, DogTable> tables = new Dictionary<string, DogTable>();
        public readonly Dictionary<System.Type, DogTable> tabTypes = new Dictionary<System.Type, DogTable>();
        public readonly Dictionary<string, DogTable> tabPrefixes = new Dictionary<string, DogTable>();
        public readonly Dictionary<int, DogTable> tabIntcodes = new Dictionary<int, DogTable>();
        public readonly Dictionary<string, DogField> tabProperties = new Dictionary<string, DogField>();
        public readonly Dictionary<string, DogField> tabFields = new Dictionary<string, DogField>();
        //----Tabelle di selezione-------------------------
        public readonly Dictionary<string, DogTable> selfilters = new Dictionary<string, DogTable>();
        public readonly Dictionary<System.Type, DogTable> selTypes = new Dictionary<System.Type, DogTable>();
        public readonly Dictionary<string, DogField> selProperties = new Dictionary<string, DogField>();
        public readonly Dictionary<string, DogField> selFields = new Dictionary<string, DogField>();


        public class DogTable
        {
            public string tableName = "";
            public System.Type tableTpy;
            public List<DogField> fields = new List<DogField>();
            public List<DogField> XrefFromFld = new List<DogField>();  //campi che referenziano questa tabella
            //--
            public string Description = "";
            public string SqlTableName = "";
            public string SqlTableNameExt = "";
            public string SqlTableProperties = "";
            public string RowIdName = "";
            public string SqlRowIdName = "";
            public string SqlRowIdNameExt = "";
            public string SqlPrefix = "";
            public string SqlPrefixExt = "";
            public string SqlXdataTableName = "";
            public string SqlXdataTableNameExt = "";
            public string MODEL = ""; //Nome Modello es: SIO
            public string CATEG = ""; //Categoria Oggetto es: TAB=Table, SEL=Selection, ecc.
            public int INTCODE = 0; //Internal Table Code
            public string TBAREA = ""; //Table Area
            public string PREFIX = ""; //Table Prefix
            public string LIVEDESC = ""; //Table type: Live or Description
            public string IS_RELTABLE = ""; //Is Relation Table: Yes or No
        }
        public class DogField
        {
            public string fieldName = "";
            public System.Type fieldTyp;
            public DogTable table;
            //--
            public string SqlFieldName = "";  // eg: AV_CODICE
            public string SqlFieldProperties = ""; // eg: prop() xref() xdup(ATTIVITA.AV__ICODE[AV__ICODE] {AV_CODICE=' '}) multbxref()
            public string SqlFieldOptions = "";  // [UID] [XID] codice univoco utente e esterno
            public string SqlFieldNameExt = "";  // AY_CODE
            public string Xref = "";  // external reference (if any) eg: Pa1Icode
            public DogField XrefObj;  // external reference (if any) eg: Pa1Icode
            //--
            public bool optXREF = false;
            public bool optSID = false;
            public bool optUID = false;
            public bool optXID = false;
            public bool optDATE = false;
            public bool optTIME = false;
            public bool optDATETIME = false;
            public bool optBIGINT = false;
            //--
            public string Description;  
            public object? DefaultValue = null;  
            public int? StringLength = null;
        }

        internal DogManager(string modelName, string modelMode, DbTyp databaseType, string connectionStringName, string dbRoot, string dbHome)
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
            //set dog
            _modelName = modelName;
            _modelMode = modelMode;  //indica come interpretare il Modello. Se _modelMode == "FREE" allora il modello non prevede i campi standard _deleted _timestamp, ecc. e non gestisce le date come stringhe
            _databaseType = databaseType;
            _connectionStringName = connectionStringName;
            _dbRoot = dbRoot;
            _dbHome = dbHome;

            //-----------------------
            //Load Default Data Model
            //-----------------------
            // Ottieni tutti i tipi nell'assembly corrente, il cui namespace inizia con "Models"
            //////////var typesInNamespace = Assembly.GetExecutingAssembly().GetTypes()
            //////////    .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith(BASE_MODEL));

            //var typesInNamespace = ErpContext.Instance.AssemblyMODEL.GetTypes()
            //    .Where(t => t.IsClass && t.Namespace != null && t.Namespace.StartsWith(BASE_MODEL));

            // Ottieni tutti i tipi che hanno "Models" come seconda parte del namespace
            var typesInNamespace = ErpContext.Instance.AssemblyMODEL.GetTypes()
                .Where(t => t.IsClass && t.Namespace != null)
                .Where(t =>
                {
                    var parts = t.Namespace.Split('.');
                    return parts.Length > 1 && parts[1] == "Models";
                });


            foreach (var objType in typesInNamespace)
            {
                // Cerca le costanti MODELNAME e MODELTYPE
                var modName = objType.GetField("MODEL", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                var categName = objType.GetField("CATEG", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                if (modName != null && modName.IsLiteral && !modName.IsInitOnly && categName != null && categName.IsLiteral && !categName.IsInitOnly)
                {
                    string modNameVal = modName.GetValue(null)?.ToString().Trim() ?? ""; // null perché la costante è statica
                    string categNameVal = categName.GetValue(null)?.ToString().Trim() ?? ""; // null perché la costante è statica
                    if (modNameVal == modelName && categNameVal != "")
                    {
                        DogTable tab = new DogTable();
                        tab.tableName = objType.Name;
                        tab.tableTpy = objType;
                        //--
                        tab.Description = objType.GetField("Description")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableName = objType.GetField("SqlTableName")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableNameExt = objType.GetField("SqlTableNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableProperties = objType.GetField("SqlTableProperties")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.RowIdName = objType.GetField("RowIdName")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlRowIdName = objType.GetField("SqlRowIdName")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlRowIdNameExt = objType.GetField("SqlRowIdNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlPrefix = objType.GetField("SqlPrefix")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlPrefixExt = objType.GetField("SqlPrefixExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlXdataTableName = objType.GetField("SqlXdataTableName")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlXdataTableNameExt = objType.GetField("SqlXdataTableNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.MODEL = modNameVal;
                        tab.CATEG = categNameVal;
                        tab.INTCODE = Convert.ToInt32(objType.GetField("INTCODE")?.GetRawConstantValue());
                        tab.TBAREA = objType.GetField("TBAREA")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.PREFIX = objType.GetField("PREFIX")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.LIVEDESC = objType.GetField("LIVEDESC")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.IS_RELTABLE = objType.GetField("IS_RELTABLE")?.GetRawConstantValue()?.ToString() ?? "";
                        //---------
                        foreach (var property in objType.GetProperties())
                        {
                            ErpDogFieldAttribute? erpDogFieldAttribute = property.GetCustomAttribute(typeof(ErpDogFieldAttribute)) as ErpDogFieldAttribute;
                            if (erpDogFieldAttribute != null)
                            {
                                //---------
                                DogField fld = new DogField();
                                fld.fieldName = property.Name;
                                fld.fieldTyp = property.PropertyType;
                                fld.table = tab;
                                //--
                                fld.SqlFieldName = erpDogFieldAttribute.SqlFieldName?.ToString() ?? "";
                                fld.SqlFieldProperties = erpDogFieldAttribute.SqlFieldProperties?.ToString() ?? "";
                                fld.SqlFieldOptions = erpDogFieldAttribute.SqlFieldOptions?.ToString() ?? "";
                                fld.SqlFieldNameExt = erpDogFieldAttribute.SqlFieldNameExt?.ToString() ?? "";
                                fld.Xref = erpDogFieldAttribute.Xref?.ToString() ?? "";
                                //---------
                                fld.optXREF = String.IsNullOrWhiteSpace(fld.Xref) == false;
                                fld.optSID = fld.SqlFieldOptions.Contains("[SID]");
                                fld.optUID = fld.SqlFieldOptions.Contains("[UID]");
                                fld.optXID = fld.SqlFieldOptions.Contains("[XID]");
                                fld.optDATE = fld.SqlFieldOptions.Contains("[DATE]");
                                fld.optTIME = fld.SqlFieldOptions.Contains("[TIME]");
                                fld.optDATETIME = fld.SqlFieldOptions.Contains("[DATETIME]");
                                fld.optBIGINT = fld.SqlFieldOptions.Contains("[BIGINT]");
                                //---------
                                DisplayAttribute? displaydAttribute = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                                if (displaydAttribute != null)
                                {
                                    fld.Description = displaydAttribute.Description;
                                }
                                DefaultValueAttribute? defaultValueAttribute = property.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;
                                if (defaultValueAttribute != null)
                                {
                                    fld.DefaultValue = defaultValueAttribute.Value;
                                }
                                StringLengthAttribute? stringLengthAttribute = property.GetCustomAttribute(typeof(StringLengthAttribute)) as StringLengthAttribute;
                                if (stringLengthAttribute != null)
                                {
                                    fld.StringLength = stringLengthAttribute.MaximumLength;
                                }
                                //---------
                                tab.fields.Add(fld);
                                switch (categNameVal)
                                {
                                    case "TAB":
                                        tabProperties.Add(fld.fieldName, fld);
                                        tabFields.Add(fld.SqlFieldName, fld);
                                        break;
                                    case "SEL":
                                        selProperties.Add(fld.fieldName, fld);
                                        selFields.Add(fld.SqlFieldName, fld);
                                        break;
                                }
                            }
                        }
                        //-------
                        switch (categNameVal)
                        {
                            case "TAB":
                                tables.Add(tab.SqlTableName, tab);
                                tabTypes.Add(tab.tableTpy, tab);
                                tabPrefixes.Add(tab.SqlPrefix, tab);
                                tabIntcodes.Add(tab.INTCODE, tab);
                                break;
                            case "SEL":
                                selfilters.Add(tab.SqlTableName, tab);
                                selTypes.Add(tab.tableTpy, tab);
                                break;
                        }
                    }
                }
            }
            // carica XrefObj & XrefFromFld
            foreach (var fld in tabProperties.Values)
            {
                if (fld.optXREF)
                {
                    if (tabProperties.ContainsKey(fld.Xref)) { fld.XrefObj = tabProperties[fld.Xref]; tabProperties[fld.Xref].table.XrefFromFld.Add(fld); }  // il field deve esistere
                    else throw new ArgumentException($"Errore: impossibile creare db, legame campo Xref {fld.Xref} non presente ");
                }
            }
            //foreach (var fld in selProperties.Values)
            //{
            //    if (fld.optXREF)
            //    {
            //        if (selProperties.ContainsKey($"Sel{fld.Xref}")) fld.XrefObj = selProperties[$"Sel{fld.Xref}"]; // il field deve esistere (nota: SelXx1Icode ..non esiste)
            //        else throw new ArgumentException($"Errore: impossibile creare db, legame campo Xref Sel{fld.Xref} non presente ");
            //    }
            //}

        }
        ~DogManager()
        {
            Dispose();
        }
        public void Dispose()
        {
            // Rilascia risorse non gestite
            if (tables != null) { tables.Clear(); }
            if (tabTypes != null) { tabTypes.Clear(); }
            if (tabPrefixes != null) { tabPrefixes.Clear();  }
            if (tabIntcodes != null) { tabIntcodes.Clear();  }
            if (tabProperties != null) { tabProperties.Clear(); }
            if (tabFields != null) { tabFields.Clear(); }
            if (selfilters != null) { selfilters.Clear(); }
            if (selTypes != null) { selTypes.Clear(); }
            if (selProperties != null) { selProperties.Clear(); }
            if (selFields != null) { selFields.Clear(); }
            GC.SuppressFinalize(this);
        }

        //***************************************************************************************************************************************************
        //*** ModelErp UTIL
        //***************************************************************************************************************************************************

        //public

        public string? getRowIdName(ModelErp obj) { return (this.tabTypes.ContainsKey(obj.GetType())) ? this.tabTypes[obj.GetType()].RowIdName : null; }


        //***************************************************************************************************************************************************
        //*** ICODE
        //***************************************************************************************************************************************************

        //public

        public string GenerateIcode() { return $"{this._dbRoot}{GenCodeHelper.EpochIcode()}"; }


        //***************************************************************************************************************************************************
        //*** TRANSAZIONI
        //***************************************************************************************************************************************************

        //public

        public string BeginTransaction(string transactionId, string transactionName = "") { return _getDbMg().BeginTransaction(transactionId, transactionName); }
        public void CommitTransaction(string transactionId, string transactionName = "") { _getDbMg().CommitTransaction(transactionId, transactionName); }
        public void RollbackTransaction(string transactionId, string transactionName = "") { _getDbMg().RollbackTransaction(transactionId, transactionName); }


        //***************************************************************************************************************************************************
        //*** QUERY - MANTAIN
        //***************************************************************************************************************************************************

        //public

        // ExecuteScalar
        public bool RecordExists(string tableName, string keyField, object keyValue, string transactionId = null) 
        { 
            return _getDbMg().RecordExists(tableName, keyField, keyValue, transactionId); 
        }
        public byte[] ReadBlob(string tableName, string keyField, object keyValue, string blobField, int pageNumber, string transactionId = null)
        {
            return _getDbMg().ReadBlob(tableName, keyField, keyValue, blobField, pageNumber, transactionId);
        }
        public void WriteBlob(string tableName, string keyField, object keyValue, string blobField, byte[] data, int pageNumber, string transactionId = null)
        {
            _getDbMg().WriteBlob(tableName, keyField, keyValue, blobField, data, pageNumber, transactionId);
        }

        //ExecuteQuery
        public DataTable ExecuteQuery(string sql, IDictionary<string, object> parameters, string options = "", int maxRecords = 10000, string transactionId = null)
        {
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return DecodeSpecialTable(_getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), maxRecords, transactionId), options);
        }
        public List<T> ExecuteQuery<T>(string sql, IDictionary<string, object> parameters, string options = "", int maxRecords = 10000, string transactionId = null) 
        {
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (options.Contains("[skipCheckSqlParms]") == false && (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--"))) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return DecodeSpecialTable<T>(_getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), maxRecords, transactionId), options);
        }
        public Dictionary<object, ModelErp> ExecuteQuery(Dictionary<object, ModelErp> dict, System.Type modelType, string sql, IDictionary<string, object> parameters, string options = "", int maxRecords = 10000, string transactionId = null)
        {
            if (dict == null) dict = new Dictionary<object, ModelErp>();
            if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (options.Contains("[skipCheckSqlParms]") == false && (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--"))) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return DecodeSpecialTable(dict, modelType, _getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), maxRecords, transactionId), options);
        }

        //ExecNonQuery
        public void DeleteRecord(string tableName, string keyField, IDictionary<string, object> fields, string transactionId = null)
        {
            _getDbMg().DeleteRecord(tableName, keyField, fields, transactionId);
        }


        //***************************************************************************************************************************************************
        //*** IMPORT-EXPORT CSV
        //***************************************************************************************************************************************************

        //public

        public void ExportTableToCsv(string tableName, string filePath, string whereClause = null, int chunkSize = 10000)
        {
            _getDbMg().ExportTableToCsv(tableName, filePath, whereClause, chunkSize);
        }
        public void ImportCsvToTable(string tableName, string filePath)
        {
            _getDbMg().ImportCsvToTable(tableName, filePath);
        }

        //***************************************************************************************************************************************************
        //*** MANTAIN
        //***************************************************************************************************************************************************


        public void MantainRecord(char action, string tableName, string keyField, string timestampField, string deleteField, IDictionary<string, object> parameters, string options, string transactionId = null)
        {
            _getDbMg().MantainRecord(action, tableName, keyField, timestampField, deleteField, EncodeSpecialFields(parameters, options), options, transactionId);
        }


        //***************************************************************************************************************************************************
        //*** ENCODE-DECODE
        //***************************************************************************************************************************************************

        private Dictionary<string, object> EncodeSpecialFields(IDictionary<string, object> fields, string options="")
        {
            String Key ="", Value="";
            try { 
                if (fields == null) return null;
                var parameters = new Dictionary<string, object>();
                foreach (KeyValuePair<string, object>  field in fields) { Key = field.Key; Value = field.Value?.ToString() ?? ""; parameters[field.Key] = EncodeSpecialField(field.Value, options); }
                return parameters;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"EncodeSpecialFields[{Key}={Value}]: {ex.Message}.");
            }

        }
        private DataTable DecodeSpecialTable(DataTable dataTable, string options = "")  //DecodeSpecialFields
        {
            String Key = "", Value = "";
            try
            { 
                if (dataTable == null) return null;
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        Key = column.ColumnName; Value = row[column]?.ToString() ?? "";
                        row[column] = DecodeSpecialField(null, column.ColumnName, row[column], options + " [ToDataRow]"); // in caso di DataRow => uso DBNull
                        Key = ""; Value = "";
                    }
                }
                return dataTable;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialTable[{Key}={Value}]: {ex.Message}.");
            }
        }
        public List<T> DecodeSpecialTable<T>(DataTable dt, string options = "")
        {
            try { 
                if (dt == null) return null;
                List<T> data = new List<T>();
                foreach (DataRow row in dt.Rows)
                {
                    T item = DecodeSpecialRow<T>(row, options);
                    data.Add(item);
                }
                return data;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialTable<T>: {ex.Message}.");
            }
        }
        public Dictionary<object, ModelErp> DecodeSpecialTable(Dictionary<object, ModelErp> dict, System.Type modelType, DataTable dt, string options = "")
        {
            if (dict == null) dict = new Dictionary<object, ModelErp>();
            if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
            try
            {
                if (dt == null) return dict;
                // Usa il tipo dell'oggetto per chiamare la funzione DecodeSpecialRow generica
                MethodInfo method_DecodeSpecialRow = typeof(DogManager).GetMethod("DecodeSpecialRow", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(modelType);
                //--
                foreach (DataRow row in dt.Rows)
                {
                    ModelErp item = (ModelErp)method_DecodeSpecialRow.Invoke(this, new object[] { row, options }); // ModelErp item = DecodeSpecialRow<ModelErp>(row, options);
                    if (item != null && item.getIcode() != null) dict[item.getIcode()] = item;
                }
                return dict;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialTable[ModelErp]: {ex.Message}.");
            }
        }
        public T DecodeSpecialRow<T>(DataRow dr, string options = "")
        {
            String Key = "", Value = "";
            try
            {
                System.Type temp = typeof(T);
                //decode in object array
                if (temp == typeof(System.Object[]))
                {
                    object[] objArr = new object[dr.Table.Columns.Count];
                    //foreach (DataColumn column in dr.Table.Columns)
                    for (int i = 0; i < dr.Table.Columns.Count; i++)
                    {
                        DataColumn column = dr.Table.Columns[i];
                        Key = column.ColumnName; Value = dr[column.ColumnName]?.ToString() ?? "";
                        objArr[i] = DecodeSpecialField(null, column.ColumnName, dr[column.ColumnName], options);
                        Key = ""; Value = "";
                    }
                    return (T)(Object)objArr;
                }
                //decode in object model
                T obj = Activator.CreateInstance<T>();
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    DataColumn column = dr.Table.Columns[i];
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName)
                        {
                            Key = column.ColumnName; Value = dr[column.ColumnName]?.ToString() ?? "";
                            pro.SetValue(obj, DecodeSpecialField(pro.PropertyType, column.ColumnName, dr[column.ColumnName], options), null);
                            Key = ""; Value = "";
                        }
                    }
                }
                return obj;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialRow<T>[{Key}={Value}]: {ex.Message}.");
            }
        }

        //-----------------------------------

        //CODIFICHE: da STRUTTURA a DB
        private object EncodeSpecialField(object value, string options = "")
        {
            try
            {
                value = UtilHelper.DecodeJsonElement(value); //Decodifica tipi JSON, se le variabili vengono da pagina web

                //converti in tipi standard
                if (_modelMode == "FREE")
                {
                    // conversioni minime valide per tutti i DB
                    if (value == null)
                    {
                        return (object)System.DBNull.Value;  // ATTENZIONE!! non devo eliminare eventuali bianchi alla fine
                    }
                    if (value is string str)
                    {
                        if (str == "") str = DB_STRING_EMPTY;  //in caso di stringa vuota devo sbianchettare
                        return str;  // ATTENZIONE!! non devo eliminare eventuali bianchi alla fine
                    }
                    if (value is DateOnly date)
                    {
                        return date.ToDateTime(new TimeOnly(0, 0, 1, 0));  //fisso l'ora al primo secondo del giorno
                    }
                    if (value is TimeOnly time)
                    {
                        return time.ToTimeSpan();
                    }
                    if (value is DateTime datetime)
                    {
                        return datetime;
                    }

                    return value;
                }
                else
                {
                    if (value is string str)
                    {
                        if (str == "") str = DB_STRING_EMPTY;  //in caso di stringa vuota devo sbianchettare
                        return str;  // ATTENZIONE!! non devo eliminare eventuali bianchi alla fine
                    }
                    if (value is DateOnly date)
                    {
                        if (date.Equals(DateOnly.MinValue)) return DB_DATE_MIN;
                        else if (date.Equals(DateOnly.MaxValue)) return DB_DATE_MAX;
                        else return date.ToString(DB_FORMAT_DATE);
                    }
                    if (value is TimeOnly time)
                    {
                        if (time == default) return DB_TIME_EMPTY;
                        else return time.ToString(DB_FORMAT_TIME);
                    }
                    if (value is DateTime datetime)
                    {
                        return datetime.ToString(DB_FORMAT_DATETIME);
                    }
                    // Aggiungere altre conversioni speciali qui se necessario
                    return value;
                }

            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"EncodeSpecialField[{value?.ToString() ?? ""}]: Errore nella codifica del campo -- {ex.Message}.");
            }
        }

        //CODIFICHE: da DB a STRUTTURA (type = tipo campo in struttura [string/short/int/long/double/DateOnly/TimeOnly/DateTime/byte[]/bool])
        private object DecodeSpecialField(System.Type type, string colName, object value, string options = "")
        {
            try
            {
                if (value == null || value.GetType() == typeof(System.DBNull))
                {
                    if (options.Contains("[ToDataRow]")) return (object)System.DBNull.Value;  // se carico DataRow => uso DBNull
                    else return null;       // se carico la struttura => uso null;
                }
                if (value.GetType() == typeof(string))
                {
                    string strVal = ((string)value).Trim();
                    if (type == typeof(DateOnly?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optDATE == true))
                    {
                        if (strVal == "" || strVal == "/  /" || strVal == DB_DATE_MIN) return DateOnly.MinValue;
                        if (strVal == DB_DATE_MAX) return DateOnly.MaxValue;
                        if (DateOnly.TryParseExact((string)value, DB_FORMAT_DATE, null, DateTimeStyles.None, out DateOnly date)) return date;
                    }
                    if (type == typeof(TimeOnly?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optTIME == true))
                    {
                        if (strVal == "" || strVal == ":  :" || strVal == DB_TIME_EMPTY) return null;
                        if (TimeOnly.TryParseExact(value.ToString(), DB_FORMAT_TIME, null, DateTimeStyles.None, out TimeOnly time)) return time;
                    }
                    if (type == typeof(DateTime?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optDATETIME == true))
                    {
                        if (strVal == "" || strVal == "/  /" || strVal == "/  /     :  :") return DateTime.MinValue;
                        if (this.tabFields[colName]?.optDATE == true && DateTime.TryParseExact(value.ToString(), DB_FORMAT_DATE, null, DateTimeStyles.None, out DateTime datetimeDate)) return datetimeDate;
                        else if (this.tabFields[colName]?.optTIME == true && DateTime.TryParseExact(value.ToString(), DB_FORMAT_TIME, null, DateTimeStyles.None, out DateTime datetimeTime)) return datetimeTime;
                        else if (DateTime.TryParseExact(value.ToString(), DB_FORMAT_DATETIME, null, DateTimeStyles.None, out DateTime datetime)) return datetime;
                    }
                }
                if (value.GetType() == typeof(System.DateTime) || value.GetType() == typeof(System.DateTime?))
                {
                    if (type == typeof(DateOnly?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optDATE == true))
                    {
                        DateOnly dt = DateOnly.FromDateTime((DateTime)value); return dt;
                    }
                    if (type == typeof(DateTime?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optDATETIME == true))
                    {
                        return (DateTime?)value;
                    }
                }
                if (value.GetType() == typeof(System.TimeSpan) || value.GetType() == typeof(System.TimeSpan?))
                {
                    if (type == typeof(TimeOnly?) || (this.tabFields.ContainsKey(colName) && this.tabFields[colName]?.optTIME == true))
                    {
                        TimeOnly tm = TimeOnly.FromTimeSpan((TimeSpan)value); return tm;
                    }
                }
                //---
                if (type == typeof(System.String)) { string str = Convert.ToString(value) ?? ""; return str.TrimEnd(); }  //string
                if (type == typeof(System.Int16?)) { short shr = Convert.ToInt16(value); if (shr == DogManager.DB_SHORT_EMPTY) return null; else return shr; }  //short
                if (type == typeof(System.Int32?)) { int integer = Convert.ToInt32(value); if (integer == DogManager.DB_LONG_EMPTY) return null; else return integer; }  //long
                if (type == typeof(System.Int64?)) { long lng = Convert.ToInt64(value); if (lng == DogManager.DB_LONG_EMPTY) return null; else return lng; }  //bigint
                if (type == typeof(System.Double?)) { double dbl = Convert.ToDouble(value); if (dbl == DogManager.DB_DOUBLE_EMPTY) return null; else return dbl; }  //double
                if (type == typeof(System.Byte[])) { return value; }  //byte[]   ?????????????????????????????
                if (type == typeof(System.Boolean?)) { bool b = Convert.ToBoolean(value); return b; }  //bool

                // Aggiungere altre conversioni speciali qui se necessario
                return value;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialField[{colName}={value?.ToString() ?? ""}]: Errore nella decodifica del campo -- {ex.Message}.");
            }
        }


        //***************************************************************************************************************************************************
        //*** List - Row - Add - Upd
        //***************************************************************************************************************************************************


        ////carica list oggetti con il contenuto del DB in base alla struttura in selezione  
        //public List<T> List<T>(object selModel, string options = "") where T : ModelErp
        //{
        //    if (selModel == null) { throw new ArgumentNullException(nameof(selModel)); }
        //    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        //    IDictionary<string, object> parameters = new Dictionary<string, object>();
        //    StringBuilder sb = new StringBuilder(); List<T> outList;

        //    string sqlFromWhere = objModel.ViewQueryFromWhere();
        //    if (string.IsNullOrEmpty(sqlFromWhere))
        //    {

        //        sb.Append(DogManagerInt.sqlSelect(this, objModel, ref parameters))
        //            .Append(DogManagerInt.sqlFrom(this, objModel, ref parameters))
        //            .Append(DogManagerInt.sqlWhere(this, selModel, ref parameters));
        //        //access DB
        //        outList = this.ExecuteQuery<T>(sb.ToString(), parameters);

        //    } 
        //    else
        //    {
        //        sb.Append("SELECT * FROM ( \n")
        //            .Append(DogManagerInt.sqlSelect(this, objModel, ref parameters))
        //            .Append(sqlFromWhere)
        //            .Append(") AS subquery \n")
        //            .Append(DogManagerInt.sqlWhere(this, selModel, ref parameters, options: "[UsePropertyNameField]")); // Componi il filtro dinamicamente
        //        string sql = DogManagerInt.replaceSqlTextWithPlaceholders(sb.ToString(), ref parameters);  // elimino le stringhe esplicite dalla query
        //        //access DB
        //        outList = this.ExecuteQuery<T>(sql, parameters);  //this.ExecuteQuery<T>(sql, parameters, options: "[skipCheckSqlParms]");
        //    }
        //    return outList;

        //}


        ////carica list oggetti con il contenuto del DB in base alla struttura in selezione  
        //public List<T> List__OLD<T>(object selModel, string options = "") where T : ModelErp
        //{
        //    if (selModel == null) { throw new ArgumentNullException("Null " + nameof(selModel)); }
        //    return List_int__OLD<T>(selModel, null, options);
        //}
        ////carica list oggetti con il contenuto del DB in base alla lista icode'  
        //public List<T> List__OLD<T>(List<object> lstRowId, string options = "") where T : ModelErp
        //{
        //    if (lstRowId == null) { throw new ArgumentNullException("Null " + nameof(lstRowId)); }
        //    if (lstRowId.Count() == 0) { throw new ArgumentNullException("Empty " + nameof(lstRowId)); }
        //    return List_int__OLD<T>(null, lstRowId, options);
        //}
        ////carica row con il contenuto del DB in base all'icode'  
        //public T Row__OLD<T>(object icode, string options = "") where T : ModelErp
        //{
        //    if (UtilHelper.IsNullOrEmptyObject(icode)) { throw new ArgumentNullException(nameof(icode)); }
        //    List<T> outList = List_int__OLD<T>(null, new List<object>() { icode }, options);
        //    if (outList.Count() == 0) throw new DatabaseException(ERR_DB_BAD_IDEN, $"Nessun record corrispondente alla Chiave Primaria specificata [{icode}].", null);
        //    else if (outList.Count() > 1) throw new DatabaseException(ERR_DB_AMBIGOUS, $"a Chiave Primaria specificata è ambiqua. Più di un record trovato  [{icode}].", null);
        //    return outList[0];
        //}

        //private List<T> List_int__OLD<T>(object selModel, List<object> lstRowId, string options = "") where T : ModelErp
        //{
        //    List<T> outList;
        //    if (selModel == null && lstRowId == null) { throw new ArgumentNullException(nameof(selModel) + " - " + nameof(lstRowId)); }
        //    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        //    IDictionary<string, object> parameters = new Dictionary<string, object>();

        //    string sql = sqlList(objModel, ref parameters, selModel, lstRowId, options);
        //    outList = this.ExecuteQuery<T>(sql, parameters, options: options);

        //    if (options.Contains("[PLAIN]") == false)  //if (options.Contains("[DecodeLabels]"))
        //    {
        //        if (this.tabTypes.ContainsKey(objModel.GetType()))
        //        {
        //            DogTable tab = this.tabTypes[objModel.GetType()];
        //            foreach (var fld in tab.fields)
        //            {
        //                try
        //                {
        //                    var xrefObj = fld?.XrefObj;
        //                    if (xrefObj == null) continue; //per applicare la condizione la proprietà deve avere un attributo [ErpDogField(..)]
        //                    string propertyName = fld.fieldName; // Get property name and value

        //                    // Usa il tipo dell'oggetto per chiamare la funzione LIST_int generica
        //                    MethodInfo method = typeof(DogManager).GetMethod("List_int__OLD", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(xrefObj.table.tableTpy);
        //                    object propList = (IEnumerable)method.Invoke(this, new object[] { null, (List<object>)outList.Select(s => objModel.GetType().GetProperty(propertyName).GetValue(s)).ToList<object>(), "[PLAIN] " + options }); // non ricorsivo
        //                    // Carico i risultati della tabella collegata
        //                    Dictionary<object, ModelErp> propDict = new Dictionary<object, ModelErp>();
        //                    foreach (var item in (IEnumerable)propList) { propDict.Add(((ModelErp)item).getIcode(), (ModelErp)item); } // Salvo i risultati della tabella collegata in un dizionario
        //                    foreach (T rec in outList) // Assegno il record della tabella collegata ad ogni riga della tabella principale (campo + "Obj")
        //                    {
        //                        try { rec.GetType().GetProperty(propertyName + "Obj").SetValue(rec, propDict[rec.GetType().GetProperty(propertyName).GetValue(rec)]); }
        //                        catch (Exception ex) { }  //skip exceptions (salta se la chiave non è valorizzata 
        //                    }
        //                }
        //                catch (Exception ex) { }  //skip exceptions
        //            }
        //        }


        //        ////ciclo sulle proprietà
        //        //System.Type type = objModel.GetType();
        //        //foreach (var property in type.GetProperties())
        //        //{
        //        //    try
        //        //    {
        //        //        string propertyName = property.Name; // Get property name and value
        //        //        if (!this.tabProperties.ContainsKey(propertyName)) continue; //per applicare la condizione la proprietà deve avere un attributo [ErpDogField(..)]
        //        //        var fld = this.tabProperties[propertyName];
        //        //        var xrefObj = fld?.XrefObj;
        //        //        if (xrefObj == null) continue; //per applicare la condizione la proprietà deve avere un attributo [ErpDogField(..)]

        //        //        // Usa il tipo dell'oggetto per chiamare la funzione LIST_int generica
        //        //        MethodInfo method = typeof(DogManager).GetMethod("List_int", BindingFlags.NonPublic | BindingFlags.Instance).MakeGenericMethod(xrefObj.table.tableTpy);
        //        //        object propList =(IEnumerable)method.Invoke(this, new object[] { null, (List<object>)outList.Select(s => property.GetValue(s)).ToList<object>(), "" });
        //        //        // Carico i risultati della tabella collegata
        //        //        Dictionary<object, ModelErp> propDict = new Dictionary<object, ModelErp>();
        //        //        foreach (var item in (IEnumerable)propList) { propDict.Add(((ModelErp)item).getIcode(), (ModelErp)item); } // Salvo i risultati della tabella collegata in un dizionario
        //        //        foreach (T rec in outList) { rec.GetType().GetProperty(propertyName + "Obj").SetValue(rec, propDict[property.GetValue(rec)]); }  // Assegno il record della tabella collegata ad ogni riga della tabella principale (campo + "Obj")
        //        //    }
        //        //    catch (Exception ex) { }  //skip exceptions
        //        //}
        //    }
        //    return outList;
        //}



        //carica list oggetti con il contenuto del DB in base alla struttura in selezione  
        public List<T> List<T>(object selModel, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return List<T>(selModel, null, ref dogCache, options);  }
        public List<T> List<T>(object selModel, List<string> xrefFrom, ref DogCache dogCache, string options = "") where T : ModelErp
        {
            if (selModel == null) { throw new ArgumentNullException("Null " + nameof(selModel)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            return List_int<T>(selModel, null, xrefFrom, ref dogCache, options);
        }
        public List<T> List<T>(List<object> lstRowId, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return List<T>(lstRowId, null, ref dogCache, options); }
        public List<T> List<T>(List<object> lstRowId, List<string> xrefFrom, ref DogCache dogCache, string options = "") where T : ModelErp
        {
            if (lstRowId == null) { throw new ArgumentNullException("Null " + nameof(lstRowId)); }
            if (lstRowId.Count() == 0) { throw new ArgumentNullException("Empty " + nameof(lstRowId)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            return List_int<T>(null, lstRowId, xrefFrom, ref dogCache, options);
        }
        //carica row con il contenuto del DB in base all'icode'  
        public T Row<T>(object icode, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return Row<T>(icode, null, ref dogCache, options); }
        public T Row<T>(object icode, List<string> xrefFrom, ref DogCache dogCache, string options = "") where T : ModelErp
        {
            if (UtilHelper.IsNullOrEmptyObject(icode)) { throw new ArgumentNullException(nameof(icode)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            List<T> outList = List_int<T>(null, new List<object>() { icode }, xrefFrom, ref dogCache, options);
            if (outList.Count() == 0) throw new DatabaseException(ERR_DB_BAD_IDEN, $"Nessun record corrispondente alla Chiave Primaria specificata [{icode}].", null);
            else if (outList.Count() > 1) throw new DatabaseException(ERR_DB_AMBIGOUS, $"a Chiave Primaria specificata è ambiqua. Più di un record trovato  [{icode}].", null);
            return outList[0];
        }
        private List<T> List_int<T>(object selModel, List<object> lstRowId, List<string> xrefFrom, ref DogCache dogCache, string options = "") where T : ModelErp
        {
            List<object> outKeyList;
            if (selModel == null && lstRowId == null) { throw new ArgumentNullException(nameof(selModel) + " - " + nameof(lstRowId)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            if (xrefFrom == null) { xrefFrom = new List<string>(); }
            T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
            IDictionary<string, object> parameters = new Dictionary<string, object>();

            string sql = sqlList(objModel, ref parameters, selModel, null, lstRowId, options);

            //init
            CacheFuncInit(ref dogCache, "List_int", 'R', objModel.GetType(), options: options); // Inizializzo la cache per il tipo di oggetto, in modo da poterla usare per le query successive.

            //outList = this.ExecuteQuery<T>(sql, parameters, options: options);
            Dictionary<object, ModelErp> outDict = this.ExecuteQuery(null, objModel.GetType(), sql, parameters, options: options);  //dict contiene una copia di tutti i record estratti in tutte le sessioni
            outKeyList = CacheAddDict(ref dogCache, objModel.GetType(), outDict, options: options);

            // se richiestro riempio i riferimenti all'oggetto referenziati nelle tabelle esterne
            if (xrefFrom.Count > 0)
            {
                dogCache.AddRuleXrefFrom(xrefFrom); // Aggiungo le regole di xrefFrom al DogCache per ricalcolare le relazioni in fase di ricostruzione dei legami della cache (ie: CacheFillNull()).
                foreach (var xrefFromPropertyName in xrefFrom)
                {
                    DogField fld = this.tabProperties[xrefFromPropertyName];
                    if (fld?.XrefObj?.table?.tableTpy != objModel.GetType()) continue;
                    System.Type xrefFromType = fld?.table?.tableTpy;
                    if (xrefFromType == null) continue;

                    ModelErp xrefFromObj = (ModelErp)Activator.CreateInstance(xrefFromType); // create an instance of that type
                    IDictionary<string, object> xrefFromParameters = new Dictionary<string, object>();
                    string xrefFromSql = sqlList(xrefFromObj, ref xrefFromParameters, null, fld, outKeyList, options);
                    Dictionary<object, ModelErp> outDictFrom = ExecuteQuery(null, xrefFromType, xrefFromSql, xrefFromParameters, options: options);
                    //carico nella cache i riferimenti per ogni record della lista
                    CacheAddDict(ref dogCache, xrefFromObj.GetType(), outDictFrom); // salvo i record estratti in cache
                }
            }
            return CacheFillNull<T>(ref dogCache, outKeyList, options: options); 
        }
        private string sqlList(ModelErp objModel, ref IDictionary<string, object> parameters, object selModel, DogField fldXref, List<object> lstRowId, string options = "")
        {
            string sql = "";
            if (selModel == null && lstRowId == null) { throw new ArgumentNullException(nameof(selModel) + " - " + nameof(lstRowId)); }
            StringBuilder sb = new StringBuilder();

            string sqlFromWhere = objModel.ViewQueryFromWhere();
            if (string.IsNullOrEmpty(sqlFromWhere))
            {
                sb.Append(DogManagerInt.sqlSelect(this, objModel, ref parameters))
                    .Append(DogManagerInt.sqlFrom(this, objModel, ref parameters));
                if (selModel == null && fldXref == null) sb.Append(DogManagerInt.sqlWhereListIcode(this, objModel, lstRowId, ref parameters, options: options));  //lista icode
                else if (selModel == null) sb.Append(DogManagerInt.sqlWhereListXref(this, objModel, fldXref, lstRowId, ref parameters, options: options));  //lista icode
                else sb.Append(DogManagerInt.sqlWhereSelection(this, selModel, ref parameters, options: options));  //filtro parametri
                sql = sb.ToString();
            }
            else
            {
                sb.Append("SELECT * FROM ( \n")
                    .Append(DogManagerInt.sqlSelect(this, objModel, ref parameters))
                    .Append(sqlFromWhere)
                    .Append(") AS subquery \n");
                if (selModel == null && fldXref == null) sb.Append(DogManagerInt.sqlWhereListIcode(this, objModel, lstRowId, ref parameters, options: "[UsePropertyNameField] " + options));  //lista icode
                if (selModel == null) sb.Append(DogManagerInt.sqlWhereListXref(this, objModel, fldXref, lstRowId, ref parameters, options: "[UsePropertyNameField] " + options));  //lista icode
                else sb.Append(DogManagerInt.sqlWhereSelection(this, selModel, ref parameters, options: "[UsePropertyNameField] " + options)); // Componi il filtro dinamicamente
                sql = DogManagerInt.replaceSqlTextWithPlaceholders(sb.ToString(), ref parameters);  // elimino le stringhe esplicite dalla query
            }
            return sql;
        }




        ////salva su DB modifiche e nuovi record  
        //public DogResult Mnt<T>(T tablModel, string options = "", string transactionId = null) {
        //    List<object> tabModels = new List<object>() { tablModel };
        //    List<DogResult> dogResults = MntList(tabModels, options, transactionId);
        //    return dogResults.First(); 
        //}

        //public List<DogResult> MntList(List<object> tabModels, string options = "", string transactionId = null)
        //{
        //    if (tabModels == null) { throw new ArgumentNullException(nameof(tabModels)); }
        //    List<DogResult> results = new List<DogResult>();
        //    IDictionary<string, object> parameters = new Dictionary<string, object>();
        //    StringBuilder sb = new StringBuilder();
        //    foreach (var tab in tabModels)
        //    {
        //        if (tab == null) { throw new ArgumentNullException(nameof(tab)); }
        //        sb.Append(DogManagerInt.sqlMantain(this, tab, ref parameters, ref results)).AppendLine("; ");

        //    }
        //    //access DB
        //    string sql = sb.ToString(); 
        //    if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
        //    int affectedRows = _getDbMg().ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);
        //    if (affectedRows != results.Count()) throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non valido o errore in insert/update.", null);
            
        //    //se necessario rileggo i timestamp
        //    if (_databaseType == DbTyp.SqlServer || _databaseType == DbTyp.Sybase)
        //    {
        //        IDictionary<string, object> parametersIcodeTimestamp = new Dictionary<string, object>();
        //        string sqlIcodeTimestamp = DogManagerInt.sqlSelectIcodeTimestamp(this, results, ref parametersIcodeTimestamp);
        //        DataTable dtIcodeTimestamp = _getDbMg().ExecuteQuery(sqlIcodeTimestamp, EncodeSpecialFields(parametersIcodeTimestamp, options), results.Count(), transactionId);
        //        for(int i=0; i < results.Count(); i++)
        //        {
        //            var row = dtIcodeTimestamp.AsEnumerable().FirstOrDefault(r => r.Field<string>("ICODE").Equals(results[i].Icode)); // Cerca la riga con ICODE uguale a results[i].Icode
        //            if (row == null) throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non trovato o errore in insert/update.", null);
        //            results[i].Timestamp = row.Field<byte[]>("TIMESTAMP");
        //        }
        //    }
            
        //    return results;
        //}


        //salva su DB modifiche e nuovi record  
        public DogResult Mnt<T>(T tablModel, string options = "", string transactionId = null) where T : ModelErp
        {
            List<ModelErp> tabModels = new List<ModelErp>() { tablModel };
            List<DogResult> dogResults = MntList(tabModels, options, transactionId);
            return dogResults.First();
        }

        public List<DogResult> MntList(ref DogCache dogCache, string options = "", string transactionId = null)
        {
            List<DogResult> results = new List<DogResult>();
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }

            // Scorri il dizionario esterno
            List<ModelErp> tabModels = new List<ModelErp>();   // Lista vuota dove raccogliere tutti i ModelErp da processare
            foreach (var kvpTipo in dogCache.dbCache)
            {
                // Scorri il dizionario interno
                foreach (var kvpOggetto in kvpTipo.Value)
                {
                    ModelErp model = kvpOggetto.Value;
                    //if (model != null && model.action != null && "AMD".Contains(model.action.ToString())) tabModels.Add(model);
                    if (model != null && model.orderMnt() >= 0) tabModels.Add(model);  //se orderMnt() == -1 => non devo forzare aggiornamento su DB
                }
            }
            // Ordina la lista in base alla proprietà getOrd()
            tabModels.Sort((a, b) => a.orderMnt().CompareTo(b.orderMnt()));

            // genera la query di mantenimento e agiorna DB
            List<DogResult> dogResults = MntList(tabModels, options, transactionId);

            //resetta il flag di aggiornamento su DB dai record della cache
            foreach(var model in tabModels) { model.resetMnt(); }

            return dogResults;
        }

        public List<DogResult> MntList(List<ModelErp> tabModels, string options = "", string transactionId = null)
        {
            if (tabModels == null) { throw new ArgumentNullException(nameof(tabModels)); }
            List<DogResult> results = new List<DogResult>();
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder sb = new StringBuilder();
            foreach (var tab in tabModels)
            {
                if (tab == null) { throw new ArgumentNullException(nameof(tab)); }
                sb.Append(DogManagerInt.sqlMantain(this, tab, ref parameters, ref results)).AppendLine("; ");

            }
            //access DB
            string sql = sb.ToString();
            if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            int affectedRows = _getDbMg().ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);
            if (affectedRows != results.Count()) throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non valido o errore in insert/update.", null);

            //se necessario rileggo i timestamp
            if (_databaseType == DbTyp.SqlServer || _databaseType == DbTyp.Sybase)
            {
                IDictionary<string, object> parametersIcodeTimestamp = new Dictionary<string, object>();
                string sqlIcodeTimestamp = DogManagerInt.sqlSelectIcodeTimestamp(this, results, ref parametersIcodeTimestamp);
                DataTable dtIcodeTimestamp = _getDbMg().ExecuteQuery(sqlIcodeTimestamp, EncodeSpecialFields(parametersIcodeTimestamp, options), results.Count(), transactionId);
                for (int i = 0; i < results.Count(); i++)
                {
                    var row = dtIcodeTimestamp.AsEnumerable().FirstOrDefault(r => r.Field<string>("ICODE").Equals(results[i].Icode)); // Cerca la riga con ICODE uguale a results[i].Icode
                    if (row == null) throw new DatabaseException(ERR_DB_TIMESTAMP, "Timestamp non trovato o errore in insert/update.", null);
                    results[i].Timestamp = row.Field<byte[]>("TIMESTAMP");
                }
            }

            return results;
        }









        ////***************************************************************************************************************************************************
        ////*** View
        ////***************************************************************************************************************************************************


        ////carica list oggetti con il contenuto del DB in base alla struttura in selezione  

        //// es:  List<VistaPaziente> vistaPaziente = ErpContext.Instance.DogFactory.GetDog(dogId).View<VistaPaziente>(selobj);

        //public List<T> View<T>(object selModel) where T : ModelErp
        //{
        //    if (selModel == null) { throw new ArgumentNullException(nameof(selModel)); }
        //    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        //    string sqlFromWhere = objModel.ViewQuery();
        //    if (string.IsNullOrEmpty(sqlFromWhere)) { throw new ArgumentNullException("View: empty sql"); }

        //    IDictionary<string, object> parameters = new Dictionary<string, object>();
        //    StringBuilder sb = new StringBuilder()
        //        .Append("SELECT * FROM ( \n")
        //        .Append(DogManagerInt.sqlSelect(this, objModel, ref parameters))
        //        .Append(sqlFromWhere)
        //        .Append(") AS subquery \n")
        //        .Append(DogManagerInt.sqlWhere(this, selModel, ref parameters)); // Componi il filtro dinamicamente
        //    //access DB
        //    return this.ExecuteQuery<T>(sb.ToString(), parameters, options: "[skipCheckSqlParms]");
        //}



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
        public void CacheFuncInit(ref DogCache dogCache, string serviceName, char serviceAction, System.Type serviceTabType, string options = "")
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
        public List<object> CacheAddDict(ref DogCache dogCache, System.Type objType, Dictionary<object, ModelErp> outDict, string options = "")
        {
            // Recupera il dizionario finale esistente o inizializza uno nuovo
            if (!dogCache.dbCache.TryGetValue(objType, out var dictFinale)) { dictFinale = new Dictionary<object, ModelErp>(); }

            //Hai Un dizionario esistente (Dictionary<object, ModelErp> dictFinale) e Un dizionario sorgente (Dictionary<object, List<ModelErp>> dizionario)
            //Vuoi usare le chiavi di dizionario come riferimento: (1) Se la chiave esiste in dizionarioFinale, aggiorni il valore (2) Se non esiste, la aggiungi

            // Unisci i dati: aggiorna o aggiungi
            foreach (var kv in outDict) { if (kv.Value != null) { kv.Value.addToCache(ref dogCache); dictFinale[kv.Key] = kv.Value; } }

            // Salva nella cache
            dogCache.dbCache[objType] = dictFinale;

            // Inserisci nella cache i riferimenti agli oggetti referenziati (XrefObj) se non sono già presenti
            // Integra in Cache l'elenco degli oggetti icodeXref referenziati nella lista "outList". Ad ogni Chiave Icode viene associato un Valore null. 
            if (options.Contains("[PLAIN]") == false)  //if (options.Contains("[DecodeLabels]"))
            {
                //Hai Un dizionario esistente (Dictionary<object, ModelErp> dictFinale) e Un dizionario sorgente (Dictionary<object, List<ModelErp>> dizionario)
                //Vuoi usare le chiavi di dizionario come riferimento: (1) Se la chiave esiste in dizionarioFinale, salti (2) Se non esiste, la aggiungi la chiave Icode con un valore null (questo sarà valorizzato successivamente)

                // Unisci i dati: aggiungi solo se non esiste, per tutti i riferimenti a campi Xref presenti nel modello
                if (this.tabTypes.ContainsKey(objType))
                {
                    DogTable tab = this.tabTypes[objType];

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
                                if (icodeXref == null) continue; // Salta se il valore della chiave è null
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
            return dictFinale.Keys.ToList<object>(); 
        }

        // Integra in Cache tutti i riferimenti a Chiave Icode con Valore null.
        // A fine processo effettua l'abbinamento dei riferimenti Xref per tutti i record presenti nella Cache
        public List<T> CacheFillNull<T>(ref DogCache dogCache, List<object> mainObjKeyList, string options = "")
        {
            T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
            return CacheFillNull(ref dogCache, objModel.GetType(), mainObjKeyList, options).OfType<T>().ToList(); //  OfType<T>() : filtra e fa cast solo se possibile (cioè solo se tipo T, atrimenti scarta la struttura);
        }
        public List<ModelErp> CacheFillNull(ref DogCache dogCache, System.Type mainObjType, List<object> mainObjKeyList, string options = "")
        {
            int recursiveCicle = 0; bool mustAddRecursiveObj = false;
            do {
                //----------------------------------------------------------
                recursiveCicle++; mustAddRecursiveObj = false;
                if (recursiveCicle > 100) { throw new IndexOutOfRangeException(nameof(recursiveCicle)); }
                // ----------------------------------------------------------
                //riempi i valori degli oggetti con Valore null presenti in Cache 
                foreach (var objType in dogCache.dbCache.Keys)
                {
                    ModelErp obj = (ModelErp)Activator.CreateInstance(objType); // create an instance of that type
                    IDictionary<string, object> objParameters = new Dictionary<string, object>();
                    string objSql = sqlList(obj, ref objParameters, null, null, dogCache.dbCache[objType].Keys.ToList<object>(), options);
                    //dogCache.dbCache[objType] = this.ExecuteQuery(dogCache.dbCache[objType], objType, objSql, objParameters, "[PLAIN] " + options); // non ricorsivo ?????
                    Dictionary<object, ModelErp> outDict = this.ExecuteQuery(dogCache.dbCache[objType], objType, objSql, objParameters, options);
                    foreach (var kv in outDict) { if (kv.Value != null) { kv.Value.addToCache(ref dogCache); dogCache.dbCache[objType][kv.Key] = kv.Value; } } //aggiorna o aggiungi alla cache
                }
                // ----------------------------------------------------------
                //riassegno a tutti i record della Cache il riferimento agli oggetto referenziati (ie: per tutte le tabelle presenti nella cache
                Dictionary<System.Type, Dictionary<object, ModelErp>> appIcodeList = new Dictionary<System.Type, Dictionary<object, ModelErp>>();
                foreach (var objType in dogCache.dbCache.Keys)
                {
                    if (this.tabTypes.ContainsKey(objType))
                    {
                        List<ModelErp> outList = dogCache.dbCache[objType].Values.OfType<ModelErp>().ToList(); // prendo la lista dei record della tabella corrente
                        DogTable tab = this.tabTypes[objType];
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
                                        //if (options.Contains("[PLAIN]") == false && options.Contains("[RECURSIVE]") == true)
                                        //{
                                        //    if (!dogCache.dbCache.ContainsKey(xrefObjType)) dogCache.dbCache[xrefObjType] = new Dictionary<object, ModelErp>();
                                        //    if (!dogCache.dbCache[xrefObjType].ContainsKey(icode))
                                        //    {
                                        //        mustAddRecursiveObj = true; // devo aggiungere l'oggetto ricorsivo alla cache
                                        //        dogCache.dbCache[xrefObjType].Add(icode, null); // Aggiungo la chiave con valore null
                                        //    }
                                        //    if (mustAddRecursiveObj) continue; // Se devo aggiungere l'oggetto ricorsivo, salto il ciclo (non aggiorno i riferimenti)
                                        //}

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
            if (dogCache.ruleXrefFrom.Count > 0)
            {
                foreach (var xrefFromPropertyName in dogCache.ruleXrefFrom)
                {
                    DogField fld = this.tabProperties[xrefFromPropertyName];
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
                        var el = dogCache.dbCache[objModelType][key];
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
            Dictionary<object, ModelErp> mainObjDict = dogCache.dbCache[mainObjType];
            return mainObjKeyList.Select(k => mainObjDict[k]).ToList();     // restituisce la lista di valori T corrispondenti a una lista di chiavi List<object> da un Dictionary<object, T> 
                                                                            // se anche una sola chiave non è presente, otterrai una KeyNotFoundException.
        }




    }
}