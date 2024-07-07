namespace Fluxx.Tests.TestTypes
{
    public class YesNo
    {
        private readonly bool isYes;

        public YesNo(bool isYes)
        {
            this.isYes = isYes;
        }

        public bool Equals(YesNo other)
        {
            return this.isYes == other.isYes;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((YesNo)obj);
        }

        public override int GetHashCode()
        {
            return this.isYes.GetHashCode();
        }
    }
}
