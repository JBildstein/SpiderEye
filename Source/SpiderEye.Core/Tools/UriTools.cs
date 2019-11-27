using System;

namespace SpiderEye.Tools
{
    internal static class UriTools
    {
        private static readonly Random Rng = new Random(); // TODO: use crypto RNG

        public static string GetRandomResourceUrl(string scheme)
        {
            return $"{scheme}://resources.{CreateRandomString(8)}.internal";
        }

        private static string CreateRandomString(int length)
        {
            const string possible = "0123456789abcdefghijklmnopqrstuvwxyz";
            char[] result = new char[length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = possible[Rng.Next(0, possible.Length)];
            }

            return new string(result);
        }
    }
}
