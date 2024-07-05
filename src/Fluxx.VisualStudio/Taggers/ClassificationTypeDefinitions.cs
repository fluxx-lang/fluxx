using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.StandardClassification;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Faml.VisualStudio.Taggers {
    public static class ClassificationTypeDefinitions {
        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FamlDeemphasizedPunctuation)]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        internal static ClassificationTypeDefinition DeemphasizedPunctuation = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FamlFunctionReference)]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        internal static ClassificationTypeDefinition FunctionReference = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FamlPropertyReference)]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        internal static ClassificationTypeDefinition PropertyName = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FamlPropertyValue)]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        internal static ClassificationTypeDefinition PropertyValue = null;

        [Export(typeof(ClassificationTypeDefinition))]
        [Name(ClassificationTypeNames.FamlUIText)]
        [BaseDefinition(PredefinedClassificationTypeNames.FormalLanguage)]
        internal static ClassificationTypeDefinition UIText = null;
    }
}
