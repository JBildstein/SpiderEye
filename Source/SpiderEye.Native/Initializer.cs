using System;

namespace SpiderEye.Native
{
    /// <summary>
    /// Cross platform initializer for SpiderEye apps.
    /// </summary>
    public static class SpiderEyeInitializer
    {
        /// <summary>
        /// Initializes the application.
        /// </summary>
        /// <exception cref="InvalidOperationException">If no native binary is linked.</exception>
        public static void Init()
        {
            throw new InvalidOperationException("looks like the native library is not linked correctly.");
        }
    }
}
