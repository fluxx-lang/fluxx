using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace Faml.Binding.External
{
    public class ExternalSequenceTypeBinding : SequenceTypeBinding
    {
        private readonly FamlProject project;
        private readonly SequenceType typeToolingType;
        public readonly DotNetRawType dotNetType;


        public ExternalSequenceTypeBinding(FamlProject project, SequenceType typeToolingType, TypeBinding elementTypeBinding) : base(elementTypeBinding)
        {
            this.project = project;
            this.typeToolingType = typeToolingType;

            this.dotNetType = (DotNetRawType)typeToolingType.UnderlyingType;
        }

        public FamlProject Project => this.project;

        public SequenceType TypeToolingType => this.typeToolingType;

        protected bool Equals(ExternalSequenceTypeBinding other)
        {
            return this.dotNetType.Equals(other.dotNetType);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExternalSequenceTypeBinding))
            {
                return false;
            }

            return this.Equals((ExternalSequenceTypeBinding)obj);
        }

        public override int GetHashCode()
        {
            return this.dotNetType.GetHashCode();
        }

        public override bool IsAssignableFrom(TypeBinding other)
        {
            if (other is ExternalSequenceTypeBinding otherSequenceTypeBinding)
            {
                return this.dotNetType.IsAssignableFrom(otherSequenceTypeBinding.dotNetType);
            }
            else
            {
                return base.IsAssignableFrom(other);
            }
        }
    }
}
