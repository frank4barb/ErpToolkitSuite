# ErpToolkitSuite

L'obiettivo del progetto è costruire una piattaforma di supporto per la creazione, gestione e monitoraggio di semplici ERP (Enterprise Resource Planning). 
La Suite si compone da una libreria NuGet **ErpToolkit** che contiene le funzionalità core, da un progetto Listener **HealthDemo** dimostrativo di un Erp Sanitario e da un progetto Scheduler **PublisherDemo** dimostrativo di un allineatore tra DB. 

# ErpToolkit

Modulo Core con funzionalità di Listener e Scheduler.
La struttura del software si compone di:

1. **Server Web** (ASP.NET Core 8) per la visualizzazione dell'interfaccia grafica basata su architettura MVC (Model-View-Controller), che separa logicamente struttura, visualizzazione e gestione dei dati.
2. **Scheduler programmabile** per l'esecuzione temporizzata di attività.
3. **Listener** per l'esposizione di servizi SOAP, REST, ecc.

I tre moduli condividono una libreria di utilities che espone le seguenti funzionalità.

- `COMP`: Componenti grafici per facilitare la creazione delle pagine web: Autocomplete, Calendario, Tabella, ecc.
- `DB`: Moduli di accesso a diversi database e normalizzazione dei dati: SQL Server, ORACLE, IRIS, MongoDB, PostgreSQL, ecc.
- `DOG`: Funzionalità di archiviazione controllata dei dati per la generazione dei contatori, la gestione delle transazioni, l'audit delle attività e i processi di allineamento asincrono da DB.
- `BO`: Business Objects per la verifica dei dati e la governance delle transazioni complesse su DB.
- `ENV`: Sistema di autenticazione e profilazione utente, con gestione delle variabili di sessione.

La logica di programmazione dei singoli progetto (es: HealthDemo, PublisherDemo, ecc.) risiede invece nei file:

- `Model`: struttura dati e proprietà dei campi del modello,
- `Controller`: accesso alla base di dati ed elaborazioni funzionali,
- `View`: usato esclusivamente per formalizzare la disposizione grafica dei componenti nella pagina.

che usano la libreria **ErpToolkit** per implementare il progetto specifico.  

![](./images/ERPtoolkit-Architettura.png)


Il modello tecnologico prevede l'uso di un servizio Windows o di un processo in background Linux con scalabilità su più macchine in load-balancing, collaborazione tra i processi e monitoraggio centralizzato.

Ognuna delle seguenti parti dell'architettura MVC può sfruttare le funzionalità esposte dalla libreria comune per facilitare lo sviluppo di interfacce e/o allineatori. 
E' necessario descrivere il modello usando specifici attributi riconosciuti dalla libreria (es: `[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]`).
Gli attributi fanno riferimento a funzionalità programmate nelle classi del controller (es: `public JsonResult AutocompleteGetAll()`).
La classe Views serve a collocare nella pagina il controllo (es: `<input asp-for="AvIdGruppo" class="form-control" />`).
Le proprietà del modello consentono anche alla libreria di acquisire le relazini tra i vari campi delle tabelle, per l'impementazione di funzioni di pubblicazione e/o la realizzazione di componenti specifici (es: `[ErpDogField("AV_ID_GRUPPO", SqlFieldNameExt="AV_ID_GRUPPO",  ...  xref(ATTIVITA.AV__ICODE)")]`).

## Model

Attributi usati a corredo di una proprietà del Model

```c#
public class Attivita {
	[Display(Name = "Id Gruppo", ShortName="", Description = "Codice dell'attività di cui questa è una sotto-attività", Prompt="")]
	[ErpDogField("AV_ID_GRUPPO", SqlFieldNameExt="AV_ID_GRUPPO", SqlFieldProperties="prop() xref(ATTIVITA.AV__ICODE) xdup() multbxref()")]
	[DefaultValue("")]
	[AutocompleteClient("Attivita", "AutocompleteGetAll", 1)]
	[DataType(DataType.Text)]
	public string? AvIdGruppo  { get; set; }
	public ErpToolkit.Models.SIO.Act.Attivita? AvIdGruppoObj  { get; set; }
}
```
**Display**: contiene il testo da visualizzare nelle _label_.<br>
**ErpDogField**: contiene i riferimenti ai campi del DB.<br>
**AutocompleteClient**: indica che in visualizzazione la textbox avrà funzionalità di _autocomplete_ acquisendo i valori dal controller _AttivitaController_.<br>

## Controller

Funzioni condivise dal Controller

