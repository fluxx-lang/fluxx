using System;
using System.Collections.Generic;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.External;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;

namespace Faml.Syntax.Expression {
    public class FunctionInvocationSyntax : ExpressionSyntax {
        private readonly InvocationStyle _invocationStyle;
        private readonly ExpressionSyntax _functionReference;
        private readonly ArgumentNameValuePairSyntax[] _namedArguments;
        private readonly ContentArgumentSyntax? _contentArgument;
        private FunctionBinding _functionBinding;
        private ExpressionSyntax _literalConstructorValue;


        public FunctionInvocationSyntax(TextSpan span, InvocationStyle invocationStyle, ExpressionSyntax functionReference, ArgumentNameValuePairSyntax[] namedArguments, ContentArgumentSyntax? contentArgument) : base(span) {
            _invocationStyle = invocationStyle;

            _functionReference = functionReference;
            if (functionReference == null)
                throw new Exception("Invoking a function without specifying the function name not currently supported");
            _functionReference.SetParent(this);

            _namedArguments = namedArguments;
            foreach (ArgumentNameValuePairSyntax propertyNameValuePair in _namedArguments)
                propertyNameValuePair.SetParent(this);

            _contentArgument = contentArgument;
            _contentArgument?.SetParent(this);
        }

        public InvocationStyle InvocationStyle => _invocationStyle;

        public ExpressionSyntax FunctionReference => _functionReference;

        //public ApiObjects.Name FunctionName => _functionExpression.Name;

        public ArgumentNameValuePairSyntax[] NamedArguments => _namedArguments;

        public ContentArgumentSyntax? ContentArgument => _contentArgument;

        public FunctionBinding FunctionBinding => _functionBinding;

        public QualifiableName[] GetQualifiedArgumentNames() {
            var argumentNames = new List<QualifiableName>();
            foreach (ArgumentNameValuePairSyntax argument in _namedArguments) {
                if (argument.ArgumentName.IsQualified())
                    argumentNames.Add(argument.ArgumentName);
            }

            return argumentNames.ToArray();
        }

        /// <summary>
        /// Create dictionary of all the arguments, mapping their names to values.
        /// </summary>
        /// <returns>dictionary of arguments and their expression values</returns>
        public Dictionary<QualifiableName, ExpressionSyntax> CreateArgumentsDictionary() {

            var argumentsDictionary = new Dictionary<QualifiableName, ExpressionSyntax>();
            foreach (ArgumentNameValuePairSyntax argument in _namedArguments)
                argumentsDictionary[argument.PropertySpecifier.PropertyName] = argument.Value;

            if (_contentArgument != null) {
                Name? contentProperty = _functionBinding.GetContentProperty();
                if (contentProperty == null)
                    throw new InvalidOperationException($"No content property exists for function {_functionBinding.FunctionName}");

                argumentsDictionary[contentProperty.Value.ToQualifiableName()] = _contentArgument.Value;
            }

            return argumentsDictionary;
        }

#if false
        /// <summary>
        /// Create dictionary of all the qualified arguments, mapping their names to values.  Unqualified names are not included here.
        /// </summary>
        /// <returns>dictionary of arguments and their evals (except for the ones with qualified names)</returns>
        public Dictionary<QualifiableName, ExpressionSyntax> CreateQualifiedArgumentsDictionary() {
            Name? contentParameter = _functionBinding.GetContentProperty();

            var argumentDictionary = new Dictionary<QualifiableName, ExpressionSyntax>();
            foreach (ArgumentNameValuePairSyntax argument in _namedArguments) {
                QualifiableName name = argument.GetNameOrDefault(contentParameter);
                if (!name.IsQualified())
                    continue;
                argumentDictionary[name] = argument.Value;
            }
            return argumentDictionary;
        }
#endif

        public override TypeBinding GetTypeBinding() {
            if (_functionBinding == null)
                throw new Exception($"Function binding not computed for function: {_functionReference}");
            return _functionBinding.ReturnTypeBinding;
        }

        public override bool IsTerminalNode() => false;

        public override SyntaxNodeType NodeType => SyntaxNodeType.FunctionInvocation;

        public override void VisitChildren(SyntaxVisitor visitor) {
            if (_functionReference != null)
                visitor(_functionReference);

            foreach (ArgumentNameValuePairSyntax propertyNameValuePair in _namedArguments)
                visitor(propertyNameValuePair);

            if (_contentArgument != null)
                visitor(_contentArgument);
        }

