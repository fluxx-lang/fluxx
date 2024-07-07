using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using Faml.Binding.Internal;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Operator;
using Faml.Util;
using TypeTooling;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CodeGeneration.Expressions.Literals;
using TypeTooling.CodeGeneration.Operators;
using TypeTooling.DotNet.CodeGeneration;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.RawTypes;
using TypeTooling.Types;
using TypeTooling.Types.PredefinedTypes;
using Operator = Faml.Syntax.Operator.Operator;

namespace Faml.CodeGeneration
{
    public class CreateFunctionCode
    {
        private readonly FamlProject _famlProject;
        private readonly Dictionary<string, ParameterExpressionCode> _parametersCodeDictionary;
        public LambdaCode Result { get; }

#if false
        public ObjectEval GetOrCreateExampleEval(ExampleDefinitionSyntax example) {
            ModuleSyntax moduleSyntax = example.GetModuleSyntax();
            ModuleEvals moduleEvals = moduleSyntax.GetProject().GetModuleEvals(moduleSyntax);

            ObjectEval exampleEval = moduleEvals.GetExampleEvalIfExists(example);

            if (exampleEval == null) {
                exampleEval = BoxIfPrimitiveType(CreateExpressionEval(example.Expression));
                moduleEvals.AddExampleEval(example, exampleEval);

                CompleteDelayedResolves();
            }

            return exampleEval;
        }
#endif

        public CreateFunctionCode(FunctionDefinitionSyntax function)
        {
            this._famlProject = function.GetProject();

            ImmutableArray<ParameterExpressionCode> parametersCode = function.Parameters.Select(
                propertyNameTypePairSyntax =>
                    new ParameterExpressionCode(
                        propertyNameTypePairSyntax.PropertyName.ToString(),
                        this.GetRawType(propertyNameTypePairSyntax.TypeReferenceSyntax.GetTypeBinding()))
            ).ToImmutableArray();

            this._parametersCodeDictionary =
                parametersCode.ToDictionary(parameterExpressionCode => parameterExpressionCode.Name.ToString());

            ExpressionCode body = this.CreateExpressionCode(function.Expression);
            
            this.Result = Code.Lambda(function.FunctionName.ToString(), parametersCode, body);

            //CompleteDelayedResolves();
        }

#if false
        public Delegate GetOrCreateFunctionDelegate(FunctionDefinitionSyntax function) {
            ModuleSyntax moduleSyntax = function.GetModuleSyntax();
            ModuleDelegates moduleDelegates = moduleSyntax.GetProject().GetModuleDelegates(moduleSyntax);

            Delegate functionDelegate = moduleDelegates.GetFunctionDelegateIfExists(function);
            if (functionDelegate != null) {
                ParameterExpressionCode[] parametersCode = function.Parameters.Select(propertyNameTypePairSyntax =>
                    new ParameterExpressionCode(
                        GetRawType(propertyNameTypePairSyntax.TypeReferenceSyntax.GetTypeBinding()),
                        propertyNameTypePairSyntax.PropertyName.ToString() )
                ).ToArray();

                LambdaCode lambda = Code.Lambda(function.FunctionName.ToString(), parametersCode,
                    CreateExpressionCode(function.Expression));

                functionDelegate = CreateExpressionEval(function.Expression);

                moduleDelegates.AddFunctionDelegate(function, functionDelegate);

                CompleteDelayedResolves();
            }

            return functionDelegate;
        }
#endif

        public RawType GetRawType(TypeBinding typeBinding)
        {
            if (typeBinding is BuiltInTypeBinding predefinedTypeBinding)
            {
                return ReflectionDotNetRawType.ForPredefinedType((PredefinedType) predefinedTypeBinding.TypeToolingType);
            }
            else if (typeBinding is ExternalObjectTypeBinding externalObjectTypeBinding)
            {
                return externalObjectTypeBinding.TypeToolingType.UnderlyingType;
            }
            else if (typeBinding is ExternalEnumTypeBinding externalEnumTypeBinding)
            {
                return externalEnumTypeBinding.TypeToolingType.UnderlyingType;
            }
            else
            {
                throw new Exception($"Type {typeBinding.GetType().FullName} doesn't current support returning its RawType");
            }
        }

