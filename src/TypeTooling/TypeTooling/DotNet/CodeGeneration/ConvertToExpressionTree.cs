using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using TypeTooling.CodeGeneration;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CodeGeneration.Expressions.Literals;
using TypeTooling.CodeGeneration.Operators;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.RawTypes.Reflection;
using TypeTooling.RawTypes;

namespace TypeTooling.DotNet.CodeGeneration
{
    public class ConvertToExpressionTree : IConvertToExpressionTree
    {
        private readonly TypeToolingEnvironment _typeToolingEnvironment;
        private readonly Dictionary<string, ParameterExpression> _parameterExpressionsDictionary;
        public LambdaExpression Result { get; }

        public ConvertToExpressionTree(TypeToolingEnvironment typeToolingEnvironment, LambdaCode lambdaCode)
        {
            this._typeToolingEnvironment = typeToolingEnvironment;

            ParameterExpression[] parameterExpressions = lambdaCode.Parameters
                .Select(parameterExpressionCode =>
                    Expression.Parameter(GetType(parameterExpressionCode.Type), parameterExpressionCode.Name))
                .ToArray();
            this._parameterExpressionsDictionary = parameterExpressions.ToDictionary(parameterExpression => parameterExpression.Name);

            Expression bodyExpression = this.ConvertExpression(lambdaCode.Body);

            // TODO: Should we always pass tailCall:true here?
            this.Result = Expression.Lambda(bodyExpression, true, parameterExpressions);
        }

        public ConvertToExpressionTree(TypeToolingEnvironment typeToolingEnvironment, ExpressionCode expressionCode)
        {
            this._typeToolingEnvironment = typeToolingEnvironment;
            this._parameterExpressionsDictionary = new Dictionary<string, ParameterExpression>();

            Expression bodyExpression = this.ConvertExpression(expressionCode);

            // TODO: Should we always pass tailCall:true here?
            this.Result = Expression.Lambda(bodyExpression, true, Array.Empty<ParameterExpression>());
        }

        public Expression? ConvertExpressionOrNull(ExpressionCode? expressionCode)
        {
            return expressionCode == null ? null : this.ConvertExpression(expressionCode);
        }

        public Expression ConvertExpression(ExpressionCode expressionCode)
        {
            switch (expressionCode)
            {
                case StringLiteralCode stringLiteralCode:
                    return Expression.Constant(stringLiteralCode.Value);
                case BooleanLiteralCode booleanLiteralCode:
                    return Expression.Constant(booleanLiteralCode.Value);
                case ByteLiteralCode byteLiteralCode:
                    return Expression.Constant(byteLiteralCode.Value);
                case IntLiteralCode intLiteralCode:
                    return Expression.Constant(intLiteralCode.Value);
                case EnumValueLiteralCode enumValueLiteralCode:
                {
                        RawType enumType = enumValueLiteralCode.EnumType;
                        if (!(enumType is DotNetRawType dotNetRawType))
                            throw new Exception($"Enum type is {enumType.GetType().FullName}, not a DotNetRawType as expected");
                        return Expression.Constant(dotNetRawType.GetEnumUnderlyingValue(enumValueLiteralCode.ValueName));
                    }
                case NewObjectCode newObjectCode:
                    return this.ConvertNewObject(newObjectCode);
                case NewArrayInitCode newArrayInit:
                    return this.ConvertNewArrayInit(newArrayInit);
                case NewSequenceCode newSequenceCode:
                    return this.ConvertNewSequence(newSequenceCode);
                case MethodCallCode methodCallCode:
                    return this.ConvertMethodCall(methodCallCode);
                case GenericMethodCallCode genericMethodCallCode:
                    return this.ConvertGenericMethodCall(genericMethodCallCode);
                case GetPropertyCode getPropertyCode:
                    return this.ConvertGetProperty(getPropertyCode);
                case BinaryExpressionCode binaryExpressionCode:
                    return this.ConvertBinaryExpression(binaryExpressionCode);
                case UnaryExpressionCode unaryExpressionCode:
                    return this.ConvertUnaryExpression(unaryExpressionCode);
                case ParameterExpressionCode parameterExpressionCode:
                    return this._parameterExpressionsDictionary[parameterExpressionCode.Name];
                case FunctionDelegateInvokeCode functionDelegateInvokeCode:
                    return this.ConvertFunctionDelegateInvoke(functionDelegateInvokeCode);
                default:
                    throw new Exception($"Unsupported ExpressionCode type: {expressionCode.GetType()}");
            }
        }

