using Amazon.SecurityToken.Model;
using DnsClient.Protocol;
using ErpToolkit.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog.LayoutRenderers;
using Org.BouncyCastle.Utilities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Transactions;
using static ErpToolkit.Helpers.Db.DatabaseManager;
using static ErpToolkit.Helpers.Db.DogManager;
using static ErpToolkit.Helpers.Db.ExtraFilterCompiler;
using static ErpToolkit.Helpers.ErpError;


namespace ErpToolkit.Helpers.Db
{
    //------------------- 
    //Data Object Gateway
    //-------------------
    // Funzioni di gestione accesso al Database, con il supporto del Data Model 
    public class DogManager
    {
        private static readonly NLog.ILogger _logger;
        static DogManager()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }


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
        internal const int DOG_MAX_OBJ_DEPTH = (int)3; //massimo livello di profondità di inclusione degli oggetti. Serve per evitare loop infiniti in caso di inclusione di oggetti che si richiamano a vicenda, come ad esempio un cliente che ha un ordine, e l'ordine ha un cliente, e così via. Se il livello di profondità è maggiore di DOG_MAX_OBJ_DEPTH, allora l'oggetto non viene incluso.
        internal const int DOG_DEFAULT_QUERY_MAX_RECORDS = (int)10000; //numero massimo di record restituiti dalle query di selezione. Serve per evitare di caricare in memoria un numero eccessivo di record, che potrebbe causare problemi di performance o di memoria. Il valore può essere configurato tramite la proprietà MaxRecords del DogManager.
        internal const int DOG_DEFAULT_XREF_CACHE_MAX_RECORDS = (int)100000; //numero massimo di record memorizzati nella cache delle tabelle di relazione (xref). Serve per evitare di caricare in memoria un numero eccessivo di record, che potrebbe causare problemi di performance o di memoria. Il valore può essere configurato tramite la proprietà MaxRecords del DogManager. Se il numero di record supera questo valore, allora la cache non viene utilizzata e le tabelle di relazione vengono caricate dinamicamente ad ogni accesso.

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
            internal ModelErp? GetObject(System.Type type, object icode) { try { return dbCache[type][icode]; } catch (Exception ex) { return null; } } // return= null se non trovato
            internal Dictionary<object, ModelErp>? GetDictionary(System.Type type) { try { return dbCache[type]; } catch (Exception ex) { return null; } } // return= null se non trovato

            //-----

            private List<string> ruleXrefFrom = new List<string>();
            public List<string> RuleXrefFrom { get { return ruleXrefFrom; } }
            public void AddRuleXrefFrom(List<string> newRuleXrefFrom) { this.ruleXrefFrom = this.ruleXrefFrom.Union<string>(newRuleXrefFrom).ToList<string>(); } //integra valori passati a quelli presenti. esclude duplicati in base alla funzione Equal. E' case sensitive.

            public Dictionary<System.Type, Dictionary<object, ModelErp>> dbCache { get; } = new Dictionary<System.Type, Dictionary<object, ModelErp>>();

