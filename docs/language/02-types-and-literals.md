---
title: Types and Literals
layout: default
parent: Language
nav_order: 2
---

Types and Literals
-----

### Built-in Types

Here are the built-in types that Fluxx supports:

| Type| Example literal(s) | Range/notes |
| ------------- | ------------- | -----------|
| `bool` | `true`, `false` |
| `byte` | `42` | 0 to 255 (unsigned 8-bit integer)
| `sbyte` | `-42` | -128 to 127 (signed 8-bit integer)
| `char` | `'a'` `'$'` `'\u006A'` | U+0000 to U+FFFF (16-bit character)
| `int` | `42` | -2,147,483,648 to 2,147,483,647 (signed 32-bit integer)
| `uint` | `42` | 0 to 4,294,967,295 (unsigned 32-bit integer)
| `long` | `42` | -9,223,372,036,854,775,808 to 9,223,372,036,854,775,807 (signed 64-bit integer)
| `ulong` | `42` | 	0 to 18,446,744,073,709,551,615 (unsigned 64-bit integer)
| `double` | `1.42` | 64-bit floating point
| `float` | `1.42` | 32-bit floating point
| `string` | `"foo"` | Unicode string
| `uitext` | `<uitext id=BackLabel; Go back/>` | Localizable text (see below)

Most of these data types should be familiar, especially to C# developers.

### Localizable text

Fluxx is unique in that it has a special data type, `uitext`, for localizable text. It allows
for localizable UI with the UI text embedded directly in the UI markup.

`uitext` has an optional `id` property for a resource string ID and `description` property
for a comment:

```
<input type=button
    value=<uitext Go back/>
/input>

<input type=button
    value=<uitext id=BackLabel; description=Label for nav bar back button; Go back/>
/input>
```

Fluxx build tooling can automatically generate localizable resource files from the `uitext` literals.

Treating localizable text as a separate data type from `string` has other advantages too. It means the tooling knows what's
localizable and what isn't and the type checker can help ensure that the developer doesn't make mistakes, failing to localize something that should be translated. It also allows the editor to show UI text in a different
visual style.