```c#
    public class AttivitaController : ControllerErp
    {

        ...

        [HttpGet]
        public JsonResult AutocompleteGetAll()
        {
            try
            {
                string sql = "select AV_CODICE + ' - ' + AV_DESCRIZIONE as label, AV__ICODE as value from ATTIVITA where AV__DELETED='N' ";
                return Json(DogHelper.ExecQuery<Choice>(DbConnectionString, sql));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetAll Attivita: " + ex.Message }); }
        }
        [HttpGet]
        public JsonResult AutocompleteGetSelect(string term)
        {
            try
            {
                string sql = "select AV_CODICE + ' - ' + AV_DESCRIZIONE as label, AV__ICODE as value from ATTIVITA where AV__DELETED='N' and upper(AV_CODICE + ' - ' + AV_DESCRIZIONE) like '%" + term.ToUpper() + "%'";
                return Json(DogHelper.ExecQuery<Choice>(DbConnectionString, sql));
            }
            catch (Exception ex)  { return Json(new { error = "Problemi in accesso al DB: AutocompleteGetSelect Attivita: " + ex.Message }); }
        }
        [HttpPost]
        public JsonResult AutocompletePreLoad([FromBody] List<string> values)
        {
            try
            {
                string sql = "select AV_CODICE + ' - ' + AV_DESCRIZIONE as label, AV__ICODE as value from ATTIVITA where AV__DELETED='N' and AV__ICODE in ('" + string.Join("', '", values.ToArray()) + "')";
                return Json(DogHelper.ExecQuery<Choice>(DbConnectionString, sql));
            }
            catch (Exception ex) { return Json(new { error = "Problemi in accesso al DB: AutocompletePreLoad Attivita: " + ex.Message }); }
        }

        ...

        [Authorize(AuthenticationSchemes = "Cookies")]
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            //carico eventuali parametri presenti in TempData
            foreach (var item in TempData.Keys) ViewData[item] = TempData[item];
            return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);  //passo il Controller alla vista, come Model
        }

        [Authorize(AuthenticationSchemes = "Cookies")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index()
        {
            return View("~/Views/SIO/Act/Attivita/Index.cshtml", this);
        }

        ...

    }
```

**AutocompleteGetAll**, **AutocompleteGetSelect** e **AutocompletePreLoad**: sono funzioni a supporto del _**Componente Autocomplete**_ con valori in _Attivita_.<br>
L'attrbuto **Authorize** indica che la pagina può essere visualizzata solo se è stata effettuata la _Login_.<br>

## View

Tag usati nel View e per visualizzare una proprietà del model. 

```html
@model Attivita
@{
    ViewData["Title"] = "Attivita Edit";
}
<div class="modal-dialog" role="document">
                <div class="form-group">
                    <label asp-for="AvIdGruppo" class="control-label"></label>
                    <input asp-for="AvIdGruppo" class="form-control" />
                    <span asp-validation-for="AvIdGruppo" class="text-danger"></span>
                </div>
</div>
```

Il formato grafico è deciso nel Model. Nel View si dispongono semplicemente i tag nella pagina. 


# Use Case

Come Use Case consideriamo, 
- [**HealthDemo**](./HealthDemo/): il modello dati di un ERP sanitario, di cui proponiamo una semplice rappresentazione a puro titolo esemplificativo.
- **PublisherDemo**: un semplice allineatore tra DB che, tenendo conto dell'integrità referenziale del modello, trasferisce anche i record relazionati. 


# Installazione

## Dal pacchetto NuGet

