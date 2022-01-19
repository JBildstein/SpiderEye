namespace SpiderEye.Tools
{
    internal static class JsTools
    {
        public static string NormalizeToDotnetName(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsUpper(name[0])) { return name; }

            return char.ToUpper(name[0]) + name[1..];
        }

        public static string NormalizeToJsName(string name)
        {
            if (string.IsNullOrEmpty(name) || char.IsLower(name[0])) { return name; }

            return char.ToLower(name[0]) + name[1..];
        }
    }
}
