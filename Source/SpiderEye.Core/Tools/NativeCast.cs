using System;

namespace SpiderEye.Tools
{
    /// <summary>
    /// Provides methods to handle casts to the native type.
    /// e.g. to cast a core interface to it's platform specific type.
    /// </summary>
    internal static class NativeCast
    {
        /// <summary>
        /// Casts a general instance into the platform native type.
        /// If the type does not match, it'll throw the appropriate exception.
        /// </summary>
        /// <typeparam name="T">The native type.</typeparam>
        /// <param name="item">The item to cast.</param>
        /// <returns>The cast item.</returns>
        public static T To<T>(object item)
            where T : class
        {
            if (item == null) { return null; }
            else if (item is T actual) { return actual; }
            else
            {
                throw new InvalidOperationException($"Item is not the expected native type of {typeof(T).Name}");
            }
        }
    }
}
