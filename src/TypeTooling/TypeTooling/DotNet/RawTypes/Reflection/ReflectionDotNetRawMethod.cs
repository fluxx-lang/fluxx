using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawMethod : DotNetRawMethod
    {
        private readonly MethodInfo _methodInfo;

        public ReflectionDotNetRawMethod(MethodInfo methodInfo)
        {
            _methodInfo = methodInfo;
        }

        public override DotNetRawParameter[] GetParameters()
        {
            return ToParameterDescriptors(_methodInfo.GetParameters());
        }

        public static DotNetRawParameter[] ToParameterDescriptors(ParameterInfo[] parameterInfos)
        {
            var parameterDescriptors = new DotNetRawParameter[parameterInfos.Length];
            for (int i = 0; i < parameterInfos.Length; i++)
                parameterDescriptors[i] = new ReflectionDotNetRawParameter(parameterInfos[i]);
            return parameterDescriptors;
        }

        public override DotNetRawType ReturnType => new ReflectionDotNetRawType(_methodInfo.ReturnType);

        public override string Name => _methodInfo.Name;

        public override bool IsStatic => _methodInfo.IsStatic;

        public MethodInfo MethodInfo => _methodInfo;
    }
}
