using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding;
using Faml.CodeAnalysis.Text;
using Faml.Syntax;
using Faml.Syntax.Expression;
using TypeTooling.ClassifiedText;

namespace Faml.IntelliSense {
    public class IntelliSenseProvider {
        public delegate Task<ClassifiedTextMarkup?> GetDescriptionAsyncDelegate(CultureInfo preferredCulture, CancellationToken cancellationToken);

        public static IntelliSense? GetIntelliSense(FamlModule module, int position) {
            ModuleSyntax moduleSyntax = module.ModuleSyntax;

            // If the position is out of range (e.g. past the end of the module source) then
            // don't provide IntelliSense
            if (!moduleSyntax.Span.ContainsInclusiveEnd(position)) {
                // Hack for now
                return new FunctionInvocationIntelliSense(module, position);
                //return null;
            }

            SyntaxNode node = moduleSyntax.GetNodeAtPosition(position);

            SyntaxNode? previousTerminalNode = null;
            if (!node.IsTerminalNode())
            {
                previousTerminalNode = moduleSyntax.GetPreviousTerminalNodeFromPosition(position);
            }

            // If the caret is at the end of a property specifier (right after the colon) then we want to show
            // IntelliSense for the value that comes after. That's an exception to the normal rule that
            // IntelliSense is end inclusive.
            {
                if (node is QualifiableNameSyntax name &&
                    name.Parent is PropertySpecifierSyntax propertySpecifier &&
                    position == node.Span.End) {
                    previousTerminalNode = node;
                    node = propertySpecifier;
                }
            }

            // Check if the caret is on a function invocation argument name
            {
                if (node is QualifiableNameSyntax name &&
                    name.Parent is PropertySpecifierSyntax propertySpecifier &&
                    propertySpecifier.Parent is ArgumentNameValuePairSyntax argumentNameValuePair &&
                    argumentNameValuePair.Parent is FunctionInvocationSyntax functionInvocation) {

                    FunctionBinding functionBinding = functionInvocation.FunctionBinding;
                    if (functionBinding is InvalidFunctionBinding)
                    {
                        return null;
                    }

                    // If the caret is at the start of the property, then IntelliSense should add a new
                    // property, not update the existing one
                    if (propertySpecifier.Span.Start == position)
                    {
                        return new ArgumentNameIntelliSense(module, position, functionBinding);
                    }

                    return new ArgumentNameIntelliSense(module, position, functionBinding,
                        propertySpecifier.PropertyNameSyntax);
                }
            }

            // Check if the caret is on a function invocation, but not on any argument nor the function name itself
            {
                if (node is FunctionInvocationSyntax functionInvocation) {
                    FunctionBinding functionBinding = functionInvocation.FunctionBinding;
                    if (functionBinding is InvalidFunctionBinding)
                    {
                        return null;
                    }

                    return new ArgumentNameIntelliSense(module, position, functionBinding);
                }
            }

            // Check if the caret isn't on a terminal node but is right after an argument property specifier
            {
                if (previousTerminalNode is QualifiableNameSyntax name &&
                    name.Parent is PropertySpecifierSyntax propertySpecifier &&
                    propertySpecifier.Parent is ArgumentNameValuePairSyntax argumentNameValuePair) {

                    TypeBinding parameterTypeBinding = argumentNameValuePair.ParameterTypeBinding;
                    if (parameterTypeBinding is InvalidTypeBinding)
                    {
                        return null;
                    }

                    return new ValueIntelliSense(module, position, parameterTypeBinding);
                }
            }

            return null;
        }

        public static Task<ClassifiedTextMarkup?> GetDescriptionAsync(object completionItemData, CultureInfo preferredCulture,
            CancellationToken cancellationToken) {
            GetDescriptionAsyncDelegate getDescriptionDelegate = (GetDescriptionAsyncDelegate) completionItemData;
            return getDescriptionDelegate.Invoke(preferredCulture, cancellationToken);
        }
    }
}
