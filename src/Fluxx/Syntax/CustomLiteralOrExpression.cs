using System;
using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax
{
    public sealed class MarkupOrExpression
    {
        private readonly TextSpan customLiteralTextSpan;
        private ExpressionSyntax? expression;

        public MarkupOrExpression(TextSpan customLiteralTextSpan)
        {
            this.customLiteralTextSpan = customLiteralTextSpan;
        }

        public MarkupOrExpression(ExpressionSyntax expression)
        {
            this.expression = expression;
        }

        public ExpressionSyntax ResolveExpression(SyntaxNode parentNode, TypeBinding typeBinding, BindingResolver bindingResolver)
        {
            if (this.expression != null)
            {
                return this.expression;
            }

            if (this.customLiteralTextSpan.IsEmpty)
            {
                throw new InvalidOperationException("Both _expression and _customLiteralTextSpan are unset, which shouldn't happen");
            }

            FamlModule? module = parentNode.GetModule();

            ExpressionSyntax expression;
            if (typeBinding is ObjectTypeBinding objectTypeBinding && objectTypeBinding.SupportsCreateLiteral())
            {
                expression = objectTypeBinding.ParseLiteralValueSource(parentNode.GetModule(), this.customLiteralTextSpan);
                expression.SetParent(parentNode);
            }
            else
            {
                string customLiteralString = module.ModuleSyntax.SourceText.ToString(this.customLiteralTextSpan);
                expression = new InvalidExpressionSyntax(this.customLiteralTextSpan, customLiteralString, typeBinding);
            }

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            expression.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveBindings(bindingResolver); });

            return expression;
        }

#if false
        public string GetCustomLiteralValueString() {
            var literalValueCharIterator = new LiteralValueCharIterator(GetModule().SourceText, _customLiteralTextSpan); ;

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

                        default:
                            GetModule().AddError(Span, $"Unrecoginzed escape sequence: \\{next}");
                            break;
                    }
                }
                else if (curr == '\0')
                    break;
                else buffer.Append(curr);
            }

            return buffer.ToString();
        }
#endif
    }
}
