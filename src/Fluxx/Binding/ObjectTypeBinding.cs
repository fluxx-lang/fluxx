using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Binding
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
