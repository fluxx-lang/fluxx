using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TypeTooling.DotNet.RawTypes;
using TypeTooling.DotNet.Types;
using TypeTooling.Images;
using TypeTooling.Types;
using TypeTooling.Xaml.UwpWpf;

namespace TypeTooling.Xaml {
    public class XamlObjectType : DotNetObjectType {
        private readonly XamlTypeToolingProvider _xamlTypeToolingProvider;

        public XamlObjectType(XamlTypeToolingProvider xamlXamlTypeToolingProvider, DotNetRawType rawType, DotNetRawType? companionType) :
            base(xamlXamlTypeToolingProvider.TypeToolingEnvironment, rawType, companionType) {
            _xamlTypeToolingProvider = xamlXamlTypeToolingProvider;
        }

        public XamlTypeToolingProvider XamlTypeToolingProvider => _xamlTypeToolingProvider;

        protected sealed override ObjectProperty? GetContentProperty(List<ObjectProperty> objectProperties, object? companionTypeObject) {
            ObjectProperty? contentProperty = GetContentPropertyForMe(objectProperties, companionTypeObject, out bool explicitlyUnset);

            if (contentProperty != null || explicitlyUnset)
                return contentProperty;

            // If the current type doesn't specify a ContentProperty, check its base type
            return GetBaseDotNetObjectType()?.ContentProperty;
        }

        private ObjectProperty? GetContentPropertyForMe(List<ObjectProperty> objectProperties, object? companionTypeObject, out bool explicitlyUnset) {
            explicitlyUnset = false;

            // If the companion class provides a content property, let it take precedence
            ObjectProperty? contentProperty = GetContentPropertyFromCompanion(objectProperties, companionTypeObject, out bool companionContentPropertyExplicitlyUnset);

            if (contentProperty != null || companionContentPropertyExplicitlyUnset) {
                explicitlyUnset = companionContentPropertyExplicitlyUnset;
                return contentProperty;
            }

            // Next check the synthetic attributes
            string? contentPropertyName = null;
            foreach (object attribute in XamlTypeToolingProvider.GetSyntheticCustomAttributes(RawType.FullName)) {
                if (attribute is CustomAttributes.ContentPropertyAttribute contentPropertyAttribute) {
                    contentPropertyName = contentPropertyAttribute.Name;
                    break;
                }
            }

            if (contentPropertyName == null)
                contentPropertyName = GetContentPropertyNameFromAttributes(RawType.GetCustomAttributes());

            if (contentPropertyName == null)
                return null;

            // The empty string means that there's explicitly no content property; it can be used to remove the
            // content property set on a base class
            if (contentPropertyName.Length == 0) {
                explicitlyUnset = true;
                return null;
            }

            // We should always find a match here
            foreach (ObjectProperty property in objectProperties) {
                if (property.Name == contentPropertyName)
                    return property;
            }

            return null;
        }

        protected sealed override CustomLiteralParser? GetObjectCustomLiteralParser(List<ObjectProperty> objectProperties, object? companionTypeObject) {
            CustomLiteralParser? customLiteralParser = base.GetObjectCustomLiteralParser(objectProperties, companionTypeObject);

            // If the companion class provides a custom literal manager, let it take precedence
            if (customLiteralParser != null)
                return customLiteralParser;

            // Next check the synthetic attributes
            customLiteralParser = GetCustomLiteralManagerFromSyntheticAttributes(
                XamlTypeToolingProvider.GetSyntheticCustomAttributes(RawType.FullName));
            if (customLiteralParser != null)
                return customLiteralParser;

            // Next check the synthetic attributes
            foreach (object attribute in XamlTypeToolingProvider.GetSyntheticCustomAttributes(RawType.FullName)) {
                if (attribute is CustomAttributes.CustomLiteralParserAttribute customLiteralManagerAttribute)
                    return customLiteralManagerAttribute.CustomLiteralParser;
                else if (attribute is CustomAttributes.TypeConverterAttribute typeConverterAttribute) {
                    TypeConverter typeConverter = typeConverterAttribute.TypeConverter;
                    return new UwpWpfCustomLiteralParser(XamlTypeToolingProvider, typeConverter);
                }
            }

            // Finally check the real attributes
            return GetCustomLiteralManagerFromAttributes(RawType.GetCustomAttributes());
        }

