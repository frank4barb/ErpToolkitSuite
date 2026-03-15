using ErpToolkit.Models;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Runtime.Serialization;
using System.Text;

namespace ErpToolkit.Helpers.Db
{
    // Funzioni di supporto per la serializzazione e deserializzazione Json con sistema: Newtonsoft.Json (Questo modello viene usato come default perchè funziona anche per riferimenti ciclici del modello)
    public static class DogManagerNewtonsoftJson
    {
        private static readonly NLog.ILogger _logger;
        static DogManagerNewtonsoftJson()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }


        //formatta una stringa JSON in modo leggibile
        public static string FormatJsonString(string json)
        {
            try
            {
                if (json == null) return "";
                var parsedJson = JToken.Parse(json);
                return parsedJson.ToString(Formatting.Indented);
            }
            catch (JsonReaderException ex) { return json; }
        }





        //******************************************************************************************************************
        //********  JSON DE-SERIALIZE (JSON to ModelErp)
        //******************************************************************************************************************
        internal static T SafeDeserialize<T>(DogManager dogMng, ModelObject dataObj, string? prefix, string? options)
        {
            if (dataObj == null || dataObj.data == null)
                throw new ArgumentNullException(nameof(dataObj), "ModelObject o la sua proprietà data è null.");
            return SafeDeserialize<T>(dogMng, dataObj.data, prefix,  options);
        }
        internal static T SafeDeserialize<T>(DogManager dogMng, object jsonObj, string? prefix, string? options)
        {
            if (jsonObj == null)
                throw new ArgumentNullException(nameof(jsonObj), "JsonObject è null.");

            JToken token;

            // Se data è un System.Text.Json.JsonElement -> prendi il raw text corretto
            if (jsonObj is System.Text.Json.JsonElement je)
            {
                token = JToken.Parse(je.GetRawText());
            }
            else if (jsonObj is System.Text.Json.JsonDocument jd)
            {
                token = JToken.Parse(jd.RootElement.GetRawText());
            }
            else if (jsonObj is string s)
            {
                s = s.Trim();
                token = (s.StartsWith("{") || s.StartsWith("[")) ? JToken.Parse(s) : JToken.FromObject(jsonObj);
            }
            else
            {
                // ultimo fallback: prova a creare JToken direttamente dall'oggetto CLR
                token = JToken.FromObject(jsonObj);
            }

            // Rimuove il prefisso dalle proprietà se è un oggetto
            if (token.Type == JTokenType.Object && !string.IsNullOrEmpty(prefix))
            {
                var obj = (JObject)token;
                var cleaned = new JObject();

                foreach (var prop in obj.Properties())
                {
                    var newName = prop.Name.StartsWith(prefix + ".") ? prop.Name.Substring(prefix.Length + 1) : prop.Name;
                    cleaned[newName] = prop.Value;
                }

                token = cleaned;
            }

            return SafeDeserialize<T>(dogMng, token, options);
        }

