using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpiderEye.AspNetCore
{
    public class SpiderEyeWebHost
    {
        public static void Run(AppConfiguration config)
        {
            Run(config, null, EnvironmentName.Production);
        }

        public static void Run(AppConfiguration config, string[] args)
        {
            Run(config, args, EnvironmentName.Production);
        }

        public static void Run(AppConfiguration config, string[] args, string environment)
        {
            if (!string.IsNullOrWhiteSpace(config.Host))
            {
                Application.Create(config).Run();
            }
            else
            {
                CreateDefaultBuilder(config, args, environment)
                    .Build()
                    .RunSpiderEye(config);
            }
        }

        public static IWebHostBuilder CreateDefaultBuilder(AppConfiguration config, string[] args, string environment)
        {
            if (environment == null) { throw new ArgumentNullException(nameof(environment)); }

            var builder = new WebHostBuilder();
            builder.UseEnvironment(environment);

            if (string.IsNullOrEmpty(builder.GetSetting(WebHostDefaults.ContentRootKey)))
            {
                builder.UseContentRoot(Directory.GetCurrentDirectory());
            }

            if (args != null)
            {
                builder.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
            }

            builder.ConfigureAppConfiguration((context, appConfig) =>
            {
                if (args != null) { appConfig.AddCommandLine(args); }
            });

            builder.UseDefaultServiceProvider((context, options) =>
            {
                options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
            });

            builder.UseUrls("http://127.0.0.1:0");
            builder.UseKestrel();

            builder.Configure(appBuilder =>
            {
                if (!string.IsNullOrWhiteSpace(config.ContentFolder))
                {
                    appBuilder.UseSpiderEye(config.ContentFolder);
                }
            });

            return builder;
        }
    }
}
