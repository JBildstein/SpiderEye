using System;

namespace SpiderEye.Native
{
    public static class SpiderEyeInitializer
    {
        public static void Init()
        {
            throw new InvalidOperationException("looks like the native library is not linked correctly.");
        }
    }
}
