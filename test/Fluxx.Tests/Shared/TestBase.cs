using System;
using System.Linq;
using Faml.Api;
using Faml.DotNet;
using Faml.Interpreter;
using Faml.Parser;
using Faml.SourceProviders;
using Faml.Syntax;
using Faml.Syntax.Expression;
using NUnit.Framework;
using TypeTooling.DotNet.CodeGeneration;


/**
 * @author Bret Johnson
 * @since 3/28/2015
 */

namespace Faml.Tests.Shared
{
    public class TestBase {
        public void AssertMainFunctionValueIs(string moduleSource, object expectedValue) {
            object actualValue = this.EvaluateFunction(moduleSource, "main");
            Assert.AreEqual(actualValue, (object)expectedValue);
        }

        public void AssertExpressionValueIs(string expressionSource, object expectedValue) {
            object actualValue = EvaluateFunction("fn = " + expressionSource, "fn");
            Assert.AreEqual(expectedValue, actualValue);
        }

        public object EvaluateFunction(string moduleSource, string functionName) {
            FamlModule mainModule = CreateSingleModuleProgram(moduleSource);

            FunctionDefinitionSyntax functionDefinition = mainModule.ModuleSyntax.GetFunctionDefinition(new Name(functionName));
            if (functionDefinition == null)
                throw new InvalidOperationException($"Function '{functionName}' unexpectedly doesn't exist");

            FunctionDelegateHolder delegateHolder = mainModule.ModuleDelegates.GetOrCreateFunctionDelegate(functionDefinition);
            return delegateHolder.FunctionDelegate.DynamicInvoke();
        }

        public static FamlModule CreateSingleModuleProgram(string source) {
            var workspace = new FamlWorkspace();

            var sourceProvider = new InMemorySourceProvider();
            sourceProvider.AddTextResource("main.faml", source);

            FamlProject project = workspace.CreateProject();
            project.DotNetProjectInfo.RawTypeProvider = new AppDotNetRawTypeProvider(typeof(TestBase).Assembly);
            project.FullyInitialize();

            project.UpdateSource("main.faml", source);

            //project.AddLibrary(new DotNetExternalLibrary(project, "lux-test"));
            //project.DotNetProjectInfo.AssemblyLoader = new FamlTestAssemblyLoader();

            FamlModule mainModule = project.GetModule(new QualifiableName("main"));
            if (project.AnyErrors)
                throw new Exception($"Project has errors, the first being: {project.GetAllDiagnostics().First().Message}");

            return mainModule;
        }

        public static Eval CreateMainFunctionInvocationEval(FamlModule mainModule) {
            return mainModule.Project.CreateFunctionInvocationEval(new QualifiableName("main.main"), new Args());
        }
    }
}
