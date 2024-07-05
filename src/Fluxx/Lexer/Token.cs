using System;
using System.Text;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;
using System.Diagnostics;

namespace Faml.Lexer {
    public sealed class Token {
        private readonly ParseableSource _source;
        private readonly int _sourceEnd;
        private int _prevTokenEndPosition;
        private int _tokenStartPosition;
        private int _position;
        private TokenType _type;
        private bool _inErrorMode;

        public string StringValue { get; private set; }
        public int IntValue { get; private set; }
        public long LongValue { get; }
        public double DoubleValue { get; }

        public Token(SourceText sourceText) : this(new ParseableSource(sourceText, 0, sourceText.Length)) {}

        public Token(ParseableSource source, bool noInitialAdvance = false) {
            _source = source;
            _sourceEnd = source.EndPosition;

            SetPosition(source.StartPosition);

            _prevTokenEndPosition = -1;

            if (! noInitialAdvance)
                Advance();
        }

        public ParseableSource Source => _source;

        public bool InErrorMode {
            get => _inErrorMode;
            set => _inErrorMode = value;
        }

        /// <summary>
        /// Return the current token type. When in error mode, for panic mode error recovery,
        /// the special token ERROR_MODE is returned. Normally the parsing code treats that token as
        /// special, not matching anything valid, and instead generating a stub item in the AST.
        /// </summary>
        public TokenType Type => _inErrorMode ? TokenType.ErrorMode : _type;

        /// <summary>
        /// Return the current token type, even in error mode. This is normally just used by the sync
        /// code, trying to to match on a valid synchronizing token, for panic mode error recovery,
        /// to then exit error mode.
        /// </summary>
        public TokenType TypeForErrorMode => _type;

        public TokenType TypeAllowPropertyIdentifier {
            get {
                //TransformIntoPropertyIdentifierIfCan();
                return Type;
            }
        }

        public override string ToString() {
            string typeString = _type.ToString();
            if (_type == TokenType.Identifier || _type == TokenType.PropertyIdentifier /* || type == TokenType.FUNCTION_NAME */)
                return typeString + "(" + StringValue + ")";
            else return typeString;
        }

        public int TokenStartPosition => _tokenStartPosition;

        public int TokenStartColumn => _source.GetColumn(_tokenStartPosition);

        public int TokenEndPosition => _position;

        public TextSpan TokenSpan => TextSpan.FromBounds(_tokenStartPosition, _position);

        public TextSpan TokenSourceSpanExceptForLastCharacter => TextSpan.FromBounds(_tokenStartPosition, _position - 1);

        public int TokenLength => _position - _tokenStartPosition;

        public int PrevTokenEndPosition => _prevTokenEndPosition;

        public void RescanAsTextBlock(int introducingPosition, bool allowSemicolonToTerminate = false, bool allowRightBraceToTerminate = false, bool allowIfConditionPipeToTerminate = false) {
            StartRescanAsTextBlock();

            // Multi line context sensitive text must always be the complete contents of 1 or more lines. So it must always start on a new line.
            // Single line text must always be preceded by something else on the same line and can't span multiple lines.
            if (GetCurrChar() == '\n' && introducingPosition != -1) {
                AdvanceChar();
                ScanMultiLineTextBlock(introducingPosition);
            }
            else
                ScanSingleLineTextBlock(allowSemicolonToTerminate: allowSemicolonToTerminate, allowRightBraceToTerminate: allowRightBraceToTerminate, allowIfConditionPipeToTerminate: allowIfConditionPipeToTerminate);

            FinishRescanAsTextBlock();
        }

        public bool IsAtStartOfLine => _source.IsAtStartOfLine(_tokenStartPosition);

        public bool IsIndentedOnSubsequentLineFrom(int basePosition) {
            return _source.IsIndentedOnSubsequentLine(basePosition, _tokenStartPosition);
        }

