using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api.IntelliSense;
using Fluxx.Syntax;
using Microsoft.CodeAnalysisP.Text;
using Microsoft.VisualStudio.Core.Imaging;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Adornments;
using TypeTooling.ClassifiedText;

namespace Fluxx.VisualStudio.IntelliSense {
    public class FamlAsyncCompletionSource : IAsyncCompletionSource {
        private readonly FamlModuleBuffer _famlModuleBuffer;

        private static readonly Guid ImageCatalogGuid = Guid.Parse("ae27a6b0-e345-4288-96df-5eaf394ee369");
        private static readonly ImageElement PropertyIcon = new ImageElement(new ImageId(ImageCatalogGuid, KnownImageIds.PropertyPublic), "Property");
        private static readonly ImageElement FunctionIcon = new ImageElement(new ImageId(ImageCatalogGuid, KnownImageIds.CustomControl), "Function");
        private static readonly ImageElement ValueIcon = new ImageElement(new ImageId(ImageCatalogGuid, KnownImageIds.Literal), "Value");

        public FamlAsyncCompletionSource(FamlModuleBuffer famlModuleBuffer) {
            _famlModuleBuffer = famlModuleBuffer;
        }

        public async Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger,
            SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken cancellationToken) {
            try {
                CompletionContext completionContext = await GetCompletionContextAsync(triggerLocation, cancellationToken);
                return completionContext;
            }
            catch(Exception e) {
                await Logger.LogErrorAsync("GetCompletionContextAsync failed", e);
                return CompletionContext.Empty;
            }
        }

        private async Task<CompletionContext> GetCompletionContextAsync(SnapshotPoint triggerLocation, CancellationToken cancellationToken) {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            FamlModule? module = _famlModuleBuffer.FamlModule;
            if (module == null)
                return CompletionContext.Empty;

            IntelliSenseCompletions intelliSenseCompletionsData =
                await module.GetIntelliSenseCompletionsAsync(triggerLocation.Position, cancellationToken);
            if (intelliSenseCompletionsData == null)
                return CompletionContext.Empty;

            var builder = ImmutableArray.CreateBuilder<CompletionItem>();
            foreach (IntelliSenseCompletion intelliSenseCompletion in intelliSenseCompletionsData.Completions) {
                ImageElement vsIcon;

                var icon = intelliSenseCompletion.Icon;
                if (icon != null)
                    vsIcon = await VsImageElementConverter.ToVsAsync(icon);
                else {
                    vsIcon = intelliSenseCompletion.Type switch
                    {
                        CompletionType.Property => PropertyIcon,
                        CompletionType.Function => FunctionIcon,
                        CompletionType.Value => ValueIcon,
                        _ => throw new InvalidOperationException($"Unknown CompletionItemType")
                    };
                }

                string displayText = intelliSenseCompletion.DisplayText;
                var item = new CompletionItem(
                    displayText: displayText,
                    source: this,
                    icon: vsIcon,
                    filters: ImmutableArray<CompletionFilter>.Empty,
                    suffix: string.Empty,
                    insertText: intelliSenseCompletion.InsertText ?? displayText,
                    sortText: displayText,
                    filterText: displayText,
                    attributeIcons: ImmutableArray<ImageElement>.Empty
                );

                object? descriptionData = intelliSenseCompletion.Data;
                if (descriptionData != null)
                    item.Properties.AddProperty("DescriptionData", descriptionData);

                builder.Add(item);
            }

            return new CompletionContext(builder.ToImmutable());
        }

        public async Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token) {
            if (!item.Properties.TryGetProperty("DescriptionData", out object descriptionData))
                return null;

            FamlModule? famlModule = _famlModuleBuffer.FamlModule;
            if (famlModule == null)
                return null;

            ClassifiedTextMarkup? descriptionMarkup =
                await famlModule.GetIntelliSenseDescriptionAsync(descriptionData, CultureInfo.CurrentUICulture, token);

            if (descriptionMarkup == null)
                return null;

            return VsClassifiedTextConverter.ToVsContent(descriptionMarkup);
        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation,
            CancellationToken token) {
            try {
                IntelliSenseStartData? intelliSenseStartData =
                    _famlModuleBuffer.FamlModule?.GetIntelliSenseStartData(triggerLocation.Position);
                if (intelliSenseStartData == null)
                    return CompletionStartData.DoesNotParticipateInCompletion;

                TextSpan applicableToSpan = intelliSenseStartData.ApplicableToSpan;
                return new CompletionStartData(CompletionParticipation.ProvidesItems,
                    new SnapshotSpan(triggerLocation.Snapshot, applicableToSpan.Start, applicableToSpan.Length));
            }
            catch (Exception e) {
                Logger.LogError("InitializeCompletion failed", e);
                return CompletionStartData.DoesNotParticipateInCompletion;
            }
        }
    }
}
