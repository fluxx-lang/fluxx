using TypeTooling.Types;

namespace TypeTooling
{
    public abstract class TypeToolingEnhancer
    {
        public TypeToolingEnvironment TypeToolingEnvironment { get;  }

        protected TypeToolingEnhancer(TypeToolingEnvironment typeToolingEnvironment)
        {
            this.TypeToolingEnvironment = typeToolingEnvironment;
        }

        public abstract TypeToolingType EnhanceType(TypeToolingType type);

        public abstract AttachedType EnhanceAttachedType(AttachedType typeToolingType);
    }
}