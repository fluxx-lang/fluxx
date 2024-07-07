using System.Collections.Immutable;
using Faml.Api;
using Faml.Binding.External;
using Faml.Binding.Internal;
using Faml.Syntax;
using Faml.Syntax.Type;

namespace Faml.Binding.Resolver {
    public class ModuleBindingResolver : BindingResolver {
        private readonly ModuleSyntax _module;
        private readonly FamlProject _project;

        public ModuleBindingResolver(ModuleSyntax module) {
            this._module = module;
            this._project = module.Project;
        }

        public override FunctionBinding ResolveFunctionBinding(TypeBinding? thisArgumentTypeBinding,
            QualifiableName functionName, SyntaxNode nameSyntaxForErrors) {
            // TODO: Handle "this" (differently than here, treating it as a function reference property)
#if LATER
            if (thisArgumentTypeBinding != null) {
                FunctionBinding functionBinding = thisArgumentTypeBinding.GetMethodBinding(functionName);

                if (functionBinding != null)
                    return functionBinding;

                var qualifiedFunctionName = new QualifiableName(thisArgumentTypeBinding.TypeName + "." + functionName);

                nameSyntaxForErrors.AddError($"Method function '{qualifiedFunctionName}' not found");
                return new InvalidFunctionBinding(qualifiedFunctionName);
            }
#endif

            if (!functionName.IsQualified()) {
                Name unqualifiableName = functionName.ToUnqualifiableName();

                FunctionDefinitionSyntax? functionDefinition = this._module.GetFunctionDefinition(unqualifiableName);
                if (functionDefinition != null)
                    return new InternalFunctionBinding(functionDefinition);

                RecordTypeDefinitionSyntax? recordTypeDefinition = this._module.GetRecordTypeDefinition(unqualifiableName);
                if (recordTypeDefinition != null)
                    return new NewRecordFunctionBinding(recordTypeDefinition);

                // These function names are special
                if (unqualifiableName.ToString() == "example")
                    return new NewExternalObjectFunctionBinding(this._project.ExampleTypeBinding);
                if (unqualifiableName.ToString() == "examples")
                    return new NewExternalObjectFunctionBinding(this._project.ExamplesTypeBinding);
            }

            // Treat function name to a type name and see if that type exists, as it may be a
            // constructor function
            TypeBindingResult typeBindingResult = this.FindTypeBindingForType(functionName);

            if (typeBindingResult is TypeBindingResult.Success success) {
                if (success.TypeBinding is ExternalObjectTypeBinding externalObjectTypeBinding)
                    return new NewExternalObjectFunctionBinding(externalObjectTypeBinding);
                else if (success.TypeBinding is BuiltInTypeBinding predefinedTypeBinding)
                    return new NewPredefinedTypeFunctionBinding(predefinedTypeBinding);
                else return new InvalidFunctionBinding(functionName);
            }
            else {
                nameSyntaxForErrors.AddError(typeBindingResult.GetNotFoundOrOtherErrorMessage($"Function '{functionName}' not found"));
                return new InvalidFunctionBinding(functionName);
            }
        }

        public override TypeBinding ResolveObjectTypeBinding(ObjectTypeReferenceSyntax objectTypeReferenceSyntax) {
            QualifiableName typeName = objectTypeReferenceSyntax.TypeName;
            TypeBindingResult typeBindingResult = this.FindTypeBindingForType(typeName);

            if (typeBindingResult is TypeBindingResult.Success success)
                return success.TypeBinding;
            else {
                objectTypeReferenceSyntax.AddError(
                    typeBindingResult.GetNotFoundOrOtherErrorMessage($"Type '{typeName}' not found"));
                return new InvalidTypeBinding(typeName);
            }
        }

        public override TypeBindingResult FindTypeBindingForType(QualifiableName typeName) {
            if (typeName.IsQualified())
                return this._project.ResolveTypeBinding(typeName);

            Name unqualifiableTypeName = typeName.ToUnqualifiableName();

            BuiltInTypeBinding? predefinedTypeBinding = BuiltInTypeBinding.GetBindingForTypeName(unqualifiableTypeName.ToString());
            if (predefinedTypeBinding != null)
                return new TypeBindingResult.Success(predefinedTypeBinding);

            RecordTypeDefinitionSyntax? recordTypeDefinition = this._module.GetRecordTypeDefinition(unqualifiableTypeName);
            if (recordTypeDefinition != null)
                return new TypeBindingResult.Success(recordTypeDefinition.TypeBinding);

            foreach (ImportSyntax import in this._module.Imports) {
                ImmutableArray<ImportTypeReferenceSyntax>? importImportTypeReferences = import.ImportTypeReferences;

                if (importImportTypeReferences == null) {
                    // If importing all types, combine the namespace qualifier with this name and see if that's a valid
                    // type. If so, match on it.

                    var potentialQualifiedTypeName = new QualifiableName(import.Qualifier, unqualifiableTypeName);
                    TypeBindingResult typeBindingResult = this._project.ResolveTypeBinding(potentialQualifiedTypeName);

                    // If we found something or got an error, return that
                    if (! (typeBindingResult is TypeBindingResult.NotFound))
                        return typeBindingResult;
                }
                else {
                    // If just importing specified types, see if any of them match this name

                    foreach (ImportTypeReferenceSyntax importReference in importImportTypeReferences) {
                        if (importReference.Name == unqualifiableTypeName)
                            return new TypeBindingResult.Success(importReference.GetTypeBinding());
                    }
                }
            }

            return TypeBindingResult.NotFoundResult;
        }

        public override AttachedTypeBinding? ResolveAttachedTypeBinding(QualifiableName typeName) {
            if (typeName.IsQualified())
                return this._project.ResolveAttachedTypeBinding(typeName);

            Name unqualifiableTypeName = typeName.ToUnqualifiableName();

            foreach (ImportSyntax import in this._module.Imports) {
                ImmutableArray<ImportTypeReferenceSyntax>? importImportTypeReferences = import.ImportTypeReferences;

                if (importImportTypeReferences == null) {
                    // If importing all types, combine the namespace qualifier with this name and see if that's a valid
                    // type. If so, match on it.

                    var potentialQualifiedTypeName = new QualifiableName(import.Qualifier, unqualifiableTypeName);
                    AttachedTypeBinding? typeBinding = this._project.ResolveAttachedTypeBinding(potentialQualifiedTypeName);

                    if (typeBinding != null)
                        return typeBinding;
                }
                else {
                    // If just importing specified types, see if any of them match this name

                    foreach (ImportTypeReferenceSyntax importReference in importImportTypeReferences) {
                        if (importReference.Name == unqualifiableTypeName)
                            return importReference.GetAttachedTypeBinding();
                    }
                }
            }

            return null;
        }
    }
}
