/**
 * @author Bret Johnson
 * @since 4/5/2015
 */

using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;

namespace Faml.Syntax.Type {
    public abstract class TypeReferenceSyntax : SyntaxNode {
        protected TypeReferenceSyntax(TextSpan span) : base(span) {}

        public abstract TypeBinding GetTypeBinding();
    }
}