using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Faml.VisualStudio.Example
{
    public class ImageExample {
        private readonly StackPanel _stackPanel;

        internal ImageExample(Image image, string label) {
            _stackPanel = new StackPanel();

#if false
            var border = new Border {
                Background = Brushes.LightBlue,
                BorderBrush = Brushes.LightSeaGreen,
                BorderThickness = new Thickness(2),
                Child = image
            };
#endif
            _stackPanel.Children.Add(image);

            if (label != null) {
                var run = new Run(label);
                var labelText = new TextBlock(run) {
                    Foreground = new SolidColorBrush(Colors.Wheat),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                _stackPanel.Children.Add(labelText);
            };
        }

        public FrameworkElement RootElement => _stackPanel;
    }
}