        private ExpressionCode CreateSymbolReferenceCode(SymbolReferenceSyntax symbolReference)
        {
            SymbolBinding symbolBinding = symbolReference.GetVariableBinding();

            if (symbolBinding is ParameterBinding parameterBinding)
            {
                string parameterName = symbolReference.VariableName.ToString();
                return this._parametersCodeDictionary[parameterName];
            }
            else if (symbolBinding is FunctionSymbolBinding functionSymbolBinding)
            {
                FunctionBinding functionBinding = functionSymbolBinding.FunctionBinding;
                if (functionBinding is InternalFunctionBinding internalFunctionBinding)
                {
                    return this.CreateInternalFunctionInvocationCode(internalFunctionBinding, ImmutableArray<ExpressionCode>.Empty);
                }
                else
                    throw new InvalidOperationException(
                        $"Unexpected function symbol binding type {functionBinding.GetType().FullName} for symbol reference '{symbolReference.Name}'");
            }
#if false
            else if (variableBinding is ForVariableBinding forVariableBinding) {
                TypeBinding typeBinding = forVariableBinding.GetTypeBinding();

                int variableIndex = forVariableBinding.VariableIndex;

                if (typeBinding == PredefinedTypeBinding.Int)
                    return new StackIntEval(variableIndex);
                else if (typeBinding == PredefinedTypeBinding.String || typeBinding is ObjectTypeBinding)
                    return new StackObjectEval(variableIndex);
                else throw new Exception($"Unexpected 'for' variable type {typeBinding} for variable {variableReference}");
            }
#endif
            else
                throw new InvalidOperationException(
                    $"Unexpected variable binding type '{symbolBinding}' for variable reference {symbolReference}");
        }

        private ExpressionCode CreatePropertyAccessCode(PropertyAccessSyntax propertyAccess)
        {
            PropertyBinding propertyBinding = propertyAccess.PropertyBinding;

            ExpressionSyntax expression = propertyAccess.Expression;
            ExpressionCode expressionCode = this.CreateExpressionCode(expression);

#if false
            if (propertyBinding is RecordPropertyBinding) {
                TypeBinding typeBinding = memberAccess.GetTypeBinding();
                string propertyName = memberAccess.PropertyName.ToString();

                if (typeBinding == PredefinedTypeBinding.Integer)
                    return new AstRecordPropertyIntEval(propertyName, expressionCode);
                else if (typeBinding == PredefinedTypeBinding.Double)
                    return new AstRecordPropertyDoubleEval(propertyName, expressionCode);
                else if (typeBinding == PredefinedTypeBinding.Boolean)
                    return new AstRecordPropertyBooleanEval(propertyName, expressionCode);
                else if (typeBinding == PredefinedTypeBinding.String || typeBinding is ObjectTypeBinding)
                    return new AstRecordPropertyObjectEval(propertyName, expressionCode);
                else
                    throw new Exception("Unexpected expression type '" + typeBinding + "' for property access " +
                                        memberAccess.ToString());
            }
#endif
            if (propertyBinding is ExternalPropertyBinding externalPropertyBinding)
                return externalPropertyBinding.ObjectType.GetGetPropertyCode(expressionCode, propertyAccess.PropertyNameString);
            else
            {
                throw new Exception($"Unexpected property access object type: {propertyBinding.ObjectTypeBinding}");
            }
        }

        private ExpressionCode CreateSequenceLiteralCode(SequenceLiteralExpressionSyntax sequenceLiteral)
        {
            ImmutableArray<ExpressionCode>.Builder itemsBuilder = ImmutableArray.CreateBuilder<ExpressionCode>();

            foreach (ExpressionSyntax expression in sequenceLiteral.Expressions)
            {
                ExpressionCode expressionCode = this.CreateExpressionCode(expression);
                itemsBuilder.Add(expressionCode);
            }

            SequenceTypeBinding sequenceTypeBinding = (SequenceTypeBinding) sequenceLiteral.GetTypeBinding();
            TypeBinding elementType = sequenceTypeBinding.ElementType;
            RawType elementRawType = this.GetRawType(elementType);

            NewSequenceCode sequenceLiterCode = Code.NewSequence(elementRawType, itemsBuilder.ToImmutable());
            return sequenceLiterCode;
        }

#if false
            RawType? sequenceUtilsRawType = _famlProject.GetTypeToolingRawType("ReactiveData.Sequence.SequenceUtils");
            if (sequenceUtilsRawType == null)
                throw new Exception($"Type 'ReactiveData.Sequence.SequenceUtils' not found; ReactiveData should always be present");

            var methodCall = DotNetCode.CallStatic(sequenceUtilsRawType!, "Items",
                new[] { _doubleType!, _gridUnitTypeType! },
                Code.DoubleLiteral(numericValue), DotNetCode.EnumValue(_gridUnitTypeType!, "Star"));


