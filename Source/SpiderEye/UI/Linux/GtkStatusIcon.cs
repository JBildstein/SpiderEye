using System;
using System.IO;
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

        public AppIcon Icon
        {
            get { return icon; }
            set
            {
                icon = value;
                UpdateIcon(value);
            }
        }

        public readonly IntPtr Handle;
        private AppIcon icon;
        private string tempIconFile;

        public GtkStatusIcon()
        {
            using (GLibString id = GetAppId())
            {
                Handle = AppIndicator.Create(id, IntPtr.Zero, AppIndicatorCategory.ApplicationStatus);
                AppIndicator.SetTitle(Handle, id);
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
            ClearTempFile();
        }

        private string GetAppId()
        {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        private void UpdateIcon(AppIcon icon)
        {
            string tempPath = null;

            if (icon == null || icon.Icons.Length == 0)
            {
                AppIndicator.SetIcon(Handle, IntPtr.Zero);
            }
            else
            {
                string path;
                if (icon.Source == AppIcon.IconSource.File) { path = icon.DefaultIcon.Path; }
                else
                {
                    tempPath = Path.GetTempFileName();
                    using (var tmpStream = File.Open(tempPath, FileMode.Create))
                    using (var iconStream = icon.GetIconDataStream(icon.DefaultIcon))
                    {
                        iconStream.CopyTo(tmpStream);
                    }

                    path = tempPath;
                }

                using (GLibString gpath = path)
                {
                    AppIndicator.SetIcon(Handle, gpath);
                }
            }

            ClearTempFile();
            tempIconFile = tempPath;
        }

        private void ClearTempFile()
        {
            if (tempIconFile != null)
            {
                try { File.Delete(tempIconFile); }
                catch { /* don't care if not deleted, it's just a temp file */ }
            }
        }
    }
}
