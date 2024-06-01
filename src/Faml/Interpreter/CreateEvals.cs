using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using Faml.Binding.Internal;
using Faml.Interpreter.Ast;
using Faml.Interpreter.External;
using Faml.Interpreter.record;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Operator;
using Faml.Util;
using TypeTooling;
using TypeTooling.CodeGeneration;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.Types;


/**
 * @author Bret Johnson
 * @since 4/4/2015
 */

namespace Faml.Interpreter {
    public class CreateEvals {
        private TypeToolingEnvironment _typeToolingEnvironment;
        private readonly ConcurrentDictionary<FunctionDefinitionSyntax, List<FunctionEvalResolved>> _functionsNeedingResolving =
            new ConcurrentDictionary<FunctionDefinitionSyntax, List<FunctionEvalResolved>>();

        public CreateEvals(TypeToolingEnvironment typeToolingEnvironment) {
            _typeToolingEnvironment = typeToolingEnvironment;
        }

        public ObjectEval GetOrCreateExampleEval(ExampleDefinitionSyntax example) {
            ModuleSyntax moduleSyntax = example.GetModuleSyntax();
            ModuleEvals moduleEvals = moduleSyntax.GetProject().GetModuleEvals(moduleSyntax);

            ObjectEval? exampleEval = moduleEvals.GetExampleEvalIfExists(example);

            if (exampleEval == null) {
                exampleEval = BoxIfPrimitiveType(CreateExpressionEval(example.Expression));
                moduleEvals.AddExampleEval(example, exampleEval);

                CompleteDelayedResolves();
            }

            return exampleEval;
        }

        public Eval GetOrCreateFunctionEval(FunctionDefinitionSyntax function) {
            ModuleSyntax moduleSyntax = function.GetModuleSyntax();
            ModuleEvals moduleEvals = moduleSyntax.GetProject().GetModuleEvals(moduleSyntax);

            Eval functionEval = moduleEvals.GetFunctionDefinitionEvalIfExists(function);

            if (functionEval == null) {
                functionEval = CreateExpressionEval(function.Expression);

                moduleEvals.AddFunctionDefinitionEval(function, functionEval);

                CompleteDelayedResolves();
            }

            return functionEval;
        }

        public Eval CreateFunctionInvocationEval(FunctionDefinitionSyntax function, Args args) {
            var functionBinding = new InternalFunctionBinding(function);

            Eval[] argumentEvals = CreateArgumentEvals(functionBinding, args);
            Eval eval = CreateAstFunctionInvocationEval(function, argumentEvals);
            CompleteDelayedResolves();

            return eval;
        }

        public delegate void FunctionEvalResolved(Eval functionDefinitionEval);

        public void DelayResolveFunctionEval(FunctionDefinitionSyntax functionDefinition, FunctionEvalResolved functionEvalResolved) {
            ModuleEvals moduleEvals = functionDefinition.GetProject().GetModuleEvals(functionDefinition.GetModuleSyntax());

            Eval? functionEval = moduleEvals.GetFunctionDefinitionEvalIfExists(functionDefinition);

            // See if the function has already been resolved. If so, call the callback immediately. Otherwise, add it to our list
            // to resolve later.
            if (functionEval != null)
                functionEvalResolved(functionEval);
            else {
                List<FunctionEvalResolved> functionEvalResolveds = _functionsNeedingResolving.GetOrAdd(functionDefinition, (_) => new List<FunctionEvalResolved>());
                functionEvalResolveds.Add(functionEvalResolved);
            }
        }

