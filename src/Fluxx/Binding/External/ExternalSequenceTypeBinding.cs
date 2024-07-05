using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Faml.Binding.External {
    public class ExternalSequenceTypeBinding : SequenceTypeBinding {
        private readonly FamlProject _project;
        private readonly SequenceType _typeToolingType;
        public readonly DotNetRawType _dotNetType;


        public ExternalSequenceTypeBinding(FamlProject project, SequenceType typeToolingType, TypeBinding elementTypeBinding) : base(elementTypeBinding) {
            _project = project;
            _typeToolingType = typeToolingType;

            _dotNetType = (DotNetRawType) typeToolingType.UnderlyingType;
        }

        public FamlProject Project => _project;

        public SequenceType TypeToolingType => _typeToolingType;

        protected bool Equals(ExternalSequenceTypeBinding other) {
            return _dotNetType.Equals(other._dotNetType);
        }

        public override bool Equals(object obj) {
            if (!(obj is ExternalSequenceTypeBinding))
                return false;
            return Equals((ExternalSequenceTypeBinding) obj);
        }

        public override int GetHashCode() {
            return _dotNetType.GetHashCode();
        }

        public override bool IsAssignableFrom(TypeBinding other) {
            if (other is ExternalSequenceTypeBinding otherSequenceTypeBinding)
                return _dotNetType.IsAssignableFrom(otherSequenceTypeBinding._dotNetType);
            else return base.IsAssignableFrom(other);
        }
    }
}