        public void RescanAsMultiLineTextBlock(int introducingPosition) {
            StartRescanAsTextBlock();

            // Multi line context sensitive text must always be the complete contents of 1 or more lines. So it must always start on a new line.
            // Single line text must always be preceded by something else on the same line and can't span multiple lines.
            if (GetCurrChar() == '\n') {
                AdvanceChar();
                ScanMultiLineTextBlock(introducingPosition);
            }
            else
                throw new InvalidOperationException("RescanAsMultiLineTextBlock called when token isn't a multiline text block");

            FinishRescanAsTextBlock();
        }

        private void StartRescanAsTextBlock() {
            _position = PrevTokenEndPosition;

            // Skip leading whitespace, until the first content or end of line
            SkipSpaces();
        }

        private void FinishRescanAsTextBlock() {
            // Trim trailing whitespace
            while (_position > _tokenStartPosition) {
                if (_source.IsSpaceOrNewlineAt(_position - 1))
                    --_position;
                else break;
            }

            // For TextBlock tokens, there's nothing stored in the token value & need to get the source span to get the text
            StringValue = "";
            _type = TokenType.TextBlock;
        }

        public void AdvanceForBracketedTextBlock() {
            _prevTokenEndPosition = _position;
            _tokenStartPosition = _position;

            int nestedBrackets = 0;
            while (true) {
                char currChar = GetCurrChar();

                // For a backslash, always skip the character following including both in the text block. It's then
                // up to the custom literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\') {
                    AdvanceChar();
                    if (GetCurrChar() != '\n')
                        AdvanceChar();
                }
                else if (currChar == '[') {
                    AdvanceChar();
                    ++nestedBrackets;
                }
                else if (currChar == ']') {
                    if (nestedBrackets == 0)
                        break;
                    else {
                        --nestedBrackets;
                        AdvanceChar();
                    }
                }
                else if (currChar == '\0')
                    break;
                else
                    AdvanceChar();
            }

            StringValue = "";  // For TextBlock tokens, there's nothing stored in the token value & need to get the source span to get the text
            _type = TokenType.TextBlock;
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

        private void ScanSingleLineTextBlock(bool allowSemicolonToTerminate = false, bool allowRightBraceToTerminate = false, bool allowIfConditionPipeToTerminate = false) {
            _tokenStartPosition = _position;

            DelimiterStack delimiterStack = new DelimiterStack();
            while (true) {
                char currChar = GetCurrChar();

                // For a backslash, always skip the character following including both in the property value. It's then
                // up to the property literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\') {
                    AdvanceChar();
                    if (GetCurrChar() != '\n')
                        AdvanceChar();
                }
                else if (currChar == ';' && allowSemicolonToTerminate && delimiterStack.IsEmpty)
                    return;
                else if (currChar == '{') {
                    AdvanceChar();
                    delimiterStack.Push(currChar);
                }
                else if (currChar == '}') {
                    if (allowRightBraceToTerminate && delimiterStack.IsEmpty)
                        return;
                    delimiterStack.PopUntil('{');
                    AdvanceChar();
                }
                else if (currChar == '[') {
                    AdvanceChar();
                    delimiterStack.Push(currChar);
                }
                else if (currChar == ']') {
                    delimiterStack.PopUntil(']');
                    AdvanceChar();
                }
                else if (currChar == '\n' || currChar == '\0')
                    return;
                else
                    AdvanceChar();
            }
        }

        private void ScanMultiLineTextBlock(int introducingPosition) {
            // Skip any leading whitespace and blank lines
            SkipSpacesAndNewlines();

            _tokenStartPosition = _position;

            int introducingColumn = _source.GetColumn(introducingPosition);
            while (true) {
                SkipSpaces();

                char currChar = GetCurrChar();

                // If a line is completely blank, move on to the next, no matter how much it's indented
                if (currChar == '\n') {
                    AdvanceChar();
                    continue;
                }

                // If at end of file, stop
                if (currChar == '\0')
                    break;

                // If the current indent isn't more than the introducing line, we're done
                if (_source.GetColumn(_position) <= introducingColumn)
                    break;

                // Read the rest of the line
                while (currChar != '\n' && currChar != '\0') {
                    AdvanceChar();
                    currChar = GetCurrChar();
                }
                if (currChar == '\n')
                    AdvanceChar();
            }
        }

