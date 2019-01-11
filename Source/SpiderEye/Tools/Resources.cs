using System.IO;
using System.Reflection;

namespace SpiderEye
{
    internal static class Resources
    {
        public static string GetInitScript(string type)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string seiName = $"{assembly.GetName().Name}.Scripts.SpiderEyeInterface.js";
            string name = $"{assembly.GetName().Name}.Scripts.Init{type}.js";

            return GetScript(assembly, seiName) + GetScript(assembly, name);
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
