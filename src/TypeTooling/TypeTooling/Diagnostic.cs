using System.Collections.Immutable;

namespace TypeTooling
{
    public class Diagnostic(string message, DiagnosticSeverity severity)
    {
        // Auto properties
        public string Message { get; } = message;

        public DiagnosticSeverity Severity { get; } = severity;

        public static ImmutableArray<Diagnostic> SingleError(string message)
        {
            ImmutableArray<Diagnostic>.Builder diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();
            diagnostics.Add(new Diagnostic(message, DiagnosticSeverity.Error));
            return diagnostics.ToImmutable();
        }
    }
}
