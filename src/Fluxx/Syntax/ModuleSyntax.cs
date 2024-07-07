using System.Collections.Generic;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Faml.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling;

/**
 * @author Bret Johnson
 * @since 4/13/2015
 */
namespace Faml.Syntax {
    public sealed class ModuleSyntax : SyntaxNode {
        private readonly FamlModule _module;
        //private readonly Use[] _uses;
        private readonly FunctionInvocationSyntax? _projectDefinition;
        private readonly ImportSyntax[] _imports;
        private readonly SyntaxNode[] _moduleItem;
        private readonly Dictionary<Name, FunctionDefinitionSyntax> _functionDefinitions = new Dictionary<Name, FunctionDefinitionSyntax>();
        private readonly Dictionary<Name, RecordTypeDefinitionSyntax> _recordTypeDefinitions = new Dictionary<Name, RecordTypeDefinitionSyntax>();
        private readonly ExampleDefinitionSyntax[] _exampleDefinitions;


        public ModuleSyntax(FamlModule module, TextSpan span, FunctionInvocationSyntax? projectDefinition, UseSyntax[] uses, ImportSyntax[] imports, SyntaxNode[] moduleItems) : base(span) {
            _module = module;
            _projectDefinition = projectDefinition;

            _projectDefinition = projectDefinition;
            if (_projectDefinition != null)
                _projectDefinition.SetParent(this);

            _imports = imports;
            foreach (ImportSyntax importObj in imports)
                importObj.SetParent(this);

            _moduleItem = moduleItems;
            foreach (SyntaxNode moduleItem in this.ModuleItems)
                moduleItem.SetParent(this);

            var exampleDefinitions = new List<ExampleDefinitionSyntax>();
            foreach (SyntaxNode moduleItem in this.ModuleItems) {
                if (moduleItem is FunctionDefinitionSyntax functionDefinition) {
                    Name functionName = functionDefinition.FunctionName;

                    if (_functionDefinitions.ContainsKey(functionName))
                        functionDefinition.FunctionNameSyntax.AddError($"Function '{functionName}' is already defined in module");
                    else if (_recordTypeDefinitions.ContainsKey(functionName))
                        functionDefinition.FunctionNameSyntax.AddError($"There is already a record type named '{functionName}' in this module; functions and record types cannot share the same name");
                    else _functionDefinitions.Add(functionName, functionDefinition);
                }
                else if (moduleItem is RecordTypeDefinitionSyntax recordTypeDefinition) {
                    Name typeName = recordTypeDefinition.TypeNameSyntax.Name;

                    if (_recordTypeDefinitions.ContainsKey(typeName))
                        recordTypeDefinition.TypeNameSyntax.AddError($"Type '{typeName}' is already defined in module");
                    else if (_functionDefinitions.ContainsKey(typeName))
                        recordTypeDefinition.TypeNameSyntax.AddError($"There is already a function named '{typeName}' in this module; functions and record types cannot share the same name");
                    else _recordTypeDefinitions.Add(typeName, recordTypeDefinition);
                }
                else if (moduleItem is ExampleDefinitionSyntax exampleDefinition) {
                    int index = exampleDefinitions.Count;
                    exampleDefinitions.Add(exampleDefinition);
                    exampleDefinition.SetExampleIndex(index);
                }
            }

            _exampleDefinitions = exampleDefinitions.ToArray();
        }

        public FamlModule Module => _module;

        public FamlProject Project => _module.Project;
        public QualifiableName ModuleName => _module.ModuleName;
        public SourceText SourceText => _module.SourceText;
        public FunctionInvocationSyntax? ProjectDefinition => _projectDefinition;
        public ImportSyntax[] Imports => _imports;
        public SyntaxNode[] ModuleItems => _moduleItem;
        public Dictionary<Name, RecordTypeDefinitionSyntax> RecordTypeDefinitions => _recordTypeDefinitions;

        public ExampleDefinitionSyntax[] ExampleDefinitions => _exampleDefinitions;

        public FunctionDefinitionSyntax? GetFunctionDefinition(Name name) {
            _functionDefinitions.TryGetValue(name, out FunctionDefinitionSyntax functionDefinition);
            return functionDefinition;
        }

        public RecordTypeDefinitionSyntax? GetRecordTypeDefinition(Name name) {
            _recordTypeDefinitions.TryGetValue(name, out RecordTypeDefinitionSyntax recordTypeDefinition);
            return recordTypeDefinition;
        }

        public ExampleDefinitionSyntax GetExampleDefinitionAtIndex(int exampleIndex) {
            if (exampleIndex >= _exampleDefinitions.Length)
                throw new UserViewableException($"Example index {exampleIndex} doesn't exist for module {ModuleName}; it only has {_exampleDefinitions.Length} examples");
            return _exampleDefinitions[exampleIndex];
        }

        public ExampleDefinitionSyntax? GetExampleDefinitionAtSourcePosition(int position) {
            foreach (ExampleDefinitionSyntax exampleDefinition in _exampleDefinitions)
                if (exampleDefinition.Span.Contains(position))
                    return exampleDefinition;

            return null;
        }

        public void ResolveAllBindings() {
            var moduleBindingResolver = new ModuleBindingResolver(this);

            // First resolve the explicit type bindings, where the types are specified and not inferred
            VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveExplicitTypeBindings(moduleBindingResolver); });

            // Now resolve all the bindings.  Do it postorder, bottom up, so a node can assume its descendents' bindings have
            // already been resolved when trying to resolve its own bindings.
            VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(moduleBindingResolver); });
        }

        public override void VisitChildren(SyntaxVisitor visitor) {
            foreach (ImportSyntax import in _imports)
                visitor(import);

            foreach (SyntaxNode moduleItem in _moduleItem)
                visitor(moduleItem);
        }

        public override bool IsTerminalNode() {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.Module;

        public override void WriteSource(SourceWriter sourceWriter) {
            foreach (SyntaxNode syntaxNode in this.ModuleItems) {
                syntaxNode.WriteSource(sourceWriter);
            }
        }

        public TypeBinding? GetObjectTypeBinding(QualifiableName typeName) {
            ModuleSyntax module = GetModuleSyntax();

            if (!typeName.IsQualified()) {
                Name unqualifiableTypeName = typeName.ToUnqualifiableName();

                RecordTypeDefinitionSyntax recordTypeDefinition;
                if (_recordTypeDefinitions.TryGetValue(unqualifiableTypeName, out recordTypeDefinition))
                    return recordTypeDefinition.TypeBinding;

                // See if it matches any imports
                foreach (ImportSyntax import in module.Imports) {
                    foreach (ImportTypeReferenceSyntax importReference in import.ImportTypeReferences) {
                        if (importReference.Name == unqualifiableTypeName)
                            return importReference.GetTypeBinding();
                    }
                }
            }

            return null;
        }

        public AttachedTypeBinding? GetAttachedTypeBinding(QualifiableName typeName) {
            ModuleSyntax module = GetModuleSyntax();

            if (!typeName.IsQualified()) {
                Name unqualifiableTypeName = typeName.ToUnqualifiableName();

                // See if it matches any imports
                foreach (ImportSyntax import in module.Imports) {
                    foreach (ImportTypeReferenceSyntax importReference in import.ImportTypeReferences) {
                        if (importReference.Name == unqualifiableTypeName)
                            return importReference.GetAttachedTypeBinding();
                    }
                }
            }

            return null;
        }
    }
}
