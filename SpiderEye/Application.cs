using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SpiderEye
{
    public static class Application
    {
        public static IApplication Create(AppConfiguration config)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CreateApp("SpiderEye.Windows", config);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CreateApp("SpiderEye.Linux", config);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return CreateApp("SpiderEye.Mac", config);
            }
            else { throw new PlatformNotSupportedException(); }
        }

        private static IApplication CreateApp(string name, AppConfiguration config)
        {
            var appType = typeof(IApplication);
            var entryAssembly = Assembly.GetEntryAssembly();

            string path = Path.Combine(Path.GetDirectoryName(entryAssembly.Location), name + ".dll");
            var assembly = Assembly.LoadFile(path);

            if (assembly != null)
            {
                var type = assembly.GetTypes()
                    .FirstOrDefault(t => t.GetInterfaces()
                        .Contains(appType));

                if (type != null) { return Activator.CreateInstance(type, config) as IApplication; }
            }

            throw new TypeLoadException("Could not find assembly with SpiderEye application");
        }
    }
}
