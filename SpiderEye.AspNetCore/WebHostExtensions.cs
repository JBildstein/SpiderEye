using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace SpiderEye.AspNetCore
{
    /// <summary>
    /// Contains methods for running a SpiderEye app.
    /// </summary>
    public static class WebHostExtensions
    {
        /// <summary>
        /// Creates a new SpiderEye application and runs it.
        /// </summary>
        /// <param name="host">The web host.</param>
        /// <param name="config">The application configuration.</param>
        public static void RunSpiderEye(this IWebHost host, AppConfiguration config)
        {
            host.Start();

            var features = host.ServerFeatures.Get<IServerAddressesFeature>();
            config.Host = features.Addresses.FirstOrDefault();

            Application.Create(config).Run();
            host.StopAsync().GetAwaiter().GetResult();
        }
    }
}
