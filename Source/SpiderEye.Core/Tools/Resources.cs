using System.IO;
using System.Reflection;

namespace SpiderEye
{
    internal static class Resources
    {
        public static string GetInitScript(string type)
        {
            var coreAssembly = Assembly.GetExecutingAssembly();
            var platformAssembly = Assembly.GetCallingAssembly();
            string bridge = $"{coreAssembly.GetName().Name}.Scripts.SpiderEyeBridge.js";
            string init = $"{platformAssembly.GetName().Name}.Scripts.Init{type}.js";
            string ready = $"{coreAssembly.GetName().Name}.Scripts.ReadyEvent.js";

            return string.Concat(
                GetScript(coreAssembly, bridge),
                GetScript(platformAssembly, init),
                GetScript(coreAssembly, ready));
        }

        private static string GetScript(Assembly assembly, string manifestName)
        {
            using (var stream = assembly.GetManifestResourceStream(manifestName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
