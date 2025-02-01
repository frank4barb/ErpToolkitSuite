using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

namespace ErpToolkit.Extensions
{
    public static class EmbeddedStaticFilesExtensions
    {
        public static void UseEmbeddedStaticFiles(this IApplicationBuilder app, Assembly assembly, string rootNamespace)
        {
            // per richiamare i file presenti nella directory /wwwroot della DLL uso: 
            //     <p><img src="/wwwroot/ErpToolkitLogo.png"></p>
            try
            {
                var fileProvider = new ManifestEmbeddedFileProvider(assembly, rootNamespace);
                var options = new StaticFileOptions
                {
                    FileProvider = fileProvider,
                    RequestPath = "/wwwroot"
                };
                app.UseStaticFiles(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errore nel caricamento delle risorse incorporate: {ex.Message}");
                throw;
            }



        }

    }
}
