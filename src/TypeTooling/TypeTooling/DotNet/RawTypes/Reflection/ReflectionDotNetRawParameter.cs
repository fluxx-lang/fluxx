using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawParameter : DotNetRawParameter
    {
        private readonly ParameterInfo parameterInfo;

        public ReflectionDotNetRawParameter(ParameterInfo parameterInfo)
        {
            this.parameterInfo = parameterInfo;
        }

        public override string Name => this.parameterInfo.Name;

        public override DotNetRawType ParameterType => new ReflectionDotNetRawType(this.parameterInfo.ParameterType);

        public ParameterInfo ParameterInfo => this.parameterInfo;
    }
}
