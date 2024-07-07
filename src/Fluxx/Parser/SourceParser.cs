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
            this._module = module;
            this._token = new Token(module.SourceText);
            this._source = this._token.Source;
        }

        private SourceParser(FamlModule module, TextSpan span) {
            this._module = module;
            this._token = new Token(new ParseableSource(module.SourceText, span));
            this._source = this._token.Source;
        }

        private SourceParser(FamlModule module, Token token) {
            this._module = module;
            this._token = token;
            this._source = this._token.Source;
        }

        private void ParseModule() {
            var synchronizingTokens = new Tokens(TokenType.Eof);

            FunctionInvocationSyntax? projectDefinition = null;
            var imports = new List<ImportSyntax>();
            var useLibraries = new List<UseSyntax>();

            int startPosition = this._token.TokenStartPosition;

            if (this._token.Type == TokenType.Identifier && this._token.StringValue == "App")
                projectDefinition = this.ParseFunctionInvocation(null);

            while (this._token.Type == TokenType.Use) {
                UseSyntax use = this.ParseUse();
                useLibraries.Add(use);
            }

            while (this._token.Type == TokenType.Import) {
                ImportSyntax import = this.ParseImport();
                imports.Add(import);
            }

            SyntaxNode[] moduleItems = this.ParseModuleItems(synchronizingTokens);
            var moduleSyntax = new ModuleSyntax(this._module, this.TextSpanFrom(startPosition), projectDefinition, useLibraries.ToArray(), imports.ToArray(), moduleItems);

            this._module.ModuleSyntax = moduleSyntax;
        }

        private SyntaxNode[] ParseModuleItems(Tokens synchronizingTokens) {
            var moduleItems = new List<SyntaxNode>();

            Tokens definitionStartTokens = new Tokens(TokenType.Type).Add(TokenType.Identifier, 0);
            Tokens parseDefinitionSynchronizingTokens = synchronizingTokens.Add(definitionStartTokens);

            // TODO: Check for duplicate names
            while (true) {
                if (this._token.Type == TokenType.Identifier) {
                    if (this._token.StringValue == "example" || this._token.StringValue == "examples") {
                        ExampleDefinitionSyntax exampleDefinition = this.ParseExampleDefinition(
                            parseDefinitionSynchronizingTokens);
                        moduleItems.Add(exampleDefinition);
                    }
                    else {
                        FunctionDefinitionSyntax functionDefinition =
                            this.ParseFunctionDefinition(parseDefinitionSynchronizingTokens);
                        moduleItems.Add(functionDefinition);
                    }
                }
                else if (this._token.Type == TokenType.PropertyIdentifier) {
                    FunctionDefinitionSyntax functionDefinition =
                        this.ParseFunctionDefinition(parseDefinitionSynchronizingTokens);
                    moduleItems.Add(functionDefinition);
                }
                else if (this._token.Type == TokenType.Type) {
                    RecordTypeDefinitionSyntax recordTypeDefinition =
                        this.ParseRecordTypeDefinition(parseDefinitionSynchronizingTokens);
                    moduleItems.Add(recordTypeDefinition);
                }
                else if (this._token.Type == TokenType.Eof) {
                    break;
                }
                else {
                    this.UnexpectedToken("function definition, data, or example");

                    // If we found something unexpected, try to sync. If after the sync we're still in error mode, give up
                    this.Sync(definitionStartTokens, synchronizingTokens);
                    if (this._token.InErrorMode)
                        break;
                }
            }

            return moduleItems.ToArray();
        }

        private ExampleDefinitionSyntax ParseExampleDefinition(Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            // Advance past example/examples keywords
            this.Advance();

            ExpressionSyntax expression = this.ParseSingleExpression(synchronizingTokens);
            return new ExampleDefinitionSyntax(this.TextSpanFrom(startPosition), expression);
        }

        private FunctionDefinitionSyntax ParseFunctionDefinition(Tokens synchronizingTokens) {
            if (this._token.Type == TokenType.PropertyIdentifier)
                this._token.RescanPropertyIdentifierAsIdentifier();

            int startPosition = this._token.TokenStartPosition;

            this.Check(TokenType.Identifier, "function name");
            NameSyntax functionNameSyntax = this.ParseName();

            List<PropertyNameTypePairSyntax> parameters;
            if (this._token.Type == TokenType.LeftBrace) {
                this.Advance();
                parameters = this.ParsePropertyNameTypePairs(synchronizingTokens.Add(TokenType.RightBrace));
                this.CheckOrSyncAndAdvance(TokenType.RightBrace, synchronizingTokens.Add(TokenType.Assign));
            }
            else parameters = new List<PropertyNameTypePairSyntax>();

            TypeReferenceSyntax? returnType = null;
            if (this._token.Type == TokenType.Colon) {
                this.Advance();
                returnType = this.ParseTypeReference();
            }

            this.CheckOrSyncAndAdvance(TokenType.Assign, synchronizingTokens);

            ExpressionSyntax expression = this.ParseExpressionAllowTextualLiteral(synchronizingTokens, TextualLiteralContext.FunctionDefinition);

            return new FunctionDefinitionSyntax(this.TextSpanFrom(startPosition), functionNameSyntax, parameters.ToArray(),
                returnType, expression, new DefinitionSyntax[0]);
        }

        private List<PropertyNameTypePairSyntax> ParsePropertyNameTypePairs(Tokens synchronizingTokens) {
            var parameters = new List<PropertyNameTypePairSyntax>();

            while (this._token.Type == TokenType.PropertyIdentifier) {
                int parameterStartPosition = this._token.TokenStartPosition;

                string tokenString = this._token.StringValue;
                if (tokenString.IndexOf('.') != -1)
                    this.AddError(this._token.TokenSpan, $"Function parameter names can't contain a period: {tokenString}");

                var propertyNameIdentifier = new NameSyntax(this._token.TokenSourceSpanExceptForLastCharacter, new Name(tokenString));

                this.Advance();

                TypeReferenceSyntax typeReferenceSyntax = this.ParseTypeReference();

                ExpressionSyntax? defaultValue = null;
                if (this._token.Type == TokenType.Assign) {
                    this.CheckAndAdvance(TokenType.Assign);
                    defaultValue = this.ParseExpression(synchronizingTokens);
                }

                parameters.Add(
                    new PropertyNameTypePairSyntax(this.TextSpanFrom(parameterStartPosition), propertyNameIdentifier, typeReferenceSyntax, defaultValue));

                if (this._token.Type != TokenType.RightBrace && !this._token.IsAtStartOfLine)
                    this.CheckAndAdvance(TokenType.Semicolon);
            }

            return parameters;
        }

        private NameSyntax ParseName() {
            this.Check(TokenType.Identifier);

            // TODO: Handle errors here, returning an error node

            var nameIdentifier = new NameSyntax(this._token.TokenSpan, new Name(this._token.StringValue));
            this.Advance();
            return nameIdentifier;
        }

        private SymbolReferenceSyntax ParseSymbolReference() {
            var nameIdentifier = this.ParseName();
            return new SymbolReferenceSyntax(this._token.TokenSpan, nameIdentifier);
        }

        private ExpressionSyntax ParseQualifiableSymbolReference() {
            int startPosition = this._token.TokenStartPosition;

            NameSyntax initialSymbol = this.ParseName();

            // If not qualified, return a SymbolReference
            if (this._token.Type != TokenType.Period)
                return new SymbolReferenceSyntax(initialSymbol.Span, initialSymbol);

            QualifiedSymbolReferenceSyntax qualifiedSymbolReference = new QualifiedSymbolReferenceSyntax(
                this.TextSpanFrom(startPosition), null, initialSymbol);

            do {
                this.Advance();     // Advance past the period

                NameSyntax symbol = this.ParseName();
                qualifiedSymbolReference = new QualifiedSymbolReferenceSyntax(
                    this.TextSpanFrom(startPosition), qualifiedSymbolReference, symbol);
            }
            while (this._token.Type == TokenType.Period);

            return qualifiedSymbolReference;
        }

        private void CheckAndAdvanceIdentifierKeyword(string keywordText) {
            if (this._token.Type != TokenType.Identifier || this._token.StringValue != keywordText)
                this.UnexpectedToken($"'{keywordText}'");
            else this.Advance();
        }

        private ImportSyntax ParseImport() {
            int startPosition = this._token.TokenStartPosition;

            this.CheckAndAdvance(TokenType.Import);

            if (this._token.Type == TokenType.LeftBrace) {
                this.Advance();

                var importReferences = new List<ImportTypeReferenceSyntax>();
                while (this._token.Type == TokenType.Identifier) {
                    NameSyntax nameSyntax = this.ParseName();
                    importReferences.Add(new ImportTypeReferenceSyntax(nameSyntax.Span, nameSyntax));
                }

                this.CheckAndAdvance(TokenType.RightBrace);

                this.CheckAndAdvanceIdentifierKeyword("from");

                QualifiableNameSyntax qualifier = this.ParseQualifiableName();
                return new ImportSyntax(this.TextSpanFrom(startPosition), importReferences.ToImmutableArray(), qualifier);
            }
            else {
                // This is the import all case
                QualifiableNameSyntax qualifier = this.ParseQualifiableName();
                return new ImportSyntax(this.TextSpanFrom(startPosition), qualifier);
            }
        }

        private UseSyntax ParseUse() {
            int startPosition = this._token.TokenStartPosition;

            this.CheckAndAdvance(TokenType.Use);

            FunctionInvocationSyntax libraryInfo = this.ParseFunctionInvocation(null);

            return new UseSyntax(this.TextSpanFrom(startPosition), libraryInfo);
        }

        private FunctionInvocationSyntax ParseFunctionInvocation(ExpressionSyntax functionReference) {
            int startPosition = functionReference.Span.Start;

            this.CheckAndAdvance(TokenType.LeftBrace);

            // Parse named arguments first
            var namedArguments = new List<ArgumentNameValuePairSyntax>();
            while (this._token.Type == TokenType.PropertyIdentifier) {
                int argumentStartPosition = this._token.TokenStartPosition;

                var propertyName = new QualifiableNameSyntax(this._token.TokenSourceSpanExceptForLastCharacter,
                    new QualifiableName(this._token.StringValue));
                var propertySpecifier = new PropertySpecifierSyntax(this._token.TokenSpan, propertyName);
                this.Advance();

                ExpressionSyntax value = this.ParseExpressionAllowTextualLiteral(new Tokens(TokenType.Eof),
                    TextualLiteralContext.PropertyValue);

                namedArguments.Add(new ArgumentNameValuePairSyntax(this.TextSpanFrom(argumentStartPosition), propertySpecifier, value));

                if (this._token.Type != TokenType.RightBrace && !this._token.IsAtStartOfLine)
                    this.CheckAndAdvance(TokenType.Semicolon);
            }

            // Parse the content property, if there is content
            ContentArgumentSyntax? contentArgument = null;
            if (this._token.Type != TokenType.RightBrace) {
                ExpressionSyntax contentValue = this.ParseExpressionAllowTextualLiteral(new Tokens(TokenType.Eof),
                    TextualLiteralContext.ContentPropertyValue);
                contentArgument = new ContentArgumentSyntax(contentValue.Span, contentValue);
            }

            this.CheckAndAdvance(TokenType.RightBrace);

            return new FunctionInvocationSyntax(this.TextSpanFrom(startPosition), InvocationStyle.Delimiter, functionReference, namedArguments.ToArray(), contentArgument);
        }

        private QualifiableNameSyntax ParseQualifiableName() {
            int startPosition = this._token.TokenStartPosition;

            StringBuilder buffer = new StringBuilder();
            while (true) {
                this.Check(TokenType.Identifier);
                buffer.Append(this._token.StringValue);
                this.Advance();

                if (this._token.Type != TokenType.Period)
                    break;
                buffer.Append('.');
                this._token.Advance();
            }

            return new QualifiableNameSyntax(this.TextSpanFrom(startPosition), new QualifiableName(buffer.ToString()));
        }

        private RecordTypeDefinitionSyntax ParseRecordTypeDefinition(Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            this.CheckAndAdvance(TokenType.Type, "type definition");

            NameSyntax typeNameSyntax = this.ParseName();

            this.CheckAndAdvance(TokenType.Assign);

            this.CheckAndAdvance(TokenType.LeftBrace);
            List<PropertyNameTypePairSyntax> propertyNameTypePairs = this.ParsePropertyNameTypePairs(synchronizingTokens.Add(TokenType.RightBrace));
            this.CheckOrSyncAndAdvance(TokenType.RightBrace, synchronizingTokens);

            return new RecordTypeDefinitionSyntax(this.TextSpanFrom(startPosition), typeNameSyntax, propertyNameTypePairs.ToArray());
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
            if (this._token.Type == TokenType.Identifier) {
                int startPosition = this._token.TokenStartPosition;

                QualifiableNameSyntax typeNameSyntax = this.ParseQualifiableName();
                TextSpan sourceSpan = typeNameSyntax.Span;
                string typeNameString = typeNameSyntax.Name.ToString();

                TypeReferenceSyntax typeReferenceSyntax;

                BuiltInTypeBinding? predefinedTypeBinding = BuiltInTypeBinding.GetBindingForTypeName(typeNameString);
                if (predefinedTypeBinding != null)
                    typeReferenceSyntax = new PredefinedTypeReferenceSyntax (sourceSpan, predefinedTypeBinding);
                else
                    typeReferenceSyntax = new ObjectTypeReferenceSyntax (sourceSpan, typeNameSyntax);

                if (this._token.Type == TokenType.Ellipsis) {
                    this.Advance();
                    return new SequenceTypeReferenceSyntax(this.TextSpanFrom(startPosition), typeReferenceSyntax);
                }
                else return typeReferenceSyntax;
            }
            else {
                this.UnexpectedToken("type name");
                return new InvalidTypeReferenceSyntax();
            }
        }

        private ExpressionSyntax ParseBooleanLiteral() {
            int startPosition = this._token.TokenStartPosition;

            if (this._token.Type == TokenType.True) {
                this.Advance();
                return new BooleanLiteralSyntax(this.TextSpanFrom(startPosition), true);
            }
            else if (this._token.Type == TokenType.False) {
                this.Advance();
                return new BooleanLiteralSyntax(this.TextSpanFrom(startPosition), false);
            }
            else {
                this.UnexpectedToken("boolean literal (true or false)");
                return new InvalidExpressionSyntax();
            }
        }

        private ExpressionSyntax ParseIntLiteral() {
            int startPosition = this._token.TokenStartPosition;

            if (this._token.Type == TokenType.Int32) {
                int tokenIntValue = this._token.IntValue;
                this.Advance();
                return new IntLiteralSyntax(this.TextSpanFrom(startPosition), tokenIntValue);
            }
            else {
                this.Advance();
                this.UnexpectedToken("int literal");
                return new InvalidExpressionSyntax();
            }
        }

        private void Check(TokenType expectedType) {
            this.Check(expectedType, expectedType.ToString());
        }

        private void Check(TokenType expectedType, string expected) {
            if (this._token.Type != expectedType)
                this.UnexpectedToken(expected);
        }

        private void CheckAndAdvance(TokenType expectedType) {
            this.Check(expectedType);
            if (!this._token.InErrorMode)
                this._token.Advance();
        }

        private void CheckOrSync(TokenType expectedType, Tokens synchronizingTokens) {
            this.Check(expectedType);
            if (this._token.InErrorMode)
                this.Sync(new Tokens(expectedType), synchronizingTokens);
        }

        private void CheckOrSync(TokenType expectedType, string expected, Tokens synchronizingTokens) {
            this.Check(expectedType, expected);
            if (this._token.InErrorMode)
                this.Sync(new Tokens(expectedType), synchronizingTokens);
        }

        private void CheckOrSyncAndAdvance(TokenType expectedType, Tokens synchronizingTokens) {
            this.Check(expectedType);
            if (this._token.InErrorMode)
                this.Sync(new Tokens(expectedType), synchronizingTokens);
            if (!this._token.InErrorMode)
                this._token.Advance();
        }

        private void AddError(TextSpan problemSourceSpan, string message) {
            var diagnostic = new Diagnostic(this._module, problemSourceSpan, DiagnosticSeverity.Error, message);
            this._module.AddDiagnostic(diagnostic);
        }

        private void UnexpectedToken(string expected) {
           if (!this._token.InErrorMode) {
                string message = $"Encountered {this._token.ToString()} when expected {expected}";
                this.AddError(this._token.TokenSpan, message);
                this._token.InErrorMode = true;
            }
        }

        private void Sync(Tokens? allowedTokens, Tokens otherTokens) {
            if (!this._token.InErrorMode)
                return;

            while (true) {
                TokenType currentTokenType = this._token.TypeForErrorMode;
                int column = this._token.TokenStartColumn;

                if (allowedTokens != null && allowedTokens.Contains(currentTokenType, column)) {
                    this._token.InErrorMode = false;
                    break;
                }

                if (otherTokens.Contains(currentTokenType, column))
                    break;
                
                this._token.Advance();
            }
        }

        private void CheckAndAdvance(TokenType expectedType, string expectedMessage) {
            this.Check(expectedType, expectedMessage);
            this._token.Advance();
        }

        private void Advance() {
            this._token.Advance();
        }

        public enum TextualLiteralContext {
            FunctionDefinition,
            PropertyValue,
            ContentPropertyValue
        }

        private ExpressionSyntax ParseExpressionAllowTextualLiteral(Tokens synchronizingTokens, TextualLiteralContext markupContext) {
            if (this._token.LooksLikeStartOfExpression()) {
                if (markupContext == TextualLiteralContext.FunctionDefinition)
                    return this.ParseSingleExpression(synchronizingTokens);
                else return this.ParseExpression(synchronizingTokens);
            }
            else {
                int startPosition = -1;
                var items = new List<TextualLiteralItemSyntax>();

                bool bracketed = false;
                if (this._token.Type == TokenType.LeftBracket) {
                    startPosition = this._token.TokenStartPosition;

                    this.CheckAndAdvance(TokenType.LeftBracket);
                    bracketed = true;
                }

                bool allowMultiline = bracketed || markupContext == TextualLiteralContext.ContentPropertyValue;

                bool allowSemicolonToTerminate = markupContext == TextualLiteralContext.PropertyValue;
                bool allowNewlineToTerminate = !allowMultiline;
                bool allowRightBraceToTerminate = markupContext == TextualLiteralContext.PropertyValue || markupContext == TextualLiteralContext.ContentPropertyValue;
                bool allowRightBracketToTerminate = bracketed;

                while (true) {
                    this._token.ReinterpretAlloWTextualLiteral(
                        allowSemicolonToTerminate: allowSemicolonToTerminate,
                        allowNewlineToTerminate: allowNewlineToTerminate,
                        allowRightBraceToTerminate: allowRightBraceToTerminate,
                        allowRightBracketToTerminate: allowRightBracketToTerminate);

                    if (startPosition == -1)
                        startPosition = this._token.TokenStartPosition;

                    if (this._token.Type == TokenType.TextualLiteralText) {
                        items.Add(new TextualLiteralTextItemSyntax(this._token.TokenSpan, this._token.StringValue));

                        bool atEnd = allowNewlineToTerminate && this._token.LookaheadIsNewline();

                        this.Advance();
                        if (atEnd)
                            break;
                    }
                    else if (this._token.Type == TokenType.LeftBrace) {
                        ExpressionSyntax expression = this.ParseExpression(synchronizingTokens);
                        items.Add(new TextualLiteralExpressionItemSyntax(expression));
                    }
                    else if (this._token.Type == TokenType.Identifier) {
                        ExpressionSyntax qualifiableSymbolReference = this.ParseQualifiableSymbolReference();
                        FunctionInvocationSyntax functionInvocation = this.ParseFunctionInvocation(qualifiableSymbolReference);

                        items.Add(new TextualLiteralExpressionItemSyntax(functionInvocation));
                    }
                    else break;
                }

                if (bracketed)
                    this.CheckAndAdvance(TokenType.RightBracket);

                return new TextualLiteralSyntax(this.TextSpanFrom(startPosition), items.ToImmutableArray());
            }
        }

        private ExpressionSyntax ParseExpression(Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            var expressions = new List<ExpressionSyntax>();

            while (true) {
                TokenType lookahead = this._token.Type;
                if (lookahead == TokenType.Eof || lookahead == TokenType.ErrorMode || lookahead == TokenType.Semicolon ||
                        lookahead == TokenType.RightBrace || lookahead == TokenType.PropertyIdentifier )
                    break;

                ExpressionSyntax expression = this.ParseSingleExpression(synchronizingTokens);
                expressions.Add(expression);
            }

            ExpressionSyntax overallExpression;
            if (expressions.Count == 1)
                overallExpression = expressions[0];
            else overallExpression = new SequenceLiteralExpressionSyntax(this.TextSpanFrom(startPosition), expressions.ToArray());

            return overallExpression;
        }

        private ExpressionSyntax ParseSingleExpression(Tokens synchronizingTokens) {
            return this.ParseExpressionComponent(0, synchronizingTokens);
        }

        /// <summary>
        /// Parse an expression, using the "Precedence climbing" algorithm, described at
        /// http://www.engr.mun.ca/~theo/Misc/exp_parsing.htm.
        /// </summary>
        /// <param name="precedence">precedence level</param>
        /// <param name="synchronizingTokens">error recovery synchronizing tokens</param>
        /// <remarks> AST subtree for the expression</remarks>
        private ExpressionSyntax ParseExpressionComponent(int precedence, Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            // Parse the prefix which can be a terminal, a parenthesized subexpression, or unary operator followed by an
            // operand. If there's a binary operator after the prefix, then the prefix acts as a left operand for that
            // operator
            ExpressionSyntax leftOperand = this.ParseExpressionPrefix(synchronizingTokens);

            // Now parse zero or more sets of binary operators followed by right operands, where the binary operator
            // precedence is <= precedence
            while (true) {
                InfixOperator infixOperator = Operator.GetInfixOperator(this._token.Type);
                if (infixOperator == null || infixOperator.GetPrecedence() < precedence)
                    break;
                this.Advance();

                if (infixOperator == Operator.Dot) {
                    if (this._token.LookaheadIsLeftBrace())
                        leftOperand = this.ParseFunctionInvocation(leftOperand);
                    else {
                        NameSyntax nameSyntax = this.ParseName();
                        leftOperand = new PropertyAccessSyntax(this.TextSpanFrom(startPosition), leftOperand, nameSyntax);
                    }
                }
                else if (infixOperator == Operator.For)
                    return this.ParseForExpression(leftOperand, synchronizingTokens);
                else {
                    ExpressionSyntax rightOperand =
                        this.ParseExpressionComponent(infixOperator.GetPrecedence() + 1, synchronizingTokens);

                    leftOperand = new InfixExpressionSyntax(this.TextSpanFrom(startPosition), leftOperand, infixOperator,
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
            int startPosition = this._token.TokenStartPosition;

            PrefixOperator prefixOperator = Operator.GetPrefixOperator(this._token.Type);

            if (prefixOperator != null) {
                this._token.Advance();
                ExpressionSyntax operand = this.ParseExpressionComponent(prefixOperator.GetPrecedence(), synchronizingTokens);
                return new PrefixExpressionSyntax(this.TextSpanFrom(startPosition), prefixOperator, operand);
            }
            else {
                switch (this._token.Type) {
                    case TokenType.LeftParen:
                        this._token.Advance();
                        ExpressionSyntax expression = this.ParseExpressionComponent(0, synchronizingTokens);
                        this.CheckAndAdvance(TokenType.RightParen);
                        return new ParenthesizedExpressionSyntax(this.TextSpanFrom(startPosition), expression);

                    case TokenType.True:
                    case TokenType.False:
                        return this.ParseBooleanLiteral();

                    case TokenType.Int32:
                        return this.ParseIntLiteral();

                    case TokenType.If:
                        return this.ParseIfExpression(synchronizingTokens);

                    case TokenType.Null:
                        this.Advance();
                        return new NullLiteralSyntax(this.TextSpanFrom(startPosition));

                    case TokenType.Identifier:
                        ExpressionSyntax qualifiableSymbolReference = this.ParseQualifiableSymbolReference();

                        if (this._token.Type == TokenType.LeftBrace)
                            return this.ParseFunctionInvocation(qualifiableSymbolReference);
                        else return qualifiableSymbolReference;

                    // TODO: FIX THIS, SEEING WHERE IT'S USED;  StringValue isn't set for ContextSensitiveText, for one thing
                    case TokenType.TextBlock:
                        string tokenStringValue = this._token.StringValue;
                        this._token.Advance();
                        return new StringLiteralSyntax(this.TextSpanFrom(startPosition), tokenStringValue);

                    case TokenType.LeftBrace:
                        this.Advance();

                        if (this._token.Type == TokenType.PropertyIdentifier) {
                            this.UnexpectedToken("function invocation without specifying the function name which isn't currently supported");
                            return new InvalidExpressionSyntax();

                            /*
                            var arguments = new List<ArgumentNameValuePair>();

                            parseArgumentNameValuePairs(arguments);
                            checkAndAdvance(TokenType.RIGHT_BRACE);

                            return new FunctionInvocation(sourceSpanFrom(startPosition), null, null, arguments.ToArray());
                            */
                        }
                        else {
                            ExpressionSyntax innerExpression = this.ParseExpression(synchronizingTokens);
                            this.CheckAndAdvance(TokenType.RightBrace);
                            return new BracedExpressionSyntax(this.TextSpanFrom(startPosition), innerExpression);
                        }

                    /*
                    case FUNCTION_NAME:
                        return parseFunctionInvocation();
                    */

                    default:
                        this.UnexpectedToken("start of expression");
                        return new InvalidExpressionSyntax();
                }
            }
        }

        private ExpressionSyntax ParseIfExpression(Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            bool haveIfToken = this._token.Type == TokenType.If;

            if (haveIfToken) {
                this.Advance();

                // TODO: Handle if ... is ... syntax
            }

            var conditionValuePairs = new List<ConditionValuePairSyntax>();
            TextSpan? elseTextBlock = null;

            while (true) {
                int ifItemStartPosition = this._token.TokenStartPosition;

                this.CheckOrSyncAndAdvance(TokenType.Pipe, synchronizingTokens.Add(TokenType.Colon));

                // "|:" indicates the else part of the if
                if (this._token.Type == TokenType.Colon) {
                    this.Advance();
                    this._token.RescanAsTextBlock(ifItemStartPosition, allowIfConditionPipeToTerminate: true);
                    elseTextBlock = this._token.TokenSpan;
                    this.Advance();

                    break;  // "else" must be the last item in the if
                }

                ExpressionSyntax conditionExpression = this.ParseExpression(synchronizingTokens.Add(TokenType.Pipe).Add(TokenType.Colon));
                this.CheckOrSyncAndAdvance(TokenType.Colon, synchronizingTokens);

                // TODO: Allow braces here
                //ExpressionSyntax valueExpression = ParseExpressionAllowSequence(synchronizingTokens.Add(TokenType.Pipe));

                this._token.RescanAsTextBlock(ifItemStartPosition, allowIfConditionPipeToTerminate: true);
                TextSpan valueTextBlock = this._token.TokenSpan;
                this.Advance();

                //ExpressionSyntax valueExpression = ParseExpressionAllowSequence(synchronizingTokens.Add(TokenType.Pipe));

                var conditionValuePair = new ConditionValuePairSyntax(this.TextSpanFrom(ifItemStartPosition), conditionExpression, valueTextBlock);
                conditionValuePairs.Add(conditionValuePair);

                if (this._token.Type != TokenType.Pipe)
                    break;
            }

            return new IfExpressionSyntax(this.TextSpanFrom(startPosition), conditionValuePairs.ToArray(), elseTextBlock);
        }

        private ExpressionSyntax ParseIfIsExpression(Tokens synchronizingTokens) {
            this.AddError(this._token.TokenSpan, $"if ... is ... syntax is not yet supported");
            return new InvalidExpressionSyntax();
        }

        private ExpressionSyntax ParseForExpression(ExpressionSyntax expression, Tokens synchronizingTokens) {
            int startPosition = this._token.TokenStartPosition;

            NameSyntax nameSyntax = this.ParseName();
            this.CheckAndAdvance(TokenType.In);
            ExpressionSyntax inExpression = this.ParseSingleExpression(synchronizingTokens);

            var forVariableDefinition = new ForVariableDefinitionSyntax(this.TextSpanFrom(startPosition), nameSyntax, inExpression);

            return new ForExpressionSyntax(this.TextSpanFrom(expression.Span.Start), expression, forVariableDefinition);
        }

        private TextSpan TextSpanFrom(int startPosition) {
            int endPosition = this._token.PrevTokenEndPosition;

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
