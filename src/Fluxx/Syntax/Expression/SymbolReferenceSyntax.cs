using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.Binding.Internal;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Fluxx.Syntax.Expression
{
    public sealed class SymbolReferenceSyntax : ExpressionSyntax
    {
        private readonly NameSyntax name;
        private SymbolBinding symbolBinding;

        public SymbolReferenceSyntax(TextSpan span, NameSyntax name) : base(span)
        {
            this.name = name;
            name.SetParent(this);
        }

        public NameSyntax Name => this.name;

        public Name VariableName => this.name.Name;

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            SyntaxNode ancestor = this.Parent;

            while (ancestor != null)
            {
                if (ancestor is FunctionDefinitionSyntax functionDefinition)
                {
                    int parameterIndex = functionDefinition.GetParameterIndex(this.name.Name);

                    if (parameterIndex != -1)
                    {
                        this.symbolBinding = new ParameterBinding(functionDefinition, parameterIndex);
                        return;
                    }
                }
                else if (ancestor is ForExpressionSyntax forExpressionSyntax)
                {
                    if (this.name.Name == forExpressionSyntax.ForVariableDefinition.VariableNameSyntax.Name)
                    {
                        // TODO: Fix hack that assumes there's just a single 'for' variable in a function
                        this.symbolBinding = new ForSymbolBinding(forExpressionSyntax, 0);
                        return;
                    }
                }
                else if (ancestor is ModuleSyntax)
                {
                    // TODO: Call lower level API to resolve binding, which returns null if not found
                    FunctionBinding functionBinding = bindingResolver.ResolveFunctionBinding(null, this.name.Name.ToQualifiableName(), this.name);
                    if (functionBinding != null)
                    {
                        this.symbolBinding = new FunctionSymbolBinding(functionBinding);
                        return;
                    }
                }

                ancestor = ancestor.Parent;
            }

            // Not found
            this.AddError($"Symbol '{this.name}' not found");
            this.symbolBinding = InvalidSymbolBinding.Instance;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this.symbolBinding.GetTypeBinding();
        }

        public SymbolBinding GetVariableBinding() { return this.symbolBinding; }
     
        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this.name);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            this.name.WriteSource(sourceWriter);
        }
    }
}
