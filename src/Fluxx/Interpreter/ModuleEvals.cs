﻿using System.Collections.Generic;
using Faml.Syntax;

namespace Faml.Interpreter {
    public class ModuleEvals {
        private readonly Dictionary<FunctionDefinitionSyntax, Eval> _functionDefinitionEvals = new Dictionary<FunctionDefinitionSyntax, Eval>();
        private readonly Dictionary<ExampleDefinitionSyntax, ObjectEval> _exampleEvals = new Dictionary<ExampleDefinitionSyntax, ObjectEval>();


        public Dictionary<FunctionDefinitionSyntax, Eval> FunctionDefinitionEvals => _functionDefinitionEvals;

        public void AddFunctionDefinitionEval(FunctionDefinitionSyntax functionDefinition, Eval eval) {
            _functionDefinitionEvals.Add(functionDefinition, eval);
        }

        public Eval GetFunctionDefinitionEval(FunctionDefinitionSyntax functionDefinition) {
            return _functionDefinitionEvals[functionDefinition];
        }

        public Eval? GetFunctionDefinitionEvalIfExists(FunctionDefinitionSyntax functionDefinition) {
            if (!_functionDefinitionEvals.TryGetValue(functionDefinition, out Eval functionDefinitionEval))
                return null;
            return functionDefinitionEval;
        }

        public void AddExampleEval(ExampleDefinitionSyntax exampleDefinition, ObjectEval objectEval) {
            _exampleEvals.Add(exampleDefinition, objectEval);
        }

        public ObjectEval? GetExampleEvalIfExists(ExampleDefinitionSyntax exampleDefinition) {
            if (!_exampleEvals.TryGetValue(exampleDefinition, out ObjectEval objectEval))
                return null;
            return objectEval;
        }
    }
}
