namespace SpiderEye
{
    /// <summary>
    /// Represents an application.
    /// </summary>
    public interface IApplication
    {
        /// <summary>
        /// Gets the main window.
        /// </summary>
        IWindow MainWindow { get; }

        /// <summary>
        /// Runs this application. This call blocks until the application exits.
        /// </summary>
        void Run();

        /// <summary>
        /// Closes this application and allows <see cref="Run"/> to return.
        /// </summary>
        void Exit();
    }
}
