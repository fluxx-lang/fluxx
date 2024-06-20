Fluxx is a *Functional* Markup Language
---------------

Traditional markup languages, like XML and JSON, don't have any kind of abstraction mechanism built
in to allow a chunk of markup that's repeated several places to be abstracted into a higher level
thing and shared. But that's commonly needed, so it's often supplied through external means, like
XML elements with special semantics (e.g. UserControl and ResourceDictionary in XAML or `<include/>`
in Anroid XML) or through template preprocessors (e.g. SASS for CSS). Arguably, though, it'd be much
better if the markup language itself supported abstraction directly, which Fluxx does.

Fluxx is a functional language and like any functional language, functions are its core unit of
abstraction. Let's look at some examples.

Here's a simple function, with another function that calls it:  
```
let <RedSquare Size:Integer /> =
    <BoxView
        Width=Size
        Height=Size
        Color=Red />

let <RedSquares/> =
    <RedSquare Size=10 />
    <RedSquare Size=20 />
    <RedSquare Size=30 />
```

Functions are defined with the syntax `let <function-name> = <function-value>`. If the function can take
parameter properties, those are listed along with their types in braces after the function name. The type syntax here (":" followed by type) is the same as that used by TypeScript,
F#, Swift, Kotlin, and many other modern languages.

Integer is a primitive type in Fluxx. If the function doesn't take any parameters, as is the case with
RedSquares, no braces are necessary.

Note that invoking a function (say `RedSquare` above) uses the exact same syntax as instantiating an
object (say `BoxView` above) and setting properties on it. In fact, instantiating an object can
actually be thought of as calling a function (called the constructor function), so conceptually they
are exactly the same thing--all functions!

Normally the function return type is inferred, by the type of the expression after the `=` sign.
But if desired you can specify the type exlicitly, providing it after a colon:

```
let <RedSquare Size:Integer/>: BoxView = 
    <BoxView
        Width=@Size
        Height=@Size
        Color=Red
    /BoxView>
```


`Width` and `Height` above specify their value as `{Size}`, with the property in braces.  Why the braces there?
Fluxx uses this basic rule for property values: The value can either be:

1. A literal.  A literal is a bunch of text, whose actual interpretation depends on the property's
type. Literal text could be a number (`Size: 10` above), an enumeration value (`Color: Red` above),
some text (`Label: my label text`), or really anything including something custom like a regular
expression.  Again a literal value is just a bunch of text (up until the next property or right
brace), but how that text is interpreted is *context sensitive*, based on the property's type.
Custom types can be added with their own literal syntax. XAML has a similar concept, with
TypeConverters, and Fluxx in fact can use XAML TypeConverters if available for the type.

2. A Fluxx expression. Syntactically, that will either start with a left brace (as is the case for
`Width: {Size}` above) or with an identifier followed by a left brace for a function call (for
instance `MyBoxView: BoxView {Color: Red}`. The optional identifier + left brace is the syntactic
clue that Fluxx should treat the value as an expression. In `Width: {Size}`, the `Size` isn't a
literal value (a literal would be a number like `20`) but instead a parameter name which substituted
with the actual value. Fluxx expressions can use operators (e.g. `Width: {Size + 20}`), call
functions, and have conditional logic, much like a traditional programming language. More about
expressions is below.

Note that the combination of literal and expression values for properties work well in making Fluxx a
*functional markup* language. Markup languages are traditionally good at declaratively defining a
bunch of (literal) data. The context sensitive nature of Fluxx literal values mean that data often be
expressed more concisely than it would be in a traditional programming language. For example
`TextColor: Green` suffices in Fluxx, since Fluxx knows `TextColor` is of Color type, whereas in a
traditional programming language you'd typically have to say something like `TextColor: Color.Green`
since it's not context aware. Literals make Fluxx a nice markup language. On the other hand, the power
to evaluate expressions and call functions when needed make it a functional language.

Here's one of the simplest possible functions:

```
MyBorderWidth = 3.0
```

In Fluxx, constants are just trivial functions. And while `MyBorderWidth` may look like a variable in traditional programming languages,
it's not a variable--it can't be reassigned. Fluxx, as a pure functional language, doesn't have variables. 

In XAML, constants are often handled by a ResourceDictionary.   Here's a comparison of XAML:

```
<ContentPage.Resources>
    <ResourceDictionary>
      <x:Double x:Key="MyBorderWidth">
        3.0
      </x:Double>
    </ResourceDictionary>
</ContentPage.Resources>

...

<Button
    Text="Do this!"
    BorderWidth="{StaticResource MyBorderWidth}"
/>
```

With the equivalent, more concise, Fluxx:

```
MyBorderWidth = 3.0

...

Button
    Text: Do this!
    BorderWidth: {MyBorderWidth}
```
In Fluxx the `if` operator serves the purpose of `if` (conditionals), `switch` (case statements), and `match` (pattern matching) in other languages.
Being a functional language, `if` always evalues to a value and can be used anywhere in a Fluxx expression.
Here are some examples:
```
if platform is
|iOS: Label Text:I'm a label on iOS!
|Android: Label Text:I'm a label on Android
```
The `|` symbol here is consistent with F# and makes the different cases stand out more visually.
Here's a traditional conditional:
```
controlWidth =
if
|screenWidth > 500 && screenOrientation == landscape: 30
|else: 20
```