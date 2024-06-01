using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection {
    public class ReflectionDotNetRawParameter : DotNetRawParameter {
        private readonly ParameterInfo _parameterInfo;

        public ReflectionDotNetRawParameter(ParameterInfo parameterInfo) {
            _parameterInfo = parameterInfo;
        }

        public override string Name => _parameterInfo.Name;

        public override DotNetRawType ParameterType => new ReflectionDotNetRawType(_parameterInfo.ParameterType);

        public ParameterInfo ParameterInfo => _parameterInfo;
    }
}
