using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MongoDB.Driver;
using System.Data.Entity.Infrastructure;
using System.Text;
using System.Transactions;
using static ErpToolkit.Helpers.Db.DogFactory;

namespace ErpToolkit.Controllers
{
    /// <summary>
    /// A <see cref="FeatureController"/> that implements API and RPC methods for the connection manager.
    /// </summary>
    public class ControllerErp : Controller
    {

        protected static NLog.ILogger _logger; //private readonly ILogger<HomeController> _logger;

        static ControllerErp()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
            //_logger.Info($"- InitController[{ErpContext.Session(base.HttpContext)?.SessionId ?? ""}/{ErpContext.Session(base.HttpContext)?.UserId ?? ""}/{ErpContext.Session(base.HttpContext)?.UnitId ?? ""}/{ErpContext.Session(base.HttpContext)?.ListMenu.Count ?? -1}]: ...");
        }
        //public class ControllerErpLog { public ControllerErpLog(HttpContext httpContext) { _logger.Info($"- InitController[{this.ErpContextSession?.SessionId ?? ""}/{ErpContext.Session(httpContext)?.UserId ?? ""}/{ErpContext.Session(httpContext)?.UnitId ?? ""}/{ErpContext.Session(httpContext)?.ListMenu.Count ?? -1}]: ..."); } }
        //private ControllerErpLog fittizio_per_log = new ControllerErpLog(ControllerBase.HttpContext);

        //==========================================================================================================
        //==========================================================================================================

        // VARIABILI GLOBALI
        //------------------

        private const bool VISUALIZZA_LISTA_CAMPI_ERRORE = true;   //se false non visualizza la lista dei campi errati nel messaggio di errore globale ma solo nei campi specifici


        //$$//public const string DbConnectionString = "#connectionString_SQLSLocal";
        //$$//public readonly DogId dogId = new DogId("SIO", "SqlServer", "#connectionString_SQLSLocal");
        public readonly DogId dogId = new DogId(ErpContext.Instance.GetString("#defaultServerDOG"), ErpContext.Instance.GetString("#defaultDbRoot"));  //connectionStringFull_NameTypeModel syntax: connectionStringAMM__SqlServer__SIO eg: #connectionStringAMM__SqlServer__SIO 


        // VARIABILI DI SESSIONE
        //----------------------

        public ErpContext? ErpContextSession { get { return ErpContext.Session(HttpContext); } }   //questa variabile consente al Controller un accesso diretto alle variabili di sessione (restituisce null se non è stato fatto Login o la sessione è scaduta)


        //==========================================================================================================
        //==========================================================================================================

        // VALIDAZIONE MODELLO
        //---------------
        
