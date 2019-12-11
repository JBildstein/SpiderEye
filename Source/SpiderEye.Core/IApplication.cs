using System.Threading;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to manage and  run an application.
    /// </summary>
    internal interface IApplication
    {
        /// <summary>
        /// Gets the UI factory.
        /// </summary>
        IUiFactory Factory { get; }

        /// <summary>
        /// Gets the synchronization context.
        /// </summary>
        SynchronizationContext SynchronizationContext { get; }

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
