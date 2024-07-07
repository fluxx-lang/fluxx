using Faml.Api;
using Faml.Syntax;
using Faml.Syntax.Expression;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Text;
using TypeTooling.Types;

namespace Faml.Binding.External {
    public class ExternalEnumTypeBinding : EnumTypeBinding {
        private readonly FamlProject _project;
        private readonly EnumType _typeToolingType;
        private readonly ImmutableArray<EnumValueBinding> _values;


        // TODO: This name is fully qualified.   Do we want that?
        public ExternalEnumTypeBinding(FamlProject project, EnumType typeToolingType) :
            base(new QualifiableName(typeToolingType.FullName)) {
            _project = project;
            _typeToolingType = typeToolingType;

            var values = ImmutableArray.CreateBuilder<EnumValueBinding>();
            foreach (EnumValue enumValue in _typeToolingType.Values)
                values.Add(new ExternalEnumValueBinding(this, enumValue));
            _values = values.ToImmutable();
        }

        public FamlProject Project => _project;

        public EnumType TypeToolingType => _typeToolingType;

        public override ExpressionSyntax ParseEnumValue(FamlModule module, TextSpan sourceSpan) {
            SourceText sourceText = module.SourceText;
            string valueSource = sourceText.ToString(sourceSpan);

            foreach (EnumValue enumValue in _typeToolingType.Values) {
                if (enumValue.Name == valueSource) {
                    ExternalEnumValueBinding enumValueBinding = new ExternalEnumValueBinding(this, enumValue);
                    NameSyntax enumNameSyntax = new NameSyntax(sourceSpan, new Name(valueSource));
                    return new EnumValueLiteralSyntax(sourceSpan, enumNameSyntax, enumValueBinding);
                }
            }

            module.AddError(sourceSpan, $"'{valueSource}' isn't a valid value for enum {_typeToolingType.FullName}");
            return new InvalidExpressionSyntax(sourceSpan, valueSource, this);
        }

        public override ImmutableArray<EnumValueBinding> GetValues() => _values;

        protected bool Equals(ExternalEnumTypeBinding other) {
            return _typeToolingType.Equals(other._typeToolingType);
        }

        public override bool Equals(object obj) {
            if (!(obj is ExternalEnumTypeBinding))
                return false;
            return Equals((ExternalEnumTypeBinding) obj);
        }

        public override int GetHashCode() {
            return _typeToolingType.GetHashCode();
        }
    }
}
