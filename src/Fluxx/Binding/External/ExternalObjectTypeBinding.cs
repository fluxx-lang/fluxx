using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Syntax;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling.ClassifiedText;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Types;

namespace Faml.Binding.External
{
    public class ExternalObjectTypeBinding : ObjectTypeBinding
    {
        private readonly FamlProject project;
        private readonly ObjectType typeToolingType;
        private bool gotAttachedType;
        private AttachedType? attachedType;
        private readonly DotNetRawType rawType;
        private bool gotProperties;
        private Dictionary<string, ObjectProperty>? objectProperties;
        private Name? contentProperty;


        // TODO: This name is fully qualified.   Do we want that?
        public ExternalObjectTypeBinding(FamlProject project, DotNetRawType rawType) : base(new QualifiableName(rawType.FullName))
        {
            this.project = project;
            this.rawType = rawType;

            this.typeToolingType = (ObjectType)this.project.GetTypeToolingType(this.rawType);
            this.attachedType = this.project.GetTypeToolingAttachedType(this.rawType);
        }

        public ExternalObjectTypeBinding(FamlProject project, ObjectType typeToolingType) : base(new QualifiableName(typeToolingType.FullName))
        {
            this.project = project;
            this.typeToolingType = typeToolingType;

            this.rawType = (DotNetRawType)typeToolingType.UnderlyingType;
        }

        public FamlProject Project => this.project;

        public ObjectType TypeToolingType => this.typeToolingType;

        public AttachedType? AttachedType
        {
            get
            {
                // Only lookup the attached type if someone actually needs it
                if (!this.gotAttachedType)
                {
                    this.attachedType = this.project.GetTypeToolingAttachedType(this.rawType);
                    this.gotAttachedType = true;
                }

                return this.attachedType;
            }
        }

        public Dictionary<string, ObjectProperty> ObjectProperties
        {
            get
            {
                this.GetPropertiesIfNeeded();
                return this.objectProperties;
            }
        }

        public ObjectProperty GetObjectProperty(QualifiableName name) => this.objectProperties[name.ToString()];

        public Name? ContentProperty
        {
            get
            {
                this.GetPropertiesIfNeeded();
                return this.contentProperty;
            }
        }

        public void GetPropertiesIfNeeded()
        {
            if (this.gotProperties)
            {
                return;
            }

            this.objectProperties = new Dictionary<string, ObjectProperty>();

            // Add all properties - for the type itself and its ancestors
            foreach (ObjectType type in GetTypeAndAncestors(this.typeToolingType))
            {
                foreach (ObjectProperty property in type.Properties)
                {
                    if (!this.objectProperties.ContainsKey(property.Name))
                    {
                        this.objectProperties.Add(property.Name, property);
                    }
                }
            }

            // Looks first in the type itself to see if it has a content property, then search its ancestors
            this.contentProperty = null;
            foreach (ObjectType type in GetTypeAndAncestors(this.typeToolingType))
            {
                ObjectProperty? contentProperty = type.ContentProperty;
                if (contentProperty != null)
                {
                    this.contentProperty = new Name(contentProperty.Name);
                    break;
                }
            }

            this.gotProperties = true;
        }

        public static IEnumerable<ObjectType> GetTypeAndAncestors(ObjectType objectType)
        {
            yield return objectType;

            foreach (ObjectType baseType in objectType.GetBaseTypes())
            {
                foreach (ObjectType ancestorType in GetTypeAndAncestors(baseType))
                {
                    yield return ancestorType;
                }
            }
        }

        protected bool Equals(ExternalObjectTypeBinding other)
        {
            return this.rawType.Equals(other.rawType);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExternalObjectTypeBinding))
            {
                return false;
            }

            return this.Equals((ExternalObjectTypeBinding)obj);
        }

        public override int GetHashCode()
        {
            return this.rawType.GetHashCode();
        }

        public override bool IsAssignableFrom(TypeBinding other)
        {
            if (other is ExternalObjectTypeBinding otherDotNetObjectTypeBinding)
            {
                return this.rawType.IsAssignableFrom(otherDotNetObjectTypeBinding.rawType);
            }
            else
            {
                return base.IsAssignableFrom(other);
            }
        }

        public override FunctionBinding? GetMethodBinding(Name methodName)
        {
            throw new NotImplementedException();
#if false
            // TODO: Support method overloading
            MethodInfo methodInfo = DotNetUtil.GetMethodInfo(_rawType.GetTypeInfo(), methodName);
            if (methodInfo != null)
                return new DotNetMethodFunctionBinding(this, methodInfo);

            return null;
#endif
        }

        public override PropertyBinding? GetPropertyBinding(Name propertyName)
        {
            if (!this.ObjectProperties.TryGetValue(propertyName.ToString(), out ObjectProperty property))
            {
                return null;
            }

            return new ExternalPropertyBinding(this, property);
        }

        public override bool SupportsCreateLiteral()
        {
            return this.typeToolingType?.GetCustomLiteralParser() != null;
        }

        public override ExpressionSyntax ParseLiteralValueSource(FamlModule module, TextSpan span)
        {
            SourceText sourceText = module.SourceText;
            string literalSource = sourceText.ToString(span);

            // Now see if there's a custom literal manager for the type
            CustomLiteralParser customLiteralParser = this.typeToolingType?.GetCustomLiteralParser();
            if (customLiteralParser != null)
            {
                try
                {
                    CustomLiteral customLiteral = customLiteralParser.Parse(literalSource);

                    bool anyErrors = false;
                    if (customLiteral.Diagnostics != null)
                    {
                        foreach (TypeTooling.Diagnostic diagnostic in customLiteral.Diagnostics)
                        {
                            module.AddError(span, diagnostic.Message);
                            if (diagnostic.Severity == TypeTooling.DiagnosticSeverity.Error)
                            {
                                anyErrors = true;
                            }
                        }

                        if (anyErrors)
                        {
                            return new InvalidExpressionSyntax(span, literalSource, this);
                        }
                    }

                    return new ExternalTypeCustomLiteralSytax(span, this, this.typeToolingType, literalSource, customLiteral);
                }
                catch (Exception e)
                {
                    module.AddError(span, e.Message);
                    return new InvalidExpressionSyntax(span, literalSource, this);
                }
            }

            module.AddError(span,
                $"'{this.rawType.Name}' can't be expressed as textual literal--it's not an enum nor does it have a custom literal manager");
            return new InvalidExpressionSyntax(span, literalSource, this);
        }

        public override Task<ClassifiedTextMarkup?> GetDescriptionAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult((ClassifiedTextMarkup?)null);
        }
    }
}