            public DogCache()
            {
            }
        }


        //gestione objects
        public static void checkTableObj(object tabModel) { if("TAB" != (tabModel.GetType().GetField("CATEG", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null)?.ToString()?.Trim() ?? "")) throw new ArgumentException(nameof(tabModel)); }


        //gestione properties
        //^^//public static object? getPropertyValue(object selModel, string propName) { return DogManagerInt.getPropertyValue(selModel, propName); }
        //^^//public static bool setPropertyValue(object selModel, string propName, string? propValue) { return DogManagerInt.setPropertyValue(selModel, propName, propValue); }
        public static object? getPropertyValue(ModelErp selModel, string propName) { return DogManagerQuery.getPropertyValue_static(selModel, propName); }
        public static bool setPropertyValue(ModelErp selModel, string propName, string? propValue) { return DogManagerQuery.setPropertyValue_static(selModel, propName, propValue); }
        public object? getDogPropertyValue(ModelErp selModel, string propName) { return DogManagerQuery.getPropertyValue(this, selModel, propName); }
        public bool setDogPropertyValue(ModelErp selModel, string propName, string? propValue) { return DogManagerQuery.setPropertyValue(this, selModel, propName, propValue); }


        //gestione parameters
        public static string addParam(object value, ref IDictionary<string, object> parameters) { string parName = $"PARM{parameters.Count}X"; parameters.Add(parName, value); return $"@{parName}"; }
        public static List<string> addListParam(List<object> values, ref IDictionary<string, object> parameters) { List<string> cond = new List<string>(); foreach (var value in values) { string parName = $"PARM{parameters.Count}X"; parameters.Add(parName, value); cond.Add($"@{parName}"); } return cond; }



        //***************************************************************************************************************************************************
        //*** INIZIO CLASSE
        //***************************************************************************************************************************************************


        private string _modelName; // = "SIO";
        private string _modelMode; // = "";  //indica come interpretare il Modello. Se _modelMode == "FREE" allora il modello non prevede i campi standard _deleted _timestamp, ecc. e non gestisce le date come stringhe
        private DbTyp _databaseType; // = SqlServer;
        private string _connectionStringName; // = "#connectionString_SQLSLocal";
        private string _dbRoot; // = "IU01";
        private string _dbHome; // = "sio_PROD";


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
        public long MaxFileLengthBytes { get { return _getDbMg().MaxFileLengthBytes; } set { _getDbMg().MaxFileLengthBytes = value; } }


        //***************************************************************************************************************************************************
        //*** INIT
        //***************************************************************************************************************************************************

        public readonly Dictionary<string, DogTable> tables = new Dictionary<string, DogTable>();
        public readonly Dictionary<System.Type, DogTable> tabTypes = new Dictionary<System.Type, DogTable>();
        //%%//public readonly Dictionary<string, DogTable> tabPrefixes = new Dictionary<string, DogTable>();
        public readonly Dictionary<int, DogTable> tabIntcodes = new Dictionary<int, DogTable>();
        public readonly Dictionary<string, DogField> tabProperties = new Dictionary<string, DogField>();
        public readonly Dictionary<string, DogField> tabFields = new Dictionary<string, DogField>();
        //----Tabelle di selezione-------------------------
        public readonly Dictionary<string, DogTable> selfilters = new Dictionary<string, DogTable>();
        public readonly Dictionary<System.Type, DogTable> selTypes = new Dictionary<System.Type, DogTable>();
        public readonly Dictionary<string, DogField> selProperties = new Dictionary<string, DogField>();
        public readonly Dictionary<string, DogField> selFields = new Dictionary<string, DogField>();
        //----TopologicalSort-------------------------
        public readonly Dictionary<System.Type, List<System.Type>> typeGraph = new Dictionary<System.Type, List<System.Type>>();


        public DogField getDogField(string propertyName)
        {
            if (tabProperties.TryGetValue(propertyName, out var fld)) return fld;
            if (selProperties.TryGetValue(propertyName, out fld)) return fld;
            return null;
        }
        public DogField getDbDogField(DogField dogFld)
        {
            if (dogFld == null) return null;
            if (dogFld.table.CATEG == "TAB") return dogFld;
            if (tabFields.TryGetValue($"{dogFld.table.SqlTableName}.{dogFld.SqlFieldName}", out var fld)) return fld;
            return null;
        }


        public class DogTable
        {
            internal DogTable() { }
            //--
            public string tableName = "";
            public System.Type tableTpy;
            public ModelErp tableModelErpObj;
            public List<DogField> fields = new List<DogField>();
            public List<DogField> XrefFromFld = new List<DogField>();  //campi che referenziano questa tabella
            //--
            public string Description = "";
            public string SqlTableName = "";
            public string SqlTableNameExt = "";
            public string SqlTableProperties = "";
            //public string RowIdName = "";
            //public string SqlRowIdName = "";
            //public string SqlRowIdNameExt = "";
            public string SqlPrefix = "";
            public string SqlPrefixExt = "";
            //---
            public bool isXdataTable = false; //indica se la tabella è una tabella di dati estesi (Xdata).
            //---
            public string MODEL = ""; //Nome Modello es: SIO
            public string CATEG = ""; //Categoria Oggetto es: TAB=Table, SEL=Selection, ecc.
            public int INTCODE = 0; //Internal Table Code
            public string TBAREA = ""; //Table Area
            public string PREFIX = ""; //Table Prefix
            public string LIVEDESC = ""; //Table type: Live or Description
            public string IS_RELTABLE = ""; //Is Relation Table: Yes or No
            //---
            public DogTable tabSelection = null; //ref tabella legata alla classe di selezione.
            public DogTable tabXdata = null; //ref tabella legata ai dati estesi.
            public System.Type tabXdataIcodeTyp = typeof(string); //tipo Icode tabella legata ai dati estesi ie: string|long.
            //---
            public DogField fldIcode = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldDeleted = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldTimestamp = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldCdate = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldCtime = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldCagent = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldCunit = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldMdate = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldMtime = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldMagent = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldMunit = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldHome = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldVersion = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldInactive = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldExtatt = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldMref = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldSeq = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldDescr = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldFmt = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldXdurl = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            public DogField fldXdatum = null; //ref campo di sistema. se presente può essere solo uno per tabella.
            //--
            public DogField fldGetFirstByOption(string token) => this.fields.FirstOrDefault(f => !string.IsNullOrEmpty(f?.SqlFieldOptions) && f.SqlFieldOptions.Contains(token, StringComparison.OrdinalIgnoreCase));
        }
        public class DogField
        {
            private DogField() { }
            internal DogField(Func<ModelDog, object?>? getter, Action<ModelDog, object?>? setter, Func<ModelDog, object?>? objGetter, Action<ModelDog, object?>? objSetter) { _getter = getter; _setter = setter; _objGetter = objGetter; _objSetter = objSetter; }
            internal void setXrefListGetterSetter(Func<ModelDog, object?>? getter, Action<ModelDog, object?>? setter) { _listXrefGetter = getter; _listXrefSetter = setter; }
            internal void setXrefDictGetterSetter(Func<ModelDog, object?>? getter, Action<ModelDog, object?>? setter) { _dictXrefGetter = getter; _dictXrefSetter = setter; }
            //--
            public string fieldName = "";
            public System.Type fieldTyp;
            public System.Type fieldObjTyp;
            public System.Type fieldXrefListTyp;  // List<T>?
            public System.Type fieldXrefDictTyp;  // Dictionary<string,T>?
            public DogTable table;
            //--
            public string SqlFieldName = "";  // eg: AV_CODICE
            public string SqlFieldProperties = ""; // eg: prop() xref() xdup(ATTIVITA.AV__ICODE[AV__ICODE] {AV_CODICE=' '}) multbxref()
            public string SqlFieldOptions = "";  // [UID] [XID] codice univoco utente e esterno
            public string SqlFieldNameExt = "";  // AY_CODE
            public string Xref = "";  // external reference (if any) eg: Pa1Icode
            public DogField XrefObj;  // external reference (if any) eg: Pa1Icode
            //--
            public bool optXREF = false;        //campo di relazione (es. PrIdAttivitaRichiesta)
            public bool optXREFlist = false;    //campo di relazione (es. PrIdAttivitaRichiesta) con lista definita sul modello della tabella relazionata (es: List<Prestazione> XrefPrIdAttivitaRichiesta; definita sul modello Attivita)
            public bool optXREFdict = false;    //campo di relazione (es. PrIdAttivitaRichiesta) con dictionary definita sul modello della tabella relazionata (es: Dictionary<string,Prestazione> XrefPrIdAttivitaRichiesta; definita sul modello Attivita)
            public bool optMANDATORY = false;   //campo obbligatorio. Il valore non può essere NULL o vuoto, anche se il campo è di relazione (optXREF).
            public bool optSYS = false;         //campo di sistema su cui non devo effettuare confronto tra record (sono solo: _icode _deleted, _timestamp, _home, _cdate,_ctime,_cagent,_cunit, _mdate,_mtime,_magent,_munit)
            //-
            public bool optSID = false;         //campo di sistema: chiave univoca  (es. _icode)
            public bool optDEL = false;         //campo di sistema: di sistema (es. _deleted)
            public bool optTMS = false;         //campo di sistema timestamp di sistema (es. _timestamp)
            public bool optCDATE = false;       //campo di sistema: data creazione (es. _cdate)
            public bool optCTIME = false;       //campo di sistema: ora creazione (es. _ctime)
            public bool optCAGENT = false;      //campo di sistema: agente creazione (es. _cagent)
            public bool optCUNIT = false;       //campo di sistema: unità creazione (es. _cunit)
            public bool optMDATE = false;       //campo di sistema: data ultima modifica (es. _mdate)
            public bool optMTIME = false;       //campo di sistema: ora ultima modifica (es. _mtime)
            public bool optMAGENT = false;      //campo di sistema: agente ultima modifica (es. _magent)
            public bool optMUNIT = false;       //campo di sistema: unità ultima modifica (es. _munit)
            public bool optHOME = false;        //campo home (es. _home)
            public bool optVERSION = false;     //campo version (es. _version)
            public bool optINACTIVE = false;    //campo inactive (es. _inactive)
            public bool optEXTATT = false;      //campo extatt (es. _extatt)
            //--
            public bool optMREF = false;      //campo mref della tabella Xdata 
            public bool optSEQ = false;      //campo seq della tabella Xdata 
            public bool optDESCR = false;      //campo descr della tabella Xdata 
            public bool optFMT = false;      //campo fmt della tabella Xdata 
            public bool optXDURL = false;      //campo xdurl della tabella Xdata 
            public bool optXDATUM = false;      //campo xdatum della tabella Xdata 
            //-
            public bool optUID = false;         //campo chiave univoca di utente (es. codice cliente)
            public bool optXID = false;         //campo chiave univoca esterna (es. codice esterno cliente)
            public bool optDATE = false;        //campo data (es. data nascita)
            public bool optTIME = false;        //campo ora (es. ora appuntamento)
            public bool optDATETIME = false;    //campo data e ora (es. data e ora registrazione)
            public bool optBIGINT = false;      //campo intero lungo usato come chiave relazione esterna FK (usato generalmente su IRIS)
            public bool optLABEL = false;       //campo label viene usato come etichetta descrittiva in liste e combo (es. descrizione cliente)
            //--
            public string Description;  
            public object? DefaultValue = null;  
            public int? StringLength = null;

            //--- NUOVO: template ExtraFilter letto da AutocompleteServerAttribute al momento
            //--- della costruzione del DogField. Null se il campo non ha ExtraFilter.
            public string? AutocompleteExtraFilter { get; internal set; } = null;
            //---
            //---

            // Getter e Setter
            private Func<ModelDog, object?>? _getter;
            private Action<ModelDog, object?>? _setter;
            public object? GetValue(ModelDog model)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (_getter == null) throw new InvalidOperationException("_getter = null");
                if (this.table.tableTpy != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.table.tableTpy.FullName}]");
                return _getter!(model);
            }
            public void SetValue(ModelDog model, object? value)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (_setter == null) throw new InvalidOperationException("_setter = null");
                if (this.table.tableTpy != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.table.tableTpy.FullName}]");
                System.Type targetType = Nullable.GetUnderlyingType(this.fieldTyp) ?? this.fieldTyp;
                if (value == null)
                {
                    if (!this.fieldTyp.IsValueType || Nullable.GetUnderlyingType(this.fieldTyp) != null) { _setter!(model, null); } // Se il tipo è nullable, possiamo assegnare null
                    else { throw new Exception($"Tipo non nullable nel ModelDog ({model.GetType().FullName}.{this.fieldName})"); }
                }
                else
                {
                    try
                    {
                        // Se il valore è già compatibile, lo assegniamo direttamente
                        if (targetType.IsAssignableFrom(value.GetType())) { _setter!(model, value); }
                        else { object convertedValue = Convert.ChangeType(value, targetType); _setter!(model, convertedValue); } // Proviamo a convertire dinamicamente
                    }
                    catch (Exception ex) { throw new Exception($"Tipo {value.GetType().Name} non assegnabile nel ModelErp ({model.GetType().FullName}.{this.fieldName})", ex); }
                }
            }
            public void CopyValue(ModelErp model, object? value)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (_setter == null) throw new InvalidOperationException("_setter = null");
                if (this.table.tableTpy != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.table.tableTpy.FullName}]");
                if (value == null) { _setter!(model, null); }
                else
                {
                    var type = value.GetType();
                    var underlyingType = Nullable.GetUnderlyingType(type) ?? type;

                    // Caso: Dictionary<string,string>
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                        type.GetGenericArguments()[0] == typeof(string) && type.GetGenericArguments()[1] == typeof(string))
                    {
                        var dictStr = (Dictionary<string, string>)value; _setter!(model, new Dictionary<string, string>(dictStr));
                    }
                    // Caso: List<string>
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && type.GetGenericArguments()[0] == typeof(string))
                    {
                        var listString = (List<string>)value; _setter!(model, new List<string>(listString));
                    }
                    // Caso: List<long>
                    else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && type.GetGenericArguments()[0] == typeof(long))
                    {
                        var listLong = (List<long>)value; _setter!(model, new List<long>(listLong));
                    }
                    // Tipi semplici e nullable
                    else if (underlyingType == typeof(string) ||
                        underlyingType == typeof(int) ||
                        underlyingType == typeof(long) ||
                        underlyingType == typeof(double) ||
                        underlyingType == typeof(short) ||
                        underlyingType == typeof(char) ||
                        underlyingType == typeof(DateTime) ||
                        underlyingType == typeof(DateOnly) ||
                        underlyingType == typeof(TimeOnly))
                    {
                        _setter!(model, value);
                    }
                    // DateRange
                    else if (type == typeof(DateRange))
                    {
                        var originalDateRange = (DateRange)value;
                        var newDateRange = new DateRange(); newDateRange.StartDate = originalDateRange.StartDate; newDateRange.EndDate = originalDateRange.EndDate;
                        _setter!(model, newDateRange);
                    }
                    // Clonazione profonda di byte[]
                    else if (type == typeof(byte[]))
                    {
                        var originalBytes = (byte[])value;
                        _setter!(model, (originalBytes != null) ? (byte[])originalBytes.Clone() : null);
                    }
                    else
                    {
                        //non sono consentite altre tipologie di proprietà nei ModelErp
                        throw new Exception($"Tipo {type.Name} non consentito nel ModelErp ({model.GetType().FullName}.{this.fieldName})");
                    }
                }
            }
            // Obj Getter e Setter
            private Func<ModelDog, object?>? _objGetter;
            private Action<ModelDog, object?>? _objSetter;
            public object? GetObjValue(ModelDog model)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (_objGetter == null) throw new InvalidOperationException("_objGetter = null");
                if (this.table.tableTpy != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.table.tableTpy.FullName}]");
                if (!this.optXREF || this.fieldObjTyp == null) throw new Exception($"Tipo non Object nel ModelDog ({model.GetType().FullName}.{this.fieldName})");
                return _objGetter!(model);
            }
            public void SetObjValue(ModelDog model, object? value)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));
                if (_objSetter == null) throw new InvalidOperationException("_objSetter = null");
                if (this.table.tableTpy != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.table.tableTpy.FullName}]");
                if (!this.optXREF || this.fieldObjTyp == null) throw new Exception($"Tipo non Object nel ModelDog ({model.GetType().FullName}.{this.fieldName})");
                System.Type targetType = Nullable.GetUnderlyingType(this.fieldObjTyp) ?? this.fieldObjTyp;
                if (value == null)
                {
                    if (!this.fieldObjTyp.IsValueType || Nullable.GetUnderlyingType(this.fieldObjTyp) != null) { _objSetter!(model, null); } // Se il tipo è nullable, possiamo assegnare null
                    else { throw new Exception($"Tipo non nullable nel ModelDog ({model.GetType().FullName}.{this.fieldName}Obj)"); }
                }
                else
                {
                    try
                    {
                        // Se il valore è già compatibile, lo assegniamo direttamente
                        if (targetType.IsAssignableFrom(value.GetType())) { _objSetter!(model, value); }
                        else { object convertedValue = Convert.ChangeType(value, targetType); _objSetter!(model, convertedValue); } // Proviamo a convertire dinamicamente
                    }
                    catch (Exception ex) { throw new Exception($"Tipo {value.GetType().Name} non assegnabile nel ModelDog ({model.GetType().FullName}.{this.fieldName}Obj)", ex); }
                }
            }
            // List Getter e Setter
            protected Func<ModelDog, object?>? _listXrefGetter;
            protected Action<ModelDog, object?>? _listXrefSetter;
            public object? GetListXrefValue(ModelDog model)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));  // model deve essere di tipo this.fieldObjTyp  
                if (_listXrefGetter == null) throw new InvalidOperationException("_listXrefGetter = null");
                if (this.fieldObjTyp != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.fieldObjTyp.FullName}]");
                if (!this.optXREFlist || this.fieldXrefListTyp == null) throw new Exception($"Proprietà Xref{this.fieldName} non definita nel ModelDog {model.GetType().FullName} per il campo {this.table.tableTpy.FullName}.{this.fieldName}");
                return _listXrefGetter!(model);
            }
            public void SetListXrefValue(ModelDog model, object? value)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));  // model deve essere di tipo this.fieldObjTyp  
                if (_listXrefSetter == null) throw new InvalidOperationException("_listXrefSetter = null");
                if (this.fieldObjTyp != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.fieldObjTyp.FullName}]");
                if (!this.optXREFlist || this.fieldXrefListTyp == null) throw new Exception($"Proprietà Xref{this.fieldName} non definita nel ModelDog {model.GetType().FullName} per il campo {this.table.tableTpy.FullName}.{this.fieldName}");
                try
                {
                    // Se il valore è già compatibile, lo assegniamo direttamente
                    if (value == null || this.fieldXrefListTyp.IsAssignableFrom(value.GetType())) { _listXrefSetter!(model, value); }
                    else { object convertedValue = Convert.ChangeType(value, this.fieldXrefListTyp); _listXrefSetter!(model, convertedValue); } // Proviamo a convertire dinamicamente
                }
                catch (Exception ex) { throw new Exception($"Tipo {value?.GetType().Name ?? "null"} non assegnabile alla proprietà {model.GetType().FullName}.Xref{this.fieldName} ", ex); }
            }
            // Dictionary Getter e Setter
            protected Func<ModelDog, object?>? _dictXrefGetter;
            protected Action<ModelDog, object?>? _dictXrefSetter;
            public object? GetDictXrefValue(ModelDog model)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));  // model deve essere di tipo this.fieldObjTyp  
                if (_dictXrefGetter == null) throw new InvalidOperationException("_dictXrefGetter = null");
                if (this.fieldObjTyp != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.fieldObjTyp.FullName}]");
                if (!this.optXREFdict || this.fieldXrefDictTyp == null) throw new Exception($"Proprietà Xref{this.fieldName} non definita nel ModelDog {model.GetType().FullName} per il campo {this.table.tableTpy.FullName}.{this.fieldName}");
                return _dictXrefGetter!(model);
            }
            public void SetDictXrefValue(ModelDog model, object? value)
            {
                if (model == null) throw new ArgumentNullException(nameof(model));  // model deve essere di tipo this.fieldObjTyp  
                if (_dictXrefSetter == null) throw new InvalidOperationException("_dictXrefSetter = null");
                if (this.fieldObjTyp != model.GetType()) throw new InvalidOperationException($"Wrong model type [{model.GetType().FullName}]. Expected [{this.fieldObjTyp.FullName}]");
                if (!this.optXREFdict || this.fieldXrefDictTyp == null) throw new Exception($"Proprietà Xref{this.fieldName} non definita nel ModelDog {model.GetType().FullName} per il campo {this.table.tableTpy.FullName}.{this.fieldName}");
                try
                {
                    // Se il valore è già compatibile, lo assegniamo direttamente
                    if (value == null || this.fieldXrefDictTyp.IsAssignableFrom(value.GetType())) { _dictXrefSetter!(model, value); }
                    else { object convertedValue = Convert.ChangeType(value, this.fieldXrefDictTyp); _dictXrefSetter!(model, convertedValue); } // Proviamo a convertire dinamicamente
                }
                catch (Exception ex) { throw new Exception($"Tipo {value?.GetType().Name ?? "null"} non assegnabile alla proprietà {model.GetType().FullName}.Xref{this.fieldName} ", ex); }
            }

        }
        internal DogManager(string modelName, string modelMode, DbTyp databaseType, string connectionStringName, string dbRoot, string dbHome)
        {
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
                if (typeof(ModelErp).IsAssignableFrom(objType) == false) continue; // esclude i tipi che non estendono ModelErp. (Se si vuole includere anche questi, rimuovere questa riga).

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
                        tab.tableModelErpObj = (ModelErp)Activator.CreateInstance(tab.tableTpy)!;
                        //--
                        tab.Description = objType.GetField("Description")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableName = objType.GetField("SqlTableName")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableNameExt = objType.GetField("SqlTableNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlTableProperties = objType.GetField("SqlTableProperties")?.GetRawConstantValue()?.ToString() ?? "";
                        //tab.RowIdName = objType.GetField("RowIdName")?.GetRawConstantValue()?.ToString() ?? "";
                        //tab.SqlRowIdName = objType.GetField("SqlRowIdName")?.GetRawConstantValue()?.ToString() ?? "";
                        //tab.SqlRowIdNameExt = objType.GetField("SqlRowIdNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlPrefix = objType.GetField("SqlPrefix")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.SqlPrefixExt = objType.GetField("SqlPrefixExt")?.GetRawConstantValue()?.ToString() ?? "";
                        //--
                        tab.tabXdata = null;

                        //--
                        tab.MODEL = modNameVal;
                        tab.CATEG = categNameVal;
                        tab.INTCODE = Convert.ToInt32(objType.GetField("INTCODE")?.GetRawConstantValue());
                        tab.TBAREA = objType.GetField("TBAREA")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.PREFIX = objType.GetField("PREFIX")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.LIVEDESC = objType.GetField("LIVEDESC")?.GetRawConstantValue()?.ToString() ?? "";
                        tab.IS_RELTABLE = objType.GetField("IS_RELTABLE")?.GetRawConstantValue()?.ToString() ?? "";
                        //---------
                        tab.tabSelection = null;
                        //---------
                        tab.fldIcode = null;
                        tab.fldDeleted = null;
                        tab.fldTimestamp = null;
                        tab.fldCdate = null;
                        tab.fldCtime = null;
                        tab.fldCagent = null;
                        tab.fldCunit = null;
                        tab.fldMdate = null;
                        tab.fldMtime = null;
                        tab.fldMagent = null;
                        tab.fldMunit = null;
                        tab.fldHome = null;
                        tab.fldVersion = null;
                        tab.fldInactive = null;
                        tab.fldExtatt = null;
                        tab.fldMref = null;
                        tab.fldSeq = null;
                        tab.fldDescr = null;
                        tab.fldFmt = null;
                        tab.fldXdurl = null;
                        tab.fldXdatum = null;
                        //---------
                        foreach (var property in objType.GetProperties())
                        {
                            ErpDogFieldAttribute? erpDogFieldAttribute = property.GetCustomAttribute(typeof(ErpDogFieldAttribute)) as ErpDogFieldAttribute;
                            if (erpDogFieldAttribute != null)
                            {

                                DogField fld = fillDogField(tab, property, erpDogFieldAttribute);
                                
                                //---------
                                tab.fields.Add(fld);
                                switch (categNameVal)
                                {
                                    case "TAB":
                                        tabProperties.Add(fld.fieldName, fld);  
                                        tabFields.Add($"{fld.table.SqlTableName}.{fld.SqlFieldName}", fld);   //tabFields.Add(fld.SqlFieldName, fld);
                                        break;
                                    case "SEL":
                                        selProperties.Add(fld.fieldName, fld);
                                        selFields.Add($"{fld.table.SqlTableName}.{fld.SqlFieldName}", fld);   //selFields.Add(fld.SqlFieldName, fld);
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
                                //%%//tabPrefixes.Add(tab.SqlPrefix, tab);
                                tabIntcodes.Add(tab.INTCODE, tab);
                                break;
                            case "SEL":
                                selfilters.Add(tab.SqlTableName, tab);
                                selTypes.Add(tab.tableTpy, tab);
                                break;
                        }
                        //-------


                        //carico tab.tabXdata collegata (se presente)
                        string sqlXdataTableName = objType.GetField("SqlXdataTableName")?.GetRawConstantValue()?.ToString() ?? "";
                        if (!string.IsNullOrWhiteSpace(sqlXdataTableName))
                        {
                            string sqlXdataIcodeTyp = objType.GetField("SqlXdataIcodeTyp")?.GetRawConstantValue()?.ToString() ?? "";
                            tab.tabXdataIcodeTyp = (sqlXdataIcodeTyp == "long") ? typeof(long) : typeof(string);  // viene specificato sono se l'icode della tabella Xdata è di tipo long, altrimenti si assume string (default)
                            //--
                            tab.tabXdata = new DogTable();
                            tab.tabXdata.tableName = typeof(ModelXdata).Name;
                            tab.tabXdata.tableTpy = typeof(ModelXdata);
                            tab.tabXdata.tableModelErpObj = null;   // la tabella Xdata non ha un corrispondente ModelErp, ma viene gestita con un unico ModelXdata generico
                            tab.tabXdata.isXdataTable = true;
                            //--
                            tab.tabXdata.Description = "Dati estesi di: " + tab.Description;
                            tab.tabXdata.SqlTableName = sqlXdataTableName;
                            tab.tabXdata.SqlTableNameExt = objType.GetField("SqlXdataTableNameExt")?.GetRawConstantValue()?.ToString() ?? "";
                            tab.tabXdata.SqlTableProperties = "";       //???????????????????????????????????????????????????????????????????????????
                            tab.tabXdata.SqlPrefix = "";                //???????????????????????????????????????????????????????????????????????????
                            tab.tabXdata.SqlPrefixExt = "";             //???????????????????????????????????????????????????????????????????????????
                            //--
                            tab.tabXdata.MODEL = modNameVal;
                            tab.tabXdata.CATEG = categNameVal;
                            tab.tabXdata.INTCODE = Convert.ToInt32(objType.GetField("INTCODE")?.GetRawConstantValue());
                            tab.tabXdata.TBAREA = objType.GetField("TBAREA")?.GetRawConstantValue()?.ToString() ?? "";
                            tab.tabXdata.PREFIX = objType.GetField("PREFIX")?.GetRawConstantValue()?.ToString() ?? "";
                            tab.tabXdata.LIVEDESC = objType.GetField("LIVEDESC")?.GetRawConstantValue()?.ToString() ?? "";
                            tab.tabXdata.IS_RELTABLE = objType.GetField("IS_RELTABLE")?.GetRawConstantValue()?.ToString() ?? "";
                            //---------
                            tab.tabXdata.tabSelection = null;
                            //---------
                            tab.tabXdata.fldIcode = null;
                            tab.tabXdata.fldDeleted = null;
                            tab.tabXdata.fldTimestamp = null;
                            tab.tabXdata.fldCdate = null;
                            tab.tabXdata.fldCtime = null;
                            tab.tabXdata.fldCagent = null;
                            tab.tabXdata.fldCunit = null;
                            tab.tabXdata.fldMdate = null;
                            tab.tabXdata.fldMtime = null;
                            tab.tabXdata.fldMagent = null;
                            tab.tabXdata.fldMunit = null;
                            tab.tabXdata.fldHome = null;
                            tab.tabXdata.fldVersion = null;
                            tab.tabXdata.fldInactive = null;
                            tab.tabXdata.fldExtatt = null;
                            tab.tabXdata.fldMref = null;
                            tab.tabXdata.fldSeq = null;
                            tab.tabXdata.fldDescr = null;
                            tab.tabXdata.fldFmt = null;
                            tab.tabXdata.fldXdurl = null;
                            tab.tabXdata.fldXdatum = null;
                            //--
                            foreach (var property in typeof(ModelXdata).GetProperties())
                            {
                                ErpDogFieldAttribute? erpDogFieldAttribute = property.GetCustomAttribute(typeof(ErpDogFieldAttribute)) as ErpDogFieldAttribute;
                                if (erpDogFieldAttribute != null)
                                {
                                    DogField fld = fillDogField(tab.tabXdata, property, erpDogFieldAttribute,
                                        xref: (property.Name == "Xref") ? tab.fldIcode?.fieldName : null,
                                        sqlFieldName: objType.GetField($"SqlXdata{property.Name}Name")?.GetRawConstantValue()?.ToString(),
                                        sqlFieldNameExt: null);     //futura estensione per gestire campo codice esterno anche su Xdata
                                    tab.tabXdata.fields.Add(fld);
                                }
                            }
                        }



                    }
                }
            }
            // carica XrefObj & XrefFromFld
            foreach (var fld in tabProperties.Values)
            {
                if (fld.optXREF)
                {
                    if (tabProperties.ContainsKey(fld.Xref))    // il field deve esistere
                    {  

                        //carico lista di relazione
                        fld.XrefObj = tabProperties[fld.Xref]; tabProperties[fld.Xref].table.XrefFromFld.Add(fld);

                        // check if List<T> :: verifico se è definita la tabella XrefXXXXXXXX nel modello
                        ModelErp tb = (ModelErp)Activator.CreateInstance(tabProperties[fld.Xref].table.tableTpy)!;
                        if (tb == null) throw new ArgumentException($"Errore: impossibile creare istanza modello {tabProperties[fld.Xref].table.tableTpy} ");
                        var property = tb.GetType().GetProperty("Xref" + fld.fieldName);
                        if (property != null && property.CanWrite &&
                            property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(List<>) &&
                            property.PropertyType.GetGenericArguments()[0] == fld.table.tableTpy)
                        {
                            // verifica se la lista e nullable
                            if (property.GetType().IsValueType && Nullable.GetUnderlyingType(property.GetType()) == null) { throw new Exception($"Errore: Xref{fld.fieldName} non nullable nel ModelErp {tb.GetType().FullName} "); }

                            //--------- EnsureDelegates
                            var paramModel = Expression.Parameter(typeof(ModelDog), "model");
                            var castModel = Expression.Convert(paramModel, tabProperties[fld.Xref].table.tableTpy);
                            // ====== Getter ======
                            var propAccess = Expression.Property(castModel, property);
                            var castToObject = Expression.Convert(propAccess, typeof(object));
                            Func<ModelDog, object?>? _listXrefGetter = Expression.Lambda<Func<ModelDog, object?>>(castToObject, paramModel).Compile();
                            // ====== Setter ======
                            var paramValue = Expression.Parameter(typeof(object), "value");
                            var castValue = Expression.Convert(paramValue, property.PropertyType);
                            var setExpr = Expression.Call(castModel, property.GetSetMethod(), castValue);
                            Action<ModelDog, object?>? _listXrefSetter = Expression.Lambda<Action<ModelDog, object?>>(setExpr, paramModel, paramValue).Compile();
                            // Salva
                            fld.setXrefListGetterSetter(_listXrefGetter, _listXrefSetter);
                            //---
                            var xrefListType = typeof(List<>).MakeGenericType(fld.table.tableTpy);
                            fld.fieldXrefListTyp = Nullable.GetUnderlyingType(xrefListType) ?? xrefListType;  // List<T>?   lista nullable
                            fld.optXREFlist = true;     // nel modello tabProperties[fld.Xref].table è definita la propietà List<..fld.table.tableTpy..> Xef..fld.fieldName.. che fa riferimento a questo campo
                        }
                        if (property != null && property.CanWrite &&
                            property.PropertyType.IsGenericType &&
                            property.PropertyType.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                            property.PropertyType.GetGenericArguments()[0] == typeof(string) &&
                            property.PropertyType.GetGenericArguments()[1] == fld.table.tableTpy)
                        {
                            // verifica se la lista e nullable
                            if (property.GetType().IsValueType && Nullable.GetUnderlyingType(property.GetType()) == null) { throw new Exception($"Errore: Xref{fld.fieldName} non nullable nel ModelErp {tb.GetType().FullName} "); }

                            //--------- EnsureDelegates
                            var paramModel = Expression.Parameter(typeof(ModelDog), "model");
                            var castModel = Expression.Convert(paramModel, tabProperties[fld.Xref].table.tableTpy);
                            // ====== Getter ======
                            var propAccess = Expression.Property(castModel, property);
                            var castToObject = Expression.Convert(propAccess, typeof(object));
                            Func<ModelDog, object?>? _dictXrefGetter = Expression.Lambda<Func<ModelDog, object?>>(castToObject, paramModel).Compile();
                            // ====== Setter ======
                            var paramValue = Expression.Parameter(typeof(object), "value");
                            var castValue = Expression.Convert(paramValue, property.PropertyType);
                            var setExpr = Expression.Call(castModel, property.GetSetMethod(), castValue);
                            Action<ModelDog, object?>? _dictXrefSetter = Expression.Lambda<Action<ModelDog, object?>>(setExpr, paramModel, paramValue).Compile();
                            // Salva
                            fld.setXrefDictGetterSetter(_dictXrefGetter, _dictXrefSetter);
                            //---
                            var xrefDictType = typeof(Dictionary<,>).MakeGenericType(new[] { typeof(string), fld.table.tableTpy });
                            //fld.fieldXrefDictTyp = Nullable.GetUnderlyingType(xrefDictType) ?? xrefDictType;  // Dictionary<object,T>?   dictionary nullable
                            fld.fieldXrefDictTyp = xrefDictType;  // Dictionary<object,T>?   dictionary nullable
                            fld.optXREFdict = true;     // nel modello tabProperties[fld.Xref].table è definita la propietà Dictionary<object,..fld.table.tableTpy..> Xef..fld.fieldName.. che fa riferimento a questo campo
                        }

                    }
                    else throw new ArgumentException($"Errore: impossibile creare db, legame campo Xref {fld.Xref} non presente ");
                }
                if (PropertyTypeAllowed(fld.fieldTyp) == false)
                {
                    throw new ArgumentException($"Errore: impossibile creare db, tipo {fld.fieldTyp.Name} non consentito nel ModelErp ({fld.table.tableTpy.FullName}.{fld.fieldName})");
                }

            }
            // collegamento della tabella collegata ad un filtro selezione
            foreach (var sel in selfilters.Values)
            {
                if (sel.SqlTableName != null && tables.ContainsKey(sel.SqlTableName)) sel.tabSelection = tables[sel.SqlTableName];
            }
            //foreach (var fld in selProperties.Values)
            //{
            //    if (fld.optXREF)
            //    {
            //        if (selProperties.ContainsKey($"Sel{fld.Xref}")) fld.XrefObj = selProperties[$"Sel{fld.Xref}"]; // il field deve esistere (nota: SelXx1Icode ..non esiste)
            //        else throw new ArgumentException($"Errore: impossibile creare db, legame campo Xref Sel{fld.Xref} non presente ");
            //    }
            //}

            // Costruisci il grafo delle dipendenze per usare l'ordinamento topologico [TopologicalSort]
            typeGraph = DogManagerTopologicalSort.BuildTypeDependencyGraph(this);


            //Genera file all'avvio
            DogManagerFile.CreateInitFile(this);



            //--- NUOVO: precompila tutti i template ExtraFilter trovati nei DogTable ---
            _ = PrecompileExtraFiltersAsync();   // fire-and-forget con gestione errori interna
            //---

        }
        // riempe il contenuto di un campo DogField a partire dal ModelErp, verificando che il tipo di dato sia consentito e valorizzando i delegate di accesso alla proprietà
        private static DogField fillDogField(DogTable tab, PropertyInfo property, ErpDogFieldAttribute erpDogFieldAttribute,
            string? xref = null, string? sqlFieldName = null, string? sqlFieldNameExt = null)
        {
            string fld_Xref = erpDogFieldAttribute.Xref?.ToString() ?? xref ?? "";
            bool fld_optXREF = string.IsNullOrWhiteSpace(fld_Xref) == false;
            //--------- EnsureDelegates
            var paramModel = Expression.Parameter(typeof(ModelDog), "model");
            var castModel = Expression.Convert(paramModel, tab.tableTpy);
            // ====== Getter ======
            var propAccess = Expression.Property(castModel, property);
            var castToObject = Expression.Convert(propAccess, typeof(object));
            Func<ModelDog, object?>? _getter = Expression.Lambda<Func<ModelDog, object?>>(castToObject, paramModel).Compile();
            // ====== Setter ======
            var paramValue = Expression.Parameter(typeof(object), "value");
            var castValue = Expression.Convert(paramValue, property.PropertyType);
            var setExpr = Expression.Call(castModel, property.GetSetMethod(), castValue);
            Action<ModelDog, object?>? _setter = Expression.Lambda<Action<ModelDog, object?>>(setExpr, paramModel, paramValue).Compile();
            //---------
            DogField fld = null;
            if (fld_optXREF && tab.CATEG == "TAB")
            {
                var objPropertyName = property.Name + "Obj";
                var objProperty = tab.tableTpy.GetProperty(objPropertyName);
                if (objProperty == null) throw new ArgumentException($"Errore nel modello {tab.tableTpy.Name}: manca la proprietà di oggetto correlata {objPropertyName} per il campo di relazione {property.Name}.");
                // ====== Getter ======
                var objPropAccess = Expression.Property(castModel, objProperty);
                var objCastToObject = Expression.Convert(objPropAccess, typeof(object));
                Func<ModelDog, object?>? _objGetter = Expression.Lambda<Func<ModelDog, object?>>(objCastToObject, paramModel).Compile();
                // ====== Setter ======
                var objParamValue = Expression.Parameter(typeof(object), "value");
                var objCastValue = Expression.Convert(objParamValue, objProperty.PropertyType);
                var objSetExpr = Expression.Call(castModel, objProperty.GetSetMethod(), objCastValue);
                Action<ModelDog, object?>? _objSetter = Expression.Lambda<Action<ModelDog, object?>>(objSetExpr, paramModel, objParamValue).Compile();
                // Salva 
                fld = new DogField(_getter, _setter, _objGetter, _objSetter);
                fld.fieldObjTyp = objProperty.PropertyType;
                fld.fieldXrefListTyp = null; //viene valorizzato solo se presente la proprietà XrefXXXXXX
                fld.fieldXrefDictTyp = null; //viene valorizzato solo se presente la proprietà XrefXXXXXX
            }
            else
            {
                fld = new DogField(_getter, _setter, null, null);
                fld.fieldObjTyp = null;
                fld.fieldXrefListTyp = null; //viene valorizzato solo se presente la proprietà XrefXXXXXX
                fld.fieldXrefDictTyp = null; //viene valorizzato solo se presente la proprietà XrefXXXXXX
            }
            fld.Xref = fld_Xref;
            fld.optXREF = fld_optXREF;
            fld.optXREFlist = false;
            fld.optXREFdict = false;
            //---------
            fld.fieldName = property.Name;
            fld.fieldTyp = property.PropertyType;
            fld.table = tab;
            //--
            fld.SqlFieldName = erpDogFieldAttribute.SqlFieldName?.ToString() ?? sqlFieldName ?? "";
            fld.SqlFieldProperties = erpDogFieldAttribute.SqlFieldProperties?.ToString() ?? "";
            fld.SqlFieldOptions = erpDogFieldAttribute.SqlFieldOptions?.ToString() ?? "";
            fld.SqlFieldNameExt = erpDogFieldAttribute.SqlFieldNameExt?.ToString() ?? sqlFieldNameExt ?? "";
            //---------
            fld.optMANDATORY = fld.SqlFieldOptions.Contains("[MANDATORY]");
            //-
            fld.optSID = fld.SqlFieldOptions.Contains("[SID]"); if (fld.optSID) { if (fld.table.fldIcode == null) fld.table.fldIcode = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optDEL = fld.SqlFieldOptions.Contains("[DEL]"); if (fld.optDEL) { if (fld.table.fldDeleted == null) fld.table.fldDeleted = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optTMS = fld.SqlFieldOptions.Contains("[TMS]"); if (fld.optTMS) { if (fld.table.fldTimestamp == null) fld.table.fldTimestamp = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optCDATE = fld.SqlFieldOptions.Contains("[CDATE]"); if (fld.optCDATE) { if (fld.table.fldCdate == null) fld.table.fldCdate = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optCTIME = fld.SqlFieldOptions.Contains("[CTIME]"); if (fld.optCTIME) { if (fld.table.fldCtime == null) fld.table.fldCtime = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optCAGENT = fld.SqlFieldOptions.Contains("[CAGENT]"); if (fld.optCAGENT) { if (fld.table.fldCagent == null) fld.table.fldCagent = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optCUNIT = fld.SqlFieldOptions.Contains("[CUNIT]"); if (fld.optCUNIT) { if (fld.table.fldCunit == null) fld.table.fldCunit = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optMDATE = fld.SqlFieldOptions.Contains("[MDATE]"); if (fld.optMDATE) { if (fld.table.fldMdate == null) fld.table.fldMdate = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optMTIME = fld.SqlFieldOptions.Contains("[MTIME]"); if (fld.optMTIME) { if (fld.table.fldMtime == null) fld.table.fldMtime = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optMAGENT = fld.SqlFieldOptions.Contains("[MAGENT]"); if (fld.optMAGENT) { if (fld.table.fldMagent == null) fld.table.fldMagent = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optMUNIT = fld.SqlFieldOptions.Contains("[MUNIT]"); if (fld.optMUNIT) { if (fld.table.fldMunit == null) fld.table.fldMunit = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optHOME = fld.SqlFieldOptions.Contains("[HOME]"); if (fld.optHOME) { if (fld.table.fldHome == null) fld.table.fldHome = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optVERSION = fld.SqlFieldOptions.Contains("[VERSION]"); if (fld.optVERSION) { if (fld.table.fldVersion == null) fld.table.fldVersion = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optINACTIVE = fld.SqlFieldOptions.Contains("[INACTIVE]"); if (fld.optINACTIVE) { if (fld.table.fldInactive == null) fld.table.fldInactive = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optEXTATT = fld.SqlFieldOptions.Contains("[EXTATT]"); if (fld.optEXTATT) { if (fld.table.fldExtatt == null) fld.table.fldExtatt = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            //--
            fld.optMREF = fld.SqlFieldOptions.Contains("[MREF]"); if (fld.optMREF) { if (fld.table.fldMref == null) fld.table.fldMref = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optSEQ = fld.SqlFieldOptions.Contains("[SEQ]"); if (fld.optSEQ) { if (fld.table.fldSeq == null) fld.table.fldSeq = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optDESCR = fld.SqlFieldOptions.Contains("[DESCR]"); if (fld.optDESCR) { if (fld.table.fldDescr == null) fld.table.fldDescr = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optFMT = fld.SqlFieldOptions.Contains("[FMT]"); if (fld.optFMT) { if (fld.table.fldFmt == null) fld.table.fldFmt = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optXDURL = fld.SqlFieldOptions.Contains("[XDURL]"); if (fld.optXDURL) { if (fld.table.fldXdurl == null) fld.table.fldXdurl = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            fld.optXDATUM = fld.SqlFieldOptions.Contains("[XDATUM]"); if (fld.optXDATUM) { if (fld.table.fldXdatum == null) fld.table.fldXdatum = fld; else throw new ArgumentException($"Errore nelle opzioni del campo {fld.fieldName}: il tipo deve essere unico nella tabella"); }
            //-
            fld.optSYS = fld.SqlFieldOptions.Contains("[SYS]");
            fld.optSYS |= fld.optSID | fld.optDEL | fld.optTMS | fld.optHOME;           // includo sempre questi tipi di campo di sistema
            fld.optSYS |= fld.optCDATE | fld.optCTIME | fld.optCAGENT | fld.optCUNIT;   // includo sempre questi tipi di campo di sistema
            fld.optSYS |= fld.optMDATE | fld.optMTIME | fld.optMAGENT | fld.optMUNIT;   // includo sempre questi tipi di campo di sistema
                                                                                        //-
            fld.optUID = fld.SqlFieldOptions.Contains("[UID]");
            fld.optXID = fld.SqlFieldOptions.Contains("[XID]");
            fld.optDATE = fld.SqlFieldOptions.Contains("[DATE]");
            fld.optTIME = fld.SqlFieldOptions.Contains("[TIME]");
            fld.optDATETIME = fld.SqlFieldOptions.Contains("[DATETIME]");
            fld.optBIGINT = fld.SqlFieldOptions.Contains("[BIGINT]");
            fld.optLABEL = fld.SqlFieldOptions.Contains("[LABEL]");
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

            //--- NUOVO: legge ExtraFilter da AutocompleteServerAttribute
            var autocompleteServerAttr = property.GetCustomAttribute<AutocompleteServerAttribute>();
            fld.AutocompleteExtraFilter = autocompleteServerAttr?.ExtraFilter;
            if (string.IsNullOrWhiteSpace(fld.AutocompleteExtraFilter))
            {
                var autocompleteClientAttr = property.GetCustomAttribute<AutocompleteClientAttribute>();
                fld.AutocompleteExtraFilter = autocompleteClientAttr?.ExtraFilter;
            }
            //---

            //---------
            return fld;
        }

        // Controlla se il tipo di proprietà è consentito nei ModelErp
        private static bool PropertyTypeAllowed(System.Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            // Caso: Dictionary<string,string>
            if (type.IsGenericType &&
                         type.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                         type.GetGenericArguments()[0] == typeof(string) &&
                         type.GetGenericArguments()[1] == typeof(string))
            {
                return true;
            }
            // Caso: List<string>
            else if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>) &&
                    type.GetGenericArguments()[1] == typeof(string))
            {
                return true;
            }
            // Caso: List<long>
            else if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>) &&
                    type.GetGenericArguments()[1] == typeof(long))
            {
                return true;
            }
            // Caso: List<ModelErp>
            else if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(List<>) &&
                    typeof(ModelErp).IsAssignableFrom(type.GetGenericArguments()[0]))
            {
                return true;
            }
            // Caso: IDictionary<string, List<ModelErp>>
            else if (type.IsGenericType &&
                         type.GetGenericTypeDefinition() == typeof(IDictionary<,>) &&
                         type.GetGenericArguments()[0] == typeof(object) &&
                         typeof(List<ModelErp>).IsAssignableFrom(type.GetGenericArguments()[1]))
            {
                return true;
            }
            // Caso generico: ModelErp singolo
            else if (typeof(ModelErp).IsAssignableFrom(type))
            {
                return true;
            }
            // Tipi semplici e nullable
            if (underlyingType == typeof(string) ||
                underlyingType == typeof(int) ||
                underlyingType == typeof(long) ||
                underlyingType == typeof(double) ||
                underlyingType == typeof(short) ||
                underlyingType == typeof(char) ||
                underlyingType == typeof(DateTime) ||
                underlyingType == typeof(DateOnly) ||
                underlyingType == typeof(TimeOnly))
            {
                return true;
            }
            // DateRange
            else if (type == typeof(DateRange))
            {
                return true;
            }
            // Clonazione profonda di byte[]
            else if (type == typeof(byte[]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Scansiona tutti i DogField già caricati in tabProperties,
        /// raccoglie i template AutocompleteServer.ExtraFilter distinti
        /// e li precompila in parallelo.
        /// I DogField sono già tutti pronti perché questo metodo viene
        /// chiamato alla fine del costruttore.
        /// </summary>
        private async Task PrecompileExtraFiltersAsync()
        {
            try
            {
                // Raccoglie template distinti direttamente dai DogField già costruiti.
                // Usa tabProperties (tutti i field delle tabelle TAB) perché è lì
                // che vivono le proprietà con AutocompleteServerAttribute.
                var templates = tabProperties.Values
                    .Select(fld =>
                    {
                        // Recupera la PropertyInfo originale tramite reflection sul tipo della tabella
                        var prop = fld.table.tableTpy.GetProperty(fld.fieldName);
                        return prop?
                            .GetCustomAttribute<AutocompleteServerAttribute>()
                            ?.ExtraFilter;
                    })
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .ToList();

                if (templates.Count == 0) return;

                _logger.Info($"[ExtraFilterCompiler] Precompilo {templates.Count} template ExtraFilter...");

                await Parallel.ForEachAsync(
                    templates!,
                    new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
                    async (template, _) =>
                    {
                        try { await ExtraFilterCompiler.CompileAsync(template!); }
                        catch (Exception ex)
                        {
                            // Errore di sintassi nel template -> log ma non blocca il boot.
                            // (Puoi cambiare in throw se vuoi blocco bloccante come i compile error.)
                            _logger.Error($"[ExtraFilterCompiler] Errore compilazione template:\n  \"{template}\"\n  {ex.Message}");
                        }
                    });

                _logger.Info($"[ExtraFilterCompiler] Completati. {ExtraFilterCompiler.CachedCount} template in cache.");
            }
            catch (Exception ex)
            {
                _logger.Error($"[ExtraFilterCompiler] Errore generale precompilazione: {ex.Message}");
            }
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
            //%%//if (tabPrefixes != null) { tabPrefixes.Clear();  }
            if (tabIntcodes != null) { tabIntcodes.Clear();  }
            if (tabProperties != null) { tabProperties.Clear(); }
            if (tabFields != null) { tabFields.Clear(); }
            if (selfilters != null) { selfilters.Clear(); }
            if (selTypes != null) { selTypes.Clear(); }
            if (selProperties != null) { selProperties.Clear(); }
            if (selFields != null) { selFields.Clear(); }
            if (typeGraph != null) { typeGraph.Clear(); }
            GC.SuppressFinalize(this);
        }

        //***************************************************************************************************************************************************
        //*** ModelErp UTIL
        //***************************************************************************************************************************************************


        //public

        public string? getIcodeName(ModelErp obj) { return (this.tabTypes.ContainsKey(obj.GetType())) ? this.tabTypes[obj.GetType()].fldIcode?.fieldName : null; }

        //public DogTable getTable(System.Type t)
        //{
        //    // cerca per Type
        //    var match = tables.Values.FirstOrDefault(tab => tab.tableTpy == t);
        //    if (match == null) throw new InvalidOperationException($"Nessuna tabella trovata per il tipo {t.Name}");
        //    return match;
        //}
        public DogTable getTable(System.Type t)
        {
            // cerca per Type
            if (this.tabTypes.ContainsKey(t)) return this.tabTypes[t];
            throw new InvalidOperationException($"Nessuna tabella trovata per il tipo {t.Name}");
        }

        public void dumpModel() 
        {
            _logger.Info("");
            _logger.Info("TABLES");
            foreach (var tab in this.tables.Values)
            {
                if (tab == null) { _logger.Info("!!!ERROR: tab == null"); continue; }
                _logger.Info($"TABLE: {tab.tableTpy.FullName} {tab.TBAREA ?? "null"} {tab.SqlTableName ?? "null"} {tab.SqlPrefix ?? "null"} {tab.LIVEDESC ?? "null"} {tab.IS_RELTABLE ?? "null"} {tab.fldIcode?.SqlFieldName ?? "null"}");
                foreach (var fld in tab.fields)
                {
                    if (fld == null) { _logger.Info("!!!ERROR: fld == null"); continue; }
                    _logger.Info($"FIELD: {fld.fieldTyp.Name} {fld.fieldName ?? "null"} ({fld.fieldObjTyp?.FullName ?? "null"})  {fld.SqlFieldName ?? "null"}");
                }
            }
            _logger.Info("FILTERS");
            foreach (var sel in this.selfilters.Values)
            {
                if (sel == null) { _logger.Info("!!!ERROR: sel == null"); continue; }
                _logger.Info($"SELECTION: {sel.tableTpy.FullName} {sel.TBAREA ?? "null"} {sel.SqlTableName ?? "null"} {sel.SqlPrefix ?? "null"} {sel.LIVEDESC ?? "null"} {sel.IS_RELTABLE ?? "null"} {sel.fldIcode?.SqlFieldName ?? "null"}");
                foreach (var fld in sel.fields)
                {
                    if (fld == null) { _logger.Info("!!!ERROR: fld == null"); continue; }
                    _logger.Info($"FIELD: {fld.fieldTyp.Name} {fld.fieldName ?? "null"} ({fld.fieldObjTyp?.FullName ?? "null"})  {fld.SqlFieldName ?? "null"}");
                }
            }
        }

        //private
        internal DogTable _getDogTableException(string modelName, string funcName)
        {
            if (string.IsNullOrWhiteSpace(modelName)) throw new Exception($"{funcName}: modelName empty.");
            if (!this.tables.ContainsKey(modelName)) throw new Exception($"{funcName}: modelName {modelName} non trovata.");
            DogTable tab = this.tables[modelName];
            if (tab == null) throw new Exception($"{funcName}: tab [{modelName}] == null.");
            return tab;
        }
        internal DogTable _getDogTableException(System.Type modelType, string funcName)
        {
            if (modelType == null) throw new Exception($"{funcName}: modelType null.");
            if (!this.tabTypes.ContainsKey(modelType)) throw new Exception($"{funcName}: modelType {modelType.FullName} non trovata.");
            DogTable tab = this.tabTypes[modelType];
            if (tab == null) throw new Exception($"{funcName}: tabType [{modelType.FullName}] == null.");
            return tab;
        }




        //***************************************************************************************************************************************************
        //*** ModelErp COPY & CLONE
        //***************************************************************************************************************************************************

        // Restituisce una lista di oggetti ModelErp da aggiornare su DB (action in 'A','M','D'), e presenti nella gerarchia dell'oggetto corrente
        public Dictionary<ModelErp, List<string>> GetListObjToMnt(ModelErp source, string prefix) { Dictionary<ModelErp, List<string>> listObjToMnt = new Dictionary<ModelErp, List<string>>(); DogManagerClone.CloneModelErp(this, source, deepXref: false, updated: listObjToMnt, namePath: prefix); return listObjToMnt; }  //public List<ModelErp> GetListObjToMnt(ModelErp source) { List<ModelErp> listObjToMnt = new List<ModelErp>(); DogManagerClone.CloneModelErp(this, source, deepXref: false, updated: listObjToMnt); return listObjToMnt; }

        // Crea un clone Shallow dell'oggetto ModelErp, condivide le referenze interne
        // ie: gli oggetti ModelErp sono condivisi tra l'originale e il clone e presenti tutti nella cache
        public ModelErp? CopyModelErp(ModelErp? source) { return DogManagerClone.CloneModelErp(this, source, deepXref: false); }

        // Crea un clone Deep dell'oggetto ModelErp, non condivide le referenze interne
        // ie: gli oggetti ModelErp non sono più condivisi tra l'originale e il clone. viene cancellata la relazione interna con la cache
        public ModelErp? CloneModelErp(ModelErp? source) { return DogManagerClone.CloneModelErp(this, source, deepXref: true); }


        //***************************************************************************************************************************************************
        //*** ModelErp TRUNCATE CLONE
        //***************************************************************************************************************************************************

        // Crea un clone dell'oggetto ModelErp, troncando le proprietà ModelErp e le liste di ModelErp a null, mantenendo i tipi semplici e i byte[].
        // Ogni oggetto ModelErp o liste di ModelErp viene duplicato per non avere riferimenti incrociati
        // Serve per consentire la serializzazione JSON senza incorrere in loop infiniti o oggetti troppo grandi.
        public ModelErp? TruncateCloneModelErp(ModelErp? source, int maxDepth, char? action = null, string? options=null)
        {
            //return TruncateCloneModelErp(this, maxDepth, 0, action);
            return DogManagerClone.TruncateCloneModelErp(this, source, maxDepth, 0, action, options);
        }

        //Crea una copia con solo i dati vivi del record senza nessun rifermento a ModelErp Liste o dizionari esterrni eccetto le options
        public ModelErp? CleanCloneModelErp(ModelErp? source)
        {
            //return TruncateCloneModelErp(this, maxDepth: 0);
            return DogManagerClone.TruncateCloneModelErp(this, source, maxDepth: 1);
        }


        //***************************************************************************************************************************************************
        //*** Json UTIL
        //***************************************************************************************************************************************************

        //public

        // converte l'oggetto di tipo T in stringa Json eliminando eventuali riferimenti ciclici
        public static string JsonSafeSerializeToBase64Url<T>(T obj, string options = null) { return UtilHelper.ToBase64Url(JsonSafeSerialize<T>(obj, options)); }
        public static string JsonSafeSerialize<T>(T obj, string options = null) 
        {
            //string jsonStr = DogManagerJson.SafeSerialize<T>(obj, options); // System.Text.Json
            string jsonStr = DogManagerNewtonsoftJson.SafeSerialize<T>(obj, options); // Newtonsoft.Json

            if (_logger.IsTraceEnabled) { _logger.Trace($"JsonSafeSerialize_static ({typeof(T).Name}) {nameof(obj)} [{options ?? ""}] -- {DogManagerNewtonsoftJson.FormatJsonString(jsonStr)}"); }
            return jsonStr;

        }
        // converte il ModelObject in un oggetto di tipo T
        public T JsonSafeDeserialize<T>(ModelObject dataObject, string? prefix = null, string? options = null) 
        {
            //return DogManagerJson.SafeDeserialize<T>(this, dataObject);  // System.Text.Json

            if (_logger.IsTraceEnabled) { _logger.Trace($"JsonSafeDeserialize ({typeof(T).Name}) {nameof(dataObject)} {prefix ?? ""} [{options ?? ""}] -- {DogManagerNewtonsoftJson.FormatJsonString(dataObject?.data?.ToString() ?? "")}"); }
            return DogManagerNewtonsoftJson.SafeDeserialize<T>(this, dataObject, prefix, options); // Newtonsoft.Json
        }
        // converte l'oggetto Json in un oggetto di tipo T
        public T JsonSafeDeserialize<T>(object jsonObject, string? prefix = null, string? options = null) 
        {
            //return DogManagerJson.SafeDeserialize<T>(this, jsonObject);  // System.Text.Json

            if (_logger.IsTraceEnabled) { _logger.Trace($"JsonSafeDeserialize ({typeof(T).Name}) {nameof(jsonObject)} {prefix ?? ""} [{options ?? ""}] -- {DogManagerNewtonsoftJson.FormatJsonString(jsonObject?.ToString() ?? "")}"); }
            return DogManagerNewtonsoftJson.SafeDeserialize<T>(this, jsonObject, prefix, options); // Newtonsoft.Json
        }
        // converte l'oggetto Json in un oggetto di tipo T
        public static T JsonStaticSafeDeserialize<T>(object jsonObject, string? prefix = null, string? options = null) 
        {
            //return DogManagerJson.SafeDeserialize<T>(null, jsonObject);  // System.Text.Json

            if (_logger.IsTraceEnabled) { _logger.Trace($"JsonSafeDeserialize_static ({typeof(T).Name}) {nameof(jsonObject)} {prefix ?? ""} [{options ?? ""}] -- {DogManagerNewtonsoftJson.FormatJsonString(jsonObject?.ToString() ?? "")}"); }
            return DogManagerNewtonsoftJson.SafeDeserialize<T>(null, jsonObject, prefix, options); // Newtonsoft.Json
        } 

        //***************************************************************************************************************************************************
        //*** ICODE
        //***************************************************************************************************************************************************

        //public

        public string GenerateIcode() { return $"{this._dbRoot}{GenCodeHelper.EpochIcode()}"; }

        //***************************************************************************************************************************************************
        //*** BLOB
        //***************************************************************************************************************************************************

        //public         

        public static List<object> XdataFmtTypes = new List<object>
            {
                new { value = "Referto-Pdf",    text = "Referto PDF" },
                new { value = "Referto-Xml",    text = "Referto XML" },
                new { value = "Referto-Txt", text = "Referto Testo" },
                new { value = "CDA2-Xml", text = "Referto CDA2" },
                new { value = "Image-Jpeg", text = "Immagine JPEG" },
                new { value = "Image-Jpeg", text = "Immagine PNG" },
                new { value = "Doc-Text", text = "Documento Testo" },
                new { value = "Video-Mp4", text = "Video MP4" },
                new { value = "Audio-Mpeg", text = "Audio MP3" },
            };

        //-----------------------------------------------------------------------------------------------
        public sealed class BlobStreamResult
        {
            public Stream Stream { get; init; } = default!;
            public byte[] Bytes { get; init; } = default!;
            public string ContentType { get; init; } = "application/octet-stream";
            public long Length { get; init; }
        }
        internal BlobStreamResult OpenBlobStream(string modelName, object blobIcode, long offset) {
            return _OpenBlobStream(_getDogTableException(modelName, "OpenBlobStream"), blobIcode, offset);
        }
        public BlobStreamResult OpenBlobStream<T>(object blobIcode, long offset) where T : ModelErp
        {
            return _OpenBlobStream(_getDogTableException(typeof(T), "OpenBlobStream"), blobIcode, offset);
        }
        private BlobStreamResult _OpenBlobStream(DogTable tab, object blobIcode, long offset)
        {
            if (string.IsNullOrWhiteSpace(tab.tabXdata.SqlTableName)) throw new Exception($"OpenBlobStream: tab [{tab.tableTpy.FullName}] SqlXdataTableName is empty.");
            if (string.IsNullOrWhiteSpace(tab.tabXdata.fldIcode.SqlFieldName)) throw new Exception($"OpenBlobStream: tab [{tab.tableTpy.FullName}] SqlXdataIcodeName is empty.");
            if (string.IsNullOrWhiteSpace(tab.tabXdata.fldXdatum.SqlFieldName)) throw new Exception($"OpenBlobStream: tab [{tab.tableTpy.FullName}] SqlXdataXdatumName is empty.");

            //  Open Blob Stream
            ////////return _getDbMg().OpenBlobStream(tab.tabXdata.SqlTableName, tab.tabXdata.fldIcode.SqlFieldName, blobIcode, tab.tabXdata.fldXdatum.SqlFieldName, offset);

            // Leggo direttamente il blob in memoria, con dimesione di soglia. Se il blob è superiore alla soglia allora riapro in modalità stream. In questo modo evito di aprire stream per leggere blob piccoli e riduco il numero di connessioni aperte al DB.
            const int DOCUMENT_SIZE = 1024 * 1024;  // 1 Mb
            string transactionId = null; int maxRecords = 1; long maxBlobSize = DOCUMENT_SIZE; string options = " ";
            //--
            List<object> rowIdList = new List<object> { blobIcode };
            List<string> fmtList = new List<string> { };
            DogField fldXref = tab.tabXdata.fldMref;  //DogField fldXref = tab.tabXdata.fldGetFirstByOption("[MREF]");
            IDictionary<string, object> xdataParameters = new Dictionary<string, object>();
            string sqlXdata = DogManagerCache.sqlListEx(this, tab, ref xdataParameters, null, null, rowIdList, fmtList, true, null, options: options);
            if (options.Contains("[skipCheckSqlParms]") == false && (sqlXdata.Contains('\'') || sqlXdata.Contains('#') || sqlXdata.Contains("--"))) { throw new FormatException($"SQL: FormatException: {nameof(sqlXdata)}"); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            Dictionary<object, ModelXdata> ret = _getDbMg().ExecuteQueryXdata(null, sqlXdata, EncodeSpecialFields(xdataParameters, options), transactionId, maxRecords, maxBlobSize, options);  // NON LEGGO I BLOB
            byte[] xdatum = ret.Values?.First()?.Xdatum;
            string mime = ret.Values?.First()?._mimeXdatum ?? "application/octet-stream";
            long size = ret.Values?.First()?._sizeXdatum ?? 0;
            //-- Se il blob è inferiore alla soglia restituisco i byte direttamente in memoria, altrimenti apro lo stream
            if (size <= maxBlobSize)
            {
                return new DogManager.BlobStreamResult
                {
                    Stream = null,
                    Bytes = xdatum,
                    ContentType = mime,
                    Length = size
                };
            }
            Stream stream = _getDbMg().OpenBlobStream2(tab.tabXdata.SqlTableName, tab.tabXdata.fldIcode.SqlFieldName, blobIcode, tab.tabXdata.fldXdatum.SqlFieldName, offset);
            return new DogManager.BlobStreamResult
            {
                Stream = stream,
                Bytes = null,
                ContentType = mime,
                Length = size
            };

        }

        //-----------------------------------------------------------------------------------------------
        internal async Task<ModelXdata> MntXdataBlobStreamAsync(string modelName, char action, string? icodeStr, string? timestampHex, string mrefStr, string? descr, string? fmt, Stream dataStream, string transactionId) 
        {
            return await _MntXdataBlobStreamAsync(_getDogTableException(modelName, "MntXdataBlobStreamAsync"), action, icodeStr, timestampHex, mrefStr, descr, fmt, dataStream, transactionId);
        }
        public async Task<ModelXdata> MntXdataBlobStreamAsync<T>(char action, string? icodeStr, string? timestampHex, string mrefStr, string? descr, string? fmt, Stream dataStream, string transactionId) where T : ModelErp
        {
            return await _MntXdataBlobStreamAsync(_getDogTableException(typeof(T), "MntXdataBlobStreamAsync"), action, icodeStr, timestampHex, mrefStr, descr, fmt, dataStream, transactionId);
        }
        //private async Task<DogResult> _MntXdataBlobStreamAsync(DogTable tab, char action, string? icodeStr, string? timestampHex, string mrefStr, string? descr, string? fmt, Stream dataStream, string transactionId)
        //{
        //    if (action != 'A' && action != 'M' && action != 'D') throw new ArgumentException("MntXdataBlobStreamAsync: wrong action '{action}'.");
        //    if (string.IsNullOrWhiteSpace(mrefStr)) throw new ArgumentException("MntXdataBlobStreamAsync: Mref non valido");
        //    //if (string.IsNullOrEmpty(fmt)) throw new ArgumentException("MntXdataBlobStreamAsync: Fmt non valido");
        //    if (dataStream == null) throw new ArgumentNullException("MntXdataBlobStreamAsync: Stream {nameof(dataStream)} is null");


        //    //convert mref to long if necessary
        //    object? icode = null; object mref = null;
        //    try { mref = (tab.fldIcode.fieldTyp is long) ? long.Parse(mrefStr) : mrefStr; }
        //    catch (Exception ex) { throw new Exception($"MntXdataBlobStreamAsync: tab [{tab.tableTpy.FullName}] mref {mrefStr} non convertibile a long.", ex); }

        //    ModelXdata xdataObj = new ModelXdata
        //    {
        //        action = action,
        //        Icode = icode,
        //        Timestamp = UtilHelper.HexStringToByteArray(timestampHex),
        //        Mref = mref,
        //        Descr = descr,
        //        Fmt = fmt,
        //        Xdatum = null,      // <--- deve essere null per usare il parametro stream in fase di costruzione dello statement sql
        //        _streamXdatum = dataStream,
        //    };


        //    List<DogResult> dogResult = await MntXdataListAsync(tab, new List<ModelXdata> { xdataObj }, transactionId: transactionId);
        //    return dogResult[0];
        //}


        private async Task<ModelXdata> _MntXdataBlobStreamAsync(DogTable tab, char action, string? icodeStr, string? timestampHex, string mrefStr, string? descr, string? fmt, Stream dataStream, string transactionId)
        {
            object? icode = null; object mref = null; byte[] bytesArray = null;
            if (action == 'A')
            {
                if (string.IsNullOrWhiteSpace(mrefStr)) throw new ArgumentException("MntXdataBlobStreamAsync: Mref non valido");
                if (string.IsNullOrEmpty(fmt)) throw new ArgumentException("MntXdataBlobStreamAsync: Fmt non valido");
                if (dataStream == null) throw new ArgumentNullException("MntXdataBlobStreamAsync: Stream {nameof(dataStream)} is null");
            }
            else if (action == 'M' || action == 'D')
            {
                if (string.IsNullOrWhiteSpace(icodeStr)) throw new ArgumentException("MntXdataBlobStreamAsync: Icode non valido");
                if (string.IsNullOrWhiteSpace(timestampHex)) throw new ArgumentException("MntXdataBlobStreamAsync: TimestampHex non valido");
            }
            else
            {
                throw new ArgumentException("MntXdataBlobStreamAsync: wrong action '{action}'.");
            }
            //convert to long if necessary
            if (!string.IsNullOrWhiteSpace(icodeStr))
            {
                try { icode = (tab.tabXdataIcodeTyp is long) ? long.Parse(icodeStr) : icodeStr; }  
                catch (Exception ex) { throw new Exception($"MntXdataBlobStreamAsync: tab [{tab.tableTpy.FullName}] icode {icodeStr} non convertibile a long.", ex); }
            }
            if (!string.IsNullOrWhiteSpace(mrefStr))
            {
                try { mref = (tab.fldIcode.fieldTyp is long) ? long.Parse(mrefStr) : mrefStr; }  
                catch (Exception ex) { throw new Exception($"MntXdataBlobStreamAsync: tab [{tab.tableTpy.FullName}] mref {mrefStr} non convertibile a long.", ex); }
            }
            if (dataStream != null) {
                //if (dataStream.CanSeek) dataStream.Position = 0;
                //using var ms = new MemoryStream();
                //dataStream.CopyTo(ms);
                if (dataStream.CanSeek) dataStream.Position = 0;
                using var ms = new MemoryStream();
                await dataStream.CopyToAsync(ms);   // <-- corretto per contesti async
                ms.Position = 0;                    // sicurezza aggiuntiva prima di ToArray()
                bytesArray = ms.ToArray();                 // <-- attenzione: questo carica tutto il contenuto del file in memoria. Va bene solo se i file non sono troppo grandi. In alternativa si potrebbe passare lo Stream direttamente al metodo di esecuzione della query, ma richiederebbe una modifica più profonda del codice.
            }

            ModelXdata xdataObj = new ModelXdata
            {
                action = action,
                Icode = icode,
                Timestamp = UtilHelper.HexStringToByteArray(timestampHex),
                Mref = mref,
                Descr = descr,
                Fmt = fmt,
                Xdatum = bytesArray,      // <--- converto lo Stream in array. Va bene solo se i file non sono troppo grandi. In alternativa si potrebbe passare lo Stream direttamente al metodo di esecuzione della query, ma richiederebbe una modifica più profonda del codice.
            };

            // richiamo la funzione sincrona in modo asincrono
            List<ModelXdata> xdataResult = await Task.Run(() => MntXdataList(tab, new List<ModelXdata> { xdataObj }, transactionId: transactionId));
            return xdataResult[0];
        }










        //***************************************************************************************************************************************************
        //*** AUTOCOMPLETE
        //***************************************************************************************************************************************************

        internal List<Choice> AutocompleteGetAll(string modelName, string? extraWhere = null, string? transactionId = null, int maxRecords = -1)  
        {
            //return DogManagerQuery.AutocompleteGetAll<T>(this, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(modelName, "AutocompleteGetAll"), "GetAll", extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }
        internal List<Choice> AutocompleteGetSelect(string modelName, string term, string? modelPropertyName = null, Dictionary<string, List<string>> extraFields = null, bool caseInsensitive = true, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) 
        {
            //return DogManagerQuery.AutocompleteGetSelect<T>(this, term: term, caseInsensitive: caseInsensitive, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(modelName, "AutocompleteGetSelect"), "GetSelect", term: term, modelPropertyName: modelPropertyName, extraFields: extraFields, caseInsensitive: caseInsensitive, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }
        internal List<Choice> AutocompletePreLoad(string modelName, List<string> values, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) 
        {
            //return DogManagerQuery.AutocompletePreLoad<T>(this, values: values, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(modelName, "AutocompletePreLoad"), "PreLoad", values: values, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }
        
        public List<Choice> AutocompleteGetAll<T>(string? modelPropertyName = null, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new()
        {
            //return DogManagerQuery.AutocompleteGetAll<T>(this, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(typeof(T), "AutocompleteGetAll"), "GetAll", modelPropertyName: modelPropertyName, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }
        public List<Choice> AutocompleteGetSelect<T>(string term, string? modelPropertyName = null, Dictionary<string, List<string>> extraFields = null, bool caseInsensitive = true, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new()
        {
            //return DogManagerQuery.AutocompleteGetSelect<T>(this, term: term, caseInsensitive: caseInsensitive, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(typeof(T), "AutocompleteGetSelect"), "GetSelect", term: term, modelPropertyName: modelPropertyName, extraFields: extraFields, caseInsensitive: caseInsensitive, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }
        public List<Choice> AutocompletePreLoad<T>(List<string> values, string? extraWhere = null, string? transactionId = null, int maxRecords = -1) where T : ModelErp, new()
        {
            //return DogManagerQuery.AutocompletePreLoad<T>(this, values: values, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
            return DogManagerQuery.Autocomplete_Int(this, _getDogTableException(typeof(T), "AutocompletePreLoad"), "PreLoad", values: values, extraWhere: extraWhere, transactionId: transactionId, maxRecords: maxRecords);
        }

        public List<Choice> AutocompleteQuery(string sql, IDictionary<string, object> parameters, string? transactionId = null, int maxRecords = -1, string options = "") 
        { 
            return ExecuteQuery<Choice>(sql, parameters, transactionId, maxRecords, options); 
        }

        //***************************************************************************************************************************************************
        //*** TRANSAZIONI
        //***************************************************************************************************************************************************

        //public

        public string BeginTransaction(string? transactionId, string transactionName = "") { return _getDbMg().BeginTransaction(transactionId, transactionName); }
        public void CommitTransaction(string transactionId, string transactionName = "") { _getDbMg().CommitTransaction(transactionId, transactionName); }
        public void RollbackTransaction(string transactionId, string transactionName = "") { _getDbMg().RollbackTransaction(transactionId, transactionName); }


        //***************************************************************************************************************************************************
        //*** QUERY - MANTAIN
        //***************************************************************************************************************************************************

        //public

        // ExecuteScalar
        public bool RecordExists(string tableName, string keyField, object keyValue, string? transactionId) 
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            return _getDbMg().RecordExists(tableName, keyField, keyValue, transactionId); 
        }
        public byte[] ReadBlob(string tableName, string keyField, object keyValue, string blobField, int pageNumber, string? transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            return _getDbMg().ReadBlob(tableName, keyField, keyValue, blobField, pageNumber, transactionId);
        }
        public void WriteBlob(string tableName, string keyField, object keyValue, string blobField, byte[] data, int pageNumber, string? transactionId)
        {
            if (_modelMode == "FREE") throw new InvalidOperationException("WriteBlob non disponibile in modalità FREE.");  // WriteBlob non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            _getDbMg().WriteBlob(tableName, keyField, keyValue, blobField, data, pageNumber, transactionId);
        }

        //ExecuteQuery
        public DataTable ExecuteQuery(string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, string options = "")
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return DecodeSpecialTable(_getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), transactionId, maxRecords, options), options: options);
        }
        public List<T> ExecuteQuery<T>(string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, string options = "") 
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (options.Contains("[skipCheckSqlParms]") == false && (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--"))) { throw new FormatException($"SQL: FormatException: {nameof(sql)}"); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return DecodeSpecialTable<T>(_getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), transactionId, maxRecords, options), options: options);
        }
        //public Dictionary<object, ModelErp> ExecuteQuery(Dictionary<object, ModelErp>? dict, System.Type modelType, string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, string options = "")
        //{
        //    if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
        //    if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
        //    if (dict == null) dict = new Dictionary<object, ModelErp>();
        //    if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
        //    if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
        //    if (options.Contains("[skipCheckSqlParms]") == false && (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--"))) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
        //    return DecodeSpecialTable(dict, modelType, _getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), transactionId, maxRecords, options), options: options);
        //}

        public Dictionary<object, ModelErp> ExecuteQuery(Dictionary<object, ModelErp>? dict, System.Type modelType, string sql, IDictionary<string, object> parameters, string? transactionId, int maxRecords, string options = "")
        {
            bool fillXdata = false; List<string> fmtList = null;
            return ExecuteQueryEx(dict, modelType, sql, parameters, fillXdata, fmtList, transactionId, maxRecords, options: options);
        }
        public Dictionary<object, ModelErp> ExecuteQueryEx(Dictionary<object, ModelErp>? dict, System.Type modelType, string sql, IDictionary<string, object> parameters, bool fillXdata, List<string> fmtList, string? transactionId, int maxRecords, string options = "")
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
            if (dict == null) dict = new Dictionary<object, ModelErp>();
            if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
            if (sql == null) { throw new ArgumentNullException(nameof(sql)); }
            if (options.Contains("[skipCheckSqlParms]") == false && (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--"))) { throw new FormatException($"SQL: FormatException: {nameof(sql)}"); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            Dictionary<object, ModelErp> dict2 = DecodeSpecialTable(dict, modelType, _getDbMg().ExecuteQuery(sql, EncodeSpecialFields(parameters, options), transactionId, maxRecords, options), options: options);
            if (fillXdata)  //riempo i dati estesi solo se richiesto, altrimenti lascio la proprietà Xdata a null. In ogni caso: NON CARICO IL CONTENUTO DEI BLOB (maxBlobSize = 0)
            {
                List<object> rowIdList = new List<object>();
                foreach (var value in dict2.Values) { if (value.Xdata == null) { rowIdList.Add(value.getIcode()); value.Xdata = new Dictionary<object, ModelXdata>(); } }
                DogTable tab = this._getDogTableException(modelType, "ExecuteQueryEx");
                ModelErp[]? cloneRowRecList = dict2.Values?.Select(x => CleanCloneModelErp(x)).ToArray(); //clone dei recods di selezione
                Dictionary<object, ModelXdata> xdataDict = ExecuteQueryXdataEx(null, tab, true, rowIdList, fmtList, transactionId, maxRecords, (long)0, cloneRowRecList, options);  // NON LEGGO I BLOB
                //??//foreach (var value in xdataDict.Values) { dict2[value.Mref].Xdata[value.Icode] = value; }

                //System.Type keyType = (dict2.Keys.Count > 0) ? dict2.Keys.First()?.GetType() : null;    // 1. Ricava il tipo esatto della chiave di dict2
                //foreach (var value in xdataDict.Values) {
                //    object convertedKey = (keyType != null) ? Convert.ChangeType(value.Mref, keyType) : value.Mref;  // 2. Converte la stringa Mref nel tipo della chiave a runtime
                //    dict2[convertedKey].Xdata[value.Icode] = value; // 3. Usa la chiave convertita per accedere a dict2
                //}

                bool assignToLong = (dict2.Keys.Count > 0 && dict2.Keys.First()?.GetType() != null && typeof(long).IsAssignableFrom(dict2.Keys.First()?.GetType())) ? true : false;
                if (assignToLong)   { foreach (var value in xdataDict.Values) { dict2[((IConvertible)value.Mref).ToInt64(null)].Xdata[value.Icode] = value; } }
                else                { foreach (var value in xdataDict.Values) { dict2[value.Mref].Xdata[value.Icode] = value; } }


            }
            return dict2;
        }
        public Dictionary<object, ModelXdata> ExecuteQueryXdataEx(Dictionary<object, ModelXdata>? dict, DogTable tab, bool isMrefRowIdList, List<object> rowIdList, List<string> fmtList, string? transactionId, int maxRecords, long maxBlobSize, ModelErp[]? cloneRowRecList, string options = "")
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
            if (dict == null) dict = new Dictionary<object, ModelXdata>();
            if (tab == null) { throw new ArgumentNullException(nameof(tab)); }
            if (tab.tabXdata == null) { throw new ArgumentNullException(nameof(tab.tabXdata)); }
            if (rowIdList == null) { throw new ArgumentNullException(nameof(rowIdList)); }
            if (fmtList == null) { fmtList = new List<string> { }; }
            DogField fldXref = tab.tabXdata.fldMref;  //DogField fldXref = tab.tabXdata.fldGetFirstByOption("[MREF]");
            IDictionary<string, object> xdataParameters = new Dictionary<string, object>();
            string sqlXdata = DogManagerCache.sqlListEx(this, tab, ref xdataParameters, null, (isMrefRowIdList) ? fldXref : null, rowIdList, fmtList, true, cloneRowRecList, options: options);
            if (options.Contains("[skipCheckSqlParms]") == false && (sqlXdata.Contains('\'') || sqlXdata.Contains('#') || sqlXdata.Contains("--"))) { throw new FormatException($"SQL: FormatException: {nameof(sqlXdata)}"); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
            return _getDbMg().ExecuteQueryXdata(null, sqlXdata, EncodeSpecialFields(xdataParameters, options), transactionId, maxRecords, maxBlobSize, options);  // NON LEGGO I BLOB
        }

        //ExecNonQuery
        public void DeleteRecord(string tableName, string keyField, IDictionary<string, object> fields, string? transactionId)
        {
            if (_modelMode == "FREE") throw new InvalidOperationException("DeleteRecord non disponibile in modalità FREE.");  // DeleteRecord non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
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
            if (_modelMode == "FREE") throw new InvalidOperationException("ImportCsvToTable non disponibile in modalità FREE.");  // ImportCsvToTable non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.
            _getDbMg().ImportCsvToTable(tableName, filePath);
        }

        //***************************************************************************************************************************************************
        //*** MANTAIN
        //***************************************************************************************************************************************************


        ////////public void MantainRecord(char action, string tableName, string keyField, string timestampField, string deleteField, IDictionary<string, object> parameters, string? transactionId, string options = "")
        ////////{
        ////////    if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
        ////////    _getDbMg().MantainRecord(action, tableName, keyField, timestampField, deleteField, EncodeSpecialFields(parameters, options), options, transactionId);
        ////////}


        //***************************************************************************************************************************************************
        //*** ENCODE-DECODE
        //***************************************************************************************************************************************************

        internal Dictionary<string, object> EncodeSpecialFields(IDictionary<string, object> fields, string options="")  // usata anche da DogManagerTopologicalSort
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
            string Key = "", Value = ""; string tbName = "#TABLE_UNKNOWN#";
            try
            { 
                if (dataTable == null) return null;
                foreach (DataRow row in dataTable.Rows)
                {
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        Key = column.ColumnName; Value = row[column]?.ToString() ?? "";
                        row[column] = DecodeSpecialField(null, tbName, column.ColumnName, row[column], options + " [ToDataRow]"); // in caso di DataRow => uso DBNull
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



        //public Dictionary<object, ModelDog> DecodeSpecialTable(Dictionary<object, ModelDog> dict, System.Type modelType, DataTable dt, string options = "")
        //{
        //    if (dict == null) dict = new Dictionary<object, ModelDog>();
        //    if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
        //    try
        //    {
        //        if (dt == null) return dict;
        //        // Usa il tipo dell'oggetto per chiamare la funzione DecodeSpecialRow generica
        //        MethodInfo method_DecodeSpecialRow = typeof(DogManager).GetMethod("DecodeSpecialRow", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(modelType);
        //        //--
        //        foreach (DataRow row in dt.Rows)
        //        {
        //            ModelErp item = (ModelErp)method_DecodeSpecialRow.Invoke(this, new object[] { row, options }); // ModelErp item = DecodeSpecialRow<ModelErp>(row, options);
        //            if (item != null && item.getIcode() != null) dict[item.getIcode()] = item;
        //        }
        //        return dict;
        //    }
        //    catch (System.Exception ex)
        //    {
        //        throw new InvalidCastException($"DecodeSpecialTable[ModelErp]: {ex.Message}.");
        //    }
        //}


        public Dictionary<object, ModelErp> DecodeSpecialTable(Dictionary<object, ModelErp> dict, System.Type modelType, DataTable dt, string options = "")
        {
            return DecodeSpecialTable<ModelErp>(dict, modelType, dt, options: options);
        }
        public Dictionary<object, ModelDog> DecodeSpecialTableDog(Dictionary<object, ModelDog> dict, System.Type modelType, DataTable dt, string options = "")
        {
            return DecodeSpecialTable<ModelDog>(dict, modelType, dt, options: options);
        }
        public Dictionary<object, ModelXdata> DecodeSpecialTableXdata(Dictionary<object, ModelXdata> dict, DataTable dt, string options = "")
        {
            return DecodeSpecialTable<ModelXdata>(dict, typeof(ModelXdata), dt, options: options);
        }
        public Dictionary<object, T> DecodeSpecialTable<T>(Dictionary<object, T> dict, System.Type modelType, DataTable dt, string options = "") where T : ModelDog
        {
            if (dict == null) dict = new Dictionary<object, T>();
            if (modelType == null) { throw new ArgumentNullException(nameof(modelType)); }
            try
            {
                if (dt == null) return dict;
                // Usa il tipo dell'oggetto per chiamare la funzione DecodeSpecialRow generica
                MethodInfo method_DecodeSpecialRow = typeof(DogManager).GetMethod("DecodeSpecialRow", BindingFlags.Public | BindingFlags.Instance).MakeGenericMethod(modelType);
                //--
                foreach (DataRow row in dt.Rows)
                {
                    T item = (T)method_DecodeSpecialRow.Invoke(this, new object[] { row, options }); // ModelErp item = DecodeSpecialRow<ModelErp>(row, options);
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
            string Key = "", Value = ""; string tbName = "#TABLE_UNKNOWN#";
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
                        objArr[i] = DecodeSpecialField(null, tbName, column.ColumnName, dr[column.ColumnName], options);
                        Key = ""; Value = "";
                    }
                    return (T)(Object)objArr;
                }
                //decode in object model
                T obj = Activator.CreateInstance<T>();
                if (this.tabTypes.ContainsKey(temp)) tbName = this.tabTypes[temp].SqlTableName;
                for (int i = 0; i < dr.Table.Columns.Count; i++)
                {
                    DataColumn column = dr.Table.Columns[i];
                    foreach (PropertyInfo pro in temp.GetProperties())
                    {
                        if (pro.Name == column.ColumnName)
                        {
                            Key = column.ColumnName; Value = dr[column.ColumnName]?.ToString() ?? "";
                            pro.SetValue(obj, DecodeSpecialField(pro.PropertyType, tbName, column.ColumnName, dr[column.ColumnName], options), null);
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
        private object DecodeSpecialField(System.Type type, string tabName, string colName, object value, string options = "")
        {
            string tabcolName = $"{tabName}.{colName}";
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
                    if (type == typeof(DateOnly?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optDATE == true))
                    {
                        if (strVal == "" || strVal == "/  /" || strVal == DB_DATE_MIN) return DateOnly.MinValue;
                        if (strVal == DB_DATE_MAX) return DateOnly.MaxValue;
                        if (DateOnly.TryParseExact((string)value, DB_FORMAT_DATE, null, DateTimeStyles.None, out DateOnly date)) return date;
                    }
                    if (type == typeof(TimeOnly?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optTIME == true))
                    {
                        if (strVal == "" || strVal == ":  :" || strVal == DB_TIME_EMPTY) return null;
                        if (TimeOnly.TryParseExact(value.ToString(), DB_FORMAT_TIME, null, DateTimeStyles.None, out TimeOnly time)) return time;
                    }
                    if (type == typeof(DateTime?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optDATETIME == true))
                    {
                        if (strVal == "" || strVal == "/  /" || strVal == "/  /     :  :") return DateTime.MinValue;
                        if (this.tabFields[tabcolName]?.optDATE == true && DateTime.TryParseExact(value.ToString(), DB_FORMAT_DATE, null, DateTimeStyles.None, out DateTime datetimeDate)) return datetimeDate;
                        else if (this.tabFields[tabcolName]?.optTIME == true && DateTime.TryParseExact(value.ToString(), DB_FORMAT_TIME, null, DateTimeStyles.None, out DateTime datetimeTime)) return datetimeTime;
                        else if (DateTime.TryParseExact(value.ToString(), DB_FORMAT_DATETIME, null, DateTimeStyles.None, out DateTime datetime)) return datetime;
                    }
                }
                if (value.GetType() == typeof(System.DateTime) || value.GetType() == typeof(System.DateTime?))
                {
                    if (type == typeof(DateOnly?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optDATE == true))
                    {
                        DateOnly dt = DateOnly.FromDateTime((DateTime)value); return dt;
                    }
                    if (type == typeof(DateTime?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optDATETIME == true))
                    {
                        return (DateTime?)value;
                    }
                }
                if (value.GetType() == typeof(System.TimeSpan) || value.GetType() == typeof(System.TimeSpan?))
                {
                    if (type == typeof(TimeOnly?) || (this.tabFields.ContainsKey(tabcolName) && this.tabFields[tabcolName]?.optTIME == true))
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

                //!!!!!!!!!! TRIM END per stringhe non mappate esplicitamente !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (value.GetType() == typeof(string)) return value.ToString().TrimEnd(); 
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                return value;
            }
            catch (System.Exception ex)
            {
                throw new InvalidCastException($"DecodeSpecialField[{tabcolName}={value?.ToString() ?? ""}]: Errore nella decodifica del campo -- {ex.Message}.");
            }
        }


        //***************************************************************************************************************************************************
        //*** List - Row - Add - Upd
        //***************************************************************************************************************************************************


        //carica list oggetti con il contenuto del DB in base alla struttura in selezione  
        public List<T> List<T>(ModelErp selModel, string? transactionId, int maxRecords, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return List<T>(selModel, null, true, null, ref dogCache, transactionId, maxRecords, options: options);  }
        public List<T> List<T>(ModelErp selModel, List<string> xrefFrom, bool fillXdata, List<string> fmtList, ref DogCache dogCache, string? transactionId, int maxRecords, string options = "") where T : ModelErp
        {
            if (selModel == null) { throw new ArgumentNullException("Null " + nameof(selModel)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            return List_int<T>(selModel, null, xrefFrom, fillXdata, fmtList, ref dogCache, transactionId, maxRecords, options: options);
        }
        public List<T> List<T>(List<object> lstRowId, string? transactionId, int maxRecords, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return List<T>(lstRowId, null, true, null, ref dogCache, transactionId, maxRecords, options: options); }
        public List<T> List<T>(List<object> lstRowId, List<string> xrefFrom, bool fillXdata, List<string> fmtList, ref DogCache dogCache, string? transactionId, int maxRecords, string options = "") where T : ModelErp
        {
            if (lstRowId == null) { throw new ArgumentNullException("Null " + nameof(lstRowId)); }
            if (lstRowId.Count() == 0) { throw new ArgumentNullException("Empty " + nameof(lstRowId)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            return List_int<T>(null, lstRowId, xrefFrom, fillXdata, fmtList, ref dogCache, transactionId, maxRecords, options: options);
        }
        //carica row con il contenuto del DB in base all'icode'  
        public T Row<T>(object icode, string? transactionId, string options = "") where T : ModelErp { DogCache dogCache = new DogCache(); return Row<T>(icode, null, true, null, ref dogCache, transactionId, options: options); }
        public T Row<T>(object icode, List<string> xrefFrom, bool fillXdata, List<string> fmtList, ref DogCache dogCache, string? transactionId, string options = "") where T : ModelErp
        {
            if (UtilHelper.IsNullOrEmptyObject(icode)) { throw new ArgumentNullException(nameof(icode)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            int maxRecords = -1;  // !!!!! NESSUN LIMITE ALLE RIGHE RESTITUITE NELLE TABELLE RELAZIONATE !!!!!!
            List<T> outList = List_int<T>(null, new List<object>() { icode }, xrefFrom, fillXdata, fmtList, ref dogCache, transactionId, maxRecords, options: options); //
            if (outList.Count() == 0) throw new DatabaseException(ERR_DB_BAD_IDEN, $"Nessun record corrispondente alla Chiave Primaria specificata [{icode}].", null);
            else if (outList.Count() > 1) throw new DatabaseException(ERR_DB_AMBIGOUS, $"La Chiave Primaria specificata è ambiqua. Più di un record trovato  [{icode}].", null);
            return outList[0];
        }
        private List<T> List_int<T>(ModelErp selModel, List<object> lstRowId, List<string> xrefFrom, bool fillXdata, List<string> fmtList, ref DogCache dogCache, string? transactionId, int maxRecords, string options = "") where T : ModelErp
        {
            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_QUERY_MAX_RECORDS;
            List<object> outKeyList;
            if (selModel == null && lstRowId == null) { throw new ArgumentNullException(nameof(selModel) + " - " + nameof(lstRowId)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            if (xrefFrom == null) { xrefFrom = new List<string>(); }
            //T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type

            DogTable tab = this._getDogTableException(typeof(T), "List_int");  // verifico che esista la tabella per quel tipo di oggetto, altrimenti è un errore di configurazione grave e fermo tutto con un'eccezione

            IDictionary<string, object> parameters = new Dictionary<string, object>();
            //se richiesto esplicitamente, uso in visualizzazione i record già presenti in cache 
            //funziona solo se effettuo una query per lista di Icode 
            if (options.Contains("[ICODELIST_USE_CACHE]") && lstRowId != null) // !!!!!!!!!! questa opzione non serve molto e forse andrebbe eliminata !!!!!!!!!!!!!!!!
            {
                //foreach(var item in lstRowId) { 
                //    if (dogCache.dbCache.ContainsKey(objModel.GetType()) && item != null && dogCache.dbCache[objModel.GetType()][item] != null) // Controllo se l'oggetto è già in cache
                //    {
                //        lstRowId.Remove(item); // Rimuovo l'oggetto dalla lista dei record da cercare nel DB
                //    }
                //}
                for (int i = lstRowId.Count - 1; i >= 0; i--)
                {
                    var item = lstRowId[i];
                    if (dogCache.dbCache.ContainsKey(tab.tableTpy) && item != null && dogCache.dbCache[tab.tableTpy].ContainsKey(item)) // Controllo se l'oggetto è già in cache
                    {
                        lstRowId.RemoveAt(i); // Rimuovo l'oggetto dalla lista dei record da cercare nel DB // sicuro perché parti dalla fine
                    }
                }
            }

            //string sql = DogManagerCache.sqlList(this, objModel, ref parameters, selModel, null, lstRowId, options);
            string sql = DogManagerCache.sqlListEx(this, tab, ref parameters, selModel, null, lstRowId, null, false, null, options: options);    //cloneRowRecList=null, perchè fldXref=null

            //init
            DogManagerCache.CacheFuncInit(this, ref dogCache, "List_int", 'R', tab.tableTpy, options: options); // Inizializzo la cache per il tipo di oggetto, in modo da poterla usare per le query successive.

            //outList = this.ExecuteQuery<T>(sql, parameters, options: options);

            //!!!//Dictionary<object, ModelErp> outDict = this.ExecuteQuery(null, objModel.GetType(), sql, parameters, transactionId, maxRecords, options: options);  //dict contiene una copia di tutti i record estratti in tutte le sessioni
            Dictionary<object, ModelErp> outDict = this.ExecuteQueryEx(null, tab.tableTpy, sql, parameters, fillXdata, fmtList, transactionId, maxRecords, options: options);  //dict contiene una copia di tutti i record estratti in tutte le sessioni
            //!!!//

            outKeyList = DogManagerCache.CacheAddDict(this, ref dogCache, tab.tableTpy, outDict, options: options);

            // se richiesto riempio i riferimenti all'oggetto referenziati nelle tabelle esterne
            if (xrefFrom.Count > 0 && outKeyList.Count > 0)
            {
                dogCache.AddRuleXrefFrom(xrefFrom); // Aggiungo le regole di xrefFrom al DogCache per ricalcolare le relazioni in fase di ricostruzione dei legami della cache (ie: CacheFillNull()).
                foreach (var xrefFromPropertyName in xrefFrom)
                {
                    DogField fld = this.tabProperties[xrefFromPropertyName];
                    if (fld?.XrefObj?.table?.tableTpy != tab.tableTpy) continue;
                    if (fld?.table == null) continue;

                    //System.Type xrefFromType = fld?.table?.tableTpy;
                    //if (xrefFromType == null) continue;

                    //ModelErp xrefFromObj = (ModelErp)Activator.CreateInstance(xrefFromType); // create an instance of that type
                    IDictionary<string, object> xrefFromParameters = new Dictionary<string, object>();

                    //string xrefFromSql = DogManagerCache.sqlList(this, xrefFromObj, ref xrefFromParameters, null, fld, outKeyList, options);
                    //---
                    //ModelErp[]? cloneRowRecList = dogCache.GetDictionary(tab.tableTpy)?.Values?.Select(x => CleanCloneModelErp(x)).ToArray(); //clone dei recods di selezione
                    DogCache localDogCache = dogCache;  // per evitare il problema della closure nella Select LINQ
                    ModelErp[]? cloneRowRecList = outKeyList.Select(id => CleanCloneModelErp(localDogCache.GetObject(tab.tableTpy, id))).ToArray();  //clone dei recods di selezione: restituisco solo i record idividuati da outKeyList (ie: quelli estratti da DB) con i campi aggiornati (es: timestamp) e non tutti i record presenti in cache per quel tipo
                    //---
                    string xrefFromSql = DogManagerCache.sqlListEx(this, fld?.table, ref xrefFromParameters, null, fld, outKeyList, null, false, cloneRowRecList, options: options);


                    //!!!//Dictionary<object, ModelErp> outDictFrom = this.ExecuteQuery(null, xrefFromType, xrefFromSql, xrefFromParameters, transactionId, maxRecords, options: options);
                    Dictionary<object, ModelErp> outDictFrom = this.ExecuteQueryEx(null, fld?.table?.tableTpy, xrefFromSql, xrefFromParameters, fillXdata, fmtList, transactionId, maxRecords, options: options);
                    //!!!//

                    //carico nella cache i riferimenti per ogni record della lista
                    DogManagerCache.CacheAddDict(this, ref dogCache, fld?.table?.tableTpy, outDictFrom, options: options); // salvo i record estratti in cache
                }
            }
            List<T> outList = DogManagerCache.CacheFillNull<T>(this, ref dogCache, outKeyList, fillXdata, fmtList, transactionId, maxRecords, options: options);


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //!!!!!!! RESTITUISCO UN CLONE DELL'OGGETTO ModelErp TRONCATO AL SECONDO LIVELLO (depth=2)                    !!!!!!
            //!!!!!!! in questo modo eventuali modifiche effettuate sulla lista restituita non hanno impatto sulla CACHE  !!!!!!
            //!!!!!!! e posso usarlo come Model in una pagina .cshtml                                                     !!!!!!
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


            //List<T> outListTruncated = outList.Select(item => (T)item.TruncateClone(DogManager.DOG_MAX_OBJ_DEPTH, action: 'R')).ToList();     // restituisce un clone troncato la lista di valori T 

            List<T> outListTruncated = outList.Select(item => (T)this.TruncateCloneModelErp(item, DogManager.DOG_MAX_OBJ_DEPTH, action: 'R', options: options)).ToList();  // restituisce un clone troncato la lista di valori T 

            return outListTruncated;
        }

        //***************************************************************************************************************************************************
        //***************************************************************************************************************************************************


        // MntList con cache (ordina i modelli in base alle dipendenze topologiche e gestisce la cache)
        // generalmente gli oggetti ModelErp presenti in tabModels sono già tutti referenziati in cache (caricati in precedenza con List/Row)
        // in ogni caso, a seguito dell'aggiornamento, i record modificati vengono rimossi dalla cache (ie: [FORCE_CACHE_RELOAD_MNT])
        // in modo che alla prossima lettura vengano ricaricati da DB
        public List<DogResult> MntList(List<ModelErp> tabModels, ref DogCache dogCache, string? transactionId, int maxRecords, string options = "")
        {
            return MntList(tabModels, false, null, ref dogCache, transactionId, maxRecords, options: options);
        }
        public List<DogResult> MntList(List<ModelErp> tabModels, bool fillXdata, List<string> fmtList, ref DogCache dogCache, string? transactionId, int maxRecords, string options = "")
        {
            if (_modelMode == "FREE") throw new InvalidOperationException("MntList non disponibile in modalità FREE.");  // MntList non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.

            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            if (maxRecords < 0) maxRecords = DOG_DEFAULT_XREF_CACHE_MAX_RECORDS;
            List<DogResult> dogResults = new List<DogResult>();
            if (tabModels == null) { throw new ArgumentNullException(nameof(tabModels)); }
            if (dogCache == null) { throw new ArgumentNullException(nameof(dogCache)); }
            try
            {
                //------------------------------------------------------------------------------------------------
                // Pianifica le modifiche e ordina i modelli in base alle dipendenze topologiche
                List<ModelErp> tabModelsTopologicalSorted = DogManagerTopologicalSort.PlanSortedChanges(
                    this,
                    tabModels,  // lista di modelli DA_INSERIRE_AGGIORNARE
                    dogCache,   // cache con i record già caricati
                    this.typeGraph,
                    checkDbForOrphans: true,
                    tryBreakCyclesByNullingFK: true
                );

                //------------------------------------------------------------------------------------------------
                // Esegue mantain su DB
                IDictionary<string, object> parameters = new Dictionary<string, object>();
                StringBuilder sb = new StringBuilder();
                foreach (var tab in tabModelsTopologicalSorted)
                {
                    if (tab == null) { throw new ArgumentNullException(nameof(tab)); }
                    sb.Append(DogManagerQuery.sqlMantain(this, tab, ref parameters, ref dogResults, options: options)).AppendLine("; ");
                }
                //access DB
                string sql = sb.ToString();
                if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
                int affectedRows = _getDbMg().ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);
                if (affectedRows != dogResults.Count()) throw new DatabaseException(ERR_DB_TIMESTAMP, $"Timestamp non valido o errore in insert/update. affectedRows: {affectedRows} resultsCount: {dogResults.Count()}", null);

                //------------------------------------------------------------------------------------------------
                //forza la cache a ricaricare i records modificati [FORCE_CACHE_RELOAD_MNT], alla prossima rilettura
                //foreach (var model in tabModels) { if (dogCache.dbCache.TryGetValue(model.GetType(), out var dizInterno)) { dizInterno.Remove(model.getIcode()); } } //rimuovo dalla cache i record modificati su DB (alla prossima lettura verranno ricaricati da DB)
                foreach (var result in dogResults)
                {
                    if (dogCache.dbCache.TryGetValue(result.TabType, out var dizInterno))
                    {
                        dizInterno[result.Icode] = (ModelErp)null; //annullo nella cache il record aggiunto/modificati su DB (quando eseguo CacheFillNull verranno ricaricati da DB)
                    }
                    else 
                    {
                        var dizNewType = new Dictionary<object, ModelErp>(); dizNewType[result.Icode] = null;   //aggiungo i rcord nelle nuove tabelle se non presenti
                        dogCache.dbCache.Add(result.TabType, dizNewType);
                    }
                }
                DogManagerCache.CacheFillNull(this, ref dogCache, null, null, fillXdata, fmtList, transactionId, maxRecords, options: options);    //ricarico la cache con i record modificati (forza reload sulla cache)

                return dogResults;
            }
            catch (DatabaseException dbEx)
            {
                string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
                throw new DatabaseException (dbEx.ErrorCode, $"MntList[{strResults}]: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
                throw new DatabaseException(ERR_DB_ERROR, $"MntList[{strResults}]: {ex.Message} {ex.InnerException?.Message ?? ""}");
            }
        }




        //// Mantain solo tabelle Xdata senza cache
        //public async Task<List<DogResult>> MntXdataListAsync(DogTable tab, List<ModelXdata> tabXdataModels, string? transactionId, string options = "") 
        //{
        //    if (_modelMode == "FREE") throw new InvalidOperationException("MntXdataList non disponibile in modalità FREE.");  // MntXdataList non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.
        //    if (tab.isXdataTable) throw new InvalidOperationException("MntXdataList tab deve estendere ModelErp.");  // MntXdataList: deve essere specificata la tabella padre dei dati estesi 
        //    if (tab.tabXdata == null) throw new InvalidOperationException("MntXdataList tab deve avere una tabella Xdata.");  // MntXdataList: deve essere specificata la tabella padre dei dati estesi 

        //    if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
        //    List<DogResult> dogResults = new List<DogResult>();
        //    if (tabXdataModels == null) { throw new ArgumentNullException(nameof(tabXdataModels)); }
        //    try
        //    {
        //        //------------------------------------------------------------------------------------------------
        //        // Esegue mantain su DB
        //        IDictionary<string, object> parameters = new Dictionary<string, object>();
        //        StringBuilder sb = new StringBuilder();
        //        foreach (var el in tabXdataModels)
        //        {
        //            if (el == null) { throw new ArgumentNullException(nameof(tab)); }
        //            sb.Append(DogManagerQuery.sqlMantainXdata(this, tab.tabXdata, el, ref parameters, ref dogResults, options: options)).AppendLine("; ");
        //        }
        //        //access DB
        //        string sql = sb.ToString();
        //        if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 
                
        //        //int affectedRows = await this._getDbMg().ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);
        //        int affectedRows = await _getDbMg().ExecuteNonQueryAsync(sql, EncodeSpecialFields(parameters, options), transactionId);

        //        if (affectedRows != dogResults.Count()) throw new DatabaseException(ERR_DB_TIMESTAMP, $"MntXdataList: Timestamp non valido o errore in insert/update. affectedRows: {affectedRows} resultsCount: {dogResults.Count()}", null);

        //        //------------------------------------------------------------------------------------------------
        //        //forza rilettura dei records modificati per recuperare il timestamp
        //        if (this.DatabaseType == DbTyp.SqlServer || this.DatabaseType == DbTyp.Sybase)
        //        {
        //            List<object> rowIdList = dogResults.Select(r => r.Icode).ToList(); //lista di chiavi primarie dei record modificati su DB
        //            Dictionary<object, ModelXdata> xdataDict = ExecuteQueryXdataEx(null, tab, false, rowIdList, null, transactionId, -1, (long)0, options: options);  // NON LEGGO I BLOB
        //            foreach (var el in dogResults) { el.Timestamp = xdataDict[el.Icode].Timestamp; }
        //        }
        //        return dogResults;
        //    }
        //    catch (DatabaseException dbEx)
        //    {
        //        string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
        //        throw new DatabaseException(dbEx.ErrorCode, $"MntXdataList[{strResults}]: {dbEx.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
        //        throw new DatabaseException(ERR_DB_ERROR, $"MntXdataList[{strResults}]: {ex.Message} {ex.InnerException?.Message ?? ""}");
        //    }
        //}


        // Mantain solo tabelle Xdata senza cache
        public List<ModelXdata> MntXdataList(DogTable tab, List<ModelXdata> tabXdataModels, string? transactionId, string options = "")
        {
            if (_modelMode == "FREE") throw new InvalidOperationException("MntXdataList non disponibile in modalità FREE.");  // MntXdataList non è disponibile in modalità FREE, perché è strettamente legata alla gestione della cache e delle dipendenze tra tabelle, che in modalità FREE sono limitate o assenti.
            if (tab.isXdataTable) throw new InvalidOperationException("MntXdataList tab deve estendere ModelErp.");  // MntXdataList: deve essere specificata la tabella padre dei dati estesi 
            if (tab.tabXdata == null) throw new InvalidOperationException("MntXdataList tab deve avere una tabella Xdata.");  // MntXdataList: deve essere specificata la tabella padre dei dati estesi 

            if (string.IsNullOrWhiteSpace(transactionId)) transactionId = null;
            List<DogResult> dogResults = new List<DogResult>();
            if (tabXdataModels == null) { throw new ArgumentNullException(nameof(tabXdataModels)); }
            try
            {
                //------------------------------------------------------------------------------------------------
                // Esegue mantain su DB
                List<object> rowIdList = new List<object>(); //lista di chiavi primarie dei record modificati su DB, da riutilizzare per la rilettura forzata a seguito del mantain
                IDictionary<string, object> parameters = new Dictionary<string, object>();
                StringBuilder sb = new StringBuilder();
                foreach (var el in tabXdataModels)
                {
                    if (el == null) { throw new ArgumentNullException(nameof(tab)); }
                    sb.Append(DogManagerQuery.sqlMantainXdata(this, tab.tabXdata, el, ref parameters, ref dogResults, options: options)).AppendLine("; ");
                    rowIdList.Add(el.Icode);
                }
                //access DB
                string sql = sb.ToString();
                if (sql.Contains('\'') || sql.Contains('#') || sql.Contains("--")) { throw new FormatException(nameof(sql)); }  // Non devo passare i parametri esplicitamente ma sempre attraverso il Dictionary parameters 

                //int affectedRows = await this._getDbMg()._getDbMg().   .ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);
                int affectedRows = _getDbMg().ExecuteNonQuery(sql, EncodeSpecialFields(parameters, options), transactionId);

                if (affectedRows != dogResults.Count()) throw new DatabaseException(ERR_DB_TIMESTAMP, $"MntXdataList: Timestamp non valido o errore in insert/update. affectedRows: {affectedRows} resultsCount: {dogResults.Count()}", null);

                //------------------------------------------------------------------------------------------------
                //forza rilettura dei records modificati: devo rileggere anche i DELETED 
                Dictionary<object, ModelXdata> dictXdata = ExecuteQueryXdataEx(null, tab, false, rowIdList, null, transactionId, (int)-1, (long)0, null, options: options + " [DELETED=Y]");  //cloneRowRecList=null, perchè isMrefRowIdList=false e quindi fldXref=null //non rileggo i BLOB, ma solo i campi necessari per recuperare il timestamp e verificare la validità del mantain
                return rowIdList.Select(id => dictXdata[id]).ToList(); //restituisco i record modificati con i campi aggiornati (es: timestamp)
            }
            catch (DatabaseException dbEx)
            {
                string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
                throw new DatabaseException(dbEx.ErrorCode, $"MntXdataList[{strResults}]: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                string strResults = dogResults != null ? string.Join(", ", dogResults.Select(d => $"{d.Action}:{d.Icode ?? "null"}")) : string.Empty;
                throw new DatabaseException(ERR_DB_ERROR, $"MntXdataList[{strResults}]: {ex.Message} {ex.InnerException?.Message ?? ""}");
            }
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




    }
}