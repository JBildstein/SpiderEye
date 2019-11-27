using System;

namespace SpiderEye.Linux.Interop
{
    [Flags]
    internal enum GtkDialogFlags
    {
        Modal = 1 << 0,
        DestroyWithParent = 1 << 1,
        UseHeaderBar = 1 << 2,
    }
}
