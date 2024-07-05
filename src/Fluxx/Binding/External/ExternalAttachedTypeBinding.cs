using Faml.Api;
using TypeTooling.Types;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Faml.Binding.External {
    public class ExternalAttachedTypeBinding : AttachedTypeBinding {
        private readonly FamlProject _project;
        private readonly AttachedType _attachedType;


        public ExternalAttachedTypeBinding(FamlProject project, AttachedType attachedType) : base(new QualifiableName(attachedType.FullName)) {
            _project = project;
            _attachedType = attachedType;
        }

        public FamlProject Project => _project;

        public AttachedType AttachedType => _attachedType;

        public AttachedProperty GetAttachedProperty(Name propertyName)
        {
            foreach (AttachedProperty attachedProperty in _attachedType.AttachedProperties)
                if (attachedProperty.Name == propertyName.ToString())
                    return attachedProperty;

            return null;
        }
    }
}
