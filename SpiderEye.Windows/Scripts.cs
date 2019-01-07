using System.IO;
using System.Reflection;

namespace SpiderEye.Windows
{
    internal static class Scripts
    {
        public static string GetScript(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            name = $"{assembly.GetName().Name}.{name}";
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
