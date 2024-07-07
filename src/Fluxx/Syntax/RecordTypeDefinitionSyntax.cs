using Faml.Api;
using Faml.Binding;
using Faml.Binding.Internal;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

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
            this._typeNameSyntax = typeNameSyntax;
            this._typeNameSyntax.SetParent(this);

            this._propertyNameTypePairs = propertyNameTypePairs;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._propertyNameTypePairs) {
                propertyNameTypePair.SetParent(this);
            }

            this._typeBinding = new RecordTypeBinding(this);
        }

        public NameSyntax TypeNameSyntax => this._typeNameSyntax;

        public Name TypeName => this._typeNameSyntax.Name;

        public PropertyNameTypePairSyntax[] PropertyNameTypePairs => this._propertyNameTypePairs;

        public RecordTypeBinding TypeBinding => this._typeBinding;

        public TypeBinding? GetPropertyTypeBinding(Name propertyName) {
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._propertyNameTypePairs) {
                if (propertyNameTypePair.PropertyNameSyntax.Name == propertyName)
                    return propertyNameTypePair.TypeReferenceSyntax.GetTypeBinding();
            }

            return null;
        }

        public bool HasProperty(Name properName) {
            return this.GetPropertyTypeBinding(properName) != null;
        }

        public Name[] GetProperties() {
            int length = this._propertyNameTypePairs.Length;
            var properties = new Name[length];
            for (int i = 0; i < length; i++)
                properties[i] = this._propertyNameTypePairs[i].PropertyName;

            return properties;
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.RecordTypeDefinition;

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._typeNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._propertyNameTypePairs)
                visitor(propertyNameTypePair);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("type ");
            sourceWriter.Write(this._typeNameSyntax);
            sourceWriter.Writeln(" = {");

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._propertyNameTypePairs) {
                sourceWriter.Write("    ");
                sourceWriter.Writeln(propertyNameTypePair);
            }

            sourceWriter.Writeln("}");
        }
    }
}
