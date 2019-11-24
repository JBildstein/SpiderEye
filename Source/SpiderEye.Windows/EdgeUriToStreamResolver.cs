using System;
using System.IO;
using System.Threading.Tasks;
using SpiderEye.Content;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace SpiderEye.UI.Windows
{
    internal class EdgeUriToStreamResolver : global::Windows.Web.IUriToStreamResolver
    {
        private readonly IContentProvider contentProvider;

        public EdgeUriToStreamResolver(IContentProvider contentProvider)
        {
            this.contentProvider = contentProvider ?? throw new ArgumentNullException(nameof(contentProvider));
        }

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetStreamAsync(uri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetStreamAsync(Uri uri)
        {
            var stream = await contentProvider.GetStreamAsync(uri);
            return stream?.AsInputStream();
        }
    }
}
