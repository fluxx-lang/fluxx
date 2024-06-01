using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Binding;
using Faml.Binding.Internal;
using Faml.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/5/2015
 */
namespace Faml.Syntax {
    public class RecordTypeDefinitionSyntax : DefinitionSyntax {
        private readonly NameSyntax _typeNameSyntax;
        private readonly PropertyNameTypePairSyntax[] _propertyNameTypePairs;
        private readonly RecordTypeBinding _typeBinding;

        public RecordTypeDefinitionSyntax(TextSpan span, NameSyntax typeNameSyntax, PropertyNameTypePairSyntax[] propertyNameTypePairs) : base(span) {
            _typeNameSyntax = typeNameSyntax;
            _typeNameSyntax.SetParent(this);

            _propertyNameTypePairs = propertyNameTypePairs;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _propertyNameTypePairs) {
                propertyNameTypePair.SetParent(this);
            }

            _typeBinding = new RecordTypeBinding(this);
        }

        public NameSyntax TypeNameSyntax => _typeNameSyntax;

        public Name TypeName => _typeNameSyntax.Name;

        public PropertyNameTypePairSyntax[] PropertyNameTypePairs => _propertyNameTypePairs;

        public RecordTypeBinding TypeBinding => _typeBinding;

        public TypeBinding? GetPropertyTypeBinding(Name propertyName) {
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _propertyNameTypePairs) {
                if (propertyNameTypePair.PropertyNameSyntax.Name == propertyName)
                    return propertyNameTypePair.TypeReferenceSyntax.GetTypeBinding();
            }

            return null;
        }

        public bool HasProperty(Name properName) {
            return GetPropertyTypeBinding(properName) != null;
        }

        public Name[] GetProperties() {
            int length = _propertyNameTypePairs.Length;
            var properties = new Name[length];
            for (int i = 0; i < length; i++)
                properties[i] = _propertyNameTypePairs[i].PropertyName;

            return properties;
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.RecordTypeDefinition;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_typeNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _propertyNameTypePairs)
                visitor(propertyNameTypePair);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("type ");
            sourceWriter.Write(_typeNameSyntax);
            sourceWriter.Writeln(" = {");

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _propertyNameTypePairs) {
                sourceWriter.Write("    ");
                sourceWriter.Writeln(propertyNameTypePair);
            }

            sourceWriter.Writeln("}");
        }
    }
}
