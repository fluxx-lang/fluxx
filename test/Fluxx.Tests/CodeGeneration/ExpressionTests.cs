using Faml.Tests.Shared;
using NUnit.Framework;


/**
 * @author Bret Johnson
 * @since 3/28/2015
 */

namespace Faml.Tests.CodeGeneration
{
    public class ExpressionTests : TestBase
    {
        [Test] public void TestParenthesized()
        {
            this.AssertExpressionValueIs("(true || false) && (false)", false);
            this.AssertExpressionValueIs("(3 > 4 || (2 > 4))", false);
        }

        [Test] public void TestAndOr()
        {
            this.AssertExpressionValueIs("true && true", true);
            this.AssertExpressionValueIs("true && false", false);
            this.AssertExpressionValueIs("false && true", false);
            this.AssertExpressionValueIs("false && false", false);

            this.AssertExpressionValueIs("true || true", true);
            this.AssertExpressionValueIs("true || false", true);
            this.AssertExpressionValueIs("false || true", true);
            this.AssertExpressionValueIs("false || false", false);
        }

        [Test] public void TestEqualsNotEquals()
        {
            this.AssertExpressionValueIs("1 == 3", false);
            this.AssertExpressionValueIs("1 == 1", true);

            this.AssertExpressionValueIs("true == true", true);
            this.AssertExpressionValueIs("true == false", false);
            this.AssertExpressionValueIs("false == true", false);
            this.AssertExpressionValueIs("false == false", true);

            this.AssertExpressionValueIs("1 != 3", true);
            this.AssertExpressionValueIs("1 != 1", false);

            this.AssertExpressionValueIs("true != true", false);
            this.AssertExpressionValueIs("true != false", true);
            this.AssertExpressionValueIs("false != true", true);
            this.AssertExpressionValueIs("false != false", false);
        }

        [Test] public void TestGreaterThanLessThan()
        {
            this.AssertExpressionValueIs("1 > 3", false);
            this.AssertExpressionValueIs("1 > 1", false);
            this.AssertExpressionValueIs("1 > 0", true);

            this.AssertExpressionValueIs("1 >= 3", false);
            this.AssertExpressionValueIs("1 >= 1", true);
            this.AssertExpressionValueIs("1 >= 0", true);

            this.AssertExpressionValueIs("1 < 3", true);
            this.AssertExpressionValueIs("1 < 1", false);
            this.AssertExpressionValueIs("1 < 0", false);

            this.AssertExpressionValueIs("1 <= 1", true);
            this.AssertExpressionValueIs("1 <= 0", false);
            this.AssertExpressionValueIs("1 <= 3", true);
        }

        [Test] public void TestFunction()
        {
            AssertMainFunctionValueIs(
                "bar:bool = true || false\n" +
                "main:bool = ! bar", false);
            AssertMainFunctionValueIs(
                "bar{}:int = 3\r\n" +
                "main{}:int = bar{}", 3);
            AssertMainFunctionValueIs(
                "bar{val:int}:int = 3\r\n" +
                "main{}:int = bar{val:5}",
                3);
            //assertBooleanFunctionValueIs("function foo(param1:Boolean):Boolean = false", "foo()", false);
        }

        [Test] public void TestFunctionWithParams()
        {
            AssertMainFunctionValueIs(
                "bar{val: int}: int = 3\r\n" +
                "main{}: int = bar{val: 5}",
                3);
            AssertMainFunctionValueIs(
                "bar{val:int}:int = {val}\n" +
                "main{}:int = bar{val:5}",
                5);
            AssertMainFunctionValueIs(
                "bothTrue{val1:bool  val2:bool}: bool = {val1} && {val2}\n" +
                "main{}: bool = bothTrue{val1:true val2:true}",
                true);
        }

