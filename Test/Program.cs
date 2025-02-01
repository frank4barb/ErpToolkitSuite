using ErpToolkit;
using ErpToolkit.Extensions;
using ErpToolkit.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Test
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                ErpContext.Init(Assembly.GetExecutingAssembly()); // Init Erp Model before start services
                var isDebugging = !(Debugger.IsAttached || args.Contains("--console"));
                var hostBuilder = new HostBuilder()
                    .ConfigureServices((context, services) =>
                    {

                        //services.AddHostedService<ServiceScheduler>();    //attiva solo se serve lo scheduler
                        services.AddHostedService<ServiceListener>();       //attiva solo se serve il listener

                    });
                if (isDebugging)
                {
                    await hostBuilder.RunTheServiceAsync();
                }
                else
                {
                    await hostBuilder.RunConsoleAsync();
                }
            }
            catch (Exception ex)
            {
                // ERRORE ALL'AVVIO: Mostra il messaggio di errore nella console
                Console.WriteLine($"Errore: {ex.Message}");

                // Esci dal programma
                Environment.Exit(1);
            }
        }
    }
}