        //Restituisce JSON di STATO del Modell con gestione Toasts e FieldErrors 
        protected JsonResult ValidationResult(string? successMessage = null)
        {
            string[] InternalErrorList = { "LISTA_RECORD_AGGIORNATI" };

            if (ModelState.IsValid)
            {
                return Json(new AjaxValidationResult
                {
                    success = true,
                    message = successMessage
                });
            }

            //estrae lista errori della maschera
            var fieldErrors = ModelState
                .Where(x => !string.IsNullOrEmpty(x.Key))
                .Where(x => !InternalErrorList.Contains<string>(x.Key))
                .Where(x => (x.Value?.Errors?.Any() ?? false) && (!string.IsNullOrWhiteSpace(x.Value?.Errors?.First().ErrorMessage)))
                .Select(x => new AjaxFieldError
                {
                    field = x.Key,
                    message = x.Value?.Errors.First().ErrorMessage ?? ""
                });

            //aggiunge lista errori della maschera
            if (VISUALIZZA_LISTA_CAMPI_ERRORE) { addModelErrorList(fieldErrors); }

            //rimuovo errori interni 
            foreach (string ierr in InternalErrorList) { ModelState.Remove(ierr); }  // rimuovo questi campi virtuale dalla lista degli errori

            // Messaggio globale (chiave vuota)
            //var errorMessage = ModelState.TryGetValue("", out var globalErrors)
            //    ? globalErrors.Errors.FirstOrDefault()?.ErrorMessage
            //    : "Generic Error Found";
            var errorMessage = ModelState.TryGetValue("", out var globalErrors)
                ? ((globalErrors?.Errors != null) ? string.Join("\n", globalErrors.Errors.Select(e => e.ErrorMessage)) : "Empty Error Found")
                : "Generic Error Found";

            return Json(new AjaxValidationResult
            {
                success = false,
                message = errorMessage,
                fieldErrors = fieldErrors.Any() ? fieldErrors : null
            });
        }
        private void addModelErrorList(IEnumerable<AjaxFieldError> fieldErrors)
        {
            //------------------------------------------------------------------------
            // ----LISTA_RECORD_AGGIORNATI--------------------------------------------
            //------------------------------------------------------------------------
            // aggiunge all'errore principale l'elenco degli errori dei singoli campi
            // ricongiungendo i campi con le informazioni salvate in LISTA_RECORD_AGGIORNATI

            // 1. Parse lista: basePath -> descrizioneCampo
            Dictionary<string, string>? lista_record_aggiornati = null;
            if (ModelState.TryGetValue("LISTA_RECORD_AGGIORNATI", out var listErrors) &&
                listErrors?.Errors != null)
            {
                lista_record_aggiornati = listErrors.Errors
                    .Select(err =>
                    {
                        var parts = err.ErrorMessage.Split("--");
                        return new
                        {
                            BasePath = parts[0].Trim(),
                            Descrizione = parts.Length > 1 ? parts[1].Trim() : ""
                        };
                    })
                    .ToDictionary(
                        k => k.BasePath,
                        v => v.Descrizione
                    );
            }

            // 2. Raggruppo gli errori per basePath
            var erroriPerPath = fieldErrors
                .GroupBy(e =>
                {
                    var key = e.field;
                    int idx = key.LastIndexOf('.');
                    return idx > 0 ? key.Substring(0, idx) : key; // base path
                })
                .ToDictionary(
                    g => (lista_record_aggiornati != null &&
                          lista_record_aggiornati.ContainsKey(g.Key))
                            ? $"{lista_record_aggiornati[g.Key]} -- {g.Key}"
                            : $"XXX-{g.Key}",  // sostituisce "X" con una chiave UNIVOCA -- scarto se non è in lista_record_aggiornati (perchè non ho effettuato l'aggiornamento di quel record)
                    g => g.Select(e =>
                    {
                        var key = e.field;
                        int idx = key.LastIndexOf('.');
                        return idx > 0 ? key[(idx + 1)..] : key;
                    }).ToList()
                );

            // 3. Genero output
            bool foundError = false;
            StringBuilder sb = new("Verifica il valore dei campi.\n");
            foreach (var kv in erroriPerPath) { if (!kv.Key.StartsWith("XXX-")) { sb.AppendLine($"    {kv.Key}: {string.Join(", ", kv.Value)}.\n"); foundError = true; } }
            if (foundError) { ModelState.AddModelError(string.Empty, sb.ToString()); }
        }


        public class AjaxValidationResult
        {
            public bool success { get; set; }
            public string? message { get; set; }
            public IEnumerable<AjaxFieldError>? fieldErrors { get; set; }
        }
        public class AjaxFieldError
        {
            public string field { get; set; } = "";
            public string message { get; set; } = "";
        }


        //==========================================================================================================
        //==========================================================================================================

        //////////---------------
        ////////// GESTIONE MODELLO
        //////////---------------