        [Test] public void TestTextEscapes()
        {
            AssertMainFunctionValueIs(
                "foo{val: string}: string = {val}\n" +
                "main{}: string = foo{val: ab\\:cdef}",
                "ab:cdef");

            AssertMainFunctionValueIs(
                "foo{val: string}: string = {val}\n" +
                "main{}: string = foo{val: ab\\{cdef}",
                "ab{cdef");

            AssertMainFunctionValueIs(
                "foo{val: string}: string = {val}\n" +
                "main{}: string = foo{val: ab\\}cdef}",
                "ab}cdef");

            AssertMainFunctionValueIs(
                "foo{val: string}: string = {val}\n" +
                "main{}: string = foo{val: ab\\\\cdef}",
                "ab\\cdef");

            AssertMainFunctionValueIs(
                "foo{val: string}: string = {val}\n" +
                "main{}: string = foo{val: abc\\n\\r\\tdef}",
                "abc\n\r\tdef");

            // TODO: Add back
            /*
            assertBooleanFunctionValueIs(
                "bothTrue{val1: Boolean val2: Boolean}: Boolean = val1 && val2\n" +
                "main{}: Boolean = bothTrue{val1:true val2:true}",
                true);
                */
        }

        /*
        @Test public void testDataDefinition() {
            assertIntFunctionValueIs(
                    "data MyRecord {\n" +
                    "    age: Int\n" +
                    "}\n" +
                    "\n" +
                    "main():Int =\n" +
                    "myRecord {\n" +
                    "    age:23\n" +
                    "}.age \n",
                    23);
        }
    */

        [Test] public void TestNewExternalObject()
        {
            this.AssertExpressionValueIs(
                "Faml.Tests.TestTypes.TestObject{ IntProp: 23 }.IntProp\n",
                23);
        }

#if false
        [Test] public void TestObjectInstantiation()
        {
            TestContainer expectedTestContainer = new TestContainer();
            expectedTestContainer.Add(new TestSimpleObj(23, false, null));
            expectedTestContainer.Add(new TestSimpleObj(24, true, "my text"));

            AssertObjectFunctionValueIs(
                "import luxtest.shared.TestSimpleObj\n" +
                "import luxtest.shared.TestContainer\n" +
                "\n" +
                "main{}: TestContainer = TestContainer {\n" +
                "  testSimpleObj {\n" +
                "    intProp: 23\n" +
                "  }\n" +
                "  testSimpleObj {\n" +
                "    intProp: 24\n" +
                "    booleanProp: true\n" +
                "    textProp: my text\n" +
                "    enumProp: EnumValue1\n" +
                "  }\n" +
                "}\n",
                expectedTestContainer);
        }

        [Test] public void TestEnumProperty()
        {
            TestSimpleObj expectedSimpleObj = new TestSimpleObj();
            expectedSimpleObj.EnumProp = TestEnum.EnumValue1;

            AssertObjectFunctionValueIs(
                "import luxtest.shared.TestSimpleObj\n" +
                "\n" +
                "main{}: TestSimpleObj = TestSimpleObj {\n" +
                "    enumProp: EnumValue1\n" +
                "}\n",
                expectedSimpleObj);
        }

        [Test] public void TestTypeConverterProperty()
        {
            TestSimpleObj expectedSimpleObj = new TestSimpleObj();
            expectedSimpleObj.YesNoProp = new YesNo(true);

            AssertObjectFunctionValueIs(
                "import luxtest.shared.TestSimpleObj\n" +
                "\n" +
                "main{}: TestSimpleObj = TestSimpleObj {\n" +
                "    yesNoProp: yes\n" +
                "}\n",
                expectedSimpleObj);
        }
#endif

        [Test] public void TestMaxFunction()
        {
            AssertMainFunctionValueIs(
                "max{x:int  y:int}: int = {x}\n" +
                "main{}: int = max{x:3  y:4}",
                3);
        }

        [Test] public void TestStringFunction()
        {
            AssertMainFunctionValueIs(
                "foo{x:string}: string = {x}\n" +
                "main{}: string = foo{ x:abc }",
                "abc");
        }

#if LATER
        [Test] public void TestMethodInvocation()
        {
            AssertMainFunctionValueIs(
                "import Faml.Tests.Shared.TestObjects\n" +
                "foo:TestObject = TestObject{IntProp: 3}\n" +
                "main{}: int = foo.AddValue{toAdd: 4}",
                7);

#if false
            AssertMainFunctionValueIs(
                "import Faml.Tests.Shared.TestObjects\n" +
                "main{}: bool = TestObject{intProp: 3}.isIntEven{}",
                false);

            AssertMainFunctionValueIs(
                "import Faml.Tests.Shared.TestObjects\n" +
                "main{}: bool = TestObject{intProp: 4}.isIntEven{}",
                true);
#endif
        }
#endif
    }
}
