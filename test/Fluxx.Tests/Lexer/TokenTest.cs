using Faml.Lexer;
using Microsoft.CodeAnalysisP.Text;
using NUnit.Framework;

namespace Faml.Tests.Lexer
{
    public sealed class TokenTest
    {
        [Test] public void TestPunctuation() {
            AssertTokenTypeIs("{", TokenType.LeftBrace);
            AssertTokenTypeIs("}", TokenType.RightBrace);
            AssertTokenTypeIs("(", TokenType.LeftParen);
            AssertTokenTypeIs(")", TokenType.RightParen);
            AssertTokenTypeIs(",", TokenType.Comma);
        }

        [Test] public void TestKeywords() {
            AssertTokenTypeIs("true", TokenType.True);
            AssertTokenTypeIs("false", TokenType.False);
            AssertTokenTypeIs("null", TokenType.Null);
            //assertTokenTypeIs("function", TokenType.FUNCTION);
        }

        [Test] public void TestIdentifier() {
            AssertTokenTypeIs("abc", TokenType.Identifier);
            AssertTokenTypeIs("abc123", TokenType.Identifier);
            AssertTokenTypeIs("ABC123", TokenType.Identifier);
        }

        [Test] public void TestProperty() {
            AssertTokenTypeIs("abc:", TokenType.PropertyIdentifier);
            AssertTokenTypeIs("abc123:", TokenType.PropertyIdentifier);
            AssertTokenTypeIs("ABC123:", TokenType.PropertyIdentifier);
            AssertTokenTypeIs("a:", TokenType.PropertyIdentifier);
            AssertTokenTypeIs("abc-def:", TokenType.PropertyIdentifier);
            AssertTokenTypeIs("abc.def:", TokenType.PropertyIdentifier);
        }

        [Test] public void TestOperators() {
            AssertTokenTypeIs("-", TokenType.Minus);
            AssertTokenTypeIs("!", TokenType.Not);

            AssertTokenTypeIs("*", TokenType.Times);
            AssertTokenTypeIs("/", TokenType.Divide);
            AssertTokenTypeIs("%", TokenType.Remainder);

            AssertTokenTypeIs("+", TokenType.Plus);
            AssertTokenTypeIs("-", TokenType.Minus);

            AssertTokenTypeIs("<", TokenType.Less);
            AssertTokenTypeIs(">", TokenType.Greater);
            AssertTokenTypeIs("<=", TokenType.LessEquals);
            AssertTokenTypeIs(">=", TokenType.GreaterEquals);

            AssertTokenTypeIs("==", TokenType.Equals);
            AssertTokenTypeIs("!=", TokenType.NotEquals);

            AssertTokenTypeIs("&&", TokenType.And);
            AssertTokenTypeIs("||", TokenType.Or);
        }

        private void AssertTokenTypeIs(string source, TokenType expectedTokenType)
        {
            var token = new Token(SourceText.From(source));
            Assert.AreEqual(expectedTokenType, (object) token.TypeAllowPropertyIdentifier);
        }

        private void AssertTokenTypeAndValueIs(string source, TokenType expectedTokenType, string expectedTokenValue)
        {
            var token = new Token(SourceText.From(source));
            Assert.AreEqual(expectedTokenType, (object) token.Type);
            Assert.AreEqual(expectedTokenValue, (object) token.StringValue);
        }
    }
}
