using System;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api.IntelliSense;
using Faml.CodeAnalysis.Text;
using Faml.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Faml.IntelliSense {
    public abstract class IntelliSense {
        public FamlModule Module { get; }
        public int Position { get; }
        public SyntaxNode? TerminalNode { get; }
        private ParseableSource? _parseableSource;


        protected IntelliSense(FamlModule module, int position, SyntaxNode? terminalNode) {
            this.Module = module;
            this.Position = position;
            this.TerminalNode = terminalNode;
        }

        public IntelliSenseStartData GetStartData() {
            if (this.TerminalNode == null)
                return new IntelliSenseStartData(new TextSpan(this.Position, 0));
            else return new IntelliSenseStartData(this.TerminalNode.Span);
        }

        public abstract Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken);

        protected ParseableSource ParseableSource {
            get {
                if (this._parseableSource == null)
                    this._parseableSource = new ParseableSource(this.Module.SourceText);
                return this._parseableSource;
            }
        }

        protected string GetSpaces(int count) {
            switch (count) {
                case 0: return "";
                case 1: return " ";
                case 2: return "  ";
                default: throw new ArgumentException("Only space counts <= 2 are currently supported");
            }
        }
    }
}
