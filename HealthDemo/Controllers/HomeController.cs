using ErpToolkit.Helpers;
using ErpToolkit.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ErpToolkit.Helpers.Db;
using ErpToolkit.Controllers;

namespace HealthDemo.Controllers
{
    /// <summary>
    /// A <see cref="FeatureController"/> that implements API and RPC methods for the connection manager.
    /// </summary>
    public class HomeController : ControllerErp
    {
        // DEFINIZIONE VOCI DI MENU' 
        private static readonly List<DogManager.MenuItem> SessionListMenu = new List<DogManager.MenuItem>
        {
            new DogManager.MenuItem("ERModel", "Home", "ERModel")
            ,new DogManager.MenuItem("Attivita", "Attivita", "Index")
            ,new DogManager.MenuItem("Episodio", "Episodio", "Index")
            ,new DogManager.MenuItem("Percorso 1", "Home", "Percorso1Start")
            ,new DogManager.MenuItem("Percorso 2", "Home", "Percorso2Start")
        };

        public HomeController()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public IActionResult DefaultPage()
        {
            return RedirectToAction("Index", "Home");
        }
        public IActionResult Privacy()
        {
            return View(this);
        }
        public IActionResult ERModel()
        {
            return View(this);
        }

        ////==========================================================================================================
        ////==========================================================================================================

        //// GESTIONE LOGIN SULLA PAGINA HOME
        ////---------------------------------

        //[Authorize(AuthenticationSchemes = "Cookies")]  //<<< non scommentare: innesca un cilco
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            if (returnUrl != null)
            {
                ModelState.AddModelError(string.Empty, "E' necessario effettuare la login per accedere alla pagina!");
                HttpContext.Session.SetString(SessionReturnUrl, returnUrl);
            }
            //return View();
            return View("~/Views/Home/Index.cshtml", this);  //passo il Controller alla vista, come Model
            
        }

        //[Authorize(AuthenticationSchemes = "Cookies")]  //<<< non scommentare: innesca un cilco
        [HttpPost]
        public async Task<IActionResult> Index(InputLogin Input)
        {
            base.Input2 = Input;
            ModelState.Clear(); //ModelState.ClearValidationState("CompanyName"); //FORZA RICONVALIDA MODELLO >>> https://learn.microsoft.com/it-it/aspnet/core/mvc/models/validation?view=aspnetcore-8.0
            if (!TryValidateModel(Input))
            {
                // logout
                await ErpContext.TermSessionAsync(HttpContext); //clean current session ErpContext and LOGOUT
                //ErpContext.TermSession(HttpContext); //clean current session ErpContext and LOGOUT
                if (HttpContext.User.Identity != null && HttpContext.User.Identity.IsAuthenticated) return LocalRedirect(Url.Content("~/"));  // <<<-- ricarica la pagina di login dopo LOGOUT
                //return View(); // <<<-- visualizza gli errori LOGIN
                return View("~/Views/Home/Index.cshtml", this);  //passo il Controller alla vista, come Model
            }



            // login and new session
            bool login = await ErpContext.InitSessionAsync(HttpContext, Input.Matricola, Input.Password, "");
            //bool login = ErpContext.InitSession(HttpContext, Input.Matricola, Input.Password, "");
            if (!login)
            {
                //errore utente non abilitato
                ModelState.AddModelError(string.Empty, "Matricola o Password non valide!");
                //return View(); // <<<-- visualizza gli errori
                return View("~/Views/Home/Index.cshtml", this);  //passo il Controller alla vista, come Model
            }


            // ASSEGNO VARIABILI DI SESSIONE
            if (ErpContextSession != null) ErpContextSession.ListMenu = SessionListMenu;  //per il momento il men� � uguale per tutti ma pu� essere distinto per utente
            // ... 
            // ... 
            // ... 
            // ... 


            //redirect to ReturnUrl
            string? returnUrl = HttpContext.Session.GetString(SessionReturnUrl); HttpContext.Session.Remove(SessionReturnUrl); //scarico il ReturnUrl
            returnUrl ??= Url.Content("~/"); //default
            return LocalRedirect(returnUrl);
        }


        ////==========================================================================================================
        ////==========================================================================================================
        //// GESTIONE MENU' PERCORSI
        ////---------------------------------

        //Gestore dei Men� a Percorsi.
        //----------------------------
        // - Ogni voce di men� prevede l'esecuzione di un percorso di pagine
        // - Si passa alla pagina successiva mediante il link della tabella (ie: campo [UID] del Modello)
        // - Nell'accesso alla pagina successiva possono essere forzati dei valori di default per i campi del modello, e impostate le variabili readOnly e visible dei campi del filtro di selezione (eg: AddDefault("Te1Icode", "UCSC", false, true) -->  Te1Icode__NY=UCSC)
        // - Si pu� impostare solo readOnly e visible passando il contenuto del campo vuoto
        // - Le impostazioni di Default dei campi possono dipendere dall'appartenenza di Gruppi e Profili dell'utente che ha fatto Login
        // - .... � possibile impostare dei controlli al passagio di pagina, e bloccare gli utenti se non hanno compilato tutti i campi previsti per il profilo.


        //<ul class="navbar-nav mr-auto">
        //    <li class="nav-item">
        //        <a class="nav-link" asp-controller="Shared" asp-action="Percorso1Start">Percorso 1</a>
        //    </li>
        //    <li class="nav-item">
        //        <a class="nav-link" asp-controller="Shared" asp-action="Percorso2Start">Percorso 2</a>
        //    </li>
        //</ul>

