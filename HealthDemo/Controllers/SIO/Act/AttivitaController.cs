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
    public class AttivitaController : ControllerErp
    {
        private static readonly List<string> xrefTables = new List<string> {
             "PrIdAttivitaRichiesta"   //carico in cache tutti i dati di Prestazione collegati
            ,"PrIdAttivitaEseguita"   //carico in cache tutti i dati di Prestazione collegati
            ,"AvIdGruppo"   //carico in cache tutti i dati di Attivita collegati
            ,"AcIdAttivita"   //carico in cache tutti i dati di RelAttivitaTipoCampione collegati
            ,"ArIdAttivita"   //carico in cache tutti i dati di RelAttivitaRichiestaDa collegati
            ,"AuIdAttivita"   //carico in cache tutti i dati di RelAttivitaUsa collegati
            ,"AeIdAttivita"   //carico in cache tutti i dati di RelAttivitaErogataDa collegati
            ,"AaIdAttivitaPadre"   //carico in cache tutti i dati di RelAttivitaContiene collegati
            ,"AaIdAttivitaFiglio"   //carico in cache tutti i dati di RelAttivitaContiene collegati
        };
        private const string ErpContext_dogCache = "@HealthDemo.Controllers.SIO.Act.Attivita_dogCache";
        private DogCache _dogCache = new DogCache();

        //private static NLog.ILogger _logger;
        public AttivitaController()
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
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetAll<Attivita>());
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetAll Attivita: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string term)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetSelect<Attivita>(term));
            }
            catch (Exception ex)  { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetSelect Attivita: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad([FromBody] List<string> values)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompletePreLoad<Attivita>(values));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompletePreLoad Attivita: " + ex.Message }); }
        }
        [Authorize]
        [HttpGet]
        public IActionResult ViewBlob(string icode)
        {
            return base.ViewBlobModel<Attivita>(icode);
        }

        [BindProperty]
        public SelAttivita Select { get; set; }
        [BindProperty]
        public List<Attivita> List { get; set; } = new List<Attivita>();
        [BindProperty]
        public Attivita Row { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            this._dogCache = new DogCache();    // Inizializza le risorse ...in caso di chiamata della pagina dall'esterno (ie: no reload)

            this.Select = new SelAttivita();
            foreach (var key in Request.Query.Keys) DogManager.setPropertyValue(this.Select, key, Request.Query[key]); // carica parametri QueryString
            this.List = new List<Attivita>();
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SelAttivita selobj)
        {
            if (selobj != null) { this.Select = selobj; }
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            if (!TryValidateModel(this.Select))
            {
                ModelState.AddModelError(string.Empty, "Verifica valore dei campi.");
                return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);
            }
            if (!this.Select.TryValidateInt(ModelState)) {
                return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);
            }
            //carica lista
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<Attivita>(this.Select, xrefTables, ref this._dogCache, null, -1); }  
            catch (Exception ex) { ModelState.AddModelError(string.Empty, "Problemi in accesso al DB: List: " + ex.Message); }
            this.StatusMessage = "Lista caricata!";
            return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);
        }

        [HttpPost]
        public IActionResult ReadForEdit([FromBody] ModelParam parms)  
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Attivita obj = this.ReadForEditModel<Attivita>(parms, xrefTables, ref this._dogCache, prefix: modelPrefix);
            return PartialView("~/Views/SIO/Act/Attivita/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Save([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Attivita obj = this.SaveModel<Attivita>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "*allowTouch*");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record aggiornato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Act/Attivita/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult ReadForDelete([FromBody] ModelParam parms)  
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Attivita obj = this.ReadForEditModel<Attivita>(parms, null, ref this._dogCache, prefix: modelPrefix, action: 'D');    // non carico tabelle relazionate per il delete
            return PartialView("~/Views/SIO/Act/Attivita/_PartialDelete.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Delete([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Attivita obj = this.SaveModel<Attivita>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record cancellato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Act/Attivita/_PartialDelete.cshtml", obj);
        }
    }
}
