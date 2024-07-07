namespace Faml.Lexer
{
    public enum TokenType
    {
        LeftBrace,
        RightBrace,

        LeftBracket,
        RightBracket,

        LeftParen,
        RightParen,

        Comma,
        Assign,
        Colon,
        Semicolon,
        Period,
        Ellipsis,

        Not,
        Times,
        Divide,
        Remainder,

        Plus,
        Minus,
        Less,
        Greater,

        LessEquals,
        GreaterEquals,

        Equals,
        NotEquals,

        And,
        Or,
        Pipe,

        Identifier,
        PropertyIdentifier,

        Int32,
        TextBlock,
        TextualLiteralText,

        // Reserved words
        True,
        False,
        Null,
        Type,
        Import,
        Use,
        If,
        Is,
        Else,
        For,
        In,

        ErrorMode,
        Invalid,
        Eof,
    }
}
