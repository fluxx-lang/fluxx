using System.Reflection;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.Types;

namespace TypeTooling.DotNet.Types
{
    public class DotNetInterpretedObjectCreator : InterpretedObjectCreator
    {
        private readonly DotNetRawConstructor _constructorMethod;
        private readonly PropertyInitializer[] _propertyInitializers;


        public DotNetInterpretedObjectCreator(DotNetRawConstructor constructorMethod, PropertyInitializer[] propertyInitializers)
        {
            this._constructorMethod = constructorMethod;
            this._propertyInitializers = propertyInitializers;
        }

        public override object Create(object[] values, int startOffset)
        {
            ConstructorInfo constructorInfo = ((ReflectionDotNetRawConstructor)this._constructorMethod).ConstructorInfo;

            object obj = constructorInfo.Invoke(null);

            int propertyCount = this._propertyInitializers.Length;
            for (int i = 0; i < propertyCount; i++)
                this._propertyInitializers[i].Initialize(obj, values[startOffset + i]);

            return obj;
        }
    }
}
