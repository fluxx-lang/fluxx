using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
    {
        private readonly InterpolatedStringFragmentSyntax[] stringFragments;
        private readonly ExpressionSyntax[] expressions;

        // AST structure properties
        public InterpolatedStringExpressionSyntax(TextSpan span, InterpolatedStringFragmentSyntax[] stringFragments,
            ExpressionSyntax[] expressions) : base(span)
        {
            this.stringFragments = stringFragments;
            foreach (InterpolatedStringFragmentSyntax stringFragment in stringFragments)
            {
                stringFragment.SetParent(this);
            }

            this.expressions = expressions;
            foreach (ExpressionSyntax expression in expressions)
            {
                expression.SetParent(this);
            }
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InterpolatedStringExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            int length = this.expressions.Length;
            for (int i = 0; i < length; i++)
            {
                visitor(this.stringFragments[i]);
                visitor(this.expressions[i]);
            }

            visitor(this.stringFragments[length - 1]);
        }

        public override Binding.TypeBinding GetTypeBinding() { return BuiltInTypeBinding.String; }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            int length = this.expressions.Length;
            for (int i = 0; i < length; i++)
            {
                this.stringFragments[i].WriteSource(sourceWriter);

                sourceWriter.Write("{");
                this.expressions[i].WriteSource(sourceWriter);
                sourceWriter.Write("}");
            }

            this.stringFragments[length - 1].WriteSource(sourceWriter);
        }

        public InterpolatedStringFragmentSyntax[] StringFragments => this.stringFragments;

        public ExpressionSyntax[] Expressions => this.expressions;
    }
}
