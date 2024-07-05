using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Api.IntelliSense;
using Microsoft.CodeAnalysisP.Text;
using Faml.CodeGeneration;
using Faml.IntelliSense;
using Faml.QuickInfo;
using Microsoft.CodeAnalysisP;
using TypeTooling.ClassifiedText;
using Diagnostic = Faml.CodeAnalysis.Diagnostic;
using DiagnosticSeverity = Faml.Api.DiagnosticSeverity;


namespace Faml.Syntax {
    public sealed class FamlModule {
        public FamlProject Project { get; }
        public  QualifiableName ModuleName { get; }
        public ModuleSyntax ModuleSyntax { get; internal set; }
        public SourceText SourceText { get; }
        public ModuleDelegates ModuleDelegates { get; }

        public FamlModule(FamlProject project, QualifiableName moduleName, SourceText sourceText) {
            Project = project;
            ModuleName = moduleName;
            SourceText = sourceText;
            ModuleDelegates = new ModuleDelegates(project.TypeToolingEnvironment);
        }

        /// <summary>
        /// The path of the source document file.
        /// </summary>
        /// <remarks>
        /// If this syntax tree is not associated with a file, this value can be empty.
        /// The path shall not be null.
        /// </remarks>
        public string FilePath => Project.GetModuleFilePath(ModuleName);

        /// <summary>
        /// Gets the location in terms of path, line and column for a given span.
        /// </summary>
        /// <param name="span">Span within the tree.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        /// <see cref="Microsoft.CodeAnalysisP.FileLinePositionSpan"/> that contains path, line and column information.
        /// </returns>
        /// <remarks>The values are not affected by line mapping directives (<c>#line</c>).</remarks>
        public FileLinePositionSpan GetLineSpan(TextSpan span, CancellationToken cancellationToken = default(CancellationToken)) {
            return new FileLinePositionSpan(this.FilePath, GetLinePosition(span.Start), GetLinePosition(span.End));
        }

        private LinePosition GetLinePosition(int position) {
            return SourceText.Lines.GetLinePosition(position);
        }

        public void AddDiagnostic(Diagnostic diagnostic) {
            Project.AddModuleDiagnostic(ModuleName, diagnostic);
        }

        public void AddTypeToolingDiagnostic(TextSpan sourceSpan, TypeTooling.Diagnostic typeToolingDiagnostic) {
            Diagnostic diagnostic = Diagnostic.FromTypeToolingDiagnostic(this, sourceSpan, typeToolingDiagnostic);
            AddDiagnostic(diagnostic);
        }

        public void AddError(TextSpan span, string message) {
            AddDiagnostic(new Diagnostic(this, span, DiagnosticSeverity.Error, message));
        }

        public void AddError(SyntaxNode syntaxNode, string message) {
            AddDiagnostic(new Diagnostic(syntaxNode, DiagnosticSeverity.Error, message));
        }

        public SyntaxHighlightTag[] GetSyntaxHighlightTags(TextSpan[] textSpans) {
            var tags = new List<SyntaxHighlightTag>();
            new GetSyntaxHighlightTags(ModuleSyntax).GetTags(textSpans, tags);
            return tags.ToArray();
        }

        public SyntaxHighlightTag[] GetSyntaxHighlightTags(TextSpan textSpan) {
            var tags = new List<SyntaxHighlightTag>();
            new GetSyntaxHighlightTags(ModuleSyntax).GetTags(textSpan, tags);
            return tags.ToArray();
        }

        public IconTag[] GetIconTags(TextSpan[] textSpans) {
            var sourceTags = new List<IconTag>();
            new GetIconTags(ModuleSyntax).GetTags(textSpans, sourceTags);
            return sourceTags.ToArray();
        }

        public IconTag[] GetIconTags(TextSpan textSpan) {
            var sourceTags = new List<IconTag>();
            new GetIconTags(ModuleSyntax).GetTags(textSpan, sourceTags);
            return sourceTags.ToArray();
        }

        public Api.QuickInfo.QuickInfo? GetQuickInfo(int position) {
            SyntaxNode? terminalNode = ModuleSyntax.GetNextTerminalNodeFromPosition(position);
            if (terminalNode == null || !terminalNode.Span.Contains(position))
                return null;

            return QuickInfoProvider.GetQuickInfo(terminalNode);
        }

        public IntelliSenseStartData? GetIntelliSenseStartData(int position) {
            return IntelliSenseProvider.GetIntelliSense(this, position)?.GetStartData();
        }

        public async Task<IntelliSenseCompletions?> GetIntelliSenseCompletionsAsync(int position, CancellationToken cancellationToken) {
            IntelliSense.IntelliSense? intelliSense = IntelliSenseProvider.GetIntelliSense(this, position);
            if (intelliSense == null)
                return null;
            return await intelliSense.GetCompletionsAsync(cancellationToken);
        }

        public Task<ClassifiedTextMarkup?> GetIntelliSenseDescriptionAsync(object completionItemData,
            CultureInfo preferredCulture, CancellationToken cancellationToken) {
            return IntelliSenseProvider.GetDescriptionAsync(completionItemData, preferredCulture, cancellationToken);
        }
    }
}