        protected sealed override CustomLiteralParser? GetPropertyCustomLiteralParser(DotNetRawProperty rawProperty, object? companionTypeObject) {
            CustomLiteralParser? customLiteralParser = base.GetPropertyCustomLiteralParser(rawProperty, companionTypeObject);

            // First check if the base class provides a CustomLiteralParser, via the companion object; if present, that takes precedence
            if (customLiteralParser != null)
                return customLiteralParser;

            // Next check the synthetic attributes
            customLiteralParser = GetCustomLiteralManagerFromSyntheticAttributes(
                XamlTypeToolingProvider.GetSyntheticCustomAttributes(RawType.FullName, rawProperty.Name));
            if (customLiteralParser != null)
                return customLiteralParser;

            // Finally check the real attributes
            return GetCustomLiteralManagerFromAttributes(rawProperty.GetCustomAttributes());
        }

        private CustomLiteralParser? GetCustomLiteralManagerFromSyntheticAttributes(IEnumerable<object> attributes) {
            foreach (object attribute in attributes) {
                if (attribute is CustomAttributes.CustomLiteralParserAttribute customLiteralManagerAttribute)
                    return customLiteralManagerAttribute.CustomLiteralParser;
                else if (attribute is CustomAttributes.TypeConverterAttribute typeConverterAttribute) {
                    TypeConverter typeConverter = typeConverterAttribute.TypeConverter;
                    return new UwpWpfCustomLiteralParser(XamlTypeToolingProvider, typeConverter);
                }
            }

            return null;
        }

        protected virtual CustomLiteralParser? GetCustomLiteralManagerFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            return null;
        }

        protected virtual string? GetContentPropertyNameFromAttributes(IEnumerable<DotNetRawCustomAttribute> attributes) {
            return null;
        }

        /// <summary>
        /// Get the value of the specified attribute. The value considered the constructor argument with the specified name, if
        /// the attribute constructor has named arguments, or the first positional argument.
        /// </summary>
        /// <param name="attribute">attribute in question</param>
        /// <param name="argumentName">constructor arg name, with attribute value</param>
        /// <returns>attribute value or null if not found</returns>
        private object? GetAttributeValue(CustomAttributeData attribute, string argumentName) {
            // First, see if the value was specified via a named argument
            foreach (CustomAttributeNamedArgument namedArgument in attribute.NamedArguments) {
                if (namedArgument.MemberName == argumentName)
                    return namedArgument.TypedValue.Value;
            }

            IList<CustomAttributeTypedArgument> arguments = attribute.ConstructorArguments;
            if (arguments.Count < 1)
                return null;

            return arguments[0].Value;
        }

        public override Image? GetIcon() {
            if (_toolboxIcons == null)
                ReadToolboxIcons();

            if (!_toolboxIcons!.TryGetValue(FullName, out Image icon))
                return null;

            return icon;
        }

        private static Dictionary<string, Image>? _toolboxIcons;

        public void ReadToolboxIcons() {
            _toolboxIcons = new Dictionary<string, Image>();

            var document = new XmlDocument() { XmlResolver = null };
            var reader = new XmlTextReader("c:\\temp\\toolbox\\Xamarin.Forms.toolbox.xml") { DtdProcessing = DtdProcessing.Prohibit };
            document.Load(reader);

            XmlNodeList? categoryNodes = document.SelectSingleNode("Catalog")?.SelectNodes("Category");
            if (categoryNodes == null)
                return;

            foreach (XmlNode categoryNode in categoryNodes) {
                XmlNodeList? itemNodes = categoryNode.SelectNodes("Item");
                if (itemNodes == null)
                    return;

                foreach (XmlNode itemNode in itemNodes) {
                    if (itemNode != null)
                        ReadToolboxIcon(itemNode);
                }
            }
        }

        static void ReadToolboxIcon(XmlNode node) {
            XmlAttributeCollection? attributes = node.Attributes;
            if (attributes == null)
                return;

            //string? categoryAttribute = categoryNode.Attributes?["Name"]?.Value;
            string? classValue = attributes["Class"]?.Value;
            string? nameValue = attributes["Name"]?.Value;
            string? imageValue = attributes["Image"]?.Value;
            //var snippet = node.FirstChild?.InnerXml;

            if (classValue == null || imageValue == null)
                return;
            
            byte[] imageBytes;
            try {
                imageValue = imageValue.Replace(".png", "-16.png");

                var path = $"c:\\temp\\toolbox\\{imageValue}";
                imageBytes = File.ReadAllBytes(path);
            }
            catch (FileNotFoundException) {
                return;
            }

            var icon = new PngImage(imageBytes, nameValue);
            _toolboxIcons!.Add(classValue, icon);
        }
    }
}
