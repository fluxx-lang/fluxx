using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        private readonly ConditionValuePairSyntax[] _conditionValuePairs;
        private readonly TextSpan? _elseSpan;
        private ExpressionSyntax? _elseValue;
        private TypeBinding? _typeBinding;

        public IfExpressionSyntax(TextSpan span, ConditionValuePairSyntax[] conditionValuePairs, TextSpan? elseSpan) : base(span)
        {
            this._conditionValuePairs = conditionValuePairs;
            foreach (ConditionValuePairSyntax conditionValuePair in conditionValuePairs)
            {
                conditionValuePair.SetParent(this);
            }

            this._elseSpan = elseSpan;
        }

        public ConditionValuePairSyntax[] ConditionValuePairs => this._conditionValuePairs;

        public ExpressionSyntax? ElseValue => this._elseValue;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (ConditionValuePairSyntax conditionValuePair in this._conditionValuePairs)
            {
                visitor(conditionValuePair);
            }

            if (this._elseValue != null)
            {
                visitor(this._elseValue);
            }
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            TypeBinding? typeBinding = null;

            foreach (ConditionValuePairSyntax conditionValuePair in this._conditionValuePairs)
            {
                conditionValuePair.ParseValueSource(bindingResolver);

                TypeBinding currentTypeBinding = conditionValuePair.Value.GetTypeBinding();

                if (typeBinding == null)
                    typeBinding = currentTypeBinding;
                else
                {
                    if (!currentTypeBinding.IsSameAs(typeBinding))
                    {
                        this.AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            if (this._elseSpan.HasValue)
            {
                if (this._elseValue == null)
                {
                    this._elseValue = SourceParser.ParseTextBlockExpression(this.GetModule(), this._elseSpan.Value);
                    this._elseValue.SetParent(this);

                    // Now resolve the bindings on what we just parsed
                    this._elseValue.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
                }

                TypeBinding currentTypeBinding = this._elseValue.GetTypeBinding();

                if (typeBinding == null)
                    typeBinding = currentTypeBinding;
                else
                {
                    if (!currentTypeBinding.IsSameAs(typeBinding))
                    {
                        this.AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            this._typeBinding = typeBinding;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.IfExpression;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Writeln("if");

            foreach (ConditionValuePairSyntax conditionValuePair in this._conditionValuePairs)
            {
                sourceWriter.Writeln(conditionValuePair);
            }

            if (this._elseValue != null)
            {
                sourceWriter.Write("|: ");
                sourceWriter.Writeln(this._elseValue);
            }
        }
    }
}
