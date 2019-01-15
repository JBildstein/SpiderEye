using System;
#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace SpiderEye
{
    /// <summary>
    /// Provides methods to create or run an application.
    /// </summary>
    public static class Application
    {
        /// <summary>
        /// Creates a new application for the current operating system.
        /// </summary>
        /// <param name="config">The app configuration.</param>
        /// <returns>The created application.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is null.</exception>
        public static IApplication Create(AppConfiguration config)
        {
            if (config == null) { throw new ArgumentNullException(nameof(config)); }

            if (IsWindows())
            {
#if NET462
                return new UI.Windows.WpfApplication(config);
#elif NETSTANDARD2_0
                throw new PlatformNotSupportedException("Windows is only supported on .Net 4.6.2 or newer");
#else
#error Unknown target runtime
#endif
            }
            else if (IsLinux())
            {
                return new UI.Linux.GtkApplication(config);
            }
            else if (IsMac())
            {
                return new UI.Mac.CocoaApplication(config);
            }
            else { throw new PlatformNotSupportedException(); }
        }

        /// <summary>
        /// Creates a new application for the current operating system and runs it.
        /// </summary>
        /// <param name="config">The app configuration.</param>
        /// <exception cref="ArgumentNullException"><paramref name="config"/> is null.</exception>
        public static void Run(AppConfiguration config)
        {
            Create(config).Run();
        }

        private static bool IsWindows()
        {
#if NET462
            return Environment.OSVersion.Platform == PlatformID.Win32NT;
#elif NETSTANDARD2_0
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
#else
#error Unknown target runtime
#endif
        }

        private static bool IsLinux()
        {
#if NET462
            return Environment.OSVersion.Platform == PlatformID.Unix;
#elif NETSTANDARD2_0
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
#else
#error Unknown target runtime
#endif
        }

        private static bool IsMac()
        {
#if NET462
            return Environment.OSVersion.Platform == PlatformID.MacOSX;
#elif NETSTANDARD2_0
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
#else
#error Unknown target runtime
#endif
        }
    }
}
