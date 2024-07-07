using System.Collections.Generic;
using Fluxx.Api;
using TypeTooling.Types;
using TypeTooling.Types.PredefinedTypes;

namespace Fluxx.Binding
{
    public class BuiltInTypeBinding : TypeBinding
    {
        public TypeToolingType TypeToolingType { get; }

        private static readonly Dictionary<string, BuiltInTypeBinding> bindings = new Dictionary<string, BuiltInTypeBinding>();

        public static readonly BuiltInTypeBinding Bool = new BuiltInTypeBinding("bool", BooleanType.Instance);
        public static readonly BuiltInTypeBinding Int = new BuiltInTypeBinding("int", IntegerType.Instance);
        public static readonly BuiltInTypeBinding Double = new BuiltInTypeBinding("double", DoubleType.Instance);
        public static readonly BuiltInTypeBinding String = new BuiltInTypeBinding("string", StringType.Instance);
        public static readonly BuiltInTypeBinding UIText = new BuiltInTypeBinding("uitext", UITextType.Instance);
        public static readonly BuiltInTypeBinding Null = new BuiltInTypeBinding("null", null);

        public static BuiltInTypeBinding? GetBindingForTypeName(string typeName)
        {
            if (!bindings.TryGetValue(typeName, out BuiltInTypeBinding binding))
            {
                return null;
            }

            return binding;
        }

        // TODO: This is a hack--replace it eventually
        public static readonly BuiltInTypeBinding Event = new BuiltInTypeBinding("Event", null);

        public BuiltInTypeBinding(string typeName, TypeToolingType typeToolingType) : base(new QualifiableName(typeName), TypeFlags.None)
        {
            this.TypeToolingType = typeToolingType;

            bindings.Add(typeName, this);
        }
    }
}
