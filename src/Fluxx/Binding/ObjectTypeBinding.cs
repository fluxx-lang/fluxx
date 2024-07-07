using Faml.Api;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Binding
{
    public abstract class ObjectTypeBinding : TypeBinding
    {
        protected ObjectTypeBinding(QualifiableName typeName)
            : base(typeName, TypeFlags.None)
        {
        }

        public abstract bool SupportsCreateLiteral();

        public abstract ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceSpan);
    }
}
