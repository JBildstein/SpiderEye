using System;
#if NETSTANDARD2_0
using System.Runtime.InteropServices;
#endif

namespace SpiderEye
{
    public static class Application
    {
        public static IApplication Create(AppConfiguration config)
        {
            if (IsWindows())
            {
#if NET462
                return new Windows.WpfApplication(config);
#elif NETSTANDARD2_0
                throw new PlatformNotSupportedException("Windows is only supported on .Net 4.6.2 or newer");
#else
#error Unknown target runtime
#endif
            }
            else if (IsLinux())
            {
                return new Linux.GtkApplication(config);
            }
            else if (IsMac())
            {
                return new Mac.CocoaApplication(config);
            }
            else { throw new PlatformNotSupportedException(); }

            throw new NotImplementedException();
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
