using System;
using System.Windows.Media.Imaging;

namespace Fluxx.VisualStudio.IconAdornments {
    internal sealed class IconCache {
        private static readonly Lazy<IconCache> LazyInstance = new Lazy<IconCache>(() => new IconCache());

        public static IconCache Instance => LazyInstance.Value;

        public BitmapImage VisualizeIcon { get; }

        public IconCache() {
            VisualizeIcon = LoadIconFromResource("Command1.png");
        }

        private BitmapImage LoadIconFromResource(string iconName) {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri($"pack://application:,,,/Faml.VisualStudio;component/Resources/{iconName}", UriKind.RelativeOrAbsolute);
            bitmapImage.EndInit();

            return bitmapImage;
        }
    }
}
