using System.Diagnostics;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax {
    public sealed class FunctionDefinitionSyntax : DefinitionSyntax {
        private readonly NameSyntax _functionNameSyntax;
        private readonly PropertyNameTypePairSyntax[] _parameters;
        private readonly TypeReferenceSyntax? _returnType;
        private TypeBinding? _returnTypeBinding;
        private ObjectIdentifiersBinding? _objectIdentifiersBinding;
        //private @Nullable TypeBinding returnTypeBinding;
        private ExpressionSyntax _expression;           // Expression, forming the function body
        private readonly DefinitionSyntax[] _whereDefinitions;


        public FunctionDefinitionSyntax(TextSpan span, NameSyntax functionNameSyntax, PropertyNameTypePairSyntax[] parameters,
                                        TypeReferenceSyntax? returnType, ExpressionSyntax expression, DefinitionSyntax[] whereDefinitions) : base(span) {
            this._functionNameSyntax = functionNameSyntax;
            functionNameSyntax.SetParent(this);

            this._parameters = parameters;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._parameters)
                propertyNameTypePair.SetParent(this);

            this._returnType = returnType;
            if (returnType != null)
                returnType.SetParent(this);

            this._expression = expression;
            expression.SetParent(this);

            this._whereDefinitions = whereDefinitions;
            foreach (DefinitionSyntax whereDefinition in whereDefinitions)
                whereDefinition.SetParent(this);
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(this._functionNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this._parameters)
                visitor(propertyNameTypePair);

            if (this._returnType != null)
                visitor(this._returnType);

            if (this._expression != null)
                visitor(this._expression);

            foreach (DefinitionSyntax whereDefinition in this._whereDefinitions)
                visitor(whereDefinition);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.FunctionDefinition;

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {
            if (this._returnType != null)
                this._returnTypeBinding = this._returnType.GetTypeBinding();
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            if (this._expression is TextualLiteralSyntax markupValue) {
                if (this._returnTypeBinding != null)
                    this._expression = markupValue.ResolveMarkup(this._returnTypeBinding, bindingResolver);
                else
                    this.AddError("Must specify explicit return type; it can't be inferred");
            }

            this._expression.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            this._expression.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });

            if (this._returnType == null)
                this._returnTypeBinding = this._expression.GetTypeBinding();
            else Debug.Assert(this._returnTypeBinding != null);
        }

        public NameSyntax FunctionNameSyntax => this._functionNameSyntax;

        public Name FunctionName => this._functionNameSyntax.Name;

        public PropertyNameTypePairSyntax[] Parameters => this._parameters;

        /// <summary>
        /// Return the index of the specified parameter name.   If the parameter name isn't a valid parameter, -1 is
        /// returned.
        /// </summary>
        /// <param name="parameterName">parameter name in question</param>
        /// <remarks> index of parameter or -1 if the function doesn't have a parameter of that name</remarks>

        public int GetParameterIndex(Name parameterName) {
            int length = this._parameters.Length;
            for (int i = 0; i < length; i++) {
                if (this._parameters[i].PropertyName == parameterName) {
                    return i;
                }
            }
            return -1;
        }
        
        public TypeBinding GetParameterTypeBinding(int parameterIndex) {
            return this._parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public TypeReferenceSyntax ReturnType => this._returnType;

        public TypeBinding ReturnTypeBinding => this._returnTypeBinding;

        public Expression.ExpressionSyntax Expression => this._expression;

        public DefinitionSyntax[] WhereDefinitions => this._whereDefinitions;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(this._functionNameSyntax);
            
            int parameterCount = this._parameters.Length;
            if (parameterCount > 0) {
                sourceWriter.Write("{");
                for (int i = 0; i < parameterCount; i++) {
                    if (i > 0)
                        sourceWriter.Write(" ");

                    this._parameters[i].WriteSource(sourceWriter);
                }

                sourceWriter.Write("}");
            }

            if (this._returnType != null) {
                sourceWriter.Write(":");
                this._returnType.WriteSource(sourceWriter);
            }

            sourceWriter.Write(" = ");

            this._expression.WriteSource(sourceWriter);
        }
    }
}
