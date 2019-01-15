using System;

namespace SpiderEye.Json
{
    [Flags]
    internal enum JsonValueType
    {
        Object = 1 << 0,
        Array = 1 << 1,
        String = 1 << 2,
        Int = 1 << 3,
        Float = 1 << 4,
        Bool = 1 << 5,
        Null = 1 << 6,
        DateTime = 1 << 7,
    }
}