        protected internal override void ResolveBindings(BindingResolver bindingResolver) {
            if (_functionReference == null) {
                AddError("Invoking a function without specifying the function name not currently supported");
                _functionBinding = new InvalidFunctionBinding(new QualifiableName("no name"));
                return;
            }

            QualifiableName functionName;
            if (_functionReference is SymbolReferenceSyntax symbolReference)
                functionName = new QualifiableName(symbolReference.Name.ToString());
            else if (_functionReference is QualifiedSymbolReferenceSyntax qualifiedSymbolReference)
                functionName = qualifiedSymbolReference.QualifiableName;
            else {
                this.AddError($"Function reference expressions of type {_functionReference.GetType()} not currently supported");
                _functionBinding = new InvalidFunctionBinding(new QualifiableName("no name"));
                return;
            }

            _functionBinding = bindingResolver.ResolveFunctionBinding(null, functionName, _functionReference);

            // If the function binding can't be resolved, don't try to resolve anything else
            if (_functionBinding is InvalidFunctionBinding)
                return;

#if false
            // See if this is a literal constructor
            if (_arguments.Length == 1 && _arguments.First().UseDefault()) {
                _literalConstructorValue = SourceParser.ParseLiteralConstructorTextBlockExpression(GetModule(), _arguments.First().Span,
                    _functionBinding.ReturnTypeBinding);
                if (_literalConstructorValue != null) {
                    _literalConstructorValue.SetParent(this);

                    // Now resolve the bindings on what we just parsed, since the parse was in turn triggered by resolving bindings
                    _literalConstructorValue.VisitNodeAndDescendentsPostorder((syntaxNode) => { syntaxNode.ResolveBindings(bindingResolver); });

                    return;
                }
            }
#endif

            var argumentSet = new HashSet<Name>();

            foreach (ArgumentNameValuePairSyntax argumentNameValuePair in _namedArguments) {
                QualifiableName argumentName = argumentNameValuePair.ArgumentName;
                if (! argumentName.IsQualified())
                    argumentSet.Add(argumentName.ToUnqualifiableName());

                TypeBinding parameterTypeBinding = _functionBinding.ResolveArgumentTypeBinding(argumentName, argumentNameValuePair, bindingResolver);

                argumentNameValuePair.ResolveValueBindings(parameterTypeBinding, bindingResolver);

                TypeBinding argumentTypeBinding = argumentNameValuePair.Value.GetTypeBinding();
                if (parameterTypeBinding.IsValid() && argumentTypeBinding.IsValid() && !parameterTypeBinding.IsAssignableFrom(argumentTypeBinding)) {
                    string typeCheckError = $"Argument type {argumentTypeBinding.TypeName} can't be converted to parameter type {parameterTypeBinding.TypeName}";
                    // TODO: Enforce this later, once all the implicit conversions are handled ok
                    //propertyNameValuePair.value.addProblem(typeCheckError);
                }
            }

            if (_contentArgument != null) {
                Name? contentProperty = _functionBinding.GetContentProperty();

                if (contentProperty != null)
                    argumentSet.Add(contentProperty.Value);

                TypeBinding parameterTypeBinding = _functionBinding.ResolveContentArgumentTypeBinding(_contentArgument, bindingResolver);

                _contentArgument.ResolveValueBindings(parameterTypeBinding, bindingResolver);

                TypeBinding argumentTypeBinding = _contentArgument.Value.GetTypeBinding();
                if (parameterTypeBinding.IsValid() && argumentTypeBinding.IsValid() && !parameterTypeBinding.IsAssignableFrom(argumentTypeBinding)) {
                    string typeCheckError = $"Argument type {argumentTypeBinding.TypeName} can't be converted to parameter type {parameterTypeBinding.TypeName}";
                    // TODO: Enforce this later, once all the implicit conversions are handled ok
                    //propertyNameValuePair.value.addProblem(typeCheckError);
                }
            }

            // Now check for any required arguments that weren't provided
            List<Name> missingArguments = new List<Name>();
            foreach (Name parameterName in _functionBinding.GetParameters()) {
                if (! argumentSet.Contains(parameterName))
                    missingArguments.Add(parameterName);
            }

            // TODO: Also check for missing content parameter and content parameter specified twice (by default and via name)
            // TODO: Add this, once provide a way to know if args have a default value or not
#if false
            if (missingArguments.Count > 0) {
                StringBuilder missingArgumentsString = new StringBuilder();
                foreach (Util.Name missingArgument in missingArguments) {
                    if (missingArgumentsString.Length > 0)
                        missingArgumentsString.Append(", ");
                    missingArgumentsString.Append(missingArgument);
                }

                AddError($"Required arguments missing: {missingArgumentsString}");
            }
#endif
        }

        /// <summary>
        /// Functions that are literal constructors are somewhat special. For them, LiteralConstructorValue holds the value of the
        /// literal. For other kinds of functions, LiteralConstructorValue is null and the arguments are used to invoke the function
        /// to get its value.
        /// </summary>
        public ExpressionSyntax? LiteralConstructorValue => _literalConstructorValue;

        protected internal override void GetObjectIdentifiersBinding(ObjectIdentifiersBinding objectIdentifiersBinding) {
            TypeBinding returnType = _functionBinding.ReturnTypeBinding;
            if (returnType is ExternalObjectTypeBinding) {

            }

            foreach (ArgumentNameValuePairSyntax propertyNameValuePair in _namedArguments) {
                propertyNameValuePair.SetParent(this);
            }
        }

        // This is just for unit test & debugging purposes
        public void SetFunctionBinding(FunctionBinding binding) {
            _functionBinding = binding;
        }

        public override void WriteSource(SourceWriter sourceWriter) {
            // TODO: Handle non-literal function expressions, for the "this" case and general case
#if LATER
            if (_thisArgument != null) {
                sourceWriter.Write(_thisArgument);
                sourceWriter.Write(".");
            }
#endif

            if (_functionReference != null)
                sourceWriter.Write(_functionReference.ToString());

            if (_invocationStyle == InvocationStyle.Delimiter)
                sourceWriter.Write("{");
            else sourceWriter.Write("  ");

            bool wroteProperty = false;
            foreach (ArgumentNameValuePairSyntax namedArgument in _namedArguments) {
                if (wroteProperty && _invocationStyle == InvocationStyle.Delimiter)
                    sourceWriter.Write("; ");

                sourceWriter.Write(namedArgument);

                wroteProperty = true;
            }

            if (_contentArgument != null) {
                if (wroteProperty && _invocationStyle == InvocationStyle.Delimiter)
                    sourceWriter.Write("; ");

                sourceWriter.Write(_contentArgument.Value);

                wroteProperty = true;
            }

            if (_invocationStyle == InvocationStyle.Delimiter)
                sourceWriter.Write("}");

            // TODO: Fix up output formatting when line style, to indent
        }
    }
}