        /// <summary>
        /// Resolve all functions needing resolving and notify the function users of the resolution.
        /// </summary>
        private void CompleteDelayedResolves() {
            while (true) {
                FunctionDefinitionSyntax function = _functionsNeedingResolving.Keys.FirstOrDefault();
                if (function == null)
                    break;

                if (!_functionsNeedingResolving.TryRemove(function, out List<FunctionEvalResolved> functionEvalResolveds))
                    continue;

                ModuleSyntax moduleSyntax = function.GetModuleSyntax();
                ModuleEvals moduleEvals = moduleSyntax.GetProject().GetModuleEvals(moduleSyntax);
                Eval? functionEval = moduleEvals.GetFunctionDefinitionEvalIfExists(function);

                if (functionEval == null) {
                    functionEval = CreateExpressionEval(function.Expression);
                    moduleEvals.AddFunctionDefinitionEval(function, functionEval);
                }

                foreach (FunctionEvalResolved functionEvalResolved in functionEvalResolveds)
                    functionEvalResolved(functionEval);
            }
        }

        private Eval CreateVariableEval(SymbolReferenceSyntax symbolReference) {
            SymbolBinding symbolBinding = symbolReference.GetVariableBinding();

            if (symbolBinding is ParameterBinding astParameterBinding) {
                int parameterIndex = astParameterBinding.ParameterIndex;
                int parameterCount = astParameterBinding.FunctionDefinition.Parameters.Length;

                // Parameters are pushed left to right, and the BaseIndex will point to the spot on the
                // stack right after they are pushed. Thus all have a negative offset, the last parameter
                // at offset -1
                int parameterBaseOffset = -parameterCount + parameterIndex;

                TypeBinding typeBinding = symbolReference.GetTypeBinding();

                if (typeBinding == BuiltInTypeBinding.Int)
                    return new StackIntEval(parameterBaseOffset);
                else if (typeBinding == BuiltInTypeBinding.Double)
                    return new StackDoubleEval(parameterBaseOffset);
                else if (typeBinding == BuiltInTypeBinding.Bool)
                    return new StackBooleanEval(parameterBaseOffset);
                else if (typeBinding == BuiltInTypeBinding.String || typeBinding is ObjectTypeBinding)
                    return new StackObjectEval(parameterBaseOffset);
                else throw new Exception($"Unexpected parameter type {typeBinding} for parameter {symbolReference}");
            }
            else if (symbolBinding is ForSymbolBinding forVariableBinding) {
                TypeBinding typeBinding = forVariableBinding.GetTypeBinding();

                int variableIndex = forVariableBinding.VariableIndex;

                if (typeBinding == BuiltInTypeBinding.Int)
                    return new StackIntEval(variableIndex);
                else if (typeBinding == BuiltInTypeBinding.String || typeBinding is ObjectTypeBinding)
                    return new StackObjectEval(variableIndex);
                else throw new Exception($"Unexpected 'for' variable type {typeBinding} for variable {symbolReference}");
            }
            else
                throw new Exception(
                    $"Unexpected variable binding type '{symbolBinding}' for variable reference {symbolReference}");
        }

        private Eval CreatePropertyAccessEval(PropertyAccessSyntax propertyAccess) {
            PropertyBinding propertyBinding = propertyAccess.PropertyBinding;

            ExpressionSyntax expression = propertyAccess.Expression;
            var expressionEval = (ObjectEval) CreateExpressionEval(expression);

            if (propertyBinding is RecordPropertyBinding) {
                TypeBinding typeBinding = propertyAccess.GetTypeBinding();
                string propertyName = propertyAccess.PropertyNameString;

                if (typeBinding == BuiltInTypeBinding.Int)
                    return new AstRecordPropertyIntEval(propertyName, expressionEval);
                else if (typeBinding == BuiltInTypeBinding.Double)
                    return new AstRecordPropertyDoubleEval(propertyName, expressionEval);
                else if (typeBinding == BuiltInTypeBinding.Bool)
                    return new AstRecordPropertyBooleanEval(propertyName, expressionEval);
                else if (typeBinding == BuiltInTypeBinding.String || typeBinding is ObjectTypeBinding)
                    return new AstRecordPropertyObjectEval(propertyName, expressionEval);
                else
                    throw new Exception("Unexpected expression type '" + typeBinding + "' for property access " +
                                        propertyAccess.ToString());
            }
            else if (propertyBinding is ExternalPropertyBinding externalPropertyBinding)
                return new ExternalPropertyAccessEval(expressionEval, externalPropertyBinding.ObjectType, externalPropertyBinding.ObjectProperty);
            else throw new Exception($"Unexpected property access object type: {propertyBinding.ObjectTypeBinding}");
        }

