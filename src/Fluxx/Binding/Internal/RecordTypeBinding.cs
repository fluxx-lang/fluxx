using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Binding.Internal
{
    public class RecordTypeBinding : ObjectTypeBinding
    {
        private readonly RecordTypeDefinitionSyntax recordTypeDefinition;

        public RecordTypeBinding(RecordTypeDefinitionSyntax recordTypeDefinition) : base(recordTypeDefinition.TypeNameSyntax.Name.ToQualifiableName())
        {
            this.recordTypeDefinition = recordTypeDefinition;
        }

        public RecordTypeDefinitionSyntax RecordTypeDefinition => this.recordTypeDefinition;

        protected bool Equals(RecordTypeBinding other)
        {
            return this.recordTypeDefinition.Equals(other.recordTypeDefinition);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is RecordTypeBinding))
            {
                return false;
            }

            return this.Equals((RecordTypeBinding)obj);
        }

        public override int GetHashCode()
        {
            return this.recordTypeDefinition.GetHashCode();
        }

        public override PropertyBinding? GetPropertyBinding(Name propertyName)
        {
            if (!this.recordTypeDefinition.HasProperty(propertyName))
            {
                return null;
            }

            return new RecordPropertyBinding(this.recordTypeDefinition, propertyName);
        }

        public override bool SupportsCreateLiteral()
        {
            return false;
        }

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceSpan)
        {
            string literalSource = module.SourceText.ToString(sourceSpan);

            var invalidExpression = new InvalidExpressionSyntax(sourceSpan, literalSource, this);
            module.AddError(invalidExpression, $"'{this.recordTypeDefinition.TypeNameSyntax}' can't be expressed with a literal value");
            return invalidExpression;
        }
    }
}
