namespace Faml.Tests.TestTypes
{
    public class YesNo {
        private readonly bool _isYes;

        public YesNo(bool isYes) {
            _isYes = isYes;
        }

        protected bool Equals(YesNo other) {
            return _isYes == other._isYes;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((YesNo) obj);
        }

        public override int GetHashCode() {
            return _isYes.GetHashCode();
        }
    }
}
