using ErpToolkit.Helpers;
using Microsoft.AspNetCore.Mvc;


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



    }
}
