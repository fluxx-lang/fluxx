using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Binding
{
    public abstract class EnumTypeBinding : TypeBinding
    {
        protected EnumTypeBinding(QualifiableName typeName) : base(typeName, TypeFlags.None)
        {
        }

        public abstract ExpressionSyntax ParseEnumValue(FamlModule module, TextSpan sourceSpan);

        public abstract ImmutableArray<EnumValueBinding> GetValues();
    }
}
