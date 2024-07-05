using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetInterpretedObjectCreator : InterpretedObjectCreator
    {
        private readonly DotNetRawConstructor constructorMethod;
        private readonly PropertyInitializer[] propertyInitializers;


        public DotNetInterpretedObjectCreator(DotNetRawConstructor constructorMethod, PropertyInitializer[] propertyInitializers)
        {
            this.constructorMethod = constructorMethod;
            this.propertyInitializers = propertyInitializers;
        }

        public override object Create(object[] values, int startOffset)
        {
            ConstructorInfo constructorInfo = ((ReflectionDotNetRawConstructor)this.constructorMethod).ConstructorInfo;

            object obj = constructorInfo.Invoke(null);

            int propertyCount = this.propertyInitializers.Length;
            for (int i = 0; i < propertyCount; i++)
            {
                this.propertyInitializers[i].Initialize(obj, values[startOffset + i]);
            }

            return obj;
        }
    }
}
