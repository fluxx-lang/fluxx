using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Binding.Resolver;
using Faml.Syntax;
using TypeTooling.ClassifiedText;
using TypeTooling.Types;

namespace Faml.Binding.External
{
    public sealed class NewExternalObjectFunctionBinding : FunctionBinding
    {
        private readonly ExternalObjectTypeBinding objectTypeBinding;

        public NewExternalObjectFunctionBinding(ExternalObjectTypeBinding objectTypeBinding)
        {
            this.objectTypeBinding = objectTypeBinding;
        }

        public TypeToolingType TypeToolingType => this.objectTypeBinding.TypeToolingType;

        public override QualifiableName FunctionName => this.objectTypeBinding.TypeName;

        public override TypeBinding ReturnTypeBinding => this.objectTypeBinding;

        public ExternalObjectTypeBinding ReturnExternalObjectTypeBinding => this.objectTypeBinding;

        public override TypeBinding? GetParameterTypeBinding(Name parameterName)
        {
            PropertyBinding? propertyBinding = this.objectTypeBinding.GetPropertyBinding(parameterName);
            return propertyBinding?.GetTypeBinding();
        }

        public override TypeBinding ResolveArgumentTypeBinding(
            QualifiableName argumentName, ArgumentNameValuePairSyntax argumentNameValuePair, BindingResolver bindingResolver)
        {
            if (argumentName.IsQualified())
                return this.ResolveAttachedPropertyArgumentTypeBinding(argumentName, argumentNameValuePair, bindingResolver);
            else
            {
                Name unqualifiableArgumentName = argumentName.ToUnqualifiableName();

                PropertyBinding? propertyBinding = this.objectTypeBinding.GetPropertyBinding(unqualifiableArgumentName);
                if (propertyBinding != null)
                {
                    return propertyBinding.GetTypeBinding();
                }

                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.PropertySpecifier,
                    $"No '{argumentName}' property exists for class '{this.TypeToolingType.FullName}'");
                return InvalidTypeBinding.Instance;
#if false
                TypeInfo typeInfo = GetDotNetTypeInfo();

                Project project = _objectTypeBinding.Project;

                if (unqualifiableArgumentName.ToString() == "~self")
                    return ExternalBindingUtil.DotNetTypeToTypeBinding(project, typeInfo.AsType());

                PropertyInfo propertyInfo = DotNetUtil.GetPropertyInfo(typeInfo, unqualifiableArgumentName);
                if (propertyInfo != null)
                    return ExternalBindingUtil.DotNetTypeToTypeBinding(project, propertyInfo.PropertyType);

                EventInfo eventInfo = DotNetUtil.GetEventInfo(typeInfo, unqualifiableArgumentName);
                if (eventInfo != null)
                    return PredefinedTypeBinding.Event;

                argumentNameValuePair.GetModule().AddError(argumentNameValuePair.ArgumentNameIdentifier,
                    $"No '{argumentName}' property exists for class '{typeInfo.FullName}'");
                return InvalidTypeBinding.Instance;
#endif
            }
        }

        public override TypeBinding ResolveContentArgumentTypeBinding(ContentArgumentSyntax contentArgument, BindingResolver bindingResolver)
        {
            Name? contentProperty = this.GetContentProperty();
            if (contentProperty == null)
            {
                contentArgument.AddError($"No content property exists for class '{this.TypeToolingType.FullName}'");
                return InvalidTypeBinding.Instance;
            }

            PropertyBinding? propertyBinding = this.objectTypeBinding.GetPropertyBinding(contentProperty.Value);
            if (propertyBinding != null)
            {
                return propertyBinding.GetTypeBinding();
            }

            if (propertyBinding == null)
            {
                throw new InvalidOperationException($"Binding for content property '{contentProperty}' unexpectedly not found");
            }

            return propertyBinding.GetTypeBinding();
        }

        public override Task<ClassifiedTextMarkup?> GetParameterDescriptionAsync(Name parameterName, CancellationToken cancellationToken)
        {
            PropertyBinding? propertyBinding = this.objectTypeBinding.GetPropertyBinding(parameterName);
            if (propertyBinding == null)
            {
                return Task.FromResult((ClassifiedTextMarkup?)null);
            }

            return propertyBinding.GetDescriptionAsync(cancellationToken);
        }

