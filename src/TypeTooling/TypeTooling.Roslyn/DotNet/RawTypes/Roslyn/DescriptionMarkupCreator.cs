using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes.Roslyn.DocumentationCommentParser;

namespace TypeTooling.DotNet.RawTypes.Roslyn
{
    class DescriptionMarkupCreator
    {
        public static ClassifiedTextMarkup? CreateDescriptionMarkup(string documentCommentXml) {

            var documentationComment = DocumentationComment.FromXmlFragment(documentCommentXml);
            if (documentationComment.HadXmlParseError)
                return null;

            string? summaryText = documentationComment.SummaryText;
            if (summaryText == null)
                return null;

            return new ClassifiedTextMarkup(summaryText);
        }
    }
}
