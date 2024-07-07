using System.Text;
using Faml.Api;
using Faml.CodeAnalysis.Text;
using Faml.Parser;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Faml.Tests.Shared;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#pragma warning disable SA1118 // Parameter should not span multiple lines

namespace Faml.Tests.Parser
{
    [TestClass]
    public sealed class ParserTests : TestBase
    {
        [TestMethod]
        public void TestParseExpression()
        {
            this.AssertExpressionParsingMatches("true || false && (false)");
            this.AssertExpressionParsingMatches("(abc > def) || (ghi != true)");
            //assertExpressionParsingMatches("{ abc:true }");
            //assertExpressionParsingMatches("xxx{abc:def\nxxx:ghi\n}");
            //assertExpressionParsingMatches("foo(1, 2, 3)");
        }

        [TestMethod]
        public void TestParseFunctionWithSimpleBody()
        {
            this.AssertModuleParsingMatches("foo:int = 2");
        }

        [TestMethod]
        public void TestParseFunctionWithExprBody()
        {
            this.AssertModuleParsingMatches("foo{param: int} = { 2 * param }");
        }

        [TestMethod]
        public void TestParseFunctionInvocation()
        {
            this.AssertModuleParsingMatches("foo = TestObject{abcdef}");
            this.AssertModuleParsingMatches("foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc}");
            this.AssertModuleParsingMatches("foo = TestObject{IntProp:3; BoolProp:true; TextProp:abc; abcdef}");
            this.AssertModuleParsingMatches("foo = TestObject{}");
        }

        [TestMethod]
        public void TestParseFunctionInvocationExpressionValue()
        {
            this.AssertModuleParsingMatches("foo =\n" +
                                       "  TestObject{IntProp:{ 3 + 5 }; TextProp:abc}");
        }

        [TestMethod]
        public void TestParseFunctionInvocationMultiLines()
        {
            this.AssertModuleParsingMatches(
                """
                foo =
                  TestObject {
                    IntProp:3
                    TextProp:abc
                  }
                """,
                "foo = TestObject{IntProp:3; TextProp:abc}");
        }

        [TestMethod]
        public void TestParseContainer()
        {
            this.AssertModuleParsingMatches(
                """
                foo =
                  TestContainer {
                    Children:
                      TestObject{IntProp:3}
                      TestObject{IntProp:3}
                  }
                """);
        }

        [TestMethod]
        public void Parse_RecordDefinition()
        {
            this.AssertModuleParsingMatches(
                """
                type Foo = {
                    foo:int
                    bar:int
                    baz:string
                }
                """);
        }

        [TestMethod]
        public void Parse_RecordDefinitionSameLine()
        {
            this.AssertModuleParsingMatches(
                """
                type Foo = {
                    foo:int; bar:int; baz:string
                }
                """,
                """
                type Foo = {
                    foo:int
                    bar:int
                    baz:string
                }
                """);
        }

        [TestMethod]
        public void Parse_RecordDefinitionDefaltValue()
        {
            this.AssertModuleParsingMatches(
                "type Foo = {\n" +
                "    foo:int = 3\n" +
                "    bar:int = 7\n" +
                "}");
        }

        [TestMethod]
        public void TestCustomLiteralFunction()
        {
            this.AssertModuleParsingMatches("foo:CustomLiteralObject = !!!");
            this.AssertModuleParsingMatches("foo{}:CustomLiteralObject = !!!", "foo:CustomLiteralObject = !!!");
            this.AssertModuleParsingMatches("foo = CustomLiteralObject{!!!}");
        }

        [TestMethod]
        public void TestContentProperty()
        {
            this.AssertModuleParsingMatches("foo = string{This is my string}");
            this.AssertModuleParsingMatches("foo = int{3}");
        }

        [TestMethod]
        public void TestErrorRecovery()
        {
            /*
            typeof(Project).Assembly.GetName().Name;
            Assembly.Load().GetEntryAssembly().GetReferencedAssemblies().Select(assembly => Assembly.LoadFrom(assembly.Name)).ToList();

            assertModuleParsingMatches("import luxtest.shared.TestSimpleObj\n" +
                                "foo{} = TestSimpleObj { booleanProp: true  intProp: 3 }");
            */
        }

        private void AssertModuleParsingMatches(string source, string? expectedReconstructedSource = null)
        {
            string sourceWithImport = "import Faml.Tests.TestTypes\n" + source;

            FamlModule mainModule = CreateSingleModuleProgram(sourceWithImport);
            this.AssertParsingMatches(source, expectedReconstructedSource, mainModule.ModuleSyntax);
        }

        private void AssertExpressionParsingMatches(string source, string? expectedReconstructedSource = null)
        {
            FamlProject project = new FamlWorkspace().CreateProject();
            FamlModule module = new FamlModule(project, new QualifiableName("main"), null);
            ExpressionSyntax expression = SourceParser.ParseExpression(module, source);

            this.AssertParsingMatches(source, expectedReconstructedSource, expression);
        }

        private void AssertParsingMatches(string source, string? expectedReconstructedSource, SyntaxNode syntaxNode)
        {
            var sourceWriter = new SourceWriter();
            syntaxNode.WriteSource(sourceWriter);
            string reconstructedSource = sourceWriter.GetSource();

            if (expectedReconstructedSource == null)
            {
                expectedReconstructedSource = source;
            }

            Assert.AreEqual(NormalizeWhitespace(expectedReconstructedSource), NormalizeWhitespace(reconstructedSource));
        }

        /// <summary>
        /// Normalize whitespace, collapsing all runs of whitespace into a single space. Newlines, carriage returns, and
        /// tabs are turned into spaces, which are then consolidated. Unnecessary spaces, before/after certain tokens,
        /// are removed.
        /// </summary>
        /// <param name="input">input string</param>
        /// <remarks> normalized string</remarks>
        private static string NormalizeWhitespace(string input)
        {
            var output = new StringBuilder();

            int length = input.Length;
            for (int i = 0; i < length; i++)
            {
                char c = input[i];
                char next = i + 1 < length ? input[i + 1] : '\0';

                if (char.IsWhiteSpace(c))
                {
                    // Turn other whitespace into a space
                    c = ' ';

                    // Conslidate spaces, just using the last one in a sequence
                    if (char.IsWhiteSpace(next))
                    {
                        continue;
                    }

                    char previous = output.Length > 0 ? output[output.Length - 1] : '\0';

                    // If the previous character doesn't require a space after, don't include one
                    if (previous == '\0' || previous == '{' || previous == ':')
                    {
                        continue;
                    }

                    // If the next character doesn't require a space before, don't include one
                    if (next == '\0' || next == '{' || next == '}')
                    {
                        continue;
                    }
                }

                output.Append(c);
            }

            return output.ToString();
        }
    }
}
