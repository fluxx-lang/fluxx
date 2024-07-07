using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public class IfExpressionSyntax : ExpressionSyntax
    {
        private readonly ConditionValuePairSyntax[] conditionValuePairs;
        private readonly TextSpan? elseSpan;
        private ExpressionSyntax? elseValue;
        private TypeBinding? typeBinding;

        public IfExpressionSyntax(TextSpan span, ConditionValuePairSyntax[] conditionValuePairs, TextSpan? elseSpan) : base(span)
        {
            this.conditionValuePairs = conditionValuePairs;
            foreach (ConditionValuePairSyntax conditionValuePair in conditionValuePairs)
            {
                conditionValuePair.SetParent(this);
            }

            this.elseSpan = elseSpan;
        }

        public ConditionValuePairSyntax[] ConditionValuePairs => this.conditionValuePairs;

        public ExpressionSyntax? ElseValue => this.elseValue;

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (ConditionValuePairSyntax conditionValuePair in this.conditionValuePairs)
            {
                visitor(conditionValuePair);
            }

            if (this.elseValue != null)
            {
                visitor(this.elseValue);
            }
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            TypeBinding? typeBinding = null;

            foreach (ConditionValuePairSyntax conditionValuePair in this.conditionValuePairs)
            {
                conditionValuePair.ParseValueSource(bindingResolver);

                TypeBinding currentTypeBinding = conditionValuePair.Value.GetTypeBinding();

                if (typeBinding == null)
                {
                    typeBinding = currentTypeBinding;
                }
                else
                {
                    if (!currentTypeBinding.IsSameAs(typeBinding))
                    {
                        this.AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            if (this.elseSpan.HasValue)
            {
                if (this.elseValue == null)
                {
                    this.elseValue = SourceParser.ParseTextBlockExpression(this.GetModule(), this.elseSpan.Value);
                    this.elseValue.SetParent(this);

                    // Now resolve the bindings on what we just parsed
                    this.elseValue.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });
                }

                TypeBinding currentTypeBinding = this.elseValue.GetTypeBinding();

                if (typeBinding == null)
                {
                    typeBinding = currentTypeBinding;
                }
                else
                {
                    if (!currentTypeBinding.IsSameAs(typeBinding))
                    {
                        this.AddError($"Different conditions of 'if' don't all evaluate to the same type");
                        return;
                    }
                }
            }

            this.typeBinding = typeBinding;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.typeBinding;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.IfExpression;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Writeln("if");

            foreach (ConditionValuePairSyntax conditionValuePair in this.conditionValuePairs)
            {
                sourceWriter.Writeln(conditionValuePair);
            }

            if (this.elseValue != null)
            {
                sourceWriter.Write("|: ");
                sourceWriter.Writeln(this.elseValue);
            }
        }
    }
}
