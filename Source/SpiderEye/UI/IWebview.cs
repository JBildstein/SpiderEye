using System;
using System.Threading.Tasks;
using SpiderEye.Scripting;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents a webview.
    /// </summary>
    internal interface IWebview : IDisposable
    {
        /// <summary>
        /// Gets the script handler.
        /// </summary>
        ScriptHandler ScriptHandler { get; }

        /// <summary>
        /// Loads the given URL.
        /// </summary>
        /// <param name="url">The URL to load.</param>
        void NavigateToFile(string url);

        /// <summary>
        /// Executes the given JavaScript within the webview.
        /// </summary>
        /// <param name="script">The JavaScript to execute.</param>
        /// <returns>The result of the script.</returns>
        string ExecuteScript(string script);

        /// <summary>
        /// Executes the given JavaScript within the webview and gets the result.
        /// </summary>
        /// <param name="script">The JavaScript to execute.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the result of the script.</returns>
        Task<string> ExecuteScriptAsync(string script);
    }
}
