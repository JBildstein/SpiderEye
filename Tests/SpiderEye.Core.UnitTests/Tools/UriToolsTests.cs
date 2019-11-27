using System;
using Xunit;

namespace SpiderEye.Tools
{
    public class UriToolsTests
    {
        [Fact]
        public void GetRandomResourceUrl_WithScheme_ReturnsResourceUrl()
        {
            string result = UriTools.GetRandomResourceUrl("somescheme");

            Assert.Matches(@"somescheme://resources\.[0-9a-z]{8}\.internal", result);
        }
    }
}
