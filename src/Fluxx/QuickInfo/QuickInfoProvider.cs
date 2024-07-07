using Fluxx.Api;
using Fluxx.Api.QuickInfo;
using Fluxx.Binding;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;

namespace Fluxx.QuickInfo
{
    public class QuickInfoProvider
    {
        public static Api.QuickInfo.QuickInfo? GetQuickInfo(SyntaxNode syntaxNode)
        {
            if (syntaxNode.Parent is SymbolReferenceSyntax parentSymbolReferenceSyntax)
            {
                syntaxNode = parentSymbolReferenceSyntax;
            }

            SyntaxNode parentNode = syntaxNode.Parent;

            if (parentNode is FunctionInvocationSyntax functionInvocationSyntax && ReferenceEquals(functionInvocationSyntax.FunctionReference, syntaxNode))
            {
                FunctionBinding functionBinding = functionInvocationSyntax.FunctionBinding;
                if (functionBinding == null)
                {
                    return null;
                }

                QualifiableName? returnTypeName = functionBinding.ReturnTypeBinding.TypeName;
                if (returnTypeName == functionBinding.FunctionName)
                {
                    returnTypeName = null;
                }

                return new FunctionInvocationQuickInfo(syntaxNode.Span, functionBinding.FunctionName, returnTypeName);
            }
            else
            {
                return null;
            }
        }
    }
}
