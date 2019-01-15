using Xunit;

namespace SpiderEye.Tools
{
    public class JsToolsTests
    {
        [Theory]
        [InlineData("FooBar", "FooBar")]
        [InlineData("fooBar", "FooBar")]
        public void NormalizeToDotnetName_WithValidValues_ReturnsNormlizedName(string value, string expected)
        {
            string result = JsTools.NormalizeToDotnetName(value);

            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("fooBar", "fooBar")]
        [InlineData("FooBar", "fooBar")]
        public void NormalizeToJsName_WithValidValues_ReturnsNormlizedName(string value, string expected)
        {
            string result = JsTools.NormalizeToJsName(value);

            Assert.Equal(expected, result);
        }
    }
}
