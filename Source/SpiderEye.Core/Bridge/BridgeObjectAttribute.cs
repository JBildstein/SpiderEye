using System;

namespace SpiderEye.Bridge
{
    /// <summary>
    /// Provides additional metadata for a bridge object.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class BridgeObjectAttribute : Attribute
    {
        internal readonly string Path;

        /// <summary>
        /// Initializes a new instance of the <see cref="BridgeObjectAttribute"/> class.
        /// </summary>
        /// <param name="path">The path used to access the bridge object from the webview.</param>
        public BridgeObjectAttribute(string path)
        {
            Path = path;
        }
    }
}
