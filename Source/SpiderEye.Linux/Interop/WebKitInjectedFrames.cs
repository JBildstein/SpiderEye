namespace SpiderEye.Linux.Interop
{
    internal enum WebKitInjectedFrames
    {
        /// <summary>
        /// Insert the user content in all of the frames loaded
        /// by the web view, including nested frames. This is the default.
        /// </summary>
        AllFrames,

        /// <summary>
        /// Insert the user content *only* in the top-level frame loaded by
        /// the web view, and *not* in the nested frames.
        /// </summary>
        TopFrame,
    }
}
