using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;

namespace Faml.Binding {
    public class InvalidFunctionBinding : FunctionBinding {
        private readonly QualifiableName _functionName;

        public override QualifiableName FunctionName => _functionName;

        public InvalidFunctionBinding(QualifiableName functionName) {
            _functionName = functionName; 
        }

        public override TypeBinding ReturnTypeBinding => InvalidTypeBinding.Instance;

        public override TypeBinding? GetParameterTypeBinding(Name parameterName) {
            return null;
        }

        public override TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair,
            BindingResolver bindingResolver) {
            return InvalidTypeBinding.Instance;
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver) {
            return InvalidTypeBinding.Instance;
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken) {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override string GetNoContentPropertyExistsError() {
            return "Use of unnamed property not allowed.";
        }

        public override Name? GetThisParameter() {
            return null;
        }

        public override Name? GetContentProperty() {
            return null;
        }

        public override Name[] GetParameters() {
            return new Name[0];
        }
    }
}
