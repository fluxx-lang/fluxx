using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Faml.Binding.Internal {
    public class RecordTypeBinding : ObjectTypeBinding {
        private readonly RecordTypeDefinitionSyntax _recordTypeDefinition;

        public RecordTypeBinding(RecordTypeDefinitionSyntax recordTypeDefinition) : base(recordTypeDefinition.TypeNameSyntax.Name.ToQualifiableName()) {
            _recordTypeDefinition = recordTypeDefinition;
        }

        public RecordTypeDefinitionSyntax RecordTypeDefinition => _recordTypeDefinition;

        protected bool Equals(RecordTypeBinding other) {
            return _recordTypeDefinition.Equals(other._recordTypeDefinition);
        }

        public override bool Equals(object obj) {
            if (! (obj is RecordTypeBinding))
                return false;
            return Equals((RecordTypeBinding) obj);
        }

        public override int GetHashCode() {
            return _recordTypeDefinition.GetHashCode();
        }

        public override PropertyBinding? GetPropertyBinding(Name propertyName) {
            if (!_recordTypeDefinition.HasProperty(propertyName))
                return null;
            return new RecordPropertyBinding(_recordTypeDefinition, propertyName);
        }

        public override bool SupportsCreateLiteral() {
            return false;
        }

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceSpan) {
            string literalSource = module.SourceText.ToString(sourceSpan);

            var invalidExpression = new InvalidExpressionSyntax(sourceSpan, literalSource, this);
            module.AddError(invalidExpression, $"'{_recordTypeDefinition.TypeNameSyntax}' can't be expressed with a literal value");
            return invalidExpression;
        }
    }
}
