using Faml.Api;
using Faml.Syntax;
using Faml.Tests.Shared;
using Microsoft.CodeAnalysisP.Text;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Faml.Tests.Tagger {
    public sealed class SyntaxHighlightTagTests : TestBase {
        const SyntaxHighlightTagType LeftBrace = SyntaxHighlightTagType.DeemphasizedPunctuation;
        const SyntaxHighlightTagType RightBrace = SyntaxHighlightTagType.DeemphasizedPunctuation;
        const SyntaxHighlightTagType Assign = SyntaxHighlightTagType.Punctuation;
        const SyntaxHighlightTagType Colon = SyntaxHighlightTagType.DeemphasizedPunctuation;
        const SyntaxHighlightTagType Semicolon = SyntaxHighlightTagType.DeemphasizedPunctuation;
        const SyntaxHighlightTagType Times = SyntaxHighlightTagType.Operator;
        const SyntaxHighlightTagType TypeReference = SyntaxHighlightTagType.TypeReference;
        const SyntaxHighlightTagType Int = SyntaxHighlightTagType.TypeReference;
        const SyntaxHighlightTagType FunctionReference = SyntaxHighlightTagType.FunctionReference;
        const SyntaxHighlightTagType PropertyReference = SyntaxHighlightTagType.PropertyReference;
        const SyntaxHighlightTagType True = SyntaxHighlightTagType.Keyword;
        const SyntaxHighlightTagType False = SyntaxHighlightTagType.Keyword;
        const SyntaxHighlightTagType Type = SyntaxHighlightTagType.ControlKeyword;
        const SyntaxHighlightTagType SymbolReference = SyntaxHighlightTagType.SymbolReference;
        const SyntaxHighlightTagType NumberLiteral = SyntaxHighlightTagType.NumberLiteral;
        const SyntaxHighlightTagType StringLiteral = SyntaxHighlightTagType.StringLiteral;

        [Test]
        public void Tag_FunctionWithSimpleBody() {
            AssertTaggingMatches("foo:int = 2",
                FunctionReference, Colon, TypeReference, Assign, NumberLiteral);
        }

        [Test]
        public void Tag_FunctionWithExprBody() {
            AssertTaggingMatches("foo{param: int} = { 2 * param }",
                FunctionReference, LeftBrace, PropertyReference, Colon, Int, RightBrace, Assign, // foo{param: int} =
                LeftBrace, NumberLiteral, Times, SymbolReference, RightBrace); // { 2 * param }
        }

        [Test]
        public void Tag_FunctionInvocationContentArg() {
            AssertTaggingMatches("foo = TestObject{abcdef}",
                FunctionReference, Assign, FunctionReference, LeftBrace, StringLiteral, RightBrace);
        }

        [Test]
        public void Tag_FunctionInvocationMultipleArgs() {
            AssertTaggingMatches("foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc}",
                FunctionReference, Assign, FunctionReference, LeftBrace, // foo = TestObject{
                PropertyReference, Colon, NumberLiteral, Semicolon, // IntProp:3;
                PropertyReference, Colon, True, Semicolon, // BoolProp:true;
                PropertyReference, Colon, StringLiteral, // TextProp:abc
                RightBrace); // }
        }

        [Test]
        public void Tag_KeywordsInPropertyValue() {
            AssertTaggingMatches("foo = TestObject{TextProp:abc if is}",
                FunctionReference, Assign, FunctionReference, LeftBrace, // foo = TestObject{
                PropertyReference, Colon, StringLiteral, RightBrace); // TextProp:abc if is}
        }

        [Test]
        public void Tag_RecordDefinition() {
            AssertTaggingMatches("type Foo = {foo:int}",
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

        private void AssertTaggingMatches(string source, params SyntaxHighlightTagType[] expectedTagTypes) {
            string sourceWithImport = "import Faml.Tests.TestTypes\n" + source;

            FamlModule module = CreateSingleModuleProgram(sourceWithImport);

            SyntaxNode? firstModuleItem = module.ModuleSyntax.ModuleItems.FirstOrDefault();
            if (firstModuleItem == null) {
                Assert.AreEqual(expectedTagTypes.Length, 0, "With no module items, there shouldn't be any tags");
                return;
            }

            int start = firstModuleItem.Span.Start;
            int end = module.ModuleSyntax.Span.End;
            var textSpan = new TextSpan(start, end - start);

            List<SyntaxHighlightTagType>? actualTagTypes = module.GetSyntaxHighlightTags(new TextSpan[] { textSpan })
                .Select(tag => tag.SyntaxHighlightTagType)
                .ToList();

            Assert.AreEqual(expectedTagTypes, actualTagTypes);
        }
    }
}
