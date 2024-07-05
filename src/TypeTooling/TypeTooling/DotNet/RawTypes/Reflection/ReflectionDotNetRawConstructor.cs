using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawConstructor : DotNetRawConstructor
    {
        private readonly ConstructorInfo _constructorInfo;

        public ReflectionDotNetRawConstructor(ConstructorInfo constructorInfo)
        {
            this._constructorInfo = constructorInfo;
        }

        public override DotNetRawParameter[] GetParameters()
        {
            return ReflectionDotNetRawMethod.ToParameterDescriptors(this._constructorInfo.GetParameters());
        }

        public ConstructorInfo ConstructorInfo => this._constructorInfo;
    }
}
