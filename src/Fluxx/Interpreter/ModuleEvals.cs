using System.Collections.Generic;
using Fluxx.Syntax;

namespace Fluxx.Interpreter
{
    public class ModuleEvals
    {
        private readonly Dictionary<FunctionDefinitionSyntax, Eval> functionDefinitionEvals = new Dictionary<FunctionDefinitionSyntax, Eval>();
        private readonly Dictionary<ExampleDefinitionSyntax, ObjectEval> exampleEvals = new Dictionary<ExampleDefinitionSyntax, ObjectEval>();

        public Dictionary<FunctionDefinitionSyntax, Eval> FunctionDefinitionEvals => this.functionDefinitionEvals;

        public void AddFunctionDefinitionEval(FunctionDefinitionSyntax functionDefinition, Eval eval)
        {
            this.functionDefinitionEvals.Add(functionDefinition, eval);
        }

        public Eval GetFunctionDefinitionEval(FunctionDefinitionSyntax functionDefinition)
        {
            return this.functionDefinitionEvals[functionDefinition];
        }

        public Eval? GetFunctionDefinitionEvalIfExists(FunctionDefinitionSyntax functionDefinition)
        {
            if (!this.functionDefinitionEvals.TryGetValue(functionDefinition, out Eval functionDefinitionEval))
            {
                return null;
            }

            return functionDefinitionEval;
        }

        public void AddExampleEval(ExampleDefinitionSyntax exampleDefinition, ObjectEval objectEval)
        {
            this.exampleEvals.Add(exampleDefinition, objectEval);
        }

        public ObjectEval? GetExampleEvalIfExists(ExampleDefinitionSyntax exampleDefinition)
        {
            if (!this.exampleEvals.TryGetValue(exampleDefinition, out ObjectEval objectEval))
            {
                return null;
            }

            return objectEval;
        }
    }
}
