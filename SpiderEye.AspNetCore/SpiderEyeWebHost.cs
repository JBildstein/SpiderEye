using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SpiderEye.AspNetCore
{
    /// <summary>
    /// Provides methods to start a SpiderEye application with an ASP.Net Core server.
    /// </summary>
    public class SpiderEyeWebHost
    {
        /// <summary>
        /// Runs the application with the given configuration.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        public static void Run(AppConfiguration config)
        {
            string environment = EnvironmentName.Production;
            SetDevEnvironment(ref environment); // only called in Debug mode

            Run(config, null, environment);
        }

        /// <summary>
        /// Runs the application with the given configuration.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <param name="args">The command line arguments.</param>
        public static void Run(AppConfiguration config, string[] args)
        {
            string environment = EnvironmentName.Production;
            SetDevEnvironment(ref environment); // only called in Debug mode

            Run(config, args, environment);
        }

        /// <summary>
        /// Runs the application with the given configuration.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <param name="args">The command line arguments.</param>
        /// <param name="environment">The environment to use. Use values from <see cref="EnvironmentName"/>.</param>
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

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/> with default values for a SpiderEye app.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <returns>The created <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateDefaultBuilder(AppConfiguration config)
        {
            string environment = EnvironmentName.Production;
            SetDevEnvironment(ref environment); // only called in Debug mode

            return CreateDefaultBuilder(config, null, environment);
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/> with default values for a SpiderEye app.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The created <see cref="IWebHostBuilder"/>.</returns>
        public static IWebHostBuilder CreateDefaultBuilder(AppConfiguration config, string[] args)
        {
            string environment = EnvironmentName.Production;
            SetDevEnvironment(ref environment); // only called in Debug mode

            return CreateDefaultBuilder(config, args, environment);
        }

        /// <summary>
        /// Creates a <see cref="IWebHostBuilder"/> with default values for a SpiderEye app.
        /// </summary>
        /// <param name="config">The application configuration.</param>
        /// <param name="args">The command line arguments.</param>
        /// <param name="environment">The environment to use. Use values from <see cref="EnvironmentName"/>.</param>
        /// <returns>The created <see cref="IWebHostBuilder"/>.</returns>
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

        [Conditional("DEBUG")]
        private static void SetDevEnvironment(ref string environment)
        {
            environment = EnvironmentName.Development;
        }
    }
}
