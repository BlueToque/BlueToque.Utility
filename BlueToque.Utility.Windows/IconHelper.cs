using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace BlueToque.Utility.Windows
{
    /// <summary>
    /// Internals are mostly from here: http://www.codeproject.com/Articles/2532/Obtaining-and-managing-file-and-folder-icons-using
    /// Caches all results.
    /// </summary>
    public static class IconHelper
    {
        private static readonly Dictionary<string, Icon> s_smallIconCache = [];

        private static readonly Dictionary<string, Icon> s_largeIconCache = [];

        /// <summary>
        /// Get an icon for a given filename
        /// </summary>
        /// <param name="fileName">any filename</param>
        /// <param name="large">16x16 or 32x32 icon</param>
        /// <returns>null if path is null, otherwise - an icon</returns>
        public static Icon? FindIconForFilename(string fileName, bool large)
        {
            var extension = Path.GetExtension(fileName);
            if (extension == null)
                return null;

            var cache = large ? s_largeIconCache : s_smallIconCache;

            if (cache.TryGetValue(extension, out Icon? icon))
                return icon;

            icon = IconReader.GetFileIcon(fileName, large ? IconReader.IconSize.Large : IconReader.IconSize.Small, false);
            cache.Add(extension, icon);

            return icon;
        }

        /// <summary>
        /// Provides static methods to read system icons for both folders and files.
        /// </summary>
        /// <example>
        /// <code>IconReader.GetFileIcon("c:\\general.xls");</code>
        /// </example>
        static class IconReader
        {
            /// <summary>
            /// Options to specify the size of icons to return.
            /// </summary>
            public enum IconSize
            {
                /// <summary> Specify large icon - 32 pixels by 32 pixels.</summary>
                Large = 0,

                /// <summary> Specify small icon - 16 pixels by 16 pixels.</summary>
                Small = 1,
            }

            /// <summary>
            /// Returns an icon for a given file - indicated by the name parameter.
            /// </summary>
            /// <param name="name">Pathname for file.</param>
            /// <param name="size">Large or small</param>
            /// <param name="linkOverlay">Whether to include the link icon</param>
            /// <returns>System.Drawing.Icon</returns>
            public static Icon GetFileIcon(string name, IconSize size, bool linkOverlay)
            {
                var shfi = new NativeMethods.Shfileinfo();
                var flags = NativeMethods.ShgfiIcon | NativeMethods.ShgfiUsefileattributes;
                if (linkOverlay) flags += NativeMethods.ShgfiLinkoverlay;

                /* Check the size specified for return. */
                if (IconSize.Small == size)
                    flags += NativeMethods.ShgfiSmallicon;
                else
                    flags += NativeMethods.ShgfiLargeicon;

                NativeMethods.SHGetFileInfo(name, NativeMethods.FileAttributeNormal, ref shfi, (uint)Marshal.SizeOf(shfi), flags);

                // Copy (clone) the returned icon to a new object, thus allowing us to clean-up properly
                var icon = (Icon)Icon.FromHandle(shfi.hIcon).Clone();
                var _ = NativeMethods.DestroyIcon(shfi.hIcon);     // Cleanup

                return icon;
            }
        }

        /// <summary>
        /// Get an embedded resource from the assembly it was called from
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Icon? GetIconFromEmbeddedResource(string name, Size size)
        {
            var asm = System.Reflection.Assembly.GetCallingAssembly();
            var resourceNames = asm.GetManifestResourceNames();
            var tofind = $".{name}.ICO";

            foreach (string resource in resourceNames)
            {
                if (resource.EndsWith(tofind, StringComparison.CurrentCultureIgnoreCase))
                {
                    using var stream = asm.GetManifestResourceStream(resource);
                    if (stream == null)
                        return null;
                    return new Icon(stream, size);
                }
            }

            throw new ApplicationException("Icon not found");
        }

        /// <summary>
        /// get icon of a given size
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Icon GetIcon(this Icon icon, int width, int height) => icon.GetIcon(new Size(width, height));

        /// <summary>
        /// Get an icon of a given size
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Icon GetIcon(this Icon icon, Size size)
        {
            using var mem = new MemoryStream();
            icon.Save(mem);
            mem.Position = 0;
            return new Icon(mem, size);
        }

        /// <summary>
        /// Get an image of the given size from the icon
        /// </summary>
        /// <param name="icon"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image ToImage(this Icon icon, int size = 0)
        {
            ArgumentNullException.ThrowIfNull(icon, nameof(icon));

            if (size == 0) size = 256;

            var resizedIcon = icon.GetIcon(new Size(size, size));
            if (resizedIcon == null)
                return icon.ToBitmap().Resize(size);

            return resizedIcon.ToBitmap();
        }

        public static byte[] ToBytes(this Icon icon)
        {
            using MemoryStream ms = new();
            icon.Save(ms);
            return ms.ToArray();
        }

        public static Icon FromBytes(byte[] bytes)
        {
            using MemoryStream ms = new(bytes);
            return new Icon(ms);
        }

    }
}
