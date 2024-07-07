using Faml.Api;
using Faml.Syntax;
using Faml.Tests.Shared;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Faml.Tests.Tagger
{
    public sealed class SyntaxHighlightTagTests : TestBase
    {
        private const SyntaxHighlightTagType LeftBrace = SyntaxHighlightTagType.DeemphasizedPunctuation;
        private const SyntaxHighlightTagType RightBrace = SyntaxHighlightTagType.DeemphasizedPunctuation;
        private const SyntaxHighlightTagType Assign = SyntaxHighlightTagType.Punctuation;
        private const SyntaxHighlightTagType Colon = SyntaxHighlightTagType.DeemphasizedPunctuation;
        private const SyntaxHighlightTagType Semicolon = SyntaxHighlightTagType.DeemphasizedPunctuation;
        private const SyntaxHighlightTagType Times = SyntaxHighlightTagType.Operator;
        private const SyntaxHighlightTagType TypeReference = SyntaxHighlightTagType.TypeReference;
        private const SyntaxHighlightTagType Int = SyntaxHighlightTagType.TypeReference;
        private const SyntaxHighlightTagType FunctionReference = SyntaxHighlightTagType.FunctionReference;
        private const SyntaxHighlightTagType PropertyReference = SyntaxHighlightTagType.PropertyReference;
        private const SyntaxHighlightTagType True = SyntaxHighlightTagType.Keyword;
        private const SyntaxHighlightTagType False = SyntaxHighlightTagType.Keyword;
        private const SyntaxHighlightTagType Type = SyntaxHighlightTagType.ControlKeyword;
        private const SyntaxHighlightTagType SymbolReference = SyntaxHighlightTagType.SymbolReference;
        private const SyntaxHighlightTagType NumberLiteral = SyntaxHighlightTagType.NumberLiteral;
        private const SyntaxHighlightTagType StringLiteral = SyntaxHighlightTagType.StringLiteral;

        [TestMethod]
        public void Tag_FunctionWithSimpleBody()
        {
            this.AssertTaggingMatches("foo:int = 2", FunctionReference, Colon, TypeReference, Assign, NumberLiteral);
        }

        [TestMethod]
        public void Tag_FunctionWithExprBody()
        {
            this.AssertTaggingMatches(
                "foo{param: int} = { 2 * param }",
                FunctionReference, LeftBrace, PropertyReference, Colon, Int, RightBrace, Assign, // foo{param: int} =
                LeftBrace, NumberLiteral, Times, SymbolReference, RightBrace); // { 2 * param }
        }

        [TestMethod]
        public void Tag_FunctionInvocationContentArg()
        {
            this.AssertTaggingMatches(
                "foo = TestObject{abcdef}",
                FunctionReference, Assign, FunctionReference, LeftBrace, StringLiteral, RightBrace);
        }

        [TestMethod]
        public void Tag_FunctionInvocationMultipleArgs()
        {
            this.AssertTaggingMatches(
                "foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc}",
                FunctionReference, Assign, FunctionReference, LeftBrace, // foo = TestObject{
                PropertyReference, Colon, NumberLiteral, Semicolon, // IntProp:3;
                PropertyReference, Colon, True, Semicolon, // BoolProp:true;
                PropertyReference, Colon, StringLiteral, // TextProp:abc
                RightBrace); // }
        }

        [TestMethod]
        public void Tag_KeywordsInPropertyValue()
        {
            this.AssertTaggingMatches(
                "foo = TestObject{TextProp:abc if is}",
                FunctionReference, Assign, FunctionReference, LeftBrace, // foo = TestObject{
                PropertyReference, Colon, StringLiteral, RightBrace); // TextProp:abc if is}
        }

        [TestMethod]
        public void Tag_RecordDefinition()
        {
            this.AssertTaggingMatches(
                "type Foo = {foo:int}",
                Type, TypeReference, Assign, LeftBrace, PropertyReference, Colon, Int, RightBrace);
        }

#if LATER
        [Test]
        public void TestParseFunctionInvocationExpressionValue() {
            AssertTaggingMatches("foo =\n" +
                                       "  TestObject{IntProp:{ 3 + 5 }; TextProp:abc}");
        }

        [Test] public void TestParseFunctionInvocationMultiLines() {
            AssertTaggingMatches("foo =\n" +
                                       "  TestObject {\n" +
                                       "    IntProp:3\n" +
                                       "    TextProp:abc\n" +
                                       "  }",
                                       "foo = TestObject{IntProp:3; TextProp:abc}");
        }

        [Test]
        public void TestParseContainer() {
            AssertTaggingMatches("foo =\n" +
                                       "  TestContainer {\n" +
                                       "    Children:\n" +
                                       "      TestObject{IntProp:3}\n" +
                                       "      TestObject{IntProp:3}\n" +
                                       "  }");
        }

        [Test] public void TestParseRecordDefinition() {
            AssertTaggingMatches("data Foo {\n" +
                                       "    foo: int\n" +
                                       "}");
        }

        [Test] public void TestCustomLiteralFunction() {
            AssertTaggingMatches("foo:CustomLiteralObject = !!!");
            AssertTaggingMatches("foo{}:CustomLiteralObject = !!!", "foo:CustomLiteralObject = !!!");
            AssertTaggingMatches("foo = CustomLiteralObject{!!!}");
        }

        [Test]
        public void TestContentProperty() {
            AssertTaggingMatches("foo = string{This is my string}");
            AssertTaggingMatches("foo = int{3}");
        }

        [Test] public void TestErrorRecovery() {
            /*
            typeof(Project).Assembly.GetName().Name;
            Assembly.Load().GetEntryAssembly().GetReferencedAssemblies().Select(assembly => Assembly.LoadFrom(assembly.Name)).ToList();

            AssertTaggingMatches("import luxtest.shared.TestSimpleObj\n" +
                                "foo{} = TestSimpleObj { booleanProp: true  intProp: 3 }");
            */
        }
#endif

        private void AssertTaggingMatches(string source, params SyntaxHighlightTagType[] expectedTagTypes)
        {
            string sourceWithImport = "import Faml.Tests.TestTypes\n" + source;

            FamlModule module = CreateSingleModuleProgram(sourceWithImport);

            SyntaxNode? firstModuleItem = module.ModuleSyntax.ModuleItems.FirstOrDefault();
            if (firstModuleItem == null)
            {
                Assert.AreEqual(expectedTagTypes.Length, 0, "With no module items, there shouldn't be any tags");
                return;
            }

            int start = firstModuleItem.Span.Start;
            int end = module.ModuleSyntax.Span.End;
            var textSpan = new TextSpan(start, end - start);

            List<SyntaxHighlightTagType>? actualTagTypes = module.GetSyntaxHighlightTags(new TextSpan[] { textSpan })
                .Select(tag => tag.SyntaxHighlightTagType)
                .ToList();

            Assert.AreEqual<IEnumerable<SyntaxHighlightTagType>>(expectedTagTypes, actualTagTypes);
        }
    }
}
