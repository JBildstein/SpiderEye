using SpiderEye.UI;

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
        /// Gets the window factory.
        /// </summary>
        IWindowFactory Factory { get; }

        /// <summary>
        /// Runs this application and shows the main window. This call blocks until the application exits.
        /// </summary>
        void Run();

        /// <summary>
        /// Runs this application and optionally shows the main window. This call blocks until the application exits.
        /// </summary>
        /// <param name="showWindow">True to show the main window automatically; False to do it yourself.</param>
        void Run(bool showWindow);

        /// <summary>
        /// Closes this application and allows <see cref="Run()"/> to return.
        /// </summary>
        void Exit();
    }
}
