using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;

namespace Faml.Interpreter.External
{
    public sealed class DotNetMethodInvocaionEval : ObjectEval
    {
        private readonly ObjectEval thisArgumentEval;
        private readonly ObjectEval[] argumentEvals;
        private readonly DotNetRawMethod method;

        public DotNetMethodInvocaionEval(ObjectEval thisArgumentEval, ObjectEval[] argumentEvals, DotNetRawMethod method)
        {
            this.thisArgumentEval = thisArgumentEval;
            this.argumentEvals = argumentEvals;
            this.method = method;
        }

        public override object Eval()
        {
            object thisArgument = this.thisArgumentEval.Eval();

            int argumentsLength = this.argumentEvals.Length;
            object[] arguments = new object[argumentsLength];
            for (int i = 0; i < argumentsLength; i++)
            {
                arguments[i] = this.argumentEvals[i].Eval();
            }

            MethodInfo methodInfo = ((ReflectionDotNetRawMethod)this.method).MethodInfo;
            return methodInfo.Invoke(thisArgument, arguments);
        }
    }
}
