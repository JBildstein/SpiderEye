using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SpiderEye
{
    /// <summary>
    /// Represents an application icon with one or more resolutions.
    /// </summary>
    public class AppIcon
    {
        /// <summary>
        /// Gets or sets a value indicating whether to cache file based icons or read them from disk every time.
        /// </summary>
        public bool UseFileCache { get; set; }

        internal readonly IconSource Source;
        internal readonly IconInfo DefaultIcon;
        internal readonly IconInfo[] Icons;

        private readonly Assembly? iconAssembly;
        private readonly Dictionary<string, byte[]> iconCache = new();

        private AppIcon(IconSource source, string iconName, IEnumerable<string> names, bool cacheFiles)
        {
            Source = source;
            UseFileCache = cacheFiles;
            Icons = GetIcons(iconName, names);
            DefaultIcon = GetDefaultIcon(Icons);
            UseFileCache = true;
        }

        private AppIcon(Assembly iconAssembly, string iconName, IEnumerable<string> names)
            : this(IconSource.Resource, iconName, names, false)
        {
            this.iconAssembly = iconAssembly;
        }

        /// <summary>
        /// Creates a new icon that looks for the appropriate icon in the given directory.
        /// </summary>
        /// <param name="iconName">The name of the icon file. e.g. "MyIcon".</param>
        /// <param name="directory">The directory to look for icon files.</param>
        /// <returns>The created icon.</returns>
        public static AppIcon FromFile(string iconName, string directory)
        {
            return FromFile(iconName, directory, true);
        }

        /// <summary>
        /// Creates a new icon that looks for the appropriate icon in the given directory.
        /// </summary>
        /// <param name="iconName">The name of the icon file. e.g. "MyIcon".</param>
        /// <param name="directory">The directory to look for icon files.
        /// May be absolute or relative to the executable.
        /// To use the executable directory, use either an empty string or a period.
        /// </param>
        /// <param name="cacheFiles">True to cache files after the first read; False to read from disk every time.</param>
        /// <returns>The created icon.</returns>
        public static AppIcon FromFile(string iconName, string directory, bool cacheFiles)
        {
            if (iconName == null) { throw new ArgumentNullException(nameof(iconName)); }
            if (directory == null) { throw new ArgumentNullException(nameof(directory)); }

            if (!Path.IsPathRooted(directory))
            {
                string? exeDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                if (exeDir == null) { throw new InvalidOperationException("Unable to get executable directory"); }

                if (directory == string.Empty || directory == ".") { directory = exeDir; }
                else { directory = Path.Combine(exeDir, directory); }
            }

            var files = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly);
            if (!files.Any()) { throw new InvalidOperationException("No files found."); }

            return new AppIcon(IconSource.File, iconName, files, cacheFiles);
        }

        /// <summary>
        /// Creates a new icon that looks for the appropriate icon with the given base resource name.
        /// </summary>
        /// <param name="iconName">The name of the icon file. e.g. "MyIcon".</param>
        /// <param name="baseName">The base resource name for the icon files.</param>
        /// <returns>The created icon.</returns>
        public static AppIcon FromResource(string iconName, string baseName)
        {
            var assembly = Assembly.GetEntryAssembly() ?? throw new InvalidOperationException("Unable to get entry assembly");
            return FromResource(iconName, baseName, assembly);
        }

        /// <summary>
        /// Creates a new icon that looks for the appropriate icon with the given base resource name.
        /// </summary>
        /// <param name="iconName">The name of the icon file. e.g. "MyIcon".</param>
        /// <param name="baseName">The base resource name for the icon files.</param>
        /// <param name="assembly">The assembly containing the resource.</param>
        /// <returns>The created icon.</returns>
        public static AppIcon FromResource(string iconName, string baseName, Assembly assembly)
        {
            if (iconName == null) { throw new ArgumentNullException(nameof(iconName)); }
            if (baseName == null) { throw new ArgumentNullException(nameof(baseName)); }
            if (assembly == null) { throw new ArgumentNullException(nameof(assembly)); }

            var names = assembly.GetManifestResourceNames().Where(t => t.StartsWith(baseName));
            if (!names.Any()) { throw new InvalidOperationException("No files match the given base name."); }

            return new AppIcon(assembly, iconName, names);
        }

        internal Stream GetIconDataStream(IconInfo icon)
        {
            if (icon == null) { throw new ArgumentNullException(nameof(icon)); }

            switch (Source)
            {
                case IconSource.File:
                    if (UseFileCache)
                    {
                        if (iconCache.TryGetValue(icon.Path, out byte[]? data)) { return new MemoryStream(data); }
                        else
                        {
                            var stream = new MemoryStream();
                            using (var fstream = File.OpenRead(icon.Path))
                            {
                                fstream.CopyTo(stream);
                            }

                            iconCache.Add(icon.Path, stream.ToArray());

                            stream.Position = 0;
                            return stream;
                        }
                    }
                    else { return File.OpenRead(icon.Path); }

                case IconSource.Resource:
                    return iconAssembly!.GetManifestResourceStream(icon.Path) ?? throw new InvalidOperationException($"Assembly does not contain icon path {icon.Path}");

                default:
                    throw new InvalidOperationException($"Invalid icon source \"{Source}\".");
            }
        }

        internal byte[] GetIconData(IconInfo icon)
        {
            if (icon == null) { throw new ArgumentNullException(nameof(icon)); }

            if (UseFileCache && iconCache.TryGetValue(icon.Path, out byte[]? data))
            {
                return data;
            }

            using var stream = GetIconDataStream(icon);
            using var reader = new BinaryReader(stream);
            return reader.ReadBytes((int)stream.Length);
        }

        private static IconInfo[] GetIcons(string iconName, IEnumerable<string> names)
        {
            string ext = Application.OS switch
            {
                OperatingSystem.Windows => "ico",
                OperatingSystem.MacOS => "icns",
                OperatingSystem.Linux => "png",
                _ => throw new PlatformNotSupportedException(),
            };

            // matches patterns like:
            // Icon.png
            // Icon32.png
            // Icon-32.png
            // Icon_32.png
            // Icon-32x32.png
            // Icon-32-32.png
            // Icon-32_32.png
            var regex = new Regex($@"{iconName}[-_]?(\d{{0,4}})[xX-_]?(\d{{0,4}})*\.{ext}$", RegexOptions.Compiled);
            var found = new List<IconInfo>();
            foreach (string name in names)
            {
                var match = regex.Match(name);
                if (match.Success)
                {
                    int? width = int.TryParse(match.Groups[1].Value, out int w) ? w : (int?)null;
                    int? height = int.TryParse(match.Groups[2].Value, out int h) ? h : (int?)null;

                    found.Add(new IconInfo(name, width, height));
                }
            }

            if (found.Count == 0) { throw new InvalidOperationException("No matching files found."); }

            return found.ToArray();
        }

        private static IconInfo GetDefaultIcon(IconInfo[] icons)
        {
            if (icons.Length == 1) { return icons[0]; }
            else
            {
                var noSize = icons.FirstOrDefault(t => t.Height == null && t.Width == null);
                if (noSize != null) { return noSize; }
                else
                {
                    return icons
                        .OrderByDescending(t => t.Width)
                        .ThenByDescending(t => t.Height)
                        .First();
                }
            }
        }

        internal sealed class IconInfo
        {
            public string Path { get; }
            public int? Width { get; }
            public int? Height { get; }

            public IconInfo(string path, int? width, int? height)
            {
                if (width != null && height == null) { height = width; }

                Path = path;
                Width = width;
                Height = height;
            }
        }

        internal enum IconSource
        {
            File,
            Resource,
        }
    }
}
