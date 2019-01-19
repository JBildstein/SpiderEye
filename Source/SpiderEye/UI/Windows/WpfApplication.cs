using System;
using SpiderEye.Configuration;

namespace SpiderEye.UI.Windows
{
    internal class WpfApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        private readonly System.Windows.Application application;
        private readonly WpfWindow window;

        public WpfApplication(AppConfiguration config)
            : base(config)
        {
            window = new WpfWindow(config);
            window.WindowReady += Window_WindowReady;

            application = new System.Windows.Application();
            application.MainWindow = window;
        }

        public override void Exit()
        {
            application.Shutdown();
        }

        protected override void RunMainLoop()
        {
            MainWindow.Show();
            application.Run();
        }

        private void Window_WindowReady(object sender, EventArgs e)
        {
            MainWindow.LoadUrl(config.StartPageUrl);
        }
    }
}
