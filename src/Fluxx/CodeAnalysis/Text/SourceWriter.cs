/**
 * @author Bret Johnson
 * @since 6/29/2014 5:02 PM
 */
using System.Text;
using Fluxx.Api;
using Fluxx.Syntax;

namespace Fluxx.CodeAnalysis.Text
{
    public sealed class SourceWriter
    {
        internal StringBuilder Source = new StringBuilder();

        public void Write(string text) { this.Source.Append(text); }

        public void Writeln(string text)
        {
            this.Source.Append(text);
            this.Source.Append("\r\n");
        }

        public void Write(SyntaxNode syntaxNode)
        {
            syntaxNode.WriteSource(this);
        }

        public void Writeln(SyntaxNode syntaxNode)
        {
            syntaxNode.WriteSource(this);
            this.Writeln();
        }

        public void Write(Name name)
        {
            this.Write(name.ToString());
        }

        public void Write(QualifiableName name)
        {
            this.Write(name.ToString());
        }

        public void Writeln() { this.Source.Append("\r\n"); }

        public string GetSource() { return this.Source.ToString(); }

        public override string ToString() { return this.Source.ToString(); }
    }
}
