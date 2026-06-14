using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using ErpToolkit.Models;
using Newtonsoft.Json;
using static ErpToolkit.Helpers.Db.DogManager;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;


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


        public static string ResolveExtraFilterCondition(DogManager dogMng, DogTable tab, string tpy, ref IDictionary<string, object> parameters, string? modelPropertyName = null, Dictionary<string, List<string>> extraFields = null)
        {
            // 1. Costruisci il contesto globale per i template di filtro dinamico, includendo eventuali campi extra specificati, informazioni sull'applicazione, l'utente e la data odierna.
            Dictionary<string, List<object>> extraFieldsObj = ConvertExtraFields(dogMng, extraFields);
            var globals = new ExtraFilterGlobals
            {
                parameters = parameters,
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

            // 2. recupera il template di filtro dinamico associato alla tabella e alla proprietà specificata (ad esempio, da una configurazione o da attributi sui modelli).
            //!!//string filterTemplate = tab.fields.FirstOrDefault(f => f.fieldName == modelPropertyName)?.AutocompleteExtraFilter ?? ""; // Se modelPropertyName è null, restituisce "" (no filtro)
            string filterTemplate = dogMng.getDogField(modelPropertyName ?? "")?.AutocompleteExtraFilter ?? ""; // Se modelPropertyName è null, restituisce "" (no filtro)

            // 2. Compila ed esegui il template di filtro dinamico, ottenendo la stringa di filtro risultante o eventuali errori di validazione.
            ExtraFilterResult result = ResolveAsync(filterTemplate, extraFieldsObj, globals).GetAwaiter().GetResult();
            if (!result.IsValid)
            {
                // Gestisci l'errore di validazione (ad esempio, loggalo e restituisci un messaggio user-friendly)
                _logger.Warn($"ExtraFilter validation failed: {result.ValidationError}");
                throw new InvalidOperationException(result.ValidationError);
            }
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
            //public dynamic model { get; set; }   // model concreto deserializzato (dynamic → late binding)
            public IDictionary<string, object> parameters = new Dictionary<string, object>();
            public Dictionary<string, List<object>> extraFields { get; set; } = new();
            public ExtraFilterAppContext app { get; set; } = new();
            public ExtraFilterUserContext user { get; set; } = new();
            public ExtraFilterDateContext today { get; set; } = new();
            public ExtraFilterValid valid { get; set; } = new();

            //-----------------------------------------------------------------------------------
            //-----------------------------------------------------------------------------------

            //ESEMPIO DI USO NEI TEMPLATE:
            //....
            //[AutocompleteServer(...,
            //    ExtraFilter = "{valid.Require(\"PrIdGruppo\", extraFields, \"Seleziona prima il Gruppo\")}" +
            //                  "PR_GRUPPO = '{f(\"PrIdGruppo\")}' AND ANNO = {today.YearOf(f(\"PrDataInizio\"))}")]
            //...

            // NUOVO: accesso senza virgolette doppie nel template
            // Uso: {f("PrIdGruppo")} invece di {extraFields["PrIdGruppo"]}
            public List<object>? f(string fieldName)
                => extraFields.TryGetValue(fieldName, out var v) ? v : null;
            public object? f0(string fieldName)
                => (extraFields.TryGetValue(fieldName, out var v) && v != null && v.Count > 0) ? v[0] : null;
            public object? f1(string fieldName)
                => (extraFields.TryGetValue(fieldName, out var v) && v != null && v.Count > 1) ? v[1] : null;

            // Con default
            public List<object> fd(string fieldName, List<object> defaultValue)
                => extraFields.TryGetValue(fieldName, out var v) && !UtilHelper.IsNullOrEmptyObject(v)
                    ? v : defaultValue;
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
            public string Require(string fieldName, Dictionary<string, string> fields, string errorMessage)
            {
                if (!fields.TryGetValue(fieldName, out var val) || string.IsNullOrWhiteSpace(val))
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
            public string RequireAny(string errorMessage, Dictionary<string, string> fields, params string[] fieldNames)
            {
                if (fieldNames.All(f => !fields.TryGetValue(f, out var v) || string.IsNullOrWhiteSpace(v)))
                    throw new ExtraFilterValidationException(errorMessage);
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
