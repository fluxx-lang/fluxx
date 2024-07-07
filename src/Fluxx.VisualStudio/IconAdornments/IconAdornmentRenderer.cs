using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Fluxx.Api;
using Fluxx.VisualStudio.Taggers;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace Fluxx.VisualStudio.IconAdornments {
    internal sealed class IconAdornmentRenderer {
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly IWpfTextView _textView;
        private readonly FamlModuleBuffer _famlModuleBuffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAdornmentRenderer"/> class.
        /// </summary>
        /// <param name="textView">Text view to create the adornment for</param>
        /// <param name="textDocumentFactoryService">ITextDocumentFactoryService instance</param>
        public IconAdornmentRenderer(IWpfTextView textView, ITextDocumentFactoryService textDocumentFactoryService) {
            _famlModuleBuffer = FamlModuleBuffer.GetOrCreateFromTextBuffer(textView.TextBuffer);
            _adornmentLayer = textView.GetAdornmentLayer("IconAdornments");

            _textView = textView;
            _textView.LayoutChanged += this.OnLayoutChanged;
        }

        public FamlModuleBuffer FamlModuleBuffer => _famlModuleBuffer;

        /// <summary>
        /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
        /// </summary>
        /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
        /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
        /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
        /// </remarks>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        internal void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
                RefreshAdornmentsOnLine(line);
        }

        private void RefreshAdornmentsOnLine(ITextViewLine line) {
            ReadOnlyCollection<object> iconTagObjects = line.GetAdornmentTags(IconSpaceNegotiatingTag.IconProviderTag);

            foreach (object iconTagObject in iconTagObjects) {
                var iconTag = (IconTag) iconTagObject;

                TextBounds? bounds = line.GetAdornmentBounds(iconTag);
                if (bounds != null) {
#if false
                    // Offset to the inside of the left and right edges so that the adornment does not spill outside its bounds.
                    const double offset = 1.0;
                    double trailingX = bounds.Value.Left + offset;
                    double leadingX = bounds.Value.Right - offset;

                    PointCollection points = new PointCollection(3);
                    points.Add(new Point(leadingX, bounds.Value.TextTop + bounds.Value.TextHeight * 0.5));
                    points.Add(new Point(trailingX, bounds.Value.TextTop + offset));
                    points.Add(new Point(trailingX, bounds.Value.TextBottom - offset));

                    Polygon polygon = new Polygon();
                    polygon.Points = points;

                    polygon.Stroke = Brushes.WhiteSmoke;
#endif

#if false
                    var iconImage = new Image {
                        Source = IconCache.Instance.VisualizeIcon,
                    };
                    _adornmentLayer.AddAdornment(line.Extent, iconTag, iconImage);
#endif
                }
            }
        }
    }
}
