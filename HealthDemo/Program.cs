using ErpToolkit;
using ErpToolkit.Extensions;
using ErpToolkit.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Mysqlx.Crud;
using System;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;



namespace HealthDemo
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                bool isStartup = true; StreamWriter fileWriter = null;
                bool isService = WindowsServiceHelpers.IsWindowsService();  //var isDebugging = Debugger.IsAttached || args.Contains("--console");
                if (isService) //disattiva ouput su console 
                {
                    if (isStartup)
                    {
                        var logPath = Path.Combine(AppContext.BaseDirectory, "startup.log");
                        Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                        fileWriter = new StreamWriter(logPath, append: true)
                        {
                            AutoFlush = true
                        };
                        Console.SetOut(fileWriter);
                        Console.SetError(fileWriter); // opzionale, ma consigliato
                    }
                    else
                    {
                        Console.SetOut(TextWriter.Null);
                        Console.SetError(TextWriter.Null);
                    }

                }
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] \n\n\n\n\n\n");
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ********** Starting Windows Service *************************************************************");

                ErpContext.Init(Assembly.GetExecutingAssembly()); // Init Erp Model before start services
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ErpContext.Init");

                var builder = Host.CreateDefaultBuilder(args)
                    .UseContentRoot(ErpContext.CurrentDirectory)
                    .ConfigureServices((context, services) =>
                    {
                        //services.AddHostedService<ServiceScheduler>();    //attiva solo se serve lo scheduler
                        services.AddHostedService<ServiceListener>();       //attiva solo se serve il listener
                    });

                if (!isService)
                {
                    Console.WriteLine("Modalità DEBUG / CONSOLE attiva...");
                    await builder.RunConsoleAsync();
                }
                else
                {
                    Console.WriteLine("Modalità Windows Service...");
                    builder.UseWindowsService();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] UseWindowsService");

                    // Intercetta gli eventi di stop del servizio per loggare lo stato
                    ServiceBaseLifetimeExtension.RegisterStopCallback((status) =>
                    {
                        if (fileWriter != null)
                        {
                            fileWriter.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] >>> {status}");
                            fileWriter.Flush();
                        }
                    });



                    await builder.RunTheServiceAsync();  //await builder.Build().RunAsync();
                    Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] RunTheServiceAsync");

                }
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] End");

            }
            catch (Exception ex)
            {
                // ERRORE ALL'AVVIO: Mostra il messaggio di errore nella console
                Console.WriteLine($"Errore: {ex.Message}");
                try { NLog.LogManager.GetCurrentClassLogger().Error(ex); } catch { }

                // Esci dal programma
                Environment.Exit(1);
            }
            Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Exit");
        }
    }
}


