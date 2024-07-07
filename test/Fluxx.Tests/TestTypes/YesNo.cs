namespace Faml.Tests.TestTypes
{
    public class YesNo
    {
        private readonly bool _isYes;

        public YesNo(bool isYes)
        {
            this._isYes = isYes;
        }

        protected bool Equals(YesNo other)
        {
            return this._isYes == other._isYes;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return this.Equals((YesNo) obj);
        }

        public override int GetHashCode()
        {
            return this._isYes.GetHashCode();
        }
    }
}
