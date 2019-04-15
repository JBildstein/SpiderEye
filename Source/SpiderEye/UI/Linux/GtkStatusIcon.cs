using System;
using System.Reflection;
using SpiderEye.UI.Linux.Interop;
using SpiderEye.UI.Linux.Menu;
using SpiderEye.UI.Linux.Native;

namespace SpiderEye.UI.Linux
{
    internal class GtkStatusIcon : IStatusIcon
    {
        public string Title
        {
            get { return GLibString.FromPointer(AppIndicator.GetTitle(Handle)); }
            set
            {
                using (GLibString title = value)
                {
                    AppIndicator.SetTitle(Handle, title);
                }
            }
        }

        public Icon Icon { get; set; }

        public readonly IntPtr Handle;

        public GtkStatusIcon()
        {
            using (GLibString id = GetAppId())
            using (GLibString icon = "applications-accessories") // TODO: set icon properly
            {
                Handle = AppIndicator.Create(id, icon, AppIndicatorCategory.ApplicationStatus);
            }
        }

        public IMenu AddMenu()
        {
            var menu = new GtkMenu();
            AppIndicator.SetMenu(Handle, menu.Handle);

            return menu;
        }

        public void Dispose()
        {
            AppIndicator.Dispose(Handle);
        }

        private string GetAppId()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }
    }
}
