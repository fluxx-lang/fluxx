using Faml.SourceProviders;

namespace Faml.Interpreter
{
    public class 
        ProgramEval
        {
        private readonly FamlProject _program;
        private readonly SourceProvider _sourceProvider;

        public ProgramEval(SourceProvider sourceProvider)
        {
            this._sourceProvider = sourceProvider;
        }
    }
}
