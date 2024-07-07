/**
 * @author Bret Johnson
 * @since 4/5/2015
 */
using Fluxx.Binding;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Type
{
    public abstract class TypeReferenceSyntax : SyntaxNode
    {
        protected TypeReferenceSyntax(TextSpan span) : base(span) {}

        public abstract TypeBinding GetTypeBinding();
    }
}
