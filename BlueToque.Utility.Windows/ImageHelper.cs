using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace BlueToque.Utility.Windows
{
    //
    // Summary:
    //     Static methods to deal with images
    public static class ImageHelper
    {
        //
        // Parameters:
        //   image:
        //
        //   format:
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static string ToBase64(this Image image, ImageFormat? format = null) =>
            Convert.ToBase64String(image.ToBytes());

        //
        // Parameters:
        //   base64String:
        public static Image? FromBase64(string base64String) =>
            FromBytes(Convert.FromBase64String(base64String));

        //
        // Summary:
        //     http://stackoverflow.com/questions/1668469/system-drawing-image-to-stream-c-sharp
        //
        //
        // Parameters:
        //   image:
        //
        //   format:
        public static Stream ToStream(this Image image, ImageFormat? format = null)
        {
            ArgumentNullException.ThrowIfNull(image);

            MemoryStream memoryStream = new();
            image.Save(memoryStream, format ?? ImageFormat.Png);
            memoryStream.Position = 0L;
            return memoryStream;
        }

        //
        // Summary:
        //     Convert image to bytes
        //
        // Parameters:
        //   image:
        //
        //   format:
        //
        //   clone:
        public static byte[] ToBytes(this Image image, ImageFormat? format = null, bool clone = true)
        {
            ArgumentNullException.ThrowIfNull(image);

            try
            {
                if (clone)
                {
                    Image image2 = (Image)image.Clone();
                    using MemoryStream memoryStream = new();
                    image2.Save(memoryStream, format ?? image.RawFormat);
                    return memoryStream.ToArray();
                }

                using MemoryStream memoryStream2 = new();
                image.Save(memoryStream2, format ?? image.RawFormat);
                return memoryStream2.ToArray();
            }
            catch (Exception ex)
            {
                Trace.TraceError("ImageHelpers.ToBytes: Error\r\n{0}", ex);
                return [];
            }
        }

        //
        // Summary:
        //     composite two images
        //
        // Parameters:
        //   baseImage:
        //
        //   overlayImage:
        public static Image Composite(Image baseImage, Image overlayImage)
        {
            Size size = new(baseImage.Size.Width / 2, baseImage.Size.Height / 2);
            Image image = new Bitmap(baseImage.Width, baseImage.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.CompositingMode = CompositingMode.SourceOver;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(baseImage, 0, 0);
                graphics.DrawImage(overlayImage, size.Width / 2, size.Height / 2, size.Width, size.Height);
            }

            return image;
        }

        //
        // Summary:
        //     Open an image from bytes
        //
        // Parameters:
        //   bytes:
        public static Image? FromBytes(byte[] bytes)
        {
            ArgumentNullException.ThrowIfNull(bytes);
            if (bytes.Length == 0) throw new ArgumentOutOfRangeException(nameof(bytes), 0, "value cannot be zero");

            try
            {
                using MemoryStream stream = new(bytes);
                return Image.FromStream(stream);
            }
            catch (Exception ex)
            {
                Trace.TraceError("ImageHelper.FromBytes: {0}", ex);
                return null;
            }
        }

        //
        // Summary:
        //     Resize an image
        //
        // Parameters:
        //   image:
        //
        //   size:
        public static Image Resize(this Image image, Size size)
        {
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            if (size.Width <= 0 || size.Height <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "size cannot be negative or zero");

            if (image.Width == size.Width && image.Height == size.Height)
                return image;

            return new Bitmap(image, size);
        }

        //
        // Summary:
        //     Resize an image
        //
        // Parameters:
        //   image:
        //
        //   size:
        public static Image Resize(this Image image, int size)
        {
            ArgumentNullException.ThrowIfNull(image, nameof(image));

            if (size <= 0)
                throw new ArgumentOutOfRangeException(nameof(size), "size cannot be negative or zero");

            if (image.Width == size && image.Height == size)
                return image;

            return new Bitmap(image, new Size(size, size));
        }

        //
        // Summary:
        //     Convert a bitmap to an icon
        //
        // Parameters:
        //   bitmap:
        public static Icon ToIcon(this Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap, nameof(bitmap));

            bitmap.MakeTransparent(Color.White);
            return Icon.FromHandle(bitmap.GetHicon());
        }

        //
        // Summary:
        //     Converts an image into an icon.
        //
        // Parameters:
        //   image:
        //     The image that shall become an icon
        //
        //   size:
        //     The width and height of the icon. Standard sizes are 16x16, 32x32, 48x48, 64x64.
        //
        //
        //   keepAspectRatio:
        //     Whether the image should be squashed into a square or whether whitespace should
        //     be put around it.
        //
        // Returns:
        //     An icon!!
        public static Icon? ToIcon(this Image image, int size, bool keepAspectRatio = true)
        {
            try
            {
                using Bitmap bitmap = new(size, size);
                using Graphics graphics = Graphics.FromImage(bitmap);
                int x;
                int y;
                int num2;
                int num;
                if (!keepAspectRatio || image.Height == image.Width)
                {
                    x = (y = 0);
                    num2 = (num = size);
                }
                else
                {
                    float num3 = (float)image.Width / (float)image.Height;
                    if (num3 > 1f)
                    {
                        num2 = size;
                        num = (int)((float)size / num3);
                        x = 0;
                        y = (size - num) / 2;
                    }
                    else
                    {
                        num2 = (int)((float)size * num3);
                        num = size;
                        y = 0;
                        x = (size - num2) / 2;
                    }
                }

                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, x, y, num2, num);
                graphics.Flush();
                return Icon.FromHandle(bitmap.GetHicon());
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error:\r\n{0}", ex);
                return null;
            }
        }

        //
        // Parameters:
        //   url:
        //
        //   imageRoot:
        public static Image? LoadImage(Uri url, string? imageRoot = null)
        {
            return url.Scheme switch
            {
                "res" => LoadFromResource(url),
                "http" or "https" => LoadFromUrl(url),
                "file" => LoadFromFile(url.LocalPath),
                _ => LoadFromFile(url.LocalPath, imageRoot),
            };
        }

        //
        // Summary:
        //     load an image
        //
        // Parameters:
        //   url:
        //
        //   imageRoot:
        //     If an imageRoot is provided, it is used to find the image
        public static Image? LoadImage(string url, string? imageRoot = null)
        {
            try
            {
                //string text = Paths.Expand(url);
                if (string.IsNullOrEmpty(url))
                {
                    Trace.TraceError("ImageHelper.LoadImage: fileName is null");
                    return null;
                }

                try
                {
                    return LoadImage(new Uri(url), imageRoot);
                }
                catch
                {
                }

                return LoadFromFile(url, imageRoot);
            }
            catch (Exception ex)
            {
                Trace.TraceError("ImageHelper.LoadImage: Error loading file {0}:\r\n{1}", url, ex);
                return null;
            }
        }

        //
        // Summary:
        //     Load an image from the filesystem
        //
        // Parameters:
        //   fileName:
        //
        //   iconRoot:
        private static Image? LoadFromFile(string fileName, string? iconRoot = null)
        {
            string text = (string.IsNullOrEmpty(iconRoot) ? fileName : Path.Combine(iconRoot, fileName));
            return (!File.Exists(text)) ? null : Image.FromFile(text);
        }

        //
        // Summary:
        //     load an image from the web
        //
        // Parameters:
        //   uri:
        private static Image? LoadFromUrl(Uri uri)
        {
            try
            {

                using var webClient = new HttpClient();
                using Stream stream = webClient.GetStreamAsync(uri.ToString()).Result;
                return Image.FromStream(stream);
            }
            catch (Exception ex)
            {
                Trace.TraceError("ImageHelper.LoadFromUrl:\r\n{0}", ex);
                return null;
            }
        }

        //
        // Summary:
        //     Load an image from a .Net resource
        //
        // Parameters:
        //   url:
        private static Image? LoadFromResource(Uri url)
        {
            Trace.TraceVerbose("ImageHelper.LoadFromResource: loading \"{0}\" from resources", url);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Assembly? assembly = assemblies.FirstOrDefault(x => !x.FullName.IsNullOrEmpty() && x.FullName.StartsWith(url.Authority, StringComparison.CurrentCultureIgnoreCase));
            if (assembly == null)
                return null;

            string[] manifestResourceNames = assembly.GetManifestResourceNames();
            string text = assembly.ShortName() + url.PathAndQuery.Replace("/", ".");
            using Stream? stream = assembly.GetManifestResourceStream(text);
            string[] manifestResourceNames2 = assembly.GetManifestResourceNames();
            if (stream == null)
            {
                Trace.TraceError("ImageHelper.LoadFromResource: Could not load \"{0}\" from resources", text);
                return null;
            }

            return Image.FromStream(stream);
        }

        //
        // Parameters:
        //   url:
        public static Image? LoadFromResource(string url) => LoadFromResource(new Uri(url));
    }

}
