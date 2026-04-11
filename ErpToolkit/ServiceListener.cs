//using ErpToolkit.Helpers;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;

////>>>>swagger
//using Google.Protobuf.WellKnownTypes;
//using Microsoft.OpenApi.Models;
//using Swashbuckle.AspNetCore.SwaggerGen;
//using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.Extensions.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Builder;
////<<<<swagger

using ErpToolkit.Extensions;
using ErpToolkit.Helpers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ErpToolkit.Helpers.Db;
using static ErpToolkit.Helpers.Db.DogFactory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace ErpToolkit
{
    public class ServiceListener : IHostedService
    {
        private Task? _webTask;
        private CancellationTokenSource? _cts;
        
        private static NLog.ILogger _logger;
        public ServiceListener()
        {
            //SetUpNLog();
            NLog.LogManager.Configuration = UtilHelper.GetNLogConfig(); // Apply config
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                //ATTIVA SERVER WEB INTERNO Kestrel IIS in base a quanto presente in configurazione appsettings.json
                //https://learn.microsoft.com/it-it/aspnet/core/fundamentals/host/generic-host?view=aspnetcore-8.0
                //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/host/web-host?view=aspnetcore-8.0
                //https://learn.microsoft.com/it-it/dotnet/architecture/microservices/multi-container-microservice-net-applications/background-tasks-with-ihostedservice
                //https://learn.microsoft.com/it-it/aspnet/core/fundamentals/environments?view=aspnetcore-8.0
                //swagger
                //https://learn.microsoft.com/it-it/aspnet/core/grpc/json-transcoding-openapi?view=aspnetcore-8.0



                // var builder = WebApplication.CreateBuilder(); //var builder = WebApplication.CreateBuilder(args);


                var builder = WebApplication.CreateBuilder(new WebApplicationOptions
                {
                    //Args = args,
                    ContentRootPath = ErpContext.CurrentDirectory,  // <<< QUESTA È LA CHIAVE DEL PROBLEMA
                    WebRootPath = Path.Combine(ErpContext.CurrentDirectory, "wwwroot")
                });


                // FORZO IP E PORTA DI ASCOLTO DEL SERVER WEB INTERNO Kestrel (se non specificato in appsettings.json)
                //
                // ATTENZIONE: se specificato in appsettings.json allora sovrascrive questa configurazione, quindi è importante che in appsettings.json sia presente la configurazione di Kestrel con l'endpoint Http e la porta di ascolto, altrimenti il server non si avvia.
                //{
                //    "Kestrel": {
                //        "Endpoints": {
                //            "Http": {
                //                "Url": "http://0.0.0.0:8080"
                //            }
                //        }
                //    }
                //}
                string webServerUrl = ErpContext.Instance.GetString("#webServerUrl");  //formato: "http://0.0.0.0:8080"
                if (!string.IsNullOrWhiteSpace(webServerUrl)) { builder.WebHost.UseUrls(webServerUrl); }


                //inserisco autenticazione custom (LDAP)

                //>>>authentication
                // >>>>>  https://www.tektutorialshub.com/asp-net-core/user-registration-login-using-cookie-authentication-asp-net-core/     <<<<<<<

                //https://learn.microsoft.com/it-it/aspnet/core/security/authentication/cookie?view=aspnetcore-8.0
                //https://github.com/dotnet/AspNetCore.Docs/blob/main/aspnetcore/security/authentication/cookie.md
                //https://medium.com/@bahadirdamar/net-core-cookie-authentication-50be9a385b38    <<<<<<

                //builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                //                .AddCookie("Cookies", options =>
                //                {
                //                    options.LoginPath = "/Account/Login";
                //                    options.LogoutPath = "/Account/Logout";
                //                    options.AccessDeniedPath = "/Account/AccessDenied";
                //                    options.ReturnUrlParameter = "ReturnUrl";
                //                })
                //                .AddJwtBearer();

                builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                                .AddCookie("Cookies", options =>
                                {
                                    options.LoginPath = "/Home/Index";                        // se non è stata effettuata la Login ridireziono su Home Page
                                    options.LogoutPath = "/Home/Index";
                                    options.AccessDeniedPath = "/Account/AccessDenied";
                                    options.ReturnUrlParameter = "ReturnUrl";
                                });
                //<<<authentication

                //>>>swagger
                builder.Services.AddGrpc().AddJsonTranscoding();
                builder.Services.AddGrpcSwagger();
                builder.Services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1",
                        new OpenApiInfo { Title = "gRPC transcoding", Version = "v1" });

                    // Scaffolds the folder to obtain documentation related to Controllers.
                    //!!!! var filePath = Path.Combine(System.AppContext.BaseDirectory, "ErpToolkit.xml");  //file che descrive le funzioni di XxxxController.cs per la generazione delle descrizioni swagger
                    foreach (System.Reflection.Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        try
                        {
                            if (!asm.GetTypes().Any(type => typeof(Controller).IsAssignableFrom(type))) throw new Exception("skip"); // skip if not XxxxController class
                            string XmlFile = Path.ChangeExtension(asm.Location, "xml");
                            if (File.Exists(XmlFile))
                            {
                                c.IncludeXmlComments(XmlFile);
                                c.IncludeGrpcXmlComments(XmlFile, includeControllerXmlComments: true);
                            }
                        }
                        catch (Exception ex) { } //Assembly.GetTypes() can throw in some cases. This skip and return only the types which were successfully loaded from the assembly.
                    }
                });
                //<<<swagger

                //// Add services to the container.
                var controllersWithViews = builder.Services.AddControllersWithViews();

                //!!!!!ATTENZIONE: NON FUNZIONA AUTOCOMPLETE CON QUESTE IMPOSTAZIONI
                ////>>> Configura JsonOptions per ignorare il riferimento ciclico con ReferenceHandler.Preserve
                //// Quando chiamo la funzione: System.Text.Json.JsonSerializer.Deserialize<T>((System.Text.Json.JsonElement)jsonObj, jsonOptionsConverters)
                //// per evitare l'errore: System.Text.Json.JsonException: 'The JSON value could not be converted to System.Text.Json.Serialization.ReferenceHandler. Path: $.ReferenceHandler | LineNumber: 0 | BytePositionInLine: 0.'
                //// Configura il ReferenceHandler per preservare i riferimenti ciclici
                //// Attenzione: questo non "risolve" il ciclo — lo gestisce introducendo $id e $ref nel JSON per mantenere i riferimenti. È utile per API, ma non per le View Razor, che non gestiscono $ref.
                //controllersWithViews.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                //});
                ////<<< Configura JsonOptions per ignorare il riferimento ciclico con ReferenceHandler.Preserve



                //>>> Configura JsonOptions per troncare il riferimento ciclico manualmente (senza ReferenceHandler.Preserve)

                //// System.Text.Json
                //builder.Services.AddControllersWithViews()
                //    .AddJsonOptions(options =>
                //    {
                //        //options.JsonSerializerOptions.TypeInfoResolver = new TruncatingTypeResolver(maxDepth: 2); // Imposta qui la profondità
                //        //options.JsonSerializerOptions.ReferenceHandler = null; // disattiva ReferenceHandler.Preserve che crea $id/$ref e può interferire con i TagHelper
                //        //                                                       //options.JsonSerializerOptions.Converters.Add(new ModelErpTruncateConverter(2)); // profondità 2

                //        // Usa direttamente la variabile statica
                //        options.JsonSerializerOptions.DefaultIgnoreCondition = DogManagerJson.jsonSerializerOptions.DefaultIgnoreCondition;
                //        options.JsonSerializerOptions.PropertyNamingPolicy = DogManagerJson.jsonSerializerOptions.PropertyNamingPolicy;
                //        options.JsonSerializerOptions.ReferenceHandler = DogManagerJson.jsonSerializerOptions.ReferenceHandler;

                //        // Copia i converter uno per uno
                //        foreach (var converter in DogManagerJson.jsonSerializerOptions.Converters)
                //        {
                //            options.JsonSerializerOptions.Converters.Add(converter);
                //        }

                //    });

                // NewtonsoftJson 
                builder.Services.AddControllersWithViews()
                    .AddNewtonsoftJson(options =>
                    {
                        // Copia le impostazioni dalla tua libreria
                        options.SerializerSettings.NullValueHandling = DogManagerNewtonsoftJson.jsonSerializerSettings.NullValueHandling;
                        options.SerializerSettings.ReferenceLoopHandling = DogManagerNewtonsoftJson.jsonSerializerSettings.ReferenceLoopHandling;
                        options.SerializerSettings.Formatting = DogManagerNewtonsoftJson.jsonSerializerSettings.Formatting;
                        //options.SerializerSettings.TypeNameHandling = DogManagerJson_Newtonsoft.jsonSerializerSettings.TypeNameHandling;
                        options.SerializerSettings.ContractResolver = DogManagerNewtonsoftJson.jsonSerializerSettings.ContractResolver;

                        // Copia anche i converter
                        foreach (var conv in DogManagerNewtonsoftJson.jsonSerializerSettings.Converters)
                        {
                            options.SerializerSettings.Converters.Add(conv);
                        }
                    });

                //<<< Configura JsonOptions per troncare il riferimento ciclico manualmente (senza ReferenceHandler.Preserve)





                //>>> Configura Razor per caricare le Views incorporate
                controllersWithViews.AddRazorRuntimeCompilation(options =>
                    {
                        // Aggiungi runtime compilation per Razor
                        var embeddedFileProvider = new EmbeddedFileProvider(ErpContext.Instance.AssemblyLIBRARY, "ErpToolkit");  //var embeddedFileProvider = new EmbeddedFileProvider(ErpContext.Instance.AssemblyLIBRARY, "ErpToolkit.Views");
                        options.FileProviders.Add(embeddedFileProvider);

                        //    var libraryPath = Path.GetDirectoryName(ErpContext.Instance.AssemblyLIBRARY.Location);
                        //    options.FileProviders.Add(new CompositeFileProvider(
                        //        new PhysicalFileProvider(libraryPath),
                        //        new EmbeddedFileProvider(ErpContext.Instance.AssemblyLIBRARY)
                        //    ));

                    });
                //<<< Configura Razor per caricare le Views incorporate

                //>>>manage session client
                // USE: @HttpContext.Session.GetString(IndexModel.SessionKeyName)
                //https://learn.microsoft.com/en-us/aspnet/core/fundamentals/app-state?view=aspnetcore-8.0

                builder.Services.AddDistributedMemoryCache();

                builder.Services.AddSession(options =>
                {
                    options.IdleTimeout = TimeSpan.FromMinutes(ErpContext.SessionMinuteTimeout); //la sessione client si cancella dopo 20 minuti   // TimeSpan.FromSeconds(10);
                    options.Cookie.HttpOnly = true;
                    options.Cookie.IsEssential = true;
                });
                //<<<manage session client

                // Registra IHttpContextAccessor
                builder.Services.AddHttpContextAccessor();


                //------------------------------------
                //------------------------------------
                //------------------------------------


                var app = builder.Build();

                //>>>Gestisci i file statici incorporati nella directory wwwroot
                app.UseEmbeddedStaticFiles(ErpContext.Instance.AssemblyLIBRARY, "wwwroot");  // Se i file statici venivano serviti direttamente da wwwroot, ora saranno disponibili sotto il prefisso /static. Ad esempio: /static/js/script.js /static/css/style.css
                //<<<Gestisci i file statici incorporati nella directory wwwroot

                //>>>swagger
                app.UseSwagger();
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwaggerUI(c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    });
                }
                //???//app.MapGrpcService<GreeterService>();
                //<<<swagger



                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                }
                app.UseStaticFiles();

                app.UseRouting();

                //>>>authentication
                app.UseAuthentication();  //per CookieAuthenticationDefaults
                app.UseAuthorization();
                //<<<authentication


                //>>>manage session client
                app.UseSession();
                //<<<manage session client

                //>>>Aggiungi il tuo middleware per loggare le richieste
                app.UseMiddleware<RequestLogging>();
                //<<<Aggiungi il tuo middleware per loggare le richieste

                //>>>pagina di default
                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=DefaultPage}/{id?}");
                //<<<


                ///////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////

                // VERIFICO CARICAMENTO VIEW 
                //var loadedViewAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                //    .Where(a => a.GetName().Name!.EndsWith(".Views")).ToArray();
                //_logger.Info($">>>Lista View caricate:");
                //foreach (var asm in loadedViewAssemblies) { _logger.Info($"assembly loaded: {asm.FullName}"); }
                //if (loadedViewAssemblies.Length == 0) { _logger.Info("ATTENZIONE: NESSUNA View Assembly caricata."); }
                string viewsPath = Path.Combine(app.Environment.ContentRootPath, "Views");
                _logger.Info($">>>Lista View caricate:");
                if (!Directory.Exists(viewsPath)) _logger.Info("ATTENZIONE: Cartella Views NON trovata!");
                else 
                {
                    var loadedView = Directory.GetFiles(viewsPath, "*.cshtml", SearchOption.AllDirectories);
                    foreach (var fname in loadedView) { _logger.Info($"file loaded: {fname}"); }
                }

                // VERIFICO CARICAMENTO CONTROLLER 
                var controllers = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes())
                    .Where(t => typeof(Controller).IsAssignableFrom(t)).ToList();
                _logger.Info($">>>Lista Controller caricati:");
                foreach (var c in controllers) { _logger.Info($"assembly loaded: {c.FullName}"); }

                // FORZO LETTURA MODELLO
                _logger.Info($">>>Lista Modelli caricati:\n");
                DogId dogId = new DogId(ErpContext.Instance.GetString("#defaultServerDOG"), ErpContext.Instance.GetString("#defaultDbRoot"));
                ErpContext.Instance.DogFactory.GetDog(dogId).dumpModel();

                // Log porte e URL del server
                app.Lifetime.ApplicationStarted.Register(() =>
                {
                    _logger.Info($"");
                    _logger.Info($"---------------------------------------------------------------------------------------- ");
                    _logger.Info($"--------- ");
                    _logger.Info($"---------> Server AVVIATO!!");
                    _logger.Info($"--------- ");
                    foreach (var url in app.Urls) { _logger.Info($"---------> Server listening on: {url}", url); }
                    _logger.Info($"--------- ");
                    _logger.Info($"---------> ContentRootPath: {app.Environment.ContentRootPath}");
                    _logger.Info($"---------> WebRootPath: {app.Environment.WebRootPath}");
                    _logger.Info($"--------- ");
                    _logger.Info($"---------------------------------------------------------------------------------------- \n");

                    //da questo momento in produzione disabilito l'output della console
                    if(!ErpContext.IsDevelopment)
                    {
                        Console.SetOut(TextWriter.Null);
                        Console.SetError(TextWriter.Null);
                    }

                });

                ///////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////
                ///////////////////////////////////////////////////////////////////////

                //app.Run();
                _cts = new CancellationTokenSource();
                _webTask = Task.Run(async () =>
                {
                    await app.RunAsync(_cts.Token);
                });


            }
            catch (Exception ex)
            {
                _logger.Error(new ErpConfigurationException(ex.Message));
            }
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _cts?.Cancel();
                if (_webTask != null)
                    await _webTask;
            }
            catch
            {
                // ignora eccezioni da cancellazione
            }
        }
        //public Task StopAsync(CancellationToken cancellationToken)
        //{
        //    return Task.CompletedTask;
        //}

    }

    public class RequestLogging
    {
        private readonly RequestDelegate _next;
        public RequestLogging(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // Logga le informazioni sulla richiesta
            Console.WriteLine($"Incoming Request: {context.Request.Method} {context.Request.Path}");

            // Logga altre informazioni utili, se necessario
            Console.WriteLine($"Headers: {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");

            // Chiamata al middleware successivo
            await _next(context);

            // Logga le informazioni sulla risposta
            Console.WriteLine($"Response Status Code: {context.Response.StatusCode}");
        }
    }

}
