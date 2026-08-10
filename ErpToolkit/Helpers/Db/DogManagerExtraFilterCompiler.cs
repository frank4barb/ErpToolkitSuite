using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ErpToolkit.Models;
using Newtonsoft.Json;
using static ErpToolkit.Helpers.Db.DogManager;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Swashbuckle.AspNetCore.SwaggerGen;


namespace ErpToolkit.Helpers.Db
{
    public static class ExtraFilterCompiler
    {
        private static readonly NLog.ILogger _logger;
        static ExtraFilterCompiler()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************
        //******************************************************************************************************************


        public static string ResolveExtraFilterCondition(DogManager dogMng, DogTable tab, string tpy, ref IDictionary<string, object> parameters, ref IDictionary<string,string> extraAutocompleteFieldNames, string filterTemplate, Dictionary<string, List<string>> extraFields = null)
        {
            extraAutocompleteFieldNames = null; // resetta la lista per questa chiamata (sarà popolata dai template che usano campi extra)
            // 1. Costruisci il contesto globale per i template di filtro dinamico, includendo eventuali campi extra specificati, informazioni sull'applicazione, l'utente e la data odierna.
            Dictionary<string, List<object>> extraFieldsObj = ConvertExtraFields(dogMng, extraFields);
            var globals = new ExtraFilterGlobals
            {
                dogMng = dogMng,
                tab = tab,
                parameters = parameters,
                autocompleteType = tpy,
                extraFields = extraFieldsObj,
                //app = new ExtraFilterAppContext
                //{
                //    CompanyCode = ErpContext.Instance.CompanyCode,
                //    DbSchema = ErpContext.Instance.DbSchema,
                //},
                //user = new ExtraFilterUserContext
                //{
                //    UserId = User.Identity?.Name ?? "",
                //    UserName = User.FindFirst("DisplayName")?.Value ?? "",
                //    Role = User.FindFirst("Role")?.Value ?? "",
                //},
                today = new ExtraFilterDateContext(),
                valid = new ExtraFilterValid()
            };

            // 2. Compila ed esegui il template di filtro dinamico, ottenendo la stringa di filtro risultante o eventuali errori di validazione.
            ExtraFilterResult result = ResolveAsync(filterTemplate, extraFieldsObj, globals).GetAwaiter().GetResult();
            if (!result.IsValid)
            {
                // Gestisci l'errore di validazione (ad esempio, loggalo e restituisci un messaggio user-friendly)
                _logger.Warn($"ExtraFilter validation failed: {result.ValidationError}");
                throw new InvalidOperationException(result.ValidationError);
            }
            extraAutocompleteFieldNames = globals.extraAutocompleteFieldNames; // restituisci la lista dei campi per l'autocompleteClient
            return result.Filter ?? "";
        }
        public static Dictionary<string, List<object>> ConvertExtraFields(DogManager dogMng, Dictionary<string, List<string>> extraFields)
        {
            if (extraFields == null) return null;

            var result = new Dictionary<string, List<object>>();
            foreach (var (key, values) in extraFields)
            {
                DogField fld = dogMng.getDogField(key);
                DogField dbFld = dogMng.getDbDogField(fld);
                if (dbFld == null)
                {
                    // Proprietà non trovata nel tipo T: passa le stringhe così come sono
                    result[key] = values.Cast<object>().ToList();
                    continue;
                }

                // Unwrap Nullable<T> se necessario (es. int? → int)
                var targetType = Nullable.GetUnderlyingType(dbFld.fieldTyp) ?? dbFld.fieldTyp;

                result[key] = values.Select(v => ConvertValue(v, targetType)).ToList();
            }

            return result;
        }
        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value)) return null;

            // Enum
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value, ignoreCase: true);

            // TypeConverter generico (copre int, decimal, DateTime, bool, Guid, ecc.)
            var converter = TypeDescriptor.GetConverter(targetType);
            if (converter.CanConvertFrom(typeof(string)))
                return converter.ConvertFromInvariantString(value);

            // Fallback
            return Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
        }


        //******************************************************************************************************************
        //******************************************************************************************************************


        // ─────────────────────────────────────────────────────────────────
        // Globals accessibili nei template ExtraFilter
        // ─────────────────────────────────────────────────────────────────
        public class ExtraFilterGlobals
        {
            public DogManager dogMng { get; set; } = null;
            public DogTable tab { get; set; } = null;
            //public dynamic model { get; set; }   // model concreto deserializzato (dynamic → late binding)
            public IDictionary<string, object> parameters = new Dictionary<string, object>();
            public string autocompleteType { get; set; } = "";
            public Dictionary<string, List<object>> extraFields { get; set; } = new();
            public Dictionary<string, string> extraAutocompleteFieldNames { get; set; } = new();
            public ExtraFilterAppContext app { get; set; } = new();
            public ExtraFilterUserContext user { get; set; } = new();
            public ExtraFilterDateContext today { get; set; } = new();
            public ExtraFilterValid valid { get; set; } = new();

            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------

            //////////// VECCHI FILTRI, NON USARE PIÙ:
            ////////////[AutocompleteServer(...,
            ////////////    ExtraFilter = "{valid.Require(\"PrIdGruppo\", extraFields, \"Seleziona prima il Gruppo\")}" +
            ////////////                  "PR_GRUPPO = '{f(\"PrIdGruppo\")}' AND ANNO = {today.YearOf(f(\"PrDataInizio\"))}")]
            ////////////...
            ////////////    ExtraFilter = "EP_ID_PAZIENTE IN ({string.Join(\", \", DogManager.addListParam(f(\"SelReIdPaziente\"), ref parameters))})")]
            ////////////...
            ////////////    ExtraFilter = "TC_CLASSE in ({DogManager.addParam('2', ref parameters)})")]

            
            //Esempi di template di filtro dinamico:
            //--------------------------------------

            //ExtraFilter = "{And(Eq(\"PA__GRUPPO\",\"SelGruppo\"), In(\"PA__ICODE\",\"SelCodici\"))}"

            //ExtraFilter = "{valid.Require(\"PrIdGruppo\", extraFields, \"Seleziona prima il Gruppo\")}" +
            //              "{And(Eq(\"PR_GRUPPO\",\"PrIdGruppo\"), Between(\"DATA_EPI\",\"SelDate\"))}"

            //ExtraFilter = "{Or(In(\"PA__ICODE\",\"SelCodici\"), InVals(\"PA__STATO\",\"A\",\"B\"))}"

            // ── Accesso ai campi extra ─────────────────────────────────────────────
            public List<object>? f(string fieldName)
                => extraFields.TryGetValue(fieldName, out var v) ? v : null;
            public object? f0(string fieldName)
                => (extraFields.TryGetValue(fieldName, out var v) && v?.Count > 0) ? v[0] : null;
            public object? f1(string fieldName)
                => (extraFields.TryGetValue(fieldName, out var v) && v?.Count > 1) ? v[1] : null;
            public List<object> fd(string fieldName, List<object> defaultValue)
                => extraFields.TryGetValue(fieldName, out var v) && !UtilHelper.IsNullOrEmptyObject(v)
                    ? v : defaultValue;

            // ── Helper interno ─────────────────────────────────────────────────────
            private bool HasValue(object? v)
                => v != null && !string.IsNullOrEmpty(v.ToString());

            private string CompareCheck(string op, string sqlColumn, object? v)
            {
                if (this.autocompleteType == "GetAll") throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: autocompleteClient non può usare il filtro '{op}' sul campo {sqlColumn}.");
                return Compare(op, sqlColumn, v);
            }
            private string Compare(string op, string sqlColumn, object? v)
            {
                if (!HasValue(v)) return "";
                var p = DogManager.addParam(v!, ref parameters);
                return $"{sqlColumn} {op} {p}";
            }

            // ── Uguaglianza ────────────────────────────────────────────────────────
            // {Eq("PA__ICODE", "SelCodice")}
            // {EqVal("PA__STATO", "A")}
            public string Eq(string sqlColumn, string fieldName) => CompareCheck("=", sqlColumn, f0(fieldName));
            public string EqVal(string sqlColumn, object value) => Compare("=", sqlColumn, value);
            public string Neq(string sqlColumn, string fieldName) => CompareCheck("<>", sqlColumn, f0(fieldName));
            public string NeqVal(string sqlColumn, object value) => Compare("<>", sqlColumn, value);

            // ── Confronti ──────────────────────────────────────────────────────────
            // {Gt("PA__DATA", "SelData")}  {GtVal("PA__ANNO", 2020)}
            // {Like("PA__DESC", "SelDesc")}
            // {LikeVal("PA__DESC", "%rossi%")}        valore letterale
            public string Gt(string sqlColumn, string fieldName) => CompareCheck(">", sqlColumn, f0(fieldName));
            public string Gte(string sqlColumn, string fieldName) => CompareCheck(">=", sqlColumn, f0(fieldName));
            public string Lt(string sqlColumn, string fieldName) => CompareCheck("<", sqlColumn, f0(fieldName));
            public string Lte(string sqlColumn, string fieldName) => CompareCheck("<=", sqlColumn, f0(fieldName));
            public string Like(string sqlColumn, string fieldName) => CompareCheck("LIKE", sqlColumn, f0(fieldName));
            public string GtVal(string sqlColumn, object value) => Compare(">", sqlColumn, value);
            public string GteVal(string sqlColumn, object value) => Compare(">=", sqlColumn, value);
            public string LtVal(string sqlColumn, object value) => Compare("<", sqlColumn, value);
            public string LteVal(string sqlColumn, object value) => Compare("<=", sqlColumn, value);
            public string LikeVal(string sqlColumn, object value) => Compare("LIKE", sqlColumn, value);

            // ── IN / NOT IN ────────────────────────────────────────────────────────
            // {In("PA__ICODE", "SelCodici")}
            // {InVals("PA__STATO", "A","B","C")}
            public string In(string sqlColumn, string fieldName)
            {
                if (this.autocompleteType == "GetAll")
                {
                    // In modalità autocompleteClient, non filtrare i risultati ma registra il nome della colonna per l'autocomplete
                    extraAutocompleteFieldNames[fieldName] = sqlColumn; // Aggiungi il nome della colonna alla lista per l'autocompleteClient (posso usare la stessa proprietà{fieldName} per filtrare su diversi campi del DB{sqlColumn})
                    return "";
                }
                var list = f(fieldName)?.Where(v => v != null).ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{sqlColumn} IN ({string.Join(", ", pNames)})";
            }
            public string NotIn(string sqlColumn, string fieldName)
            {
                if (this.autocompleteType == "GetAll") throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: autocompleteClient non può usare il filtro 'NotIn' sul campo {sqlColumn}.");
                var list = f(fieldName)?.Where(v => v != null).ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{sqlColumn} NOT IN ({string.Join(", ", pNames)})";
            }
            public string InVals(string sqlColumn, params object[] values)
            {
                var list = values?.Where(v => v != null).Cast<object>().ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{sqlColumn} IN ({string.Join(", ", pNames)})";
            }
            public string NotInVals(string sqlColumn, params object[] values)
            {
                var list = values?.Where(v => v != null).Cast<object>().ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{sqlColumn} NOT IN ({string.Join(", ", pNames)})";
            }


            // ── BETWEEN / date-range ───────────────────────────────────────────────
            // {Between("PA__DATA", "SelDateRange")}    f(field)[0]=start, f(field)[1]=end
            // {BetweenVals("PA__ANNO", 2020, 2024)}
            public string Between(string sqlColumn, string fieldName)
            {
                if (this.autocompleteType == "GetAll") throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: autocompleteClient non può usare il filtro 'Between' sul campo {sqlColumn}.");
                var list = f(fieldName);
                var start = list?.ElementAtOrDefault(0);
                var end = list?.ElementAtOrDefault(1);
                return BuildBetween(sqlColumn, start, end);
            }
            public string BetweenVals(string sqlColumn, object? start, object? end)
                => BuildBetween(sqlColumn, start, end);

            private string BuildBetween(string sqlColumn, object? start, object? end)
            {
                bool hasStart = HasValue(start);
                bool hasEnd = HasValue(end);
                if (!hasStart && !hasEnd) return "";
                if (hasStart && hasEnd)
                {
                    var ps = DogManager.addParam(start!, ref parameters);
                    var pe = DogManager.addParam(end!, ref parameters);
                    return $"{sqlColumn} BETWEEN {ps} AND {pe}";
                }
                if (hasStart)
                {
                    var ps = DogManager.addParam(start!, ref parameters);
                    return $"{sqlColumn} >= {ps}";
                }
                var pe2 = DogManager.addParam(end!, ref parameters);
                return $"{sqlColumn} <= {pe2}";
            }

            // ── IS NULL / IS NOT NULL ──────────────────────────────────────────────
            // {IsNull("PA__NOTE")}
            public string IsNull(string sqlColumn) => $"{sqlColumn} IS NULL";
            public string IsNotNull(string sqlColumn) => $"{sqlColumn} IS NOT NULL";

            // ── Exclude condition ──────────────────────────────────────────────
            // {IsNull("PA__NOTE")}
            public string ExcludeCond() => "1=0";

            //============================================================================================================================

            // CUSTOM FILTERS CONDITIO: 
            //---------------

            // schema: <sqlColumn> <op> <funcCond>(<fieldName>)
            public string CustomIn(string funcColumn, string sqlColumn, string op, string funcCond, string fieldName)
            {
                if (string.IsNullOrWhiteSpace(op)) op = "IN";
                if (this.autocompleteType == "GetAll")
                {
                    // In modalità autocompleteClient, non filtrare i risultati ma registra il nome della colonna per l'autocomplete
                    extraAutocompleteFieldNames[fieldName] = sqlColumn; // Aggiungi il nome della colonna alla lista per l'autocompleteClient (posso usare la stessa proprietà{fieldName} per filtrare su diversi campi del DB{sqlColumn})
                    return "";
                }
                var list = f(fieldName)?.Where(v => v != null).ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{funcColumn}({sqlColumn}) {op} ({funcCond}({string.Join(", ", pNames)}))";
            }
            // schema: <sqlColumn> <op> <func>(<value1>, <value2>, ...)
            public string CustomInVals(string funcColumn, string sqlColumn, string op, string funcCond, params object[] values)
            {
                if (string.IsNullOrWhiteSpace(op)) op = "IN";
                var list = values?.Where(v => v != null).Cast<object>().ToList();
                if (list == null || list.Count == 0) return "";
                var pNames = DogManager.addListParam(list, ref parameters);
                return $"{funcColumn}({sqlColumn}) {op} ({funcCond}({string.Join(", ", pNames)}))";
            }


            // funzioni usabili come funcCond: "SELECT PA__ICODE FROM PA__TAB WHERE PA__GRUPPO = {f("SelGruppo")}" 

            public string FuncSelect(string sqlSelect)
            {
                if (string.IsNullOrWhiteSpace(sqlSelect)) return "";
                return $"{sqlSelect}";
            }



            //============================================================================================================================

            //// ── From tabella relazionata ────────────────────────────────────────────────────────
            public string From(string sqlColumn, string condition)
            {
                if (this.autocompleteType == "GetAll") throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: autocompleteClient non può usare il filtro 'From' sul campo {sqlColumn}.");
                DogField fld = tab.fldByName(sqlColumn);
                string joinTableName = fld?.XrefObj?.table?.tableName ?? throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: campo {sqlColumn} non ha una tabella relazionata (XrefObj).");
                string joinIcodeFieldName = fld?.XrefObj?.table?.fldIcode?.SqlFieldName ?? throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: campo {sqlColumn} non ha 'fldIcode?.SqlFieldName' nella tabella relazionata (XrefObj).");

                return $"{sqlColumn} IN (SELECT {joinIcodeFieldName} FROM {joinTableName} WHERE {condition} )";
            }
            public string NotFrom(string sqlColumn, string condition)
            {
                if (this.autocompleteType == "GetAll") throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: autocompleteClient non può usare il filtro 'From' sul campo {sqlColumn}.");
                DogField fld = tab.fldByName(sqlColumn);
                string joinTableName = fld?.XrefObj?.table?.tableName ?? throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: campo {sqlColumn} non ha una tabella relazionata (XrefObj).");
                string joinIcodeFieldName = fld?.XrefObj?.table?.fldIcode?.SqlFieldName ?? throw new InvalidOperationException($"ExtraFilterCompiler: Errore di validazione: campo {sqlColumn} non ha 'fldIcode?.SqlFieldName' nella tabella relazionata (XrefObj).");

                return $"{sqlColumn} NOT IN (SELECT {joinIcodeFieldName} FROM {joinTableName} WHERE {condition} )";
            }


            //============================================================================================================================

            // ── Combinatori ────────────────────────────────────────────────────────
            // {And(Eq("PA__GRUPPO","SelGruppo"), In("PA__ICODE","SelCodici"))}
            // {Or(Eq("PA__TIPO","SelTipo"), IsNull("PA__TIPO"))}
            public string And(params string[] conditions)
            {
                var parts = conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                return parts.Count switch { 0 => "", 1 => parts[0], _ => "(" + string.Join(" AND ", parts) + ")" };
            }
            public string Or(params string[] conditions)
            {
                var parts = conditions.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
                return parts.Count switch { 0 => "", 1 => parts[0], _ => "(" + string.Join(" OR ", parts) + ")" };
            }


        }

        public class ExtraFilterAppContext
        {
            public string CompanyCode { get; set; } = "";
            public string CurrentYear { get; set; } = DateTime.Today.Year.ToString();
            public string DbSchema { get; set; } = "";
        }

        public class ExtraFilterUserContext
        {
            public string UserId { get; set; } = "";
            public string UserName { get; set; } = "";
            public string Role { get; set; } = "";
        }

        public class ExtraFilterDateContext
        {
            public int Year => DateTime.Today.Year;
            public int Month => DateTime.Today.Month;
            public int Day => DateTime.Today.Day;
            public DateTime Date => DateTime.Today;
            public DateTime Now => DateTime.Now;

            // CAMBIATO: riceve il valore stringa direttamente (già estratto dal dictionary)
            public int YearOf(string? dateStr)
                => DateTime.TryParse(dateStr, out var d) ? d.Year : 0;
            public string Format(string? dateStr, string fmt)
                => DateTime.TryParse(dateStr, out var d) ? d.ToString(fmt) : "";
        }

        public class ExtraFilterValid
        {
            /// Uso: {valid.Require("PrIdGruppo", extraFields, "Seleziona prima il Gruppo")}
            public string Require(string fieldName, Dictionary<string, List<object>> fields)
            {
                return Require(fieldName, fields, $"Seleziona prima il campo '{fieldName}'");
            }
            public string Require(string fieldName, Dictionary<string, List<object>> fields, string errorMessage)
            {
                if (!fields.TryGetValue(fieldName, out var list) || list == null || list.Count == 0
                    || list.All(v => v == null || string.IsNullOrWhiteSpace(v.ToString())))
                    throw new ExtraFilterValidationException(errorMessage);
                return "";
            }

            /// Uso: {valid.RequireIf(extraFields["PrAnno"] == "0", "Anno non valido")}
            public string RequireIf(bool condition, string errorMessage)
            {
                if (condition) throw new ExtraFilterValidationException(errorMessage);
                return "";
            }

            /// Uso: {valid.RequireAny("Gruppo o Tipo obbligatorio", extraFields, "PrIdGruppo", "PrTipo")}
            public string RequireAny(string errorMessage, Dictionary<string, List<object>> fields, params string[] fieldNames)
            {
                bool anyFilled = fieldNames.Any(fn =>
                    fields.TryGetValue(fn, out var list) && list != null && list.Count > 0
                    && list.Any(v => v != null && !string.IsNullOrWhiteSpace(v.ToString())));
                if (!anyFilled) throw new ExtraFilterValidationException(errorMessage);
                return "";
            }

        }
        public class ExtraFilterValidationException : Exception
        {
            public ExtraFilterValidationException(string message) : base(message) { }
        }

        public class ExtraFilterResult
        {
            public bool IsValid { get; init; }
            public string? Filter { get; init; }
            public string? ValidationError { get; init; }
        }

        // ─────────────────────────────────────────────────────────────────
        // Compiler + cache
        // ─────────────────────────────────────────────────────────────────

        // Cache: chiave = template grezzo → script compilato
        // Chiave solo sul template: il tipo concreto non serve a compile-time
        // perché model è dichiarato dynamic nei globals
        private static readonly Dictionary<string, Script<string>> _cache = new();
        private static readonly SemaphoreSlim _lock = new(1, 1);

        // ── Opzioni Roslyn ────────────────────────────────────────────
        private static ScriptOptions BuildOptions()
            => ScriptOptions.Default
                .AddReferences(typeof(ExtraFilterGlobals).Assembly)   // assembly ErpToolkit
                .AddReferences(typeof(ModelErp).Assembly)             // assembly models
                .AddReferences(typeof(System.Linq.Enumerable).Assembly)
                .AddImports("System", "System.Linq",
                            "System.Collections.Generic",
                            "ErpToolkit.Models",
                            typeof(ExtraFilterGlobals).Namespace!);

        // ── Compila un template e lo mette in cache ───────────────────
        public static async Task CompileAsync(string template)
        {
            if (string.IsNullOrWhiteSpace(template)) return;

            await _lock.WaitAsync();
            try
            {
                if (_cache.ContainsKey(template)) return;

                var script = CSharpScript.Create<string>(
                    code: $"$\"{template}\"",
                    options: BuildOptions(),
                    globalsType: typeof(ExtraFilterGlobals)   // model=dynamic → no dipendenza dal tipo concreto
                );

                var errors = script.Compile()
                    .Where(d => d.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
                    .ToList();

                if (errors.Any())
                    throw new InvalidOperationException(
                        $"ExtraFilter compile error in template:\n  \"{template}\"\n" +
                        string.Join("\n", errors.Select(e => "  • " + e.GetMessage())));

                _cache[template] = script;
                _logger.Debug($"[ExtraFilterCompiler] ✅ Compiled: {template[..Math.Min(80, template.Length)]}");
            }
            finally { _lock.Release(); }
        }

        public static async Task<ExtraFilterResult> ResolveAsync(
            string filterTemplate,
            Dictionary<string, List<object>> extraFieldsObj,   // CAMBIATO: no più Type + modelJson
            ExtraFilterGlobals globals)
        {
            if (string.IsNullOrWhiteSpace(filterTemplate))
                return new ExtraFilterResult { IsValid = true, Filter = "" };

            if (!_cache.ContainsKey(filterTemplate))
                await CompileAsync(filterTemplate);

            // Inietta il dictionary nei globals
            globals.extraFields = extraFieldsObj;

            try
            {
                var result = await _cache[filterTemplate].RunAsync(globals);
                return new ExtraFilterResult { IsValid = true, Filter = result.ReturnValue ?? "" };
            }
            catch (ExtraFilterValidationException ex)
            {
                return new ExtraFilterResult { IsValid = false, ValidationError = ex.Message };
            }
        }

        // ── Statistiche (utile per health-check / diagnostica) ────────
        public static int CachedCount => _cache.Count;
        public static IEnumerable<string> CachedTemplates => _cache.Keys;



    }
}
