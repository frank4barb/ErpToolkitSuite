using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ErpToolkit.Controllers;
using HealthDemo.Models.SIO.Act;
using static ErpToolkit.Helpers.Db.DogManager;
using HealthDemo.Models.SIO.HealthData;

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







        ////[Authorize]
        ////[HttpGet("ViewBlob/{table}/{fileExtension}/{id}")]
        //[HttpGet]
        //public IActionResult ViewBlob(string table, string fileExtension, string id)
        //{

        //    FileContentResult file = null;
        //    try { file = ErpContext.Instance.DogFactory.GetDog(dogId).ReadBlob<HealthDemo.Models.SIO.HealthData.RisultatoEsame>("IU047HXZLC6R"); } 
        //    catch (Exception ex) { return BadRequest("Problemi in accesso al DB: ViewBlob: " + ex.Message); }
        //    if (file == null) { return NotFound(); }
        //    Response.Headers["Content-Disposition"] = "inline";
        //    return file;

        //}


        //[Authorize]
        //[HttpGet]
        //public IActionResult ViewBlobStreaming(string idx) 
        //{
        //    try
        //    {
        //        string id = "IU047HXZLC6R";
        //        var blob = ErpContext.Instance.DogFactory.GetDog(dogId).OpenBlobStream<HealthDemo.Models.SIO.HealthData.RisultatoEsame>(id, 0);

        //        Response.Headers["Content-Disposition"] = "inline";

        //        return new FileStreamResult(blob.Stream, blob.ContentType)
        //        {
        //            EnableRangeProcessing = true   // FONDAMENTALE
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Errore ViewBlobStreaming: " + ex.Message);
        //    }
        //}


        [Authorize]
        [HttpGet]
        public IActionResult ViewBlob(string icode) { 
            return base.ViewXdataTable("RisultatoEsame", "IU047HXZLC6R"); 
        } 



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
            ////---TEST----------------------------------
            //var xxx = XdataTest(null);
            ////-----------------------------------------


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
            try { this.List = ErpContext.Instance.DogFactory.GetDog(dogId).List<Prestazione>(this.Select, xrefPrestazione, false, null, ref this._dogCache, null, -1); }
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















        ////////// ============================================================
        //////////  AGGIUNGERE in RisultatoEsameController.cs
        //////////  (dentro la classe RisultatoEsameController)
        ////////// ============================================================

        ////////#region ── TEST XDATA ────────────────────────────────────────

        /////////// <summary>
        /////////// Pagina HTML che esegue il test write→read del blob Xdata.
        /////////// Raggiungibile via GET  /RisultatoEsame/XdataTest
        /////////// </summary>
        ////////[Authorize(AuthenticationSchemes = "Cookies")]
        ////////[HttpGet]
        ////////public IActionResult XdataTest()
        ////////{
        ////////    //return View("~/Views/SIO/HealthData/RisultatoEsame/XdataTest.cshtml");
        ////////    return View("~/Views/SIO/BO/StatoRichieste/Index.cshtml");
        ////////}

        /////////// <summary>
        /////////// Esegue il test completo:
        ///////////   1. Genera un PDF minimo in memoria (o usa il file caricato)
        ///////////   2. Chiama MntXdataBlobStreamAsync action='A'  (INSERT)
        ///////////   3. Rilegge il blob con OpenBlobStream
        ///////////   4. Confronta byte per byte
        ///////////   5. Fa DELETE del record inserito (cleanup)
        ///////////   6. Restituisce JSON con il dettaglio dei risultati
        ///////////
        /////////// POST /RisultatoEsame/XdataTest
        /////////// body: multipart/form-data  oppure  nessun body (usa PDF sintetico)
        /////////// </summary>
        /////////// 


        ////////public byte[] FileToByteArray(string fileName)
        ////////{
        ////////    byte[] buff = null;
        ////////    FileStream fs = new FileStream(fileName,
        ////////                                   FileMode.Open,
        ////////                                   FileAccess.Read);
        ////////    BinaryReader br = new BinaryReader(fs);
        ////////    long numBytes = new FileInfo(fileName).Length;
        ////////    buff = br.ReadBytes((int)numBytes);
        ////////    return buff;
        ////////}



        ////////[HttpPost]
        ////////public async Task<IActionResult> XdataTest(IFormFile? file)
        ////////{
        ////////    var steps = new List<object>();          // log step-by-step
        ////////    string? insertedIcode = null;            // icode del record inserito (per cleanup)

        ////////    try
        ////////    {
        ////////        // ── STEP 1 · Prepara i byte originali ─────────────────────────────
        ////////        byte[] originalBytes;
        ////////        string fileName;

        ////////        if (file != null && file.Length > 0)
        ////////        {
        ////////            // Legge il file uploadato in modo ASINCRONO (best-practice)
        ////////            using var uploadMs = new MemoryStream();
        ////////            await file.CopyToAsync(uploadMs);
        ////////            originalBytes = uploadMs.ToArray();
        ////////            fileName = file.FileName;
        ////////        }
        ////////        else
        ////////        {
        ////////            // Genera un PDF sintetico minimo (header PDF valido, 1 pagina vuota)


        ////////            //originalBytes = BuildMinimalPdf();
        ////////            originalBytes = FileToByteArray("C:\\_FRANK\\test.pdf");
                    
        ////////            fileName = "test_synthetic.pdf";
        ////////        }

        ////////        steps.Add(new
        ////////        {
        ////////            step = 1,
        ////////            label = "Bytes originali pronti",
        ////////            ok = true,
        ////////            detail = $"File: {fileName} | Dimensione: {originalBytes.Length} byte | " +
        ////////                     $"Primi 8 byte: {ToHex(originalBytes, 8)}"
        ////////        });

        ////////        // ── STEP 2 · INSERT via MntXdataBlobStreamAsync ───────────────────
        ////////        // Usa un Mref fittizio: prende il primo Icode disponibile (o usa 0/1 se numerico)
        ////////        // ATTENZIONE: sostituire "TEST_MREF" con un Icode valido della tabella padre
        ////////        //             oppure usare un record di test già esistente.
        ////////        const string TEST_MREF = "IUF18CSNJKBI";   // <── adatta a un Icode padre valido
        ////////        const string TEST_DESCR = "[XDATA-TEST] Documento di test automatico";
        ////////        const string TEST_FMT = "PDF";

        ////////        ModelXdata insertResult;
        ////////        using (var writeStream = new MemoryStream(originalBytes))
        ////////        {
        ////////            insertResult = await ErpContext.Instance.DogFactory.GetDog(dogId)
        ////////                .MntXdataBlobStreamAsync<RisultatoEsame>(
        ////////                    action: 'A',
        ////////                    icodeStr: null,
        ////////                    timestampHex: null,
        ////////                    mrefStr: TEST_MREF,
        ////////                    descr: TEST_DESCR,
        ////////                    fmt: TEST_FMT,
        ////////                    dataStream: writeStream,
        ////////                    transactionId: null);
        ////////        }

        ////////        insertedIcode = insertResult?.Icode?.ToString();

        ////////        steps.Add(new
        ////////        {
        ////////            step = 2,
        ////////            label = "INSERT MntXdataBlobStreamAsync",
        ////////            ok = insertedIcode != null,
        ////////            detail = insertedIcode != null
        ////////                ? $"Icode inserito: {insertedIcode} | Timestamp: {ToHex(insertResult!.Timestamp, 8)}"
        ////////                : "Icode null — INSERT non riuscito"
        ////////        });

        ////////        if (insertedIcode == null)
        ////////            return Json(new { success = false, steps });

        ////////        // ── STEP 3 · READ via OpenBlobStream ──────────────────────────────
        ////////        BlobStreamResult blob = ErpContext.Instance.DogFactory.GetDog(dogId)
        ////////            .OpenBlobStream<RisultatoEsame>(insertedIcode, 0);

        ////////        byte[] readBytes;
        ////////        if (blob.Bytes != null)
        ////////        {
        ////////            readBytes = blob.Bytes;
        ////////        }
        ////////        else if (blob.Stream != null)
        ////////        {
        ////////            using var readMs = new MemoryStream();
        ////////            await blob.Stream.CopyToAsync(readMs);
        ////////            readBytes = readMs.ToArray();
        ////////        }
        ////////        else
        ////////        {
        ////////            readBytes = Array.Empty<byte>();
        ////////        }

        ////////        steps.Add(new
        ////////        {
        ////////            step = 3,
        ////////            label = "READ OpenBlobStream",
        ////////            ok = readBytes.Length > 0,
        ////////            detail = $"Byte riletti: {readBytes.Length} | ContentType rilevato: {blob.ContentType} | " +
        ////////                     $"Primi 8 byte: {ToHex(readBytes, 8)}"
        ////////        });

        ////////        // ── STEP 4 · Confronto byte per byte ──────────────────────────────
        ////////        bool lengthMatch = originalBytes.Length == readBytes.Length;
        ////////        int firstDiffIdx = -1;

        ////////        if (lengthMatch)
        ////////        {
        ////////            for (int i = 0; i < originalBytes.Length; i++)
        ////////            {
        ////////                if (originalBytes[i] != readBytes[i]) { firstDiffIdx = i; break; }
        ////////            }
        ////////        }

        ////////        bool bytesMatch = lengthMatch && firstDiffIdx == -1;

        ////////        steps.Add(new
        ////////        {
        ////////            step = 6,
        ////////            label = "Confronto byte per byte",
        ////////            ok = bytesMatch,
        ////////            detail = bytesMatch
        ////////                ? $"✅ IDENTICI — {originalBytes.Length} byte corrispondono perfettamente"
        ////////                : lengthMatch
        ////////                    ? $"❌ DIVERSI — stessa lunghezza ({originalBytes.Length}), " +
        ////////                      $"primo byte diverso a indice {firstDiffIdx}: " +
        ////////                      $"originale=0x{originalBytes[firstDiffIdx]:X2} letto=0x{readBytes[firstDiffIdx]:X2}"
        ////////                    : $"❌ DIMENSIONI DIVERSE — originale: {originalBytes.Length} byte, " +
        ////////                      $"riletti: {readBytes.Length} byte"
        ////////        });

        ////////        // ── STEP 5 · Salva i due file per ispezione manuale ───────────────
        ////////        string tmpDir = Path.Combine(Path.GetTempPath(), "XdataTest");
        ////////        Directory.CreateDirectory(tmpDir);
        ////////        string pathOrig = Path.Combine(tmpDir, "original.bin");
        ////////        string pathRead = Path.Combine(tmpDir, "readback.bin");
        ////////        await System.IO.File.WriteAllBytesAsync(pathOrig, originalBytes);
        ////////        await System.IO.File.WriteAllBytesAsync(pathRead, readBytes);

        ////////        steps.Add(new
        ////////        {
        ////////            step = 5,
        ////////            label = "File salvati per ispezione",
        ////////            ok = true,
        ////////            detail = $"Originale → {pathOrig} | Riletto → {pathRead}"
        ////////        });

        ////////        // ── STEP 6 · CLEANUP: DELETE del record inserito ──────────────────
        ////////        try
        ////////        {
        ////////            string tsHex = insertResult?.Timestamp != null
        ////////                ? "0x" + UtilHelper.ByteArrayToHexString(insertResult.Timestamp)
        ////////                : "";

        ////////            await ErpContext.Instance.DogFactory.GetDog(dogId)
        ////////                .MntXdataBlobStreamAsync<RisultatoEsame>(
        ////////                    action: 'D',
        ////////                    icodeStr: insertedIcode,
        ////////                    timestampHex: tsHex,
        ////////                    mrefStr: TEST_MREF,
        ////////                    descr: null,
        ////////                    fmt: "",
        ////////                    dataStream: Stream.Null,
        ////////                    transactionId: null);

        ////////            steps.Add(new
        ////////            {
        ////////                step = 6,
        ////////                label = "CLEANUP DELETE",
        ////////                ok = true,
        ////////                detail = $"Record {insertedIcode} eliminato correttamente"
        ////////            });
        ////////        }
        ////////        catch (Exception exDel)
        ////////        {
        ////////            steps.Add(new
        ////////            {
        ////////                step = 6,
        ////////                label = "CLEANUP DELETE",
        ////////                ok = false,
        ////////                detail = $"DELETE fallita (il record {insertedIcode} è rimasto nel DB): {exDel.Message}"
        ////////            });
        ////////        }

        ////////        return Json(new
        ////////        {
        ////////            success = bytesMatch,
        ////////            summary = bytesMatch ? "✅ PASS — blob write/read identici" : "❌ FAIL — blob corrotto",
        ////////            steps
        ////////        });
        ////////    }
        ////////    catch (Exception ex)
        ////////    {
        ////////        // Prova cleanup emergenza
        ////////        if (insertedIcode != null)
        ////////        {
        ////////            try
        ////////            {
        ////////                await ErpContext.Instance.DogFactory.GetDog(dogId)
        ////////                    .MntXdataBlobStreamAsync<RisultatoEsame>(
        ////////                        'D', insertedIcode, null, "TEST_MREF", null, "", Stream.Null, null);
        ////////            }
        ////////            catch { /* ignora */ }
        ////////        }

        ////////        steps.Add(new { step = 99, label = "ECCEZIONE", ok = false, detail = ex.ToString() });
        ////////        return Json(new { success = false, summary = $"💥 EXCEPTION: {ex.Message}", steps });
        ////////    }
        ////////}

        ////////// ── Helpers privati ───────────────────────────────────────────────────────────

        /////////// <summary>Restituisce i primi <paramref name="n"/> byte come stringa esadecimale.</summary>
        ////////private static string ToHex(byte[]? data, int n)
        ////////{
        ////////    if (data == null || data.Length == 0) return "(vuoto)";
        ////////    int take = Math.Min(n, data.Length);
        ////////    var sb = new System.Text.StringBuilder(take * 3);
        ////////    for (int i = 0; i < take; i++) sb.Append(data[i].ToString("X2")).Append(' ');
        ////////    if (data.Length > n) sb.Append("...");
        ////////    return sb.ToString().Trim();
        ////////}

        /////////// <summary>
        /////////// Genera un PDF sintetico valido al minimo indispensabile
        /////////// (header %PDF-1.4 + oggetti minimi + %%EOF).
        /////////// Utile per testare senza caricare un file esterno.
        /////////// </summary>
        ////////private static byte[] BuildMinimalPdf()
        ////////{
        ////////    // PDF minimale hand-crafted — valido per qualsiasi reader
        ////////    const string pdf = "%PDF-1.4\n" +
        ////////        "1 0 obj<</Type/Catalog/Pages 2 0 R>>endobj\n" +
        ////////        "2 0 obj<</Type/Pages/Kids[3 0 R]/Count 1>>endobj\n" +
        ////////        "3 0 obj<</Type/Page/MediaBox[0 0 612 792]/Parent 2 0 R/Resources<<>>>>endobj\n" +
        ////////        "xref\n0 4\n" +
        ////////        "0000000000 65535 f \n" +
        ////////        "0000000009 00000 n \n" +
        ////////        "0000000058 00000 n \n" +
        ////////        "0000000115 00000 n \n" +
        ////////        "trailer<</Size 4/Root 1 0 R>>\n" +
        ////////        "startxref\n210\n%%EOF\n";
        ////////    return System.Text.Encoding.ASCII.GetBytes(pdf);
        ////////}

        ////////#endregion









    }
}
