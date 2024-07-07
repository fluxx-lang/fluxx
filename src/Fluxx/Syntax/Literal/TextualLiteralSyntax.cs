using System;
using System.Collections.Immutable;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Literal
{
    public sealed class TextualLiteralSyntax : ExpressionSyntax
    {
        private ImmutableArray<TextualLiteralItemSyntax> items;
        private TypeBinding? typeBinding;

        public TextualLiteralSyntax(TextSpan span, ImmutableArray<TextualLiteralItemSyntax> items) : base(span)
        {
            this.items = items;

            foreach (TextualLiteralItemSyntax item in this.items)
            {
                item.SetParent(this);
            }
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.TextualLiteral;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (TextualLiteralItemSyntax item in this.items)
            {
                visitor(item);
            }
        }

        public override TypeBinding GetTypeBinding()
        {
            if (this.typeBinding == null)
            {
                throw new InvalidOperationException("TypeBinding not set");
            }

            return this.typeBinding;
        }

        public ExpressionSyntax ResolveMarkup(TypeBinding typeBinding, BindingResolver bindingResolver)
        {
            this.typeBinding = typeBinding;

            ExpressionSyntax expression;

            if (typeBinding == BuiltInTypeBinding.String || typeBinding == BuiltInTypeBinding.UIText)
            {
                expression = this;
            }
            else if (typeBinding is ObjectTypeBinding objectTypeBinding && objectTypeBinding.SupportsCreateLiteral())
            {
                if (!this.ValidateThatSimpleText(typeBinding))
                {
                    return this;
                }

                TextSpan sourceSpan = this.GetSimpleTextSpan();
                expression = objectTypeBinding.ParseLiteralValueSource(this.GetModule(), sourceSpan);
            }
            else if (typeBinding is EnumTypeBinding enumTypeBinding)
            {
                if (!this.ValidateThatSimpleText(typeBinding))
                {
                    return this;
                }

                expression = enumTypeBinding.ParseEnumValue(this.GetModule(), this.Span);
            }
            else if (typeBinding == BuiltInTypeBinding.Bool)
            {
                if (!this.ValidateThatSimpleText(typeBinding))
                {
                    return this;
                }

                expression = SourceParser.ParseSingleBooleanLiteral(this.GetModule(), this.Span);
            }
            else if (typeBinding == BuiltInTypeBinding.Int)
            {
                if (!this.ValidateThatSimpleText(typeBinding))
                {
                    return this;
                }

                expression = SourceParser.ParseSingleIntLiteral(this.GetModule(), this.Span);
            }
            else
            {
                this.AddError($"Invalid value for type {typeBinding.TypeName}");

                string sourceString = this.GetModule().ModuleSyntax.SourceText.ToString(this.Span);
                expression = new InvalidExpressionSyntax(this.Span, sourceString, typeBinding);
            }

            if (!ReferenceEquals(expression, this))
            {
                expression.SetParent(this.Parent);
            }

            // TODO: Fix this up to pass type down
            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            expression.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveBindings(bindingResolver); });

            return expression;
        }

        private bool ValidateThatSimpleText(TypeBinding typeBinding)
        {
            if (!this.IsSimpleText)
            {
                this.AddError(
                    $"Invalid {typeBinding.TypeName} literal. Enclose the entire value in braces to specify an expression.");
                return false;
            }

            return true;
        }

        public bool IsSimpleText => this.items.Length == 1 && this.items[0] is TextualLiteralTextItemSyntax;

        public TextSpan GetSimpleTextSpan()
        {
            if (!this.IsSimpleText)
            {
                throw new InvalidOperationException("Markup isn't simple text");
            }

            return this.items[0].Span;
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            foreach (TextualLiteralItemSyntax item in this.items)
            {
                sourceWriter.Write(item);
            }
        }
    }
}
