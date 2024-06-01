using System.Text;
using Faml.Api;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Faml.Tests.Shared;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Faml.Tests.Parser {
    public sealed class ParserTests : TestBase {
        [Test]
        public void TestParseExpression() {
            AssertExpressionParsingMatches("true || false && (false)");
            AssertExpressionParsingMatches("(abc > def) || (ghi != true)");
            //assertExpressionParsingMatches("{ abc:true }");
            //assertExpressionParsingMatches("xxx{abc:def\nxxx:ghi\n}");
            //assertExpressionParsingMatches("foo(1, 2, 3)");
        }

        [Test]
        public void TestParseFunctionWithSimpleBody() {
            AssertModuleParsingMatches("foo:int = 2");
        }

        [Test]
        public void TestParseFunctionWithExprBody() {
            AssertModuleParsingMatches("foo{param: int} = { 2 * param }");
        }

        [Test]
        public void TestParseFunctionInvocation() {
            AssertModuleParsingMatches("foo = TestObject{abcdef}");
            AssertModuleParsingMatches("foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc}");
            AssertModuleParsingMatches("foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc; abcdef}");
            AssertModuleParsingMatches("foo = TestObject{}");
        }

        [Test]
        public void TestParseFunctionInvocationExpressionValue() {
            AssertModuleParsingMatches("foo =\n" +
                                       "  TestObject{IntProp:{ 3 + 5 }; TextProp:abc}");
        }

        [Test] public void TestParseFunctionInvocationMultiLines() {
            AssertModuleParsingMatches("foo =\n" +
                                       "  TestObject {\n" +
                                       "    IntProp:3\n" +
                                       "    TextProp:abc\n" +
                                       "  }",
                                       "foo = TestObject{IntProp:3; TextProp:abc}");
        }

        [Test]
        public void TestParseContainer() {
            AssertModuleParsingMatches("foo =\n" +
                                       "  TestContainer {\n" +
                                       "    Children:\n" +
                                       "      TestObject{IntProp:3}\n" +
                                       "      TestObject{IntProp:3}\n" +
                                       "  }");
        }

        [Test]
        public void Parse_RecordDefinition() {
            AssertModuleParsingMatches("type Foo = {\n" +
                                       "    foo:int\n" +
                                       "    bar:int\n" +
                                       "    baz:string\n" +
                                       "}");
        }

        [Test]
        public void Parse_RecordDefinitionSameLine() {
            AssertModuleParsingMatches("type Foo = {\n" +
                                       "    foo:int; bar:int; baz:string\n" +
                                       "}",
                                       "type Foo = {\n" +
                                       "    foo:int" +
                                       "    bar:int\n" +
                                       "    baz:string" +
                                       "}");
        }

        [Test]
        public void Parse_RecordDefinitionDefaltValue() {
            AssertModuleParsingMatches("type Foo = {\n" +
                                       "    foo:int = 3\n" +
                                       "    bar:int = 7\n" +
                                       "}");
        }

        [Test] public void TestCustomLiteralFunction() {
            AssertModuleParsingMatches("foo:CustomLiteralObject = !!!");
            AssertModuleParsingMatches("foo{}:CustomLiteralObject = !!!", "foo:CustomLiteralObject = !!!");
            AssertModuleParsingMatches("foo = CustomLiteralObject{!!!}");
        }

        [Test]
        public void TestContentProperty() {
            AssertModuleParsingMatches("foo = string{This is my string}");
            AssertModuleParsingMatches("foo = int{3}");
        }

        [Test] public void TestErrorRecovery() {
            /*
            typeof(Project).Assembly.GetName().Name;
            Assembly.Load().GetEntryAssembly().GetReferencedAssemblies().Select(assembly => Assembly.LoadFrom(assembly.Name)).ToList();

            assertModuleParsingMatches("import luxtest.shared.TestSimpleObj\n" +
                                "foo{} = TestSimpleObj { booleanProp: true  intProp: 3 }");
            */
        }

        private void AssertModuleParsingMatches(string source, string? expectedReconstructedSource = null) {
            string sourceWithImport = "import Faml.Tests.TestTypes\n" + source;

            FamlModule mainModule = CreateSingleModuleProgram(sourceWithImport);
            AssertParsingMatches(source, expectedReconstructedSource, mainModule.ModuleSyntax);
        }

        private void AssertExpressionParsingMatches(string source, string? expectedReconstructedSource = null) {
            FamlProject project = new FamlWorkspace().CreateProject();
            FamlModule module = new FamlModule(project, new QualifiableName("main"), null);
            ExpressionSyntax expression = SourceParser.ParseExpression(module, source);

            AssertParsingMatches(source, expectedReconstructedSource, expression);
        }

        private void AssertParsingMatches(string source, string? expectedReconstructedSource, SyntaxNode syntaxNode) {
            var sourceWriter = new SourceWriter();
            syntaxNode.WriteSource(sourceWriter);
            string reconstructedSource = sourceWriter.GetSource();

            if (expectedReconstructedSource == null)
                expectedReconstructedSource = source;

            Assert.AreEqual(NormalizeWhitespace(expectedReconstructedSource), NormalizeWhitespace(reconstructedSource));
        }

        /// <summary>
        /// Normalize whitespace, collapsing all runs of whitespace into a single space. Newlines, carriage returns, and
        /// tabs are turned into spaces, which are then consolidated. Unnecessary spaces, before/after certain tokens,
        /// are removed.
        /// </summary>
        /// <param name="input">input string</param>
        /// <remarks> normalized string</remarks>
        private static string NormalizeWhitespace(string input) {
            var output = new StringBuilder();

            int length = input.Length;
            for (int i = 0; i < length; i++) {
                char c = input[i];
                char next = i + 1 < length ? input[i + 1] : '\0';

                if (char.IsWhiteSpace(c)) {
                    // Turn other whitespace into a space
                    c = ' ';

                    // Conslidate spaces, just using the last one in a sequence
                    if (char.IsWhiteSpace(next))
                        continue;

                    char previous = output.Length > 0 ? output[output.Length - 1] : '\0';

                    // If the previous character doesn't require a space after, don't include one
                    if (previous == '\0' || previous == '{' || previous == ':')
                        continue;

                    // If the next character doesn't require a space before, don't include one
                    if (next == '\0' || next == '{' || next == '}')
                        continue;
                }

                output.Append(c);
            }

            return output.ToString();
        }
    }
}
