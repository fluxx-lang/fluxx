using System.Reflection;
using Faml.Binding;
using Faml.Binding.External;
using Faml.CodeAnalysis.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.DotNet
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
