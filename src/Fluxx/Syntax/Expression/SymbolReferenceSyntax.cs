using Faml.Api;
using Faml.Binding;
using Faml.Binding.Internal;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression
{
    public sealed class SymbolReferenceSyntax : ExpressionSyntax
    {
        private readonly NameSyntax _name;
        private SymbolBinding _symbolBinding;

        public SymbolReferenceSyntax(TextSpan span, NameSyntax name) : base(span)
        {
            this._name = name;
            name.SetParent(this);
        }

        public NameSyntax Name => this._name;

        public Name VariableName => this._name.Name;

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            SyntaxNode ancestor = this.Parent;

            while (ancestor != null)
            {
                if (ancestor is FunctionDefinitionSyntax functionDefinition)
                {
                    int parameterIndex = functionDefinition.GetParameterIndex(this._name.Name);

                    if (parameterIndex != -1)
                    {
                        this._symbolBinding = new ParameterBinding(functionDefinition, parameterIndex);
                        return;
                    }
                }
                else if (ancestor is ForExpressionSyntax forExpressionSyntax)
                {
                    if (this._name.Name == forExpressionSyntax.ForVariableDefinition.VariableNameSyntax.Name)
                    {
                        // TODO: Fix hack that assumes there's just a single 'for' variable in a function
                        this._symbolBinding = new ForSymbolBinding(forExpressionSyntax, 0);
                        return;
                    }
                }
                else if (ancestor is ModuleSyntax)
                {
                    // TODO: Call lower level API to resolve binding, which returns null if not found
                    FunctionBinding functionBinding = bindingResolver.ResolveFunctionBinding(null, this._name.Name.ToQualifiableName(), this._name);
                    if (functionBinding != null)
                    {
                        this._symbolBinding = new FunctionSymbolBinding(functionBinding);
                        return;
                    }
                }

                ancestor = ancestor.Parent;
            }

            // Not found
            this.AddError($"Symbol '{this._name}' not found");
            this._symbolBinding = InvalidSymbolBinding.Instance;
        }

        public override TypeBinding GetTypeBinding()
        {
            return this._symbolBinding.GetTypeBinding();
        }

        public SymbolBinding GetVariableBinding() { return this._symbolBinding; }
     
        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor)
        {
            visitor(this._name);
        }

        public override void WriteSource(SourceWriter sourceWriter)
        {
            this._name.WriteSource(sourceWriter);
        }
    }
}