Puoi installare **ErpToolkit** direttamente dalla [NuGet Gallery](https://www.nuget.org/packages/ErpToolkit):

```bash
dotnet add package ErpToolkit --version 1.0.4
```

Oppure, se utilizzi Visual Studio:

1. Apri il tuo progetto.
2. Fai clic con il tasto destro sul progetto in Solution Explorer e seleziona **Manage NuGet Packages....**
3. Cerca il pacchetto ErpToolkit, seleziona la versione desiderata (es. 1.0.4) e installalo.


Inoltre, per interagire con la piattaforma InterSystems IRIS, copia nella directory di esecuzione la Libreria **InterSystems.Data.IRISClient.dll**. Maggiori informazioni sul sito ufficiale di [InterSystems IRIS](https://community.intersystems.com/).


## Dal repository GitHub

Il codice sorgente è disponibile su GitHub [ErpToolkitSuite](https://github.com/frank4barb/ErpToolkitSuite) per studiare il codice o contribuire al progetto.

1. Assicurarsi di avere .NET 8.0 SDK installato nel sistema
2. Clonare il repository:
   ```bash
   git clone https://github.com/frank4barb/ErpToolkitSuite
   ```
3. Copiare il file `ERPdesktop.ini.sample` in `ERPdesktop.ini` e configurarlo secondo le proprie necessità
4. Eseguire il restore dei pacchetti NuGet:
   ```bash
   dotnet restore
   ```
5. Compilare il progetto:
   ```bash
   dotnet build
   ```

# Utilizzo

Il progetto è strutturato in due parti principali:

1. **ErpToolkit**: La libreria core che contiene tutte le funzionalità base dell'ERP
   - Supporta molteplici database (SQL Server, MySQL, PostgreSQL, Oracle, SQLite, MongoDB)
   - Include funzionalità per la gestione dei file e risorse embedded
   - Fornisce supporto per elaborazione CSV e JSON
   - Implementa funzionalità di logging e scheduling

2. **HealthDemo**: Un'applicazione web di esempio che dimostra l'utilizzo della libreria ErpToolkit

Per utilizzare ErpToolkit nel tuo progetto, aggiungi il riferimento al pacchetto NuGet:

```xml
<PackageReference Include="ErpToolkit" Version="1.0.4" />
```

Una volta installato il pacchetto, potrai utilizzare le funzionalità messe a disposizione da ErpToolkit nel tuo progetto. Ad esempio, se stai utilizzando il progetto HealthDemo (che include un riferimento al progetto ErpToolkit), assicurati di:

1. Avere il pacchetto aggiornato (tramite NuGet o compilando direttamente la solution).
2. Utilizzare gli spazi dei nomi e le classi esposte da ErpToolkit per integrare le funzionalità ERP nel tuo progetto.
3. Esempio di utilizzo (l'implementazione effettiva dipende dalle funzionalità esposte dal pacchetto):

```csharp
using ErpToolkit;

public class Program
{
    public static void Main()
    {
        // Inizializza e utilizza le funzionalità di ErpToolkit
        var erp = new ErpCore();
        erp.Initialize();

        // Altre logiche applicative
    }
}
```

Consulta la documentazione interna al repository per ulteriori dettagli su configurazione e personalizzazione.


# Librerie esterne usate

### Database e Connettività
- [AdoNetCore.AseClient](https://github.com/DataAction/AdoNetCore.AseClient) (v0.19.2) - Client SAP ASE per .NET Core
- [Microsoft.Data.SqlClient](https://github.com/dotnet/SqlClient) (v5.2.0) - Client SQL Server
- [MongoDB.Driver](https://github.com/mongodb/mongo-csharp-driver) (v2.28.0) - Driver MongoDB
- [MySql.Data](https://github.com/mysql/mysql-connector-net) (v9.0.0) - Connector MySQL
- [Npgsql](https://github.com/npgsql/npgsql) (v8.0.3) - Provider PostgreSQL
- [Oracle.ManagedDataAccess](https://github.com/oracle/dotnet-db-samples) (v23.5.1) - Provider Oracle
- [System.Data.SQLite](https://system.data.sqlite.org) (v1.0.118) - Provider SQLite

### Framework e Runtime
- [Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation](https://github.com/dotnet/aspnetcore) (v8.0.12) - Compilazione runtime Razor
- [Microsoft.Extensions.FileProviders.Embedded](https://github.com/dotnet/runtime) (v8.0.12) - Gestione file embedded
- [Microsoft.Extensions.Hosting](https://github.com/dotnet/runtime) (v8.0.0) - Hosting .NET
- [Microsoft.Windows.Compatibility](https://github.com/dotnet/runtime) (v8.0.6) - Compatibilità Windows

### Utility e Strumenti
- [CsvHelper](https://github.com/JoshClose/CsvHelper) (v33.0.1) - Elaborazione CSV
- [MemoryPack](https://github.com/Cysharp/MemoryPack) (v1.21.1) - Serializzazione ad alte prestazioni
- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json) (v13.0.3) - Elaborazione JSON
- [NLog](https://github.com/NLog/NLog) (v5.3.2) - Logging
- [Quartz](https://github.com/quartznet/quartznet) (v3.9.0) - Scheduling

### API e Swagger
- [Microsoft.AspNetCore.Grpc.Swagger](https://github.com/dotnet/aspnetcore) (v0.8.5) - Supporto Swagger per gRPC

### Librerie JavaScript
- [jQuery](https://jquery.com) (v3.6.0) - Libreria JavaScript per manipolazione DOM e AJAX
- [jQuery Validation](https://jqueryvalidation.org) (v1.19.5) - Plugin jQuery per validazione form
- [jQuery Validation Unobtrusive](https://github.com/aspnet/jquery-validation-unobtrusive) (v4.0.0) - Integrazione ASP.NET con jQuery Validation
- [DataTables](https://datatables.net) (v1.10.15) - Plugin jQuery per tabelle interattive
- [Bootstrap](https://getbootstrap.com) (v5.1.0) - Framework CSS per UI responsive
- [Bootstrap Icons](https://icons.getbootstrap.com) (v1.11.3) - Set di icone per Bootstrap
- [Quill](https://quilljs.com) (v2.0.2) - Editor di testo ricco
- [Mermaid](https://mermaid.js.org) (v10.0.0) - Libreria per diagrammi e visualizzazioni


# Licenza

Questo progetto è distribuito sotto la licenza AGPL-3.0. Vedi il file `LICENSE` per maggiori dettagli.


------------------------------------



