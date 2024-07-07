using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Faml.Api;
using Faml.Syntax;
using TypeTooling;
using TypeTooling.DotNet.CodeGeneration;

namespace Faml.CodeGeneration {
    public class ModuleDelegates {
        private readonly TypeToolingEnvironment _typeToolingEnvironment;
        private readonly Dictionary<Name, FunctionDelegateHolder> _functionDelegateHolders = new Dictionary<Name, FunctionDelegateHolder>();
        private readonly Dictionary<ExampleDefinitionSyntax, Delegate> _exampleDelegates = new Dictionary<ExampleDefinitionSyntax, Delegate>();
        

        public ModuleDelegates(TypeToolingEnvironment typeToolingEnvironment) {
            this._typeToolingEnvironment = typeToolingEnvironment;
        }

        /// <summary>
        /// Create, if needed, and return the function delegate holder for the specified function. This
        /// method also creates function delegates as needed for all functions called, directly or indirectly, from
        /// the given function.
        /// </summary>
        /// <param name="functionDefinition">function definition</param>
        /// <returns>delegate holder</returns>
        public FunctionDelegateHolder GetOrCreateFunctionDelegate(FunctionDefinitionSyntax functionDefinition) {
            Name functionName = functionDefinition.FunctionName;
            if (this._functionDelegateHolders.TryGetValue(functionName, out FunctionDelegateHolder existingDelegateHolder))
                return existingDelegateHolder;

            // First add the delegate holder, so anything that recursively calls back to this function won't try to
            // recreate its delegate. The rule is that if the delegate holder exists, then the delegate exists or is
            // already in the process of getting created.
            var delegateHolder = new FunctionDelegateHolder();
            this._functionDelegateHolders.Add(functionName, delegateHolder);

            var createFunctionCode = new CreateFunctionCode(functionDefinition);
            LambdaExpression lambdaExpression = new ConvertToExpressionTree(this._typeToolingEnvironment, createFunctionCode.Result).Result;
            Delegate functionDelegate = lambdaExpression.Compile();

            delegateHolder.FunctionDelegate = functionDelegate;

            return delegateHolder;
        }

        public void AddExampleDelegate(ExampleDefinitionSyntax exampleDefinition, Delegate expressionDelegate) {
            this._exampleDelegates.Add(exampleDefinition, expressionDelegate);
        }

        public Delegate? GetExampleDelegate(ExampleDefinitionSyntax exampleDefinition) {
            if (!this._exampleDelegates.TryGetValue(exampleDefinition, out Delegate exampleDelegate))
                return null;
            return exampleDelegate;
        }
    }
}
