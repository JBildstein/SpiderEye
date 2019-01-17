namespace SpiderEye.UI.Linux.Interop
{
    internal enum WebKitInjectionTime
    {
        /// <summary>
        /// Insert the code of the user script at the beginning of loaded documents. This is the default.
        /// </summary>
        DocumentStart,

        /// <summary>
        /// Insert the code of the user script at the end of the loaded documents.
        /// </summary>
        DocumentEnd,
    }
}