            MethodCallCode itemsMethodCall = Code.CallStatic(
                DotNetCode.New(_typeConverterRawType!),
                _convertFromInvariantStringMethod!,
                Code.StringLiteral(literal));


            ExpressionSyntax[] expressions = sequenceLiteral.Expressions;
            int length = expressions.Length;
            ObjectEval[] expressionEvals = new ObjectEval[length];

            for (int i = 0; i < length; i++)
                expressionEvals[i] = BoxIfPrimitiveType(CreateExpressionEval(expressions[i]));

            return new SequenceLiteralEval(expressionEvals);
#endif

#if false
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
#endif

        private ExpressionCode CreateFunctionInvocationCode(FunctionInvocationSyntax functionInvocation)
        {
            if (functionInvocation.LiteralConstructorValue != null)
            {
                return this.CreateExpressionCode(functionInvocation.LiteralConstructorValue);
            }

            FunctionBinding functionBinding = functionInvocation.FunctionBinding;
            Name? contentParameter = functionBinding.GetContentProperty();

            if (functionBinding is InternalFunctionBinding internalFunctionBinding)
            {
                ImmutableArray<ExpressionCode> argumentsCode = this.CreateArgumentsCode(functionInvocation, functionBinding.GetParameters());
                return this.CreateInternalFunctionInvocationCode(internalFunctionBinding, argumentsCode);
            }
#if false
            else if (functionBinding is NewRecordFunctionBinding) {
                var newAstRecordFunctionBinding = (NewRecordFunctionBinding)functionBinding;

                Name[] propertyNames = newAstRecordFunctionBinding.RecordTypeDefinition.GetProperties();
                Eval[] propertyEvals = CreateArgumentEvals(functionInvocation, propertyNames);
                return new NewAstRecordObjectEval(propertyNames, propertyEvals);
            }
#endif
            if (functionBinding is NewExternalObjectFunctionBinding newExternalObjectFunctionBinding)
            {
                return this.CreateNewExternalObjectCode(functionInvocation, newExternalObjectFunctionBinding);
            }
#if false
            else if (functionBinding is DotNetMethodFunctionBinding) {
                var dotNetMethodFunctionBinding = (DotNetMethodFunctionBinding)functionBinding;

                var thisEval = (ObjectEval)CreateExpressionEval(functionInvocation.ThisArgument);
                ObjectEval[] argumentEvals = CreateArgumentObjectEvals(functionInvocation, functionBinding.GetParameters());

                return UnboxIfDesirePrimitiveType(new DotNetMethodInvocaionEval(thisEval, argumentEvals, dotNetMethodFunctionBinding.RawMethod),
                    functionBinding.ReturnTypeBinding);
            }
#endif
            else
                throw new Exception($"Unexpected function binding type {functionBinding} for function {functionInvocation.FunctionReference}");
        }

        private ExpressionCode CreateNewExternalObjectCode(FunctionInvocationSyntax functionInvocation, NewExternalObjectFunctionBinding functionBinding)
        {
            var propertyValues = new List<PropertyValue<string, ExpressionCode>>();
            foreach (ArgumentNameValuePairSyntax argumentNameValuePair in functionInvocation.NamedArguments)
            {
                QualifiableName argumentName = argumentNameValuePair.ArgumentName;

                if (argumentNameValuePair.ArgumentName.IsQualified())
                {
                    continue;
                }

                var propertyValue = new PropertyValue<string, ExpressionCode>(argumentName.ToString(), this.CreateExpressionCode(argumentNameValuePair.Value));
                propertyValues.Add(propertyValue);
            }

            ContentArgumentSyntax? contentArgument = functionInvocation.ContentArgument;
            if (contentArgument != null)
            {
                Name? contentProperty = functionBinding.GetContentProperty();

                var propertyValue = new PropertyValue<string, ExpressionCode>(contentProperty.ToString(), this.CreateExpressionCode(contentArgument.Value));
                propertyValues.Add(propertyValue);
            }

            // TODO: Handle AttachedProperties here
            return functionBinding.ReturnExternalObjectTypeBinding.TypeToolingType.GetCreateObjectCode(propertyValues.ToArray(), new PropertyValue<AttachedProperty, ExpressionCode>[0]);
        }

