namespace TypeTooling
{
    public class CharRangeDiagnostic : Diagnostic
    {
        // Auto properties
        public CharRange CharRange { get; }

        public CharRangeDiagnostic(string message, DiagnosticSeverity severity, CharRange charRange) : base(message, severity)
        {
            CharRange = charRange;
        }
    }
}