        private SequenceLiteralEval CreateListBuilderEval(SequenceLiteralExpressionSyntax sequenceLiteralExpression) {
            ExpressionSyntax[] expressions = sequenceLiteralExpression.Expressions;
            int length = expressions.Length;
            ObjectEval[] expressionEvals = new ObjectEval[length];

            for (int i = 0; i < length; i++)
                expressionEvals[i] = BoxIfPrimitiveType(CreateExpressionEval(expressions[i]));

            return new SequenceLiteralEval(expressionEvals);
        }

        private SequenceLiteralEval CreateEmptySequenceEval() {
            ObjectEval[] expressionEvals = new ObjectEval[0];
            return new SequenceLiteralEval(expressionEvals);
        }

        private InterpolatedStringEval CreateInterpolatedStringEval(
            InterpolatedStringExpressionSyntax interpolatedStringExpression) {
            ExpressionSyntax[] expressions = interpolatedStringExpression.Expressions;
            int length = expressions.Length;
            InterpolatedStringFragmentSyntax[] stringFragments = interpolatedStringExpression.StringFragments;

            string[] evalStringFragments = new string[length + 1];
            ObjectEval[] evalExpressions = new ObjectEval[length];

            for (int i = 0; i < length; i++) {
                evalStringFragments[i] = stringFragments[i].Value;
                evalExpressions[i] = BoxIfPrimitiveType(CreateExpressionEval(expressions[i]));
            }
            evalStringFragments[length] = stringFragments[length].Value;

            return new InterpolatedStringEval(evalStringFragments, evalExpressions);
        }

        /*
        private Eval createParamFunctionInvocationEval(ParamFunctionInvocation paramFunctionInvocation) {
            AstFunctionBinding astFunctionBinding = (AstFunctionBinding) paramFunctionInvocation.getFunctionBinding();
            FunctionDefinition functionDefinition = astFunctionBinding.getFunctionDefinition();

            Expression[] parameters = paramFunctionInvocation.getParameters();
            int parametersLength = parameters.Length;

            Eval[] parametersEvals = new Eval[parametersLength];
            for (int i = 0; i < parametersLength; i++) {
                parametersEvals[i] = createExpressionEval(parameters[i]);
            }

            return createAstFunctionInvocationEval(functionDefinition, parametersEvals);
        }
        */

        private Eval CreateFunctionInvocationEval(FunctionInvocationSyntax functionInvocation) {
            if (functionInvocation.LiteralConstructorValue != null)
                return CreateExpressionEval(functionInvocation.LiteralConstructorValue);

            FunctionBinding functionBinding = functionInvocation.FunctionBinding;
            Name? contentParameter = functionBinding.GetContentProperty();

#if NOMORE
            ArgumentNameValuePairSyntax[] arguments = functionInvocation.NamedArguments;
            if (arguments.Length == 1 && arguments[0].UseDefault() && contentParameter == null)
                throw new Exception($"No content parameter exists for function '{functionBinding.FunctionName}'");
#endif

            if (functionBinding is InternalFunctionBinding astFunctionBinding) {
                Eval[] argumentEvals = CreateArgumentEvals(functionInvocation, functionBinding.GetParameters());
                return CreateAstFunctionInvocationEval(astFunctionBinding.FunctionDefinition, argumentEvals);
            }
            else if (functionBinding is NewRecordFunctionBinding) {
                var newAstRecordFunctionBinding = (NewRecordFunctionBinding) functionBinding;

                Name[] propertyNames = newAstRecordFunctionBinding.RecordTypeDefinition.GetProperties();
                Eval[] propertyEvals = CreateArgumentEvals(functionInvocation, propertyNames);
                return new NewAstRecordObjectEval(propertyNames, propertyEvals);
            }
            else if (functionBinding is NewExternalObjectFunctionBinding) {
                var newCSharpObjectFunctionBinding = (NewExternalObjectFunctionBinding) functionBinding;

                Name[] argumentNames = new Name[0]; // functionInvocation.GetArgumentNames();
                Eval[] argumentEvals = CreateArgumentEvals(functionInvocation, argumentNames);

                QualifiableName[] qualifiedArgumentNames = functionInvocation.GetQualifiedArgumentNames();
                Eval[] qualifiedArgumentEvals = CreateQualfiedArgumentEvals(functionInvocation, qualifiedArgumentNames);

                return new NewExternalObjectEval(functionInvocation.GetModuleSyntax(), newCSharpObjectFunctionBinding,
                    argumentNames, argumentEvals, qualifiedArgumentNames, qualifiedArgumentEvals);
            }
#if FIXME
            else if (functionBinding is DotNetMethodFunctionBinding) {
                var dotNetMethodFunctionBinding = (DotNetMethodFunctionBinding) functionBinding;

                var thisEval = (ObjectEval) CreateExpressionEval(functionInvocation.ThisArgument);
                ObjectEval[] argumentEvals = CreateArgumentObjectEvals(functionInvocation, functionBinding.GetParameters());

                return UnboxIfDesirePrimitiveType(new DotNetMethodInvocaionEval(thisEval, argumentEvals, dotNetMethodFunctionBinding.RawMethod),
                    functionBinding.ReturnTypeBinding);
            }
#endif
            else
                throw new Exception($"Unexpected function binding type {functionBinding} for function {functionInvocation.FunctionReference}");
        }

