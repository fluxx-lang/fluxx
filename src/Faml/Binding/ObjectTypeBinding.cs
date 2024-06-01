using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;

namespace Faml.Binding {
    public abstract class ObjectTypeBinding : TypeBinding {
        protected ObjectTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None) {}

        public abstract bool SupportsCreateLiteral();

        public abstract ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan sourceSpan);
    }
}
