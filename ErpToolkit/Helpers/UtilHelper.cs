//using ErpToolkit.Controllers;  //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using NLog;
using System.Buffers.Text;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace ErpToolkit.Helpers
{
    public static class UtilHelper
    {
        private static readonly NLog.ILogger _logger;
        static UtilHelper()
        {
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();  //SetUpNLog();
        }
        //******************************************************************************************************************
        //configura NLog per la classe
        //////public static NLog.Config.LoggingConfiguration GetNLogConfig()
        //////{

        //////    NLog.GlobalDiagnosticsContext.Set("SessionLog", ErpContext.Session);

        //////    var config = new NLog.Config.LoggingConfiguration();
        //////    // Targets where to log to: File and Console
        //////    //var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "ErpToolkit.log" };  //{ FileName = "backupclientlogfile_backupservice.txt" };
        //////    var logfile = new NLog.Targets.FileTarget("logfile") { 
        //////        FileName = "ErpToolkit.log",
        //////        Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${exception:format=tostring}"
        //////    };  
        //////    var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
        //////    // Rules for mapping loggers to targets            
        //////    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
        //////    config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);
        //////    return config;
        //////}
        public static NLog.Config.LoggingConfiguration GetNLogConfig()
        {
            // Directory base: EXE in produzione, progetto in sviluppo
            string baseDir = ErpContext.CurrentDirectory; // <-- il tuo helper
            string logDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logDir);

            NLog.GlobalDiagnosticsContext.Set("SessionLog", ErpContext.Session);

            var config = new NLog.Config.LoggingConfiguration();

            var logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = Path.Combine(logDir, "ErpToolkit.log"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${exception:format=tostring}",

                ArchiveEvery = NLog.Targets.FileArchivePeriod.Day,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.Date,
                ArchiveDateFormat = "yyyyMMdd",
                MaxArchiveFiles = 30,

                ConcurrentWrites = true,
                KeepFileOpen = false
            };

            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");

            // Rules for mapping loggers to targets            
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logconsole);
            config.AddRule(NLog.LogLevel.Info, NLog.LogLevel.Fatal, logfile);

            return config;
        }
        //******************************************************************************************************************


        //Converti stringa NomeCampo in NomeProprieta
        public static string field2Property(string s)
        {
            string ret = "", c_2,c_1,c; s = "###" + s;
            for (var I = Microsoft.VisualBasic.Strings.Len(s); I >= 0; I += -1)
            {
                c_2 = Microsoft.VisualBasic.Strings.Mid(s, I - 2, 1); c_1 = Microsoft.VisualBasic.Strings.Mid(s, I - 1, 1); c = Microsoft.VisualBasic.Strings.Mid(s, I, 1);
                if (c == "#") break;
                else if (c_2 == "_" & c_1 == "_")
                    ret = "1" + c.ToUpper() + ret;
                else if (c_1 == "#" | c_1 == "_")
                {
                    if (c != "_") ret = c.ToUpper() + ret;
                }
                else if (c == "_") ; // skip
                else ret = c.ToLower() + ret;
            }
            return ret;

        }
        //Converti stringa NomeCampo in NomeProprieta, cercando il campo SQL nell'attributo ErpDogFieldAttribute delle proprietà della classe
        public static string? sqlFieldName2PropertyName(System.Type classType, string qlFieldNameAttributeValue)
        {
            // Cerca la proprietà che ha l'attributo ErpDogField con il valore cercato
            var property = classType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(ErpDogFieldAttribute), true)
                    .Cast<ErpDogFieldAttribute>()
                    .Any(a => a.SqlFieldName == qlFieldNameAttributeValue)); 
            return property?.Name; // Ritorna il nome della proprietà (es. "SelPaadmDataDimissione") o null
        }
        // Restituisce la lista dei nomi delle proprietà della classe che hanno l'attributo ErpDogFieldAttribute con Xref valorizzato (non vuoto)
        public static List<string> getPropertiesWithXref(System.Type classType)
        {
            return classType.GetProperties()
                .Where(p =>
                {
                    // Cerca l'attributo ErpDogField sulla proprietà
                    var attr = p.GetCustomAttribute<ErpDogFieldAttribute>();

                    // Verifica che l'attributo esista e che Xref non sia vuoto
                    return attr != null && !string.IsNullOrWhiteSpace(attr.Xref);
                })
                .Select(p => p.Name) // Estrae il nome della proprietà
                .ToList();
        }

        //Cripta & Decripta -- Simple3Des
        private const string CRYP_KEY_STR = "&%£73Erp#$";

        public static string EncryptData(string plaintext)
        {
            TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
            // Initialize the crypto provider.
            TripleDes.Key = TruncateHash(CRYP_KEY_STR, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);

            // Convert the plaintext string to a byte array.
            byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);

            // Create the stream.
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            // Create the encoder to write to the stream.
            CryptoStream encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            // Use the crypto stream to write the byte array to the stream.
            encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
            encStream.FlushFinalBlock();

            // Convert the encrypted stream to a printable string.
            return Convert.ToBase64String(ms.ToArray());
        }

        public static string DecryptData(string encryptedtext)
        {
            TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
            // Initialize the crypto provider.
            TripleDes.Key = TruncateHash(CRYP_KEY_STR, TripleDes.KeySize / 8);
            TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);

            // Convert the encrypted text string to a byte array.
            byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);

            // Create the stream.
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            // Create the decoder to write to the stream.
            CryptoStream decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

            // Use the crypto stream to write the byte array to the stream.
            decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
            decStream.FlushFinalBlock();

            // Convert the plaintext stream to a string.
            return System.Text.Encoding.Unicode.GetString(ms.ToArray());
        }
        private static byte[] TruncateHash(string key, int length)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            // Hash the key.
            byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);
            var oldHash = hash;
            hash = new byte[length - 1 + 1];

            // Truncate or pad the hash.
            if (oldHash != null)
                Array.Copy(oldHash, hash, Math.Min(length - 1 + 1, oldHash.Length));
            return hash;
        }

        // Converto Object in Byte Array

        //public static StructureType ByteArrayToObject2<StructureType>(byte[] Bytes) where StructureType : struct
        //{
        //    int Length = Marshal.SizeOf(typeof(StructureType));
        //    IntPtr Handle = Marshal.AllocHGlobal(Length);
        //    Marshal.Copy(Bytes, 0, Handle, Length);
        //    StructureType Result = (StructureType)Marshal.PtrToStructure(Handle, typeof(StructureType));
        //    Marshal.FreeHGlobal(Handle);
        //    return Result;
        //}

        //public static T ByteArrayToObject<T>(byte[] Bytes)
        //{
        //    if (Bytes == null) throw new ArgumentNullException(nameof(Bytes));
        //    int Length = Marshal.SizeOf(typeof(T));
        //    IntPtr Handle = Marshal.AllocHGlobal(Length);
        //    Marshal.Copy(Bytes, 0, Handle, Length);
        //    T Result = (T)Marshal.PtrToStructure(Handle, typeof(T));
        //    Marshal.FreeHGlobal(Handle);
        //    return Result;
        //}
        //public static byte[] ObjectToByteArray(object Structure)
        //{
        //    int Length = Marshal.SizeOf(Structure);
        //    byte[] Bytes = new byte[Length];
        //    IntPtr Handle = Marshal.AllocHGlobal(Length);
        //    Marshal.StructureToPtr(Structure, Handle, true);
        //    Marshal.Copy(Handle, Bytes, 0, Length);
        //    Marshal.FreeHGlobal(Handle);
        //    return Bytes;
        //}

        //https://github.com/Cysharp/MemoryPack (https://steven-giesel.com/blogPost/4271d529-5625-4b67-bd59-d121f2d8c8f6)

        /// <summary>
        /// Convert an object to a Byte Array, using Protobuf.
        /// </summary>
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null) throw new ArgumentNullException("ObjectToByteArray null " + nameof(obj));
            return MemoryPack.MemoryPackSerializer.Serialize(obj);
        }

        /// <summary>
        /// Convert a byte array to an Object of T, using Protobuf.
        /// </summary>
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            if (arrBytes == null) throw new ArgumentNullException("ByteArrayToObject null " + nameof(arrBytes));
            return MemoryPack.MemoryPackSerializer.Deserialize<T>(arrBytes);
        }


        //Clona Oggetto mediante serializzazione
        //https://www.wwt.com/article/how-to-clone-objects-in-dotnet-core
        public static T DeepCopy<T>(this T self)
        {
            //var serialized = JsonConvert.SerializeObject(self);
            //return JsonConvert.DeserializeObject<T>(serialized);
            if (self == null) throw new ArgumentNullException("DeepCopy null " + nameof(self));
            var serialized = ObjectToByteArray(self);
            return ByteArrayToObject<T>(serialized);
        }

        // LDAP autenticazione
        // If Not LoginLdap(tmpUname, tmpPswd) Then ERROR
        public static bool LoginLdap(string ldap_server, string Username, string Password)
        {
            bool Autenticato = false; string tmpPassword = "", tmpUsername = "", errorMessage = "";
            var entry = new System.DirectoryServices.DirectoryEntry(ldap_server, Username, Password, System.DirectoryServices.AuthenticationTypes.Secure);
            try
            {
                var searcher = new DirectorySearcher(entry);
                searcher.SearchScope = SearchScope.OneLevel;
                if (searcher.FindOne() != null) Autenticato = true;
                else
                {
                    Autenticato = false;
                    LogManager.GetCurrentClassLogger().Info("Autenticazione [" + tmpUsername + "] non riuscita");
                }
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error("Errore di autenticazione [" + tmpUsername + "]: " + ex.Message);
                Autenticato = false;
            }
            return Autenticato;
        }

        //#############################################################################

        //calcola restrizioni visibilità pagina
        //-------------------------------------

        public static DogManager.FieldAttr fieldAttrTagHelper(string prefix, string fieldName, string xrefFieldName, ViewContext viewContext)
        {
            DogManager.FieldAttr attrField = new DogManager.FieldAttr("");
            try
            {

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //string nomePercorso = viewContext.TempData["NomeSequenzaPagine"] as string; viewContext.TempData["NomeSequenzaPagine"] = nomePercorso;  //ricarico per mantenere in memoria
                //List<HomeController.Page> sequenzaPagine = HomeController.PathMenu[nomePercorso];
                //string nomePagina = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)viewContext.ActionDescriptor).ControllerName;
                //int paginaIdx = sequenzaPagine.FindIndex(page => page.pageName.Equals(nomePagina, StringComparison.Ordinal));
                //if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{fieldName}_Attr"))
                //{
                //    attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{fieldName}_Attr"] ?? "");
                //}
                //else if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{prefix}.{fieldName}_Attr"))
                //{
                //    attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{prefix}.{fieldName}_Attr"] ?? "");
                //}
                //else if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{xrefFieldName}_Attr"))
                //{
                //    attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{xrefFieldName}_Attr"] ?? "");
                //}
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

                string homeClassName = ErpContext.Instance.GetString("#homeClassName"); // "ErpToolkit.Controllers.HomeController";     //nome della classe in cui sono definiti i percorsi
                string homePathMenuFieldName = ErpContext.Instance.GetString("#homePathMenuFieldName"); // "PathMenu";     //nome della variabile in cui sono definiti i percorsi
                System.Type type = ErpContext.Instance.AssemblyMODEL.GetType(homeClassName);
                if (type != null)
                {
                    FieldInfo field = type.GetField(homePathMenuFieldName, BindingFlags.Public | BindingFlags.Static);
                    if (field != null && field.FieldType == typeof(Dictionary<string, List<DogManager.Page>>) ) 
                    {
                        Dictionary<string, List<DogManager.Page>> homeFieldValue = (Dictionary<string, List<DogManager.Page>>)field.GetValue(null);
                        if (homeFieldValue != null)
                        {
                            string nomePercorso = viewContext.TempData["NomeSequenzaPagine"] as string; viewContext.TempData["NomeSequenzaPagine"] = nomePercorso;  //ricarico per mantenere in memoria
                            List<DogManager.Page> sequenzaPagine = homeFieldValue[nomePercorso];
                            string nomePagina = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)viewContext.ActionDescriptor).ControllerName;
                            int paginaIdx = sequenzaPagine.FindIndex(page => page.pageName.Equals(nomePagina, StringComparison.Ordinal));
                            if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{fieldName}_Attr"))
                            {
                                attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{fieldName}_Attr"] ?? "");
                            }
                            else if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{prefix}.{fieldName}_Attr"))
                            {
                                attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{prefix}.{fieldName}_Attr"] ?? "");
                            }
                            else if (sequenzaPagine[paginaIdx].defaultFields.ContainsKey($"{xrefFieldName}_Attr"))
                            {
                                attrField = new DogManager.FieldAttr(sequenzaPagine[paginaIdx].defaultFields[$"{xrefFieldName}_Attr"] ?? "");
                            }
                        }
                    }
                }

                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


                if (viewContext.ViewData.ContainsKey("TagHelper__READONLY_PAGE"))
                {
                    string READONLY_PAGE = viewContext?.ViewData["TagHelper__READONLY_PAGE"]?.ToString() ?? ""; //deve essere scritto nella PartialView
                    if (READONLY_PAGE == "Y") { attrField.Readonly = 'Y'; }
                }
            }
            catch (Exception ex) { } // skip exeptions

            // Convert the plaintext stream to a string.
            return attrField;
        }



        //#############################################################################

        //DB utility
        //public static object DecodeJsonElement(object xvalue) //Decodifica tipi JSON, se le variabili vengono da pagina web
        //{
        //    if (xvalue != null && xvalue is System.Text.Json.JsonElement jsonElement)
        //    {
        //        if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Null) xvalue = (object)null;
        //        else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String) xvalue = (string)jsonElement.GetString();
        //        else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
        //        {
        //            if (jsonElement.TryGetInt16(out short shortValue)) xvalue = (short)shortValue;
        //            else if (jsonElement.TryGetInt32(out int intValue)) xvalue = (int)intValue;
        //            else if (jsonElement.TryGetInt64(out long longValue)) xvalue = (long)longValue;
        //            else if (jsonElement.TryGetDouble(out double doubleValue)) xvalue = (double)doubleValue;
        //        }
        //        else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.True) xvalue = (bool)true;
        //        else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.False) xvalue = (bool)false;
        //        else throw new System.Text.Json.JsonException($"Il tipo JsonElement {jsonElement.ValueKind.ToString()} non è supportato ({xvalue?.ToString() ?? ""})");
        //    }
        //    return xvalue;
        //}

        /// <summary>
        /// Decodifica valori JSON generici, che possano provenire da System.Text.Json o da Newtonsoft.Json.
        /// </summary>
        public static object? DecodeJsonElement(object? xvalue)
        {
            // Caso 1: System.Text.Json.JsonElement
            if (xvalue is System.Text.Json.JsonElement jsonElement)
            {
                if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Null)
                    return null;
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.String)
                    return jsonElement.GetString();
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.Number)
                {
                    if (jsonElement.TryGetInt16(out short shortValue)) return shortValue;
                    if (jsonElement.TryGetInt32(out int intValue)) return intValue;
                    if (jsonElement.TryGetInt64(out long longValue)) return longValue;
                    if (jsonElement.TryGetDouble(out double doubleValue)) return doubleValue;
                    return jsonElement.GetRawText(); // fallback
                }
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.True)
                    return true;
                else if (jsonElement.ValueKind == System.Text.Json.JsonValueKind.False)
                    return false;
                else
                    throw new System.Text.Json.JsonException(
                        $"Il tipo JsonElement {jsonElement.ValueKind} non è supportato ({xvalue?.ToString() ?? ""})");
            }

            // Caso 2: Newtonsoft.Json.Linq.JValue
            if (xvalue is Newtonsoft.Json.Linq.JValue jvalue)
            {
                if (jvalue.Type == Newtonsoft.Json.Linq.JTokenType.Null || jvalue.Type == Newtonsoft.Json.Linq.JTokenType.Undefined)
                    return null;
                else if (jvalue.Type == Newtonsoft.Json.Linq.JTokenType.String)
                    return (string)jvalue.Value;
                else if (jvalue.Type == Newtonsoft.Json.Linq.JTokenType.Integer)
                    return (long)jvalue.Value; // attenzione: Newtonsoft usa long come default
                else if (jvalue.Type == Newtonsoft.Json.Linq.JTokenType.Float)
                    return (double)jvalue.Value;
                else if (jvalue.Type == Newtonsoft.Json.Linq.JTokenType.Boolean)
                    return (bool)jvalue.Value;
                else
                    throw new Newtonsoft.Json.JsonException(
                        $"Il tipo JValue {jvalue.Type} non è supportato ({jvalue?.ToString() ?? ""})");
            }

            // Caso 3: già primitivo o null => restituisci com'è
            return xvalue;
        }



        public static bool IsNullOrEmptyObject(object? icode)
        {
            icode = UtilHelper.DecodeJsonElement(icode); //Decodifica tipi JSON, se le variabili vengono da pagina web

            if (icode == DBNull.Value) return true; // Il dato è DBNull
            else if (icode == null) return true; // Il dato è null
            else if (icode is string str && string.IsNullOrWhiteSpace(str)) return true; // Il dato è una stringa vuota
            else return false; // Il dato non è né DBNull, né null, né una stringa vuota
        }
        public static object TrimEndObject(object icode)
        {
            icode = UtilHelper.DecodeJsonElement(icode); //Decodifica tipi JSON, se le variabili vengono da pagina web

            if (icode == null) return null; // Il dato è null
            else if (icode is string str) return str.TrimEnd(); // Il dato è una stringa 
            else return icode; // se non è stringa restituisco l'oggetto
        }


        //https://github.com/dotnet/efcore/issues/4675
        //convert: SqlDataReader to DbSet
        //Usage: var rows = await dbContext.TestObjects.FromReaderAsync(dbDataReader);
        //public static async Task<IReadOnlyList<T>> FromReaderAsync<T>(this DbSet<T> dbSet, DbDataReader reader) where T : class
        //{
        //    var valueBufferParameter = Expression.Parameter(typeof(ValueBuffer));
        //    var materializer = Expression.Lambda<Func<ValueBuffer, T>>(
        //        dbSet.GetService<IEntityMaterializerSource>().CreateMaterializeExpression(
        //            dbSet.GetService<IModel>().FindEntityType(typeof(T)),
        //            valueBufferParameter),
        //        valueBufferParameter).Compile();

        //    var valueBufferFactory = dbSet.GetService<IRelationalValueBufferFactoryFactory>().Create(new[] { typeof(T) }, null);


        //    var r = new List<T>();

        //    while (await reader.ReadAsync())
        //        r.Add(materializer.Invoke(valueBufferFactory.Create(reader)));

        //    return r.AsReadOnly();
        //}


        //https://www.entityframeworktutorial.net/entityframework6/raw-sql-query-in-entity-framework.aspx
        //https://learn.microsoft.com/en-us/dotnet/api/system.data.linq.datacontext.executequery?view=netframework-4.8.1&redirectedfrom=MSDN#System_Data_Linq_DataContext_ExecuteQuery__1_System_String_System_Object___
        //https://mcuslu.medium.com/executing-raw-sql-queries-using-entity-framework-core-and-returns-to-generic-data-model-534356b1c2b3


        //#############################################################################

        //MODEL utility


        //1. Valida il modello usando gli attributi di data annotation([Required], [Range], ecc.).
        //2. Chiama la funzione TryValidateInt del modello(se disponibile).
        //3. Accetta un ModelStateDictionary esterno opzionale:
        //      - Se fornito, lo aggiorna con gli errori.
        //      - Se non fornito, restituisce una struttura con gli errori.
        public static bool ValidateModelState<T>(T model, ModelStateDictionary modelState, List<string> listNamePath, string? prefix = null ) where T : ModelErp
        {
            // verifica modello
            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(model, context, results, true);
            foreach (var result in results)
            {
                foreach (var member in result.MemberNames)
                {
                    //string key = string.IsNullOrEmpty(prefix) ? member : $"{prefix}.{member}";
                    //modelState.AddModelError(key, result.ErrorMessage);
                    if (listNamePath == null)
                    {
                        modelState.AddModelError(member, result.ErrorMessage);
                    }
                    else
                    {
                        foreach(var namePath in listNamePath) { modelState.AddModelError($"{namePath}{member}", result.ErrorMessage); }
                    }
                }

                // Se non ci sono membri specifici, è un errore generale
                if (!result.MemberNames.Any())
                {
                    modelState.AddModelError(string.Empty, result.ErrorMessage);
                }
            }

            //verifica vincoli interni del modello
            bool internalValid = model.TryValidateInt(modelState, prefix);
            if(!internalValid) isValid = false;

            //verifica action del modello
            if (model.action != 'A' && model.action != 'M' && model.action != 'D')
            {
                modelState.AddModelError(string.Empty, "L'azione impostata non è in [AMD].");  
                isValid = false;
            }

            // >>>>> sostituito con gestione LISTA_RECORD_AGGIORNATI 
            ////////aggiunge all'errore principale l'elenco degli errori dei singoli campi
            //////if (!isValid)
            //////{
            //////    modelState.AddModelError(string.Empty, $"{typeof(T).FullName}[{model?.action ?? ' '}:{model?.getIcode() ?? "(null)"}] Verifica valore dei campi: " +
            //////        string.Join(", ",
            //////            modelState.Where(ms => ms.Value.Errors.Any())
            //////                        .Select(kvp => kvp.Key)
            //////                        .ToArray()
            //////        )
            //////    );
            //////}
            // <<<<<

            //---SALVO INFORMAZIONE DI SERVIZIO DEI RECORD AGGIORNATI
            // questi record posso essere letti nella funzione ControllerErp.ValidationResult() e correlati ai campi con errore (eg: {namePath}{member})
            if (listNamePath != null)
            {
                foreach (var namePath in listNamePath) { modelState.AddModelError("LISTA_RECORD_AGGIORNATI", $"{namePath.Trim().Trim('.')} -- {typeof(T).FullName}[{model?.action ?? ' '}:{model?.getIcode() ?? "(null)"}]"); }
            }
            //---


            return isValid;
        }

        public static string ValidateModel<T>(T model, List<string> listNamePath, string? prefix = null) where T : ModelErp
        {
            var modelState = new ModelStateDictionary();
            bool isValid = ValidateModelState(model, modelState, listNamePath, prefix);
            return FormatModelStateErrors(modelState, typeof(T).Name);
        }
        private static string FormatModelStateErrors(ModelStateDictionary modelState, string? objectName = null)
        {
            if (modelState == null || !modelState.Any(ms => ms.Value.Errors.Any())) return "Nessun errore di validazione.";

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(objectName)) sb.AppendLine($"Errori di validazione per l'oggetto: **{objectName}**");
            else sb.AppendLine("Errori di validazione:");

            foreach (var entry in modelState.Where(ms => ms.Value.Errors.Any()))
            {
                string key = string.IsNullOrWhiteSpace(entry.Key) ? "(Generale)" : entry.Key;
                sb.AppendLine($"- Campo: {key}");

                foreach (var error in entry.Value.Errors)
                {
                    string message = string.IsNullOrWhiteSpace(error.ErrorMessage)
                        ? "(Errore senza messaggio)"
                        : error.ErrorMessage;

                    sb.AppendLine($"    • {message}");
                }
            }
            return sb.ToString();
        }



        // Recupera tutte le proprietà di una classe che hanno l'attributo ErpDogFieldAttribute, restituendo anche il nome del campo SQL e le opzioni specificate nell'attributo.
        //USO: foreach(var x in GetAllErpDogFields(typeof(MiaClasse))) { Console.WriteLine($"{x.Prop.Name}: SqlFieldName={x.SqlFieldName}, SqlFieldOptions={x.SqlFieldOptions}"); }
        public static IEnumerable<(PropertyInfo Prop, string SqlFieldName, string SqlFieldOptions)> GetAllErpDogFields(Type t, BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        {
            foreach (var p in t.GetProperties(flags))
            {
                var a = p.GetCustomAttribute<ErpDogFieldAttribute>(inherit: true);
                if (a != null)
                    yield return (p, a.SqlFieldName?.Trim() ?? "", a.SqlFieldOptions?.Trim() ?? "");
            }
        }



        //#############################################################################

        //GENERIC utility

        //-----------------------------------------------------------
        //--------- Convert HEX -------------------------------------

        //Converte byte[] in stringa esadecimale
        public static string ByteArrayToHexString(byte[]? bytes)
        {
            if (bytes == null) return "";
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        //Converte stringa esadecimale in byte[]
        public static byte[]? HexStringToByteArray(string? hex)
        {
            if (string.IsNullOrWhiteSpace(hex)) return null;
            if (hex.StartsWith("0x")) hex = hex.Substring(2);
            if (string.IsNullOrWhiteSpace(hex)) return null;

            if (hex.Length % 2 != 0)
                throw new ArgumentException("La stringa esadecimale deve avere lunghezza pari.");

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        //--------------------------------------------------------------
        //--------- Convert Base64 -------------------------------------

        private static readonly Encoding Utf8 = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

        // ========================
        // String ⇄ Base64 (Standard)
        // ========================

        /// <summary>
        /// Converte una stringa in Base64 (UTF-8).
        /// </summary>
        public static string ToBase64(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;
            var bytes = Utf8.GetBytes(plainText);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converte Base64 (standard) in stringa UTF-8.
        /// </summary>
        public static string FromBase64(string? base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return string.Empty;
            var bytes = Convert.FromBase64String(base64);
            return Utf8.GetString(bytes);
        }

        /// <summary>
        /// Versione Try che intercetta errori e restituisce false se non è valido Base64.
        /// </summary>
        public static bool TryFromBase64(string? base64, out string result)
        {
            result = string.Empty;
            if (string.IsNullOrWhiteSpace(base64)) return true;

            try
            {
                var bytes = Convert.FromBase64String(base64);
                result = Utf8.GetString(bytes);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Controllo rapido se la stringa sembra Base64 valida (standard).
        /// Nota: evita falsi positivi provando una decodifica.
        /// </summary>
        public static bool LooksLikeBase64(string? base64)
        {
            if (string.IsNullOrWhiteSpace(base64)) return false;

            // Lunghezza multipla di 4 non è sufficiente, ma aiuta
            if (base64!.Length % 4 != 0) return false;

            return true;
        }

        // ========================
        // String ⇄ Base64 URL-safe
        // ========================

        /// <summary>
        /// Converte una stringa in Base64 URL-safe (RFC 4648).
        /// Sostituisce '+'→'-', '/'→'_', rimuove '=' padding.
        /// </summary>
        public static string ToBase64Url(string? plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return string.Empty;

            var bytes = Utf8.GetBytes(plainText);

#if NET8_0_OR_GREATER
            var base64UrlLen = Base64.GetMaxEncodedToUtf8Length(bytes.Length);
            byte[] base64Bytes = new byte[base64UrlLen];
            Base64.EncodeToUtf8(bytes, base64Bytes, out _, out int written);
            var b64 = Encoding.ASCII.GetString(base64Bytes, 0, written);
#else
            var b64 = Convert.ToBase64String(bytes);
#endif
            // URL-safe cleanup
            return b64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        /// <summary>
        /// Converte Base64 URL-safe in stringa UTF-8.
        /// Ripristina padding e caratteri standard.
        /// </summary>
        public static string FromBase64Url(string? base64Url)
        {
            if (string.IsNullOrWhiteSpace(base64Url)) return string.Empty;

            string b64 = base64Url!.Replace('-', '+').Replace('_', '/');
            switch (b64.Length % 4)
            {
                case 2: b64 += "=="; break;
                case 3: b64 += "="; break;
                case 0: break;
                default:
                    throw new FormatException("Base64Url non valido: lunghezza non compatibile.");
            }

            var bytes = Convert.FromBase64String(b64);
            return Utf8.GetString(bytes);
        }

        public static bool TryFromBase64Url(string? base64Url, out string result)
        {
            result = string.Empty;
            if (string.IsNullOrWhiteSpace(base64Url)) return true;

            try
            {
                result = FromBase64Url(base64Url);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool LooksLikeBase64Url(string s)
        {
            // Ammessi: A-Z a-z 0-9 - _
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                bool ok =
                    (c >= 'A' && c <= 'Z') ||
                    (c >= 'a' && c <= 'z') ||
                    (c >= '0' && c <= '9') ||
                    c == '-' || c == '_';
                if (!ok) return false;
            }
            return true;
        }



        //--------------------------------------------------------------
        //--------- Detect Mime From Bin -------------------------------------

        public static string DetectMime(byte[] data)
        {
            if (data == null || data.Length < 4)
                return "application/octet-stream";

            // Cerca il marker reale saltando eventuali prefissi sporchi
            // (BOM UTF-8, BOM UTF-16, whitespace, CRLF, metadata prepended)
            int offset = FindCleanOffset(data);

            // ---------- PDF ----------
            if (StartsWith(data, offset, "%PDF"))
                return "application/pdf";

            // ---------- IMMAGINI ----------
            // JPEG, PNG, GIF, BMP, WebP: magic bytes binari → non hanno prefissi testuali
            // → si controllano SEMPRE a offset 0 (un JPEG non può avere BOM davanti)
            if (StartsWith(data, 0, new byte[] { 0xFF, 0xD8, 0xFF }))
                return "image/jpeg";
            if (StartsWith(data, 0, new byte[] { 0x89, 0x50, 0x4E, 0x47 }))
                return "image/png";
            if (StartsWith(data, 0, "GIF8"))
                return "image/gif";
            if (StartsWith(data, 0, "BM"))
                return "image/bmp";
            if (IsWebP(data))
                return "image/webp";

            // ---------- AUDIO ----------
            if (StartsWith(data, 0, "ID3") || (data[0] == 0xFF && (data[1] & 0xE0) == 0xE0))
                return "audio/mpeg";
            if (IsRiff(data, "WAVE"))
                return "audio/wav";
            if (StartsWith(data, 0, "OggS"))
                return "audio/ogg";

            // ---------- VIDEO ----------
            if (IsMp4(data))
                return "video/mp4";
            if (StartsWith(data, 0, new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }))
                return "video/x-matroska";

            // ---------- TESTO ----------
            if (LooksLikeText(data))
                return "text/plain";

            return "application/octet-stream";
        }

        /// <summary>
        /// Ritorna l'offset del primo byte "utile", saltando:
        ///   - BOM UTF-8  (EF BB BF)         → es: ï»¿%PDF  come nello screenshot
        ///   - BOM UTF-16 LE/BE (FF FE / FE FF)
        ///   - BOM UTF-32 LE/BE
        ///   - Whitespace ASCII iniziale (spazi, tab, CR, LF)
        /// Scansione limitata a MAX_JUNK_PREFIX byte per sicurezza.
        /// </summary>
        private const int MAX_JUNK_PREFIX = 1024;

        private static int FindCleanOffset(byte[] data)
        {
            int i = 0;
            int limit = Math.Min(data.Length, MAX_JUNK_PREFIX);

            // BOM UTF-32 LE: FF FE 00 00  (va controllato prima di UTF-16 LE)
            if (data.Length >= 4 &&
                data[0] == 0xFF && data[1] == 0xFE && data[2] == 0x00 && data[3] == 0x00)
                i = 4;
            // BOM UTF-32 BE: 00 00 FE FF
            else if (data.Length >= 4 &&
                data[0] == 0x00 && data[1] == 0x00 && data[2] == 0xFE && data[3] == 0xFF)
                i = 4;
            // BOM UTF-8: EF BB BF
            else if (data.Length >= 3 &&
                data[0] == 0xEF && data[1] == 0xBB && data[2] == 0xBF)
                i = 3;
            // BOM UTF-16 LE: FF FE
            else if (data.Length >= 2 && data[0] == 0xFF && data[1] == 0xFE)
                i = 2;
            // BOM UTF-16 BE: FE FF
            else if (data.Length >= 2 && data[0] == 0xFE && data[1] == 0xFF)
                i = 2;

            // Salta eventuali whitespace residui dopo il BOM
            while (i < limit && IsAsciiWhitespace(data[i]))
                i++;

            return i;
        }

        private static bool IsAsciiWhitespace(byte b)
            => b == 0x20 || b == 0x09 || b == 0x0A || b == 0x0D;  // spazio, tab, LF, CR

        // HELPER aggiornati con parametro offset
        private static bool StartsWith(byte[] data, int offset, string s)
        {
            if (data.Length < offset + s.Length) return false;
            return Encoding.ASCII.GetString(data, offset, s.Length) == s;
        }
        private static bool StartsWith(byte[] data, int offset, byte[] signature)
        {
            if (data.Length < offset + signature.Length) return false;
            for (int i = 0; i < signature.Length; i++)
                if (data[offset + i] != signature[i]) return false;
            return true;
        }

        // Overload backward-compatible (offset=0) per non rompere chiamate esistenti
        private static bool StartsWith(byte[] data, string s)
            => StartsWith(data, 0, s);
        private static bool StartsWith(byte[] data, byte[] signature)
            => StartsWith(data, 0, signature);

        private static bool IsRiff(byte[] data, string type)
            => StartsWith(data, 0, "RIFF") && data.Length >= 12 &&
               Encoding.ASCII.GetString(data, 8, type.Length) == type;

        private static bool IsMp4(byte[] data)
            => data.Length > 12 && Encoding.ASCII.GetString(data, 4, 4) == "ftyp";

        private static bool IsWebP(byte[] data) => IsRiff(data, "WEBP");

        private static bool LooksLikeText(byte[] data)
        {
            int sample = Math.Min(data.Length, 128);
            for (int i = 0; i < sample; i++)
            {
                byte b = data[i];
                if (b == 0) return false;
                if (b < 0x09) return false;
            }
            return true;
        }






    }
}
