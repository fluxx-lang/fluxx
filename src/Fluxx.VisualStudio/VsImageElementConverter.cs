using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using System.Windows.Media.Imaging;
using Fluxx.VisualStudio.IconAdornments;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using TypeTooling.Images;
using VSAdornments = Microsoft.VisualStudio.Text.Adornments;

namespace Fluxx.VisualStudio {
    public static class VsImageElementConverter {
        private static Dictionary<byte[], IImageHandle> _iconCache = new Dictionary<byte[], IImageHandle>();

        public static async Task<VSAdornments.ImageElement?> ToVsAsync(Image image) {
            if (!(Package.GetGlobalService(typeof(SVsImageService)) is IVsManagedImageService managedImageService))
                return null;

            if (image is PngImage pngImage) {
                byte[] pngData = pngImage.PngData;

                if (! _iconCache.TryGetValue(pngData, out IImageHandle imageHandle)) {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    MemoryStream memoryStream = new MemoryStream(pngData);
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    imageHandle = managedImageService.AddCustomImage(bitmapImage, true);
                    _iconCache.Add(pngData, imageHandle);
                }

                ImageMoniker moniker = imageHandle.Moniker;
                ImageId imageId = moniker.ToImageId();

                return new VSAdornments.ImageElement(imageId);
            }

            return null;
        }

        public static VSAdornments.ImageElement? ToVsTest() {
            if (!(Package.GetGlobalService(typeof(SVsImageService)) is IVsManagedImageService managedImageService))
                return null;

            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri("c:\\myhome\\plus.png");
            b.EndInit();

            IImageHandle image = managedImageService.AddCustomImage(b, true);
            ImageMoniker moniker = image.Moniker;
            ImageId imageId = moniker.ToImageId();
            return new VSAdornments.ImageElement(imageId);
        }
    }
}
