using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawParameter : DotNetRawParameter
    {
        private readonly ParameterInfo _parameterInfo;

        public ReflectionDotNetRawParameter(ParameterInfo parameterInfo)
        {
            this._parameterInfo = parameterInfo;
        }

        public override string Name => this._parameterInfo.Name;

        public override DotNetRawType ParameterType => new ReflectionDotNetRawType(this._parameterInfo.ParameterType);

        public ParameterInfo ParameterInfo => this._parameterInfo;
    }
}
