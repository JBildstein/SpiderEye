using System;

namespace SpiderEye.Tools
{
    internal static class UriExtensions
    {
        public static string WithoutScheme(this Uri uri)
        {
            int schemeLength = uri.GetLeftPart(UriPartial.Scheme).Length;
            return uri.OriginalString.Substring(schemeLength);
        }
    }
}
