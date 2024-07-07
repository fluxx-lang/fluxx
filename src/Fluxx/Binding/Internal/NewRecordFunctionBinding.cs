using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Faml.Binding.Internal {
    public sealed class NewRecordFunctionBinding : FunctionBinding {
        private readonly RecordTypeDefinitionSyntax _recordTypeDefinition;

        public NewRecordFunctionBinding(RecordTypeDefinitionSyntax recordTypeDefinition) {
            this._recordTypeDefinition = recordTypeDefinition;
        }

        public override QualifiableName FunctionName => this._recordTypeDefinition.TypeName.ToQualifiableName();

        public RecordTypeDefinitionSyntax RecordTypeDefinition => this._recordTypeDefinition;

        public override TypeBinding ReturnTypeBinding => this._recordTypeDefinition.TypeBinding;

        public override TypeBinding? GetParameterTypeBinding(Name parameterName) {
            return this._recordTypeDefinition.GetPropertyTypeBinding(parameterName);
        }

        public override TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair,
            BindingResolver bindingResolver) {
            if (argumentName.IsQualified()) {
                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    "Record types don't support attached properties");
                return InvalidTypeBinding.Instance;
            }

            Name unqualifiableArgumentName = argumentName.ToUnqualifiableName();

            TypeBinding typeBinding = this._recordTypeDefinition.GetPropertyTypeBinding(unqualifiableArgumentName);
            if (typeBinding == null) {
                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    $"No '{argumentName}' property exists for '{this._recordTypeDefinition.TypeName}'");
                return InvalidTypeBinding.Instance;
            }

            return typeBinding;;
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver) {
            contentArgument.AddError("Record types don't support content properties");
            return InvalidTypeBinding.Instance;
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken) {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override string GetNoContentPropertyExistsError() {
            return $"Use of unnamed parameter not allowed. Data can't have content properties.";
        }

        public override Name? GetThisParameter() {
            return null;
        }

        public override Name? GetContentProperty() {
            return null;
        }

        /// <summary>
        /// Return all the parameter names, in their "natural" order.  More info about parameters (e.g. their types) can be queried by methods above.
        /// </summary>
        /// <returns>parameter names</returns>
        public override Name[] GetParameters() {
            return this._recordTypeDefinition.GetProperties();
        }
    }
}
