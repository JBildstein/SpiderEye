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
    }
}
