/**
 * @author Bret Johnson
 * @since 6/5/2015
 */

using System;
using Faml.Api;
using Faml.Binding.External;
using Faml.Syntax;
using Faml.Syntax.Type;

namespace Faml.Binding.Resolver {
    public class ProjectDefinitionBindingResolver : BindingResolver {
        private readonly FamlProject _project;

        public ProjectDefinitionBindingResolver(FamlProject project) {
            this._project = project;
        }

        public override TypeBindingResult FindTypeBindingForType(QualifiableName typeName) {
            return TypeBindingResult.NotFoundResult;
        }

        public override FunctionBinding ResolveFunctionBinding(TypeBinding? thisArgumentTypeBinding,
            QualifiableName functionName, SyntaxNode nameSyntaxForErrors) {
            // TODO: Handle "this" (differently than here, treating it as a function reference property)
#if LATER
            if (thisArgumentTypeBinding != null) {
                FunctionBinding functionBinding = thisArgumentTypeBinding.GetMethodBinding(functionName);

                if (functionBinding != null)
                    return functionBinding;

                var qualifedFunctionName = new QualifiableName(thisArgumentTypeBinding.TypeName + "." + functionName);

                functionNameIdentifier.AddError($"Method function '{qualifedFunctionName}' not found");
                return new InvalidFunctionBinding(qualifedFunctionName);
            }
#endif

            if (functionName.IsQualified()) {
                nameSyntaxForErrors.AddError("Qualified names are not currently supported in project definitions");
                return new InvalidFunctionBinding(functionName);
            }

            Name unqualifiableName = functionName.ToUnqualifiableName();

            // TODO: Fix this up; should Faml prefix always be here?
            // Convert function name to a type name, as it may be a constructor function; then see if it matches any
            // imports
            string potentialTypeName = unqualifiableName.GetPascalCase();
            var className = new QualifiableName("Faml.ProjectTypes." + potentialTypeName);
            TypeBindingResult typeBindingResult = this._project.ResolveTypeBinding(className);

            if (typeBindingResult is TypeBindingResult.Success success) {
                if (success.TypeBinding is ExternalObjectTypeBinding externalObjectTypeBinding)
                {
                    return new NewExternalObjectFunctionBinding(externalObjectTypeBinding);
                }
                else
                {
                    return new InvalidFunctionBinding(functionName);
                }
            }
            else {
                string message =
                    typeBindingResult.GetNotFoundOrOtherErrorMessage($"Function '{functionName}' not found");
                nameSyntaxForErrors.AddError(message);
                return new InvalidFunctionBinding(functionName);
            }
        }

        // TODO: Implement this
        public override TypeBinding ResolveObjectTypeBinding(ObjectTypeReferenceSyntax objectTypeReferenceSyntax) {
            QualifiableName typeName = objectTypeReferenceSyntax.TypeName;
            objectTypeReferenceSyntax.AddError($"Type '{typeName}' not found");
            return new InvalidTypeBinding(typeName);
        }

        public override AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName typeName) {
            throw new Exception("Attached types aren't supported in the Project definition");
        }
    }
}
