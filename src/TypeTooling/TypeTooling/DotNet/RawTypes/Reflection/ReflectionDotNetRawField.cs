using System.Reflection;

namespace TypeTooling.DotNet.RawTypes.Reflection
{
    public class ReflectionDotNetRawField : DotNetRawField
    {
        private readonly FieldInfo fieldInfo;

        public ReflectionDotNetRawField(FieldInfo fieldInfo)
        {
            this.fieldInfo = fieldInfo;
        }

        public override string Name => this.fieldInfo.Name;

        public override DotNetRawType FieldType => new ReflectionDotNetRawType(this.fieldInfo.FieldType);

        public override bool IsStatic => this.fieldInfo.IsStatic;

        public override bool IsPublic => this.fieldInfo.IsPublic;
    }
}
