using System.Collections;
using System.Collections.Generic;
using Fluxx.Binding;
using TypeTooling;

namespace Fluxx.Interpreter
{
    public sealed class ForExpressionEval : ObjectEval
    {
        private readonly ObjectEval expressionEval;
        private readonly ObjectEval inExpressionEval;
        private readonly TypeBinding variableTypeBinding;

        public ForExpressionEval(ObjectEval expressionEval, TypeBinding variableTypeBinding, ObjectEval inExpressionEval)
        {
            this.expressionEval = expressionEval;
            this.variableTypeBinding = variableTypeBinding;
            this.inExpressionEval = inExpressionEval;
        }

        public override object Eval()
        {
            object sequence = this.inExpressionEval.Eval();

            if (!(sequence is IEnumerable enumerable))
            {
                throw new UserViewableException($"For-in expression sequence is unexpectedly type {sequence.GetType()}, not an IEnumerable");
            }

            var list = new List<object>();

            int variableStackOffset = Context.StackIndex++;
            foreach (object variableValue in enumerable)
            {
                if (this.variableTypeBinding == BuiltInTypeBinding.Int)
                {
                    Context.IntStack[variableStackOffset] = (int)variableValue;
                }
                else if (this.variableTypeBinding == BuiltInTypeBinding.Double)
                {
                    Context.DoubleStack[variableStackOffset] = (double)variableValue;
                }
                else if (this.variableTypeBinding is ObjectTypeBinding)
                {
                    Context.ObjectStack[variableStackOffset] = variableValue;
                }
                else
                {
                    throw new System.Exception(
                        $"Variable type {this.variableTypeBinding} currently not supported for 'for' expressions");
                }

                object expressionValue = this.expressionEval.Eval();
                list.Add(expressionValue);
            }

            Context.StackIndex--;

            return list;
        }
    }
}

