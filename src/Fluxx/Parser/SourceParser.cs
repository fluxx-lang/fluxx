using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis;
using Faml.CodeAnalysis.Text;
using Faml.Lexer;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Operator;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Parser {
    public sealed class SourceParser {
        private readonly Token _token;
        private readonly ParseableSource _source;
        private readonly FamlModule _module;


        public static FamlModule ParseModule(FamlProject project, QualifiableName moduleName, SourceText sourceText) {
            var module = new FamlModule(project, moduleName, sourceText);
            var parser = new SourceParser(module);
            parser.ParseModule();
            return module;
        }

        public static ExpressionSyntax ParseTextBlockExpression(FamlModule module, TextSpan sourceSpan, TypeBinding typeBinding) {
            var token = new Token(new ParseableSource(module.SourceText, sourceSpan), true);

            if (token.ArgumentValueLookaheadIsExpression())
                return ParseTextBlockExpression(module, sourceSpan);
            else if (typeBinding == BuiltInTypeBinding.String)
                return ParseStringLiteral(module, sourceSpan);
            else if (typeBinding is InvalidTypeBinding) {
                string text = ParseUnknownTextBlock(module, sourceSpan);
                return new InvalidExpressionSyntax(sourceSpan, text, typeBinding);
            }
            else if (typeBinding is EnumTypeBinding enumTypeBinding)
                return enumTypeBinding.ParseEnumValue(module, sourceSpan);
            else if (typeBinding is ObjectTypeBinding objectTypeBinding && objectTypeBinding.SupportsCreateLiteral())
                return objectTypeBinding.ParseLiteralValueSource(module, sourceSpan);
            else return ParseTextBlockExpression(module, sourceSpan);
        }

        public static ExpressionSyntax ParseTextBlockExpression(FamlModule module, TextSpan sourceSpan) {
            var parser = new SourceParser(module, sourceSpan);
            return parser.ParseExpression(new Tokens(TokenType.Eof));
        }

        public static ExpressionSyntax ParseSingleBooleanLiteral(FamlModule module, TextSpan sourceSpan) {
            var parser = new SourceParser(module, sourceSpan);

            ExpressionSyntax expression = parser.ParseBooleanLiteral();

            if (parser._token.Type != TokenType.Eof && !parser._token.InErrorMode) {
                parser.UnexpectedToken("end of value");
            }

            return expression;
        }

        public static ExpressionSyntax ParseSingleIntLiteral(FamlModule module, TextSpan sourceSpan) {
            var parser = new SourceParser(module, sourceSpan);

            ExpressionSyntax expression = parser.ParseIntLiteral();
            if (parser._token.Type != TokenType.Eof && !parser._token.InErrorMode) {
                parser.UnexpectedToken("end of value");
            }

            return expression;
        }

        public static ExpressionSyntax? ParseLiteralConstructorTextBlockExpression(FamlModule module, TextSpan span, TypeBinding typeBinding) {
            if (typeBinding == BuiltInTypeBinding.String)
                return ParseStringLiteral(module, span);
            else if (typeBinding is InvalidTypeBinding) {
                string text = ParseUnknownTextBlock(module, span);
                return new InvalidExpressionSyntax(span, text, typeBinding);
            }
            else if (typeBinding is EnumTypeBinding enumTypeBinding)
                return enumTypeBinding.ParseEnumValue(module, span);
            else if (typeBinding is ObjectTypeBinding objectTypeBinding &&
                     objectTypeBinding.SupportsCreateLiteral())
                return objectTypeBinding.ParseLiteralValueSource(module, span);
            else return null;
        }

        /// <summary>
        /// Parse source which just contains an expression. This method is only used for test purposes.
        /// </summary>
        /// <param name="module">module (for testing, it can be anything)</param>
        /// <param name="expressionSource">source string</param>
        /// <returns></returns>
        public static ExpressionSyntax ParseExpression(FamlModule module, string expressionSource) {
            var parser = new SourceParser(module, new Token(SourceText.From(expressionSource)));
            return parser.ParseExpression(new Tokens(TokenType.Eof));
        }

        private static ExpressionSyntax ParseStringLiteral(FamlModule module, TextSpan span) {
            var literalValueCharIterator = new LiteralValueCharIterator(module.SourceText, span);

            var stringFragments = new List<InterpolatedStringFragmentSyntax>();
            var expressions = new List<ExpressionSyntax>();

            int fragmentStartPosition = span.Start;
            var buffer = new StringBuilder();
            while (true) {
                char curr = literalValueCharIterator.ReadChar();

                if (curr == '\\') {
                    char next = literalValueCharIterator.ReadChar();

                    switch (next) {
                        case ':':
                        case '{':
                        case '}':
                        case '\\':
                            buffer.Append(next);
                            break;

                        case 'n':
                            buffer.Append('\n');
                            break;

                        case 'r':
                            buffer.Append('\r');
                            break;

                        case 't':
                            buffer.Append('\t');
                            break;

                        default:
                            module.AddError(span, $"Unrecognized escape sequence: \\{next}");
                            break;
                    }
                }
                else if (curr == '{') {
                    int rightBracePosition = literalValueCharIterator.GetMatchingRightBrace();
                    if (rightBracePosition == -1)
                        buffer.Append(curr);
                    else {
                        var fragmentSpan = TextSpan.FromBounds(fragmentStartPosition, literalValueCharIterator.Position - 1);
                        stringFragments.Add(new InterpolatedStringFragmentSyntax(fragmentSpan, buffer.ToString()));
                        buffer.Clear();

                        var expressionSourceSpan = new TextSpan(literalValueCharIterator.Position, rightBracePosition);

                        // TODO: Fix this up to be more general. But for now we only support variable references for interpolation
                        var parser = new SourceParser(module, expressionSourceSpan);
                        NameSyntax nameSyntax = parser.ParseName();
                        ExpressionSyntax expression = new SymbolReferenceSyntax(expressionSourceSpan, nameSyntax);

                        expressions.Add(expression);

                        literalValueCharIterator.Position = rightBracePosition + 1;
                    }
                    break;
                }
                else if (curr == '\0')
                    break;
                else buffer.Append(curr);
            }

            if (expressions.Count == 0)
                return new StringLiteralSyntax(span, buffer.ToString());
            else {
                var fragmentSpan = TextSpan.FromBounds(fragmentStartPosition, span.End);
                stringFragments.Add(new InterpolatedStringFragmentSyntax(fragmentSpan, buffer.ToString()));

                return new InterpolatedStringExpressionSyntax(span, stringFragments.ToArray(), expressions.ToArray());
            }
        }

        private static string ParseUnknownTextBlock(FamlModule module, TextSpan sourceSpan) {
            var literalValueCharIterator = new LiteralValueCharIterator(module.SourceText, sourceSpan);

            var buffer = new StringBuilder();
            while (true) {
                char curr = literalValueCharIterator.ReadChar();

                if (curr == '\0')
                    break;
                else buffer.Append(curr);
            }

            return buffer.ToString();
        }

        private SourceParser(FamlModule module) {
            _module = module;
            _token = new Token(module.SourceText);
            _source = _token.Source;
        }

        private SourceParser(FamlModule module, TextSpan span) {
            _module = module;
            _token = new Token(new ParseableSource(module.SourceText, span));
            _source = _token.Source;
        }

        private SourceParser(FamlModule module, Token token) {
            _module = module;
            _token = token;
            _source = _token.Source;
        }

        private void ParseModule() {
            var synchronizingTokens = new Tokens(TokenType.Eof);

            FunctionInvocationSyntax? projectDefinition = null;
            var imports = new List<ImportSyntax>();
            var useLibraries = new List<UseSyntax>();

            int startPosition = _token.TokenStartPosition;

            if (_token.Type == TokenType.Identifier && _token.StringValue == "App")
                projectDefinition = ParseFunctionInvocation(null);

            while (_token.Type == TokenType.Use) {
                UseSyntax use = ParseUse();
                useLibraries.Add(use);
            }

            while (_token.Type == TokenType.Import) {
                ImportSyntax import = ParseImport();
                imports.Add(import);
            }

            SyntaxNode[] moduleItems = ParseModuleItems(synchronizingTokens);
            var moduleSyntax = new ModuleSyntax(_module, TextSpanFrom(startPosition), projectDefinition, useLibraries.ToArray(), imports.ToArray(), moduleItems);

            _module.ModuleSyntax = moduleSyntax;
        }

        private SyntaxNode[] ParseModuleItems(Tokens synchronizingTokens) {
            var moduleItems = new List<SyntaxNode>();

            Tokens definitionStartTokens = new Tokens(TokenType.Type).Add(TokenType.Identifier, 0);
            Tokens parseDefinitionSynchronizingTokens = synchronizingTokens.Add(definitionStartTokens);

            // TODO: Check for duplicate names
            while (true) {
                if (_token.Type == TokenType.Identifier) {
                    if (_token.StringValue == "example" || _token.StringValue == "examples") {
                        ExampleDefinitionSyntax exampleDefinition = ParseExampleDefinition(
                            parseDefinitionSynchronizingTokens);
                        moduleItems.Add(exampleDefinition);
                    }
                    else {
                        FunctionDefinitionSyntax functionDefinition =
                            ParseFunctionDefinition(parseDefinitionSynchronizingTokens);
                        moduleItems.Add(functionDefinition);
                    }
                }
                else if (_token.Type == TokenType.PropertyIdentifier) {
                    FunctionDefinitionSyntax functionDefinition =
                        ParseFunctionDefinition(parseDefinitionSynchronizingTokens);
                    moduleItems.Add(functionDefinition);
                }
                else if (_token.Type == TokenType.Type) {
                    RecordTypeDefinitionSyntax recordTypeDefinition =
                        ParseRecordTypeDefinition(parseDefinitionSynchronizingTokens);
                    moduleItems.Add(recordTypeDefinition);
                }
                else if (_token.Type == TokenType.Eof) {
                    break;
                }
                else {
                    UnexpectedToken("function definition, data, or example");

                    // If we found something unexpected, try to sync. If after the sync we're still in error mode, give up
                    Sync(definitionStartTokens, synchronizingTokens);
                    if (_token.InErrorMode)
                        break;
                }
            }

            return moduleItems.ToArray();
        }

        private ExampleDefinitionSyntax ParseExampleDefinition(Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            // Advance past example/examples keywords
            Advance();

            ExpressionSyntax expression = ParseSingleExpression(synchronizingTokens);
            return new ExampleDefinitionSyntax(TextSpanFrom(startPosition), expression);
        }

        private FunctionDefinitionSyntax ParseFunctionDefinition(Tokens synchronizingTokens) {
            if (_token.Type == TokenType.PropertyIdentifier)
                _token.RescanPropertyIdentifierAsIdentifier();

            int startPosition = _token.TokenStartPosition;

            Check(TokenType.Identifier, "function name");
            NameSyntax functionNameSyntax = ParseName();

            List<PropertyNameTypePairSyntax> parameters;
            if (_token.Type == TokenType.LeftBrace) {
                Advance();
                parameters = ParsePropertyNameTypePairs(synchronizingTokens.Add(TokenType.RightBrace));
                CheckOrSyncAndAdvance(TokenType.RightBrace, synchronizingTokens.Add(TokenType.Assign));
            }
            else parameters = new List<PropertyNameTypePairSyntax>();

            TypeReferenceSyntax? returnType = null;
            if (_token.Type == TokenType.Colon) {
                Advance();
                returnType = ParseTypeReference();
            }

            CheckOrSyncAndAdvance(TokenType.Assign, synchronizingTokens);

            ExpressionSyntax expression = ParseExpressionAllowTextualLiteral(synchronizingTokens, TextualLiteralContext.FunctionDefinition);

            return new FunctionDefinitionSyntax(TextSpanFrom(startPosition), functionNameSyntax, parameters.ToArray(),
                returnType, expression, new DefinitionSyntax[0]);
        }

        private List<PropertyNameTypePairSyntax> ParsePropertyNameTypePairs(Tokens synchronizingTokens) {
            var parameters = new List<PropertyNameTypePairSyntax>();

            while (_token.Type == TokenType.PropertyIdentifier) {
                int parameterStartPosition = _token.TokenStartPosition;

                string tokenString = _token.StringValue;
                if (tokenString.IndexOf('.') != -1)
                    AddError(_token.TokenSpan, $"Function parameter names can't contain a period: {tokenString}");

                var propertyNameIdentifier = new NameSyntax(_token.TokenSourceSpanExceptForLastCharacter, new Name(tokenString));

                Advance();

                TypeReferenceSyntax typeReferenceSyntax = ParseTypeReference();

                ExpressionSyntax? defaultValue = null;
                if (_token.Type == TokenType.Assign) {
                    CheckAndAdvance(TokenType.Assign);
                    defaultValue = ParseExpression(synchronizingTokens);
                }

                parameters.Add(
                    new PropertyNameTypePairSyntax(TextSpanFrom(parameterStartPosition), propertyNameIdentifier, typeReferenceSyntax, defaultValue));

                if (_token.Type != TokenType.RightBrace && !_token.IsAtStartOfLine)
                    CheckAndAdvance(TokenType.Semicolon);
            }

            return parameters;
        }

        private NameSyntax ParseName() {
            Check(TokenType.Identifier);

            // TODO: Handle errors here, returning an error node

            var nameIdentifier = new NameSyntax(_token.TokenSpan, new Name(_token.StringValue));
            Advance();
            return nameIdentifier;
        }

        private SymbolReferenceSyntax ParseSymbolReference() {
            var nameIdentifier = ParseName();
            return new SymbolReferenceSyntax(_token.TokenSpan, nameIdentifier);
        }

        private ExpressionSyntax ParseQualifiableSymbolReference() {
            int startPosition = _token.TokenStartPosition;

            NameSyntax initialSymbol = ParseName();

            // If not qualified, return a SymbolReference
            if (_token.Type != TokenType.Period)
                return new SymbolReferenceSyntax(initialSymbol.Span, initialSymbol);

            QualifiedSymbolReferenceSyntax qualifiedSymbolReference = new QualifiedSymbolReferenceSyntax(
                TextSpanFrom(startPosition), null, initialSymbol);

            do {
                Advance();     // Advance past the period

                NameSyntax symbol = ParseName();
                qualifiedSymbolReference = new QualifiedSymbolReferenceSyntax(
                    TextSpanFrom(startPosition), qualifiedSymbolReference, symbol);
            }
            while (_token.Type == TokenType.Period);

            return qualifiedSymbolReference;
        }

        private void CheckAndAdvanceIdentifierKeyword(string keywordText) {
            if (_token.Type != TokenType.Identifier || _token.StringValue != keywordText)
                UnexpectedToken($"'{keywordText}'");
            else Advance();
        }

        private ImportSyntax ParseImport() {
            int startPosition = _token.TokenStartPosition;

            CheckAndAdvance(TokenType.Import);

            if (_token.Type == TokenType.LeftBrace) {
                Advance();

                var importReferences = new List<ImportTypeReferenceSyntax>();
                while (_token.Type == TokenType.Identifier) {
                    NameSyntax nameSyntax = ParseName();
                    importReferences.Add(new ImportTypeReferenceSyntax(nameSyntax.Span, nameSyntax));
                }

                CheckAndAdvance(TokenType.RightBrace);

                CheckAndAdvanceIdentifierKeyword("from");

                QualifiableNameSyntax qualifier = ParseQualifiableName();
                return new ImportSyntax(TextSpanFrom(startPosition), importReferences.ToImmutableArray(), qualifier);
            }
            else {
                // This is the import all case
                QualifiableNameSyntax qualifier = ParseQualifiableName();
                return new ImportSyntax(TextSpanFrom(startPosition), qualifier);
            }
        }

        private UseSyntax ParseUse() {
            int startPosition = _token.TokenStartPosition;

            CheckAndAdvance(TokenType.Use);

            FunctionInvocationSyntax libraryInfo = ParseFunctionInvocation(null);

            return new UseSyntax(TextSpanFrom(startPosition), libraryInfo);
        }

        private FunctionInvocationSyntax ParseFunctionInvocation(ExpressionSyntax functionReference) {
            int startPosition = functionReference.Span.Start;

            CheckAndAdvance(TokenType.LeftBrace);

            // Parse named arguments first
            var namedArguments = new List<ArgumentNameValuePairSyntax>();
            while (_token.Type == TokenType.PropertyIdentifier) {
                int argumentStartPosition = _token.TokenStartPosition;

                var propertyName = new QualifiableNameSyntax(_token.TokenSourceSpanExceptForLastCharacter,
                    new QualifiableName(_token.StringValue));
                var propertySpecifier = new PropertySpecifierSyntax(_token.TokenSpan, propertyName);
                Advance();

                ExpressionSyntax value = ParseExpressionAllowTextualLiteral(new Tokens(TokenType.Eof),
                    TextualLiteralContext.PropertyValue);

                namedArguments.Add(new ArgumentNameValuePairSyntax(TextSpanFrom(argumentStartPosition), propertySpecifier, value));

                if (_token.Type != TokenType.RightBrace && !_token.IsAtStartOfLine)
                    CheckAndAdvance(TokenType.Semicolon);
            }

            // Parse the content property, if there is content
            ContentArgumentSyntax? contentArgument = null;
            if (_token.Type != TokenType.RightBrace) {
                ExpressionSyntax contentValue = ParseExpressionAllowTextualLiteral(new Tokens(TokenType.Eof),
                    TextualLiteralContext.ContentPropertyValue);
                contentArgument = new ContentArgumentSyntax(contentValue.Span, contentValue);
            }

            CheckAndAdvance(TokenType.RightBrace);

            return new FunctionInvocationSyntax(TextSpanFrom(startPosition), InvocationStyle.Delimiter, functionReference, namedArguments.ToArray(), contentArgument);
        }

        private QualifiableNameSyntax ParseQualifiableName() {
            int startPosition = _token.TokenStartPosition;

            StringBuilder buffer = new StringBuilder();
            while (true) {
                Check(TokenType.Identifier);
                buffer.Append(_token.StringValue);
                Advance();

                if (_token.Type != TokenType.Period)
                    break;
                buffer.Append('.');
                _token.Advance();
            }

            return new QualifiableNameSyntax(TextSpanFrom(startPosition), new QualifiableName(buffer.ToString()));
        }

        private RecordTypeDefinitionSyntax ParseRecordTypeDefinition(Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            CheckAndAdvance(TokenType.Type, "type definition");

            NameSyntax typeNameSyntax = ParseName();

            CheckAndAdvance(TokenType.Assign);

            CheckAndAdvance(TokenType.LeftBrace);
            List<PropertyNameTypePairSyntax> propertyNameTypePairs = ParsePropertyNameTypePairs(synchronizingTokens.Add(TokenType.RightBrace));
            CheckOrSyncAndAdvance(TokenType.RightBrace, synchronizingTokens);

            return new RecordTypeDefinitionSyntax(TextSpanFrom(startPosition), typeNameSyntax, propertyNameTypePairs.ToArray());
        }

        /*
        public ExampleDefinition parseExampleDefinition() {
            int startPosition = _token.tokenStartPosition;

            checkAndAdvance(TokenType.EXAMPLE);

            Expression expression = parseExpression();
            return new ExampleDefinition(sourceSpanFrom(startPosition), expression);
        }
        */

        /*
        public FunctionInvocation parseFunctionInvocation() {
            check(TokenType.FUNCTION_NAME, "function name");
            SimpleName functionName = new SimpleName(token);
            token.advance();

            return parseFunctionInvocation(functionName);
        }
        */

        private TypeReferenceSyntax ParseTypeReference() {
            if (_token.Type == TokenType.Identifier) {
                int startPosition = _token.TokenStartPosition;

                QualifiableNameSyntax typeNameSyntax = ParseQualifiableName();
                TextSpan sourceSpan = typeNameSyntax.Span;
                string typeNameString = typeNameSyntax.Name.ToString();

                TypeReferenceSyntax typeReferenceSyntax;

                BuiltInTypeBinding? predefinedTypeBinding = BuiltInTypeBinding.GetBindingForTypeName(typeNameString);
                if (predefinedTypeBinding != null)
                    typeReferenceSyntax = new PredefinedTypeReferenceSyntax (sourceSpan, predefinedTypeBinding);
                else
                    typeReferenceSyntax = new ObjectTypeReferenceSyntax (sourceSpan, typeNameSyntax);

                if (_token.Type == TokenType.Ellipsis) {
                    Advance();
                    return new SequenceTypeReferenceSyntax(TextSpanFrom(startPosition), typeReferenceSyntax);
                }
                else return typeReferenceSyntax;
            }
            else {
                UnexpectedToken("type name");
                return new InvalidTypeReferenceSyntax();
            }
        }

        private ExpressionSyntax ParseBooleanLiteral() {
            int startPosition = _token.TokenStartPosition;

            if (_token.Type == TokenType.True) {
                Advance();
                return new BooleanLiteralSyntax(TextSpanFrom(startPosition), true);
            }
            else if (_token.Type == TokenType.False) {
                Advance();
                return new BooleanLiteralSyntax(TextSpanFrom(startPosition), false);
            }
            else {
                UnexpectedToken("boolean literal (true or false)");
                return new InvalidExpressionSyntax();
            }
        }

        private ExpressionSyntax ParseIntLiteral() {
            int startPosition = _token.TokenStartPosition;

            if (_token.Type == TokenType.Int32) {
                int tokenIntValue = _token.IntValue;
                Advance();
                return new IntLiteralSyntax(TextSpanFrom(startPosition), tokenIntValue);
            }
            else {
                Advance();
                UnexpectedToken("int literal");
                return new InvalidExpressionSyntax();
            }
        }

        private void Check(TokenType expectedType) {
            Check(expectedType, expectedType.ToString());
        }

        private void Check(TokenType expectedType, string expected) {
            if (_token.Type != expectedType)
                UnexpectedToken(expected);
        }

        private void CheckAndAdvance(TokenType expectedType) {
            Check(expectedType);
            if (!_token.InErrorMode)
                _token.Advance();
        }

        private void CheckOrSync(TokenType expectedType, Tokens synchronizingTokens) {
            Check(expectedType);
            if (_token.InErrorMode)
                Sync(new Tokens(expectedType), synchronizingTokens);
        }

        private void CheckOrSync(TokenType expectedType, string expected, Tokens synchronizingTokens) {
            Check(expectedType, expected);
            if (_token.InErrorMode)
                Sync(new Tokens(expectedType), synchronizingTokens);
        }

        private void CheckOrSyncAndAdvance(TokenType expectedType, Tokens synchronizingTokens) {
            Check(expectedType);
            if (_token.InErrorMode)
                Sync(new Tokens(expectedType), synchronizingTokens);
            if (!_token.InErrorMode)
                _token.Advance();
        }

        private void AddError(TextSpan problemSourceSpan, string message) {
            var diagnostic = new Diagnostic(_module, problemSourceSpan, DiagnosticSeverity.Error, message);
            _module.AddDiagnostic(diagnostic);
        }

        private void UnexpectedToken(string expected) {
           if (!_token.InErrorMode) {
                string message = $"Encountered {_token.ToString()} when expected {expected}";
                AddError(_token.TokenSpan, message);
                _token.InErrorMode = true;
            }
        }

        private void Sync(Tokens? allowedTokens, Tokens otherTokens) {
            if (!_token.InErrorMode)
                return;

            while (true) {
                TokenType currentTokenType = _token.TypeForErrorMode;
                int column = _token.TokenStartColumn;

                if (allowedTokens != null && allowedTokens.Contains(currentTokenType, column)) {
                    _token.InErrorMode = false;
                    break;
                }

                if (otherTokens.Contains(currentTokenType, column))
                    break;
                
                _token.Advance();
            }
        }

        private void CheckAndAdvance(TokenType expectedType, string expectedMessage) {
            Check(expectedType, expectedMessage);
            _token.Advance();
        }

        private void Advance() {
            _token.Advance();
        }

        public enum TextualLiteralContext {
            FunctionDefinition,
            PropertyValue,
            ContentPropertyValue
        }

        private ExpressionSyntax ParseExpressionAllowTextualLiteral(Tokens synchronizingTokens, TextualLiteralContext markupContext) {
            if (_token.LooksLikeStartOfExpression()) {
                if (markupContext == TextualLiteralContext.FunctionDefinition)
                    return ParseSingleExpression(synchronizingTokens);
                else return ParseExpression(synchronizingTokens);
            }
            else {
                int startPosition = -1;
                var items = new List<TextualLiteralItemSyntax>();

                bool bracketed = false;
                if (_token.Type == TokenType.LeftBracket) {
                    startPosition = _token.TokenStartPosition;

                    CheckAndAdvance(TokenType.LeftBracket);
                    bracketed = true;
                }

                bool allowMultiline = bracketed || markupContext == TextualLiteralContext.ContentPropertyValue;

                bool allowSemicolonToTerminate = markupContext == TextualLiteralContext.PropertyValue;
                bool allowNewlineToTerminate = !allowMultiline;
                bool allowRightBraceToTerminate = markupContext == TextualLiteralContext.PropertyValue || markupContext == TextualLiteralContext.ContentPropertyValue;
                bool allowRightBracketToTerminate = bracketed;

                while (true) {
                    _token.ReinterpretAlloWTextualLiteral(
                        allowSemicolonToTerminate: allowSemicolonToTerminate,
                        allowNewlineToTerminate: allowNewlineToTerminate,
                        allowRightBraceToTerminate: allowRightBraceToTerminate,
                        allowRightBracketToTerminate: allowRightBracketToTerminate);

                    if (startPosition == -1)
                        startPosition = _token.TokenStartPosition;

                    if (_token.Type == TokenType.TextualLiteralText) {
                        items.Add(new TextualLiteralTextItemSyntax(_token.TokenSpan, _token.StringValue));

                        bool atEnd = allowNewlineToTerminate && _token.LookaheadIsNewline();

                        Advance();
                        if (atEnd)
                            break;
                    }
                    else if (_token.Type == TokenType.LeftBrace) {
                        ExpressionSyntax expression = ParseExpression(synchronizingTokens);
                        items.Add(new TextualLiteralExpressionItemSyntax(expression));
                    }
                    else if (_token.Type == TokenType.Identifier) {
                        ExpressionSyntax qualifiableSymbolReference = ParseQualifiableSymbolReference();
                        FunctionInvocationSyntax functionInvocation = ParseFunctionInvocation(qualifiableSymbolReference);

                        items.Add(new TextualLiteralExpressionItemSyntax(functionInvocation));
                    }
                    else break;
                }

                if (bracketed)
                    CheckAndAdvance(TokenType.RightBracket);

                return new TextualLiteralSyntax(TextSpanFrom(startPosition), items.ToImmutableArray());
            }
        }

        private ExpressionSyntax ParseExpression(Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            var expressions = new List<ExpressionSyntax>();

            while (true) {
                TokenType lookahead = _token.Type;
                if (lookahead == TokenType.Eof || lookahead == TokenType.ErrorMode || lookahead == TokenType.Semicolon ||
                        lookahead == TokenType.RightBrace || lookahead == TokenType.PropertyIdentifier )
                    break;

                ExpressionSyntax expression = ParseSingleExpression(synchronizingTokens);
                expressions.Add(expression);
            }

            ExpressionSyntax overallExpression;
            if (expressions.Count == 1)
                overallExpression = expressions[0];
            else overallExpression = new SequenceLiteralExpressionSyntax(TextSpanFrom(startPosition), expressions.ToArray());

            return overallExpression;
        }

        private ExpressionSyntax ParseSingleExpression(Tokens synchronizingTokens) {
            return ParseExpressionComponent(0, synchronizingTokens);
        }

        /// <summary>
        /// Parse an expression, using the "Precedence climbing" algorithm, described at
        /// http://www.engr.mun.ca/~theo/Misc/exp_parsing.htm.
        /// </summary>
        /// <param name="precedence">precedence level</param>
        /// <param name="synchronizingTokens">error recovery synchronizing tokens</param>
        /// <remarks> AST subtree for the expression</remarks>
        private ExpressionSyntax ParseExpressionComponent(int precedence, Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            // Parse the prefix which can be a terminal, a parenthesized subexpression, or unary operator followed by an
            // operand. If there's a binary operator after the prefix, then the prefix acts as a left operand for that
            // operator
            ExpressionSyntax leftOperand = ParseExpressionPrefix(synchronizingTokens);

            // Now parse zero or more sets of binary operators followed by right operands, where the binary operator
            // precedence is <= precedence
            while (true) {
                InfixOperator infixOperator = Operator.GetInfixOperator(_token.Type);
                if (infixOperator == null || infixOperator.GetPrecedence() < precedence)
                    break;
                Advance();

                if (infixOperator == Operator.Dot) {
                    if (_token.LookaheadIsLeftBrace())
                        leftOperand = ParseFunctionInvocation(leftOperand);
                    else {
                        NameSyntax nameSyntax = ParseName();
                        leftOperand = new PropertyAccessSyntax(TextSpanFrom(startPosition), leftOperand, nameSyntax);
                    }
                }
                else if (infixOperator == Operator.For)
                    return ParseForExpression(leftOperand, synchronizingTokens);
                else {
                    ExpressionSyntax rightOperand =
                        ParseExpressionComponent(infixOperator.GetPrecedence() + 1, synchronizingTokens);

                    leftOperand = new InfixExpressionSyntax(TextSpanFrom(startPosition), leftOperand, infixOperator,
                        rightOperand);
                }
            }

            return leftOperand;
        }

        /// <summary>
        /// Parse an expression prefix, using the "Precedence climbing" algorithm, described at
        /// http://www.engr.mun.ca/~theo/Misc/exp_parsing.htm.  The prefix corresponds to "P" in the algorithm description.
        /// </summary>
        /// <remarks> parsed expression</remarks>
        private ExpressionSyntax ParseExpressionPrefix(Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            PrefixOperator prefixOperator = Operator.GetPrefixOperator(_token.Type);

            if (prefixOperator != null) {
                _token.Advance();
                ExpressionSyntax operand = ParseExpressionComponent(prefixOperator.GetPrecedence(), synchronizingTokens);
                return new PrefixExpressionSyntax(TextSpanFrom(startPosition), prefixOperator, operand);
            }
            else {
                switch (_token.Type) {
                    case TokenType.LeftParen:
                        _token.Advance();
                        ExpressionSyntax expression = ParseExpressionComponent(0, synchronizingTokens);
                        CheckAndAdvance(TokenType.RightParen);
                        return new ParenthesizedExpressionSyntax(TextSpanFrom(startPosition), expression);

                    case TokenType.True:
                    case TokenType.False:
                        return ParseBooleanLiteral();

                    case TokenType.Int32:
                        return ParseIntLiteral();

                    case TokenType.If:
                        return ParseIfExpression(synchronizingTokens);

                    case TokenType.Null:
                        Advance();
                        return new NullLiteralSyntax(TextSpanFrom(startPosition));

                    case TokenType.Identifier:
                        ExpressionSyntax qualifiableSymbolReference = ParseQualifiableSymbolReference();

                        if (_token.Type == TokenType.LeftBrace)
                            return ParseFunctionInvocation(qualifiableSymbolReference);
                        else return qualifiableSymbolReference;

                    // TODO: FIX THIS, SEEING WHERE IT'S USED;  StringValue isn't set for ContextSensitiveText, for one thing
                    case TokenType.TextBlock:
                        string tokenStringValue = _token.StringValue;
                        _token.Advance();
                        return new StringLiteralSyntax(TextSpanFrom(startPosition), tokenStringValue);

                    case TokenType.LeftBrace:
                        Advance();

                        if (_token.Type == TokenType.PropertyIdentifier) {
                            UnexpectedToken("function invocation without specifying the function name which isn't currently supported");
                            return new InvalidExpressionSyntax();

                            /*
                            var arguments = new List<ArgumentNameValuePair>();

                            parseArgumentNameValuePairs(arguments);
                            checkAndAdvance(TokenType.RIGHT_BRACE);

                            return new FunctionInvocation(sourceSpanFrom(startPosition), null, null, arguments.ToArray());
                            */
                        }
                        else {
                            ExpressionSyntax innerExpression = ParseExpression(synchronizingTokens);
                            CheckAndAdvance(TokenType.RightBrace);
                            return new BracedExpressionSyntax(TextSpanFrom(startPosition), innerExpression);
                        }

                    /*
                    case FUNCTION_NAME:
                        return parseFunctionInvocation();
                    */

                    default:
                        UnexpectedToken("start of expression");
                        return new InvalidExpressionSyntax();
                }
            }
        }

        private ExpressionSyntax ParseIfExpression(Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            bool haveIfToken = _token.Type == TokenType.If;

            if (haveIfToken) {
                Advance();

                // TODO: Handle if ... is ... syntax
            }

            var conditionValuePairs = new List<ConditionValuePairSyntax>();
            TextSpan? elseTextBlock = null;

            while (true) {
                int ifItemStartPosition = _token.TokenStartPosition;

                CheckOrSyncAndAdvance(TokenType.Pipe, synchronizingTokens.Add(TokenType.Colon));

                // "|:" indicates the else part of the if
                if (_token.Type == TokenType.Colon) {
                    Advance();
                    _token.RescanAsTextBlock(ifItemStartPosition, allowIfConditionPipeToTerminate: true);
                    elseTextBlock = _token.TokenSpan;
                    Advance();

                    break;  // "else" must be the last item in the if
                }

                ExpressionSyntax conditionExpression = ParseExpression(synchronizingTokens.Add(TokenType.Pipe).Add(TokenType.Colon));
                CheckOrSyncAndAdvance(TokenType.Colon, synchronizingTokens);

                // TODO: Allow braces here
                //ExpressionSyntax valueExpression = ParseExpressionAllowSequence(synchronizingTokens.Add(TokenType.Pipe));

                _token.RescanAsTextBlock(ifItemStartPosition, allowIfConditionPipeToTerminate: true);
                TextSpan valueTextBlock = _token.TokenSpan;
                Advance();

                //ExpressionSyntax valueExpression = ParseExpressionAllowSequence(synchronizingTokens.Add(TokenType.Pipe));

                var conditionValuePair = new ConditionValuePairSyntax(TextSpanFrom(ifItemStartPosition), conditionExpression, valueTextBlock);
                conditionValuePairs.Add(conditionValuePair);

                if (_token.Type != TokenType.Pipe)
                    break;
            }

            return new IfExpressionSyntax(TextSpanFrom(startPosition), conditionValuePairs.ToArray(), elseTextBlock);
        }

        private ExpressionSyntax ParseIfIsExpression(Tokens synchronizingTokens) {
            AddError(_token.TokenSpan, $"if ... is ... syntax is not yet supported");
            return new InvalidExpressionSyntax();
        }

        private ExpressionSyntax ParseForExpression(ExpressionSyntax expression, Tokens synchronizingTokens) {
            int startPosition = _token.TokenStartPosition;

            NameSyntax nameSyntax = ParseName();
            CheckAndAdvance(TokenType.In);
            ExpressionSyntax inExpression = ParseSingleExpression(synchronizingTokens);

            var forVariableDefinition = new ForVariableDefinitionSyntax(TextSpanFrom(startPosition), nameSyntax, inExpression);

            return new ForExpressionSyntax(TextSpanFrom(expression.Span.Start), expression, forVariableDefinition);
        }

        private TextSpan TextSpanFrom(int startPosition) {
            int endPosition = _token.PrevTokenEndPosition;

            // In error scenarios, when the token is in eror mode, it may not have advanced at all when parsing.
            // In that case, the source span can be of 0 length but shouldn't be negative
            if (endPosition < startPosition)
                endPosition = startPosition;

            return TextSpan.FromBounds(startPosition, endPosition);
        }

        /*
    Eparser is
       var t : Tree
       t := Exp( 0 )
       checkAndAdvance( end )
       return t
    Exp( p ) is
        var t : Tree
        t := P
        while next is a binary operator and prec(binary(next)) >= p
           const op := binary(next)
           consume
           const q := case associativity(op)
                      of Right: prec( op )
                         Left:  1+prec( op )
           const t1 := Exp( q )
           t := mkNode( op, t, t1)
        return t
    P is
        if next is a unary operator
             const op := unary(next)
             consume
             q := prec( op )
             const t := Exp( q )
             return mkNode( op, t )
        else if next = "("
             consume
             const t := Exp( 0 )
             checkAndAdvance ")"
             return t
        else if next is a v
             const t := mkLeaf( next )
             consume
             return t
        else
             error
         */
    }
}
