using System;
using SpiderEye.Scripting.Api;
using SpiderEye.Server;

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

            server = new ContentServer(config.ContentAssembly, config.ContentFolder, config.Port);
        }

        public virtual void Run()
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
        }

        public abstract void Exit();
    }
}
