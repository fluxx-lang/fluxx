using System;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;

namespace Faml.Binding.Internal
{
    public class NewPredefinedTypeFunctionBinding : FunctionBinding
    {
        private static readonly Name ContentParameter = new Name("Content");

        private readonly BuiltInTypeBinding _predefinedTypeBinding;

        public NewPredefinedTypeFunctionBinding(BuiltInTypeBinding predefinedTypeBinding)
        {
            this._predefinedTypeBinding = predefinedTypeBinding;
        }

        public override QualifiableName FunctionName => this._predefinedTypeBinding.TypeName;

        public override TypeBinding ReturnTypeBinding => this._predefinedTypeBinding;

        public override TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair,
            BindingResolver bindingResolver)
            {
            if (!argumentName.IsQualified())
            {
                TypeBinding? typeBinding = this.GetParameterTypeBinding(argumentName.ToUnqualifiableName());
                if (typeBinding != null)
                {
                    return typeBinding;
                }
            }

            argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                $"'{argumentName}' parameter isn't valid for function '{this.FunctionName}'. ");
            return InvalidTypeBinding.Instance;
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver)
        {
            return this._predefinedTypeBinding;
        }

        public override TypeBinding? GetParameterTypeBinding(Name parameterName)
        {
            string parameterNameString = parameterName.ToString();
            if (parameterNameString == "content" || parameterNameString == "Content")
            {
                return this._predefinedTypeBinding;
            }

            return null;
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken)
            {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override string GetNoContentPropertyExistsError()
        {
            throw new InvalidOperationException("Should not be called; primitive types always have a content parameter");
        }

        public override Name? GetThisParameter()
        {
            // TODO: Support 'object' or maybe 'this' keyword to specify object property
            return null;
        }

        public override Name? GetContentProperty()
        {
            return ContentParameter;
        }

        /// <summary>
        /// Return all the parameter names, in their "natural" order.  More info about parameters (e.g. their types) can be queried by methods above.
        /// </summary>
        /// <returns>parameter names</returns>
        public override Name[] GetParameters()
        {
            return new[] {ContentParameter};
        }
    }
}
