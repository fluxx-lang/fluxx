using Fluxx.Api;
using TypeTooling.Types;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Fluxx.Binding.External
{
    public class ExternalAttachedTypeBinding : AttachedTypeBinding
    {
        private readonly FamlProject project;
        private readonly AttachedType attachedType;

        public ExternalAttachedTypeBinding(FamlProject project, AttachedType attachedType) : base(new QualifiableName(attachedType.FullName))
        {
            this.project = project;
            this.attachedType = attachedType;
        }

        public FamlProject Project => this.project;

        public AttachedType AttachedType => this.attachedType;

        public AttachedProperty GetAttachedProperty(Name propertyName)
        {
            foreach (AttachedProperty attachedProperty in this.attachedType.AttachedProperties)
            {
                if (attachedProperty.Name == propertyName.ToString())
                {
                    return attachedProperty;
                }
            }

            return null;
        }
    }
}
