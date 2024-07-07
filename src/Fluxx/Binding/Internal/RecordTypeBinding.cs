using Faml.Api;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Binding.Internal
{
    public class RecordTypeBinding : ObjectTypeBinding
    {
        private readonly RecordTypeDefinitionSyntax _recordTypeDefinition;

        public RecordTypeBinding(RecordTypeDefinitionSyntax recordTypeDefinition) : base(recordTypeDefinition.TypeNameSyntax.Name.ToQualifiableName())
        {
            this._recordTypeDefinition = recordTypeDefinition;
        }

        public RecordTypeDefinitionSyntax RecordTypeDefinition => this._recordTypeDefinition;

        protected bool Equals(RecordTypeBinding other)
        {
            return this._recordTypeDefinition.Equals(other._recordTypeDefinition);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RecordTypeBinding))
            {
                return false;
            }

            return this.Equals((RecordTypeBinding) obj);
        }

        public override int GetHashCode()
        {
            return this._recordTypeDefinition.GetHashCode();
        }

        public override PropertyBinding? GetPropertyBinding(Name propertyName)
        {
            if (!this._recordTypeDefinition.HasProperty(propertyName))
            {
                return null;
            }

            return new RecordPropertyBinding(this._recordTypeDefinition, propertyName);
        }

        public override bool SupportsCreateLiteral()
        {
            return false;
        }

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceSpan)
        {
            string literalSource = module.SourceText.ToString(sourceSpan);

            var invalidExpression = new InvalidExpressionSyntax(sourceSpan, literalSource, this);
            module.AddError(invalidExpression, $"'{this._recordTypeDefinition.TypeNameSyntax}' can't be expressed with a literal value");
            return invalidExpression;
        }
    }
}
