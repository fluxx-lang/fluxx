using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawField : DotNetRawField
    {
        private readonly FieldInfo _fieldInfo;

        public ReflectionDotNetRawField(FieldInfo fieldInfo)
        {
            this._fieldInfo = fieldInfo;
        }

        public override string Name => this._fieldInfo.Name;

        public override DotNetRawType FieldType => new ReflectionDotNetRawType(this._fieldInfo.FieldType);

        public override bool IsStatic => this._fieldInfo.IsStatic;

        public override bool IsPublic => this._fieldInfo.IsPublic;
    }
}
