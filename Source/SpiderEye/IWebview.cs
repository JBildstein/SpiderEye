namespace SpiderEye
{
    /// <summary>
    /// Represents a webview.
    /// </summary>
    public interface IWebview
    {
        /// <summary>
        /// Loads the given URL relative to <see cref="AppConfiguration.Host"/>.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        void LoadUrl(string url);

        /// <summary>
        /// Executes the given JavaScript within the webview.
        /// </summary>
        /// <param name="script">The JavaScript to execute.</param>
        void ExecuteScript(string script);
    }
}
