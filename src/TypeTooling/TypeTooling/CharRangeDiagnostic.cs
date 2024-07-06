namespace TypeTooling
{
    public class CharRangeDiagnostic(string message, DiagnosticSeverity severity, CharRange charRange) : Diagnostic(message, severity)
    {
        // Auto properties
        public CharRange CharRange { get; } = charRange;
    }
}
