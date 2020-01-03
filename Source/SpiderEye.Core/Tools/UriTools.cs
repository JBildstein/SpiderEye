using System.Security.Cryptography;

namespace SpiderEye.Tools
{
    internal static class UriTools
    {
        private static readonly RandomNumberGenerator Rng = RandomNumberGenerator.Create();

        public static string GetRandomResourceUrl(string scheme)
        {
            return $"{scheme}://resources.{CreateRandomString(8)}.internal";
        }

        private static string CreateRandomString(int length)
        {
            const string possible = "0123456789abcdefghijklmnopqrstuvwxyz";
            const int range = 35; // == possible.Length - 1
            const byte mask = 0b111111; // bits needed for range

            char[] result = new char[length];
            byte[] buffer = new byte[1];

            for (int i = 0; i < result.Length; i++)
            {
                int charPos;
                do
                {
                    Rng.GetBytes(buffer);
                    charPos = mask & buffer[0];
                }
                while (charPos > range);

                result[i] = possible[charPos];
            }

            return new string(result);
        }
    }
}
