using System;
using System.Linq;
using Fluxx.Api;
using Fluxx.DotNet;
using Fluxx.Interpreter;
using Fluxx.Parser;
using Fluxx.SourceProviders;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeTooling.DotNet.CodeGeneration;

namespace Fluxx.Tests.Shared
{
    [TestClass]
    public class TestBase
    {
        public void AssertMainFunctionValueIs(string moduleSource, object expectedValue)
        {
            object? actualValue = this.EvaluateFunction(moduleSource, "main");
            Assert.AreEqual(actualValue, (object)expectedValue);
        }

        public void AssertExpressionValueIs(string expressionSource, object expectedValue)
        {
            object? actualValue = this.EvaluateFunction("fn = " + expressionSource, "fn");
            Assert.AreEqual(expectedValue, actualValue);
        }

        public object? EvaluateFunction(string moduleSource, string functionName)
        {
            FamlModule mainModule = CreateSingleModuleProgram(moduleSource);

            FunctionDefinitionSyntax? functionDefinition = mainModule.ModuleSyntax.GetFunctionDefinition(new Name(functionName));
            if (functionDefinition == null)
            {
                throw new InvalidOperationException($"Function '{functionName}' unexpectedly doesn't exist");
            }

            FunctionDelegateHolder delegateHolder = mainModule.ModuleDelegates.GetOrCreateFunctionDelegate(functionDefinition);
            return delegateHolder.FunctionDelegate!.DynamicInvoke();
        }

        public static FamlModule CreateSingleModuleProgram(string source)
        {
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
            {
                throw new Exception($"Project has errors, the first being: {project.GetAllDiagnostics().First().Message}");
            }

            return mainModule;
        }

        public static Eval CreateMainFunctionInvocationEval(FamlModule mainModule)
        {
            return mainModule.Project.CreateFunctionInvocationEval(new QualifiableName("main.main"), new Args());
        }
    }
}
