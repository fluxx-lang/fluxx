using System.Collections.Immutable;
using Faml.Api;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

/**
 * @author Bret Johnson
 * @since 6/6/2015
 */

namespace Faml.Syntax
{
    public sealed class ImportSyntax : SyntaxNode
    {
        private readonly ImmutableArray<ImportTypeReferenceSyntax>? _importTypeReferences;         // If null, import all types
        private readonly QualifiableNameSyntax _qualifierSyntax;


        public ImportSyntax(TextSpan span, ImmutableArray<ImportTypeReferenceSyntax>? importTypeReferences, QualifiableNameSyntax qualifierSyntax) : base(span)
        {
            this._importTypeReferences = importTypeReferences;
            if (importTypeReferences != null)
            {
                foreach (ImportTypeReferenceSyntax importReference in this._importTypeReferences)
                {
                    importReference.SetParent(this);
                }
            }

            this._qualifierSyntax = qualifierSyntax;
            this._qualifierSyntax.SetParent(this);
        }

        public ImportSyntax(TextSpan span, QualifiableNameSyntax qualifierSyntax) : base(span)
        {
            this._importTypeReferences = null;

            this._qualifierSyntax = qualifierSyntax;
            this._qualifierSyntax.SetParent(this);
        }

        public ImmutableArray<ImportTypeReferenceSyntax>? ImportTypeReferences => this._importTypeReferences;

        public QualifiableNameSyntax QualifierSyntax => this._qualifierSyntax;

        public QualifiableName Qualifier => this._qualifierSyntax.Name;

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            if (this._importTypeReferences != null)
            {
                foreach (ImportTypeReferenceSyntax importReference in this._importTypeReferences)
                {
                    visitor(importReference);
                }
            }

            visitor(this._qualifierSyntax);
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.Import;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            if (this._importTypeReferences != null)
            {
                sourceWriter.Write("import {");

                bool first = true;
                foreach (ImportTypeReferenceSyntax importReference in this._importTypeReferences)
                {
                    if (!first)
                    {
                        sourceWriter.Write("  ");
                    }

                    sourceWriter.Write(importReference);
                    first = false;
                }

                sourceWriter.Write("} from ");
                sourceWriter.Write(this._qualifierSyntax);
            }
            else
            {
                sourceWriter.Write("import ");
                sourceWriter.Write(this._qualifierSyntax);
            }
        }
    }
}
