using Fluxx.Binding;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Type
{
    public class PredefinedTypeReferenceSyntax : TypeReferenceSyntax
    {
        private readonly BuiltInTypeBinding predefinedTypeBinding;

        public PredefinedTypeReferenceSyntax(TextSpan span, BuiltInTypeBinding predefinedTypeBinding) : base(span)
            {
            this.predefinedTypeBinding = predefinedTypeBinding;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.predefinedTypeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.PredefinedTypeReference;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.predefinedTypeBinding.TypeName);
        }
    }
}
