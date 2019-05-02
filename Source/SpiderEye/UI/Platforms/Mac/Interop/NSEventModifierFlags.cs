namespace SpiderEye.UI.Platforms.Mac.Interop
{
    internal enum NSEventModifierFlags : uint
    {
        None = 0,
        Shift = 1 << 17,
        Control = 1 << 18,
        Option = 1 << 19,
        Command = 1 << 20,
    }
}
