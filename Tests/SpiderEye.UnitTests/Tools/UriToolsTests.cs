using System;
using Xunit;

namespace SpiderEye.Tools
{
    public class UriToolsTests
    {
        [Fact]
        public void Combine_WithHostNull_ThrowsException()
        {
            Exception exception = Record.Exception(() => UriTools.Combine(null, "path.html"));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Combine_WithPathNull_ThrowsException()
        {
            Exception exception = Record.Exception(() => UriTools.Combine("http://foo.bar", null));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://foo.bar/baz", "path.html", "http://foo.bar/baz/path.html")]
        [InlineData("http://foo.bar/baz/", "/path.html", "http://foo.bar/baz/path.html")]
        [InlineData("http://foo.bar/baz", "/path.html", "http://foo.bar/baz/path.html")]
        [InlineData("http://foo.bar/baz/", "path.html", "http://foo.bar/baz/path.html")]
        public void Combine_WithValidInput_ReturnsCombinedUri(string host, string path, string expected)
        {
            Uri result = UriTools.Combine(host, path);

            Assert.Equal(expected, result.OriginalString);
        }

        [Fact]
        public void GetRandomResourceUrl_WithScheme_ReturnsResourceUrl()
        {
            string result = UriTools.GetRandomResourceUrl("somescheme");

            Assert.Matches(@"somescheme://resources\.[0-9a-z]{8}\.internal", result);
        }
    }
}
