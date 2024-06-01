using System.Collections.Generic;

namespace Faml.Tests.TestTypes
{
    [ContentProperty("Children")]
    public sealed class TestContainer
    {
        public List<TestObject> Children { get; } = new List<TestObject>();

        public TestContainer()
        {
        }

        public override string ToString()
        {
            return "TestContainer{" +
                   "Children=" + Children +
                   '}';
        }
    }
}

