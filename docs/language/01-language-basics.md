---
title: Language Basics
layout: default
parent: Language
nav_order: 1
---

Fluxx Language Basics
--------------

Fluxx can be thought of as an evolved, functional XML. Below are some of the key differences from XML.

XML elements have attributes, which are string values. In Fluxx, attributes are called properties
and they are strongly typed - no quotes.

XML:
```
<element property="42" />
```

Fluxx:
```
<element property=42 />
```

Property values can contain arbitrary markup - unlike XML, markup isn't restricted to just the element content:
```
<element
    property1=<child childprop=42 />
    property2=<child childprop=43 />
    />
```

Multiple properties specified on the same line are separated with a semicolon:
```
<element property1=42; property2=43 />
<element
    property1=42
    property2=43 />
```

Elements can have a default property, with no `property=` prefix, specified after all the named properties. This is similar to element content in XML. A semicolon separates the final named property from the default property.
```
<element property=42;
    <child childprop=43 />
...
```

XML closes an element differently depending on whether or not it has content. In Fluxx, elements are always closed with `/>`, optionally including the element name, `/element>`:
```
<element property=42 />

<element property=42;
    <child childprop=43 />
/element>
...
```

By convention the `/element>` syntax is used when closing the element on a separate line.
With this syntax the element name at the start and end are vertically aligned.
