using System;
using System.Runtime.CompilerServices;
using Faml.Api;
using Faml.Binding;
using Faml.Binding.Resolver;
using Faml.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.Text;


namespace Faml.Syntax
{
    public abstract class SyntaxNode
    {
        private SyntaxNode _parent;
        private readonly TextSpan _span;


        protected SyntaxNode(TextSpan span)
        {
            this._span = span;
        }

        public virtual SyntaxNode Parent => this._parent;

        /// <summary>
        /// The absolute span of this node in characters, including its leading and trailing trivia.
        /// </summary>
        public TextSpan FullSpan => this._span;

        /// <summary>
        /// The absolute span of this node in characters, not including its leading and trailing trivia.
        /// </summary>
        public TextSpan Span => this.FullSpan;

        public bool OverlapsWith(TextSpan span)
        {
            return this.FullSpan.OverlapsWith(span);
        }

        /// <summary>
        /// Add an error to the programs diagnostic set (with parsing errors, etc.).  Note that this method CANNOT be
        /// called from the SyntaxNode's constructor, as the parent hasn't been set yet in order to get the module.
        /// </summary>
        /// <param name="message">diagnostic message, for the user</param>
        public void AddError(string message)
        {
            this.GetModule().AddError(this, message);
        }

        public virtual void SetParent(SyntaxNode parent)
        {
            this._parent = parent;
        }

        public virtual object GetPropertyValue(AstProperty property)
        {
            throw new Exception("Invalid property " + property.Name + " for object " + this.ToString());
        }

        public override string ToString()
        {
            var sourceWriter = new SourceWriter();
            this.WriteSource(sourceWriter);
            return sourceWriter.ToString();
        }

        /// <summary>
        /// Two AswNodes are equal if they are the same object.
        /// </summary>
        /// <param name="other">object to compare against</param>
        /// <returns>true if both objects are equal</returns>
        public override bool Equals(object other)
        {
            return ReferenceEquals(this, other);
        }

        /// <summary>
        /// Since AST nodes are considered equal only if they are the same object, GetHashCode uses object identity.
        /// </summary>
        /// <returns>hash code for AST node</returns>
        public override int GetHashCode()
        {
            return RuntimeHelpers.GetHashCode(this);
        }

        public ModuleSyntax GetModuleSyntax()
        {
            SyntaxNode currentNode = this;
            while (currentNode != null)
            {
                if (currentNode is ModuleSyntax moduleSyntax)
                {
                    return moduleSyntax;
                }

                currentNode = currentNode.Parent;
            }

            throw new Exception("No enclosing module");
        }

        public FamlModule GetModule() => this.GetModuleSyntax().Module;

        public FamlProject GetProject()
        {
            return this.GetModuleSyntax().Project;
        }

        public FunctionDefinitionSyntax GetEnclosingFunctionDefinition()
        {
            SyntaxNode currentNode = this;
            while (currentNode != null)
            {
                if (currentNode is FunctionDefinitionSyntax)
                {
                    return (FunctionDefinitionSyntax) currentNode;
                }

                currentNode = currentNode.Parent;
            }

            return null;
        }

        protected internal virtual void ParsePropertyValues() {}
        
        protected internal virtual void ResolveExplicitTypeBindings(BindingResolver bindingResolver) {}

        /// <summary>
        /// Resolve the bindings for this node. Bindings are resolved postorder, bottom up. So the implementor can
        /// assume that it's descendent bindings were already attempted to be resolved.
        /// </summary>
        /// <param name="bindingResolver">resolver to use when resolving bindings</param>
        protected internal virtual void ResolveBindings(BindingResolver bindingResolver) {}

        /// <summary>
        /// Get the object identiers for this node and its descendents, adding them to objectIdentifiersBinding.
        /// </summary>
        /// <param name="objectIdentifiersBinding">Collection of object identifiers and their types</param>
        protected internal virtual void GetObjectIdentifiersBinding(ObjectIdentifiersBinding objectIdentifiersBinding)
        {
            this.VisitChildren((astNode) => { astNode.GetObjectIdentifiersBinding(objectIdentifiersBinding); });
        }

