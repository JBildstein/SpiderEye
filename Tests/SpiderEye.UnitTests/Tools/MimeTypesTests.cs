using System;
using Xunit;

namespace SpiderEye.Tools
{
    public class MimeTypesTests
    {
        [Theory]
        [InlineData(null, "application/octet-stream")]
        [InlineData("http://foo.bar/index.html", "text/html")]
        [InlineData("http://foo.bar/baz/index.js", "application/javascript")]
        [InlineData("http://foo.bar/index.js.zip", "application/zip")]
        public void FindForUri_WithUri_ReturnsMimeType(string value, string expected)
        {
            Uri uri = null;
            if (value != null) { uri = new Uri(value); }

            string result = MimeTypes.FindForUri(uri);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "application/octet-stream")]
        [InlineData("", "application/octet-stream")]
        [InlineData("  ", "application/octet-stream")]
        [InlineData("C:\\Foo\\Bar\\index.html", "text/html")]
        [InlineData("C:\\Foo\\Bar\\Baz\\index.js", "application/javascript")]
        [InlineData("C:\\Foo\\Bar\\index.js.zip", "application/zip")]
        public void FindForFile_WithPath_ReturnsMimeType(string value, string expected)
        {
            string result = MimeTypes.FindForFile(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, "application/octet-stream")]
        [InlineData("", "application/octet-stream")]
        [InlineData("  ", "application/octet-stream")]
        [InlineData(".html", "text/html")]
        [InlineData("html", "text/html")]
        [InlineData(".js", "application/javascript")]
        public void FindForExtension_WithExtension_ReturnsMimeType(string value, string expected)
        {
            string result = MimeTypes.FindForExtension(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("http://foo.bar/index.html", "text/html", true)]
        [InlineData("http://foo.bar/baz/index.js", "application/javascript", true)]
        [InlineData("http://foo.bar/index.js.zip", "application/zip", true)]
        public void TryFindForUri_WithUri_ReturnsMimeType(string value, string expected, bool expectedFound)
        {
            Uri uri = null;
            if (value != null) { uri = new Uri(value); }

            bool found = MimeTypes.TryFindForUri(uri, out string result);

            Assert.Equal(expected, result);
            Assert.Equal(expectedFound, found);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", null, false)]
        [InlineData("  ", null, false)]
        [InlineData("C:\\Foo\\Bar\\index.html", "text/html", true)]
        [InlineData("C:\\Foo\\Bar\\Baz\\index.js", "application/javascript", true)]
        [InlineData("C:\\Foo\\Bar\\index.js.zip", "application/zip", true)]
        public void TryFindForFile_WithPath_ReturnsMimeType(string value, string expected, bool expectedFound)
        {
            bool found = MimeTypes.TryFindForFile(value, out string result);

            Assert.Equal(expected, result);
            Assert.Equal(expectedFound, found);
        }

        [Theory]
        [InlineData(null, null, false)]
        [InlineData("", null, false)]
        [InlineData("  ", null, false)]
        [InlineData(".html", "text/html", true)]
        [InlineData("html", "text/html", true)]
        [InlineData(".js", "application/javascript", true)]
        public void TryFindForExtension_WithExtension_ReturnsMimeType(string value, string expected, bool expectedFound)
        {
            bool found = MimeTypes.TryFindForExtension(value, out string result);

            Assert.Equal(expected, result);
            Assert.Equal(expectedFound, found);
        }
    }
}
