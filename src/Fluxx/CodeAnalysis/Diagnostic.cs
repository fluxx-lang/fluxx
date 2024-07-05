using System;
using Faml.Api;
using Microsoft.CodeAnalysisP.Text;
using Faml.Syntax;
using DiagnosticSeverity = Faml.Api.DiagnosticSeverity;
using SyntaxNode = Faml.Syntax.SyntaxNode;

namespace Faml.CodeAnalysis {
    public class Diagnostic {
        private readonly FamlModule? _module;
        //private readonly Location? _location;
        private readonly LinePosition? _startLinePosition;
        private readonly TextSpan _sourceSpan;
        private readonly DiagnosticSeverity _severity;
        private readonly string _message;


        public static Diagnostic FromTypeToolingDiagnostic(FamlModule module, TextSpan sourceSpan,
            TypeTooling.Diagnostic typeToolingDiagnostic) {
            DiagnosticSeverity severity;

            switch (typeToolingDiagnostic.Severity) {
                case TypeTooling.DiagnosticSeverity.Error:
                    severity = DiagnosticSeverity.Error;
                    break;
                case TypeTooling.DiagnosticSeverity.Warning:
                    severity = DiagnosticSeverity.Warning;
                    break;
                case TypeTooling.DiagnosticSeverity.Info:
                    severity = DiagnosticSeverity.Info;
                    break;
                default:
                    throw new Exception(
                        $"Unexpected TypeTooling.DiagnosticSeverity value: {typeToolingDiagnostic.Severity}");
            }

            return new Diagnostic(module, sourceSpan, severity, typeToolingDiagnostic.Message);
        }

        public Diagnostic(FamlModule module, TextSpan sourceSpan, DiagnosticSeverity severity, string message) {
            _module = module;

            _sourceSpan = sourceSpan;
            _startLinePosition = module.SourceText.Lines.GetLinePosition(_sourceSpan.Start);

            _severity = severity;
            _message = message;
        }

        public Diagnostic(DiagnosticSeverity severity, string message) {
            _module = null;
            _sourceSpan = TextSpanExtensions.NullTextSpan;
            _startLinePosition = null;

            _severity = severity;
            _message = message;
        }

        public Diagnostic(SyntaxNode syntaxNode, DiagnosticSeverity severity, string message)
            : this(syntaxNode.GetModule(), syntaxNode.Span, severity, message) { }

        public QualifiableName ModuleName => _module?.ModuleName ?? new QualifiableName("");

        public LinePosition? StartLinePosition => _startLinePosition;

        public TextSpan SourceSpan => _sourceSpan;

        public DiagnosticSeverity Severity => _severity;

        public string Message => _message;
    }
}
