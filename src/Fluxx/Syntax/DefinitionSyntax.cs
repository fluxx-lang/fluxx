

/**
 * @author Bret Johnson
 * @since 4/5/2015
 */
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax
{
    public abstract class DefinitionSyntax : SyntaxNode
    {
        protected DefinitionSyntax(TextSpan span)
            : base(span)
        {
        }
    }
}
