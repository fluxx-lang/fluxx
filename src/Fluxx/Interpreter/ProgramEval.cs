using Faml.SourceProviders;

namespace Faml.Interpreter
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
