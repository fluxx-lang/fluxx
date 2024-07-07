using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Faml.Api.IntelliSense;
using Faml.Binding;
using Faml.Syntax;

namespace Faml.IntelliSense
{
    public class ValueIntelliSense : IntelliSense
    {
        private readonly TypeBinding _typeBinding;


        public ValueIntelliSense(FamlModule module, int position, TypeBinding typeBinding)
            : base(module, position, null)
            {
            this._typeBinding = typeBinding;
        }

        public override Task<IntelliSenseCompletions> GetCompletionsAsync(CancellationToken cancellationToken)
        {

            if (this._typeBinding is EnumTypeBinding enumTypeBinding)
            {
                List<IntelliSenseCompletion> completions = new List<IntelliSenseCompletion>();
                foreach (EnumValueBinding enumValueBinding in enumTypeBinding.GetValues())
                {
                    IntelliSenseCompletion completion = new IntelliSenseCompletion(
                        type: CompletionType.Value,
                        displayText: enumValueBinding.Name
                    );
                    completions.Add(completion);
                }

                return Task.FromResult(new IntelliSenseCompletions(completions));
            }
            else if (this._typeBinding == BuiltInTypeBinding.Bool)
            {
                List<IntelliSenseCompletion> completions = new List<IntelliSenseCompletion>
                {
                    new IntelliSenseCompletion(type: CompletionType.Value, displayText: "false"),
                    new IntelliSenseCompletion(type: CompletionType.Value, displayText: "true")
                };

                return Task.FromResult(new IntelliSenseCompletions(completions));
            }
            else return Task.FromResult(IntelliSenseCompletions.Empty);
        }
    }
}
