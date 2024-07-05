namespace Faml.Syntax {
    public enum SyntaxNodeType {
        BracedExpression,
        FunctionInvocation,
        IfExpression,
        InfixExpression,
        ForExpression,
        ForVariableDefinition,
        InterpolatedStringExpression,
        InterpolatedStringFragment,

        InvalidExpression,
        SequenceExpression,
        ParenthesizedExpression,
        PrefixExpression,
        PropertyAccess,
        SymbolReference,

        BooleanLiteral,
        DoubleLiteral,
        IntLiteral,
        NullLiteral,
        StringLiteral,
        ExternalTypeLiteral,
        EnumValueLiteral,

        TextualLiteralTextItem,
        TextualLiteralExpressionItem,
        TextualLiteral,

        NameIdentifier,
        QualifiableName,
        QualifiedNameComponent,

        PredefinedTypeReference,
        ObjectTypeReference,
        SequenceTypeReference,
        InvalidTypeReference,

        ArgumentNameValuePair,
        PropertySpecifier,
        ConditionValuePair,
        FunctionDefinition,
        ExampleDefinition,
        ExamplesDefinition,
        Import,
        ImportReference,
        Use,
        Module,
        PropertyNameTypePair,
        RecordTypeDefinition,

        DotNetEnumValue,
    }
}
