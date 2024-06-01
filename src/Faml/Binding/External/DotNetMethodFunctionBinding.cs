using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes;

namespace Faml.Binding.External {
    public sealed class DotNetMethodFunctionBinding : FunctionBinding {
        private readonly ExternalObjectTypeBinding _objectTypeBinding;
        private readonly DotNetRawMethod _rawMethod;
        private readonly TypeBinding _returnTypeBinding;


        public DotNetMethodFunctionBinding(ExternalObjectTypeBinding objectTypeBinding, DotNetRawMethod rawMethod) {
            _objectTypeBinding = objectTypeBinding;
            _rawMethod = rawMethod;

            _returnTypeBinding = ExternalBindingUtil.DotNetTypeToTypeBinding(objectTypeBinding.Project, rawMethod.ReturnType);
        }

        public override QualifiableName FunctionName => new QualifiableName(_objectTypeBinding.TypeName + "." + _rawMethod.Name);

        public override TypeBinding ReturnTypeBinding => _returnTypeBinding;

        public override TypeBinding? GetParameterTypeBinding(Name parameterName) {
            string parameterNameString = parameterName.ToString();
            foreach (DotNetRawParameter parameter in _rawMethod.GetParameters()) {
                if (parameter.Name == parameterNameString)
                    return ExternalBindingUtil.DotNetTypeToTypeBinding(_objectTypeBinding.Project, parameter.ParameterType);
            }

            return null;
        }

        public override TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair,
            BindingResolver bindingResolver) {
            if (argumentName.IsQualified()) {
                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    "C# methods don't support attached properties for parameters");
                return InvalidTypeBinding.Instance;
            }

            string parameterNameString = argumentName.ToString();
            foreach (DotNetRawParameter parameter in _rawMethod.GetParameters()) {
                if (parameter.Name == parameterNameString)
                    return ExternalBindingUtil.DotNetTypeToTypeBinding(_objectTypeBinding.Project, parameter.ParameterType);
            }

            argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                $"No '{argumentName}' parameter exists for method '{_rawMethod.Name}'");
            return InvalidTypeBinding.Instance;
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver) {
            contentArgument.AddError($".NET methods don't currently support content (unnamed) arguments");
            return InvalidTypeBinding.Instance;
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken) {
            return Task.FromResult((ClassifiedTextMarkup?) null);
        }

        public override string GetNoContentPropertyExistsError() {
            return $"Use of unnamed property not allowed. No content parameter exists for method '{_rawMethod.Name}'";
        }

        public DotNetRawMethod RawMethod => _rawMethod;

        public override Name? GetThisParameter() {
            // TODO: Do the right thing here
            return null;
        }

        public override Name? GetContentProperty() {
            // TODO: Do the right thing here
            return null;
        }

        public override Name[] GetParameters() {
            List<Name> parameterNames = new List<Name>();
            foreach (DotNetRawParameter parameter in _rawMethod.GetParameters())
                parameterNames.Add(new Name(parameter.Name));

            return parameterNames.ToArray();
        }
    }
}