        private Eval CreateZeroArgumentFunctionInvocationEval(FunctionSymbolBinding functionSymbolBinding) {
            FunctionBinding functionBinding = functionSymbolBinding.FunctionBinding;

            if (functionBinding is InternalFunctionBinding astFunctionBinding)
                return CreateAstFunctionInvocationEval(astFunctionBinding.FunctionDefinition, new Eval[0]);
            else if (functionBinding is NewRecordFunctionBinding)
                return new NewAstRecordObjectEval(new Name[0], new Eval[0]);
            else
                throw new Exception(
                    $"Unexpected function binding type {functionBinding} for zero argument function {functionSymbolBinding.FunctionBinding.FunctionName}");
        }

        private Eval[] CreateArgumentEvals(FunctionInvocationSyntax functionInvocation, Name[] parameters) {
            FunctionBinding functionBinding = functionInvocation.FunctionBinding;

            Dictionary<QualifiableName, ExpressionSyntax> argumentsDictionary = functionInvocation.CreateArgumentsDictionary();

            int parametersLength = parameters.Length;

            Eval[] argumentEvals = new Eval[parametersLength];
            for (int i = 0; i < parametersLength; i++) {
                Name parameterName = parameters[i];
                ExpressionSyntax argumentValue = argumentsDictionary.GetValueOrNull(parameterName.ToQualifiableName());
                if (argumentValue == null)
                    throw new Exception($"Argument '{parameterName}' isn't specified for function '{functionBinding.FunctionName}'");
                argumentEvals[i] = CreateExpressionEval(argumentValue);
            }
            return argumentEvals;
        }

        private Eval[] CreateQualfiedArgumentEvals(FunctionInvocationSyntax functionInvocation, QualifiableName[] parameters) {
            FunctionBinding functionBinding = functionInvocation.FunctionBinding;

            Dictionary<QualifiableName, ExpressionSyntax> argumentsDictionary = functionInvocation.CreateArgumentsDictionary();

            int parametersLength = parameters.Length;

            Eval[] argumentEvals = new Eval[parametersLength];
            for (int i = 0; i < parametersLength; i++) {
                QualifiableName parameterName = parameters[i];
                ExpressionSyntax argumentValue = argumentsDictionary.GetValueOrNull(parameterName);
                if (argumentValue == null)
                    throw new Exception($"Parameter '{parameterName}' doesn't exist for function '{functionBinding.FunctionName}'");
                argumentEvals[i] = CreateExpressionEval(argumentValue);
            }
            return argumentEvals;
        }

