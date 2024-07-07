using System.Collections;
using System.Collections.Generic;
using Faml.Binding;
using TypeTooling;

namespace Faml.Interpreter
{
    public sealed class ForExpressionEval : ObjectEval
    {
        private readonly ObjectEval _expressionEval;
        private readonly ObjectEval _inExpressionEval;
        private readonly TypeBinding _variableTypeBinding;

        public ForExpressionEval(ObjectEval expressionEval, TypeBinding variableTypeBinding, ObjectEval inExpressionEval)
        {
            this._expressionEval = expressionEval;
            this._variableTypeBinding = variableTypeBinding;
            this._inExpressionEval = inExpressionEval;
        }

        public override object Eval()
        {
            object sequence = this._inExpressionEval.Eval();

            if (!(sequence is IEnumerable enumerable))
            {
                throw new UserViewableException($"For-in expression sequence is unexpectedly type {sequence.GetType()}, not an IEnumerable");
            }

            var list = new List<object>();

            int variableStackOffset = Context.StackIndex++;
            foreach (object variableValue in enumerable)
            {
                if (this._variableTypeBinding == BuiltInTypeBinding.Int)
                {
                    Context.IntStack[variableStackOffset] = (int)variableValue;
                }
                else if (this._variableTypeBinding == BuiltInTypeBinding.Double)
                {
                    Context.DoubleStack[variableStackOffset] = (double)variableValue;
                }
                else if (this._variableTypeBinding is ObjectTypeBinding)
                {
                    Context.ObjectStack[variableStackOffset] = variableValue;
                }
                else
                    throw new System.Exception(
                        $"Variable type {this._variableTypeBinding} currently not supported for 'for' expressions");

                object expressionValue = this._expressionEval.Eval();
                list.Add(expressionValue);
            }

            Context.StackIndex--;

            return list;
        }
    }
}