        private static T SafeDeserialize<T>(DogManager dogMng, JToken jtokenObj, string? options)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(options)) options = " ";

                // PRE-PROCESSING: normalizza le chiavi "XrefPcIdPrestazione[...]" nel payload
                if (jtokenObj is JObject rootObj)
                {
                    NormalizeBracketedDictionaryKeys(dogMng, rootObj);
                }

                var settings = new JsonSerializerSettings
                {
                    // ignora loop invece di crashare
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,

                    // evita di generare $id/$ref
                    PreserveReferencesHandling = PreserveReferencesHandling.None,

                    // tronca la profondità
                    MaxDepth = 10,     //   DogManager.DOG_MAX_OBJ_DEPTH,

                    // ignora valori null o mancanti
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore,

                    // naming camelCase
                    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),

                    // converter personalizzati
                    Converters = new List<JsonConverter>
                    {
                        new DateOnlyJsonConverter(options),
                        new TimeOnlyJsonConverter(options),
                        new CharJsonConverter(options),
                        new ShortJsonConverter(options),
                        new LongJsonConverter(options),
                        new DoubleJsonConverter(options),
                        new DictionaryStringConverter(options),
                        new ListModelErpJsonConverter(dogMng, options),
                        new DictionaryModelErpJsonConverter(dogMng, options),
                        new ModelErpConverter(options),
                    }
                };

                return JsonConvert.DeserializeObject<T>(jtokenObj.ToString(), settings);
            }
            catch (JsonException jex)
            {
                throw new InvalidOperationException(
                    $"Errore nella deserializzazione JSON in {typeof(T).Name}: {jex.Message}", jex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Errore inatteso nella deserializzazione in {typeof(T).Name}: {ex.Message}", ex);
            }
        }


        //########################################################################################################################################################
        //********  JSON SERIALIZE (ModelErp to JSON)
        //########################################################################################################################################################

        //public static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        //{
        //    NullValueHandling = NullValueHandling.Ignore,
        //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //    Formatting = Formatting.None,
        //    ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
        //    Converters = new List<JsonConverter>
        //    {
        //        new DateOnlyJsonConverter(),
        //        new TimeOnlyJsonConverter(),
        //        new ShortJsonConverter(),
        //        new LongJsonConverter(),
        //        new DoubleJsonConverter(),
        //        new ListModelErpJsonConverter(null), // dogMng serve solo in lettura
        //        new ModelErpConverter(),
        //    }
        //};

        public static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Include, // Permette di includere valori null se necessario per debug o completezza
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore, // Ignora i loop di riferimento per evitare eccezioni
            Formatting = Formatting.None, // Formattazione compatta
            ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(), // Usa camelCase per le proprietà (ie: convenzione javascrip che imposta a minuscolo la prima lettera delle proprietà)
            // Converters personalizzati
            Converters = new List<JsonConverter>
            {
                new DateOnlyJsonConverter(" "),
                new TimeOnlyJsonConverter(" "),
                new CharJsonConverter(" "),
                new ShortJsonConverter(" "),
                new LongJsonConverter(" "),
                new DoubleJsonConverter(" "),
                new DictionaryStringConverter(" "),
                new ListModelErpJsonConverter(null, " "), // dogMng serve solo in lettura
                new DictionaryModelErpJsonConverter(null, " "), // dogMng serve solo in lettura
                new ModelErpConverter(" "),
            }
        };


        internal static string SafeSerialize<T>(T obj, string? options = null)
        {
            if (String.IsNullOrWhiteSpace(options)) options = " ";
            var settings = jsonSerializerSettings;
            if (options.Contains("WriteIndented")) settings.Formatting = Formatting.Indented; // formattazione leggibile
            string json = JsonConvert.SerializeObject(obj, typeof(T), settings);
            //rimuove virvolette
            if (json != null && json.Length > 1 && json.StartsWith("\"") && json.EndsWith("\""))
            {
                json = json.Substring(1, json.Length - 2);
            }
            return json;
        }


        //==================================================================================================================
        //  PRE-PROCESSING
        //==================================================================================================================

        private static void NormalizeBracketedDictionaryKeys(DogManager dogMng, JObject root)
        {
            if (dogMng == null) return; // non posso effettuare il controllo di tipo concreto senza il dog manager, quindi esco senza modificare nulla


            // Trova tutte le proprietà che hanno forma BaseName[Key]
            // Esempio: "XrefPcIdPrestazione[IUF19TDHZN89]" => baseName="XrefPcIdPrestazione", key="IUF19TDHZN89"
            var props = root.Properties().ToList();
            var regex = new System.Text.RegularExpressions.Regex(@"^(?<base>[A-Za-z_]\w*)\[(?<key>[^\]]+)\]$");

            // 1) Raggruppa per baseName
            var groups = new Dictionary<string, List<(JProperty prop, string key)>>(StringComparer.Ordinal);
            foreach (var p in props)
            {
                var m = regex.Match(p.Name);
                if (!m.Success) continue;

                var baseName = m.Groups["base"].Value;
                var key = m.Groups["key"].Value;

                if (!groups.TryGetValue(baseName, out var list))
                {
                    list = new List<(JProperty prop, string key)>();
                    groups[baseName] = list;
                }
                list.Add((p, key));
            }

            // 2) Per ogni gruppo, assicurati che root[baseName] sia JObject;
            //    se è JArray, converti gli elementi in coppie chiave->oggetto, usando Pc1Icode/Icode come key se presenti.
            foreach (var kv in groups)
            {
                var baseName = kv.Key;
                var list = kv.Value;


                //---
                var fieldName = baseName; if (fieldName.StartsWith("Xref")) fieldName = fieldName.Substring(4); // es: XrefPcIdPrestazione -> PcIdPrestazione
                var icodeName = "";
                if (dogMng.tabProperties.TryGetValue(fieldName, out var dogField))
                {
                    icodeName = dogField.table.fldIcode.fieldName;
                }
                //---


                JObject container;

                // Se esiste già root[baseName]
                if (root.TryGetValue(baseName, out JToken existing))
                {
                    if (existing is JObject existingObj)
                    {
                        container = existingObj;
                    }
                    else if (existing is JArray existingArr)
                    {
                        container = new JObject();

                        foreach (var el in existingArr.OfType<JObject>())
                        {
                            var k =
                                el[icodeName]?.ToString()
                                ?? el["Icode"]?.ToString()
                                ?? el["icode"]?.ToString();

                            if (!string.IsNullOrWhiteSpace(k))
                            {
                                // se ci sono duplicati, tieni priorità al primo "vero" (sovrascriveremo solo con action A/M/D)
                                if (!container.ContainsKey(k))
                                    container[k] = el;
                                else
                                {
                                    // se arrivano duplicati, logica di merge minima: tieni elemento con action in A/M/D
                                    var old = container[k] as JObject;
                                    var oldAct = old?["action"]?.ToString();
                                    var newAct = el?["action"]?.ToString();
                                    if (!string.IsNullOrWhiteSpace(newAct) && "AMD".Contains(newAct))
                                        container[k] = el;
                                }
                            }
                            else
                            {
                                // Elemento senza icode: genera una chiave fittizia unica
                                container[Guid.NewGuid().ToString("N")] = el;
                            }
                        }

                        // rimpiazza l'array con l'oggetto normalizzato
                        root[baseName] = container;
                    }
                    else
                    {
                        // altro tipo: sovrascrivi con JObject
                        container = new JObject();
                        root[baseName] = container;
                    }
                }
                else
                {
                    // non esiste: crea JObject nuovo
                    container = new JObject();
                    root[baseName] = container;
                }

                // 3) Muovi le proprietà "BaseName[key]" dentro BaseName come container[key] = value
                foreach (var (p, key) in list)
                {
                    // Se esiste già una voce con la stessa key, applica priorità "AMD" in base ad 'action'
                    if (container.TryGetValue(key, out var already) && already is JObject oldObj)
                    {
                        // nuovo valore
                        var newObj = p.Value as JObject;
                        var oldAct = oldObj?["action"]?.ToString();
                        var newAct = newObj?["action"]?.ToString();

                        if (!string.IsNullOrWhiteSpace(newAct) && "AMD".Contains(newAct))
                            container[key] = p.Value;
                    }
                    else
                    {
                        container[key] = p.Value;
                    }

                    // Rimuovi la proprietà esterna "BaseName[key]"
                    p.Remove();
                }
            }
        }


        //==================================================================================================================
        //  CONVERTITORI PERSONALIZZATI
        //==================================================================================================================

        public class DateOnlyJsonConverter : JsonConverter<DateOnly?>
        {
            private readonly string _options;
            public DateOnlyJsonConverter(string options) { _options = options; }
            public override DateOnly? ReadJson(JsonReader reader, System.Type objectType, DateOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var str = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(str)) return null;
                return DateOnly.TryParse(str, out var dateOnly) ? dateOnly : null;
            }

            public override void WriteJson(JsonWriter writer, DateOnly? value, JsonSerializer serializer)
            {
                writer.WriteValue(value?.ToString("yyyy-MM-dd"));
            }
        }

        public class TimeOnlyJsonConverter : JsonConverter<TimeOnly?>
        {
            private readonly string _options;
            public TimeOnlyJsonConverter(string options) { _options = options; }
            public override TimeOnly? ReadJson(JsonReader reader, System.Type objectType, TimeOnly? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var str = reader.Value?.ToString();
                if (string.IsNullOrWhiteSpace(str)) return null;
                return TimeOnly.TryParse(str, out var timeOnly) ? timeOnly : null;
            }

            public override void WriteJson(JsonWriter writer, TimeOnly? value, JsonSerializer serializer)
            {
                writer.WriteValue(value?.ToString("HH:mm:ss"));
            }
        }

        public class CharJsonConverter : JsonConverter<char?>
        {
            private readonly string _options;
            public CharJsonConverter(string options) { _options = options; }
            public override char? ReadJson(JsonReader reader, System.Type objectType, char? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;
                string str = reader.Value.ToString();
                if (str.Length > 1) { str = str.Substring(0,1); } //trunco ad 1 carattere
                if (char.TryParse(str, out var result)) return result;
                return null;
            }

            public override void WriteJson(JsonWriter writer, char? value, JsonSerializer serializer)
            {
                if (value.HasValue) writer.WriteValue(value.Value);
                else writer.WriteNull();
            }
        }
        public class ShortJsonConverter : JsonConverter<short?>
        {
            private readonly string _options;
            public ShortJsonConverter(string options) { _options = options; }
            public override short? ReadJson(JsonReader reader, System.Type objectType, short? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;
                if (short.TryParse(reader.Value.ToString(), out var result)) return result;
                return null;
            }

            public override void WriteJson(JsonWriter writer, short? value, JsonSerializer serializer)
            {
                if (value.HasValue) writer.WriteValue(value.Value);
                else writer.WriteNull();
            }
        }

        public class LongJsonConverter : JsonConverter<long?>
        {
            private readonly string _options;
            public LongJsonConverter(string options) { _options = options; }
            public override long? ReadJson(JsonReader reader, System.Type objectType, long? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;
                if (long.TryParse(reader.Value.ToString(), out var result)) return result;
                return null;
            }

            public override void WriteJson(JsonWriter writer, long? value, JsonSerializer serializer)
            {
                if (value.HasValue) writer.WriteValue(value.Value);
                else writer.WriteNull();
            }
        }

        public class DoubleJsonConverter : JsonConverter<double?>
        {
            private readonly string _options;
            public DoubleJsonConverter(string options) { _options = options; }
            public override double? ReadJson(JsonReader reader, System.Type objectType, double? existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                if (reader.Value == null) return null;
                if (double.TryParse(reader.Value.ToString(), out var result)) return result;
                return null;
            }

            public override void WriteJson(JsonWriter writer, double? value, JsonSerializer serializer)
            {
                if (value.HasValue) writer.WriteValue(value.Value);
                else writer.WriteNull();
            }
        }

        //public class DictionaryStringConverter : JsonConverter<IDictionary<string, string>>
        //{
        //    public override IDictionary<string, string> ReadJson(JsonReader reader, System.Type objectType, IDictionary<string, string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        //    {
        //        var token = JToken.Load(reader);

        //        if (token.Type == JTokenType.Null)
        //            return new Dictionary<string, string>();

        //        if (token.Type != JTokenType.Object)
        //            throw new JsonSerializationException("Expected JSON object for IDictionary<string, string>");

        //        var result = new Dictionary<string, string>();
        //        foreach (var prop in ((JObject)token).Properties())
        //        {
        //            result[prop.Name] = prop.Value.Type == JTokenType.Null ? null : prop.Value.ToString();
        //        }

        //        return result;
        //    }

        //    public override void WriteJson(JsonWriter writer, IDictionary<string, string> value, JsonSerializer serializer)
        //    {
        //        var obj = new JObject();
        //        foreach (var kvp in value)
        //        {
        //            obj[kvp.Key] = kvp.Value;
        //        }
        //        obj.WriteTo(writer);
        //    }
        //}

        //public class DictionaryStringConverter : JsonConverter<IDictionary<string, string>>
        //{
        //    private readonly string _options;
        //    public DictionaryStringConverter(string options) { _options = options; }
        //    public override IDictionary<string, string> ReadJson(JsonReader reader, System.Type objectType, IDictionary<string, string> existingValue, bool hasExistingValue, JsonSerializer serializer)
        //    {
        //        var token = JToken.Load(reader);

        //        if (token.Type == JTokenType.Null)
        //            return new Dictionary<string, string>(StringComparer.Ordinal);

        //        // Se il token è una stringa, può essere Base64Url del JSON, oppure JSON testuale
        //        if (token.Type == JTokenType.String)
        //        {
        //            var str = token.ToString().Trim();
        //            if (string.IsNullOrEmpty(str))
        //                return new Dictionary<string, string>(StringComparer.Ordinal);

        //            if (str.StartsWith("{"))
        //            {
        //                try
        //                {
        //                    token = JToken.Parse(str);
        //                }
        //                catch (JsonReaderException)
        //                {
        //                    throw new JsonSerializationException("Stringa non valida come JSON.");
        //                }
        //            }
        //            else
        //            {
        //                throw new JsonSerializationException("Stringa non rappresenta un oggetto JSON.");
        //            }
        //        }

        //        if (token.Type != JTokenType.Object)
        //            throw new JsonSerializationException("Expected JSON object for IDictionary<string, string>");

        //        var result = new Dictionary<string, string>();
        //        foreach (var prop in ((JObject)token).Properties())
        //        {
        //            result[prop.Name] = prop.Value.Type == JTokenType.Null ? null : prop.Value.ToString();
        //        }

        //        return result;
        //    }

        //    public override void WriteJson(JsonWriter writer, IDictionary<string, string> value, JsonSerializer serializer)
        //    {
        //        if (value == null)
        //        {
        //            writer.WriteNull();
        //            return;
        //        }

        //        // Serializza il dizionario come stringa JSON
        //        string jsonString = JsonConvert.SerializeObject(value);
        //        writer.WriteValue(jsonString);
        //    }
        //}

        /// <summary>
        /// Converter per IDictionary&lt;string,string&gt; che:
        /// - Read: accetta oggetto JSON, oppure stringa Base64Url che contiene il JSON,
        ///   oppure stringa che contiene direttamente il JSON; fa parse e restituisce il dizionario.
        /// - Write: scrive una STRINGA contenente il JSON del dizionario (comportamento originale),
        ///   oppure la STRINGA Base64Url del JSON se attivato via settings.Context.
        /// </summary>
        public class DictionaryStringConverter : JsonConverter<IDictionary<string, string>>
        {
            private readonly string _options;
            public DictionaryStringConverter(string options) { _options = options; }
            public override IDictionary<string, string> ReadJson(JsonReader reader, System.Type objectType, IDictionary<string, string> existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var token = JToken.Load(reader);

                // null -> dizionario vuoto
                if (token.Type == JTokenType.Null)
                    return new Dictionary<string, string>(StringComparer.Ordinal);

                // Se il token è una stringa, può essere Base64Url del JSON, oppure JSON testuale
                if (token.Type == JTokenType.String)
                {
                    var str = token.ToString().Trim();

                    if (string.IsNullOrEmpty(str))
                        return new Dictionary<string, string>(StringComparer.Ordinal);

                    string jsonText;

                    if (UtilHelper.LooksLikeBase64Url(str) && UtilHelper.TryFromBase64Url(str, out var decoded))
                    {
                        jsonText = decoded;
                    }
                    else
                    {
                        // Non è (o non sembra) base64url -> deve essere JSON testuale
                        if (!LooksLikeJsonObject(str))
                            throw new JsonSerializationException("Stringa non rappresenta un oggetto JSON né Base64Url di JSON.");
                        jsonText = str;
                    }

                    // Parse del JSON ottenuto (deve essere un oggetto)
                    var inner = JToken.Parse(jsonText);
                    if (inner.Type != JTokenType.Object)
                        throw new JsonSerializationException("La stringa (decodificata o testuale) non contiene un oggetto JSON.");

                    return ReadDictionaryFromJObject((JObject)inner);
                }

                // Se è un oggetto JSON, parse diretto
                if (token.Type == JTokenType.Object)
                {
                    return ReadDictionaryFromJObject((JObject)token);
                }

                throw new JsonSerializationException($"Token inatteso: atteso Object o String, trovato {token.Type}.");
            }

            //public override void WriteJson(JsonWriter writer, IDictionary<string, string> value, JsonSerializer serializer)
            //{
            //    if (value == null)
            //    {
            //        writer.WriteNull();
            //        return;
            //    }

            //    //// Serializza il dizionario in un JObject usando il serializer corrente
            //    //// (così rispetta eventuali impostazioni globali).
            //    //var obj = JObject.FromObject(value, serializer);
            //    //var jsonString = obj.ToString(Formatting.None);
            //    //// Leggi eventuale configurazione in options
            //    //if (this._options.Contains("DictionaryStringEncodeAsBase64Url"))
            //    //{
            //    //    var b64 = UtilHelper.ToBase64Url(jsonString);
            //    //    writer.WriteValue(b64); // STRINGA base64url
            //    //}
            //    //else
            //    //{
            //    //    writer.WriteValue(jsonString); // STRINGA contenente il JSON (comportamento originale)
            //    //}

            //    // Serializza il dizionario come stringa JSON
            //    string jsonString = JsonConvert.SerializeObject(value);

            //    if (this._options.Contains("DictionaryStringEncodeAsBase64Url"))
            //    {
            //        // 1) Serializza il dizionario in un JSON compatto (come testo)
            //        string json = SerializeDictionaryToCompactJson(value);

            //        // 2) Codifica in Base64Url
            //        string b64 = UtilHelper.ToBase64Url(jsonString);

            //        // 3) Scrivi UNA STRINGA (ok come valore di proprietà o root)
            //        writer.WriteValue(b64);
            //        return;
            //    }

            //    // Default: scrivi VERO OGGETTO JSON (nessuna stringa "contenente JSON")
            //    writer.WriteStartObject();
            //    foreach (var kv in value)
            //    {
            //        writer.WritePropertyName(kv.Key);
            //        writer.WriteValue(kv.Value);
            //    }
            //    writer.WriteEndObject();
            //}
            public override void WriteJson(JsonWriter writer, IDictionary<string, string> value, JsonSerializer serializer)
            {
                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                if (this._options.Contains("DictionaryStringEncodeAsBase64Url"))
                {
                    // 1) Costruisci JSON testuale compatto del dizionario
                    var jo = BuildJObject(value);
                    string json = jo.ToString(Formatting.None);

                    // 2) Codifica base64url e scrivi UNA STRINGA
                    string b64 = UtilHelper.ToBase64Url(json);
                    writer.WriteValue(b64);
                    return;
                }

                // Default: scrivi un OGGETTO JSON.
                // --> Importantissimo: non chiamare StartObject/EndObject manualmente,
                //     ma costruire un JObject e poi jo.WriteTo(writer).
                var obj = BuildJObject(value);
                obj.WriteTo(writer);
            }
            // ===== Helpers =====

            private static bool LooksLikeJsonObject(string s)
                => s.Length > 1 && s[0] == '{' && s[s.Length - 1] == '}';

            private static IDictionary<string, string> ReadDictionaryFromJObject(JObject obj)
            {
                var result = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var prop in obj.Properties())
                {
                    if (prop.Value.Type == JTokenType.Null)
                    {
                        result[prop.Name] = null;
                        continue;
                    }

                    if (prop.Value.Type == JTokenType.String)
                    {
                        result[prop.Name] = prop.Value.ToString();
                    }
                    else
                    {
                        // Per valori non stringa, manteniamo il JSON "raw" del valore
                        result[prop.Name] = prop.Value.ToString(Formatting.None);
                    }
                }
                return result;
            }

            //private static string SerializeDictionaryToCompactJson(IDictionary<string, string> dict)
            //{
            //    // Evitiamo di usare il serializer corrente per non entrare in ricorsione con questo stesso converter.
            //    var obj = new JObject();
            //    foreach (var kv in dict)
            //        obj[kv.Key] = kv.Value; // string o null

            //    return obj.ToString(Formatting.None);
            //}

            private static JObject BuildJObject(IDictionary<string, string> dict)
            {
                var jo = new JObject();
                foreach (var kv in dict)
                {
                    // Valori string/nullable string
                    if (kv.Value is null)
                        jo[kv.Key] = JValue.CreateNull();
                    else
                        jo[kv.Key] = kv.Value;
                }
                return jo;
            }


        }


        public class ListModelErpJsonConverter : JsonConverter<List<ModelErp>>
        {
            private readonly DogManager _dogMng;
            private readonly string _options;

            //1//public ListModelErpJsonConverter(DogManager dogMng) => _dogMng = dogMng;
            public ListModelErpJsonConverter(DogManager dogMng, string options) { _dogMng = dogMng; _options = options; }

            public override List<ModelErp> ReadJson(JsonReader reader, System.Type objectType, List<ModelErp> existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                var array = JArray.Load(reader);
                var result = new List<ModelErp>(); var resultDict = new Dictionary<object,ModelErp>();
                if (_dogMng == null) return result;

                System.Type? tipoConcreto = null;

                foreach (var token in array)
                {
                    if (tipoConcreto == null && token.Type == JTokenType.Object)
                    {
                        //var firstProp = ((JObject)token).Properties().FirstOrDefault();
                        // prima proprietà che ha definito l'attributo ErpDogFieldAttribute
                        var firstProp = ((JObject)token)
                            .Properties()
                            .FirstOrDefault(p => p.GetType()
                                                 .GetCustomAttributes(typeof(ErpDogFieldAttribute), true)
                                                 .Length > 0);
                        //---
                        if (firstProp != null)
                        {
                            var typeKey = firstProp.Name;
                            if (_dogMng.tabProperties.TryGetValue(typeKey, out var dogField))
                            {
                                tipoConcreto = dogField.table.tableTpy;
                            }
                        }
                    }

                    try
                    {
                        ModelErp item;
                        if (tipoConcreto != null)
                        {
                            item = (ModelErp)token.ToObject(tipoConcreto, serializer);
                            var icode = item.getIcode();
                            var action = item.action;
                            // check univocità icode in caso di concomitanza tengo il primo con action in AMD
                            if (resultDict.ContainsKey(icode) == false) resultDict.Add(icode, item);
                            else if ("AMD".Contains(action ?? ' ')) resultDict[icode] = item; // se action valida allora sostituisco
                        }
                        else
                        {
                            item = token.ToObject<ModelErp>(serializer);
                            result.Add(item);
                        }
                    }
                    catch
                    {
                        continue; // ignora errori su singoli elementi
                    }
                }
                if (tipoConcreto != null) result = resultDict.Values.ToList();
                return result.All(x => x == null) ? null : result;
            }

            public override void WriteJson(JsonWriter writer, List<ModelErp> value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        public class DictionaryModelErpJsonConverter : JsonConverter
        {
            private readonly DogManager _dogMng;
            private readonly string _options;

            public DictionaryModelErpJsonConverter(DogManager dogMng, string options)
            {
                _dogMng = dogMng;
                _options = options;
            }
            public override bool CanConvert(System.Type objectType)
            {
                // intercetto SOLO Dictionary<string, T> dove T è ModelErp o sottotipo, per evitare di sovrascrivere il comportamento di default su altri dizionari
                return objectType.IsGenericType &&
                       objectType.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
                       objectType.GetGenericArguments()[0] == typeof(string) &&
                       typeof(ModelErp).IsAssignableFrom(objectType.GetGenericArguments()[1]);
            }
            public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
            {
                var token = JToken.Load(reader);

                //var dict = (IDictionary)(existingValue ?? Activator.CreateInstance(objectType));
                //IDictionary dict = (IDictionary)(existingValue ?? Activator.CreateInstance(objectType));

                System.Type keyType = typeof(string);
                System.Type valueType = objectType.GetGenericArguments()[1];  // Campione / altro ModelErp derivato
                System.Type dictType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
                IDictionary dict = (IDictionary)Activator.CreateInstance(dictType);

                if (dict == null) return null;

                if (token.Type == JTokenType.Object)
                {
                    // già un oggetto { key: { ... } } → deserializza nativo
                    foreach (var p in ((JObject)token).Properties())
                    {
                        try
                        {
                            var el_token = p.Value;
                            if (string.IsNullOrEmpty(p.Name)) continue; // scarta chiavi vuote

                            if (el_token.Type != JTokenType.Object) continue;
                            ModelErp item = (ModelErp)((JObject)el_token).ToObject(objectType.GetGenericArguments()[1], serializer);
                            var icode = item.getIcode();
                            var action = item.action;
                            // check univocità icode in caso di concomitanza tengo il primo con action in AMD
                            if (icode != null)
                            {
                                string strIcode = icode.ToString();
                                if (strIcode != p.Name) throw new JsonSerializationException($"Struttura JSON non valida per Dictionary<string,{objectType.GetGenericArguments()[1].ToString()}> : strIcode[{strIcode}] != p.Name[{p.Name}].");
                                if (dict.Contains(strIcode) == false) dict.Add(strIcode, item);
                                else if ("AMD".Contains(action ?? ' ')) dict[strIcode] = item; // se action valida allora sostituisco
                            }
                        }
                        catch
                        {
                            continue; // ignora errori su singoli elementi
                        }
                    }
                    return dict;
                }
                if (token.Type == JTokenType.Array)
                {
                    foreach (var el_token in (JArray)token)
                    {
                        try
                        {
                            if (el_token.Type != JTokenType.Object) continue;
                            ModelErp item = (ModelErp) ((JObject)el_token).ToObject(objectType.GetGenericArguments()[1], serializer);
                            var icode = item.getIcode();
                            var action = item.action;
                            // check univocità icode in caso di concomitanza tengo il primo con action in AMD
                            if (icode != null)
                            {
                                string strIcode = icode.ToString();
                                if (dict.Contains(strIcode) == false) dict.Add(strIcode, item);
                                else if ("AMD".Contains(action ?? ' ')) dict[strIcode] = item; // se action valida allora sostituisco
                            }
                        }
                        catch
                        {
                            continue; // ignora errori su singoli elementi
                        }
                    }
                    return dict;
                }

                if (token.Type == JTokenType.Null) return dict;

                throw new JsonSerializationException($"Struttura JSON non valida per Dictionary<string,{objectType.GetGenericArguments()[1].ToString()}> : trovato {token.Type}.");


                //var token = JToken.Load(reader);
                //if (token == null) return null;
                //var resultDict = new Dictionary<object, ModelErp>(); 
                //if (_dogMng == null) return resultDict;
                //if (token.Type == JTokenType.Null) return resultDict;

                //System.Type? tipoConcreto = null;
                //if (token.Type == JTokenType.Object)
                //{
                //    // già un oggetto { key: { ... } } → deserializza nativo
                //    foreach (var p in ((JObject)token).Properties())
                //    {
                //        var el_token = p.Value;
                //        if (string.IsNullOrEmpty(p.Name)) continue; // scarta chiavi vuote

                //        if (tipoConcreto == null && el_token.Type == JTokenType.Object)
                //        {
                //            // prima proprietà che ha definito l'attributo ErpDogFieldAttribute
                //            var firstProp = ((JObject)el_token)
                //                .Properties()
                //                .FirstOrDefault(p => p.GetType()
                //                                     .GetCustomAttributes(typeof(ErpDogFieldAttribute), true)
                //                                     .Length > 0);
                //            //---
                //            if (firstProp != null)
                //            {
                //                var typeKey = firstProp.Name;
                //                if (_dogMng.tabProperties.TryGetValue(typeKey, out var dogField))
                //                {
                //                    tipoConcreto = dogField.table.tableTpy;
                //                }
                //            }
                //        }
                //        if (tipoConcreto == null) throw new JsonSerializationException($"Struttura JSON non valida per dizionario<string,T> with T: ModelErp. tipoConcreto==null.");

                //        try
                //        {
                //            ModelErp item = (ModelErp)el_token.ToObject(tipoConcreto, serializer);
                //            var icode = item.getIcode();
                //            var action = item.action;
                //            // check univocità icode in caso di concomitanza tengo il primo con action in AMD
                //            if (icode != null)
                //            {
                //                string strIcode = icode.ToString();
                //                if (strIcode != p.Name) throw new JsonSerializationException($"Struttura JSON non valida per Dictionary<string,T> with T: ModelErp. strIcode[{strIcode}] != p.Name[{p.Name}].");
                //                if (resultDict.ContainsKey(strIcode) == false) resultDict.Add(strIcode, item);
                //                else if ("AMD".Contains(action ?? ' ')) resultDict[strIcode] = item; // se action valida allora sostituisco
                //            }
                //        }
                //        catch
                //        {
                //            continue; // ignora errori su singoli elementi
                //        }
                //    }
                //    return resultDict;
                //}
                //if (token.Type == JTokenType.Array)
                //{
                //    foreach (var el_token in (JArray)token)
                //    {
                //        if (tipoConcreto == null && el_token.Type == JTokenType.Object)
                //        {
                //            // prima proprietà che ha definito l'attributo ErpDogFieldAttribute
                //            foreach (var prop in ((JObject)el_token).Properties())
                //            {
                //                if (_dogMng.tabProperties.TryGetValue(prop.Name, out var dogField)) 
                //                { 
                //                    tipoConcreto = dogField.table.tableTpy;
                //                    break; 
                //                }
                //            }
                //        }
                //        if (tipoConcreto == null) throw new JsonSerializationException($"Struttura JSON non valida per Dictionary<string,T> with T: ModelErp. tipoConcreto==null.");

                //        try
                //        {
                //            ModelErp item = (ModelErp)el_token.ToObject(tipoConcreto, serializer);
                //            var icode = item.getIcode();
                //            var action = item.action;
                //            // check univocità icode in caso di concomitanza tengo il primo con action in AMD
                //            if (icode != null)
                //            {
                //                string strIcode = icode.ToString();
                //                if (resultDict.ContainsKey(strIcode) == false) resultDict.Add(strIcode, item);
                //                else if ("AMD".Contains(action ?? ' ')) resultDict[strIcode] = item; // se action valida allora sostituisco
                //            }
                //        }
                //        catch
                //        {
                //            continue; // ignora errori su singoli elementi
                //        }
                //    }
                //    //return resultDict;
                //    return Convert.ChangeType(resultDict, tipoConcreto);
                //}

                //throw new JsonSerializationException($"Struttura JSON non valida per Dictionary<string,T> with T: ModelErp. trovato {token.Type}.");
            }


            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                var dict = (System.Collections.IDictionary)value;
                var jo = new JObject();

                foreach (System.Collections.DictionaryEntry de in dict)
                {
                    jo[(string)de.Key] = JToken.FromObject(de.Value, serializer);
                }

                jo.WriteTo(writer);
            }
        }


        public class ModelErpConverter : JsonConverter<ModelErp>
        {
            private readonly string _options;
            public ModelErpConverter(string options) { _options = options; }
            public override ModelErp ReadJson(JsonReader reader, System.Type objectType, ModelErp existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                // salva il path PRIMA di consumare il reader
                string propertyPath = reader.Path;
                string propertyName = propertyPath?.Split('.').Last() ?? "(root)";
                string objectTypeName = objectType?.Name ?? "(unknown)";

                if (_logger.IsTraceEnabled) { _logger.Trace($"[ModelErpConverter] Inizio deserializzazione di {objectTypeName} (prop='{propertyName}')"); }

                // carica il token (consuma il reader)
                JToken token = JToken.Load(reader);

                // se il token non è un oggetto, prova fallback semplice
                if (token == null || token.Type != JTokenType.Object)
                {
                    try
                    {
                        // fallback: lascia fare a Json.NET
                        return token?.ToObject(objectType, serializer) as ModelErp;
                    }
                    catch (Exception ex)
                    {
                        throw new JsonSerializationException($"Token non oggetto durante deserializzazione di {objectTypeName}: {ex.Message}", ex);
                    }
                }

                JObject obj = (JObject)token;


                //-----------------
                //!!//LogPropertiesRecursive(obj, indent: 0, maxDepth: DebugMaxDepth); // LOG: stampa ricorsiva delle proprietà visitate (debug)

                //!!!!! rimuove proprietà con valore null/undefined per evitare di sovrascrivere valori esistenti e per evitare CICLI RICORSIVI ==> stack overflow
                var toRemove = obj.Properties().Where(p => p.Value.Type == JTokenType.Null || p.Value.Type == JTokenType.Undefined).ToList();
                foreach (var p in toRemove) p.Remove();
                //!!!!!

                //!!//obj = JsonFilterExclude(obj);  //applica alri filtri di esclusione
                //-----------------

                // CREA l'istanza target: usa existingValue se presente, altrimenti un'istanza "non inizializzata"
                object targetInstance;
                if (existingValue != null)
                {
                    targetInstance = existingValue;
                }
                else
                {
                    try
                    {
                        // ATTENZIONE: GetUninitializedObject non esegue il costruttore.
                        // Se il costruttore è importante, sostituisci con:
                        // targetInstance = Activator.CreateInstance(objectType, nonPublic: true);
                        targetInstance = FormatterServices.GetUninitializedObject(objectType);
                    }
                    catch (Exception ex)
                    {
                        // fallback più "sicuro" se GetUninitializedObject non è permesso
                        targetInstance = Activator.CreateInstance(objectType, nonPublic: true)
                                         ?? throw new JsonSerializationException($"Impossibile creare istanza di {objectTypeName}: {ex.Message}", ex);
                    }
                }

                // Usa Populate su un reader creato dal JObject: questo popolerà l'istanza esistente.
                // I figli ModelErp saranno deserializzati normalmente e passeranno attraverso questo converter (senza loop infinito sul root).
                using (var subReader = obj.CreateReader())
                {
                    try
                    {
                        serializer.Populate(subReader, targetInstance);

                        if (_logger.IsTraceEnabled) { _logger.Trace($"[ModelErpConverter] Fine deserializzazione di {objectTypeName} (prop='{propertyName}')"); }

                        return (ModelErp)targetInstance;
                    }
                    catch (Exception ex)
                    {
                        if (_logger.IsTraceEnabled) { _logger.Trace($"[ModelErpConverter][ERRORE] Errore populate su {objectTypeName}: {ex.GetType().Name} - {ex.Message}"); }
                        throw new JsonSerializationException($"Errore durante Populate di {objectTypeName}: {ex.Message}", ex);
                    }
                }
            }

            public override void WriteJson(JsonWriter writer, ModelErp value, JsonSerializer serializer)
            {
                //// serializzazione normale
                //serializer.Serialize(writer, value);

                if (value == null)
                {
                    writer.WriteNull();
                    return;
                }

                JsonSerializer newSerializer = new JsonSerializer();
                // Copia le impostazioni dalla tua libreria
                newSerializer.NullValueHandling = serializer.NullValueHandling;
                newSerializer.ReferenceLoopHandling = serializer.ReferenceLoopHandling;
                newSerializer.Formatting = serializer.Formatting;
                newSerializer.ContractResolver = serializer.ContractResolver;

                // Copia anche i converter
                foreach (var conv in serializer.Converters)
                {
                    if (conv.GetType() != this.GetType()) newSerializer.Converters.Add(conv);  // scarto questo converter per evitare ricorsione infinita
                }

                // Usa JToken.FromObject per serializzare l'oggetto senza il converter corrente,
                // in modo da evitare la ricorsione infinita.
                JToken t = JToken.FromObject(value, newSerializer);
                t.WriteTo(writer);

            }


            //////// esempio di serializzazione con limite di profondità (non usato)

            //////private const int MaxDepth = 3; // Imposta il limite di profondità desiderato
            //////private static readonly ThreadLocal<int> _currentDepth = new ThreadLocal<int>();

            //////public override void WriteJson(JsonWriter writer, ModelErp value, JsonSerializer serializer)
            //////{
            //////    // Incrementa la profondità corrente
            //////    _currentDepth.Value++;

            //////    // Controlla se abbiamo superato il limite di profondità
            //////    if (_currentDepth.Value > MaxDepth)
            //////    {
            //////        // Scrivi null o un altro valore placeholder e esci
            //////        writer.WriteNull();
            //////        _currentDepth.Value--; // Decrementa prima di uscire
            //////        return;
            //////    }

            //////    // Serializza l'oggetto usando un nuovo serializzatore per evitare loop infiniti.
            //////    // Puoi anche usare JToken.FromObject() come suggerito in precedenza.
            //////    // Questo è il modo corretto per procedere senza ricorsione infinita.
            //////    JObject obj = JObject.FromObject(value, serializer);
            //////    obj.WriteTo(writer);

            //////    // Decrementa la profondità
            //////    _currentDepth.Value--;
            //////}




        }

        // ----- helper -----

        private static JObject JsonFilterExclude(JObject obj)
        {
            // Log e filtro: rimuovi proprietà con suffisso _New e valori null/undefined
            string SuffixToIgnore = "_New";
            var toRemove = obj.Properties()
                              .Where(p => p.Name.EndsWith(SuffixToIgnore, StringComparison.Ordinal)  // SuffixToIgnore
                                       || p.Value.Type == JTokenType.Null
                                       || p.Value.Type == JTokenType.Undefined)
                              .ToList();

            foreach (var p in toRemove)
            {
                //_logger.Trace($"  [IGNORATA] {p.Name} (motivo: {(p.Name.EndsWith(SuffixToIgnore, StringComparison.Ordinal) ? $"suffisso {SuffixToIgnore}" : "null/undefined")})");
                p.Remove();
            }
            return obj;
        }

        //funzione di Log
        private static void LogPropertiesRecursive(JToken token, int indent, int maxDepth)
        {
            StringBuilder sb = new StringBuilder();
            if (indent > maxDepth)
            {
                //Console.WriteLine(new string(' ', indent * 2) + "... (depth limit reached)");
                _logger.Trace(new string(' ', indent * 2) + "... (depth limit reached)"); 
                return;
            }

            if (token is JObject obj)
            {
                foreach (var prop in obj.Properties())
                {
                    var line = new string(' ', indent * 2) + $"- {prop.Name} (Type={prop.Value.Type})";

                    //Console.WriteLine(line);
                    _logger.Trace(line);

                    // se l'elemento è un oggetto o array, ricorsione
                    if (prop.Value is JObject || prop.Value is JArray)
                    {
                        LogPropertiesRecursive(prop.Value, indent + 1, maxDepth);
                    }
                    else
                    {
                        // stampa breve valore (attenzione a non stampare stringone)
                        if (prop.Value.Type == JTokenType.String || prop.Value.Type == JTokenType.Integer ||
                            prop.Value.Type == JTokenType.Float || prop.Value.Type == JTokenType.Boolean)
                        {
                            //Console.WriteLine(new string(' ', (indent + 1) * 2) + $"  valore-preview: {TrimPreview(prop.Value.ToString())}");
                            _logger.Trace(new string(' ', (indent + 1) * 2) + $"  valore-preview: {TrimPreview(prop.Value.ToString())}");
                        }
                    }
                }
            }
            else if (token is JArray arr)
            {
                int i = 0;
                foreach (var item in arr)
                {
                    //Console.WriteLine(new string(' ', indent * 2) + $"- [array][{i}] (Type={item.Type})");
                    _logger.Trace(new string(' ', indent * 2) + $"- [array][{i}] (Type={item.Type})");

                    if (item is JObject || item is JArray)
                    {
                        LogPropertiesRecursive(item, indent + 1, maxDepth);
                    }
                    else
                    {
                        //Console.WriteLine(new string(' ', (indent + 1) * 2) + $"  valore-preview: {TrimPreview(item.ToString())}");
                        _logger.Trace(new string(' ', (indent + 1) * 2) + $"  valore-preview: {TrimPreview(item.ToString())}");
                    }
                    i++;
                }
            }
            else
            {
                //Console.WriteLine(new string(' ', indent * 2) + $"- token (Type={token.Type}): {TrimPreview(token.ToString())}");
                _logger.Trace(new string(' ', indent * 2) + $"- token (Type={token.Type}): {TrimPreview(token.ToString())}");

            }
        }
        private static string TrimPreview(string s, int max = 120)
        {
            if (string.IsNullOrEmpty(s)) return s;
            s = s.Replace(Environment.NewLine, " ");
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }

        //verifica se stringa è Json
        private static bool IsJson(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            s = s.TrimStart();
            return s.StartsWith("{") || s.StartsWith("[");
        }


    }
}
