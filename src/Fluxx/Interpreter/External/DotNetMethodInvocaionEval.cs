using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace Faml.Interpreter.External
{
    public sealed class DotNetMethodInvocaionEval : ObjectEval
    {
        private readonly ObjectEval _thisArgumentEval;
        private readonly ObjectEval[] _argumentEvals;
        private readonly DotNetRawMethod _method;

        public DotNetMethodInvocaionEval(ObjectEval thisArgumentEval, ObjectEval[] argumentEvals, DotNetRawMethod method)
        {
            this._thisArgumentEval = thisArgumentEval;
            this._argumentEvals = argumentEvals;
            this._method = method;
        }

        public override object Eval()
        {
            object thisArgument = this._thisArgumentEval.Eval();

            int argumentsLength = this._argumentEvals.Length;
            object[] arguments = new object[argumentsLength];
            for (int i = 0; i < argumentsLength; i++)
            {
                arguments[i] = this._argumentEvals[i].Eval();
            }

            MethodInfo methodInfo = ((ReflectionDotNetRawMethod) this._method).MethodInfo;
            return methodInfo.Invoke(thisArgument, arguments);
        }
    }
}
