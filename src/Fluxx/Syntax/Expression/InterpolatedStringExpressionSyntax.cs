using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.CodeAnalysis.Text;


/**
 * Created by Bret on 6/28/2014.
 */

namespace Faml.Syntax.Expression {
    public sealed class InterpolatedStringExpressionSyntax : ExpressionSyntax {
        private readonly InterpolatedStringFragmentSyntax[] _stringFragments;
        private readonly ExpressionSyntax[] _expressions;

        // AST structure properties
        public InterpolatedStringExpressionSyntax(TextSpan span, InterpolatedStringFragmentSyntax[] stringFragments,
            ExpressionSyntax[] expressions) : base(span)
        {
            _stringFragments = stringFragments;
            foreach (InterpolatedStringFragmentSyntax stringFragment in stringFragments)
                stringFragment.SetParent(this);

            _expressions = expressions;
            foreach (ExpressionSyntax expression in expressions)
                expression.SetParent(this);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.InterpolatedStringExpression;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            int length = _expressions.Length;
            for (int i = 0; i < length; i++) {
                visitor(_stringFragments[i]);
                visitor(_expressions[i]);
            }
            visitor(_stringFragments[length - 1]);
        }

        public override Binding.TypeBinding GetTypeBinding() { return BuiltInTypeBinding.String; }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            int length = _expressions.Length;
            for (int i = 0; i < length; i++) {
                _stringFragments[i].WriteSource(sourceWriter);

                sourceWriter.Write("{");
                _expressions[i].WriteSource(sourceWriter);
                sourceWriter.Write("}");
            }
            _stringFragments[length - 1].WriteSource(sourceWriter);
        }

        public InterpolatedStringFragmentSyntax[] StringFragments => _stringFragments;

        public ExpressionSyntax[] Expressions => _expressions;
    }
}
