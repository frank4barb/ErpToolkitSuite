using ErpToolkit.Helpers;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MySqlX.XDevAPI.Common;
using System.Diagnostics;
using static ErpToolkit.Helpers.Db.DogManager;


namespace ErpToolkit.Controllers
{
    /// <summary>
    /// A <see cref="FeatureController"/> that implements API and RPC methods for the connection manager.
    /// </summary>
    public class ErpUtilitiesController : ControllerErp
    {

        public ErpUtilitiesController()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public IActionResult DefaultPage()
        {
            return RedirectToAction("Index", "Home");
        }

        ////==========================================================================================================
        ////==========================================================================================================



        [HttpGet]
        public JsonResult AutocompleteGetAll(string modelName)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetAll(modelName));
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: AutocompleteGetAll {modelName}: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string modelName, string term)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompleteGetSelect(modelName, term));
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: AutocompleteGetSelect {modelName}: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad(string modelName, [FromBody] List<string> values)
        {
            try
            {
                return Json(ErpContext.Instance.DogFactory.GetDog(dogId).AutocompletePreLoad(modelName, values));
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: AutocompletePreLoad {modelName}: " + ex.Message }); }
        }




        // -- GET: lista tipi documento ----------------------------
        [HttpGet]
        public IActionResult XdataTypes()
        {
            return Json(DogManager.XdataFmtTypes);
        }
        // -- GET: visualizza documento ----------------------------
        [HttpGet]
        public IActionResult ViewXdataModel(string modelName, string icode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(icode)) return BadRequest("Identificativo documento mancante");
                BlobStreamResult blob = ErpContext.Instance.DogFactory.GetDog(dogId).OpenBlobStream(modelName, icode, 0);
                Response.Headers["Content-Disposition"] = "inline";
                if (blob.Bytes != null) return new FileContentResult(blob.Bytes, blob.ContentType) { EnableRangeProcessing = true };
                else return new FileStreamResult(blob.Stream, blob.ContentType) { EnableRangeProcessing = true };
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: ViewXdata {modelName}: " + ex.Message }); }
        }
        // -- POST: aggiunge documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> AddXdataModel(string modelName, string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync(modelName, 'A', null, null, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento caricato.",
                    error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : "",
                    mime = xdataResult?._mimeXdatum ?? ""
                });
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: AddXdataModel {modelName}: " + ex.Message }); }
        }
        // -- POST: aggiorna documento -----------------------------
        [HttpPost]
        public async Task<IActionResult> UpdateXdataModel(string modelName, string icode, string timestampHex, string mref, string descr, string fmt, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0) return BadRequest("File mancante");
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync(modelName, 'M', icode, timestampHex, mref, descr, fmt, file.OpenReadStream(), null);
                return Json(new
                {
                    info = "Documento modificato.",
                    error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : ""
                });
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: UpdateXdata {modelName}: " + ex.Message }); }
        }
        // -- POST: elimina documento ------------------------------
        [HttpPost]
        public async Task<IActionResult> DeleteXdataModel(string modelName, string icode, string timestampHex)
        {
            try
            {
                ModelXdata xdataResult = await ErpContext.Instance.DogFactory.GetDog(dogId).MntXdataBlobStreamAsync(modelName, 'D', icode, timestampHex, null, "", "", null, null);
                return Json(new
                {
                    info = "Documento cancellato.",
                    error = (string?)null,
                    icode = xdataResult?.Icode?.ToString() ?? "",
                    timestampHex = xdataResult?.Timestamp != null ? "0x" + UtilHelper.ByteArrayToHexString(xdataResult.Timestamp) : ""
                });
            }
            catch (Exception ex) { return Json(new { info = (string?)null, error = $"Problemi in accesso al DB: DeleteXdata {modelName}: " + ex.Message }); }
        }







    }
}
