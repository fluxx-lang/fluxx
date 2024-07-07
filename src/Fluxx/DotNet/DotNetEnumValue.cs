using System.Reflection;
using Faml.Binding;
using Faml.Binding.External;
using Faml.CodeAnalysis.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */
namespace Faml.DotNet
{
    public class DotNetEnumValue : ExpressionSyntax
    {
        private readonly ExternalObjectTypeBinding _typeBinding;
        private readonly FieldInfo _value;

        public DotNetEnumValue(ExternalObjectTypeBinding typeBinding, TextSpan span, FieldInfo value) : base(span)
        {
            this._typeBinding = typeBinding;
            this._value = value;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.DotNetEnumValue;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this._value.ToString());
        }

        public FieldInfo GetValue()
        {
            return this._value;
        }
    }
}
