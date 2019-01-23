using System;

namespace SpiderEye.Tools
{
    internal static class UriTools
    {
        public static Uri Combine(string host, string path)
        {
            if (!host.EndsWith("/")) { host += "/"; }
            path = path.TrimStart('/');

            return new Uri(new Uri(host), path);
        }

        public static string GetRandomResourceUrl(string scheme)
        {
            return $"{scheme}://resources.{CreateRandomString(8)}.internal";
        }

        private static string CreateRandomString(int length)
        {
            const string possible = "0123456789abcdefghijklmnopqrstuvwxyz";
            var rand = new Random();
            char[] result = new char[length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = possible[rand.Next(0, possible.Length)];
            }

            return new string(result);
        }
    }
}
