using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace TypeTooling.Xaml {
    public class XamlAttachedProperty : AttachedProperty {
        // Automatic properties
        public override string Name { get; }

        public DotNetRawMethod SetterMethod { get; }

        public override TypeToolingType Type { get; }

        public override ObjectType TargetObjectType { get; }

        public XamlAttachedProperty(AttachedType attachedType, string propertyName, DotNetRawMethod setterMethod, TypeToolingType type, ObjectType targetObjectType) : base(attachedType) {
            Name = propertyName;
            SetterMethod = setterMethod;
            Type = type;
            TargetObjectType = targetObjectType;
        }
    }
}
