using System;
using System.Collections.Generic;
using Faml.Api;
using Faml.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Faml {
    internal class GetIconTags {
        private readonly ModuleSyntax _module;
        
        public GetIconTags(ModuleSyntax module) {
            this._module = module;
        }

        // TODO: Implement this propertly
        public void GetTags(TextSpan[] textSpans, List<IconTag> tags) {
            if (textSpans.Length == 1)
                this.GetTags(textSpans[0], tags);
            else throw new NotImplementedException();
        }

        public void GetTags(TextSpan span, List<IconTag> tags) {
            // Now get everything else
            this.GetSyntaxNodeTags(span, this._module, tags);
        }

        private void GetSyntaxNodeTags(TextSpan span, SyntaxNode syntaxNode, List<IconTag> tags) {
            if (syntaxNode.Span.IsNull() || !syntaxNode.OverlapsWith(span))
                ;   // If there's not source associated with the node (true for some Invalid... nodes generated via parser error recvery), add nothing
            else if (! syntaxNode.IsTerminalNode()) {
                SyntaxNodeType nodeType = syntaxNode.NodeType;

                if (nodeType == SyntaxNodeType.ExampleDefinition || nodeType == SyntaxNodeType.ExamplesDefinition)
                    tags.Add(new IconTag(new TextSpan(syntaxNode.Span.Start, 0), IconTagType.Example));

                syntaxNode.VisitChildren((child) => {
                    if (child.OverlapsWith(span))
                        this.GetSyntaxNodeTags(span, child, tags);
                });
            }
        }
    }
}
