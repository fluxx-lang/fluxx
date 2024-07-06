using TypeTooling.CompanionType;

namespace Faml.Tests.TestTypes {
    public sealed class TestObject {
        private bool _boolProp;
        private int _intProp;
        private string? _textProp;
        private SimpleEnum _enumProp;
        private YesNo? _yesNo;
        private string? _content;

        public TestObject(int intProp, bool boolProp, string textProp) {
            _intProp = intProp;
            _boolProp = boolProp;
            _textProp = textProp;
        }

        public bool BoolProp {
            get => _boolProp;
            set => _boolProp = value;
        }

        public int IntProp {
            get => _intProp;
            set => _intProp = value;
        }

        public string? TextProp {
            get => _textProp;
            set => _textProp = value;
        }

        public SimpleEnum EnumProp
        {
            get => _enumProp;
            set => _enumProp = value;
        }

        public YesNo? YesNoProp
        {
            get => _yesNo;
            set => _yesNo = value;
        }

        public string? ContentProp {
            get => _content;
            set => _content = value;
        }

        public override string ToString() {
            return "TestSimpleObj{" +
                   "intProp=" + _intProp +
                   ", boolProp=" + _boolProp +
                   ", textProp='" + _textProp + '\'' +
                   '}';
        }

        public int AddValue(int toAdd) {
            return _intProp + toAdd;
        }

        public bool IsIntEven() {
            return _intProp % 2 == 0;
        }

        public override bool Equals(object o) {
            if (this == o) return true;
            // TEST CONV - fix this
            /*
            if (o == null || getClass() != o.getClass()) return false;
            */

            var that = (TestObject) o;

            if (_intProp != that._intProp) return false;
            if (_boolProp != that._boolProp) return false;
            return !(_textProp != null ? !_textProp.Equals(that._textProp) : that._textProp != null);
        }

        public override int GetHashCode() {
            int result = _intProp;
            result = 31*result + (_boolProp ? 1 : 0);
            result = 31*result + (_textProp != null ? _textProp.GetHashCode() : 0);
            return result;
        }
    }

    public class TestObjectTypeTooling : IContentPropertyProvider {
        public string GetContentProperty() {
            return nameof(TestObject.ContentProp);
        }
    }
}
