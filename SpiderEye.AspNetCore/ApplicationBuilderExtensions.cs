using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;

namespace SpiderEye.AspNetCore
{
    /// <summary>
    /// Contains methods for settings up a SpiderEye app.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Sets up a file provider from embedded files.
        /// </summary>
        /// <param name="builder">The application builder.</param>
        /// <param name="contentFolder">The folder where the embedded files are located.</param>
        /// <returns>The application builder from <paramref name="builder"/>.</returns>
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
