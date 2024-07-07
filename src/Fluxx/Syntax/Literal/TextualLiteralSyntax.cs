using System;
using System.Collections.Immutable;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal {
    public sealed class TextualLiteralSyntax : ExpressionSyntax {
        private ImmutableArray<TextualLiteralItemSyntax> _items;
        private TypeBinding? _typeBinding;

        public TextualLiteralSyntax(TextSpan span, ImmutableArray<TextualLiteralItemSyntax> items) : base(span) {
            _items = items;

            foreach (TextualLiteralItemSyntax item in _items)
                item.SetParent(this);
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteral;

        public override void VisitChildren(SyntaxVisitor visitor) {
            foreach (TextualLiteralItemSyntax item in _items) {
                visitor(item);
            }
        }

        public override TypeBinding GetTypeBinding() {
            if (_typeBinding == null)
                throw new InvalidOperationException("TypeBinding not set");
            return _typeBinding;
        }

        public ExpressionSyntax ResolveMarkup(TypeBinding typeBinding, BindingResolver bindingResolver) {
            _typeBinding = typeBinding;

            ExpressionSyntax expression;

            if (typeBinding == BuiltInTypeBinding.String || typeBinding == BuiltInTypeBinding.UIText)
                expression = this;
            else if (typeBinding is ObjectTypeBinding objectTypeBinding && objectTypeBinding.SupportsCreateLiteral()) {
                if (!ValidateThatSimpleText(typeBinding))
                    return this;

                TextSpan sourceSpan = GetSimpleTextSpan();
                expression = objectTypeBinding.ParseLiteralValueSource(GetModule(), sourceSpan);
            }
            else if (typeBinding is EnumTypeBinding enumTypeBinding) {
                if (!ValidateThatSimpleText(typeBinding))
                    return this;
                expression = enumTypeBinding.ParseEnumValue(GetModule(), Span);
            }
            else if (typeBinding == BuiltInTypeBinding.Bool) {
                if (!ValidateThatSimpleText(typeBinding))
                    return this;

                expression = SourceParser.ParseSingleBooleanLiteral(GetModule(), Span);
            }
            else if (typeBinding == BuiltInTypeBinding.Int) {
                if (!ValidateThatSimpleText(typeBinding))
                    return this;

                expression = SourceParser.ParseSingleIntLiteral(GetModule(), Span);
            }
            else {
                AddError($"Invalid value for type {typeBinding.TypeName}");

                string sourceString = GetModule().ModuleSyntax.SourceText.ToString(Span);
                expression = new InvalidExpressionSyntax(Span, sourceString, typeBinding);
            }

            if (!ReferenceEquals(expression, this))
                expression.SetParent(Parent);

            // TODO: Fix this up to pass type down
            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            expression.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveBindings(bindingResolver); });

            return expression;
        }

        private bool ValidateThatSimpleText(TypeBinding typeBinding) {
            if (!IsSimpleText) {
                AddError(
                    $"Invalid {typeBinding.TypeName} literal. Enclose the entire value in braces to specify an expression.");
                return false;
            }

            return true;
        }

        public bool IsSimpleText => _items.Length == 1 && _items[0] is TextualLiteralTextItemSyntax;

        public TextSpan GetSimpleTextSpan() {
            if (!IsSimpleText)
                throw new InvalidOperationException("Markup isn't simple text");
            return _items[0].Span;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            foreach (TextualLiteralItemSyntax item in _items)
                sourceWriter.Write(item);
        }
    }
}
