using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.Web;

namespace SpiderEye.Windows
{
    internal class EdgeUriToStreamResolver : IUriToStreamResolver
    {
        public static readonly EdgeUriToStreamResolver Instance = new EdgeUriToStreamResolver();

        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            return GetStreamAsync(uri).AsAsyncOperation();
        }

        private async Task<IInputStream> GetStreamAsync(Uri uri)
        {
            var stream = await Application.ContentProvider.GetStreamAsync(uri);
            if (stream == null) { throw new FileNotFoundException(); }

            return stream.AsInputStream();
        }
    }
}
