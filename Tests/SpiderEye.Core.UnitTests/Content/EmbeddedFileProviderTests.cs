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
            Exception exception = Record.Exception(() => new EmbeddedContentProvider("\\Resources", null));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Fact]
        public void Constructor_WithContentFolderNull_ThrowsException()
        {
            Exception exception = Record.Exception(() => new EmbeddedContentProvider(null, Assembly.GetExecutingAssembly()));

            Assert.IsType<ArgumentNullException>(exception);
        }

        [Theory]
        [InlineData("http://foo.bar/SomeFile.txt", "Resource Content Text")]
        [InlineData("http://foo.bar/soMefILe.txt", "Resource Content Text")]
        [InlineData("http://foo.bar/Nested/SomeOtherFile.txt", "Other Resource Content Text")]
        public async Task GetStreamAsync_WithExistingUri_ReturnsStream(string url, string expected)
        {
            var provider = Create();
            var uri = new Uri(url);

            using (var stream = await provider.GetStreamAsync(uri))
            {
                Assert.NotNull(stream);

                using (var reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    Assert.Equal(expected, result);
                }
            }
        }

        [Theory]
        [InlineData("http://foo.bar/nonexistent.txt")]
        public async Task GetStreamAsync_WithNonExistingUri_ReturnsNull(string url)
        {
            var provider = Create();
            var uri = new Uri(url);

            using (var stream = await provider.GetStreamAsync(uri))
            {
                Assert.Null(stream);
            }
        }


        private EmbeddedContentProvider Create()
        {
            return new EmbeddedContentProvider("Content\\Resources", Assembly.GetExecutingAssembly());
        }
    }
}
