using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;


/**
 * @author Bret Johnson
 * @since 4/4/2015
 */
namespace Faml.Interpreter.External {
    public sealed class DotNetMethodInvocaionEval : ObjectEval {
        private readonly ObjectEval _thisArgumentEval;
        private readonly ObjectEval[] _argumentEvals;
        private readonly DotNetRawMethod _method;

        public DotNetMethodInvocaionEval(ObjectEval thisArgumentEval, ObjectEval[] argumentEvals, DotNetRawMethod method) {
            _thisArgumentEval = thisArgumentEval;
            _argumentEvals = argumentEvals;
            _method = method;
        }

        public override object Eval() {
            object thisArgument = _thisArgumentEval.Eval();

            int argumentsLength = _argumentEvals.Length;
            object[] arguments = new object[argumentsLength];
            for (int i = 0; i < argumentsLength; i++)
                arguments[i] = _argumentEvals[i].Eval();

            MethodInfo methodInfo = ((ReflectionDotNetRawMethod) _method).MethodInfo;
            return methodInfo.Invoke(thisArgument, arguments);
        }
    }
}
