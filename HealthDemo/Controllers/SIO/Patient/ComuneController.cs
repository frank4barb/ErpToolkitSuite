using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpToolkit.Controllers;
using HealthDemo.Models.SIO.Patient;

using static ErpToolkit.Helpers.Db.DogManager;
namespace HealthDemo.Controllers.SIO.Patient
{
    public class ComuneController : ControllerErp
    {
        private static readonly List<string> xrefTables = new List<string> {
             "DiIdComune"   //carico in cache tutti i dati di Distretto collegati
            ,"PaIdComuneNascita"   //carico in cache tutti i dati di Paziente collegati
            ,"PaIdComuneRes"   //carico in cache tutti i dati di Paziente collegati
            ,"PaIdComuneDom"   //carico in cache tutti i dati di Paziente collegati
        };
        private const string ErpContext_dogCache = "@HealthDemo.Controllers.SIO.Patient.Comune_dogCache";
        private DogCache _dogCache = new DogCache();

        //private static NLog.ILogger _logger;
        public ComuneController()
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
        public JsonResult AutocompleteGetAll(string? modelPropertyName = null, [FromQuery] List<string> extraFieldNames = null)
        {
            try
            {
                extraFieldNames?.Remove("modelPropertyName");   // Rimuovi "modelPropertyName" dai filtri se finiscono nella lista
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetAll<Comune>(modelPropertyName: modelPropertyName, extraFieldNames: extraFieldNames));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetAll Comune: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string term, string? modelPropertyName = null, [FromQuery] Dictionary<string, List<string>> extraFields = null)
        {
            try
            {
                extraFields?.Remove("term"); extraFields?.Remove("modelPropertyName");   // Rimuovi "term" e "modelPropertyName" dai filtri se finiscono nel dizionario
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetSelect<Comune>(term, modelPropertyName: modelPropertyName, extraFields: extraFields));
            }
            catch (Exception ex)  { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetSelect Comune: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad([FromBody] List<string> values)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompletePreLoad<Comune>(values));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompletePreLoad Comune: " + ex.Message }); }
        }
       // -- GET: lista tipi documento ----------------------------
        [HttpGet]
        public IActionResult XdataTypes()
        {
            return Json(DogManager.XdataFmtTypes);
        }
        // -- GET: visualizza documento ----------------------------
        [HttpGet]
        public IActionResult ViewXdata(string icode) 
        {
            try
            {
                if (string.IsNullOrWhiteSpace(icode)) return BadRequest("Identificativo documento mancante");
                BlobStreamResult blob = ErpContext.Instance.DogFactory.GetDog(dogId).OpenBlobStream<Comune>(icode, 0);
                Response.Headers["Content-Disposition"] = "inline";
                if (blob.Bytes != null) return new FileContentResult(blob.Bytes, blob.ContentType) { EnableRangeProcessing = true };
                else return new FileStreamResult(blob.Stream, blob.ContentType) { EnableRangeProcessing = true };
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: ViewXdata Comune: " + ex.Message }); }
        }
        // -- POST: aggiunge documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> AddXdata(string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<Comune>('A', null, null, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento caricato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AddXdataModel Comune: " + ex.Message }); }
        }
        // -- POST: aggiorna documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateXdata(string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<Comune>('M', icode, timestampHex, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento modificato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: UpdateXdata Comune: " + ex.Message }); }
        }
        // -- POST: elimina documento ------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteXdata(string icode, string timestampHex)
        {
            try
            {
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<Comune>('D', icode, timestampHex, null, "", "", null, null);
                return Json(new
                {
                    info = "Documento cancellato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: DeleteXdata Comune: " + ex.Message }); }
        }
        [BindProperty]
        public SelComune Select { get; set; }
        [BindProperty]
        public List<Comune> List { get; set; } = new List<Comune>();
        [BindProperty]
        public Comune Row { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            this._dogCache = new DogCache();    // Inizializza le risorse ...in caso di chiamata della pagina dall'esterno (ie: no reload)

            this.Select = new SelComune();
            foreach (var key in Request.Query.Keys) DogManager.setPropertyValue(this.Select, key, Request.Query[key]); // carica parametri QueryString
            this.List = new List<Comune>();
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/Patient/Comune/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SelComune selobj)
        {
            if (selobj != null) { this.Select = selobj; }
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            if (!TryValidateModel(this.Select))
            {
                ModelState.AddModelError(string.Empty, "Verifica valore dei campi.");
                return View("~/Views/SIO/Patient/Comune/Index.cshtml", this);
            }
            if (!this.Select.TryValidateInt(ModelState)) {
                return View("~/Views/SIO/Patient/Comune/Index.cshtml", this);
            }
            //carica lista
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<Comune>(this.Select, xrefTables, false, null, ref this._dogCache, null, -1); }  
            catch (Exception ex) { ModelState.AddModelError(string.Empty, "Problemi in accesso al DB: List: " + ex.Message); }
            this.StatusMessage = "Lista caricata!";
            return View("~/Views/SIO/Patient/Comune/Index.cshtml", this);
        }

        [HttpPost]
        public IActionResult ReadForEdit([FromBody] ModelParam parms)  
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Comune obj = this.ReadForEditModel<Comune>(parms, xrefTables, ref this._dogCache, prefix: modelPrefix);
            return PartialView("~/Views/SIO/Patient/Comune/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Save([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Comune obj = this.SaveModel<Comune>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "*allowTouch*");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record aggiornato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Patient/Comune/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult ReadForDelete([FromBody] ModelParam parms)  
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Comune obj = this.ReadForEditModel<Comune>(parms, null, ref this._dogCache, prefix: modelPrefix, action: 'D');    // non carico tabelle relazionate per il delete
            return PartialView("~/Views/SIO/Patient/Comune/_PartialDelete.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Delete([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            Comune obj = this.SaveModel<Comune>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record cancellato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/Patient/Comune/_PartialDelete.cshtml", obj);
        }
    }
}
