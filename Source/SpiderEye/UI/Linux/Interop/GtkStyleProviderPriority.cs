namespace SpiderEye.UI.Linux.Interop
{
    internal enum GtkStyleProviderPriority : uint
    {
        Fallback = 1,
        Theme = 200,
        Settings = 400,
        Application = 600,
        User = 800,
    }
}
