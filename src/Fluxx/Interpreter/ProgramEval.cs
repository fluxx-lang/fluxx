/**
 * @author Bret Johnson
 * @since 4/17/2015
 */

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