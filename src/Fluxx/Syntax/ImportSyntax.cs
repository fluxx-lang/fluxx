using System.Collections.Immutable;
using Fluxx.Api;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */
namespace Fluxx.Syntax
{
    public sealed class ImportSyntax : SyntaxNode
    {
        private readonly ImmutableArray<ImportTypeReferenceSyntax>? importTypeReferences;         // If null, import all types
        private readonly QualifiableNameSyntax qualifierSyntax;

        public ImportSyntax(TextSpan span, ImmutableArray<ImportTypeReferenceSyntax>? importTypeReferences, QualifiableNameSyntax qualifierSyntax) : base(span)
        {
            this.importTypeReferences = importTypeReferences;
            if (importTypeReferences != null)
            {
                foreach (ImportTypeReferenceSyntax importReference in this.importTypeReferences)
                {
                    importReference.SetParent(this);
                }
            }

            this.qualifierSyntax = qualifierSyntax;
            this.qualifierSyntax.SetParent(this);
        }

        public ImportSyntax(TextSpan span, QualifiableNameSyntax qualifierSyntax) : base(span)
        {
            this.importTypeReferences = null;

            this.qualifierSyntax = qualifierSyntax;
            this.qualifierSyntax.SetParent(this);
        }

        public ImmutableArray<ImportTypeReferenceSyntax>? ImportTypeReferences => this.importTypeReferences;

        public QualifiableNameSyntax QualifierSyntax => this.qualifierSyntax;

        public QualifiableName Qualifier => this.qualifierSyntax.Name;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            if (this.importTypeReferences != null)
            {
                foreach (ImportTypeReferenceSyntax importReference in this.importTypeReferences)
                {
                    visitor(importReference);
                }
            }

            visitor(this.qualifierSyntax);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.Import;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            if (this.importTypeReferences != null)
            {
                sourceWriter.Write("import {");

                bool first = true;
                foreach (ImportTypeReferenceSyntax importReference in this.importTypeReferences)
                {
                    if (!first)
                    {
                        sourceWriter.Write("  ");
                    }

                    sourceWriter.Write(importReference);
                    first = false;
                }

                sourceWriter.Write("} from ");
                sourceWriter.Write(this.qualifierSyntax);
            }
            else
            {
                sourceWriter.Write("import ");
                sourceWriter.Write(this.qualifierSyntax);
            }
        }
    }
}
