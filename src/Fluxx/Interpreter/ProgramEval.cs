using Fluxx.SourceProviders;

namespace Fluxx.Interpreter
{
    public class 
        ProgramEval
        {
        private readonly FamlProject program;
        private readonly SourceProvider sourceProvider;

        public ProgramEval(SourceProvider sourceProvider)
        {
            this.sourceProvider = sourceProvider;
        }
    }
}
