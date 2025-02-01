using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.Extensions.DependencyInjection;
using ErpToolkit.Helpers;

namespace ErpToolkit.Extensions
{
    //public static class EmbeddedResourcesExtensions
    //{
    //    // consente di rendere disponibili risorse embedded nella libreria 
    //    //...
    //    //...// Configura l'accesso alle risorse embedded dalla DLL
    //    //...services.ConfigureEmbeddedResources();
    //    //...
    //    public static IServiceCollection ConfigureEmbeddedResources(
    //        this IServiceCollection services)
    //    {
    //        // Configura MVC per utilizzare le viste embedded
    //        services.Configure<MvcRazorRuntimeCompilationOptions>(options =>
    //        {
    //            var libraryPath = Path.GetDirectoryName(ErpContext.Instance.AssemblyLIBRARY.Location);

    //            options.FileProviders.Add(new CompositeFileProvider(
    //                new PhysicalFileProvider(libraryPath),
    //                new EmbeddedFileProvider(ErpContext.Instance.AssemblyLIBRARY)
    //            ));
    //        });

    //        return services;
    //    }
    //}
}
