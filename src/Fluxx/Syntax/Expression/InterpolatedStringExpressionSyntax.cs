using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * Created by Bret on 6/28/2014.
 */

namespace Faml.Syntax.Expression
{
    public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax
    {
        private readonly InterpolatedStringFragmentSyntax[] _stringFragments;
        private readonly ExpressionSyntax[] _expressions;

        // AST structure properties
        public InterpolatedStringExpressionSyntax(TextSpan span, InterpolatedStringFragmentSyntax[] stringFragments,
            ExpressionSyntax[] expressions) : base(span)
        {
            this._stringFragments = stringFragments;
            foreach (InterpolatedStringFragmentSyntax stringFragment in stringFragments)
            {
                stringFragment.SetParent(this);
            }

            this._expressions = expressions;
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
            int length = this._expressions.Length;
            for (int i = 0; i < length; i++)
            {
                visitor(this._stringFragments[i]);
                visitor(this._expressions[i]);
            }

            visitor(this._stringFragments[length - 1]);
        }

        public override Binding.TypeBinding GetTypeBinding() { return BuiltInTypeBinding.String; }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            int length = this._expressions.Length;
            for (int i = 0; i < length; i++)
            {
                this._stringFragments[i].WriteSource(sourceWriter);

                sourceWriter.Write("{");
                this._expressions[i].WriteSource(sourceWriter);
                sourceWriter.Write("}");
            }

            this._stringFragments[length - 1].WriteSource(sourceWriter);
        }

        public InterpolatedStringFragmentSyntax[] StringFragments => this._stringFragments;

        public ExpressionSyntax[] Expressions => this._expressions;
    }
}
