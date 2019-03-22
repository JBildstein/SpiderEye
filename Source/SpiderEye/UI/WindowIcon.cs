using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SpiderEye.UI
{
    /// <summary>
    /// Represents an icon for a window with one or more resolutions.
    /// </summary>
    public class WindowIcon
    {
        internal readonly List<byte[]> Icons = new List<byte[]>();

        /// <summary>
        /// Adds an icon from a file path.
        /// </summary>
        /// <param name="path">The path to the icon file.</param>
        public void AddFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { throw new ArgumentNullException(nameof(path)); }

            Icons.Add(File.ReadAllBytes(path));
        }

        /// <summary>
        /// Adds an icon from a stream.
        /// </summary>
        /// <param name="stream">The stream containing the icon data.</param>
        public void AddFromStream(Stream stream)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }

            using (var memStream = new MemoryStream())
            {
                stream.CopyTo(memStream);
                Icons.Add(memStream.ToArray());
            }
        }

        /// <summary>
        /// Adds an icon from an embedded resource using the entry assembly.
        /// </summary>
        /// <param name="name">The name of the icon resource.</param>
        public void AddFromResource(string name)
        {
            AddFromResource(name, Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Adds an icon from an embedded resource using the given assembly.
        /// </summary>
        /// <param name="name">The name of the icon resource.</param>
        /// <param name="assembly">The assembly containing the resource.</param>
        public void AddFromResource(string name, Assembly assembly)
        {
            if (string.IsNullOrWhiteSpace(name)) { throw new ArgumentNullException(nameof(name)); }
            if (assembly == null) { throw new ArgumentNullException(nameof(assembly)); }

            using (var stream = assembly.GetManifestResourceStream(name))
            {
                if (stream == null) { throw new InvalidOperationException($"Resource with name {name} not found in assembly {assembly.GetName().Name}"); }

                using (var reader = new BinaryReader(stream))
                {
                    Icons.Add(reader.ReadBytes((int)stream.Length));
                }
            }
        }
    }
}
