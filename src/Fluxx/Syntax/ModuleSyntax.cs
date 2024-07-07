using System.Collections.Generic;
using Fluxx.Api;
using Fluxx.Binding;
using Fluxx.Binding.Resolver;
using Fluxx.CodeAnalysis.Text;
using Fluxx.Syntax.Expression;
using Microsoft.CodeAnalysis.Text;
using TypeTooling;

/**
 * @author Bret Johnson
 * @since 4/13/2015
 */
namespace Fluxx.Syntax
{
    public sealed class ModuleSyntax : SyntaxNode
    {
        private readonly FamlModule module;
        //private readonly Use[] _uses;
        private readonly FunctionInvocationSyntax? projectDefinition;
        private readonly ImportSyntax[] imports;
        private readonly SyntaxNode[] moduleItem;
        private readonly Dictionary<Name, FunctionDefinitionSyntax> functionDefinitions = new Dictionary<Name, FunctionDefinitionSyntax>();
        private readonly Dictionary<Name, RecordTypeDefinitionSyntax> recordTypeDefinitions = new Dictionary<Name, RecordTypeDefinitionSyntax>();
        private readonly ExampleDefinitionSyntax[] exampleDefinitions;

        public ModuleSyntax(FamlModule module, TextSpan span, FunctionInvocationSyntax? projectDefinition, UseSyntax[] uses, ImportSyntax[] imports, SyntaxNode[] moduleItems) : base(span)
        {
            this.module = module;
            this.projectDefinition = projectDefinition;

            this.projectDefinition = projectDefinition;
            if (this.projectDefinition != null)
            {
                this.projectDefinition.SetParent(this);
            }

            this.imports = imports;
            foreach (ImportSyntax importObj in imports)
            {
                importObj.SetParent(this);
            }

            this.moduleItem = moduleItems;
            foreach (SyntaxNode moduleItem in this.ModuleItems)
            {
                moduleItem.SetParent(this);
            }

            var exampleDefinitions = new List<ExampleDefinitionSyntax>();
            foreach (SyntaxNode moduleItem in this.ModuleItems)
            {
                if (moduleItem is FunctionDefinitionSyntax functionDefinition)
                {
                    Name functionName = functionDefinition.FunctionName;

                    if (this.functionDefinitions.ContainsKey(functionName))
                    {
                        functionDefinition.FunctionNameSyntax.AddError($"Function '{functionName}' is already defined in module");
                    }
                    else if (this.recordTypeDefinitions.ContainsKey(functionName))
                    {
                        functionDefinition.FunctionNameSyntax.AddError($"There is already a record type named '{functionName}' in this module; functions and record types cannot share the same name");
                    }
                    else
                    {
                        this.functionDefinitions.Add(functionName, functionDefinition);
                    }
                }
                else if (moduleItem is RecordTypeDefinitionSyntax recordTypeDefinition)
                {
                    Name typeName = recordTypeDefinition.TypeNameSyntax.Name;

                    if (this.recordTypeDefinitions.ContainsKey(typeName))
                    {
                        recordTypeDefinition.TypeNameSyntax.AddError($"Type '{typeName}' is already defined in module");
                    }
                    else if (this.functionDefinitions.ContainsKey(typeName))
                    {
                        recordTypeDefinition.TypeNameSyntax.AddError($"There is already a function named '{typeName}' in this module; functions and record types cannot share the same name");
                    }
                    else
                    {
                        this.recordTypeDefinitions.Add(typeName, recordTypeDefinition);
                    }
                }
                else if (moduleItem is ExampleDefinitionSyntax exampleDefinition)
                {
                    int index = exampleDefinitions.Count;
                    exampleDefinitions.Add(exampleDefinition);
                    exampleDefinition.SetExampleIndex(index);
                }
            }

            this.exampleDefinitions = exampleDefinitions.ToArray();
        }

        public FamlModule Module => this.module;

        public FamlProject Project => this.module.Project;
        public QualifiableName ModuleName => this.module.ModuleName;
        public SourceText SourceText => this.module.SourceText;
        public FunctionInvocationSyntax? ProjectDefinition => this.projectDefinition;
        public ImportSyntax[] Imports => this.imports;
        public SyntaxNode[] ModuleItems => this.moduleItem;
        public Dictionary<Name, RecordTypeDefinitionSyntax> RecordTypeDefinitions => this.recordTypeDefinitions;

