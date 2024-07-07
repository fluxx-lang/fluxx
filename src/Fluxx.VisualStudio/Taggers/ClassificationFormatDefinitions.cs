using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Fluxx.VisualStudio.Taggers {
    // NOTE: The standard is to define the Blue theme here and then override it using a theme pkgdef file,
    // but currently we only support dark theme, so it's defined here.
    // TODO: Define other themes
    public class ClassificationFormatDefinitions
	{
        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.FamlDeemphasizedPunctuation)]
        [Name(ClassificationTypeNames.FamlDeemphasizedPunctuation)]
        [Order(After = LanguagePriority.NaturalLanguage, Before = LanguagePriority.FormalLanguage)]
        [UserVisible(true)]
        class FamlDeemphasizedPunctuationFormatDefinition : ClassificationFormatDefinition {
            FamlDeemphasizedPunctuationFormatDefinition() {
                DisplayName = FamlVisualStudioResources.Classification_DeemphasizedPunctuation;
                ForegroundColor = Color.FromRgb(135, 135, 135);   // Normal punctuation is 220, 220, 220
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.FamlFunctionReference)]
        [Name(ClassificationTypeNames.FamlFunctionReference)]
        [Order(After = LanguagePriority.NaturalLanguage, Before = LanguagePriority.FormalLanguage)]
        [UserVisible(true)]
        class FamlFunctionReferenceFormatDefinition : ClassificationFormatDefinition {
            FamlFunctionReferenceFormatDefinition() {
                DisplayName = FamlVisualStudioResources.Classification_FunctionReference;
                this.ForegroundColor = Color.FromRgb(86, 156, 214);   // Matches HTML element name
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.FamlPropertyReference)]
        [Name(ClassificationTypeNames.FamlPropertyReference)]
        [Order(After = LanguagePriority.NaturalLanguage, Before = LanguagePriority.FormalLanguage)]
        [UserVisible(true)]
        class FamlPropertyFormatDefinition : ClassificationFormatDefinition {
            FamlPropertyFormatDefinition() {
                DisplayName = FamlVisualStudioResources.Classification_PropertyReference;
                ForegroundColor = Color.FromRgb(156, 220, 254);   // Matches HTML attribute name
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.FamlPropertyValue)]
        [Name(ClassificationTypeNames.FamlPropertyValue)]
        [Order(After = LanguagePriority.NaturalLanguage, Before = LanguagePriority.FormalLanguage)]
        [UserVisible(true)]
        class FamlPropertyValueFormatDefinition : ClassificationFormatDefinition {
            FamlPropertyValueFormatDefinition() {
                DisplayName = FamlVisualStudioResources.Classification_PropertyValue;
                ForegroundColor = Color.FromRgb(0xCE, 0x91, 0x78);
            }
        }

        [Export(typeof(EditorFormatDefinition))]
        [ClassificationType(ClassificationTypeNames = ClassificationTypeNames.FamlUIText)]
        [Name(ClassificationTypeNames.FamlUIText)]
        [Order(After = LanguagePriority.NaturalLanguage, Before = LanguagePriority.FormalLanguage)]
        [UserVisible(true)]
        class FamlLocalizableTextFormatDefinition : ClassificationFormatDefinition {
            FamlLocalizableTextFormatDefinition() {
                DisplayName = FamlVisualStudioResources.Classification_UIText;
                ForegroundColor = Color.FromRgb(181, 206, 168);
                FontTypeface = new Typeface("Segoe UI");
            }
        }
    }
}
