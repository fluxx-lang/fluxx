using System;
using Fluxx.Api;
using Fluxx.Syntax;
using Microsoft.CodeAnalysis.Text;
using DiagnosticSeverity = Fluxx.Api.DiagnosticSeverity;
using SyntaxNode = Fluxx.Syntax.SyntaxNode;

namespace Fluxx.CodeAnalysis
{
    public class Diagnostic
    {
        private readonly FamlModule? module;
        //private readonly Location? _location;
        private readonly LinePosition? startLinePosition;
        private readonly TextSpan sourceSpan;
        private readonly DiagnosticSeverity severity;
        private readonly string message;

        public static Diagnostic FromTypeToolingDiagnostic(FamlModule module, TextSpan sourceSpan,
            TypeTooling.Diagnostic typeToolingDiagnostic)
            {
            DiagnosticSeverity severity;

            switch (typeToolingDiagnostic.Severity)
            {
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

        public Diagnostic(FamlModule module, TextSpan sourceSpan, DiagnosticSeverity severity, string message)
        {
            this.module = module;

            this.sourceSpan = sourceSpan;
            this.startLinePosition = module.SourceText.Lines.GetLinePosition(this.sourceSpan.Start);

            this.severity = severity;
            this.message = message;
        }

        public Diagnostic(DiagnosticSeverity severity, string message)
        {
            this.module = null;
            this.sourceSpan = TextSpanExtensions.NullTextSpan;
            this.startLinePosition = null;

            this.severity = severity;
            this.message = message;
        }

        public Diagnostic(SyntaxNode syntaxNode, DiagnosticSeverity severity, string message)
            : this(syntaxNode.GetModule(), syntaxNode.Span, severity, message) { }

        public QualifiableName ModuleName => this.module?.ModuleName ?? new QualifiableName(string.Empty);

        public LinePosition? StartLinePosition => this.startLinePosition;

        public TextSpan SourceSpan => this.sourceSpan;

        public DiagnosticSeverity Severity => this.severity;

        public string Message => this.message;
    }
}
