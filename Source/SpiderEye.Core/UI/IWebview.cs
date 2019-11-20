using System;
using System.Threading.Tasks;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a webview.
    /// </summary>
    internal interface IWebview : IDisposable
    {
        /// <summary>
        /// Fires once the content in this webview has loaded.
        /// </summary>
        event PageLoadEventHandler PageLoaded;

        /// <summary>
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        void NavigateToFile(string url);

        /// <summary>
        /// Executes the given JavaScript within the webview and gets the result.
        /// </summary>
        /// <param name="script">The JavaScript to execute.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the result of the script.</returns>
        Task<string> ExecuteScriptAsync(string script);
    }
}
