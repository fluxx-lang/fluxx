using System;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api.IntelliSense;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;
using Faml.Syntax;

namespace Faml.IntelliSense {
    public abstract class IntelliSense {
        public FamlModule Module { get; }
        public int Position { get; }
        public SyntaxNode? TerminalNode { get; }
        private ParseableSource? _parseableSource;


        protected IntelliSense(FamlModule module, int position, SyntaxNode? terminalNode) {
            Module = module;
            Position = position;
            TerminalNode = terminalNode;
        }

        public IntelliSenseStartData GetStartData() {
            if (TerminalNode == null)
                return new IntelliSenseStartData(new TextSpan(Position, 0));
            else return new IntelliSenseStartData(TerminalNode.Span);
        }

        public abstract Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken);

        protected ParseableSource ParseableSource {
            get {
                if (_parseableSource == null)
                    _parseableSource = new ParseableSource(Module.SourceText);
                return _parseableSource;
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
