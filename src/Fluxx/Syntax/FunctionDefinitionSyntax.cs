using System.Diagnostics;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Faml.Syntax.Literal;
using Faml.Syntax.Type;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax
{
    public sealed class FunctionDefinitionSyntax : DefinitionSyntax
    {
        private readonly NameSyntax functionNameSyntax;
        private readonly PropertyNameTypePairSyntax[] parameters;
        private readonly TypeReferenceSyntax? returnType;
        private TypeBinding? _returnTypeBinding;
        private ObjectIdentifiersBinding? objectIdentifiersBinding;
        //private @Nullable TypeBinding returnTypeBinding;
        private ExpressionSyntax _expression;           // Expression, forming the function body
        private readonly DefinitionSyntax[] whereDefinitions;


        public FunctionDefinitionSyntax(TextSpan span, NameSyntax functionNameSyntax, PropertyNameTypePairSyntax[] parameters,
                                        TypeReferenceSyntax? returnType, ExpressionSyntax expression, DefinitionSyntax[] whereDefinitions) : base(span)
                                        {
            this.functionNameSyntax = functionNameSyntax;
            functionNameSyntax.SetParent(this);

            this.parameters = parameters;
            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.parameters)
            {
                propertyNameTypePair.SetParent(this);
            }

            this.returnType = returnType;
            if (returnType != null)
            {
                returnType.SetParent(this);
            }

            this._expression = expression;
            expression.SetParent(this);

            this.whereDefinitions = whereDefinitions;
            foreach (DefinitionSyntax whereDefinition in whereDefinitions)
            {
                whereDefinition.SetParent(this);
            }
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            visitor(this.functionNameSyntax);

            foreach (PropertyNameTypePairSyntax propertyNameTypePair in this.parameters)
            {
                visitor(propertyNameTypePair);
            }

            if (this.returnType != null)
            {
                visitor(this.returnType);
            }

            if (this._expression != null)
            {
                visitor(this._expression);
            }

            foreach (DefinitionSyntax whereDefinition in this.whereDefinitions)
            {
                visitor(whereDefinition);
            }
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.FunctionDefinition;

        protected internal override void ResolveExplicitTypeBindings(BindingResolver bindingResolver)
        {
            if (this.returnType != null)
            {
                this._returnTypeBinding = this.returnType.GetTypeBinding();
            }
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver)
        {
            if (this._expression is TextualLiteralSyntax markupValue)
            {
                if (this._returnTypeBinding != null)
                {
                    this._expression = markupValue.ResolveMarkup(this._returnTypeBinding, bindingResolver);
                }
                else
                {
                    this.AddError("Must specify explicit return type; it can't be inferred");
                }
            }

            this._expression.SetParent(this);

            // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
            this._expression.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(bindingResolver); });

            if (this.returnType == null)
            {
                this._returnTypeBinding = this._expression.GetTypeBinding();
            }
            else
            {
                Debug.Assert(this._returnTypeBinding != null);
            }
        }

        public NameSyntax FunctionNameSyntax => this.functionNameSyntax;

        public Name FunctionName => this.functionNameSyntax.Name;

        public PropertyNameTypePairSyntax[] Parameters => this.parameters;

        /// <summary>
        /// Return the index of the specified parameter name.   If the parameter name isn't a valid parameter, -1 is
        /// returned.
        /// </summary>
        /// <param name="parameterName">parameter name in question</param>
        /// <remarks> index of parameter or -1 if the function doesn't have a parameter of that name</remarks>

        public int GetParameterIndex(Name parameterName)
        {
            int length = this.parameters.Length;
            for (int i = 0; i < length; i++)
            {
                if (this.parameters[i].PropertyName == parameterName)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public TypeBinding GetParameterTypeBinding(int parameterIndex)
        {
            return this.parameters[parameterIndex].TypeReferenceSyntax.GetTypeBinding();
        }

        public TypeReferenceSyntax ReturnType => this.returnType;

        public TypeBinding ReturnTypeBinding => this._returnTypeBinding;

        public Expression.ExpressionSyntax Expression => this._expression;

        public DefinitionSyntax[] WhereDefinitions => this.whereDefinitions;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            sourceWriter.Write(this.functionNameSyntax);
            
            int parameterCount = this.parameters.Length;
            if (parameterCount > 0)
            {
                sourceWriter.Write("{");
                for (int i = 0; i < parameterCount; i++)
                {
                    if (i > 0)
                    {
                        sourceWriter.Write(" ");
                    }

                    this.parameters[i].WriteSource(sourceWriter);
                }

                sourceWriter.Write("}");
            }

            if (this.returnType != null)
            {
                sourceWriter.Write(":");
                this.returnType.WriteSource(sourceWriter);
            }

            sourceWriter.Write(" = ");

            this._expression.WriteSource(sourceWriter);
        }
    }
}