        private ImmutableArray<ExpressionCode> CreateArgumentsCode(FunctionInvocationSyntax functionInvocation, Name[] parameters)
        {
            Dictionary<QualifiableName, ExpressionSyntax> argumentsDictionary = functionInvocation.CreateArgumentsDictionary();

            ImmutableArray<ExpressionCode>.Builder argumentsCodeBuilder = ImmutableArray.CreateBuilder<ExpressionCode>(parameters.Length);
            foreach (Name parameterName in parameters)
            {
                ExpressionSyntax argumentValue = argumentsDictionary.GetValueOrNull(parameterName.ToQualifiableName());
                if (argumentValue == null)
                {
                    throw new Exception($"Argument '{parameterName}' isn't specified for function '{functionInvocation.FunctionBinding.FunctionName}'");
                }

                argumentsCodeBuilder.Add(this.CreateExpressionCode(argumentValue));
            }

            return argumentsCodeBuilder.ToImmutable();
        }

        // TODO: Simplify to not use parameters passed in, just getting from functionInvocation since qualified arguments are always treated like properties -
        // there's no required set of parameters & as the order doesn't matter
        private ImmutableArray<ExpressionCode> CreateQualifiedArgumentsCode(FunctionInvocationSyntax functionInvocation, QualifiableName[] parameters)
        {
            Dictionary<QualifiableName, ExpressionSyntax> argumentsDictionary = functionInvocation.CreateArgumentsDictionary();

            ImmutableArray<ExpressionCode>.Builder argumentsCodeBuilder = ImmutableArray.CreateBuilder<ExpressionCode>(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                QualifiableName parameterName = parameters[i];
                ExpressionSyntax argumentValue = argumentsDictionary.GetValueOrNull(parameterName);
                if (argumentValue == null)
                {
                    throw new Exception($"Required parameter '{parameterName}' doesn't exist for function '{functionInvocation.FunctionBinding.FunctionName}'");
                }

                argumentsCodeBuilder.Add(this.CreateExpressionCode(argumentValue));
            }

            return argumentsCodeBuilder.ToImmutable();
        }

#if NOTNEEDED
        private ObjectEval[] CreateArgumentObjectEvals(FunctionInvocationSyntax functionInvocation, Name[] parameters) {
            Eval[] argumentEvals = CreateArgumentsCode(functionInvocation, parameters);
            int length = argumentEvals.Length;

            ObjectEval[] argumentObjectEvals = new ObjectEval[length];
            for (int i = 0; i < length; i++)
                argumentObjectEvals[i] = BoxIfPrimitiveType(argumentEvals[i]);

            return argumentObjectEvals;
        }
#endif

#if false
        private Eval[] CreateArgumentsCode(FunctionBinding functionBinding, Args args) {
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
#endif

#if NOTNEEDED
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
            var primitveTypeBinding = desiredTypeBinding as PredefinedTypeBinding;

            // If there's no need to convert to a primitive type binding, just return the unaltered eval
            if (primitveTypeBinding == null)
                return objectEval;

            if (primitveTypeBinding == PredefinedTypeBinding.Boolean)
                return new CastObjectBooleanEval(objectEval);
            else if (primitveTypeBinding == PredefinedTypeBinding.Integer)
                return new CastObjectIntEval(objectEval);
            else if (primitveTypeBinding == PredefinedTypeBinding.Double)
                return new CastObjectDoubleEval(objectEval);
            // Strings are an object type
            else if (primitveTypeBinding == PredefinedTypeBinding.String)
                return objectEval;
            else throw new Exception($"Unknown primitive type: {primitveTypeBinding}");
        }
#endif

        private ExpressionCode CreateInternalFunctionInvocationCode(InternalFunctionBinding internalFunctionBinding, ImmutableArray<ExpressionCode> argumentsCode)
        {
            FunctionDelegateHolder delegateHolder = internalFunctionBinding.Module.ModuleDelegates.
                GetOrCreateFunctionDelegate(internalFunctionBinding.FunctionDefinition);
            return DotNetCode.Invoke(delegateHolder, argumentsCode);
        }

