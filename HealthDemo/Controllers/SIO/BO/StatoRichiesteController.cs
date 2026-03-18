using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpToolkit.Controllers;
using HealthDemo.Models.SIO.Act;
using static ErpToolkit.Helpers.Db.DogManager;

namespace HealthDemo.Controllers.SIO.BO
{
    public class StatoRichiesteController : ControllerErp
    {
        private static readonly List<string> xrefPrestazione = new List<string> {
             "PdIdPrestazione"   //carico in cache tutti i dati clinici collegati alle prestazioni
            ,"PcIdPrestazione"   //carico in cache tutti i campioni collegati alle prestazioni
            ,"PuIdPrestazione"   //carico in cache tutte le risorse usate dalle prestazioni
        };
        private const string ErpContext_dogCache = "@HealthDemo.Controllers.SIO.BO.StatoRichieste_dogCache";
        private DogCache _dogCache = new DogCache();


        //private static NLog.ILogger _logger;
        public StatoRichiesteController()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();

            // Alloca le risorse
            this._dogCache = (DogCache)ErpContext.Instance.GetObject(ErpContext_dogCache);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Rilascia le risorse gestite
                //...
            }
            // Rilascia le risorse non gestite (se presenti)
            ErpContext.Instance.Set(ErpContext_dogCache, this._dogCache);

            base.Dispose(disposing); // Chiama il metodo Dispose della classe base
        }
        //------------------------------------------------------------------



        [BindProperty]
        public SelPrestazione Select { get; set; }
        [BindProperty]
        public List<Prestazione> List { get; set; } = new List<Prestazione>();
        [BindProperty]
        public Prestazione Row { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            // Inizializza le risorse ...in caso di chiamata della pagina dall'esterno (ie: no reload)
            this._dogCache = new DogCache();
            //---

            this.Select = new SelPrestazione();
            foreach (var key in Request.Query.Keys) DogManager.setPropertyValue(this.Select, key, Request.Query[key]); // carica parametri QueryString
            this.List = new List<Prestazione>();
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/BO/StatoRichieste/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SelPrestazione selobj)
        {
            if (selobj != null) { this.Select = selobj; }
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            if (!TryValidateModel(this.Select))
            {
                ModelState.AddModelError(string.Empty, "Verifica valore dei campi.");
                return View("~/Views/SIO/BO/StatoRichieste/Index.cshtml", this);
            }
            if (!this.Select.TryValidateInt(ModelState)) {
                return View("~/Views/SIO/BO/StatoRichieste/Index.cshtml", this);
            }
            //carica lista
            //try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<Prestazione>(this.Select); }
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<Prestazione>(this.Select, xrefPrestazione, ref this._dogCache, null, -1); }
            catch (Exception ex) { ModelState.AddModelError(string.Empty, "Problemi in accesso al DB: List: " + ex.Message); }
            this.StatusMessage = "Lista caricata!";
            return View("~/Views/SIO/BO/StatoRichieste/Index.cshtml", this);
        }

        [HttpPost]
        public IActionResult ReadForEdit([FromBody] ModelParam parms)  
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Prestazione obj = this.ReadForEditModel<Prestazione>(parms, xrefPrestazione, ref this._dogCache, prefix: modelPrefix);
            return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Save([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Prestazione obj = this.SaveModel<Prestazione>(dataObj, ref this._dogCache, prefix: modelPrefix);
            if (!ModelState.IsValid)    //if (!TryValidateModel(obj, modelPrefix))
            {
                //xx//return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
                return this.ValidationResult();
            }
            this.StatusMessage = "Record aggiornato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(Prestazione dataObj)  // Perchè chiamo con updateModalWithContentForm //modificato per evitare problemi con [FromBody] e ValidateAntiForgeryToken 
        //{
        //    string modelPrefix = "EDIT";
        //    ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
        //    Prestazione obj = this.SaveModel<Prestazione>(dataObj, ref this._dogCache, modelPrefix);
        //    if (!TryValidateModel(obj, modelPrefix))
        //    {
        //        return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        //    }
        //    this.StatusMessage = "Record aggiornato!";
        //    //---GESTISCE AZIONI CLICK PULSANTE
        //    ViewData["IsModalACTION"] = "CLOSE";
        //    ViewData["IsPageACTION"] = "RELOAD";
        //    ViewData["IsPageREDIRECT"] = "";
        //    //---
        //    return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save([Bind(Prefix = "EDIT")]Prestazione dataObj)  // Perchè chiamo con updateModalWithContentForm //modificato per evitare problemi con [FromBody] e ValidateAntiForgeryToken 
        //{
        //    string modelPrefix = "EDIT";
        //    ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
        //    Prestazione obj = this.SaveModel<Prestazione>(dataObj, ref this._dogCache);
        //    if (!TryValidateModel(obj, modelPrefix))
        //    {
        //        return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        //    }
        //    this.StatusMessage = "Record aggiornato!";
        //    //---GESTISCE AZIONI CLICK PULSANTE
        //    ViewData["IsModalACTION"] = "CLOSE";
        //    ViewData["IsPageACTION"] = "RELOAD";
        //    ViewData["IsPageREDIRECT"] = "";
        //    //---
        //    return PartialView("~/Views/SIO/BO/StatoRichieste/_PartialEdit.cshtml", obj);
        //}

        [HttpPost]
        public IActionResult ReadForDelete([FromBody] ModelParam parms)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Prestazione obj = this.ReadForEditModel<Prestazione>(parms, null, ref this._dogCache, prefix: modelPrefix, action: 'D');    // non carico tabelle relazionate per il delete
            return PartialView("~/Views/SIO/Act/Prestazione/_PartialDelete.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Delete([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Prestazione obj = this.SaveModel<Prestazione>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record cancellato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Act/Prestazione/_PartialDelete.cshtml", obj);
        }

    }
}
