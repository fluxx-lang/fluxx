using Faml.Api;

namespace Faml.Binding
{
    public class NamespaceBinding
    {
        public QualifiableName NamespaceName { get; }

        public NamespaceBinding(QualifiableName namespaceName)
        {
            this.NamespaceName = namespaceName;
        }
    }
}
