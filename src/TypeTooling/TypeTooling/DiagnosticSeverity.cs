namespace TypeTooling
{
    /// <summary>
    /// Describes how severe a diagnostic is.
    ///
    /// This was copied from Roslyn's Microsoft.CodeAnalysis.DiagnosticSeverity
    /// </summary>
    public enum DiagnosticSeverity {
        /// <summary>
        /// Information that does not indicate a problem (i.e. not prescriptive).
        /// </summary>
        Info = 1,

        /// <summary>
        /// Something suspicious but allowed.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Something not allowed by the rules of the language or other authority.
        /// </summary>
        Error = 3,
    }
}
