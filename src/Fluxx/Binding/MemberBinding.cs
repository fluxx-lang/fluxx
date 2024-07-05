/**
 * @author Bret Johnson
 * @since 4/15/2015
 */

using Faml.Api;

namespace Faml.Binding
{
    public class NamespaceBinding {
        public QualifiableName NamespaceName { get; }

        public NamespaceBinding(QualifiableName namespaceName) {
            NamespaceName = namespaceName;
        }
    }
}
