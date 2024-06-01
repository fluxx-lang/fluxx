using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection {
    public class ReflectionDotNetRawConstructor : DotNetRawConstructor {
        private readonly ConstructorInfo _constructorInfo;


        public ReflectionDotNetRawConstructor(ConstructorInfo constructorInfo) {
            _constructorInfo = constructorInfo;
        }

        public override DotNetRawParameter[] GetParameters() {
            return ReflectionDotNetRawMethod.ToParameterDescriptors(_constructorInfo.GetParameters());
        }

        public ConstructorInfo ConstructorInfo => _constructorInfo;
    }
}