        ////////public T ReadForEditModel<T>(ModelParam parms, string? prefix = null) where T : ModelErp
        ////////{
        ////////    ModelState.Clear(); //FORZA RICONVALIDA MODELLO 
        ////////    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        ////////    if (parms != null && !UtilHelper.IsNullOrEmptyObject(parms.Id))
        ////////    {
        ////////        try { objModel = ErpContext.Instance.DogFactory.GetDog(dogId).Row<T>(parms.Id, null, options: "[PLAIN]"); }  //[PLAIN] => non leggo le strutture relazionete
        ////////        catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, "Problemi in accesso al DB: Row: " + ex.Message); }
        ////////        objModel.action = 'M'; //update
        ////////    }
        ////////    else
        ////////    {
        ////////        objModel.action = 'A'; //add
        ////////    }
        ////////    return objModel;
        ////////}
        ////////public T SaveModel<T>(ModelObject dataObj, string? prefix = null) where T : ModelErp
        ////////{
        ////////    ModelState.Clear(); //FORZA RICONVALIDA MODELLO
        ////////    T objModel = (T)Activator.CreateInstance(typeof(T));
        ////////    // deserializza json
        ////////    try { objModel = ErpContext.Instance.DogFactory.GetDog(dogId).JsonSafeDeserialize<T>(dataObj, prefix: prefix); }
        ////////    catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, $"Oggetto {typeof(T).FullName} non deserializzato: " + ex.Message); return objModel; } //restiuisco oggetto vuoto 
        ////////    // verifica modello
        ////////    if (!TryValidateModel(objModel, prefix))
        ////////    {
        ////////        ModelState.AddModelError(prefix ?? string.Empty, "Verifica valore dei campi: " +
        ////////            string.Join(", ",
        ////////                ModelState.Where(ms => ms.Value.Errors.Any())
        ////////                            .Select(kvp => kvp.Key)
        ////////                            .ToArray()
        ////////            )
        ////////        );
        ////////        return objModel;
        ////////    }
        ////////    //verifica vincoli interni del modello
        ////////    if (!objModel.TryValidateInt(ModelState, prefix))
        ////////    {
        ////////        return objModel;
        ////////    }
        ////////    //verifica action del modello
        ////////    if (objModel.action != 'A' && objModel.action != 'M')
        ////////    {
        ////////        ModelState.AddModelError(prefix ?? string.Empty, "L'azione impostata non è in [AM]. E' necessario ricaricare l'oggetto");
        ////////        return objModel;
        ////////    }
        ////////    // salva su DB
        ////////    try { DogManager.DogResult objResult = ErpContext.Instance.DogFactory.GetDog(dogId).Mnt<T>(objModel, null); }
        ////////    catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, $"Problemi in accesso al DB: Mnt[{objModel.action}]: " + ex.Message + " " + (ex.InnerException?.Message ?? "")); return objModel; }
        ////////    //non ci sono errori
        ////////    return objModel;
        ////////}

        ////////public T ReadForDeleteModel<T>(ModelParam parms, string? prefix = null) where T : ModelErp
        ////////{
        ////////    ModelState.Clear(); //FORZA RICONVALIDA MODELLO 
        ////////    T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
        ////////    if (parms != null && !UtilHelper.IsNullOrEmptyObject(parms.Id))
        ////////    {
        ////////        try { objModel = ErpContext.Instance.DogFactory.GetDog(dogId).Row<T>(parms.Id, null, options: "[PLAIN]"); } //[PLAIN] => non leggo le strutture relazionete
        ////////        catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, "Problemi in accesso al DB: Row: " + ex.Message); }
        ////////        objModel.action = 'D'; //update
        ////////    }
        ////////    else
        ////////    {
        ////////        ModelState.AddModelError(prefix ?? string.Empty, "Identificativo nullo. E' necessario ricaricare l'oggetto");
        ////////    }
        ////////    return objModel;
        ////////}
        ////////public T DeleteModel<T>(ModelObject dataObj, string? prefix = null) where T : ModelErp
        ////////{
        ////////    ModelState.Clear(); //FORZA RICONVALIDA MODELLO 
        ////////    T objModel = ErpContext.Instance.DogFactory.GetDog(dogId).JsonSafeDeserialize<T>(dataObj, prefix: prefix);
        ////////    if (objModel.action != 'D')
        ////////    {
        ////////        ModelState.AddModelError(prefix ?? string.Empty, "L'azione impostata non è [D]. E' necessario ricaricare l'oggetto");
        ////////        return objModel;
        ////////    }
        ////////    // cancella
        ////////    try { DogManager.DogResult objResult = ErpContext.Instance.DogFactory.GetDog(dogId).Mnt<T>(objModel, null); }
        ////////    catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, $"Problemi in accesso al DB: Mnt[{objModel.action}]: " + ex.Message); return objModel; }
        ////////    //non ci sono errori
        ////////    return objModel;
        ////////}


