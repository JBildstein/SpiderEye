using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace SpiderEye.Content
{
    public class EmbeddedFileProviderTests
    {
        [Fact]
        public void Constructor_WithAssemblyNull_ThrowsException()
        {
            Exception exception = Record.Exception(() => new EmbeddedFileProvider(null, "\\Resources"));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://foo.bar/SomeFile.txt", "Resource Content Text")]
        [InlineData("http://foo.bar/soMefILe.txt", "Resource Content Text")]
        [InlineData("http://foo.bar/Nested/SomeOtherFile.txt", "Other Resource Content Text")]
        [InlineData("http://foo.bar/nonexistent.txt", null)]
        public async Task GetStreamAsync_WithExistingUri_ReturnsStream(string url, string expected)
        {
            var provider = Create();
            var uri = new Uri(url);

            using (var stream = await provider.GetStreamAsync(uri))
            {
                if (expected == null) { Assert.Null(stream); }
                else
                {
                    Assert.NotNull(stream);

                    using (var reader = new StreamReader(stream))
                    {
                        string result = reader.ReadToEnd();
                        Assert.Equal(expected, result);
                    }
                }
            }
        }


        private EmbeddedFileProvider Create()
        {
            return new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "Content\\Resources");
        }
    }
}
