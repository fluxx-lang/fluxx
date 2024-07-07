using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using TypeTooling.ClassifiedText;

namespace Faml.Binding
{
    public abstract class TypeBinding
    {
        private readonly QualifiableName typeName;
        private readonly TypeFlags typeFlags;

        protected TypeBinding(QualifiableName typeName, TypeFlags typeFlags)
        {
            this.typeName = typeName;
            this.typeFlags = typeFlags;
        }

        public QualifiableName TypeName => this.typeName;

        public bool IsReactive => (this.typeFlags & TypeFlags.IsReactive) != 0;

        public bool IsSameAs(TypeBinding other)
        {
            // Two types are considered the same if they have the same name. FAML uses nominal, not structural, typing.
            return this.typeName.Equals(other.typeName);
        }

        public virtual bool IsAssignableFrom(TypeBinding other)
        {
            // By default, types are only assignable if they are the same, but subclasses can broaden this
            return this.IsSameAs(other);
        }

        // TODO: Make use of this
        public virtual IEnumerable<TypeBinding> GetSupertypesAndSelf()
        {
            return new List<TypeBinding> { this };
        }

        public bool IsValid()
        {
            return !(this is InvalidTypeBinding);
        }

        public virtual FunctionBinding? GetMethodBinding(Name methodName)
        {
            return null;
        }

        public virtual PropertyBinding? GetPropertyBinding(Name propertyName)
        {
            return null;
        }

        public virtual Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name}: {this.TypeName.ToString()}";
        }
    }
}
