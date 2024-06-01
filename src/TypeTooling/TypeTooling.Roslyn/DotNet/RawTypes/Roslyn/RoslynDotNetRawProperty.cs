using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes.Roslyn.DocumentationCommentParser;

namespace TypeTooling.DotNet.RawTypes.Roslyn {
    public class RoslynDotNetRawProperty : DotNetRawProperty {
        private readonly IPropertySymbol _propertySymbol;

        public RoslynDotNetRawProperty(IPropertySymbol propertySymbol) {
            _propertySymbol = propertySymbol;
        }

        public override string Name => _propertySymbol.Name;

        public override DotNetRawType PropertyType => new RoslynDotNetRawType(_propertySymbol.Type);

        public override bool CanRead => ! _propertySymbol.IsWriteOnly;
 
        public override bool CanWrite => ! _propertySymbol.IsReadOnly;

        public override IEnumerable<DotNetRawCustomAttribute> GetCustomAttributes() {
            foreach (AttributeData attributeData in _propertySymbol.GetAttributes())
                yield return new RoslynDotNetRawCustomAttribute(attributeData);
        }

        public IPropertySymbol PropertySymbol => _propertySymbol;

        public override async Task<ClassifiedTextMarkup?> GetDescriptionAsync(CultureInfo preferredCulture,
            CancellationToken cancellationToken) {

            ClassifiedTextMarkup? descriptionMarkup = await Task.Run(() => {
                string? documentCommentXml = _propertySymbol.GetDocumentationCommentXml(preferredCulture, cancellationToken: cancellationToken);
                if (documentCommentXml == null)
                    return null;

                return DescriptionMarkupCreator.CreateDescriptionMarkup(documentCommentXml);
            }, cancellationToken);

            return descriptionMarkup;
        }
    }
}
