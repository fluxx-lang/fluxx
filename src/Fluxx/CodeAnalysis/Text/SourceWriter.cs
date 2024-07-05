/**
 * @author Bret Johnson
 * @since 6/29/2014 5:02 PM
 */

using System.Text;
using Faml.Api;
using Faml.Syntax;

namespace Faml.CodeAnalysis.Text {
    public sealed class SourceWriter {
        internal StringBuilder Source = new StringBuilder();

        public void Write(string text) { Source.Append(text); }

        public void Writeln(string text)
        {
            Source.Append(text);
            Source.Append("\r\n");
        }

        public void Write(SyntaxNode syntaxNode) {
            syntaxNode.WriteSource(this);
        }

        public void Writeln(SyntaxNode syntaxNode) {
            syntaxNode.WriteSource(this);
            Writeln();
        }

        public void Write(Name name) {
            Write(name.ToString());
        }

        public void Write(QualifiableName name) {
            Write(name.ToString());
        }

        public void Writeln() { Source.Append("\r\n"); }

        public string GetSource() { return Source.ToString(); }

        public override string ToString() { return Source.ToString(); }
    }
}
