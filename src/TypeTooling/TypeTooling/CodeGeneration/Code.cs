using System.Collections.Immutable;
using TypeTooling.CodeGeneration.Expressions;
using TypeTooling.CodeGeneration.Expressions.Literals;
using TypeTooling.CodeGeneration.Operators;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.RawTypes;

namespace TypeTooling.CodeGeneration
{
    public class Code {
        public static StringLiteralCode StringLiteral(string value) {
            return new StringLiteralCode(value);
        }

        public static BooleanLiteralCode BooleanLiteral(bool value) {
            return new BooleanLiteralCode(value);
        }

        public static IntLiteralCode IntLiteral(int value) {
            return new IntLiteralCode(value);
        }

        public static DoubleLiteralCode DoubleLiteral(double value) {
            return new DoubleLiteralCode(value);
        }

        public static ByteLiteralCode ByteLiteral(byte value) {
            return new ByteLiteralCode(value);
        }

        public static EnumValueLiteralCode EnumValueLiteral(RawType enumType, string enumValueName) {
            return new EnumValueLiteralCode(enumType, enumValueName);
        }

        public static BinaryExpressionCode BinaryExpression(BinaryOperator binaryOperator, ExpressionCode leftOperand, ExpressionCode rightOperand) {
            return new BinaryExpressionCode(binaryOperator, leftOperand, rightOperand);
        }

        public static UnaryExpressionCode UnaryExpression(UnaryOperator unaryOperator, ExpressionCode operand) {
            return new UnaryExpressionCode(unaryOperator, operand);
        }

        public static ExpressionAndHelpersCode ExpressionAndHelpers(ExpressionCode expressionCode) {
            return new ExpressionAndHelpersCode(expressionCode);
        }

        public static MethodCallCode Call(ExpressionCode instance, RawMethod method, params ExpressionCode[] arguments) {
            return new MethodCallCode(instance, method, arguments);
        }

        public static MethodCallCode CallStatic(RawMethod method, params ExpressionCode[] arguments) {
            return new MethodCallCode(null, method, arguments);
        }

        public static FunctionDelegateInvokeCode Invoke(ExpressionCode functionDelegate, ImmutableArray<ExpressionCode> arguments) {
            return new FunctionDelegateInvokeCode(functionDelegate, arguments);
        }

        public static ParameterExpressionCode Parameter(string name, RawType type) {
            return new ParameterExpressionCode(name, type);
        }

        public static LambdaCode Lambda(string name, ImmutableArray<ParameterExpressionCode> parameters, ExpressionCode body) {
            return new LambdaCode(name, parameters, body);
        }

        public static NewSequenceCode NewSequence(RawType elementType, ImmutableArray<ExpressionCode> items) {
            return new NewSequenceCode(elementType, items);
        }
    }
}
