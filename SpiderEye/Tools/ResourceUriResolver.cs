using System;
using System.IO;
using System.Reflection;

namespace SpiderEye.Tools
{
    internal class ResourceUriResolver
    {
        public static readonly ResourceUriResolver Instance = new ResourceUriResolver();

        private readonly Assembly assembly;

        public ResourceUriResolver()
            : this(Assembly.GetEntryAssembly())
        {
        }

        public ResourceUriResolver(Assembly assembly)
        {
            this.assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public Stream GetResource(string uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            return GetResource(new Uri(uri));
        }

        public Stream GetResource(Uri uri)
        {
            if (uri == null) { throw new ArgumentNullException(nameof(uri)); }

            string resourceName = assembly.GetName().Name + "." + uri.WithoutScheme().Replace('/', '.');

            try { return assembly.GetManifestResourceStream(resourceName); }
            catch { return null; }
        }
    }
}