        private int AdvanceForIndentWhitespace() {
            int indentAmount = 0;
            while (true) {
                char currChar = GetCurrChar();

                if (currChar == ' ') {
                    ++indentAmount;
                    AdvanceChar();
                }
                else if (currChar == '\t') {
                    // Tabs are strongly discouraged but, when present, are assumed to be 4 spaces
                    int additionalIndent = 4 - indentAmount % 4;

                    indentAmount += additionalIndent;
                    AdvanceChar();
                }
                else if (currChar == '\r') {
                    AdvanceChar();   // Ignore carriage returns
                }
                else break;
            }
            return indentAmount;
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsPropertyIdentifier() {
            int peekPosition = _position;

            // Skip whitespace
            while (_source.IsSpaceOrNewlineAt(peekPosition))
                ++peekPosition;

            // See if there's a property name, ending with a colon
            char initialChar = _source.GetCharAt(peekPosition++);
            if (!IsPropertyIdentifierInitialCharacter(initialChar))
                return false;

            while (IsPropertyIdentifierCharacter(_source.GetCharAt(peekPosition)))
                ++peekPosition;

            return _source.GetCharAt(peekPosition) == ':';
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsLeftBracket() {
            int peekPosition = _position;

            // Skip whitespace
            while (_source.IsSpaceOrNewlineAt(peekPosition))
                ++peekPosition;

            return _source.GetCharAt(peekPosition++) == '[';
        }

        /// <summary>
        /// Lookahead to see if the next token that will be read is a PROPERTY token.
        /// </summary>
        /// <returns>true iff the next token will be a PROPERTY</returns>
        public bool LookaheadIsRightBrace() {
            int peekPosition = _position;

            // Skip whitespace
            while (_source.IsSpaceOrNewlineAt(peekPosition))
                ++peekPosition;

            return _source.GetCharAt(peekPosition++) == '}';
        }

        public bool LookaheadIsNewline() {
            int peekPosition = _position;

            // Skip whitespace
            while (_source.IsSpaceAt(peekPosition))
                ++peekPosition;

            return _source.IsNewlineAt(peekPosition);
        }

        public bool IsQualifiedIdentifier() {
            if (_type != TokenType.Identifier)
                return false;

            return GetLookaheadAt(0) == '.' && IsIdentifierInitialCharacter(GetLookaheadAt(1));
        }

        public ArgumentLookahead GetArgumentLookahead() {
            int savedPosition = _position;

            try {
                // Skip any whitespace on the same line
                SkipSpaces();

                int indentAmount = -1;    // Set to indent for indented content and -1 if content not at beginning of line

                // If at beginning of a line, skip any initial blank lines until we get a line with content, remembering its indent
                if (GetCurrChar() == '\n') {
                    AdvanceChar();

                    while (true) {
                        indentAmount = AdvanceForIndentWhitespace();
                        if (GetCurrChar() != '\n')
                            break;
                        AdvanceChar();
                    }
                }

                char currChar = GetCurrChar();

                if (currChar == ':' && GetCurrCharPlusOne() == ':')
                    return new ArgumentLookahead(ArgumentLookaheadType.DoubleColon);
                else if (IsPropertyIdentifierInitialCharacter(currChar)) {
                    AdvanceChar();
                    while (IsPropertyIdentifierCharacter(GetCurrChar()))
                        AdvanceChar();
                    if (GetCurrChar() == ':')
                        return new ArgumentLookahead(ArgumentLookaheadType.PropertyIdentifier);
                }
                else if (currChar == '\0')
                    return new ArgumentLookahead(ArgumentLookaheadType.None);

                if (indentAmount != -1)
                    return new ArgumentLookahead(indentAmount);
                else return new ArgumentLookahead(ArgumentLookaheadType.None);
            }
            finally {
                _position = savedPosition;
            }
        }

        public bool LookaheadIsLeftBrace() {
            int peekPosition = _position;

            while (_source.IsSpaceOrNewlineAt(peekPosition))
                ++peekPosition;

            return _source.GetCharAt(peekPosition) == '{';
        }

        public bool LooksLikeStartOfExpression() {
            if (Type == TokenType.LeftBrace || Type == TokenType.If)
                return true;

            // If the value starts with an identifier, see if it looks like a function invocation
            if (Type == TokenType.Identifier) {
                var remainingSourceTextSubsequence =
                    new ParseableSource(_source.SourceText, _position, _source.EndPosition);
                var lookaheadToken = new Token(remainingSourceTextSubsequence);

                if (lookaheadToken.Type == TokenType.LeftBrace)
                    return true;
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

        public bool ArgumentValueLookaheadIsExpression() {
            var remainingSourceTextSubsequence =
                new ParseableSource(_source.SourceText, _position, _source.EndPosition);
            var lookaheadToken = new Token(remainingSourceTextSubsequence);

            // If the value starts with a left brace, we treat that as an expression
            if (lookaheadToken.Type == TokenType.LeftBrace)
                return true;

            // If the value starts with an identifier, see if it looks like a function invocation (meaning it's followed by a left brace or a property identifier)
            if (lookaheadToken.Type == TokenType.Identifier) {
                lookaheadToken.Advance();

                TokenType tokenType = lookaheadToken.Type;
                if (tokenType == TokenType.LeftBrace || tokenType == TokenType.PropertyIdentifier)
                    return true;
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

        private static bool IsIdentifierInitialCharacter(char c) {
            return ParseableSource.IsLetter(c) || c == '_';
        }

        private static bool IsIdentifierCharacter(char c) {
            return ParseableSource.IsLetter(c) || ParseableSource.IsDigit(c) || c == '_' || c == '-';
        }

        private static bool IsPropertyIdentifierInitialCharacter(char c) {
            return ParseableSource.IsLetter(c) || c == '_';
        }

        private static bool IsPropertyIdentifierCharacter(char c) {
            return IsIdentifierCharacter(c) || c == '.';
        }

        public void Advance() {
            _prevTokenEndPosition = _position;

            SkipWhitespaceAndComments();

            _tokenStartPosition = _position;
            char currChar = GetCurrChar();
            switch (currChar) {
                case '{':
                    AdvanceChar();
                    _type = TokenType.LeftBrace;
                    break;

                case '}':
                    AdvanceChar();
                    _type = TokenType.RightBrace;
                    break;

                case '[':
                    AdvanceChar();
                    _type = TokenType.LeftBracket;
                    break;

                case ']':
                    AdvanceChar();
                    _type = TokenType.RightBracket;
                    break;

                case '(':
                    AdvanceChar();
                    _type = TokenType.LeftParen;
                    break;

                case ')':
                    AdvanceChar();
                    _type = TokenType.RightParen;
                    break;

                case ',':
                    AdvanceChar();
                    _type = TokenType.Comma;
                    break;

                case ':':
                    AdvanceChar();
                    _type = TokenType.Colon;
                    break;

                case ';':
                    AdvanceChar();
                    _type = TokenType.Semicolon;
                    break;

                case '.':
                    if (GetCurrCharPlus(1) == '.' && GetCurrCharPlus(2) == '.') {
                        AdvanceChar();
                        AdvanceChar();
                        AdvanceChar();
                        _type = TokenType.Ellipsis;
                    }
                    else {
                        AdvanceChar();
                        _type = TokenType.Period;
                    }
                    break;

                case '-':
                    AdvanceChar();
                    _type = TokenType.Minus;
                    break;

                case '!':
                    AdvanceChar();
                    if (GetCurrChar() == '=') {
                        AdvanceChar();
                        _type = TokenType.NotEquals;
                    }
                    else {
                        _type = TokenType.Not;
                    }
                    break;

                case '*':
                    AdvanceChar();
                    _type = TokenType.Times;
                    break;

                case '/':
                    AdvanceChar();
                    _type = TokenType.Divide;
                    break;

                case '%':
                    AdvanceChar();
                    _type = TokenType.Remainder;
                    break;

                case '+':
                    AdvanceChar();
                    _type = TokenType.Plus;
                    break;

                case '<':
                    AdvanceChar();
                    if (GetCurrChar() == '=') {
                        AdvanceChar();
                        _type = TokenType.LessEquals;
                    }
                    else {
                        _type = TokenType.Less;
                    }
                    break;

                case '>':
                    AdvanceChar();
                    if (GetCurrChar() == '=') {
                        AdvanceChar();
                        _type = TokenType.GreaterEquals;
                    }
                    else {
                        _type = TokenType.Greater;
                    }
                    break;

                case '=':
                    AdvanceChar();

                    if (GetCurrChar() == '=') {
                        AdvanceChar();
                        _type = TokenType.Equals;
                    }
                    else {
                        _type = TokenType.Assign;
                    }
                    break;

                case '&':
                    AdvanceChar();

                    if (GetCurrChar() != '&')
                        _type = TokenType.Invalid;
                    else {
                        AdvanceChar();
                        _type = TokenType.And;
                    }
                    break;

                case '|':
                    AdvanceChar();

                    if (GetCurrChar() != '|')
                        _type = TokenType.Pipe;
                    else {
                        AdvanceChar();
                        _type = TokenType.Or;
                    }
                    break;

                case '\0':
                    _type = TokenType.Eof;
                    break;

                default:
                    if (ParseableSource.IsLetter(currChar))
                        ReadAlphanumericToken();
                    else if (ParseableSource.IsDigit(currChar))
                        ReadNumericToken();
                    else {
                        AdvanceChar();
                        _type = TokenType.Invalid;
                    }
                    break;
            }
        }

        public void ReinterpretAlloWTextualLiteral(bool allowSemicolonToTerminate, bool allowNewlineToTerminate, bool allowRightBraceToTerminate, bool allowRightBracketToTerminate) {
            _position = _tokenStartPosition;

            StringBuilder buffer = new StringBuilder();

            while (true) {
                char currChar = GetCurrChar();

                // For a backslash, always skip the character following including both in the property value. It's then
                // up to the property literal value parser code to later treat the backslash and following character how
                // it wants. However, a newline still terminates the text, even if there's a backslash before it
                if (currChar == '\\') {
                    AdvanceChar();
                    char escapedChar = GetCurrChar();
                    if (escapedChar == '\n') {
                        buffer.Append('\n');
                    }
                    else {
                        buffer.Append('\\');
                        buffer.Append(escapedChar);
                        AdvanceChar();
                    }
                }
                else if (currChar == ';' && allowSemicolonToTerminate)
                    break;
                else if (currChar == '{')
                    break;
                else if (currChar == '}' && allowRightBraceToTerminate)
                    break;
                else if (currChar == ']' && allowRightBracketToTerminate)
                    break;
                else if (currChar == '\0')
                    break;
                else if (currChar == '\n' && allowNewlineToTerminate)
                    break;
                else {
                    buffer.Append(currChar);
                    AdvanceChar();
                }
            }

            if (buffer.Length == 0) {
                _position = _prevTokenEndPosition;
                Advance();
            }
            else {
                StringValue = buffer.ToString();
                _type = TokenType.TextualLiteralText;
            }
        }

        public void RescanPropertyIdentifierAsIdentifier() {
            if (_type != TokenType.PropertyIdentifier)
                throw new InvalidOperationException("Called RescanPropertyIdentifierAsIdentifier when current token isn't a PropertyIdentifier");

            _position = _tokenStartPosition;
            int startPosition = _position;

            while (true) {
                char currChar = GetCurrChar();
                if (!IsIdentifierCharacter(currChar))
                    break;
                AdvanceChar();
            }

            StringValue = _source.Substring(startPosition, _position - startPosition);
            _type = TokenType.Identifier;
        }

        private void SkipWhitespaceAndComments() {
            while (true) {
                char currChar = GetCurrChar();
                if (ParseableSource.IsSpaceOrNewline(currChar))
                    AdvanceChar();
                else if (currChar == '/' && GetCurrCharPlusOne() == '/') {
                    AdvanceChar();
                    AdvanceChar();

                    // Skip the rest of the line
                    char c;
                    do {
                        c = GetCurrChar();
                        AdvanceChar();
                    } while (c != '\n' && c != '\0');
                }
                else break;
            }
        }

        private void SkipSpaces() {
            while (_source.IsSpaceAt(_position))
                AdvanceChar();
        }

        private void SkipSpacesAndNewlines() {
            while (_source.IsSpaceOrNewlineAt(_position))
                AdvanceChar();
        }

        private char GetCurrChar() => _source.GetCharAt(_position);

        private char GetCurrCharPlusOne() {
            return _source.GetCharAt(_position + 1);
        }

        private char GetCurrCharPlus(int offset) {
            return _source.GetCharAt(_position + offset);
        }

        private char GetLookaheadAt(int offset) {
            return _source.GetCharAt(_position + offset);
        }

        /// <summary>
        /// Advance to the next character, skipping over any \r characters and stopping at the end of the source
        /// </summary>
        private void AdvanceChar() {
            if (_position < _sourceEnd)
                ++_position;
        }

        private void SetPosition(int position) {
            _position = position;
        }

        private void ReadAlphanumericToken() {
            int startPosition = _position;

            while (true) {
                char currChar = GetCurrChar();
                if (!IsIdentifierCharacter(currChar))
                    break;
                AdvanceChar();
            }

            string tokenText = _source.Substring(startPosition, _position - startPosition);

            switch (tokenText) {
                case "true":
                    _type = TokenType.True;
                    break;
                case "false":
                    _type = TokenType.False;
                    break;
                case "null":
                    _type = TokenType.Null;
                    break;
                case "type":
                    _type = TokenType.Type;
                    break;
                case "import":
                    _type = TokenType.Import;
                    break;
                /*
                case "use":
                    _type = TokenType.USE;
                    break;
                */
                case "if":
                    _type = TokenType.If;
                    break;
                case "is":
                    _type = TokenType.Is;
                    break;
                case "else":
                    _type = TokenType.Else;
                    break;
                case "for":
                    _type = TokenType.For;
                    break;
                case "in":
                    _type = TokenType.In;
                    break;
                default:
                    StringValue = tokenText;
                    _type = TokenType.Identifier;

                    // Peek ahead to see if the identifier is actually a property name. Property names can contain
                    // all the characters in normal identifiers (letters, digits, underscore) and also can include
                    // '.' and '-'.  So we peek ahead to see of a valid string of those characters is followed by colon.
                    // If so, it's a property.  So "abc.def" is two identifier tokens with a period between them
                    // while "abc.def:" is a single property identifier token.
                    for (int peekPosition = _position; peekPosition < _sourceEnd; ++peekPosition) {
                        char peekCharacter = _source.GetCharAt(peekPosition);

                        if (peekCharacter == ':') {
                            StringValue = _source.Substring(startPosition, peekPosition - startPosition);
                            _type = TokenType.PropertyIdentifier;
                            SetPosition(peekPosition + 1);
                            break;
                        }
                        if (!IsPropertyIdentifierCharacter(peekCharacter))
                            break;
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
        private void ReadNumericToken() {
            var token = new StringBuilder();

            while (true) {
                char currChar = GetCurrChar();

                if (!ParseableSource.IsDigit(currChar)) {
                    break;
                }

                token.Append(currChar);
                AdvanceChar();
            }

            string tokenString = token.ToString();

            long value = long.Parse(tokenString);

            IntValue = (int) value;
            _type = TokenType.Int32;
        }
    }
}