        //==========================================================================================================
        //==========================================================================================================

        //---------------
        // GESTIONE ICODE
        //---------------


        [HttpGet]
        public JsonResult GenerateIcode()
        {
            try
            {
                string icode = ErpContext.Instance.DogFactory.GetDog(dogId).GenerateIcode();
                return Json(new { icode });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in GenerateIcode: " + ex.Message }); }
        }


        //==========================================================================================================
        //==========================================================================================================


        //-----------------------
        // GESTIONE MODELLO
        //-----------------------

        public T ReadForEditModel<T>(ModelParam parms, string? prefix = null, string? transactionId = null) where T : ModelErp
        {
            DogManager.DogCache dogCache = new DogManager.DogCache(); List<string> xrefFrom = null;
            if (parms != null && !UtilHelper.IsNullOrEmptyObject(parms.Id)) return ReadForEditModel<T>(parms, xrefFrom, ref dogCache, prefix: prefix, action: 'M');  
            else return ReadForEditModel<T>(parms, xrefFrom, ref dogCache, prefix: prefix, action: 'A', transactionId: transactionId);  
        }
        public T SaveModel<T>(ModelObject dataObj, string? prefix = null, string? transactionId = null) where T : ModelErp
        {
            DogManager.DogCache dogCache = new DogManager.DogCache(); 
            return SaveModel<T>(dataObj, ref dogCache, prefix: prefix, options: "[MAX_ONE_OBJ]", transactionId: transactionId); 
        }
        public T ReadForDeleteModel<T>(ModelParam parms, string? prefix = null, string? transactionId = null) where T : ModelErp
        {
            DogManager.DogCache dogCache = new DogManager.DogCache(); List<string> xrefFrom = null;
            return ReadForEditModel<T>(parms, xrefFrom, ref dogCache, prefix: prefix, action: 'D', transactionId: transactionId);  
        }
        public T DeleteModel<T>(ModelObject dataObj, string? prefix = null, string? transactionId = null) where T : ModelErp
        {
            DogManager.DogCache dogCache = new DogManager.DogCache();
            return SaveModel<T>(dataObj, ref dogCache, prefix: prefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]", transactionId: transactionId);  
        }

        //-----------------------
        // GESTIONE MODELLO CACHE
        //-----------------------

        public T ReadForEditModel<T>(ModelParam parms, List<string> xrefFrom, ref DogManager.DogCache dogCache, string? prefix = null, char action = 'X', string? transactionId = null) where T : ModelErp
        {
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO 
            T objModel = (T)Activator.CreateInstance(typeof(T)); // create an instance of that type
            if (parms != null && !UtilHelper.IsNullOrEmptyObject(parms.Id))
            {
                if (!"XMD".Contains(action)) { ModelState.AddModelError(prefix ?? string.Empty, "Action [{action}] errata"); return null; ; }
                try
                {
                    //objModel = ErpContext.Instance.DogFactory.GetDog(dogId).Row<T>(parms.Id, xrefFrom, ref dogCache, options: "[PLAIN] inserisco_primo_record_vuoto_per_fare_add_su_tabella_in_grafica_cshtml");   //[PLAIN] => non leggo le strutture relazionate
                    objModel = ErpContext.Instance.DogFactory.GetDog(dogId).Row<T>(parms.Id, xrefFrom, ref dogCache, transactionId, options: "[PLAIN] ");   //[PLAIN] => non leggo le strutture relazionate
                }
                catch (Exception ex) { ModelState.AddModelError(prefix ?? string.Empty, "Problemi in accesso al DB: Row: " + ex.Message); }
                if (action == 'D') objModel.action = 'D'; //delete
                else objModel.action = 'M'; //update (default action)
            }
            else
            {
                if (!"XA".Contains(action)) { ModelState.AddModelError(prefix ?? string.Empty, "Action [{action}] errata"); return null; ; }
                objModel.action = 'A'; //add
            }

            return objModel;
        }

        public T SaveModel<T>(ModelObject dataObj, ref DogManager.DogCache dogCache, string? prefix = null, string options = "", string? transactionId = null, int maxRecords = -1) where T : ModelErp
        {
            string errMsg = ""; 
            string transactionName = $"{typeof(T).FullName}_XXX_{DateTime.Now.Ticks}"; bool isTransaction = false;

            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            T objModel = (T)Activator.CreateInstance(typeof(T));
            try
            {
                errMsg = "Impossibile accedere al DB";
                DogManager dogMng = ErpContext.Instance.DogFactory.GetDog(dogId);
                if (dogMng == null) throw new Exception("dogMng==null");

                //--------------------
                // deserializza json
                //--------------------
                errMsg = "Errore nella deserializzazione della struttura JSON restituita dalla form";
                objModel = dogMng.JsonSafeDeserialize<T>(dataObj, prefix: prefix);
                if (objModel == null) throw new Exception("objModel==null");

                //---------------------------------------------------------------------------------
                //estrae la lista degli oggetti da aggiornare
                // ie: devono contenere action[AMD] icode timestamp e deleted
                //---------------------------------------------------------------------------------
                errMsg = "Errore in estazione lista degli oggetti modificati nella form";
                Dictionary<ModelErp, List<string>> objList = dogMng.GetListObjToMnt(objModel, prefix); //List<ModelErp> objList = dogMng.GetListObjToMnt(objModel);
                if (objList == null) throw new Exception("objList==null");

                //---------------------------------------------------------------------------------
                //validate model
                //---------------------------------------------------------------------------------
                errMsg = "Verifica opzioni modello";
                if (options.Contains("[MAX_ONE_OBJ]") && objList.Count > 1) throw new Exception($"Troppi oggetti modificati {objList.Count} (max 1)");
                foreach (ModelErp obj in objList.Keys)
                {
                    if (options.Contains("[NO_ADD]") && obj.action == 'A') throw new Exception($"Azione {obj.action} non consentita");
                    if (options.Contains("[NO_UPDATE]") && obj.action == 'M') throw new Exception($"Azione {obj.action} non consentita");
                    if (options.Contains("[NO_DELETE]") && obj.action == 'D') throw new Exception($"Azione {obj.action} non consentita");
                }

                //---------------------------------------------------------------------------------
                //validate model
                //---------------------------------------------------------------------------------
                errMsg = "Errore nel processo di validazione degli oggetti modificati nella form";
                bool validate = true;
                foreach (ModelErp obj in objList.Keys)
                {
                    //recupera objOriginal dalla cache
                    //var objOriginal = dogCache.dbCache[obj.GetType()][obj.getIcode()];  // objOriginal = null se non trovato
                    var objOriginal = dogCache.GetObject(obj.GetType(), obj.getIcode());  // objOriginal = null se non trovato

                    // Merge valore originale objOriginal con valore attuale objModel (DataForm)
                    if (objOriginal != null)
                    {
                        var table = ErpContext.Instance.DogFactory.GetDog(dogId).getTable(obj.GetType());
                        foreach (var field in table.fields)
                        {
                            if (field.optSYS) continue; //non confronto i campi di sistema   //if (!field.canUpdate) continue;
                            var oldVal = field.GetValue(objOriginal);   //leggo il valore originale dalla cache 
                            var newVal = field.GetValue(obj);
                            if (newVal == null && oldVal != null) field.SetValue(obj, oldVal);  //se non è stato impostato lo prendo da objOriginal
                        }
                    }

                    // verifica modello & verifica vincoli interni del modello & verifica action del modello
                    if (!ErpToolkit.Helpers.UtilHelper.ValidateModelState<ModelErp>(obj, ModelState, objList[obj], prefix)) validate = false;
                }
                errMsg = "Le informazioni inserite nella form non sono valide e/o complete";
                if (!validate) throw new Exception("Verificare i seguenti campi obbligatori");


                //---------------------------------------------------------------------------------
                //diff model: aggiorno solo i campi modificati
                //---------------------------------------------------------------------------------
                errMsg = "Errore nel processo di selezione degli effettivi campi modificati";
                foreach (ModelErp obj in objList.Keys)
                {
                    //recupera objOriginal dalla cache
                    var objOriginal = dogCache.GetObject(obj.GetType(), obj.getIcode());  // objOriginal = null se non trovato

                    //calcola differenza rispetto all'originale per effettuare l'aggiornamento
                    if (objOriginal != null)
                    {
                        var table = ErpContext.Instance.DogFactory.GetDog(dogId).getTable(obj.GetType());
                        foreach (var field in table.fields)
                        {
                            var oldVal = field.GetValue(objOriginal);   //leggo sempre il valore originale dalla cache 
                            //---FORZO DELETED E TIMESTAMP LETTO DALLA CACHE---
                            if (field.optDEL) { field.SetValue(obj, oldVal); continue; }  //sovrascrivo sempre il campo deleted con il valore originale (non permetto di modificarlo da form)  
                            if (field.optTMS) { field.SetValue(obj, oldVal); continue; }  //sovrascrivo sempre il campo timestamp con il valore originale (non permetto di modificarlo da form)  
                            //-------------------------------------------------
                            if (field.optSYS) continue; //non confronto i campi di sistema   //if (!field.canUpdate) continue;
                            var newVal = field.GetValue(obj);
                            if (newVal == oldVal && newVal != null) field.SetValue(obj, null);  //se uguali non effettuo aggiornamento del campo su DB
                        }
                    }
                }

                //---------------------------------------------------------------------------------
                // Start Transaction 
                //---------------------------------------------------------------------------------
                transactionName = $"{typeof(T).FullName}_{objModel?.getIcode() ?? "XXX"}_{DateTime.Now.Ticks}";
                transactionId = dogMng.BeginTransaction(transactionId, transactionName);
                isTransaction = true;

                //---------------------------------------------------------------------------------
                // salva su DB e rilettura dei record salvati e aggiornamento delle modifiche nella cache 
                //---------------------------------------------------------------------------------
                errMsg = "Impossibile effettuare le modifiche su DB";
                List<DogManager.DogResult> objResults = dogMng.MntList(objList.Keys.ToList(), ref dogCache, transactionId, maxRecords, options: options);
                if (objResults == null) throw new Exception("objResults==null");

                //---------------------------------------------------------------------------------
                // Commit Transaction 
                //---------------------------------------------------------------------------------
                dogMng.CommitTransaction(transactionId, transactionName);
                isTransaction = false;

                //--------------------
                // estrae il record riletto nella cache
                //--------------------
                errMsg = "Impossibile estrarre dalla cache il record riletto";
                Dictionary<object, ModelErp> cacheDict = dogCache.dbCache[typeof(T)];
                ModelErp cacheObj = cacheDict[objModel.getIcode()];

                //--------------------
                // record truncate per passarlo alla pagina web
                //--------------------
                errMsg = "Errore nella ricomposizione dei dati da passare alla form";
                T truncateObjModel = (T)dogMng.TruncateCloneModelErp(cacheObj, DogManager.DOG_MAX_OBJ_DEPTH, action: 'R');
                if (truncateObjModel == null) throw new Exception("truncateObjModel==null");

                //--------------------
                // non ci sono errori
                //--------------------

                ModelState.Clear(); //PULISCO MODEL STATE PRIMA DI USCIRE
                return truncateObjModel;
            }
            catch (Exception ex) { 
                ModelState.AddModelError(string.Empty, $"ControllerErp.SaveModel: {typeof(T).FullName}: {errMsg}: {ex.Message}"); return objModel; //restiuisco oggetto vuoto
            }
            finally
            {
                //---------------------------------------------------------------------------------
                // RollBack Transaction 
                //---------------------------------------------------------------------------------
                if (isTransaction)
                {
                    try { ErpContext.Instance.DogFactory.GetDog(dogId).RollbackTransaction(transactionId, transactionName); } catch { }
                }
            }
        }


        //-----------------------------------------------


        //==========================================================================================================
        //==========================================================================================================

            // GESTIONE LOGIN
            //---------------

        public const string SessionReturnUrl = "_ReturnUrl";


        [BindProperty]
        public InputLogin Input2 { get; set; }


        //==========================================================================================================
        //==========================================================================================================

        // GESTIONE MENU'
        //---------------

        [BindProperty]
        public string AppName2 { get { return (ErpContext.Instance.AppName == "") ? "ErpToolkit" : ErpContext.Instance.AppName; } }   //nome dell'applicazione da visualizzare sul menù. 
        [BindProperty]
        public List<DogManager.MenuItem> ListMenu2 { get { return ErpContextSession?.ListMenu ?? new List<DogManager.MenuItem>(); } }   //questa variabile deve essere caricata da HomeController


        //==========================================================================================================
        //==========================================================================================================


        //[HttpGet]
        // //public IActionResult Index()
        // //{
        // //    return View();
        // //}
        // [HttpGet]
        // public async Task<IActionResult> Index(string returnUrl = null)
        // {
        //     if (returnUrl != null)
        //     {
        //         ModelState.AddModelError(string.Empty, "E' necessario effettuare la login per accedere alla pagina!");
        //         HttpContext.Session.SetString(SessionReturnUrl, returnUrl);
        //     }
        //     return View();
        // }

        //##############################################################################################
        //####
        //#### LOGIN
        //####

        //[HttpPost]
        //public async Task<IActionResult> Login(InputLogin Input)
        //{


        //    ModelState.Clear(); //ModelState.ClearValidationState("CompanyName"); //FORZA RICONVALIDA MODELLO >>> https://learn.microsoft.com/it-it/aspnet/core/mvc/models/validation?view=aspnetcore-8.0
        //    if (!TryValidateModel(Input))
        //    {
        //        // logout
        //        ErpContext.TermSessionAsync(HttpContext); //clean current session ErpContext and LOGOUT
        //        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated) return LocalRedirect(Url.Content("~/"));  // <<<-- ricarica la pagina di login dopo LOGOUT
        //        return View(); // <<<-- visualizza gli errori LOGIN
        //    }



        //    // login and new session
        //    bool login = await ErpContext.InitSessionAsync(HttpContext, Input.Matricola, Input.Password, "");
        //    if (!login)
        //    {
        //        //errore utente non abilitato
        //        ModelState.AddModelError(string.Empty, "Matricola o Password non valide!");
        //        return View(); // <<<-- visualizza gli errori
        //    }

        //    //redirect to ReturnUrl
        //    string? returnUrl = HttpContext.Session.GetString(SessionReturnUrl); HttpContext.Session.Remove(SessionReturnUrl); //scarico il ReturnUrl
        //    returnUrl ??= Url.Content("~/"); //default
        //    return LocalRedirect(returnUrl);
        //}


        //[HttpPost]
        //public async Task<IActionResult> Login(InputLogin Input)
        //{


        //    ModelState.Clear(); //ModelState.ClearValidationState("CompanyName"); //FORZA RICONVALIDA MODELLO >>> https://learn.microsoft.com/it-it/aspnet/core/mvc/models/validation?view=aspnetcore-8.0
        //    if (!TryValidateModel(Input))
        //    {
        //        // logout
        //        ErpContext.TermSessionAsync(HttpContext); //clean current session ErpContext and LOGOUT
        //        if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated) return LocalRedirect(Url.Content("~/"));  // <<<-- ricarica la pagina di login dopo LOGOUT
        //        return View(); // <<<-- visualizza gli errori LOGIN
        //    }



        //    // login and new session
        //    bool login = await ErpContext.InitSessionAsync(HttpContext, Input.Matricola, Input.Password, "");
        //    if (!login)
        //    {
        //        //errore utente non abilitato
        //        ModelState.AddModelError(string.Empty, "Matricola o Password non valide!");
        //        return View(); // <<<-- visualizza gli errori
        //    }

        //    //redirect to ReturnUrl
        //    string? returnUrl = HttpContext.Session.GetString(SessionReturnUrl); HttpContext.Session.Remove(SessionReturnUrl); //scarico il ReturnUrl
        //    returnUrl ??= Url.Content("~/"); //default
        //    return LocalRedirect(returnUrl);
        //}


        //==========================================================================================================
        //==========================================================================================================
        //
        //TRIGGER prima-dopo costruzione pagina HTML


        /// <summary>
        /// Called before the action executes, after model binding is complete.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutingContext"/>.</param>
        void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"- {nameof(ControllerErp)}.{nameof(OnActionExecuting)}");
            _logger.Info($"- {nameof(ControllerErp)}.{nameof(OnActionExecuting)}");

