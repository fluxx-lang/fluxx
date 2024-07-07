using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Api.IntelliSense;
using Fluxx.Binding;
using Fluxx.Syntax;

namespace Fluxx.IntelliSense
{
    public class ArgumentNameIntelliSense : IntelliSense
    {
        private readonly FunctionBinding functionBinding;
        private readonly QualifiableNameSyntax? argumentName;

        public ArgumentNameIntelliSense(FamlModule module, int position, FunctionBinding functionBinding)
            : base(module, position, null)
            {
            this.functionBinding = functionBinding;
            this.argumentName = null;
        }

        public ArgumentNameIntelliSense(FamlModule module, int position, FunctionBinding functionBinding, QualifiableNameSyntax argumentName)
            : base(module, position, argumentName)
            {
            this.functionBinding = functionBinding;
            this.argumentName = argumentName;
        }

        public override Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken)
        {
            string prefix = string.Empty;
            string suffix = string.Empty;
            if (this.argumentName == null)
            {
                int spacesBefore = 0;
                if (this.ParseableSource.IsSpaceAt(this.Position - 1))
                {
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

            Name[] properties = this.functionBinding.GetParameters();

            List<IntelliSenseCompletion> completions = new List<IntelliSenseCompletion>();
            foreach (Name propertyName in properties)
            {
                string displayText = propertyName.AsString();

                string? insertText = null;
                if (prefix.Length > 0 || suffix.Length > 0)
                {
                    insertText = prefix + displayText + suffix;
                }

                IntelliSenseProvider.GetDescriptionAsyncDelegate getDescriptionDelegate =
                    async (preferredCulture, delegateCancellationToken) =>
                        await this.functionBinding.GetParameterDescriptionAsync(propertyName, delegateCancellationToken);

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
