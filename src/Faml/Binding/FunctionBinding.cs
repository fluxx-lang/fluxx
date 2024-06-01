using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;

namespace Faml.Binding {
    public abstract class FunctionBinding {
        public abstract QualifiableName FunctionName { get; }

        public abstract TypeBinding ReturnTypeBinding { get; }

        public abstract TypeBinding? GetParameterTypeBinding(Name parameterName);

        public abstract TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair, BindingResolver bindingResolver);

        public abstract TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver);

        public abstract Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken);

        public abstract string GetNoContentPropertyExistsError();

        public abstract Name[] GetParameters();

        public abstract Name? GetThisParameter();

        public abstract Name? GetContentProperty();
    }
}
