using System.Reflection;
using Fluxx.Binding;
using Fluxx.Binding.External;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.DotNet
{
    public class DotNetEnumValue : ExpressionSyntax
    {
        private readonly ExternalObjectTypeBinding typeBinding;
        private readonly FieldInfo value;

        public DotNetEnumValue(ExternalObjectTypeBinding typeBinding, TextSpan span, FieldInfo value) : base(span)
        {
            this.typeBinding = typeBinding;
            this.value = value;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.DotNetEnumValue;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.value.ToString());
        }

        public FieldInfo GetValue()
        {
            return this.value;
        }
    }
}
