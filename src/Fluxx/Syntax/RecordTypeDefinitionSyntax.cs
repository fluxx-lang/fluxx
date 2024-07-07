using Faml.Api;
using Faml.Binding;
using Faml.Binding.Internal;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/5/2015
 */
namespace Faml.Syntax
{
    public class RecordTypeDefinitionSyntax : DefinitionSyntax
    {
        private readonly NameSyntax typeNameSyntax;
        private readonly PropertyNameTypePairSyntax[] propertyNameTypePairs;
        private readonly RecordTypeBinding typeBinding;

        public RecordTypeDefinitionSyntax(TextSpan span, NameSyntax typeNameSyntax, PropertyNameTypePairSyntax[] propertyNameTypePairs) : base(span)
        {
            this.typeNameSyntax = typeNameSyntax;
            this.typeNameSyntax.SetParent(this);

            this.propertyNameTypePairs = propertyNameTypePairs;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.propertyNameTypePairs)
            {
                propertyNameTypePair.SetParent(this);
            }

            this.typeBinding = new RecordTypeBinding(this);
        }

        public NameSyntax TypeNameSyntax => this.typeNameSyntax;

        public Name TypeName => this.typeNameSyntax.Name;

        public PropertyNameTypePairSyntax[] PropertyNameTypePairs => this.propertyNameTypePairs;

        public RecordTypeBinding TypeBinding => this.typeBinding;

        public TypeBinding? GetPropertyTypeBinding(Name propertyName)
        {
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.propertyNameTypePairs)
            {
                if (propertyNameTypePair.PropertyNameSyntax.Name == propertyName)
                {
                    return propertyNameTypePair.TypeReferenceSyntax.GetTypeBinding();
                }
            }

            return null;
        }

        public bool HasProperty(Name properName)
        {
            return this.GetPropertyTypeBinding(properName) != null;
        }

        public Name[] GetProperties()
        {
            int length = this.propertyNameTypePairs.Length;
            var properties = new Name[length];
            for (int i = 0; i < length; i++)
            {
                properties[i] = this.propertyNameTypePairs[i].PropertyName;
            }

            return properties;
        }

        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.RecordTypeDefinition;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.typeNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.propertyNameTypePairs)
            {
                visitor(propertyNameTypePair);
            }
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write("type ");
            sourceWriter.Write(this.typeNameSyntax);
            sourceWriter.Writeln(" = {");

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.propertyNameTypePairs)
            {
                sourceWriter.Write("    ");
                sourceWriter.Writeln(propertyNameTypePair);
            }

            sourceWriter.Writeln("}");
        }
    }
}
