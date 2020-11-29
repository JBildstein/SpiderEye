using System;
using System.Diagnostics;

namespace SpiderEye.Example.Spa.Core
{
    public class AngularDevUriWatcher : IUriWatcher
    {
        private readonly Uri devServerUri;

        public AngularDevUriWatcher(string devServerUri)
        {
            this.devServerUri = new Uri(devServerUri);
        }

        public Uri CheckUri(Uri uri)
        {
            // this is only called in debug mode
            CheckDevUri(ref uri);

            return uri;
        }

        [Conditional("DEBUG")]
        private void CheckDevUri(ref Uri uri)
        {
            // this changes a relative URI (e.g. /index.html) to
            // an absolute URI with the Angular dev server as host
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(devServerUri, uri);
            }
        }
    }
}
