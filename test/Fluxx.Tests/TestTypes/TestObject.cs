using TypeTooling.CompanionType;

namespace Fluxx.Tests.TestTypes
{
    public sealed class TestObject
    {
        private bool boolProp;
        private int intProp;
        private string? textProp;
        private SimpleEnum enumProp;
        private YesNo? yesNo;
        private string? content;

        public TestObject(int intProp, bool boolProp, string textProp)
        {
            this.intProp = intProp;
            this.boolProp = boolProp;
            this.textProp = textProp;
        }

        public bool BoolProp
        {
            get => this.boolProp;
            set => this.boolProp = value;
        }

        public int IntProp
        {
            get => this.intProp;
            set => this.intProp = value;
        }

        public string? TextProp
        {
            get => this.textProp;
            set => this.textProp = value;
        }

        public SimpleEnum EnumProp
        {
            get => this.enumProp;
            set => this.enumProp = value;
        }

        public YesNo? YesNoProp
        {
            get => this.yesNo;
            set => this.yesNo = value;
        }

        public string? ContentProp
        {
            get => this.content;
            set => this.content = value;
        }

        public override string ToString()
        {
            return "TestSimpleObj{" +
                   "intProp=" + this.intProp +
                   ", boolProp=" + this.boolProp +
                   ", textProp='" + this.textProp + '\'' +
                   '}';
        }

        public int AddValue(int toAdd)
        {
            return this.intProp + toAdd;
        }

        public bool IsIntEven()
        {
            return this.intProp % 2 == 0;
        }

        public override bool Equals(object? o)
        {
            if (this == o)
            {
                return true;
            }

            // TEST CONV - fix this
            /*
            if (o == null || getClass() != o.getClass()) return false;
            */

            var that = (TestObject)o;

            if (this.intProp != that.intProp)
            {
                return false;
            }

            if (this.boolProp != that.boolProp)
            {
                return false;
            }

            return !(this.textProp != null ? !this.textProp.Equals(that.textProp) : that.textProp != null);
        }

        public override int GetHashCode()
        {
            int result = this.intProp;
            result = (31 * result) + (this.boolProp ? 1 : 0);
            result = (31 * result) + (this.textProp != null ? this.textProp.GetHashCode() : 0);
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
