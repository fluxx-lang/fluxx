using TypeTooling.CompanionType;

namespace Faml.Tests.TestTypes
{
    public sealed class TestObject
    {
        private bool _boolProp;
        private int _intProp;
        private string? _textProp;
        private SimpleEnum _enumProp;
        private YesNo? _yesNo;
        private string? _content;

        public TestObject(int intProp, bool boolProp, string textProp)
        {
            this._intProp = intProp;
            this._boolProp = boolProp;
            this._textProp = textProp;
        }

        public bool BoolProp
        {
            get => this._boolProp;
            set => this._boolProp = value;
        }

        public int IntProp
        {
            get => this._intProp;
            set => this._intProp = value;
        }

        public string? TextProp
        {
            get => this._textProp;
            set => this._textProp = value;
        }

        public SimpleEnum EnumProp
        {
            get => this._enumProp;
            set => this._enumProp = value;
        }

        public YesNo? YesNoProp
        {
            get => this._yesNo;
            set => this._yesNo = value;
        }

        public string? ContentProp
        {
            get => this._content;
            set => this._content = value;
        }

        public override string ToString()
        {
            return "TestSimpleObj{" +
                   "intProp=" + this._intProp +
                   ", boolProp=" + this._boolProp +
                   ", textProp='" + this._textProp + '\'' +
                   '}';
        }

        public int AddValue(int toAdd)
        {
            return this._intProp + toAdd;
        }

        public bool IsIntEven()
        {
            return this._intProp % 2 == 0;
        }

        public override bool Equals(object o)
        {
            if (this == o) return true;
            // TEST CONV - fix this
            /*
            if (o == null || getClass() != o.getClass()) return false;
            */

            var that = (TestObject) o;

            if (this._intProp != that._intProp) return false;
            if (this._boolProp != that._boolProp) return false;
            return !(this._textProp != null ? !this._textProp.Equals(that._textProp) : that._textProp != null);
        }

        public override int GetHashCode()
        {
            int result = this._intProp;
            result = 31*result + (this._boolProp ? 1 : 0);
            result = 31*result + (this._textProp != null ? this._textProp.GetHashCode() : 0);
            return result;
        }
    }

    public class TestObjectTypeTooling : IContentPropertyProvider
    {
        public string GetContentProperty()
        {
            return nameof(TestObject.ContentProp);
        }
    }
}
