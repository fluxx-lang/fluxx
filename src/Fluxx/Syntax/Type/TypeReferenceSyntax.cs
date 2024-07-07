/**
 * @author Bret Johnson
 * @since 4/5/2015
 */
using Faml.Binding;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Type
{
    public abstract class TypeReferenceSyntax : SyntaxNode
    {
        protected TypeReferenceSyntax(TextSpan span) : base(span) {}

        public abstract TypeBinding GetTypeBinding();
    }
}
