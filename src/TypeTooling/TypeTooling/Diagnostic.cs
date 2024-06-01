using System.Collections.Immutable;

namespace TypeTooling
{
    public class Diagnostic {
        // Auto properties
        public string Message { get; }
        public DiagnosticSeverity Severity { get; }

        public static ImmutableArray<Diagnostic> SingleError(string message) {
            ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            diagnostics.Add(new Diagnostic(message, DiagnosticSeverity.Error));
            return diagnostics.ToImmutable();
        }

        public Diagnostic(string message, DiagnosticSeverity severity) {
            Message = message;
            Severity = severity;
        }
    }
}
