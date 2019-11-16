using System;
using App = System.Windows.Forms.Application;

namespace SpiderEye.UI.Windows
{
    internal class WinFormsApplication : IApplication
    {
        public bool ExitWithLastWindow { get; set; }

        public IUiFactory Factory { get; }

        public WinFormsApplication()
        {
            App.EnableVisualStyles();
            App.SetCompatibleTextRenderingDefault(false);

            ExitWithLastWindow = true;
            Factory = new WinFormsUiFactory();
        }

        public IMenu CreateAppMenu()
        {
            return null;
        }

        public IMenu CreateDefaultAppMenu()
        {
            return null;
        }

        public void Run()
        {
            App.Idle += App_Idle; // TODO: workaround to close app, should be handled better
            App.Run();
        }

        public void Exit()
        {
            App.Exit();
        }

        private void App_Idle(object sender, EventArgs e)
        {
            if (App.OpenForms.Count == 0 && ExitWithLastWindow)
            {
                Exit();
            }
        }
    }
}
