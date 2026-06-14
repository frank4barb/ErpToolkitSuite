using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpToolkit.Controllers;
using HealthDemo.Models.SIO.HealthData;

using static ErpToolkit.Helpers.Db.DogManager;
namespace HealthDemo.Controllers.SIO.HealthData
{
    public class TipoDatoClinicoController : ControllerErp
    {
        private static readonly List<string> xrefTables = new List<string> {
             "ReIdTipoDatoClinico"   //carico in cache tutti i dati di RisultatoEsame collegati
            ,"TcIdGruppo"   //carico in cache tutti i dati di TipoDatoClinico collegati
            ,"SsIdTipoDatoClinico"   //carico in cache tutti i dati di StatoSalute collegati
            ,"DcIdTipoDatoClinico"   //carico in cache tutti i dati di DocumentoClinico collegati
            ,"PvIdTipoDatoClinico"   //carico in cache tutti i dati di ParametroVitale collegati
        };
        private const string ErpContext_dogCache = "@HealthDemo.Controllers.SIO.HealthData.TipoDatoClinico_dogCache";
        private DogCache _dogCache = new DogCache();

        //private static NLog.ILogger _logger;
        public TipoDatoClinicoController()
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
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetAll<TipoDatoClinico>());
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetAll TipoDatoClinico: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string term, string? modelPropertyName = null, [FromQuery] Dictionary<string, List<string>> extraFields = null)
        {
            try
            {
                extraFields?.Remove("term"); extraFields?.Remove("modelPropertyName");   // Rimuovi "term" e "modelPropertyName" dai filtri se finiscono nel dizionario
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetSelect<TipoDatoClinico>(term, modelPropertyName: modelPropertyName, extraFields: extraFields));
            }
            catch (Exception ex)  { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetSelect TipoDatoClinico: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad([FromBody] List<string> values)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompletePreLoad<TipoDatoClinico>(values));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompletePreLoad TipoDatoClinico: " + ex.Message }); }
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
                BlobStreamResult blob = ErpContext.Instance.DogFactory.GetDog(dogId).OpenBlobStream<TipoDatoClinico>(icode, 0);
                Response.Headers["Content-Disposition"] = "inline";
                if (blob.Bytes != null) return new FileContentResult(blob.Bytes, blob.ContentType) { EnableRangeProcessing = true };
                else return new FileStreamResult(blob.Stream, blob.ContentType) { EnableRangeProcessing = true };
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: ViewXdata TipoDatoClinico: " + ex.Message }); }
        }
        // -- POST: aggiunge documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> AddXdata(string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<TipoDatoClinico>('A', null, null, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento caricato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AddXdataModel TipoDatoClinico: " + ex.Message }); }
        }
        // -- POST: aggiorna documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateXdata(string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<TipoDatoClinico>('M', icode, timestampHex, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento modificato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: UpdateXdata TipoDatoClinico: " + ex.Message }); }
        }
        // -- POST: elimina documento ------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteXdata(string icode, string timestampHex)
        {
            try
            {
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync<TipoDatoClinico>('D', icode, timestampHex, null, "", "", null, null);
                return Json(new
                {
                    info = "Documento cancellato.",
                     error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: DeleteXdata TipoDatoClinico: " + ex.Message }); }
        }
        [BindProperty]
        public SelTipoDatoClinico Select { get; set; }
        [BindProperty]
        public List<TipoDatoClinico> List { get; set; } = new List<TipoDatoClinico>();
        [BindProperty]
        public TipoDatoClinico Row { get; set; }
        [TempData]
        public string StatusMessage { get; set; }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            this._dogCache = new DogCache();    // Inizializza le risorse ...in caso di chiamata della pagina dall'esterno (ie: no reload)

            this.Select = new SelTipoDatoClinico();
            foreach (var key in Request.Query.Keys) DogManager.setPropertyValue(this.Select, key, Request.Query[key]); // carica parametri QueryString
            this.List = new List<TipoDatoClinico>();
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/HealthData/TipoDatoClinico/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(SelTipoDatoClinico selobj)
        {
            if (selobj != null) { this.Select = selobj; }
            ModelState.Clear(); //FORZA RICONVALIDA MODELLO
            if (!TryValidateModel(this.Select))
            {
                ModelState.AddModelError(string.Empty, "Verifica valore dei campi.");
                return View("~/Views/SIO/HealthData/TipoDatoClinico/Index.cshtml", this);
            }
            if (!this.Select.TryValidateInt(ModelState)) {
                return View("~/Views/SIO/HealthData/TipoDatoClinico/Index.cshtml", this);
            }
            //carica lista
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<TipoDatoClinico>(this.Select, xrefTables, false, null, ref this._dogCache, null, -1); }  
            catch (Exception ex) { ModelState.AddModelError(string.Empty, "Problemi in accesso al DB: List: " + ex.Message); }
            this.StatusMessage = "Lista caricata!";
            return View("~/Views/SIO/HealthData/TipoDatoClinico/Index.cshtml", this);
        }

        [HttpPost]
        public IActionResult ReadForEdit([FromBody] ModelParam parms)  
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoDatoClinico obj = this.ReadForEditModel<TipoDatoClinico>(parms, xrefTables, ref this._dogCache, prefix: modelPrefix);
            return PartialView("~/Views/SIO/HealthData/TipoDatoClinico/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Save([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "EDIT";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoDatoClinico obj = this.SaveModel<TipoDatoClinico>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "*allowTouch*");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record aggiornato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/HealthData/TipoDatoClinico/_PartialEdit.cshtml", obj);
        }
        [HttpPost]
        public IActionResult ReadForDelete([FromBody] ModelParam parms)  
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoDatoClinico obj = this.ReadForEditModel<TipoDatoClinico>(parms, null, ref this._dogCache, prefix: modelPrefix, action: 'D');    // non carico tabelle relazionate per il delete
            return PartialView("~/Views/SIO/HealthData/TipoDatoClinico/_PartialDelete.cshtml", obj);
        }
        [HttpPost]
        public IActionResult Delete([FromBody] ModelObject dataObj)
        {
            string modelPrefix = "DELETE";
            ViewData.TemplateInfo.HtmlFieldPrefix = modelPrefix;  //prefisso da applicare a id e name nei tag, se uso lo stesso @model più volte nella stessa pagina eg: <xx id="EDIT_IdPatient" name="EDIT.IdPatient" ..>
            TipoDatoClinico obj = this.SaveModel<TipoDatoClinico>(dataObj, ref this._dogCache, prefix: modelPrefix, options: "[MAX_ONE_OBJ] [NO_ADD] [NO_UPDATE]");
            if (!ModelState.IsValid) { return this.ValidationResult(); }

            this.StatusMessage = "Record cancellato!";
            //---GESTISCE AZIONI CLICK PULSANTE
            ViewData["IsModalACTION"] = "CLOSE";
            ViewData["IsPageACTION"] = "RELOAD";
            ViewData["IsPageREDIRECT"] = "";
            //---
            return PartialView("~/Views/SIO/HealthData/TipoDatoClinico/_PartialDelete.cshtml", obj);
        }
    }
}
