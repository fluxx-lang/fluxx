using Fluxx.Api;
using Fluxx.Syntax;
using Fluxx.Syntax.Expression;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using TypeTooling.Types;

namespace Fluxx.Binding.External
{
    public class ExternalEnumTypeBinding : EnumTypeBinding
    {
        private readonly FamlProject project;
        private readonly EnumType typeToolingType;
        private readonly ImmutableArray<EnumValueBinding> values;

        // TODO: This name is fully qualified.   Do we want that?
        public ExternalEnumTypeBinding(FamlProject project, EnumType typeToolingType) : base(new QualifiableName(typeToolingType.FullName))
            {
            this.project = project;
            this.typeToolingType = typeToolingType;

            var values = ImmutableArray.CreateBuilder<EnumValueBinding>();
            foreach (EnumValue enumValue in this.typeToolingType.Values)
            {
                values.Add(new ExternalEnumValueBinding(this, enumValue));
            }

            this.values = values.ToImmutable();
        }

        public FamlProject Project => this.project;

        public EnumType TypeToolingType => this.typeToolingType;

        public override ExpressionSyntax ParseEnumValue(FamlModule module, TextSpan sourceSpan)
        {
            SourceText sourceText = module.SourceText;
            string valueSource = sourceText.ToString(sourceSpan);

            foreach (EnumValue enumValue in this.typeToolingType.Values)
            {
                if (enumValue.Name == valueSource)
                {
                    ExternalEnumValueBinding enumValueBinding = new ExternalEnumValueBinding(this, enumValue);
                    NameSyntax enumNameSyntax = new NameSyntax(sourceSpan, new Name(valueSource));
                    return new EnumValueLiteralSyntax(sourceSpan, enumNameSyntax, enumValueBinding);
                }
            }

            module.AddError(sourceSpan, $"'{valueSource}' isn't a valid value for enum {this.typeToolingType.FullName}");
            return new InvalidExpressionSyntax(sourceSpan, valueSource, this);
        }

        public override ImmutableArray<EnumValueBinding> GetValues() => this.values;

        protected bool Equals(ExternalEnumTypeBinding other)
        {
            return this.typeToolingType.Equals(other.typeToolingType);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ExternalEnumTypeBinding))
            {
                return false;
            }

            return this.Equals((ExternalEnumTypeBinding)obj);
        }

        public override int GetHashCode()
        {
            return this.typeToolingType.GetHashCode();
        }
    }
}
