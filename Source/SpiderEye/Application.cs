using System;
using System.Runtime.ExceptionServices;
#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif
using SpiderEye.Bridge;
using SpiderEye.UI;

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
        public static bool ExitWithLastWindow
        {
            get { return Instance.ExitWithLastWindow; }
            set { Instance.ExitWithLastWindow = value; }
        }

        /// <summary>
        /// Gets the operating system the app is currently running on.
        /// </summary>
        public static OperatingSystem OS { get; }

        /// <summary>
        /// Gets the UI factory.
        /// </summary>
        public static IUiFactory Factory
        {
            get { return Instance.Factory; }
        }

        private static readonly IApplication Instance;
        private static IMenu appMenu;

        static Application()
        {
            OS = GetOS();
            Instance = CreateInstance();
            CreateDefaultAppMenu();
        }

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
        /// Creates and adds an empty app menu.
        /// </summary>
        /// <returns>The app menu.</returns>
        public static IMenu CreateAppMenu()
        {
            return appMenu = Instance.CreateAppMenu();
        }

        /// <summary>
        /// Creates and adds an app menu with default values.
        /// </summary>
        /// <returns>The app menu.</returns>
        public static IMenu CreateDefaultAppMenu()
        {
            return appMenu = Instance.CreateDefaultAppMenu();
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
            Instance.Run();
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
                Instance.Run();
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
                Instance.Run();
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
            Instance.Exit();
        }

        /// <summary>
        /// Executes the given action on the UI main thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public static void Invoke(Action action)
        {
            InvokeBase(action);
        }

        /// <summary>
        /// Executes the given action on the UI main thread.
        /// </summary>
        /// <typeparam name="T">The type of the return value.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result of the given action.</returns>
        public static T Invoke<T>(Func<T> action)
        {
            T result = default;
            InvokeBase(() => result = action());
            return result;
        }

        private static void InvokeBase(Action action)
        {
            ExceptionDispatchInfo exception = null;
            Instance.Invoke(() =>
            {
                try { action(); }
                catch (Exception ex) { exception = ExceptionDispatchInfo.Capture(ex); }
            });

            exception?.Throw();
        }

        private static IApplication CreateInstance()
        {
            switch (OS)
            {
                case OperatingSystem.Windows:
#if NET462
                    return new UI.Windows.WinFormsApplication();
#else
                    throw new PlatformNotSupportedException("Windows is only supported on .Net 4.6.2 or newer");
#endif

                case OperatingSystem.MacOSX:
                    return new UI.Mac.CocoaApplication();

                case OperatingSystem.Unix:
                    return new UI.Linux.GtkApplication();

                default:
                    throw new PlatformNotSupportedException();
            }
        }

        private static OperatingSystem GetOS()
        {
#if NET462
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    return OperatingSystem.Windows;
                case PlatformID.MacOSX:
                    return OperatingSystem.MacOSX;
                case PlatformID.Unix:
                    return OperatingSystem.Unix;

                default:
                    throw new PlatformNotSupportedException();
            }
#elif NETSTANDARD2_0
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return OperatingSystem.Windows; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return OperatingSystem.MacOSX; }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return OperatingSystem.Unix; }
            else { throw new PlatformNotSupportedException(); }
#else
#error Unknown target runtime
#endif
        }
    }
}
