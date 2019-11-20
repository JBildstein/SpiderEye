using System;
using System.Runtime.ExceptionServices;
#if !NET462
using System.Runtime.InteropServices;
#endif
using SpiderEye.Bridge;
using SpiderEye.UI;

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to create or run an application.
    /// </summary>
    public static partial class Application
    {
        /// <summary>
        /// Gets or sets a value indicating whether the application should exit once the last window is closed.
        /// Default is true.
        /// </summary>
        public static bool ExitWithLastWindow { get; set; } = true;

        /// <summary>
        /// Gets the operating system the app is currently running on.
        /// </summary>
        public static OperatingSystem OS { get; }

        /// <summary>
        /// Gets the UI factory.
        /// </summary>
        public static IUiFactory Factory { get; }

        /// <summary>
        /// Creates a new window.
        /// </summary>
        /// <param name="config">The window configuration.</param>
        /// <returns>The created window.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is null.</exception>
        public static IWindow CreateWindow(WindowConfiguration config)
        {
            return Factory.CreateWindow(config);
        }

        /// <summary>
        /// Creates a new status icon.
        /// </summary>
        /// <returns>The created status icon.</returns>
        public static IStatusIcon CreateStatusIcon()
        {
            return Factory.CreateStatusIcon();
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
            RunImpl();
        }

        /// <summary>
        /// Starts the main loop, shows the given window and blocks until the application exits.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> is null.</exception>
        public static void Run(IWindow window)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }

            using (window)
            {
                window.Show();
                RunImpl();
            }
        }

        /// <summary>
        /// Starts the main loop, loads the URL, shows the given window and blocks until the application exits.
        /// </summary>
        /// <param name="window">The window to show.</param>
        /// <param name="startUrl">The initial URL to load in the window.</param>
        /// <exception cref="ArgumentNullException"><paramref name="window"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="startUrl"/> is null.</exception>
        public static void Run(IWindow window, string startUrl)
        {
            if (window == null) { throw new ArgumentNullException(nameof(window)); }
            if (startUrl == null) { throw new ArgumentNullException(nameof(startUrl)); }

            using (window)
            {
                window.LoadUrl(startUrl);
                window.Show();
                RunImpl();
            }
        }

        /// <summary>
        /// Starts the main loop, creates a window, loads the URL, shows the window and blocks until the application exits.
        /// </summary>
        /// <param name="config">The window configuration.</param>
        /// <param name="startUrl">The initial URL to load in the window.</param>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="startUrl"/> is null.</exception>
        public static void Run(WindowConfiguration config, string startUrl)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            Run(Factory.CreateWindow(config), startUrl);
        }

        /// <summary>
        /// Exits the main loop and allows it to return.
        /// </summary>
        public static void Exit()
        {
            ExitImpl();
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
        /// Executes the given action on the UI main thread.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the given action.</returns>
        public static T Invoke<T>(Func<T> action)
        {
            if (action == null) { throw new ArgumentNullException(nameof(action)); }

            T result = default;
            InvokeSafely(() => result = action());
            return result;
        }

        private static void InvokeSafely(Action action)
        {
            ExceptionDispatchInfo exception = null;
            InvokeImpl(() =>
            {
                try { action(); }
                catch (Exception ex) { exception = ExceptionDispatchInfo.Capture(ex); }
            });

            exception?.Throw();
        }

        static partial void RunImpl();
        static partial void ExitImpl();
        static partial void InvokeImpl(Action action);

        private static OperatingSystem GetOS()
        {
#if NET462
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return OperatingSystem.Windows;
                case PlatformID.MacOSX:
                    return OperatingSystem.MacOS;
                case PlatformID.Unix:
                    return OperatingSystem.Linux;

                default:
                    throw new PlatformNotSupportedException();
            }
#else
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return OperatingSystem.Windows; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return OperatingSystem.MacOS; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return OperatingSystem.Linux; }
            else { throw new PlatformNotSupportedException(); }
#endif
        }

        private static void CheckOs(OperatingSystem expected)
        {
            if (OS != expected)
            {
                string msg = $"Wrong platform: using {expected} specific library on {OS}";
                throw new PlatformNotSupportedException(msg);
            }
        }
    }
}
