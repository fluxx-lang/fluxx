using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Faml.DevEnv
{
    public enum Style {
        BraceStyle, IndentStyle
    }

    public class XmlToFaml {
        private TextWriter _famlWriter;
        private int _lineNumber;
        private bool _atBeginningOfLine;
        private int _indent;
        private readonly int _scopeIndentation = 2;    // How much more each scope should be indented (e.g. 2 or 4 characters)

        public virtual string Convert(TextReader xml, int initialIndent) {
            XElement rootElement = XElement.Load(xml);

            _famlWriter = new StringWriter();
            _indent = initialIndent;
            _lineNumber = 1;
            _atBeginningOfLine = true;

            // Write out the imports, if any are present
            List<string> imports = GetImports(rootElement);
            if (imports.Count > 0) {
                foreach (string import in imports)
                    Writeln(import);

                Writeln();
            }

            WriteElement(rootElement, Style.IndentStyle);
            return _famlWriter.ToString();
        }

        protected virtual void WriteElement(XElement element, Style style) {
            XName elementName = element.Name;

            Write(elementName);
            if (style == Style.BraceStyle)
                Write("{ ");
            else Write("  ");

            // If this element goes to multiple lines, they will be indented
            int startLine = _lineNumber;
            _indent += _scopeIndentation;

            bool wroteAttribute = false;
            foreach (XAttribute attribute in element.Attributes()) {
                // Skip namespace attributes
                if (attribute.IsNamespaceDeclaration)
                    continue;

                if (wroteAttribute)
                    Write("  ");

                Write(attribute.Name);
                Write(":");
                WriteAttributeValue(attribute);
                wroteAttribute = true;
            }

            foreach (XNode node in element.Nodes()) {
                if (node is XElement childElement) {
                    string attributeName = TreatElementAsAttribute(childElement);

                    if (attributeName != null) {
                        Writeln();

                        Write(attributeName);
                        Write(":");

                        if (ShouldElementContentBeInline(element))
                            WriteElementContent(childElement, Style.BraceStyle);
                        else {
                            Writeln();
                            _indent += _scopeIndentation;
                            WriteElementContent(childElement, style);
                            _indent -= _scopeIndentation;
                        }
                    }
                }
            }

            if (ElementHasContent(element)) {
                if (ShouldElementContentBeInline(element)) {
                    if (wroteAttribute)
                        Write("  ");
                    WriteElementContent(element, Style.BraceStyle);
                }
                else {
                    Writeln();
                    WriteElementContent(element, style);
                }
            }

            _indent -= _scopeIndentation;
            if (style == Style.BraceStyle) {
                if (_lineNumber != startLine) {
                    Writeln();
                    Write("}");
                }
                else Write(" }");
            }
        }

        protected virtual bool ElementHasContent(XElement element) {
            foreach (XNode node in element.Nodes()) {
                if (node is XText)
                    return true;
                else if (node is XElement childElement && TreatElementAsAttribute(childElement) == null)
                    return true;
            }

            return false;
        }

        protected virtual bool ShouldElementContentBeInline(XElement element) {
            if (ElementHasTextualContent(element))
                return true;
            else if (ElementHasContent(element))
                return false;
            else return true;
        }

        protected virtual bool ElementHasTextualContent(XElement element) {
            foreach (XNode node in element.Nodes()) {
                if (node is XText text)
                    return true;
            }

            return false;
        }

        private void WriteElementContent(XElement element, Style style) {
            if (ElementHasTextualContent(element)) {
                foreach (XNode node in element.Nodes()) {
                    if (node is XText text)
                        Write(text.Value);
                    else if (node is XElement childElement && TreatElementAsAttribute(childElement) == null)
                        WriteElement(childElement, Style.BraceStyle);
                }
            }
            else {
                bool wroteElement = false;
                foreach (XNode node in element.Nodes()) {
                    if (node is XElement childElement && TreatElementAsAttribute(childElement) == null) {
                        if (wroteElement)
                            Writeln();
                        WriteElement(childElement, style);
                        wroteElement = true;
                    }
                }
            }
        }

        protected virtual string? TreatElementAsAttribute(XElement element) {
            return null;
        }

        protected virtual void WriteAttributeValue(XAttribute attribute) {
            Write(attribute.Value);
        }

        protected void Write(XName name) {
            Write(name.LocalName);
        }

        protected void Write(string text) {
            if (_atBeginningOfLine) {
                for (int i = 0; i < _indent; ++i)
                    _famlWriter.Write(" ");
                _atBeginningOfLine = false;
            }

            _famlWriter.Write(text);
        }

        protected void Writeln(string text) {
            Write(text);
            Writeln();
        }

        protected void Writeln() {
            _famlWriter.WriteLine();
            ++_lineNumber;
            _atBeginningOfLine = true;
        }

        protected virtual List<string> GetImports(XElement rootElement) {
            return new List<string>();
        }
    }
}