        public ExpressionCode CreateExpressionCode(ExpressionSyntax expression)
        {
            if (expression is InfixExpressionSyntax infixExpressionSyntax)
                return this.CreateInfixExpressionCode(infixExpressionSyntax);
            else if (expression is PrefixExpressionSyntax prefixExpressionSyntax)
                return this.CreatePrefixExpressionCode(prefixExpressionSyntax);
#if false
            else if (expression is IfExpressionSyntax ifExpressionSyntax)
                return CreateIfExpressionEval(ifExpressionSyntax);
            else if (expression is ForExpressionSyntax forExpressionSyntax)
                return CreateForExpressionEval(forExpressionSyntax);
#endif
            else if (expression is ParenthesizedExpressionSyntax parenthesizedExpressionSyntax)
                return this.CreateExpressionCode(parenthesizedExpressionSyntax.Expression);
            else if (expression is BracedExpressionSyntax bracedExpression)
                return this.CreateExpressionCode(bracedExpression.Expression);
            else if (expression is SequenceLiteralExpressionSyntax sequenceLiteral)
                return this.CreateSequenceLiteralCode(sequenceLiteral);
            else if (expression is BooleanLiteralSyntax booleanLiteral)
                return Code.BooleanLiteral(booleanLiteral.Value);
            else if (expression is IntLiteralSyntax intLiteral)
                return new IntLiteralCode(intLiteral.Value);
            else if (expression is StringLiteralSyntax stringLiteral)
                return Code.StringLiteral(stringLiteral.Value);
            else if (expression is SymbolReferenceSyntax variableReferenceSyntax)
            {
#if false
                if (variableReferenceSyntax.GetVariableBinding() is ZeroArgumentFunctionBinding zeroArgumentFunctionBinding)
                    return CreateZeroArgumentFunctionInvocationEval(zeroArgumentFunctionBinding);
                else
#endif
                return this.CreateSymbolReferenceCode(variableReferenceSyntax);
            }
            else if (expression is PropertyAccessSyntax propertyAccessSyntax)
                return this.CreatePropertyAccessCode(propertyAccessSyntax);
            /*
                        else if (expression is ParamFunctionInvocation)
                            return createParamFunctionInvocationEval((ParamFunctionInvocation) expression);
            */
            else if (expression is FunctionInvocationSyntax functionInvocationSyntax)
                return this.CreateFunctionInvocationCode(functionInvocationSyntax);
#if false
            else if (expression is InterpolatedStringExpressionSyntax interpolatedStringExpressionSyntax)
                return CreateInterpolatedStringEval(interpolatedStringExpressionSyntax);
            else if (expression is EnumValueLiteralSyntax enumValueLiteralSyntax)
                return CreateEnumValueLiteralEval(enumValueLiteralSyntax);
            else if (expression is ExternalTypeCustomLiteralSytax externalTypeCustomLiteralSytax)
                return CreateExternalCustomLiteralEval(externalTypeCustomLiteralSytax);
#endif
            else
                throw new Exception("Unexpected expression type for code creation: " + expression);
        }

        private ExpressionCode CreateInfixExpressionCode(InfixExpressionSyntax expression)
        {
            InfixOperator infixOperator = expression.Operator;

            BinaryOperator codeOperator;
            if (infixOperator == Operator.And)
            {
                codeOperator = BinaryOperator.And;
            }
            else if (infixOperator == Operator.Or)
            {
                codeOperator = BinaryOperator.Or;
            }
            else if (infixOperator == Operator.Less)
            {
                codeOperator = BinaryOperator.LessThan;
            }
            else if (infixOperator == Operator.LessEquals)
            {
                codeOperator = BinaryOperator.LessThanOrEqual;
            }
            else if (infixOperator == Operator.Greater)
            {
                codeOperator = BinaryOperator.GreaterThan;
            }
            else if (infixOperator == Operator.GreaterEquals)
            {
                codeOperator = BinaryOperator.GreaterThanOrEqual;
            }
            else if (infixOperator == Operator.Plus)
            {
                codeOperator = BinaryOperator.Add;
            }
            else if (infixOperator == Operator.Minus)
            {
                codeOperator = BinaryOperator.Subtract;
            }
            else if (infixOperator == Operator.Times)
            {
                codeOperator = BinaryOperator.Multiply;
            }
            else if (infixOperator == Operator.Equals)
            {
                codeOperator = BinaryOperator.Equals;
            }
            else if (infixOperator == Operator.NotEquals)
            {
                codeOperator = BinaryOperator.NotEquals;
            }
            else
            {
                throw new Exception("Unknown infix operator: " + infixOperator.GetSourceRepresentation());
            }

            ExpressionCode leftOperandCode = this.CreateExpressionCode(expression.LeftOperand);
            ExpressionCode rightOperandCode = this.CreateExpressionCode(expression.RightOperand);

            return Code.BinaryExpression(codeOperator, leftOperandCode, rightOperandCode);
        }

        private ExpressionCode CreatePrefixExpressionCode(PrefixExpressionSyntax expression)
        {
            PrefixOperator prefixOperator = expression.Operator;

            UnaryOperator codeOperator;
            if (prefixOperator == Operator.Not)
            {
                codeOperator = UnaryOperator.Not;
            }
            else
            {
                throw new Exception("Unknown prefix operator: " + prefixOperator.GetSourceRepresentation());
            }

            ExpressionCode operandCode = this.CreateExpressionCode(expression.Operand);

            return Code.UnaryExpression(codeOperator, operandCode);
        }
    }
}
