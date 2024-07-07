using Faml.Lexer;
using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Faml.Tests.Lexer
{
    public sealed class TokenTest
    {
        [TestMethod]
        public void TestPunctuation()
        {
            this.AssertTokenTypeIs("{", TokenType.LeftBrace);
            this.AssertTokenTypeIs("}", TokenType.RightBrace);
            this.AssertTokenTypeIs("(", TokenType.LeftParen);
            this.AssertTokenTypeIs(")", TokenType.RightParen);
            this.AssertTokenTypeIs(",", TokenType.Comma);
        }

        [TestMethod]
        public void TestKeywords()
        {
            this.AssertTokenTypeIs("true", TokenType.True);
            this.AssertTokenTypeIs("false", TokenType.False);
            this.AssertTokenTypeIs("null", TokenType.Null);
            //assertTokenTypeIs("function", TokenType.FUNCTION);
        }

        [TestMethod]
        public void TestIdentifier()
        {
            this.AssertTokenTypeIs("abc", TokenType.Identifier);
            this.AssertTokenTypeIs("abc123", TokenType.Identifier);
            this.AssertTokenTypeIs("ABC123", TokenType.Identifier);
        }

        [TestMethod]
        public void TestProperty()
        {
            this.AssertTokenTypeIs("abc:", TokenType.PropertyIdentifier);
            this.AssertTokenTypeIs("abc123:", TokenType.PropertyIdentifier);
            this.AssertTokenTypeIs("ABC123:", TokenType.PropertyIdentifier);
            this.AssertTokenTypeIs("a:", TokenType.PropertyIdentifier);
            this.AssertTokenTypeIs("abc-def:", TokenType.PropertyIdentifier);
            this.AssertTokenTypeIs("abc.def:", TokenType.PropertyIdentifier);
        }

        [TestMethod]
        public void TestOperators()
        {
            this.AssertTokenTypeIs("-", TokenType.Minus);
            this.AssertTokenTypeIs("!", TokenType.Not);

            this.AssertTokenTypeIs("*", TokenType.Times);
            this.AssertTokenTypeIs("/", TokenType.Divide);
            this.AssertTokenTypeIs("%", TokenType.Remainder);

            this.AssertTokenTypeIs("+", TokenType.Plus);
            this.AssertTokenTypeIs("-", TokenType.Minus);

            this.AssertTokenTypeIs("<", TokenType.Less);
            this.AssertTokenTypeIs(">", TokenType.Greater);
            this.AssertTokenTypeIs("<=", TokenType.LessEquals);
            this.AssertTokenTypeIs(">=", TokenType.GreaterEquals);

            this.AssertTokenTypeIs("==", TokenType.Equals);
            this.AssertTokenTypeIs("!=", TokenType.NotEquals);

            this.AssertTokenTypeIs("&&", TokenType.And);
            this.AssertTokenTypeIs("||", TokenType.Or);
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
