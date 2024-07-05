using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawField : DotNetRawField
    {
        private readonly FieldInfo _fieldInfo;

        public ReflectionDotNetRawField(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
        }

        public override string Name => _fieldInfo.Name;

        public override DotNetRawType FieldType => new ReflectionDotNetRawType(_fieldInfo.FieldType);

        public override bool IsStatic => _fieldInfo.IsStatic;

        public override bool IsPublic => _fieldInfo.IsPublic;
    }
}
