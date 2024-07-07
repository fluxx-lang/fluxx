using System;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api.IntelliSense;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.IntelliSense
{
    public abstract class IntelliSense
    {
        public FamlModule Module { get; }
        public int Position { get; }
        public SyntaxNode? TerminalNode { get; }
        private ParseableSource? parseableSource;

        protected IntelliSense(FamlModule module, int position, SyntaxNode? terminalNode)
        {
            this.Module = module;
            this.Position = position;
            this.TerminalNode = terminalNode;
        }

        public IntelliSenseStartData GetStartData()
        {
            if (this.TerminalNode == null)
            {
                return new IntelliSenseStartData(new TextSpan(this.Position, 0));
            }
            else
            {
                return new IntelliSenseStartData(this.TerminalNode.Span);
            }
        }

        public abstract Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken);

        protected ParseableSource ParseableSource
        {
            get
            {
                if (this.parseableSource == null)
                {
                    this.parseableSource = new ParseableSource(this.Module.SourceText);
                }

                return this.parseableSource;
            }
        }

        protected string GetSpaces(int count)
        {
            switch (count)
            {
                case 0: return string.Empty;
                case 1: return " ";
                case 2: return "  ";
                default: throw new ArgumentException("Only space counts <= 2 are currently supported");
            }
        }
    }
}
