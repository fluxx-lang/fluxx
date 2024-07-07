using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api;
using Faml.Api.IntelliSense;
using Faml.Binding;
using Faml.Syntax;

namespace Faml.IntelliSense {
    public class ArgumentNameIntelliSense : IntelliSense {
        private readonly FunctionBinding _functionBinding;
        private readonly QualifiableNameSyntax? _argumentName;


        public ArgumentNameIntelliSense(FamlModule module, int position, FunctionBinding functionBinding)
            : base(module, position, null) {
            this._functionBinding = functionBinding;
            this._argumentName = null;
        }

        public ArgumentNameIntelliSense(FamlModule module, int position, FunctionBinding functionBinding, QualifiableNameSyntax argumentName)
            : base(module, position, argumentName) {
            this._functionBinding = functionBinding;
            this._argumentName = argumentName;
        }

        public override Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken) {
            string prefix = "";
            string suffix = "";
            if (this._argumentName == null) {
                int spacesBefore = 0;
                if (this.ParseableSource.IsSpaceAt(this.Position - 1)) {
                    ++spacesBefore;
                    if (this.ParseableSource.IsSpaceAt(this.Position - 2))
                    {
                        ++spacesBefore;
                    }
                }

                prefix = this.GetSpaces(2 - spacesBefore);

                if (this.ParseableSource.IsSpaceOrNewlineAt(this.Position))
                {
                    suffix = ":";
                }
                else
                {
                    suffix = ": ";
                }
            }

            Name[] properties = this._functionBinding.GetParameters();

            List<IntelliSenseCompletion> completions = new List<IntelliSenseCompletion>();
            foreach (Name propertyName in properties) {
                string displayText = propertyName.AsString();

                string? insertText = null;
                if (prefix.Length > 0 || suffix.Length > 0)
                {
                    insertText = prefix + displayText + suffix;
                }

                IntelliSenseProvider.GetDescriptionAsyncDelegate getDescriptionDelegate =
                    async (preferredCulture, delegateCancellationToken) =>
                        await this._functionBinding.GetParameterDescriptionAsync(propertyName, delegateCancellationToken);

                var completion = new IntelliSenseCompletion(
                    type: CompletionType.Property,
                    displayText: displayText,
                    insertText: insertText,
                    data: getDescriptionDelegate);
                completions.Add(completion);
            }

            return Task.FromResult(new IntelliSenseCompletions(completions));
        }
    }
}
