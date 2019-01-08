using System.IO;
using System.Reflection;

namespace SpiderEye
{
    internal static class Scripts
    {
        public static string GetScript(string os, string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            name = $"{assembly.GetName().Name}.{os}.Scripts.{name}";
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