        private Expression ConvertNewObject(NewObjectCode newObjectCode)
        {
            ConstructorInfo constructorInfo = GetConstructorInfo(newObjectCode.Constructor);
            ExpressionCode[] codeArguments = newObjectCode.ConstructorArguments;

            NewExpression newExpression = Expression.New(constructorInfo, codeArguments.Select(this.ConvertExpression));

            PropertyValue<RawProperty, ExpressionCode>[] propertyValues = newObjectCode.PropertyValues;

            if (propertyValues.Length == 0)
                return newExpression;

            bool needToSetPropertiesPostCreation = false;
            List<MemberAssignment> memberAssignments = new List<MemberAssignment>();
            foreach (var propertyValue in propertyValues)
            {
                DotNetRawProperty rawProperty = (DotNetRawProperty)propertyValue.Property;

                if (!rawProperty.CanWrite)
                {
                    DotNetRawType iListRawType = (DotNetRawType)this._typeToolingEnvironment.GetRequiredRawType("System.Collections.IList");

                    if (rawProperty.PropertyType.IsAssignableTo(iListRawType))
                        needToSetPropertiesPostCreation = true;
                    else throw new UserViewableException($"Property {rawProperty.Name} isn't writable");
                }
                else
                {
                    memberAssignments.Add(Expression.Bind(GetMemberInfo(rawProperty),
                        this.ConvertExpression(propertyValue.Value)));
                }
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(newExpression, memberAssignments);

            if (! needToSetPropertiesPostCreation)
                return memberInitExpression;

            List<ParameterExpression> blockVariables = new List<ParameterExpression>();
            List<Expression> blockExpressions = new List<Expression>();

            Type objectType = constructorInfo.DeclaringType;
            if (objectType == null)
                throw new InvalidOperationException($"{constructorInfo} is unexpectedly a static method");

            ParameterExpression objVariableExpression = Expression.Variable(objectType, "obj");
            blockVariables.Add(objVariableExpression);

            BinaryExpression assignExpression = Expression.Assign(objVariableExpression, memberInitExpression);
            blockExpressions.Add(assignExpression);

            DotNetRawType sequenceUtilsRawType =
                (DotNetRawType)this._typeToolingEnvironment.GetRequiredRawType("ReactiveData.Sequence.SequenceUtils");
            Type sequenceUtilsType = ((ReflectionDotNetRawType) sequenceUtilsRawType).Type;

            foreach (var propertyValue in propertyValues)
            {
                DotNetRawProperty rawProperty = (DotNetRawProperty)propertyValue.Property;

                if (!rawProperty.CanWrite)
                {
                    DotNetRawType iListRawType = (DotNetRawType)this._typeToolingEnvironment.GetRequiredRawType("System.Collections.IList");

                    if (rawProperty.PropertyType.IsAssignableTo(iListRawType))
                    {
                        Expression propertyExpression = Expression.Property(objVariableExpression, rawProperty.Name);

                        if (!(propertyValue.Value is NewSequenceCode newSequenceCode))
                            throw new InvalidOperationException($"Property {rawProperty.Name} isn't set via NewSequenceCode");

                        Expression sequenceExpression = this.ConvertExpression(newSequenceCode);

                        RawType sequenceElementType = newSequenceCode.ElementType;
                        Type sequenceElementDotNetType = ((ReflectionDotNetRawType) sequenceElementType).Type;

                        MethodCallExpression listOnSequenceExpression = Expression.Call(sequenceUtilsType, "NonGenericIListOnSequence",
                            new [] { sequenceElementDotNetType }, propertyExpression, sequenceExpression);
                        blockExpressions.Add(listOnSequenceExpression);
                    }
                }
            }

            blockExpressions.Add(objVariableExpression);

            return Expression.Block(objectType, blockVariables, blockExpressions);
        }

        private NewArrayExpression ConvertNewArrayInit(NewArrayInitCode newArrayInitCode)
        {

            var itemExpressions = ImmutableArray.CreateBuilder<Expression>();
            foreach (ExpressionCode item in newArrayInitCode.Items)
            {
                Expression itemExpression = this.ConvertExpression(item);
                itemExpressions.Add(itemExpression);
            }

            ReflectionDotNetRawType elementType = (ReflectionDotNetRawType) newArrayInitCode.ElementType;
            return Expression.NewArrayInit(elementType.Type, itemExpressions.ToArray());
        }

        private Expression ConvertNewSequence(NewSequenceCode newSequenceCode)
        {
            DotNetRawType sequenceUtils = (DotNetRawType)this._typeToolingEnvironment.GetRequiredRawType("ReactiveData.Sequence.SequenceUtils");

            RawType elementType = newSequenceCode.ElementType;
            var itemsArrayCode = new NewArrayInitCode(elementType, newSequenceCode.Items);

            DotNetRawMethod itemsMethod = sequenceUtils.GetRequiredMethod("Items");
            GenericMethodCallCode itemsMethodCallCode = DotNetCode.CallStaticGeneric(new[] { (DotNetRawType) elementType }, itemsMethod, itemsArrayCode);

            return this.ConvertExpression(itemsMethodCallCode);
        }

        private MethodCallExpression ConvertMethodCall(MethodCallCode methodCallCode)
        {
            MethodInfo methodInfo = GetMethodInfo(methodCallCode.RawMethod);
            ExpressionCode[] argumentsCode = methodCallCode.Arguments;

            List<Expression> argumentsExpressions = new List<Expression>();
            foreach (ExpressionCode argumentCode in argumentsCode)
                argumentsExpressions.Add(this.ConvertExpression(argumentCode));

            // Handle both static and instance method calls
            ExpressionCode? objectCode = methodCallCode.ObjectCode;
            if (objectCode == null)
                return Expression.Call(methodInfo, argumentsExpressions);
            else return Expression.Call(this.ConvertExpression(objectCode), methodInfo, argumentsExpressions);
        }

        private Expression ConvertGenericMethodCall(GenericMethodCallCode genericMethodCallCode)
        {
            Type[] typeArguments = genericMethodCallCode.GenericTypeArguments.Select(type => ((ReflectionDotNetRawType) type).Type).ToArray();

            MethodInfo methodInfo = GetMethodInfo(genericMethodCallCode.RawMethod);
            MethodInfo genericMethodInfo = methodInfo.MakeGenericMethod(typeArguments);

            ExpressionCode[] argumentsCode = genericMethodCallCode.Arguments;

            List<Expression> argumentsExpressions = new List<Expression>();
            foreach (ExpressionCode argumentCode in argumentsCode)
                argumentsExpressions.Add(this.ConvertExpression(argumentCode));

            // Handle both static and instance method calls
            ExpressionCode? objectCode = genericMethodCallCode.ObjectCode;
            if (objectCode == null)
                return Expression.Call(genericMethodInfo, argumentsExpressions);
            else return Expression.Call(this.ConvertExpression(objectCode), genericMethodInfo, argumentsExpressions);
        }

        private InvocationExpression ConvertFunctionDelegateInvoke(FunctionDelegateInvokeCode functionDelegateInvokeCode)
        {
            Expression<Func<string, int, bool>> stringLengthMin = (value, min) => value.Length >= min;

            ExpressionCode functionDelegateCode = functionDelegateInvokeCode.FunctionDelegate;

            Expression functionDelegateExpression;
            if (functionDelegateCode is FunctionDelegateHolderCode functionDelegateHolderCode)
            {
                Delegate? functionDelegate = functionDelegateHolderCode.FunctionDelegateHolder.FunctionDelegate;
                if (functionDelegate == null)
                    throw new InvalidOperationException("Recursive calls (direct or indirect) aren't yet supported");

                functionDelegateExpression = Expression.Constant(functionDelegate);

                // TODO: Support getting delegate at runtime from the delegate holder, which I believe will work if it's cast to the right
                // Func type. Use MakeGenericType to create a Func with the right type params, then Expression.Convert to cast
#if LATER
                ConstantExpression functionDelegateHolderExpression = Expression.Constant(functionDelegateHolderCode.FunctionDelegateHolder);
                MemberInfo functionDelegateMemberInfo = typeof(FunctionDelegateHolder).GetMember(nameof(FunctionDelegateHolder.FunctionDelegate))[0];
                functionDelegateExpression = Expression.MakeMemberAccess(functionDelegateHolderExpression, functionDelegateMemberInfo);
#endif
            }
            else functionDelegateExpression = this.ConvertExpression(functionDelegateCode);

            List<Expression> argumentsExpressions = new List<Expression>();
            foreach (ExpressionCode argumentCode in functionDelegateInvokeCode.Arguments)
                argumentsExpressions.Add(this.ConvertExpression(argumentCode));

            return Expression.Invoke(functionDelegateExpression, argumentsExpressions);
        }

        private BinaryExpression ConvertBinaryExpression(BinaryExpressionCode binaryExpressionCode)
        {
            BinaryOperator binaryOperator = binaryExpressionCode.Operator;

            ExpressionType expressionType;
            if (binaryOperator == BinaryOperator.And)
                expressionType = ExpressionType.AndAlso;
            else if (binaryOperator == BinaryOperator.Or)
                expressionType = ExpressionType.OrElse;
            else if (binaryOperator == BinaryOperator.LessThan)
                expressionType = ExpressionType.LessThan;
            else if (binaryOperator == BinaryOperator.LessThanOrEqual)
                expressionType = ExpressionType.LessThanOrEqual;
            else if (binaryOperator == BinaryOperator.GreaterThan)
                expressionType = ExpressionType.GreaterThan;
            else if (binaryOperator == BinaryOperator.GreaterThanOrEqual)
                expressionType = ExpressionType.GreaterThanOrEqual;
            else if (binaryOperator == BinaryOperator.Add)
                expressionType = ExpressionType.Add;
            else if (binaryOperator == BinaryOperator.Subtract)
                expressionType = ExpressionType.Subtract;
            else if (binaryOperator == BinaryOperator.Multiply)
                expressionType = ExpressionType.Multiply;
            else if (binaryOperator == BinaryOperator.Equals)
                expressionType = ExpressionType.Equal;
            else if (binaryOperator == BinaryOperator.NotEquals)
                expressionType = ExpressionType.NotEqual;
            else
                throw new Exception("Unknown infix operator: " + binaryOperator.DefaultStringRepresentation);

            Expression leftOperandExpression = this.ConvertExpression(binaryExpressionCode.LeftOperand);
            Expression rightOperandExpression = this.ConvertExpression(binaryExpressionCode.RightOperand);

            return Expression.MakeBinary(expressionType, leftOperandExpression, rightOperandExpression);
        }

        private UnaryExpression ConvertUnaryExpression(UnaryExpressionCode unaryExpressionCode)
        {
            UnaryOperator unaryOperator = unaryExpressionCode.Operator;

            ExpressionType expressionType;
            if (unaryOperator == UnaryOperator.Not)
                expressionType = ExpressionType.Not;
            else
                throw new Exception("Unknown infix operator: " + unaryOperator.DefaultStringRepresentation);

            Expression operandExpression = this.ConvertExpression(unaryExpressionCode.Operand);
            return Expression.MakeUnary(expressionType, operandExpression, null);
        }

        private MemberExpression ConvertGetProperty(GetPropertyCode getPropertyCode)
        {
            if (!(getPropertyCode.Property is ReflectionDotNetRawProperty reflectionDotNetRawProperty))
                throw new Exception($"Enum type is {getPropertyCode.Property.GetType().FullName}, not a ReflectionDotNetRawProperty as expected");

            return Expression.MakeMemberAccess(this.ConvertExpressionOrNull(getPropertyCode.ObjectExpression), reflectionDotNetRawProperty.PropertyInfo);
        }

        private static ConstructorInfo GetConstructorInfo(RawConstructor rawConstructor)
        {
            if (!(rawConstructor is ReflectionDotNetRawConstructor reflectionDotNetRawConstructor))
            {
                throw new Exception(
                    $"rawConstructor must be of type ReflectionDotNetRawConstructor in order to create an expression, but it's actually of type '{rawConstructor.GetType().FullName}'");
            }

            return reflectionDotNetRawConstructor.ConstructorInfo;
        }

        private static MethodInfo GetMethodInfo(RawMethod rawMethod)
        {
            if (!(rawMethod is ReflectionDotNetRawMethod reflectionDotNetRawMethod))
            {
                throw new Exception(
                    $"rawMethod must be of type ReflectionDotNetRawMethod in order to create an expression, but it's actually of type '{rawMethod.GetType().FullName}'");
            }

            return reflectionDotNetRawMethod.MethodInfo;
        }

        private static MemberInfo GetMemberInfo(RawProperty rawProperty)
        {
            if (!(rawProperty is ReflectionDotNetRawProperty reflectionDotNetRawProperty))
                throw new Exception(
                    $"rawProperty must be of type ReflectionDotNetRawProperty in order to create an expression, but it's actually of type '{rawProperty.GetType().FullName}'");
            }

            return reflectionDotNetRawProperty.PropertyInfo;
        }

        private static Type GetType(RawType rawType)
        {
            if (!(rawType is ReflectionDotNetRawType reflectionDotNetRawType))
            {
                throw new Exception(
                    $"rawType must be of type ReflectionDotNetRawType in order to create an expression, but it's actually of type '{rawType.GetType().FullName}'");
            }

            return reflectionDotNetRawType.Type;
        }
    }
}
