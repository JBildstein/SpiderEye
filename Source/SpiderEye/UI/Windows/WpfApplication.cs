using System.Windows;

namespace SpiderEye.UI.Windows
{
    internal class WpfApplication : IApplication
    {
        public bool ExitWithLastWindow
        {
            get { return exitWithLastWindow; }
            set
            {
                exitWithLastWindow = value;
                application.ShutdownMode = value ?
                    ShutdownMode.OnLastWindowClose :
                    ShutdownMode.OnExplicitShutdown;
            }
        }

        public IUiFactory Factory { get; }

        private readonly System.Windows.Application application;
        private bool exitWithLastWindow = true;

        public WpfApplication()
        {
            Factory = new WpfUiFactory();
            application = new System.Windows.Application();
            application.ShutdownMode = ShutdownMode.OnLastWindowClose;
        }

        public void Run()
        {
            application.Run();
        }

        public void Exit()
        {
            application.Shutdown();
        }
    }
}
