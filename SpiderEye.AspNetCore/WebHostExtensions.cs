using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace SpiderEye.AspNetCore
{
    public static class WebHostExtensions
    {
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
