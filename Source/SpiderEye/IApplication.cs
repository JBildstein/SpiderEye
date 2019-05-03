using SpiderEye.UI;

namespace SpiderEye
{
    /// <summary>
    /// Represents an application.
    /// </summary>
    internal interface IApplication
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application should exit once the last window is closed.
        /// Default is true.
        /// </summary>
        bool ExitWithLastWindow { get; set; }

        /// <summary>
        /// Gets the UI factory.
        /// </summary>
        IUiFactory Factory { get; }

        /// <summary>
        /// Creates and adds an empty app menu.
        /// </summary>
        /// <returns>The app menu.</returns>
        IMenu CreateAppMenu();

        /// <summary>
        /// Creates and adds an app menu with default values.
        /// </summary>
        /// <returns>The app menu.</returns>
        IMenu CreateDefaultAppMenu();

        /// <summary>
        /// Starts the main loop and blocks until the application exits.
        /// </summary>
        void Run();

        /// <summary>
        /// Exits the main loop and allows it to return.
        /// </summary>
        void Exit();
    }
}
