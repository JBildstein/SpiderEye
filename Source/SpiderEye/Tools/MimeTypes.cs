using System;
using System.Collections.Generic;
using System.IO;

namespace SpiderEye.Tools
{
    /// <summary>
    /// Contains methods to find the mime type for file extensions.
    /// </summary>
    public static class MimeTypes
    {
        /// <summary>
        /// The JSON mime type string.
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// The HTML mime type string.
        /// </summary>
        public const string Html = "text/html";

        /// <summary>
        /// The JavaScript mime type string.
        /// </summary>
        public const string JavaScript = "application/javascript";

        /// <summary>
        /// The binary data mime type string.
        /// </summary>
        public const string Binary = "application/octet-stream";

        /// <summary>
        /// The file extension to mime type map. Add or remove values depending on your need.
        /// Note that the extension must start with a period "." and be in lower case.
        /// </summary>
        public static readonly Dictionary<string, string> Map = new Dictionary<string, string>()
        {
            { ".7z", "application/x-7z-compressed" },
            { ".aac", "audio/aac" },
            { ".avi", "video/x-msvideo" },
            { ".bmp", "image/bmp" },
            { ".css", "text/css" },
            { ".csv", "text/csv" },
            { ".dtd", "text/xml" },
            { ".flac", "audio/flac" },
            { ".flv", "video/x-flv" },
            { ".gif", "image/gif" },
            { ".gz", "application/x-gzip" },
            { ".htm", "text/html" },
            { ".html", Html },
            { ".ico", "image/x-icon" },
            { ".jpe", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".js", JavaScript },
            { ".json", Json },
            { ".m4a", "audio/m4a" },
            { ".midi", "audio/mid" },
            { ".mp3", "audio/mpeg" },
            { ".mp4", "video/mp4" },
            { ".mpeg", "video/mpeg" },
            { ".mpg", "video/mpeg" },
            { ".oga", "audio/ogg" },
            { ".ogg", "audio/ogg" },
            { ".ogv", "video/ogg" },
            { ".pdf", "application/pdf" },
            { ".png", "image/png" },
            { ".rar", "application/x-rar-compressed" },
            { ".sitemap", "application/xml" },
            { ".svg", "image/svg+xml" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" },
            { ".ttf", "application/font-sfnt" },
            { ".txt", "text/plain" },
            { ".wasm", "application/wasm" },
            { ".wav", "audio/wav" },
            { ".webm", "video/webm" },
            { ".webp", "image/webp" },
            { ".woff", "application/font-woff" },
            { ".woff2", "application/font-woff2" },
            { ".xhtml", "application/xhtml+xml" },
            { ".xml", "text/xml" },
            { ".zip", "application/zip" },
        };

        /// <summary>
        /// Find the mime type for the given URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The found mime type. If the type was not found, "application/octet-stream" is used.</returns>
        public static string FindForUri(Uri uri)
        {
            if (TryFindForUri(uri, out string mime)) { return mime; }
            else { return Binary; }
        }

        /// <summary>
        /// Find the mime type for the given filename.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <returns>The found mime type. If the type was not found, "application/octet-stream" is used.</returns>
        public static string FindForFile(string file)
        {
            if (TryFindForFile(file, out string mime)) { return mime; }
            else { return Binary; }
        }

        /// <summary>
        /// Find the mime type for the given filename.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <returns>The found mime type. If the type was not found, "application/octet-stream" is used.</returns>
        public static string FindForExtension(string extension)
        {
            if (TryFindForExtension(extension, out string mime)) { return mime; }
            else { return Binary; }
        }

        /// <summary>
        /// Find the mime type for the given URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="mimeType">The mime type if found or null otherwise.</param>
        /// <returns>True if the mime type was found; False otherwise.</returns>
        public static bool TryFindForUri(Uri uri, out string mimeType)
        {
            if (uri == null)
            {
                mimeType = null;
                return false;
            }

            string path = uri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            return TryFindForFile(path, out mimeType);
        }

        /// <summary>
        /// Find the mime type for the given filename.
        /// </summary>
        /// <param name="file">The filename.</param>
        /// <param name="mimeType">The mime type if found or null otherwise.</param>
        /// <returns>True if the mime type was found; False otherwise.</returns>
        public static bool TryFindForFile(string file, out string mimeType)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                mimeType = null;
                return false;
            }

            string extension = Path.GetExtension(file).ToLower();
            return TryFindForExtension(extension, out mimeType);
        }

        /// <summary>
        /// Find the mime type for the given filename.
        /// </summary>
        /// <param name="extension">The file extension.</param>
        /// <param name="mimeType">The mime type if found or null otherwise.</param>
        /// <returns>True if the mime type was found; False otherwise.</returns>
        public static bool TryFindForExtension(string extension, out string mimeType)
        {
            if (string.IsNullOrWhiteSpace(extension))
            {
                mimeType = null;
                return false;
            }

            if (!extension.StartsWith(".")) { extension = '.' + extension; }

            return Map.TryGetValue(extension.ToLower(), out mimeType);
        }
    }
}
