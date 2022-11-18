using System;
using System.Runtime.ExceptionServices;
using System.Threading;
#if !NET462
using System.Runtime.InteropServices;
#endif
using SpiderEye.Bridge;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to create or run an application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application should exit once the last window is closed.
        /// Default is true.
        /// </summary>
        public static bool ExitWithLastWindow { get; set; }

        /// <summary>
        /// Gets a collection of windows that are currently open.
        /// </summary>
        public static WindowCollection OpenWindows { get; }

        /// <summary>
        /// Gets or sets the domain to use as the custom host when loading webview files.
        /// </summary>
        public static string? CustomHostDomain { get; set; }

        /// <summary>
        /// Gets or sets the content provider for loading webview files.
        /// </summary>
        public static IContentProvider ContentProvider
        {
            get { return contentProvider; }
            set { contentProvider = value ?? NoopContentProvider.Instance; }
        }

        /// <summary>
        /// Gets or sets the URI watcher to check URIs before they are loaded.
        /// </summary>
        public static IUriWatcher UriWatcher
        {
            get { return uriWatcher; }
            set { uriWatcher = value ?? NoopUriWatcher.Instance; }
        }

        /// <summary>
        /// Gets the operating system the app is currently running on.
        /// </summary>
        public static OperatingSystem OS { get; }

        /// <summary>
        /// Gets the UI factory.
        /// </summary>
        internal static IUiFactory Factory
        {
            get
            {
                CheckInitialized();
                return app.Factory;
            }
        }

        private static IApplication app = null!;
        private static IContentProvider contentProvider;
        private static IUriWatcher uriWatcher;

        static Application()
        {
            OS = GetOS();
            ExitWithLastWindow = true;
            contentProvider = NoopContentProvider.Instance;
            uriWatcher = NoopUriWatcher.Instance;
            OpenWindows = new WindowCollection();
        }

        /// <summary>
        /// Adds a custom handler to be called from any webview of the application.
        /// </summary>
        /// <param name="handler">The handler instance.</param>
        public static void AddGlobalHandler(object handler)
        {
            WebviewBridge.AddGlobalHandlerStatic(handler);
        }

        /// <summary>
        /// Starts the main loop and blocks until the application exits.
        /// </summary>
        public static void Run()
        {
            CheckInitialized();

            app.Run();
        }

        /// <summary>
        /// Starts the main loop, shows the given window and blocks until the application exits.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> is null.</exception>
        public static void Run(Window window)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }

            window.Show();
            Run();
        }

        /// <summary>
        /// Starts the main loop, loads the URL, shows the given window and blocks until the application exits.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <param name="startUrl">The initial URL to load in the window.</param>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="startUrl"/> is null.</exception>
        public static void Run(Window window, string startUrl)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }
            if (startUrl == null) { throw new ArgumentNullException(nameof(startUrl)); }

            window.LoadUrl(startUrl);
            window.Show();
            Run();
        }

        /// <summary>
        /// Exits the main loop and allows it to return.
        /// </summary>
        public static void Exit()
        {
            CheckInitialized();

            app.Exit();
        }

        /// <summary>
        /// Executes the given action on the UI main thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void Invoke(Action action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            InvokeSafely(action);
        }

        /// <summary>
        /// Executes the given function on the UI main thread.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="function">The function to execute.</param>
        /// <returns>The result of the given function.</returns>
        public static T Invoke<T>(Func<T> function)
        {
            if (function == null) { throw new ArgumentNullException(nameof(function)); }

            T result = default!;
            InvokeSafely(() => result = function());
            return result!;
        }

        /// <summary>
        /// Checks if the current operating system is correct.
        /// </summary>
        /// <param name="application">The application OS specific implementation.</param>
        /// <param name="applicationOS">The operating system the implementation is made for.</param>
        internal static void Register(IApplication application, OperatingSystem applicationOS)
        {
            if (OS != applicationOS)
            {
                string msg = $"Wrong platform: using {applicationOS} specific library on {OS}";
                throw new PlatformNotSupportedException(msg);
            }

            app = application ?? throw new ArgumentNullException(nameof(application));

            SynchronizationContext.SetSynchronizationContext(app.SynchronizationContext);
        }

        private static void InvokeSafely(Action action)
        {
            CheckInitialized();

            ExceptionDispatchInfo? exception = null;
            app.SynchronizationContext.Send(
                state =>
                {
                    try { action(); }
                    catch (Exception ex) { exception = ExceptionDispatchInfo.Capture(ex); }
                },
                null);

            exception?.Throw();
        }

        private static void CheckInitialized()
        {
            if (app == null)
            {
                string platform = OS switch
                {
                    OperatingSystem.Windows => "Windows",
                    OperatingSystem.MacOS => "Mac",
                    OperatingSystem.Linux => "Linux",
                    _ => throw new PlatformNotSupportedException(),
                };
                string message = $"Application has not been initialized yet. Call {platform}Application.Init() first.";
                throw new InvalidOperationException(message);
            }
        }

        private static OperatingSystem GetOS()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return OperatingSystem.Windows; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return OperatingSystem.MacOS; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return OperatingSystem.Linux; }
            else { throw new PlatformNotSupportedException(); }
        }
    }
}
