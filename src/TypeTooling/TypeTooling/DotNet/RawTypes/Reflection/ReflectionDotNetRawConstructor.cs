using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawConstructor : DotNetRawConstructor
    {
        private readonly ConstructorInfo constructorInfo;

        public ReflectionDotNetRawConstructor(ConstructorInfo constructorInfo)
        {
            this.constructorInfo = constructorInfo;
        }

        public override DotNetRawParameter[] GetParameters()
        {
            return ReflectionDotNetRawMethod.ToParameterDescriptors(this.constructorInfo.GetParameters());
        }

        public ConstructorInfo ConstructorInfo => this.constructorInfo;
    }
}
