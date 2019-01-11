using System.Threading.Tasks;
using SpiderEye.Tools.Scripting;

namespace SpiderEye
{
    /// <summary>
    /// Represents a webview.
    /// </summary>
    internal interface IWebview
    {
        /// <summary>
        /// Gets the script handler.
        /// </summary>
        ScriptHandler ScriptHandler { get; }

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

        /// <summary>
        /// Executes the given JavaScript function and gets the result.
        /// </summary>
        /// <param name="function">The function to call.</param>
        /// <returns>A <see cref="Task{TResult}"/> with the result of the function.</returns>
        Task<string> CallFunction(string function);
    }
}