        public delegate void SyntaxVisitor(SyntaxNode node);

        public virtual void VisitChildren(SyntaxVisitor visitor) {}

        public virtual bool IsTerminalNode() => true;

        public abstract SyntaxNodeType NodeType
        {
            get;
        }

        public SyntaxNode[] GetChildren()
        {
            int childCount= 0;
            this.VisitChildren((child) => ++childCount);

            SyntaxNode[] children = new SyntaxNode[childCount];
            int index = 0;
            this.VisitChildren((child) =>
            {
                children[index++] = child;
            });
            return children;
        }

        public SyntaxNode? GetNextTerminalNodeFromPosition(int position)
        {
            if (position >= this.Span.End)
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "position is past the end of the node");
            }

            if (this.IsTerminalNode())
            {
                return this;
            }
            else
            {
                SyntaxNode? terminalNodeAtPosition = null;
                this.VisitChildren((child) =>
                {
                    if (terminalNodeAtPosition == null && position < child.Span.End)
                    {
                        terminalNodeAtPosition = child.GetNextTerminalNodeFromPosition(position);
                    }
                });
                return terminalNodeAtPosition;
            }
        }

        public SyntaxNode? GetPreviousTerminalNodeFromPosition(int position)
        {
            // The invariant here is that the position is always contained by the span or after it

            if (position < this.Span.Start)
            {
                throw new ArgumentOutOfRangeException(nameof(position), position, "position is before the start the node");
            }

            if (this.IsTerminalNode())
            {
                return this;
            }
            else
            {
                SyntaxNode? terminalNodeAtPosition = null;
                SyntaxNode? previousChildToCheck = null;
                this.VisitChildren((child) =>
                {
                    if (terminalNodeAtPosition == null && position >= child.Span.Start)
                    {
                        if (child.Span.Contains(position))
                        {
                            terminalNodeAtPosition = child.GetPreviousTerminalNodeFromPosition(position);
                        }
                        else
                        {
                            previousChildToCheck = child;
                        }
                    }
                });

                if (terminalNodeAtPosition != null)
                {
                    return terminalNodeAtPosition;
                }
                else if (previousChildToCheck != null)
                {
                    return previousChildToCheck.GetPreviousTerminalNodeFromPosition(position);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the bottom most node at the specified position. If the position is on a terminal node, that's returned.
        /// Otherwise, the most specific non-terminal that contains the position is returned.
        ///
        /// Note that a position that's at the end of a node is considered to be included. In the case where two nodes
        /// are butted up together and the position is between them, the position is thus considered to be included
        /// in both nodes; in this case the leftmost node is returned. This logic works well for IntelliSense, where
        /// (in many cases) we want IntelliSense to work at the beginning/middle/end of a node.
        /// </summary>
        /// <param name="position">position in question</param>
        /// <returns>most specific descendent SyntaxNode containing the position</returns>
        public SyntaxNode GetNodeAtPosition(int position)
        {
            if (! this.Span.ContainsInclusiveEnd(position))
                throw new ArgumentOutOfRangeException(nameof(position), position,
                    $"position {position} is outside the node's range of {this.Span.Start} - {this.Span.End}");

            if (this.IsTerminalNode())
            {
                return this;
            }
            else
            {
                SyntaxNode? descendentNodeAtPosition = null;
                this.VisitChildren((child) =>
                {
                    if (descendentNodeAtPosition == null && child.Span.ContainsInclusiveEnd(position))
                    {
                        descendentNodeAtPosition = child.GetNodeAtPosition(position);
                    }
                });

                return descendentNodeAtPosition ?? this;
            }
        }

        public void VisitNodeAndDescendentsPreorder(SyntaxVisitor visitor)
        {
            visitor(this);
            this.VisitChildren((childNode) => childNode.VisitNodeAndDescendentsPreorder(visitor));
        }

        public void VisitNodeAndDescendentsPostorder(SyntaxVisitor visitor)
        {
            this.VisitChildren((childNode) => childNode.VisitNodeAndDescendentsPostorder(visitor));
            visitor(this);
        }

        public abstract void WriteSource(SourceWriter sourceWriter);
    }
}
