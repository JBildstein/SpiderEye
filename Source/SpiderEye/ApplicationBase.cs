using System;
using SpiderEye.Configuration;
using SpiderEye.UI;

namespace SpiderEye
{
    internal abstract class ApplicationBase : IApplication
    {
        public abstract IWindow MainWindow { get; }

        public abstract IWindowFactory Factory { get; }

        protected readonly AppConfiguration config;

        public ApplicationBase(AppConfiguration config)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public void Run()
        {
            Run(true);
        }

        public void Run(bool showWindow)
        {
            try
            {
                MainWindow.LoadUrl(config.StartPageUrl);
                if (showWindow) { MainWindow.Show(); }

                RunMainLoop();
            }
            finally
            {
                MainWindow.Dispose();
            }
        }

        public abstract void Exit();

        protected abstract void RunMainLoop();
    }
}
