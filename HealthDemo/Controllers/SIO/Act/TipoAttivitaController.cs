using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpToolkit.Controllers;
using HealthDemo.Models.SIO.Act;

using static ErpToolkit.Helpers.Db.DogManager;
namespace HealthDemo.Controllers.SIO.Act
{
    public class TipoAttivitaController : ControllerErp
    {
        private static readonly List<string> xrefTables = new List<string> {
             "PrIdTipoAttivita"   //carico in cache tutti i dati di Prestazione collegati
            ,"AvIdTipoAttivita"   //carico in cache tutti i dati di Attivita collegati
            ,"TaIdGruppo"   //carico in cache tutti i dati di TipoAttivita collegati
        };
        private const string ErpContext_dogCache = "@HealthDemo.Controllers.SIO.Act.TipoAttivita_dogCache";
        private DogCache _dogCache = new DogCache();

        //private static NLog.ILogger _logger;
        public TipoAttivitaController()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();

            this._dogCache = (DogCache)ErpContext.Instance.GetObject(ErpContext_dogCache); // Alloca le risorse cache
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) { }  // Rilascia le risorse gestite
            ErpContext.Instance.Set(ErpContext_dogCache, this._dogCache);   // Rilascia le risorse non gestite (se presenti)
            base.Dispose(disposing); // Chiama il metodo Dispose della classe base
        }

        [HttpGet]
        public JsonResult AutocompleteGetAll()
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetAll<TipoAttivita>());
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetAll TipoAttivita: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string term)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetSelect<TipoAttivita>(term));
            }
            catch (Exception ex)  { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetSelect TipoAttivita: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad([FromBody] List<string> values)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompletePreLoad<TipoAttivita>(values));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompletePreLoad TipoAttivita: " + ex.Message }); }
        }
        [BindProperty]
        public SelTipoAttivita Select { get; set; }
        [BindProperty]
        public List<TipoAttivita> List { get; set; } = new List<TipoAttivita>();
        [BindProperty]
        public TipoAttivita Row { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            this._dogCache = new DogCache();    // Inizializza le risorse ...in caso di chiamata della pagina dall'esterno (ie: no reload)

            this.Select = new SelTipoAttivita();
            foreach (var key in Request.Query.Keys) DogManager.setPropertyValue(this.Select, key, Request.Query[key]); // carica parametri QueryString
            this.List = new List<TipoAttivita>();
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/Act/TipoAttivita/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SelTipoAttivita selobj)
        {
            if (selobj != null) { this.Select = selobj; }
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            if (!TryValidateModel(this.Select))
            {
                ModelState.AddModelError(string.Empty, "Verifica valore dei campi.");
                return View("~/Views/SIO/Act/TipoAttivita/Index.cshtml", this);
            }
            if (!this.Select.TryValidateInt(ModelState)) {
                return View("~/Views/SIO/Act/TipoAttivita/Index.cshtml", this);
            }
            //carica lista
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<TipoAttivita>(this.Select, null, ref this._dogCache, ""); }  // non carico tabelle relazionate per la lista di selezione
            catch (Exception ex) { ModelState.AddModelError(string.Empty, "Problemi in accesso al DB: List: " + ex.Message); }
            this.StatusMessage = "Lista caricata!";
            return View("~/Views/SIO/Act/TipoAttivita/Index.cshtml", this);
        }

        [HttpPost]
        public IActionResult ReadForEdit([FromBody] ModelParam parms)  
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoAttivita obj = this.ReadForEditModel<TipoAttivita>(parms, xrefTables, ref this._dogCache, modelPrefix);
            return PartialView("~/Views/SIO/Act/TipoAttivita/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Save([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoAttivita obj = this.SaveModel<TipoAttivita>(dataObj, ref this._dogCache, prefix: modelPrefix);
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record aggiornato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Act/TipoAttivita/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult ReadForDelete([FromBody] ModelParam parms)  
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoAttivita obj = this.ReadForEditModel<TipoAttivita>(parms, null, ref this._dogCache, modelPrefix, action: 'D');    // non carico tabelle relazionate per il delete
            return PartialView("~/Views/SIO/Act/TipoAttivita/_PartialDelete.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Delete([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoAttivita obj = this.SaveModel<TipoAttivita>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record cancellato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Act/TipoAttivita/_PartialDelete.cshtml", obj);
        }
    }
}