        private ObjectEval[] CreateArgumentObjectEvals(FunctionInvocationSyntax functionInvocation, Name[] parameters) {
            Eval[] argumentEvals = CreateArgumentEvals(functionInvocation, parameters);
            int length = argumentEvals.Length;

            ObjectEval[] argumentObjectEvals = new ObjectEval[length];
            for (int i = 0; i < length; i++)
                argumentObjectEvals[i] = BoxIfPrimitiveType(argumentEvals[i]);

            return argumentObjectEvals;
        }

        private Eval[] CreateArgumentEvals(FunctionBinding functionBinding, Args args) {
            // TODO: Handle primitive types properly, including casting
            // TODO: Check for supplied arguments that aren't supported for the function

            Name[] parameters = functionBinding.GetParameters();
            int parametersLength = parameters.Length;

            Eval[] argumentEvals = new Eval[parametersLength];
            for (int i = 0; i < parametersLength; i++) {
                Name parameterName = parameters[i];
                object argumentValue = args.GetValueOrNull(parameterName.ToString());
                if (argumentValue == null)
                    throw new Exception($"Argument '{parameterName}' isn't specified for function '{functionBinding.FunctionName}'");

                argumentEvals[i] = new ObjectConstantEval(argumentValue);
            }
            return argumentEvals;
        }

        public static ObjectEval BoxIfPrimitiveType(Eval eval) {
            if (eval is BooleanEval booleanEval)
                return new CastBooleanObjectEval(booleanEval);
            else if (eval is IntEval intEval)
                return new CastIntObjectEval(intEval);
            else if (eval is ObjectEval objectEval)
                return objectEval;
            else throw new Exception($"Unsupported property type for {eval}");
        }

        private static Eval UnboxIfDesirePrimitiveType(ObjectEval objectEval, TypeBinding desiredTypeBinding) {
            var primitveTypeBinding = desiredTypeBinding as BuiltInTypeBinding;

            // If there's no need to convert to a primitive type binding, just return the unaltered eval
            if (primitveTypeBinding == null)
                return objectEval;

            if (primitveTypeBinding == BuiltInTypeBinding.Bool)
                return new CastObjectBooleanEval(objectEval);
            else if (primitveTypeBinding == BuiltInTypeBinding.Int)
                return new CastObjectIntEval(objectEval);
            else if (primitveTypeBinding == BuiltInTypeBinding.Double)
                return new CastObjectDoubleEval(objectEval);
            // Strings are an object type
            else if (primitveTypeBinding == BuiltInTypeBinding.String)
                return objectEval;
            else throw new Exception($"Unknown primitive type: {primitveTypeBinding}");
        }

