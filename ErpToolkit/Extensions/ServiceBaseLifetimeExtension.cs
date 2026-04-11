using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace ErpToolkit.Extensions
{
    //public static class ServiceBaseLifetimeExtension
    //{
    //    public static IHostBuilder UseServiceLifetime(this IHostBuilder hostBuilder)
    //    {
    //        return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceLifeTime>());
    //    }

    //    public static Task RunTheServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
    //    {
    //        return hostBuilder.UseServiceLifetime().Build().RunAsync(cancellationToken);
    //    }
    //}


    public static class ServiceBaseLifetimeExtension
    {
        private static Action<string>? _stopCallback;

        // Program.cs registra questa callback
        public static void RegisterStopCallback(Action<string> callback)
        {
            _stopCallback = callback;
        }

        public static IHostBuilder UseServiceLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) =>
                services.AddSingleton<IHostLifetime, ServiceLifeTime>());
        }

        public static async Task RunTheServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            var host = hostBuilder.UseServiceLifetime().Build();

            var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

            // STOPPING -> il servizio sta iniziando a fermarsi
            lifetime.ApplicationStopping.Register(() =>
            {
                _stopCallback?.Invoke("STOPPING");
            });

            // STOP -> il servizio è fermo
            lifetime.ApplicationStopped.Register(() =>
            {
                _stopCallback?.Invoke("STOP");
            });

            await host.RunAsync(cancellationToken);
        }
    }


}
