using System;
using System.Threading;
using System.Windows.Forms;
using SpiderEye.UI.Windows;
using App = System.Windows.Forms.Application;

namespace SpiderEye
{
    /// <content>
    /// Windows specific implementations.
    /// </content>
    public static partial class Application
    {
        private static readonly SynchronizationContext Context;

        static Application()
        {
            OS = GetOS();
            CheckOs(OperatingSystem.Windows);

            App.EnableVisualStyles();
            App.SetCompatibleTextRenderingDefault(false);

            App.Idle += App_Idle; // TODO: workaround to close app, should be handled better

            Factory = new WinFormsUiFactory();
            Context = new WindowsFormsSynchronizationContext();
        }

        static partial void RunImpl()
        {
            App.Run();
        }

        static partial void ExitImpl()
        {
            App.Exit();
        }

        static partial void InvokeImpl(Action action)
        {
            Context.Send(state => action(), null);
        }

        private static void App_Idle(object sender, EventArgs e)
        {
            if (App.OpenForms.Count == 0 && ExitWithLastWindow)
            {
                Exit();
            }
        }
    }
}