        private Eval CreateAstFunctionInvocationEval(FunctionDefinitionSyntax functionDefinition, Eval[] parameterEvals) {
            //Eval functionExpressionEval = createExpressionEval(functionDefinition.expression);

            TypeBinding returnTypeBinding = functionDefinition.ReturnTypeBinding;
            if (returnTypeBinding == BuiltInTypeBinding.Int)
                return new FunctionInvocationIntEval(parameterEvals, this, functionDefinition);
            else if (returnTypeBinding == BuiltInTypeBinding.Double)
                return new FunctionInvocationDoubleEval(parameterEvals, this, functionDefinition);
            else if (returnTypeBinding == BuiltInTypeBinding.Bool)
                return new FunctionInvocationBooleanEval(parameterEvals, this, functionDefinition);
            else if (returnTypeBinding is ObjectTypeBinding || returnTypeBinding is SequenceTypeBinding || returnTypeBinding == BuiltInTypeBinding.String)
                return new FunctionInvocationObjectEval(parameterEvals, this, functionDefinition);
            else
                throw new Exception(
                    $"Unexpected function return type to eval, '{returnTypeBinding}', for function {functionDefinition.FunctionNameSyntax}");
        }

/*  

        private Eval createMemberAccessEval(Expression expression, SimpleName simpleName) {
            TypeBinding expressionTypeBinding = expression.getTypeBinding();

            if (expressionTypeBinding is DotNetObjectTypeBinding) {


                
            }

            (functionBinding is NewCSharpObjectFunctionBinding) {
                NewCSharpObjectFunctionBinding newCSharpObjectFunctionBinding =
                    (NewCSharpObjectFunctionBinding)functionBinding;

                PropertyValue[] propertyValues = functionInvocation.getPropertyValues();
                int propertyValuesLength = propertyValues.Length;

                string[] propertyNames = new string[propertyValuesLength];
                Eval[] propertyEvals = new Eval[propertyValuesLength];
                for (int i = 0; i < propertyValuesLength; i++) {
                    PropertyValue propertyValue = propertyValues[i];
                    propertyNames[i] = propertyValue.name.identifier;
                    propertyEvals[i] = createExpressionEval(propertyValue.value);
                }

                Expression[] expressions = functionInvocation.getExpressions();
                int expressionsLength = expressions.Length;

                Eval[] expressionEvals = new Eval[expressionsLength];
                for (int i = 0; i < expressionsLength; i++) {
                    expressionEvals[i] = createExpressionEval(expressions[i]);
                }

                return new NewCSharpObjectEval(propertyNames, propertyEvals, expressionEvals,
                    newCSharpObjectFunctionBinding.getCSharpTypeInfo());
            }




            Eval functionExpressionEval = createExpressionEval(functionDefinition.expression);

            TypeReferenceSyntax returnType = objectEval.;
            if (returnType is IntTypeReferenceSyntax)
                return new FunctionInvocationIntEval(parameterEvals, this, functionDefinition);
            else if (returnType is BoolTypeReferenceSyntax)
                return new FunctionInvocationBooleanEval(parameterEvals, this, functionDefinition);
            else if (returnType is ObjectTypeReferenceSyntax || returnType is StringTypeReferenceSyntax)
                return new FunctionInvocationObjectEval(parameterEvals, this, functionDefinition);
            else
                throw new Exception(
                    $"Unexpected function return type to eval, '{returnType.GetType()}', for function {functionDefinition.getName()}");
        }
*/

        public Eval CreateExpressionEval(ExpressionSyntax expression) {
            if (expression is InfixExpressionSyntax infixExpressionSyntax)
                return CreateInfixExpressionEval(infixExpressionSyntax);
            else if (expression is PrefixExpressionSyntax prefixExpressionSyntax)
                return CreatePrefixExpressionEval(prefixExpressionSyntax);
            else if (expression is IfExpressionSyntax ifExpressionSyntax)
                return CreateIfExpressionEval(ifExpressionSyntax);
            else if (expression is ForExpressionSyntax forExpressionSyntax)
                return CreateForExpressionEval(forExpressionSyntax);
            else if (expression is ParenthesizedExpressionSyntax parenthesizedExpressionSyntax)
                return CreateExpressionEval(parenthesizedExpressionSyntax.Expression);
            else if (expression is BracedExpressionSyntax bracedExpression)
                return CreateExpressionEval(bracedExpression.Expression);
            else if (expression is SequenceLiteralExpressionSyntax listExpressionSyntax)
                return CreateListBuilderEval(listExpressionSyntax);
            else if (expression is BooleanLiteralSyntax booleanLiteral)
                return (booleanLiteral.Value? (Eval) new TrueEval() : new FalseEval());
            else if (expression is IntLiteralSyntax intLiteral)
                return new IntLiteralEval(intLiteral.Value);
            else if (expression is StringLiteralSyntax stringLiteral)
                return new StringLiteralEval(stringLiteral.Value);
            else if (expression is SymbolReferenceSyntax variableReferenceSyntax) {
                if (variableReferenceSyntax.GetVariableBinding() is FunctionSymbolBinding zeroArgumentFunctionBinding)
                    return CreateZeroArgumentFunctionInvocationEval(zeroArgumentFunctionBinding);
                else return CreateVariableEval(variableReferenceSyntax);
            }
            else if (expression is PropertyAccessSyntax propertyAccess)
                return CreatePropertyAccessEval(propertyAccess);
/*
            else if (expression is ParamFunctionInvocation)
                return createParamFunctionInvocationEval((ParamFunctionInvocation) expression);
*/
            else if (expression is FunctionInvocationSyntax functionInvocationSyntax)
                return CreateFunctionInvocationEval(functionInvocationSyntax);
            else if (expression is InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax)
                return CreateInterpolatedStringEval(interpolatedStringExpressionSyntax);
            else if (expression is EnumValueLiteralSyntax enumValueLiteralSyntax)
                return CreateEnumValueLiteralEval(enumValueLiteralSyntax);
            else if (expression is ExternalTypeCustomLiteralSytax externalTypeCustomLiteralSytax) {
                return CreateExternalCustomLiteralEval(externalTypeCustomLiteralSytax);
            }
            else
                throw new Exception("Unexpected expression type to eval: " + expression);
        }

