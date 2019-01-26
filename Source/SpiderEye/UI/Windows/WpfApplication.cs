using SpiderEye.Configuration;

namespace SpiderEye.UI.Windows
{
    internal class WpfApplication : ApplicationBase
    {
        public override IWindow MainWindow { get; }
        public override IWindowFactory Factory { get; }

        private readonly System.Windows.Application application;

        public WpfApplication(AppConfiguration config)
            : base(config)
        {
            Factory = new WpfWindowFactory(config);
            var window = new WpfWindow(config, Factory);
            MainWindow = window;

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
