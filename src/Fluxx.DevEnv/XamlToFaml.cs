using System.Collections.Generic;
using System.Xml.Linq;

namespace Fluxx.DevEnv
{
    public class XamlToFaml : XmlToFaml {
        protected override List<string> GetImports(XElement rootElement) {
            List<string> imports = new List<string>();
            foreach (XAttribute attribute in rootElement.Attributes()) {
                if (attribute.IsNamespaceDeclaration && attribute.Value.StartsWith("using:")) {
                    string dotNetNamespace = attribute.Value.Substring("using:".Length);

                    imports.Add("import " + dotNetNamespace);
                }
            }

            return imports;
        }

        protected override void WriteAttributeValue(XAttribute attribute) {
            string value = attribute.Value;

            string bindingPrefix = "{Binding ";
            if (value.StartsWith(bindingPrefix)) {
                value = "{" + value.Substring(bindingPrefix.Length);
            }

            Write(value);
        }

        /// <summary>
        /// Sees if the element uses property element syntax and thus should be treated as an attribute.
        /// If it does, returns the attribute name to use.
        /// 
        /// The element uses property element syntax if all of these are true:
        /// (1) the element name contains a period
        /// (2) the class name before the period matches the parent element
        /// (3) the's a nonempty string after the period.
        /// </summary>
        /// <param name="element"></param>
        /// <returns>attribute name if the element should be treated as an attribute; null if it shouldn't</returns>
        protected override string? TreatElementAsAttribute(XElement element) {
            string name = element.Name.LocalName;

            XElement parent = element.Parent;
            string parentElementName = parent.Name.LocalName;

            int periodIndex = name.IndexOf('.');

            if (periodIndex == -1)
                return null;

            string className = name.Substring(0, periodIndex);

            // Ensure that the class name (before the period) matches the parent
            if (className != parentElementName)
                return null;

            string attributeName = name.Substring(periodIndex + 1);

            // Ensure the attribute name isn't empty
            if (attributeName.Length == 0)
                return null;

            return attributeName;
        }
    }
}
