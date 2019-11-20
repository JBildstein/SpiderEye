using Xunit;

namespace SpiderEye.Tools
{
    public class ColorToolsTests
    {
        [Theory]
        [InlineData("#FFFFFF", 255, 255, 255)]
        [InlineData("#000000", 0, 0, 0)]
        [InlineData("#7F7F7F", 127, 127, 127)]
        [InlineData("#291BC8", 41, 27, 200)]
        [InlineData("#123", 255, 255, 255)]
        [InlineData(null, 255, 255, 255)]
        [InlineData("", 255, 255, 255)]
        [InlineData("  ", 255, 255, 255)]
        public void ParseFromHex_WithHexValue_ReturnsChannels(string hex, byte re, byte ge, byte be)
        {
            ColorTools.ParseHex(hex, out byte r, out byte g, out byte b);

            Assert.Equal(re, r);
            Assert.Equal(ge, g);
            Assert.Equal(be, b);
        }
    }
}
