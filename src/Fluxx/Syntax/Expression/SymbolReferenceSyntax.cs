using Faml.Api;
using Faml.Binding;
using Faml.Binding.Internal;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public sealed class SymbolReferenceSyntax : ExpressionSyntax {
        private readonly NameSyntax _name;
        private SymbolBinding _symbolBinding;

        public SymbolReferenceSyntax(TextSpan span, NameSyntax name) : base(span) {
            _name = name;
            name.SetParent(this);
        }

        public NameSyntax Name => _name;

        public Name VariableName => _name.Name;

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            SyntaxNode ancestor = this.Parent;

            while (ancestor != null) {
                if (ancestor is FunctionDefinitionSyntax functionDefinition) {
                    int parameterIndex = functionDefinition.GetParameterIndex(_name.Name);

                    if (parameterIndex != -1) {
                        _symbolBinding = new ParameterBinding(functionDefinition, parameterIndex);
                        return;
                    }
                }
                else if (ancestor is ForExpressionSyntax forExpressionSyntax) {
                    if (_name.Name == forExpressionSyntax.ForVariableDefinition.VariableNameSyntax.Name) {
                        // TODO: Fix hack that assumes there's just a single 'for' variable in a function
                        _symbolBinding = new ForSymbolBinding(forExpressionSyntax, 0);
                        return;
                    }
                }
                else if (ancestor is ModuleSyntax) {
                    // TODO: Call lower level API to resolve binding, which returns null if not found
                    FunctionBinding functionBinding = bindingResolver.ResolveFunctionBinding(null, _name.Name.ToQualifiableName(), _name);
                    if (functionBinding != null) {
                        _symbolBinding = new FunctionSymbolBinding(functionBinding);
                        return;
                    }
                }

                ancestor = ancestor.Parent;
            }

            // Not found
            this.AddError($"Symbol '{_name}' not found");
            _symbolBinding = InvalidSymbolBinding.Instance;
        }

        public override TypeBinding GetTypeBinding() {
            return _symbolBinding.GetTypeBinding();
        }

        public SymbolBinding GetVariableBinding() { return _symbolBinding; }
     
        public override bool IsTerminalNode() { return false; }

        public override SyntaxNodeType NodeType => SyntaxNodeType.SymbolReference;

        public override void VisitChildren(SyntaxNode.SyntaxVisitor visitor) {
            visitor(_name);
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            _name.WriteSource(sourceWriter);
        }
    }
}
