using System;
using System.Threading;
using System.Windows.Forms;
using App = System.Windows.Forms.Application;

namespace SpiderEye.Windows
{
    internal class WinFormsApplication : IApplication
    {
        public IUiFactory Factory { get; }

        public SynchronizationContext SynchronizationContext { get; }

        public ContentServer ContentServer
        {
            get
            {
                if (server == null)
                {
                    server = new ContentServer();
                    server.Start();
                }

                return server;
            }
        }

        private ContentServer server;

        public WinFormsApplication()
        {
            App.EnableVisualStyles();
            App.SetCompatibleTextRenderingDefault(false);

            Factory = new WinFormsUiFactory();
            SynchronizationContext = new WindowsFormsSynchronizationContext();

            Application.OpenWindows.AllWindowsClosed += Application_AllWindowsClosed;
        }

        public void Run()
        {
            App.Run();
        }

        public void Exit()
        {
            App.Exit();
            server?.Dispose();
        }

        private void Application_AllWindowsClosed(object sender, EventArgs e)
        {
            if (Application.ExitWithLastWindow) { Exit(); }
        }
    }
}
