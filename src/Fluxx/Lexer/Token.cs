using System;
using System.Diagnostics;
using System.Text;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Lexer
{
    public sealed class Token
    {
        private readonly ParseableSource source;
        private readonly int sourceEnd;
        private int prevTokenEndPosition;
        private int tokenStartPosition;
        private int position;
        private TokenType type;
        private bool inErrorMode;

        public string StringValue { get; private set; }
        public int IntValue { get; private set; }
        public long LongValue { get; }
        public double DoubleValue { get; }

        public Token(SourceText sourceText) : this(new ParseableSource(sourceText, 0, sourceText.Length)) {}

        public Token(ParseableSource source, bool noInitialAdvance = false)
        {
            this.source = source;
            this.sourceEnd = source.EndPosition;

            this.SetPosition(source.StartPosition);

            this.prevTokenEndPosition = -1;

            if (!noInitialAdvance)
            {
                this.Advance();
            }
        }

        public ParseableSource Source => this.source;

        public bool InErrorMode
        {
            get => this.inErrorMode;
            set => this.inErrorMode = value;
        }

        /// <summary>
        /// Return the current token type. When in error mode, for panic mode error recovery,
        /// the special token ERROR_MODE is returned. Normally the parsing code treats that token as
        /// special, not matching anything valid, and instead generating a stub item in the AST.
        /// </summary>
        public TokenType Type => this.inErrorMode ? TokenType.ErrorMode : this.type;

        /// <summary>
        /// Return the current token type, even in error mode. This is normally just used by the sync
        /// code, trying to to match on a valid synchronizing token, for panic mode error recovery,
        /// to then exit error mode.
        /// </summary>
        public TokenType TypeForErrorMode => this.type;

        public TokenType TypeAllowPropertyIdentifier
        {
            get
            {
                //TransformIntoPropertyIdentifierIfCan();
                return this.Type;
            }
        }

        public override string ToString()
        {
            string typeString = this.type.ToString();
            if (this.type == TokenType.Identifier || this.type == TokenType.PropertyIdentifier /* || type == TokenType.FUNCTION_NAME */)
            {
                return typeString + "(" + this.StringValue + ")";
            }
            else
            {
                return typeString;
            }
        }

        public int TokenStartPosition => this.tokenStartPosition;

        public int TokenStartColumn => this.source.GetColumn(this.tokenStartPosition);

        public int TokenEndPosition => this.position;

        public TextSpan TokenSpan => TextSpan.FromBounds(this.tokenStartPosition, this.position);

        public TextSpan TokenSourceSpanExceptForLastCharacter => TextSpan.FromBounds(this.tokenStartPosition, this.position - 1);

        public int TokenLength => this.position - this.tokenStartPosition;

        public int PrevTokenEndPosition => this.prevTokenEndPosition;

        public void RescanAsTextBlock(int introducingPosition, bool allowSemicolonToTerminate = false, bool allowRightBraceToTerminate = false, bool allowIfConditionPipeToTerminate = false)
        {
            this.StartRescanAsTextBlock();

            // Multi line context sensitive text must always be the complete contents of 1 or more lines. So it must always start on a new line.
            // Single line text must always be preceded by something else on the same line and can't span multiple lines.
            if (this.GetCurrChar() == '\n' && introducingPosition != -1)
            {
                this.AdvanceChar();
                this.ScanMultiLineTextBlock(introducingPosition);
            }
            else
            {
                this.ScanSingleLineTextBlock(allowSemicolonToTerminate: allowSemicolonToTerminate, allowRightBraceToTerminate: allowRightBraceToTerminate, allowIfConditionPipeToTerminate: allowIfConditionPipeToTerminate);
            }

            this.FinishRescanAsTextBlock();
        }

        public bool IsAtStartOfLine => this.source.IsAtStartOfLine(this.tokenStartPosition);

        public bool IsIndentedOnSubsequentLineFrom(int basePosition)
        {
            return this.source.IsIndentedOnSubsequentLine(basePosition, this.tokenStartPosition);
        }

        public void RescanAsMultiLineTextBlock(int introducingPosition)
        {
            this.StartRescanAsTextBlock();

            // Multi line context sensitive text must always be the complete contents of 1 or more lines. So it must always start on a new line.
            // Single line text must always be preceded by something else on the same line and can't span multiple lines.
            if (this.GetCurrChar() == '\n')
            {
                this.AdvanceChar();
                this.ScanMultiLineTextBlock(introducingPosition);
            }
            else
            {
                throw new InvalidOperationException("RescanAsMultiLineTextBlock called when token isn't a multiline text block");
            }

            this.FinishRescanAsTextBlock();
        }

        private void StartRescanAsTextBlock()
        {
            this.position = this.PrevTokenEndPosition;

            // Skip leading whitespace, until the first content or end of line
            this.SkipSpaces();
        }

        private void FinishRescanAsTextBlock()
        {
            // Trim trailing whitespace
            while (this.position > this.tokenStartPosition)
            {
                if (this.source.IsSpaceOrNewlineAt(this.position - 1))
                {
                    --this.position;
                }
                else
                {
                    break;
                }
            }

            // For TextBlock tokens, there's nothing stored in the token value & need to get the source span to get the text
            this.StringValue = string.Empty;
            this.type = TokenType.TextBlock;
        }

        public void AdvanceForBracketedTextBlock()
        {
            this.prevTokenEndPosition = this.position;
            this.tokenStartPosition = this.position;

            int nestedBrackets = 0;
            while (true)
            {
                char currChar = this.GetCurrChar();

                // For a backslash, always skip the character following including both in the text block. It's then
                // up to the custom literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\')
                {
                    this.AdvanceChar();
                    if (this.GetCurrChar() != '\n')
                    {
                        this.AdvanceChar();
                    }
                }
                else if (currChar == '[')
                {
                    this.AdvanceChar();
                    ++nestedBrackets;
                }
                else if (currChar == ']')
                {
                    if (nestedBrackets == 0)
                    {
                        break;
                    }
                    else
                    {
                        --nestedBrackets;
                        this.AdvanceChar();
                    }
                }
                else if (currChar == '\0')
                {
                    break;
                }
                else
                {
                    this.AdvanceChar();
                }
            }

            this.StringValue = string.Empty;  // For TextBlock tokens, there's nothing stored in the token value & need to get the source span to get the text
            this.type = TokenType.TextBlock;
        }

        /*
         *
            prop: foo(val:24dp val2:The Label)[The content value]

            prop: foo(val:[24dp] val2:[The Label])[The content value]

            prop: foo{val:[24dp] val2:[The Label]}[The content value]

            prop: foo{val:24dp val2:The Label}
            prop:
              foo val:24dp val2:[The Label] val3:[The value]

            prop:
              foo val:24dp val2:[The Label] val3:[The value]

            prop:
              foo
                val:24dp
                val2:[The Label]
                val3:[The value]

            prop: foo([24dp] [The Label])

            prop: foo{val:34 val2:theValue}

            prop: Delay{amount:[23m]}

            prop:
              Delay amount:23m

            prop: Delay{amount:23m}

            foo{val:[24dp] val2:[The Label]}
         */

        private void ScanSingleLineTextBlock(bool allowSemicolonToTerminate = false, bool allowRightBraceToTerminate = false, bool allowIfConditionPipeToTerminate = false)
        {
            this.tokenStartPosition = this.position;

            DelimiterStack delimiterStack = new DelimiterStack();
            while (true)
            {
                char currChar = this.GetCurrChar();

                // For a backslash, always skip the character following including both in the property value. It's then
                // up to the property literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\')
                {
                    this.AdvanceChar();
                    if (this.GetCurrChar() != '\n')
                    {
                        this.AdvanceChar();
                    }
                }
                else if (currChar == ';' && allowSemicolonToTerminate && delimiterStack.IsEmpty)
                {
                    return;
                }
                else if (currChar == '{')
                {
                    this.AdvanceChar();
                    delimiterStack.Push(currChar);
                }
                else if (currChar == '}')
                {
                    if (allowRightBraceToTerminate && delimiterStack.IsEmpty)
                    {
                        return;
                    }

                    delimiterStack.PopUntil('{');
                    this.AdvanceChar();
                }
                else if (currChar == '[')
                {
                    this.AdvanceChar();
                    delimiterStack.Push(currChar);
                }
                else if (currChar == ']')
                {
                    delimiterStack.PopUntil(']');
                    this.AdvanceChar();
                }
                else if (currChar == '\n' || currChar == '\0')
                {
                    return;
                }
                else
                {
                    this.AdvanceChar();
                }
            }
        }

        private void ScanMultiLineTextBlock(int introducingPosition)
        {
            // Skip any leading whitespace and blank lines
            this.SkipSpacesAndNewlines();

            this.tokenStartPosition = this.position;

            int introducingColumn = this.source.GetColumn(introducingPosition);
            while (true)
            {
                this.SkipSpaces();

                char currChar = this.GetCurrChar();

                // If a line is completely blank, move on to the next, no matter how much it's indented
                if (currChar == '\n')
                {
                    this.AdvanceChar();
                    continue;
                }

                // If at end of file, stop
                if (currChar == '\0')
                {
                    break;
                }

                // If the current indent isn't more than the introducing line, we're done
                if (this.source.GetColumn(this.position) <= introducingColumn)
                {
                    break;
                }

                // Read the rest of the line
                while (currChar != '\n' && currChar != '\0')
                {
                    this.AdvanceChar();
                    currChar = this.GetCurrChar();
                }

                if (currChar == '\n')
                {
                    this.AdvanceChar();
                }
            }
        }

        private int AdvanceForIndentWhitespace()
        {
            int indentAmount = 0;
            while (true)
            {
                char currChar = this.GetCurrChar();

                if (currChar == ' ')
                {
                    ++indentAmount;
                    this.AdvanceChar();
                }
                else if (currChar == '\t')
                {
                    // Tabs are strongly discouraged but, when present, are assumed to be 4 spaces
                    int additionalIndent = 4 - indentAmount % 4;

                    indentAmount += additionalIndent;
                    this.AdvanceChar();
                }
                else if (currChar == '\r')
                {
                    this.AdvanceChar();   // Ignore carriage returns
                }
                else
                {
                    break;
                }
            }

            return indentAmount;
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsPropertyIdentifier()
        {
            int peekPosition = this.position;

            // Skip whitespace
            while (this.source.IsSpaceOrNewlineAt(peekPosition))
            {
                ++peekPosition;
            }

            // See if there's a property name, ending with a colon
            char initialChar = this.source.GetCharAt(peekPosition++);
            if (!IsPropertyIdentifierInitialCharacter(initialChar))
            {
                return false;
            }

            while (IsPropertyIdentifierCharacter(this.source.GetCharAt(peekPosition)))
            {
                ++peekPosition;
            }

            return this.source.GetCharAt(peekPosition) == ':';
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsLeftBracket()
        {
            int peekPosition = this.position;

            // Skip whitespace
            while (this.source.IsSpaceOrNewlineAt(peekPosition))
            {
                ++peekPosition;
            }

            return this.source.GetCharAt(peekPosition++) == '[';
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsRightBrace()
        {
            int peekPosition = this.position;

            // Skip whitespace
            while (this.source.IsSpaceOrNewlineAt(peekPosition))
            {
                ++peekPosition;
            }

            return this.source.GetCharAt(peekPosition++) == '}';
        }

        public bool LookaheadIsNewline()
        {
            int peekPosition = this.position;

            // Skip whitespace
            while (this.source.IsSpaceAt(peekPosition))
            {
                ++peekPosition;
            }

            return this.source.IsNewlineAt(peekPosition);
        }

        public bool IsQualifiedIdentifier()
        {
            if (this.type != TokenType.Identifier)
            {
                return false;
            }

            return this.GetLookaheadAt(0) == '.' && IsIdentifierInitialCharacter(this.GetLookaheadAt(1));
        }

        public ArgumentLookahead GetArgumentLookahead()
        {
            int savedPosition = this.position;

            try
            {
                // Skip any whitespace on the same line
                this.SkipSpaces();

                int indentAmount = -1;    // Set to indent for indented content and -1 if content not at beginning of line

                // If at beginning of a line, skip any initial blank lines until we get a line with content, remembering its indent
                if (this.GetCurrChar() == '\n')
                {
                    this.AdvanceChar();

                    while (true)
                    {
                        indentAmount = this.AdvanceForIndentWhitespace();
                        if (this.GetCurrChar() != '\n')
                        {
                            break;
                        }

                        this.AdvanceChar();
                    }
                }

                char currChar = this.GetCurrChar();

                if (currChar == ':' && this.GetCurrCharPlusOne() == ':')
                {
                    return new ArgumentLookahead(ArgumentLookaheadType.DoubleColon);
                }
                else if (IsPropertyIdentifierInitialCharacter(currChar))
                {
                    this.AdvanceChar();
                    while (IsPropertyIdentifierCharacter(this.GetCurrChar()))
                    {
                        this.AdvanceChar();
                    }

                    if (this.GetCurrChar() == ':')
                    {
                        return new ArgumentLookahead(ArgumentLookaheadType.PropertyIdentifier);
                    }
                }
                else if (currChar == '\0')
                {
                    return new ArgumentLookahead(ArgumentLookaheadType.None);
                }

                if (indentAmount != -1)
                {
                    return new ArgumentLookahead(indentAmount);
                }
                else
                {
                    return new ArgumentLookahead(ArgumentLookaheadType.None);
                }
            }
            finally
            {
                this.position = savedPosition;
            }
        }

        public bool LookaheadIsLeftBrace()
        {
            int peekPosition = this.position;

            while (this.source.IsSpaceOrNewlineAt(peekPosition))
            {
                ++peekPosition;
            }

            return this.source.GetCharAt(peekPosition) == '{';
        }

        public bool LooksLikeStartOfExpression()
        {
            if (this.Type == TokenType.LeftBrace || this.Type == TokenType.If)
            {
                return true;
            }

            // If the value starts with an identifier, see if it looks like a function invocation
            if (this.Type == TokenType.Identifier)
            {
                var remainingSourceTextSubsequence =
                    new ParseableSource(this.source.SourceText, this.position, this.source.EndPosition);
                var lookaheadToken = new Token(remainingSourceTextSubsequence);

                if (lookaheadToken.Type == TokenType.LeftBrace)
                {
                    return true;
                }
            }

            return false;

            /*
            int peekOffset = 0;

            while (isSpaceOrTabOrNewline(getLookaheadAt(peekOffset)))
                ++peekOffset;

            char initialChar = getLookaheadAt(peekOffset++);

            // If the value starts with a left brace, we treat that as an expression
            if (initialChar == '{')
                return true;

            // Otherwise, see if there's a function name followed by (optional) spaces followed by a left brace

            if (!isIdentifierInitialCharacter(initialChar))
                return false;
            while (isIdentifierCharacter(getLookaheadAt(peekOffset)))
                ++peekOffset;

            while (isSpaceOrTabOrNewline(getLookaheadAt(peekOffset)))
                ++peekOffset;

            return getLookaheadAt(peekOffset) == '{';
            */
        }

        public bool ArgumentValueLookaheadIsExpression()
        {
            var remainingSourceTextSubsequence =
                new ParseableSource(this.source.SourceText, this.position, this.source.EndPosition);
            var lookaheadToken = new Token(remainingSourceTextSubsequence);

            // If the value starts with a left brace, we treat that as an expression
            if (lookaheadToken.Type == TokenType.LeftBrace)
            {
                return true;
            }

            // If the value starts with an identifier, see if it looks like a function invocation (meaning it's followed by a left brace or a property identifier)
            if (lookaheadToken.Type == TokenType.Identifier)
            {
                lookaheadToken.Advance();

                TokenType tokenType = lookaheadToken.Type;
                if (tokenType == TokenType.LeftBrace || tokenType == TokenType.PropertyIdentifier)
                {
                    return true;
                }
            }

            return false;

            /*
            int peekOffset = 0;

            while (isSpaceOrTabOrNewline(getLookaheadAt(peekOffset)))
                ++peekOffset;

            char initialChar = getLookaheadAt(peekOffset++);

            // If the value starts with a left brace, we treat that as an expression
            if (initialChar == '{')
                return true;

            // Otherwise, see if there's a function name followed by (optional) spaces followed by a left brace

            if (!isIdentifierInitialCharacter(initialChar))
                return false;
            while (isIdentifierCharacter(getLookaheadAt(peekOffset)))
                ++peekOffset;

            while (isSpaceOrTabOrNewline(getLookaheadAt(peekOffset)))
                ++peekOffset;

            return getLookaheadAt(peekOffset) == '{';
            */
        }

        private static bool IsIdentifierInitialCharacter(char c)
        {
            return ParseableSource.IsLetter(c) || c == '_';
        }

        private static bool IsIdentifierCharacter(char c)
        {
            return ParseableSource.IsLetter(c) || ParseableSource.IsDigit(c) || c == '_' || c == '-';
        }

        private static bool IsPropertyIdentifierInitialCharacter(char c)
        {
            return ParseableSource.IsLetter(c) || c == '_';
        }

        private static bool IsPropertyIdentifierCharacter(char c)
        {
            return IsIdentifierCharacter(c) || c == '.';
        }

        public void Advance()
        {
            this.prevTokenEndPosition = this.position;

            this.SkipWhitespaceAndComments();

            this.tokenStartPosition = this.position;
            char currChar = this.GetCurrChar();
            switch (currChar)
            {
                case '{':
                    this.AdvanceChar();
                    this.type = TokenType.LeftBrace;
                    break;

                case '}':
                    this.AdvanceChar();
                    this.type = TokenType.RightBrace;
                    break;

                case '[':
                    this.AdvanceChar();
                    this.type = TokenType.LeftBracket;
                    break;

                case ']':
                    this.AdvanceChar();
                    this.type = TokenType.RightBracket;
                    break;

                case '(':
                    this.AdvanceChar();
                    this.type = TokenType.LeftParen;
                    break;

                case ')':
                    this.AdvanceChar();
                    this.type = TokenType.RightParen;
                    break;

                case ',':
                    this.AdvanceChar();
                    this.type = TokenType.Comma;
                    break;

                case ':':
                    this.AdvanceChar();
                    this.type = TokenType.Colon;
                    break;

                case ';':
                    this.AdvanceChar();
                    this.type = TokenType.Semicolon;
                    break;

                case '.':
                    if (this.GetCurrCharPlus(1) == '.' && this.GetCurrCharPlus(2) == '.')
                    {
                        this.AdvanceChar();
                        this.AdvanceChar();
                        this.AdvanceChar();
                        this.type = TokenType.Ellipsis;
                    }
                    else
                    {
                        this.AdvanceChar();
                        this.type = TokenType.Period;
                    }

                    break;

                case '-':
                    this.AdvanceChar();
                    this.type = TokenType.Minus;
                    break;

                case '!':
                    this.AdvanceChar();
                    if (this.GetCurrChar() == '=')
                    {
                        this.AdvanceChar();
                        this.type = TokenType.NotEquals;
                    }
                    else
                    {
                        this.type = TokenType.Not;
                    }

                    break;

                case '*':
                    this.AdvanceChar();
                    this.type = TokenType.Times;
                    break;

                case '/':
                    this.AdvanceChar();
                    this.type = TokenType.Divide;
                    break;

                case '%':
                    this.AdvanceChar();
                    this.type = TokenType.Remainder;
                    break;

                case '+':
                    this.AdvanceChar();
                    this.type = TokenType.Plus;
                    break;

                case '<':
                    this.AdvanceChar();
                    if (this.GetCurrChar() == '=')
                    {
                        this.AdvanceChar();
                        this.type = TokenType.LessEquals;
                    }
                    else
                    {
                        this.type = TokenType.Less;
                    }

                    break;

                case '>':
                    this.AdvanceChar();
                    if (this.GetCurrChar() == '=')
                    {
                        this.AdvanceChar();
                        this.type = TokenType.GreaterEquals;
                    }
                    else
                    {
                        this.type = TokenType.Greater;
                    }

                    break;

                case '=':
                    this.AdvanceChar();

                    if (this.GetCurrChar() == '=')
                    {
                        this.AdvanceChar();
                        this.type = TokenType.Equals;
                    }
                    else
                    {
                        this.type = TokenType.Assign;
                    }

                    break;

                case '&':
                    this.AdvanceChar();

                    if (this.GetCurrChar() != '&')
                    {
                        this.type = TokenType.Invalid;
                    }
                    else
                    {
                        this.AdvanceChar();
                        this.type = TokenType.And;
                    }

                    break;

                case '|':
                    this.AdvanceChar();

                    if (this.GetCurrChar() != '|')
                    {
                        this.type = TokenType.Pipe;
                    }
                    else
                    {
                        this.AdvanceChar();
                        this.type = TokenType.Or;
                    }

                    break;

                case '\0':
                    this.type = TokenType.Eof;
                    break;

                default:
                    if (ParseableSource.IsLetter(currChar))
                    {
                        this.ReadAlphanumericToken();
                    }
                    else if (ParseableSource.IsDigit(currChar))
                    {
                        this.ReadNumericToken();
                    }
                    else
                    {
                        this.AdvanceChar();
                        this.type = TokenType.Invalid;
                    }

                    break;
            }
        }

        public void ReinterpretAlloWTextualLiteral(bool allowSemicolonToTerminate, bool allowNewlineToTerminate, bool allowRightBraceToTerminate, bool allowRightBracketToTerminate)
        {
            this.position = this.tokenStartPosition;

            StringBuilder buffer = new StringBuilder();

            while (true)
            {
                char currChar = this.GetCurrChar();

                // For a backslash, always skip the character following including both in the property value. It's then
                // up to the property literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\')
                {
                    this.AdvanceChar();
                    char escapedChar = this.GetCurrChar();
                    if (escapedChar == '\n')
                    {
                        buffer.Append('\n');
                    }
                    else
                    {
                        buffer.Append('\\');
                        buffer.Append(escapedChar);
                        this.AdvanceChar();
                    }
                }
                else if (currChar == ';' && allowSemicolonToTerminate)
                {
                    break;
                }
                else if (currChar == '{')
                {
                    break;
                }
                else if (currChar == '}' && allowRightBraceToTerminate)
                {
                    break;
                }
                else if (currChar == ']' && allowRightBracketToTerminate)
                {
                    break;
                }
                else if (currChar == '\0')
                {
                    break;
                }
                else if (currChar == '\n' && allowNewlineToTerminate)
                {
                    break;
                }
                else
                {
                    buffer.Append(currChar);
                    this.AdvanceChar();
                }
            }

            if (buffer.Length == 0)
            {
                this.position = this.prevTokenEndPosition;
                this.Advance();
            }
            else
            {
                this.StringValue = buffer.ToString();
                this.type = TokenType.TextualLiteralText;
            }
        }

        public void RescanPropertyIdentifierAsIdentifier()
        {
            if (this.type != TokenType.PropertyIdentifier)
            {
                throw new InvalidOperationException("Called RescanPropertyIdentifierAsIdentifier when current token isn't a PropertyIdentifier");
            }

            this.position = this.tokenStartPosition;
            int startPosition = this.position;

            while (true)
            {
                char currChar = this.GetCurrChar();
                if (!IsIdentifierCharacter(currChar))
                {
                    break;
                }

                this.AdvanceChar();
            }

            this.StringValue = this.source.Substring(startPosition, this.position - startPosition);
            this.type = TokenType.Identifier;
        }

        private void SkipWhitespaceAndComments()
        {
            while (true)
            {
                char currChar = this.GetCurrChar();
                if (ParseableSource.IsSpaceOrNewline(currChar))
                {
                    this.AdvanceChar();
                }
                else if (currChar == '/' && this.GetCurrCharPlusOne() == '/')
                {
                    this.AdvanceChar();
                    this.AdvanceChar();

                    // Skip the rest of the line
                    char c;
                    do
                    {
                        c = this.GetCurrChar();
                        this.AdvanceChar();
                    }
                    while (c != '\n' && c != '\0');
                }
                else
                {
                    break;
                }
            }
        }

        private void SkipSpaces()
        {
            while (this.source.IsSpaceAt(this.position))
            {
                this.AdvanceChar();
            }
        }

        private void SkipSpacesAndNewlines()
        {
            while (this.source.IsSpaceOrNewlineAt(this.position))
            {
                this.AdvanceChar();
            }
        }

        private char GetCurrChar() => this.source.GetCharAt(this.position);

        private char GetCurrCharPlusOne()
        {
            return this.source.GetCharAt(this.position + 1);
        }

        private char GetCurrCharPlus(int offset)
        {
            return this.source.GetCharAt(this.position + offset);
        }

        private char GetLookaheadAt(int offset)
        {
            return this.source.GetCharAt(this.position + offset);
        }

        /// <summary>
        /// Advance to the next character, skipping over any \r characters and stopping at the end of the source
        /// </summary>
        private void AdvanceChar()
        {
            if (this.position < this.sourceEnd)
            {
                ++this.position;
            }
        }

        private void SetPosition(int position)
        {
            this.position = position;
        }

        private void ReadAlphanumericToken()
        {
            int startPosition = this.position;

            while (true)
            {
                char currChar = this.GetCurrChar();
                if (!IsIdentifierCharacter(currChar))
                {
                    break;
                }

                this.AdvanceChar();
            }

            string tokenText = this.source.Substring(startPosition, this.position - startPosition);

            switch (tokenText)
            {
                case "true":
                    this.type = TokenType.True;
                    break;
                case "false":
                    this.type = TokenType.False;
                    break;
                case "null":
                    this.type = TokenType.Null;
                    break;
                case "type":
                    this.type = TokenType.Type;
                    break;
                case "import":
                    this.type = TokenType.Import;
                    break;
                /*
                case "use":
                    _type = TokenType.USE;
                    break;
                */
                case "if":
                    this.type = TokenType.If;
                    break;
                case "is":
                    this.type = TokenType.Is;
                    break;
                case "else":
                    this.type = TokenType.Else;
                    break;
                case "for":
                    this.type = TokenType.For;
                    break;
                case "in":
                    this.type = TokenType.In;
                    break;
                default:
                    this.StringValue = tokenText;
                    this.type = TokenType.Identifier;

                    // Peek ahead to see if the identifier is actually a property name. Property names can contain
                    // all the characters in normal identifiers (letters, digits, underscore) and also can include
                    // '.' and '-'.  So we peek ahead to see of a valid string of those characters is followed by colon.
                    // If so, it's a property.  So "abc.def" is two identifier tokens with a period between them
                    // while "abc.def:" is a single property identifier token.
                    for (int peekPosition = this.position; peekPosition < this.sourceEnd; ++peekPosition)
                    {
                        char peekCharacter = this.source.GetCharAt(peekPosition);

                        if (peekCharacter == ':')
                        {
                            this.StringValue = this.source.Substring(startPosition, peekPosition - startPosition);
                            this.type = TokenType.PropertyIdentifier;
                            this.SetPosition(peekPosition + 1);
                            break;
                        }

                        if (!IsPropertyIdentifierCharacter(peekCharacter))
                        {
                            break;
                        }
                    }

                    break;
            }
        }

#if false
        private void TransformIntoPropertyIdentifierIfCan() {
            if (_type != TokenType.Identifier)
                return;

            // Property names can contain all the characters in normal identifiers (letters, digits, underscore) and
            // also can include '.' and '-'.  So we peek ahead to see of a valid string of those characters is
            // followed by colon.  If so, it's a property.  So "abc.def" is two identifier tokens with a period
            // between them while "abc.def:" is a single property identifier token.
            for (int peekPosition = _position; peekPosition < _sourceEnd; ++peekPosition) {
                char peekCharacter = GetCharAt(peekPosition);

                if (peekCharacter == ':') {
                    string additionalTokenText = _source.Substring(_position, peekPosition - _position);

                    StringValue = StringValue + additionalTokenText;
                    _type = TokenType.PropertyIdentifier;
                    SetPosition(peekPosition + 1);
                    return;
                }
                else if (!IsPropertyIdentifierCharacter(peekCharacter))
                    return;
            }
        }
#endif

        // TODO: Handle other integer types & negative values
        private void ReadNumericToken()
        {
            var token = new StringBuilder();

            while (true)
            {
                char currChar = this.GetCurrChar();

                if (!ParseableSource.IsDigit(currChar))
                {
                    break;
                }

                token.Append(currChar);
                this.AdvanceChar();
            }

            string tokenString = token.ToString();

            long value = long.Parse(tokenString);

            this.IntValue = (int)value;
            this.type = TokenType.Int32;
        }
    }
}