            base.OnActionExecuting(context);
        }

        /// <summary>
        /// Called after the action executes, before the action result.
        /// </summary>
        /// <param name="context">The <see cref="ActionExecutedContext"/>.</param>
        void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"- {nameof(ControllerErp)}.{nameof(OnActionExecuted)}");
            _logger.Info($"- {nameof(ControllerErp)}.{nameof(OnActionExecuted)}");

            base.OnActionExecuted(context);
        }


    }

    //La classe base ActionFilterAttribute include i metodi seguenti che è possibile eseguire l'override:
    //  ---
    //  OnActionExecuting: questo metodo viene chiamato prima dell'esecuzione di un'azione del controller.
    //  OnActionExecuted: questo metodo viene chiamato dopo l'esecuzione di un'azione del controller.
    //  OnResultExecuting: questo metodo viene chiamato prima dell'esecuzione di un risultato dell'azione del controller.
    //  OnResultExecuted: questo metodo viene chiamato dopo l'esecuzione di un risultato dell'azione del controller.
    //  ---
    //  https://learn.microsoft.com/it-it/aspnet/mvc/overview/older-versions-1/controllers-and-routing/understanding-action-filters-cs
    //  ---

    public class LogActionFilter : ActionFilterAttribute

    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.Log("OnActionExecuting", filterContext.RouteData);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            this.Log("OnActionExecuted", filterContext.RouteData);
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            this.Log("OnResultExecuting", filterContext.RouteData);
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            this.Log("OnResultExecuted", filterContext.RouteData);
        }


        private void Log(string methodName, RouteData routeData)
        {
            var controllerName = routeData.Values["controller"];
            var actionName = routeData.Values["action"];
            var message = String.Format("{0} controller:{1} action:{2}", methodName, controllerName, actionName);
            System.Diagnostics.Debug.WriteLine(message, "Action Filter Log");
        }

    }




}