        //// Use HomeController's method to determine the next page
        //return RedirectToAction("RedirectToNextPage", "Shared", new { currentPage = "Page1", .....  });
        //
        //eg:   @Html.ActionLink(item.PaCodSanitario, "RedirectToNextPage", "Shared", null, null, null, new { provenienzaPagina = "Paziente", Pa1Icode = item.Pa1Icode, Te1Icode="UCSC"}, null)



        //Impostazione campi da applicare al filtro selezione della pagina del percorso
        // 1) valore di default del campo
        // 2) campo readOnly: Y/N (default: N)
        // 3) campo visibile: Y/N (default: Y)
        //
        // formato: <nome campo modello>=<valore preimpostato del campo>
        // formato: <nome campo modello>_Attr=<Y/N readOnly><Y/N visibile>
        // nota: i nomi campo del modello non possono contenere '_' nel nome del campo

        //pagina del percorso
        //public class Page  ==>> trasferito in DogManager
        //{
        //    public string pageName { get; set; } = "";
        //    //public List<DefaultFieldValue> defaultFields { get; set; } = new List<DefaultFieldValue>();
        //    public Dictionary<string, string?> defaultFields { get; set; } = new Dictionary<string, string?>();
        //    public Page(string name) { pageName = name; }
        //    public Page AddDefault(string name, string? value) { defaultFields[name] = value; return this; }
        //}

        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        public static readonly Dictionary<string, List<DogManager.Page>> PathMenu = new Dictionary<string, List<DogManager.Page>> {
            { "Percorso1", new List<DogManager.Page> {
                 new DogManager.Page("Paziente")
                        .AddDefault("SelPaNome_Attr", DogManager.FieldAttr.strAttr(true, false))
                        .AddDefault("SelPaIdCittadinanza_Attr", DogManager.FieldAttr.strAttr(true, true))
                        .AddDefault("PaNome_Attr", DogManager.FieldAttr.strAttr(false, false))
                        .AddDefault("EDIT.PaSesso_Attr", DogManager.FieldAttr.strAttr(false, false))
                 , new DogManager.Page("Episodio").AddDefault("SelTe1Icode", "UCSC")
                        .AddDefault("SelEpIdPaziente_Attr", DogManager.FieldAttr.strAttr(true, true))
                        .AddDefault("SelTe1Icode_Attr", DogManager.FieldAttr.strAttr(true, true))
                        .AddDefault("SelEpLetto_Attr", DogManager.FieldAttr.strAttr(true, true)) } }
             ,{ "Percorso2", new List<DogManager.Page> { new DogManager.Page("Page2"), new DogManager.Page("Page1") } }
        };

        //------------------------------------------------------------------------------------------------------------------------------------------
        //------------------------------------------------------------------------------------------------------------------------------------------

        //[Authorize(AuthenticationSchemes = "Cookies")]  //<<< non scommentare: innesca un cilco
        [HttpGet]
        public IActionResult Percorso1Start() { return RedirectToStartPage("Percorso1"); }

        //[Authorize(AuthenticationSchemes = "Cookies")]  //<<< non scommentare: innesca un cilco
        [HttpGet]
        public IActionResult Percorso2Start() { return RedirectToStartPage("Percorso2"); }

        //------------------------------------------------------------------------------------------------------------------------------------------

        public IActionResult RedirectToStartPage(string nomePercorso)
        {
            TempData["NomeSequenzaPagine"] = nomePercorso; // Set data in TempData (i dati rimangono in memoria solo per la prossima request, poi sono cancellati dalla struttura
            return RedirectToAction("Index", PathMenu[nomePercorso][0].pageName);
        }

        public IActionResult RedirectToNextPage(string provenienzaPagina)
        {
            try
            {
                RouteValueDictionary routeValuesDictionary = new RouteValueDictionary();
                foreach (var key in Request.Query.Keys) routeValuesDictionary.Add(key, Request.Query[key]);
                //routeValuesDictionary.Add("AnotherFixedParm", "true");

                // trovo percorso
                string nomePercorso = TempData["NomeSequenzaPagine"] as string; TempData["NomeSequenzaPagine"] = nomePercorso; //ricarico per mantenere in memoria
                List<DogManager.Page> sequenzaPagine = PathMenu[nomePercorso];
                // calcolo prossima pagina in precorso
                int provenienzaPaginaIdx = sequenzaPagine.FindIndex(page => page.pageName.Equals(provenienzaPagina, StringComparison.Ordinal));  //int provenienzaPaginaIdx = sequenzaPagine.IndexOf(provenienzaPagina);
                int successivaPaginaIdx = provenienzaPaginaIdx + 1;
                var nextPage = sequenzaPagine[0]; // pagina di default = prima pagina (If there are no more pages in the sequence, redirect to the first page)
                if (successivaPaginaIdx < sequenzaPagine.Count) nextPage = sequenzaPagine[successivaPaginaIdx]; // trovata pagina successiva
                //inserisco inpostazioni di default per i cxampi di selezione
                foreach (var key in nextPage.defaultFields.Keys) routeValuesDictionary.Add(key, nextPage.defaultFields[key]);
                //redirect
                return RedirectToAction("Index", nextPage.pageName, routeValuesDictionary);
            }
            catch (Exception ex) { return RedirectToAction("Index", provenienzaPagina); }  // in caso di problemi rimango sulla stessa pagina
        }


        //##############################################################################################
        //####
        //#### ERROR
        //####
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    }
}