        private Eval CreateEnumValueLiteralEval(EnumValueLiteralSyntax enumValueLiteralSyntax) {
            EnumValueBinding enumValueBinding = enumValueLiteralSyntax.EnumValueBinding;

            if (enumValueBinding is ExternalEnumValueBinding externalEnumValueBinding) {
                EnumValue enumValue = externalEnumValueBinding.EnumValue;
                return CreateObjectConstantDelegateEval(enumValue.ExpressionAndHelpersCode);
            }
            else throw new UserViewableException($"Unexpected EnumValueBindingType: {enumValueBinding.GetType().FullName}");
        }

        private Eval CreateExternalCustomLiteralEval(ExternalTypeCustomLiteralSytax externalTypeCustomLiteralSytax) {
            CustomLiteralParser? customLiteralParser = externalTypeCustomLiteralSytax.ExternalType.GetCustomLiteralParser();
            if (customLiteralParser == null)
                throw new Exception(
                    $"ExternalType {externalTypeCustomLiteralSytax.ExternalType.FullName} unexpectedly doesn't have a CustomerLiteralManager");
            ExpressionAndHelpersCode? literalValueCode = externalTypeCustomLiteralSytax.CustomLiteral.ExpressionAndHelpersCode;
            return CreateObjectConstantDelegateEval(literalValueCode);
        }

        private ObjectConstantDelegateEval CreateObjectConstantDelegateEval(ExpressionAndHelpersCode literalValueCode) {
            Expression literalExpression = new ConvertToExpressionTree(_typeToolingEnvironment, literalValueCode.Expression).Result;
            LambdaExpression literalLambda = Expression.Lambda(literalExpression);
            Delegate literalDelegate = literalLambda.Compile();

            return new ObjectConstantDelegateEval(literalDelegate);
        }

