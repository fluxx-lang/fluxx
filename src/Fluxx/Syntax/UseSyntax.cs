using Microsoft.CodeAnalysisP.Text;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;


/**
 * @author Bret Johnson
 * @since 6/6/2015
 */
namespace Faml.Syntax {
    public sealed class UseSyntax : SyntaxNode {
        private readonly FunctionInvocationSyntax _functionInvocation;

        public UseSyntax(TextSpan span, FunctionInvocationSyntax functionInvocation) : base(span) {
            _functionInvocation = functionInvocation;
            _functionInvocation.SetParent(this);
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            _functionInvocation.VisitChildren(visitor);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.Use;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write("use ");
            sourceWriter.Write(_functionInvocation);
        }

        public void ResolveImportLibrary() {
            /*
            if (_functionInvocation.functionName.ToString() != "DotNet") {
                this.addProblem("'use' currently only supports 'DotNet' packages");
                return;
            }

            if (_functionInvocation.arguments.Length != 1 || _functionInvocation.arguments[0].argumentNameIdentifier != null) {
                this.addProblem("'use' currently only supports 'DotNet' packages, with a single content property with the library name");
                return;
            }

            string libraryPath = _functionInvocation.arguments[0].getLiteralValueSourceString();

            // If the libraryPath is relative, then turn it into an absolute path, assuming that it's relative to the project directory
            if (!System.IO.Path.IsPathRooted(libraryPath)) {
                string projectDirectory = getProgram().rootDirectory;

                if (projectDirectory == null) {
                    this.addProblem($"library path {libraryPath} is relative, but relative paths aren't allowed since no project directory is set for the project");
                    return;
                }

                libraryPath = Path.Combine(projectDirectory, libraryPath);
            }

            string errorMessage;
            getProgram().cSharpProgramInfo.loadLibrary(libraryPath, TODO, out errorMessage);

            if (errorMessage != null) {
                this.addProblem(errorMessage);
            }
            */
        }
    }
}
