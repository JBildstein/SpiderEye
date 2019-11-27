using System;
using System.IO;
using SpiderEye.Linux.Interop;
using SpiderEye.Linux.Native;
using SpiderEye.Tools;

namespace SpiderEye.Linux
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

        public Menu Menu
        {
            get { return menu; }
            set
            {
                menu = value;
                UpdateMenu(value);
            }
        }

        public readonly IntPtr Handle;
        private const string DefaultIconName = "applications-other";
        private AppIcon icon;
        private Menu menu;
        private string tempIconFile;

        public GtkStatusIcon(string title)
        {
            // TODO: allow setting App ID and AppIndicatorCategory
            using (GLibString id = $"com.{title}.app")
            using (GLibString icon = DefaultIconName)
            {
                Handle = AppIndicator.Create(id, icon, AppIndicatorCategory.ApplicationStatus);
                Title = title;
            }
        }

        public void Dispose()
        {
            ClearTempFile();
        }

        private void UpdateIcon(AppIcon icon)
        {
            string tempPath = null;

            string path;
            if (icon == null || icon.Icons.Length == 0) { path = DefaultIconName; }
            else if (icon.Source == AppIcon.IconSource.File) { path = icon.DefaultIcon.Path; }
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

            ClearTempFile();
            tempIconFile = tempPath;
        }

        private void UpdateMenu(Menu menu)
        {
            var nativeMenu = NativeCast.To<GtkMenu>(menu?.NativeMenu);
            AppIndicator.SetMenu(Handle, nativeMenu?.Handle ?? IntPtr.Zero);
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
