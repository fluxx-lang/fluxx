using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fluxx.Api;
using Fluxx.Api.IntelliSense;
using Fluxx.DotNet;
using Fluxx.Syntax;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.Images;
using TypeTooling.Types;

namespace Fluxx.IntelliSense
{
    public class FunctionInvocationIntelliSense : IntelliSense
    {
        public FunctionInvocationIntelliSense(FamlModule module, int position)
            : base(module, position, null)
            {
        }

        public override async Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken)
        {
            DotNetRawTypeProvider dotNetRawTypeProvider = this.Module.Project.DotNetProjectInfo.RawTypeProvider;

            DotNetRawType? viewType = dotNetRawTypeProvider.GetType("Xamarin.Forms.View");
            if (viewType == null)
            {
                return IntelliSenseCompletions.Empty;
            }

            IEnumerable<DotNetRawType> types = await dotNetRawTypeProvider.FindTypesAssignableToAsync(viewType, cancellationToken);

            List<IntelliSenseCompletion> completions = new List<IntelliSenseCompletion>();
            foreach (DotNetRawType rawType in types)
            {
                TypeToolingType? type = this.Module.Project.GetTypeToolingType(rawType);
                if (type == null)
                {
                    continue;
                }

                string fullName = type.FullName;
                string name = new QualifiableName(fullName).GetLastComponent().ToString();

                string displayText = name;
                string? insertText = name + " ";

                Image? icon = null;
                if (type is ObjectType objectType)
                {
                    icon = objectType.GetIcon();
                }

                IntelliSenseProvider.GetDescriptionAsyncDelegate getDescriptionDelegate =
                    async (preferredCulture, delegateCancellationToken) =>
                        await type.GetDescriptionAsync(delegateCancellationToken);

                var completion = new IntelliSenseCompletion(
                    type: CompletionType.Property,
                    displayText: displayText,
                    insertText: insertText,
                    icon: icon,
                    data: getDescriptionDelegate);
                completions.Add(completion);
            }

            return new IntelliSenseCompletions(completions);
        }
    }
}
