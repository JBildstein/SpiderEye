using System.Threading.Tasks;

namespace SpiderEye.Bridge
{
    /// <summary>
    /// Represents a bridge between the host and the webview.
    /// </summary>
    public interface IWebviewBridge
    {
        /// <summary>
        /// Adds a custom handler to be called from the webview.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        void AddHandler(object handler);

        /// <summary>
        /// Adds a custom handler to be called from any webview of the application.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        void AddGlobalHandler(object handler);

        /// <summary>
        /// Invoke an event in the webview.
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <param name="data">Optional event data.</param>
        void Invoke(string id, object data);

        /// <summary>
        /// Invoke an event in the webview and get the result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="id">The event ID.</param>
        /// <param name="data">Optional event data.</param>
        /// <returns>The result from the webview.</returns>
        T Invoke<T>(string id, object data);

        /// <summary>
        /// Asynchronously invoke an event in the webview.
        /// </summary>
        /// <param name="id">The event ID.</param>
        /// <param name="data">Optional event data.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task InvokeAsync(string id, object data);

        /// <summary>
        /// Asynchronously invoke an event in the webview and get the result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <param name="id">The event ID.</param>
        /// <param name="data">Optional event data.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<T> InvokeAsync<T>(string id, object data);
    }
}
