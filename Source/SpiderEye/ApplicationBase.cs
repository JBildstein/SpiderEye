using System;
using SpiderEye.Server;

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
            server = new ContentServer(config.ContentAssembly, config.ContentFolder);
        }

        public virtual void Run()
        {
            server.Start();

            if (string.IsNullOrWhiteSpace(config.Host))
            {
                config.Host = server.HostAddress;
            }

            MainWindow.Webview.LoadUrl(config.StartPageUrl);
            MainWindow.Show();
        }

        public abstract void Exit();
    }
}
