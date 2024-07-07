using System.Collections.Generic;
using TokenType = Faml.Lexer.TokenType;


/**
 * @author Bret Johnson
 * @since 6/28/2014 11:21 PM
 */
namespace Faml.Syntax.Operator
{
    public class Operator
    {
        private readonly string _sourceRepresentation;
        private readonly int _precedence;

        private static readonly Dictionary<Lexer.TokenType, InfixOperator> InfixOperators = new Dictionary<Lexer.TokenType, InfixOperator>();
        private static readonly Dictionary<Lexer.TokenType, PrefixOperator> PrefixOperators = new Dictionary<Lexer.TokenType, PrefixOperator>();

        public static InfixOperator Dot = new InfixOperator(InfixOperators, TokenType.Period, ".", 14);

        public static PrefixOperator UnvaryMinus = new PrefixOperator(PrefixOperators, TokenType.Minus, "-", 13);
        public static PrefixOperator Not = new PrefixOperator(PrefixOperators, TokenType.Not, "!", 13);

        public static InfixOperator Times = new InfixOperator(InfixOperators, TokenType.Times, "*", 12);
        public static InfixOperator Divide = new InfixOperator(InfixOperators, TokenType.Divide, "/", 12);
        public static InfixOperator Remainder = new InfixOperator(InfixOperators, TokenType.Remainder, "%", 12);

        public static InfixOperator Plus = new InfixOperator(InfixOperators, TokenType.Plus, "+", 11);
        public static InfixOperator Minus = new InfixOperator(InfixOperators, TokenType.Minus, "-", 11);
        public static InfixOperator Less = new InfixOperator(InfixOperators, TokenType.Less, "<", 10);
        public static InfixOperator Greater = new InfixOperator(InfixOperators, TokenType.Greater, ">", 10);
        public static InfixOperator LessEquals = new InfixOperator(InfixOperators, TokenType.LessEquals, "<=", 10);
        public static InfixOperator GreaterEquals = new InfixOperator(InfixOperators, TokenType.GreaterEquals, ">=", 10);

        public static InfixOperator Equals = new InfixOperator(InfixOperators, TokenType.Equals, "==", 9);
        public static InfixOperator NotEquals = new InfixOperator(InfixOperators, TokenType.NotEquals, "!=", 9);

        public static InfixOperator And = new InfixOperator(InfixOperators, TokenType.And, "&&", 8);
        public static InfixOperator Or = new InfixOperator(InfixOperators, TokenType.Or, "||", 8);

        public static InfixOperator For = new InfixOperator(InfixOperators, TokenType.For, "||", 7);

        public static PrefixOperator GetPrefixOperator(Lexer.TokenType tokenType)
        {
            return PrefixOperators.TryGetValue(tokenType, out PrefixOperator prefixOperator) ? prefixOperator : null;
        }

        public static InfixOperator GetInfixOperator(Lexer.TokenType tokenType)
        {
            return InfixOperators.TryGetValue(tokenType, out InfixOperator infixOperator) ? infixOperator : null;
        }

        public Operator(string sourceRepresentation, int precedence)
        {
            this._sourceRepresentation = sourceRepresentation;
            this._precedence = precedence;
        }

        public virtual int GetPrecedence() => this._precedence;

        public virtual string GetSourceRepresentation() => this._sourceRepresentation;
    }

}
