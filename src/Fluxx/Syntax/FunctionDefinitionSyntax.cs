using System.Diagnostics;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysisP.Text;

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
            _functionNameSyntax = functionNameSyntax;
            functionNameSyntax.SetParent(this);

            _parameters = parameters;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _parameters)
                propertyNameTypePair.SetParent(this);

            _returnType = returnType;
            if (returnType != null)
                returnType.SetParent(this);

            _expression = expression;
            expression.SetParent(this);

            _whereDefinitions = whereDefinitions;
            foreach (DefinitionSyntax whereDefinition in whereDefinitions)
                whereDefinition.SetParent(this);
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            visitor(_functionNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in _parameters)
                visitor(propertyNameTypePair);

            if (_returnType != null)
                visitor(_returnType);

            if (_expression != null)
                visitor(_expression);

            foreach (DefinitionSyntax whereDefinition in _whereDefinitions)
                visitor(whereDefinition);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.FunctionDefinition;

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {
            if (_returnType != null)
                _returnTypeBinding = _returnType.GetTypeBinding();
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            if (_expression is TextualLiteralSyntax markupValue) {
                if (_returnTypeBinding != null)
                    _expression = markupValue.ResolveMarkup(_returnTypeBinding, bindingResolver);
                else
                    AddError("Must specify explicit return type; it can't be inferred");
            }

            _expression.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            _expression.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });

            if (_returnType == null)
                _returnTypeBinding = _expression.GetTypeBinding();
            else Debug.Assert(_returnTypeBinding != null);
        }

        public NameSyntax FunctionNameSyntax => _functionNameSyntax;

        public Name FunctionName => _functionNameSyntax.Name;

        public PropertyNameTypePairSyntax[] Parameters => _parameters;

        /// <summary>
        /// Return the index of the specified parameter name.   If the parameter name isn't a valid parameter, -1 is
        /// returned.
        /// </summary>
        /// <param name="parameterName">parameter name in question</param>
        /// <remarks> index of parameter or -1 if the function doesn't have a parameter of that name</remarks>

        public int GetParameterIndex(Name parameterName) {
            int length = _parameters.Length;
            for (int i = 0; i < length; i++) {
                if (_parameters[i].PropertyName == parameterName) {
                    return i;
                }
            }
            return -1;
        }
        
        public TypeBinding GetParameterTypeBinding(int parameterIndex) {
            return _parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public TypeReferenceSyntax ReturnType => _returnType;

        public TypeBinding ReturnTypeBinding => _returnTypeBinding;

        public Expression.ExpressionSyntax Expression => _expression;

        public DefinitionSyntax[] WhereDefinitions => _whereDefinitions;

        public override void WriteSource(SourceWriter sourceWriter) {
            sourceWriter.Write(_functionNameSyntax);
            
            int parameterCount = _parameters.Length;
            if (parameterCount > 0) {
                sourceWriter.Write("{");
                for (int i = 0; i < parameterCount; i++) {
                    if (i > 0)
                        sourceWriter.Write(" ");

                    _parameters[i].WriteSource(sourceWriter);
                }

                sourceWriter.Write("}");
            }

            if (_returnType != null) {
                sourceWriter.Write(":");
                _returnType.WriteSource(sourceWriter);
            }

            sourceWriter.Write(" = ");

            _expression.WriteSource(sourceWriter);
        }
    }
}
