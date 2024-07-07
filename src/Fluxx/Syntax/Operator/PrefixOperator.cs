using System.Collections.Generic;

/**
 * @author Bret Johnson
 * @since 6/28/2014 11:25 PM
 */
namespace Faml.Syntax.Operator
{
    public class PrefixOperator : Operator
    {
        public PrefixOperator(Dictionary<Lexer.TokenType, PrefixOperator> prefixOperators, Lexer.TokenType tokenType,
            string textRepresentation, int precedence) : base(textRepresentation, precedence)
        {
            prefixOperators.Add(tokenType, this);
        }
    }
}