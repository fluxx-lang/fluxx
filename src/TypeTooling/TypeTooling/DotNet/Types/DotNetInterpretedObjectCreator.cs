using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetInterpretedObjectCreator(DotNetRawConstructor constructorMethod, PropertyInitializer[] propertyInitializers) : InterpretedObjectCreator
    {
        public override object Create(object[] values, int startOffset)
        {
            ConstructorInfo constructorInfo = ((ReflectionDotNetRawConstructor)constructorMethod).ConstructorInfo;

            object obj = constructorInfo.Invoke(null);

            int propertyCount = propertyInitializers.Length;
            for (int i = 0; i < propertyCount; i++)
            {
                propertyInitializers[i].Initialize(obj, values[startOffset + i]);
            }

            return obj;
        }
    }
}
