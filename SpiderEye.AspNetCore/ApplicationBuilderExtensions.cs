using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace SpiderEye.AspNetCore
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSpiderEye(this IApplicationBuilder builder, string contentFolder)
        {
            return builder.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new ManifestEmbeddedFileProvider(Assembly.GetEntryAssembly(), contentFolder),
                ServeUnknownFileTypes = true,
            });
        }
    }
}