        public ExampleDefinitionSyntax[] ExampleDefinitions => this.exampleDefinitions;

        public FunctionDefinitionSyntax? GetFunctionDefinition(Name name)
        {
            this.functionDefinitions.TryGetValue(name, out FunctionDefinitionSyntax functionDefinition);
            return functionDefinition;
        }

        public RecordTypeDefinitionSyntax? GetRecordTypeDefinition(Name name)
        {
            this.recordTypeDefinitions.TryGetValue(name, out RecordTypeDefinitionSyntax recordTypeDefinition);
            return recordTypeDefinition;
        }

        public ExampleDefinitionSyntax GetExampleDefinitionAtIndex(int exampleIndex)
        {
            if (exampleIndex >= this.exampleDefinitions.Length)
            {
                throw new UserViewableException($"Example index {exampleIndex} doesn't exist for module {this.ModuleName}; it only has {this.exampleDefinitions.Length} examples");
            }

            return this.exampleDefinitions[exampleIndex];
        }

        public ExampleDefinitionSyntax? GetExampleDefinitionAtSourcePosition(int position)
        {
            foreach (ExampleDefinitionSyntax exampleDefinition in this.exampleDefinitions)
            {
                if (exampleDefinition.Span.Contains(position))
                {
                    return exampleDefinition;
                }
            }

            return null;
        }

        public void ResolveAllBindings()
        {
            var moduleBindingResolver = new ModuleBindingResolver(this);

            // First resolve the explicit type bindings, where the types are specified and not inferred
            this.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveExplicitTypeBindings(moduleBindingResolver); });

            // Now resolve all the bindings.  Do it postorder, bottom up, so a node can assume its descendents' bindings have
            // already been resolved when trying to resolve its own bindings.
            this.VisitNodeAndDescendentsPostorder((astNode) => { astNode.ResolveBindings(moduleBindingResolver); });
        }

        public override void VisitChildren(SyntaxVisitor visitor)
        {
            foreach (ImportSyntax import in this.imports)
            {
                visitor(import);
            }

            foreach (SyntaxNode moduleItem in this.moduleItem)
            {
                visitor(moduleItem);
            }
        }

        public override bool IsTerminalNode()
        {
            return false;
        }

        public override SyntaxNodeType NodeType => SyntaxNodeType.Module;

        public override void WriteSource(SourceWriter sourceWriter)
        {
            foreach (SyntaxNode syntaxNode in this.ModuleItems)
            {
                syntaxNode.WriteSource(sourceWriter);
            }
        }

        public TypeBinding? GetObjectTypeBinding(QualifiableName typeName)
        {
            ModuleSyntax module = this.GetModuleSyntax();

            if (!typeName.IsQualified())
            {
                Name unqualifiableTypeName = typeName.ToUnqualifiableName();

                RecordTypeDefinitionSyntax recordTypeDefinition;
                if (this.recordTypeDefinitions.TryGetValue(unqualifiableTypeName, out recordTypeDefinition))
                {
                    return recordTypeDefinition.TypeBinding;
                }

                // See if it matches any imports
                foreach (ImportSyntax import in module.Imports)
                {
                    foreach (ImportTypeReferenceSyntax importReference in import.ImportTypeReferences)
                    {
                        if (importReference.Name == unqualifiableTypeName)
                        {
                            return importReference.GetTypeBinding();
                        }
                    }
                }
            }

            return null;
        }

        public AttachedTypeBinding? GetAttachedTypeBinding(QualifiableName typeName)
        {
            ModuleSyntax module = this.GetModuleSyntax();

            if (!typeName.IsQualified())
            {
                Name unqualifiableTypeName = typeName.ToUnqualifiableName();

                // See if it matches any imports
                foreach (ImportSyntax import in module.Imports)
                {
                    foreach (ImportTypeReferenceSyntax importReference in import.ImportTypeReferences)
                    {
                        if (importReference.Name == unqualifiableTypeName)
                        {
                            return importReference.GetAttachedTypeBinding();
                        }
                    }
                }
            }

            return null;
        }
    }
}
