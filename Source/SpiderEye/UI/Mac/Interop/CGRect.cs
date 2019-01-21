using System.Runtime.InteropServices;

namespace SpiderEye.UI.Mac.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct CGRect
    {
        public static readonly CGRect Zero = default;

        public CGPoint Origin;
        public CGSize Size;

        public CGRect(double x, double y, double width, double height)
        {
            Origin = new CGPoint(x, y);
            Size = new CGSize(width, height);
        }
    }
}
