using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace Faml.Binding.External
{
    public class ExternalSequenceTypeBinding : SequenceTypeBinding
    {
        private readonly FamlProject _project;
        private readonly SequenceType _typeToolingType;
        public readonly DotNetRawType _dotNetType;


        public ExternalSequenceTypeBinding(FamlProject project, SequenceType typeToolingType, TypeBinding elementTypeBinding) : base(elementTypeBinding)
        {
            this._project = project;
            this._typeToolingType = typeToolingType;

            this._dotNetType = (DotNetRawType) typeToolingType.UnderlyingType;
        }

        public FamlProject Project => this._project;

        public SequenceType TypeToolingType => this._typeToolingType;

        protected bool Equals(ExternalSequenceTypeBinding other)
        {
            return this._dotNetType.Equals(other._dotNetType);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExternalSequenceTypeBinding))
            {
                return false;
            }

            return this.Equals((ExternalSequenceTypeBinding) obj);
        }

        public override int GetHashCode()
        {
            return this._dotNetType.GetHashCode();
        }

        public override bool IsAssignableFrom(TypeBinding other)
        {
            if (other is ExternalSequenceTypeBinding otherSequenceTypeBinding)
            {
                return this._dotNetType.IsAssignableFrom(otherSequenceTypeBinding._dotNetType);
            }
            else
            {
                return base.IsAssignableFrom(other);
            }
        }
    }
}
