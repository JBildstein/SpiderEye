#pragma warning disable SA1602, CS1591 // Enumeration items should be documented

using System;

namespace SpiderEye
{
    /// <summary>
    /// Operating system.
    /// </summary>
    [Flags]
    public enum OperatingSystem
    {
        Windows = 1 << 0,
        MacOS = 1 << 1,
        Linux = 1 << 2,
    }
}
