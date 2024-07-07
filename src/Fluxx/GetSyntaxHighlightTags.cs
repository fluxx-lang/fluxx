using System;
using System.Collections.Generic;
using Faml.Api;
using Faml.CodeAnalysis.Text;
using Faml.Lexer;
using Faml.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Faml {
    internal class GetSyntaxHighlightTags {
        private readonly ModuleSyntax _module;
        
        public GetSyntaxHighlightTags(ModuleSyntax module) {
            _module = module;
        }

        // TODO: Implement this properly, supporting multiple spans
        public void GetTags(TextSpan[] textSpans, List<SyntaxHighlightTag> tags) {
            if (textSpans.Length == 1)
                GetTags(textSpans[0], tags);
            else throw new NotImplementedException();
        }

        public void GetTags(TextSpan span, List<SyntaxHighlightTag> tags) {
            // First return the lexical tokens that occur before the first terminal node. Getting the tags for an AST node
            // returns the lexical tags that come after it, but we need to do this here to get the lexical tags at the beginning
            SyntaxNode? firstTerminalNode = _module.GetNextTerminalNodeFromPosition(span.Start);
            int startOfFirstTerminalNode = firstTerminalNode?.Span.Start ?? -1;
            var token = new Token(new ParseableSource(_module.SourceText, span.Start, _module.SourceText.Length));
            GetLexicalTags(span, token, startOfFirstTerminalNode, tags);

            // Now get everything else
            GetSyntaxNodeTags(span, _module, tags);
        }

        private void GetSyntaxNodeTags(TextSpan span, SyntaxNode syntaxNode, List<SyntaxHighlightTag> tags) {
            if (!syntaxNode.OverlapsWith(span))
                return;

            // If there's not source associated with the node (true for some Invalid... nodes generated via parser error recovery), add nothing
            if (syntaxNode.Span.IsNull())
                return;

            if (syntaxNode.IsTerminalNode()) {
                //                if (currPosition[0] != astNodeProxy.sourceSpan.startPosition) {
                //                    log("Curr position " + currPosition[0] + " does not match node start position " + astNodeProxy.sourceSpan.startPosition);
                //                }

                TextSpan terminalSnapshotSpan = syntaxNode.Span;
                int currNodeEndPosition = syntaxNode.Span.End;

                SyntaxNodeType nodeType = syntaxNode.NodeType;

                switch (nodeType)
                {
                    case SyntaxNodeType.BooleanLiteral:
                    case SyntaxNodeType.NullLiteral:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.Keyword));
                        break;
                    case SyntaxNodeType.IntLiteral:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.NumberLiteral));
                        break;
                    case SyntaxNodeType.DotNetEnumValue:
                    case SyntaxNodeType.ExternalTypeLiteral:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.PropertyValue));
                        break;
                    case SyntaxNodeType.StringLiteral:
                    case SyntaxNodeType.InterpolatedStringFragment:
                    case SyntaxNodeType.TextualLiteralTextItem:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.UIText));
                        break;
                    case SyntaxNodeType.NameIdentifier:
                    case SyntaxNodeType.QualifiableName:
                    {
                        SyntaxNodeType parentNodeType = syntaxNode.Parent.NodeType;

                        SyntaxHighlightTagType syntaxHighlightTagType;
                        if (parentNodeType == SyntaxNodeType.PredefinedTypeReference || parentNodeType == SyntaxNodeType.ObjectTypeReference)
                            syntaxHighlightTagType = SyntaxHighlightTagType.TypeReference;
                        else if (parentNodeType == SyntaxNodeType.PropertySpecifier || parentNodeType == SyntaxNodeType.PropertyNameTypePair)
                            syntaxHighlightTagType = SyntaxHighlightTagType.PropertyReference;
                        else if (parentNodeType == SyntaxNodeType.FunctionDefinition)
                            syntaxHighlightTagType = SyntaxHighlightTagType.FunctionReference;
                        else if (parentNodeType == SyntaxNodeType.RecordTypeDefinition)
                            syntaxHighlightTagType = SyntaxHighlightTagType.TypeReference;
                        else if (syntaxNode.Parent?.Parent?.NodeType == SyntaxNodeType.FunctionInvocation)
                            /*  && ((FunctionInvocation) nameParent).functionBinding is SyntaxNodeType.NewCSharpObjectFunctionBinding */
                            syntaxHighlightTagType = SyntaxHighlightTagType.FunctionReference;
                        else syntaxHighlightTagType = SyntaxHighlightTagType.SymbolReference;

                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, syntaxHighlightTagType));
                        break;
                    }
                    case SyntaxNodeType.PredefinedTypeReference:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.TypeReference));
                        break;
                    case SyntaxNodeType.InvalidExpression:
                        tags.Add(new SyntaxHighlightTag(terminalSnapshotSpan, SyntaxHighlightTagType.InvalidToken));
                        break;
                    default:
                        throw new Exception($"Unexpected AST terminal node type: {nodeType}");
                }

                int nextTerminalNodeStartPosition;
                if (currNodeEndPosition >= _module.Span.End)
                    nextTerminalNodeStartPosition = -1;
                else {
                    SyntaxNode? nextTerminalNode = _module.GetNextTerminalNodeFromPosition(currNodeEndPosition);
                    if (nextTerminalNode == null) {
                        // This shouldn't happen, but gracefully fail if it does
                        nextTerminalNodeStartPosition = -1;
                    }
                    else nextTerminalNodeStartPosition = nextTerminalNode.Span.Start;
                }

                // Now process the lexical tokens (which aren't included in the AST) after the current terminal node up to the next terminal node
                var token = new Token(new ParseableSource(_module.SourceText, currNodeEndPosition, _module.SourceText.Length));
                GetLexicalTags(span, token, nextTerminalNodeStartPosition, tags);
            }
            else {
                syntaxNode.VisitChildren((child) => {
                    if (child.OverlapsWith(span))
                        GetSyntaxNodeTags(span, child, tags);
                });
            }
        }

        private static void GetLexicalTags(TextSpan span, Token token, int endPosition, List<SyntaxHighlightTag> sourceTags) {
            while (true) {
                // If there's an endPosition specified and we are at or past it, then we're done
                if (endPosition != -1) {
                    if (token.TokenStartPosition >= endPosition)
                        break;
                }

                //Span? tokenRange = new Span(currSpan.Start.Position + token.tokenStartPosition, token.tokenLength).Intersection(currSpan);
                //                if (tokenRange == null)
                //                    continue;

                if (token.Type == TokenType.Eof)
                    break;

                if (token.TokenSpan.OverlapsWith(span))
                    sourceTags.Add(GetTokenSourceTag(token));

                token.Advance();
            }
        }

        private static SyntaxHighlightTag GetTokenSourceTag(Token token)
        {
            TextSpan tokenSpan = token.TokenSpan;

            switch (token.Type)
            {
                case TokenType.Not:
                case TokenType.Times:
                case TokenType.Divide:
                case TokenType.Remainder:
                case TokenType.Plus:
                case TokenType.Minus:
                case TokenType.Less:
                case TokenType.Greater:
                case TokenType.LessEquals:
                case TokenType.GreaterEquals:
                case TokenType.Equals:
                case TokenType.NotEquals:
                case TokenType.And:
                case TokenType.Or:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.Operator);

                case TokenType.Assign:
                case TokenType.LeftBracket:
                case TokenType.RightBracket:
                case TokenType.LeftParen:
                case TokenType.RightParen:
                case TokenType.Comma:
                case TokenType.Period:
                case TokenType.Pipe:
                case TokenType.Ellipsis:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.Punctuation);

                case TokenType.Colon:
                case TokenType.LeftBrace:
                case TokenType.RightBrace:
                case TokenType.Semicolon:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.DeemphasizedPunctuation);

                case TokenType.Identifier:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.SymbolReference);

                case TokenType.PropertyIdentifier:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.PropertyReference);

                /*
                    case TokenType.LITERAL_PROPERTY: {
                        yield return
                            new TagSpan<ClassificationTag>(tokenSnapshotSpan, new ClassificationTag(_famlClassificationTagger.property));
                        token.advanceForArgumentValue();

                        tokenSnapshotSpan = new SnapshotSpan(_textSnapshot, token.tokenStartPosition, token.tokenLength);
                        yield return
                            new TagSpan<ClassificationTag>(tokenSnapshotSpan,
                                new ClassificationTag(_luxClassificationTagger.propertyValue));
                        break;
                    }
                    */

                case TokenType.Int32:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.NumberLiteral);

                case TokenType.True:
                case TokenType.False:
                case TokenType.Null:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.Keyword);

                case TokenType.If:
                case TokenType.Is:
                case TokenType.Else:
                case TokenType.For:
                case TokenType.In:
                case TokenType.Import:
                case TokenType.Use:
                case TokenType.Type:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.ControlKeyword);

                /*
                                        case TokenType.TEXT:
                                            yield return
                                                new TagSpan<ClassificationTag>(tokenSnapshotSpan, new ClassificationTag(_stringType));
                                            break;
                    */

                case TokenType.Invalid:
                    return new SyntaxHighlightTag(tokenSpan, SyntaxHighlightTagType.InvalidToken);

                default:
                    throw new Exception($"Unknown token type: {token.Type}");
            }
        }
    }
}
