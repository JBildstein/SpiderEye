using SpiderEye.Linux.Native;

namespace SpiderEye.Linux
{
    internal class GtkSeparatorMenuItem : GtkMenuItem
    {
        public GtkSeparatorMenuItem()
            : base(Gtk.Menu.CreateSeparatorItem())
        {
        }
    }
}
