using System;
using TypeTooling.Types;

namespace TypeTooling.Xaml.CustomAttributes
{
    /// <summary>
    /// Specifies what type to use as a converter for the object this attribute is bound to.
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class TreatAsTypeAttribute : Attribute
    {
        private readonly TypeToolingType _type;

        public TreatAsTypeAttribute(TypeToolingType type)
        {
            _type = type;
        }

        public TypeToolingType Type => _type;
    }
}
