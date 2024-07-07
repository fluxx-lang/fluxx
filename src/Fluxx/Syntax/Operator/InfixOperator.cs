using System.Collections.Generic;


/**
 * @author Bret Johnson
 * @since 6/28/2014 11:10 PM
 */

namespace Faml.Syntax.Operator
{
    public class InfixOperator : Operator
    {
        public InfixOperator(Dictionary<Lexer.TokenType, InfixOperator> infixOperators, Lexer.TokenType tokenType,
            string textRepresentation, int precedence) : base(textRepresentation, precedence)
            {
            infixOperators.Add(tokenType, this);
        }
    }
}