        public TypeBinding ResolveAttachedPropertyArgumentTypeBinding(QualifiableName argumentName, 
            ArgumentNameValuePairSyntax argumentNameValuePair, BindingResolver bindingResolver)
            {
            SyntaxNode nodeForErrors = argumentNameValuePair.PropertySpecifier ?? (SyntaxNode)argumentNameValuePair;

            // Get the attached type, failing if it doesn't exist
            QualifiableName attachedTypeName = argumentName.GetQualifier();
            AttachedTypeBinding? attachedTypeBinding = bindingResolver.ResolveAttachedTypeBinding(attachedTypeName);
            if (attachedTypeBinding == null)
            {
                nodeForErrors.AddError($"Attached type not found: {attachedTypeName}");
                return InvalidTypeBinding.Instance;
            }

            // Ensure the attaching type is an external type
            // TODO: Check that attaching type is compatible
            if (!(attachedTypeBinding is ExternalAttachedTypeBinding externalAttachedTypeBinding))
            {
                nodeForErrors.AddError($"Type {attachedTypeName} isn't an external type; only external (e.g. C#) types can be used as attached properties for external objects");
                return InvalidTypeBinding.Instance;
            }

            string unqualifiedPropertyName = argumentName.GetLastComponent().ToString();

            foreach (AttachedProperty attachedProperty in externalAttachedTypeBinding.AttachedType.AttachedProperties)
            {
                if (attachedProperty.Name == unqualifiedPropertyName)
                {
                    return ExternalBindingUtil.TypeToolingTypeToTypeBinding(this.objectTypeBinding.Project, attachedProperty.Type);
                }
            }

            // If no TypeInfo provider could provide the attached property here, indicate that in the error
            nodeForErrors.AddError(
                $"Type {attachedTypeName} doesn't provide an attached property {unqualifiedPropertyName} that can be used here");
            return InvalidTypeBinding.Instance;

#if false
            Faml.Syntax.ModuleSyntax module = argumentNameValuePair.GetModule();

            // Get the attaching type, failing if it doesn't exist
            QualifiableName attachingTypeName = argumentName.GetQualifier();
            attachingTypeBinding = module.GetObjectTypeBinding(attachingTypeName);
            if (attachingTypeBinding == null) {
                module.AddError(argumentNameValuePair.ArgumentNameIdentifier, $"Type not found: {attachingTypeName}");
                return InvalidTypeBinding.Instance;
            }

            // Ensure the attaching type is an external type
            // TODO: Check that attaching type is compatible
            var attachingExternalObjectTypeBinding = attachingTypeBinding as ExternalObjectTypeBinding;

            // TODO: Fix up this message
            if (attachingExternalObjectTypeBinding == null) {
                module.AddError(argumentNameValuePair.ArgumentNameIdentifier,
                    $"Type {attachingTypeName} isn't a C# class; only C# types can be used as attached properties for C# objects");
                return InvalidTypeBinding.Instance;
            }

            TypeInfo attachingTypeInfo = attachingExternalObjectTypeBinding.GetDotNetTypeInfo();
            //TypeInfo targetTypeInfo = _objectTypeBinding.GetDotNetTypeInfo();
            string unqualifiedPropertyName = argumentName.GetLastComponent().ToString();


            Project project = _objectTypeBinding.Project;
            AttachedType attachedType = project.GetTypeToolingAttachedType(attachingTypeInfo.AsType());

            foreach (AttachedProperty attachedProperty in attachedType.AttachedProperties) {
                if (attachedProperty.Name == unqualifiedPropertyName)
                    return ExternalBindingUtil.TypeToolingTypeToTypeBinding(_objectTypeBinding.Project, attachedProperty.Type);
            }

            // If no TypeInfo provider could provide the attached property here, indicate that in the error
            module.AddError(argumentNameValuePair.ArgumentNameIdentifier,
                $"Type {attachingTypeName} doesn't provide an attached property {unqualifiedPropertyName} that can be used here");
            return InvalidTypeBinding.Instance;
#endif
        }

        public override string GetNoContentPropertyExistsError()
        {
            return $"Use of unnamed parameter not allowed. No content property exists for class '{this.TypeToolingType.FullName}'";
        }

        public override Name? GetThisParameter()
        {
#if false
            // TODO: For now, hardcode these properties as potential defaults, but should switch to use annotations
            TypeInfo typeInfo = GetDotNetTypeInfo();

            Type objectPropertyAttributeType = typeof(ThisPropertyAttribute);

            foreach (CustomAttributeData attribute in typeInfo.CustomAttributes) {
                if (attribute.AttributeType == objectPropertyAttributeType) {
                    CustomAttributeTypedArgument constructorArg = attribute.ConstructorArguments[0];
                    return new Name((string) constructorArg.Value);
                }
            }
#endif
            throw new NotImplementedException();
            //return null;
        }

        public override Name? GetContentProperty()
        {
            return this.objectTypeBinding.ContentProperty;
#if false
            // TODO: For now, hardcode these properties as potential defaults, but should switch to use annotations
            TypeInfo typeInfo = GetDotNetTypeInfo();

            var contentName = new Name("Content");
            var childrenName =  new Name("Children");
            var textName = new Name("Text");

            if (DotNetUtil.GetPropertyInfo(typeInfo, contentName) != null)
                return contentName;
            else if (DotNetUtil.GetPropertyInfo(typeInfo, childrenName) != null)
                return childrenName;
            else if (DotNetUtil.GetPropertyInfo(typeInfo, textName) != null)
                return textName;
            else if (DotNetUtil.ImplementsIList(typeInfo.AsType()))
                return new Name("~self");
            else return null;
#endif
        }

        public override Name[] GetParameters()
        {
            return this.objectTypeBinding.ObjectProperties.Values
                .Where(property => property.CanWrite)
                .Select(property => new Name(property.Name))
                .ToArray();
        }
    }
}
