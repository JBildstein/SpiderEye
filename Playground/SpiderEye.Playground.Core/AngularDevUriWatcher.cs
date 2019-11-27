using System;
using System.Diagnostics;

namespace SpiderEye.Playground.Core
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
            CheckDevUri(ref uri);

            return uri;
        }

        [Conditional("DEBUG")]
        private void CheckDevUri(ref Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                uri = new Uri(devServerUri, uri);
            }
        }
    }
}
