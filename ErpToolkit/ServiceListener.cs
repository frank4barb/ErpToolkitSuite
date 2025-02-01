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


namespace ErpToolkit
{
    public class ServiceListener : IHostedService
    {
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



                var builder = WebApplication.CreateBuilder(); //var builder = WebApplication.CreateBuilder(args);

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


                app.Run();


            }
            catch (Exception ex)
            {
                _logger.Error(new ErpConfigurationException(ex.Message));
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

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
