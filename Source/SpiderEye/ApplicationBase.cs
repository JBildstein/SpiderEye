using System;
using SpiderEye.Scripting.Api;
using SpiderEye.Server;
using SpiderEye.Server.Middleware;

namespace SpiderEye
{
    internal abstract class ApplicationBase : IApplication
    {
        public abstract IWindow MainWindow { get; }
        public abstract IWebview Webview { get; }

        protected readonly AppConfiguration config;
        protected readonly ContentServer server;

        public ApplicationBase(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));

            server = new ContentServer(config.Port);

            server.RegisterMiddleware(new EmbeddedFileMiddleware(config.ContentAssembly, config.ContentFolder));
            server.RegisterMiddleware(new ControllerMiddleware());
        }

        public void Run()
        {
            try
            {
                ApiResolver.InitApi();

                server.Start();

                if (string.IsNullOrWhiteSpace(config.Host))
                {
                    config.Host = server.HostAddress;
                }

                string url = new Uri(new Uri(config.Host), config.StartPageUrl).ToString();
                Webview.LoadUrl(url);
                MainWindow.Show();

                RunMainLoop();
            }
            finally
            {
                Webview.Dispose();
                MainWindow.Dispose();
                server.Dispose();
            }
        }

        public abstract void Exit();

        protected abstract void RunMainLoop();
    }
}