        private Eval CreateInfixExpressionEval(InfixExpressionSyntax expression) {
            InfixOperator infixOperator = expression.Operator;

            Eval leftEval = CreateExpressionEval(expression.LeftOperand);
            Eval rightEval = CreateExpressionEval(expression.RightOperand);

            TypeBinding leftTypeBinding = expression.LeftOperand.GetTypeBinding();
            TypeBinding rightTypeBiding = expression.RightOperand.GetTypeBinding();

            /*
            if (infixOperator == Operator.DOT) {
                return new MemberAccessEval((BooleanEval)leftEval, (BooleanEval)rightEval);
            }
            */
            if (infixOperator == Operator.And)
                return new AndEval((BooleanEval) leftEval, (BooleanEval) rightEval);
            else if (infixOperator == Operator.Or)
                return new OrEval((BooleanEval) leftEval, (BooleanEval) rightEval);
            else if (infixOperator == Operator.Less)
                return new LessEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.LessEquals)
                return new LessEqualsEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.Greater)
                return new GreaterEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.GreaterEquals)
                return new GreaterEqualsEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.Plus)
                return new PlusEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.Minus)
                return new MinusEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.Times)
                return new TimesEval((IntEval) leftEval, (IntEval) rightEval);
            else if (infixOperator == Operator.Equals) {
                if (leftTypeBinding == BuiltInTypeBinding.Int && rightTypeBiding == BuiltInTypeBinding.Int)
                    return new EqualsIntEval((IntEval) leftEval, (IntEval) rightEval);
                else if (leftTypeBinding == BuiltInTypeBinding.Bool &&
                         rightTypeBiding == BuiltInTypeBinding.Bool)
                    return new EqualsBooleanEval((BooleanEval) leftEval, (BooleanEval) rightEval);
                else
                    throw new Exception("Cannot compare these two types with == :" + leftTypeBinding + " and " +
                                        rightTypeBiding);
            }
            else if (infixOperator == Operator.NotEquals) {
                if (leftTypeBinding == BuiltInTypeBinding.Int && rightTypeBiding == BuiltInTypeBinding.Int)
                    return new NotEqualsIntEval((IntEval) leftEval, (IntEval) rightEval);
                else if (leftTypeBinding == BuiltInTypeBinding.Bool &&
                         rightTypeBiding == BuiltInTypeBinding.Bool)
                    return new NotEqualsBooleanEval((BooleanEval) leftEval, (BooleanEval) rightEval);
                else
                    throw new Exception("Cannot compare these two types with == :" + leftTypeBinding + " and " +
                                        rightTypeBiding);
            }
            else
                throw new Exception("Unexpected infix operator to eval: " + infixOperator.GetSourceRepresentation());
        }

        private Eval CreatePrefixExpressionEval(PrefixExpressionSyntax expression) {
            PrefixOperator prefixOperator = expression.Operator;

            Eval operandEval = CreateExpressionEval(expression.Operand);

            if (prefixOperator == Operator.Not)
                return new NotEval((BooleanEval) operandEval);
            else throw new Exception("Unexpected prefix operator to eval: " + prefixOperator.GetSourceRepresentation());
        }

        private Eval CreateIfExpressionEval(IfExpressionSyntax ifExpression) {
            TypeBinding typeBinding = ifExpression.GetTypeBinding();
            if (typeBinding == null)
                throw new Exception("Unable to determine type binding of IfExpression");

            // TODO: Handle this error at parsing time
            if (! (typeBinding is ObjectTypeBinding))
                throw new Exception("'if' expressions are just supported for object types currently");

            var conditionEvals = new List<BooleanEval>();
            var valueEvals = new List<ObjectEval>();

            foreach (ConditionValuePairSyntax conditionValuePair in ifExpression.ConditionValuePairs) {
                conditionEvals.Add((BooleanEval) CreateExpressionEval(conditionValuePair.Condition));
                valueEvals.Add((ObjectEval) CreateExpressionEval(conditionValuePair.Value));
            }

            ExpressionSyntax elseValue = ifExpression.ElseValue;
            ObjectEval elseValueEval;
            if (elseValue == null)
                elseValueEval = CreateEmptySequenceEval();
            else elseValueEval = (ObjectEval) CreateExpressionEval(elseValue);

            return new IfObjectEval(conditionEvals.ToArray(), valueEvals.ToArray(), elseValueEval);
        }

        private Eval CreateForExpressionEval(ForExpressionSyntax forExpression) {
            TypeBinding typeBinding = forExpression.GetTypeBinding();
            if (typeBinding == null)
                throw new Exception("Unable to determine type binding of ForExpression");

            // TODO: Handle this error at parsing time
            if (!(typeBinding is ObjectTypeBinding))
                throw new Exception("'for' expressions are just supported for object types currently");

            var expressionEval = (ObjectEval) CreateExpressionEval(forExpression.Expression);
            var inExpressionEval = (ObjectEval) CreateExpressionEval(forExpression.ForVariableDefinition.InExpression);

            return new ForExpressionEval(expressionEval, forExpression.ForVariableDefinition.GetVariableTypeBinding(), inExpressionEval);
        }

        private class ItemNeedingEval {
            private SyntaxNode _item;

            public ItemNeedingEval(SyntaxNode item) {
                _item = item;
            }
        }
    }
}
