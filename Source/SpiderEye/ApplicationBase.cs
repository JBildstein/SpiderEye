using System;
using SpiderEye.Configuration;
using SpiderEye.Scripting.Api;
using SpiderEye.Server;
using SpiderEye.Server.Middleware;
using SpiderEye.UI;

namespace SpiderEye
{
    internal abstract class ApplicationBase : IApplication
    {
        public abstract IWindow MainWindow { get; }

        protected readonly AppConfiguration config;
        protected readonly ContentServer server;

        public ApplicationBase(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            if (config.Server.UseInternalServer)
            {
                server = new ContentServer(config.Server.Port);

                server.RegisterMiddleware(new EmbeddedFileMiddleware(config.Server.ContentAssembly, config.Server.ContentFolder));
                server.RegisterMiddleware(new ControllerMiddleware(config.Server.Controllers));
            }
        }

        public void Run()
        {
            try
            {
                ApiResolver.InitApi();

                bool hasHostAddress = !string.IsNullOrWhiteSpace(config.Server.Host);
                if (config.Server.UseInternalServer)
                {
                    server.Start();

                    if (!hasHostAddress) { config.Server.Host = server.HostAddress; }
                }
                else if (!hasHostAddress) { throw new InvalidOperationException("Can't load page without host address."); }

                MainWindow.Show();

                RunMainLoop();
            }
            finally
            {
                MainWindow.Dispose();
                server?.Dispose();
            }
        }

        public abstract void Exit();

        protected abstract void RunMainLoop();

        protected void LoadStartPage()
        {
            string url = new Uri(new Uri(config.Server.Host), config.Server.StartPageUrl).ToString();
            MainWindow.LoadUrl(url);
        }
    }
}
