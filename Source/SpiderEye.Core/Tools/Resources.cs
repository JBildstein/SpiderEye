using System.IO;
using System.Reflection;

namespace SpiderEye
{
    internal static class Resources
    {
        public static string GetInitScript(string type)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string bridge = $"{assembly.GetName().Name}.Scripts.SpiderEyeBridge.js";
            string init = $"{assembly.GetName().Name}.Scripts.Init{type}.js";
            string ready = $"{assembly.GetName().Name}.Scripts.ReadyEvent.js";

            return string.Concat(
                GetScript(assembly, bridge),
                GetScript(assembly, init),
                GetScript(assembly, ready));
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
