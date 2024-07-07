using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Binding.Resolver;
using Fluxx.Syntax;
using TypeTooling.ClassifiedText;

/**
 * @author Bret Johnson
 * @since 4/15/2015
 */
namespace Fluxx.Binding.Internal
{
    public class InternalFunctionBinding : FunctionBinding
    {
        public FunctionDefinitionSyntax FunctionDefinition { get; }
        public FamlModule Module { get; }

        public InternalFunctionBinding(FunctionDefinitionSyntax functionDefinition)
        {
            this.Module = functionDefinition.GetModule();
            this.FunctionDefinition = functionDefinition;
        }

        public override QualifiableName FunctionName => this.FunctionDefinition.FunctionName.ToQualifiableName();

        public override TypeBinding ReturnTypeBinding => this.FunctionDefinition.ReturnTypeBinding;

        public override TypeBinding? GetParameterTypeBinding(Name parameterName)
        {
            int parameterIndex = this.FunctionDefinition.GetParameterIndex(parameterName);
            if (parameterIndex == -1)
            {
                return null;
            }

            return this.FunctionDefinition.GetParameterTypeBinding(parameterIndex);
        }

        public override TypeBinding ResolveArgumentTypeBinding(QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair,
            BindingResolver bindingResolver)
            {
            if (argumentName.IsQualified())
            {
                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    "FAML functions don't support attached properties for parameters");
                return InvalidTypeBinding.Instance;
            }

            Name unqualifiableArgumentName = argumentName.ToUnqualifiableName();

            int parameterIndex = this.FunctionDefinition.GetParameterIndex(unqualifiableArgumentName);
            if (parameterIndex == -1)
            {
                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    $"No '{argumentName}' parameter exists for function '{this.FunctionDefinition.FunctionNameSyntax}'");
                return InvalidTypeBinding.Instance;
            }

            // TODO: Handle ordering issues; function parameter type bindings might not be initialized, if inferred, before caller tries to resolve for FunctionInvocation
            return this.FunctionDefinition.GetParameterTypeBinding(parameterIndex);
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver)
        {
            Name? contentProperty = this.GetContentProperty();
            if (contentProperty == null)
            {
                contentArgument.AddError($"No content parameter exists for FAML function '{this.FunctionName}'");
                return InvalidTypeBinding.Instance;
            }

            int parameterIndex = this.FunctionDefinition.GetParameterIndex(contentProperty.Value);
            if (parameterIndex == -1)
            {
                throw new InvalidOperationException($"Content parameter '{contentProperty}' for FAML function '{this.FunctionName}'");
            }

            // TODO: Handle ordering issues; function parameter type bindings might not be initialized, if inferred, before caller tries to resolve for FunctionInvocation
            return this.FunctionDefinition.GetParameterTypeBinding(parameterIndex);
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName,
            CancellationToken cancellationToken)
            {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }

        public override string GetNoContentPropertyExistsError()
        {
            return $"Use of unnamed parameter not allowed. No content parameter exists for function '{this.FunctionDefinition.FunctionNameSyntax}'";
        }

        public override Name? GetThisParameter()
        {
            // TODO: Support 'object' or maybe 'this' keyword to specify object property
            return null;
        }

        public override Name? GetContentProperty()
        {
            // If there's just a single property, it's the content/default parameter
            PropertyNameTypePairSyntax[] propertyNameTypePairs = this.FunctionDefinition.Parameters;
            if (propertyNameTypePairs.Length == 1)
            {
                return propertyNameTypePairs[0].PropertyName;
            }

            // If there's a property Named "Content" or "content", then it's the content property
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in propertyNameTypePairs)
            {
                Name propertyName = propertyNameTypePair.PropertyName;
                if (propertyName.ToString().Equals("Content") || propertyName.ToString().Equals("content"))
                {
                    return propertyName;
                }
            }

            return null;
        }

        /// <summary>
        /// Return all the parameter names, in their "natural" order.  More info about parameters (e.g. their types) can be queried by methods above.
        /// </summary>
        /// <returns>parameter names</returns>
        public override Name[] GetParameters()
        {
            PropertyNameTypePairSyntax[] propertyNameTypePairs = this.FunctionDefinition.Parameters;

            Name[] parameters = new Name[propertyNameTypePairs.Length];
            for (int i = 0; i < propertyNameTypePairs.Length; i++)
            {
                parameters[i] = propertyNameTypePairs[i].PropertyName;
            }

            return parameters;
        }
    }
}
