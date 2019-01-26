using SpiderEye.Configuration;

namespace SpiderEye.UI.Windows
{
    internal class WpfApplication : ApplicationBase
    {
        public override IWindow MainWindow
        {
            get { return window; }
        }

        public override IWindowFactory WindowFactory
        {
            get { return factory; }
        }

        private readonly System.Windows.Application application;
        private readonly WpfWindow window;
        private readonly WpfWindowFactory factory;

        public WpfApplication(AppConfiguration config)
            : base(config)
        {
            factory = new WpfWindowFactory(config);
            window = new WpfWindow(config);

            application = new System.Windows.Application();
            application.MainWindow = window;
        }

        public override void Exit()
        {
            application.Shutdown();
        }

        protected override void RunMainLoop()
        {
            application.Run();
        }
    }
}
