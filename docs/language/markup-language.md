Fluxx is a *Markup* Language
---------------

Fluxx is a markup language, like XML, JSON, and YAML. It's syntax is intended to be intuitive
to users of those other markup languages and to less technical technical users (e.g. designers
doing UI prototypes).  Fluxx avoids as much syntactic "cruft" it can, to make it easier to understand
and more concise.

Let's look at some basic syntax:

```
<Cat
    name=Fluffy Smithers
    age=3
    breed=Siamese
/Cat>
```

Note that Fluxx has syntax much like JSON or CSS or YAML, where objects consist of a set of property name/value pairs. But unlike JSON no quotes are used on the property names. Nor are there comma delimiters between property elements. That makes things a little more concise and avoids the JSON annoyance of missing a comma when adding/removing items at the end.

```
<Person
    name=Bob Smithers
    pets=
      <Cat name=Fluffy Smithers; age=3 />
      <Dog name=Prince Smithers;  age=2 />
/Person>
```

In Fluxx property values aren't quoted. Instead, a property's value continues until
a semi colon, end of line, or element close `/>`.

There are a few rationales for not using quotes. It makes for a lighter weight syntax. It's more appropriate for document oriented usages of Fluxx where text can be several paragraphs long (HTML doesn't use quotes for document text). And most interestingly note
that Fluxx property values are strongly typed, with the type determined by the property name (the context).
There's not just one "string" type, there can separate types for regular expressions, file paths, localizable text (see below), non-
localizable text, etc. Types are user extensible and can define more precise rules for compile time syntax validation, syntax highlighting, and editing tooling.
So a "Color" type can allow string-like values of `red` or `#C0C0C0`, validate the syntax for them, highlight that syntax by displaying the actual color, and provide 
in editor popup color picker tooling to change the color value. All of which is extensible, for custom types. At least that's the concept, though type specific
in editor tooling isn't yet implemented.

Textual properties can be multiline.

```
<Cat
    name=Fluffy Smithers
    age=3
    description=
        Fluffy is a quet, affectionate, inquisitive cat. She can be a bit lazy.
        She's 3 years old. Her previous owner moved and had to give her up.
/Cat>
```

All leading and trailing whitespace is ignored.  Whitespace at the beginning and end of lines is also ignored, so it's easy
to include long runs of text (e.g. there's no need to left align everything when you have multiple lines like there is in XAML).

Another difference from JSON, where Fluxx is more powerful and more-XML like, is that Fluxx supports objects embedded in text.
So Fluxx, like XML, can be used for document markup.  Here's an example:


```
<Cat
    name=Fluffy
    age=3

    description=
        Fluffy is a quiet, affectionate, inquisitive cat. Though she can be a <em little bit/> lazy.
        She's 3 years old. Her previous owner moved and had to give her up.

    likes=
        Fluffy's favorite toys are
        the <Toy name=Feather Teaser; link=www.kongcompany.com/products/cats/active-toys/teasers/feather-teaser/>
        and the <Toy name=Laser pointer; link=www.kongcompany.com/products/cats/active-toys/teasers/kong-laser/>.

    siblings=
        <Cat name=Chester; age=3 />
        <Cat name=Francis; age=4 />
        <Cat name=Bozo; age=3 />
/Cat>
```

Note something else about the above, where Fluxx is arguably more powerful & consistent than XML. XML allows data for an element to
be specified in two different ways--as an attribute (which can only be plain text) or as content with
child elements optionally mixed with text (but you can only have one of those). Fluxx avoids that inconsistency and allows any
of its properties to contain rich data, as is the case with 'description', 'likes', and 'siblings' above.

Fluxx has one other rule here that comes into play here: An object can have a single property that's designated as the default property. For example:

```
<em my text to emphasize/> 
<b weight=heavy; my text to emphasize/>
```

XAML had to work around the XML limitation that attributes can only contain plain text by adding something called "property elements," an alternate
syntax where any property can also be specified as element (whose name contains a period) when its value no longer works as a plain text attribute.
But that has a cost in complexity and verbosity. A better approach arguably is just to fix this limitation in the markup language itself, like Fluxx does. 

Compare the XAML that needs to use property elements (this example is for .NET MAUI):

```
<Label LineBreakMode="NoWrap">
    <Label.FormattedText>
        <FormattedString>
            <Span Text="Phone #" FontAttributes="Bold"/>
            <Span Text="403-555-1212" ForegroundColor="Red" />
        <Label.FormattedText>
    </Label.FormattedText>
</Label>
```

with the equivalent Fluxx:
```
<Label
    LineBreakMode=NoWrap
    FormattedText=
        <FormattedString
            <Span Text=Phone #; FontAttributes=Bold/>
            <Span Text=403-555-1212; ForegroundColor=Red/>
        /FormattedString>
/Label>
```

Or joining the lines and using the fact that Text is the default property for Span:
```
<Label
    LineBreakMode=NoWrap
    FormattedText=
        <FormattedString <Span FontAttributes=Bold; Phone #/> <Span ForegroundColor=Red; 403-555-1212/> />
/Label>
```

Here's another XAML comparison, with a place property elements are often used--with Grids:

```
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="*" />
        <RowDefinition Height="100" />
    </Grid.RowDefinitions>
    
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="Auto" />
        <ColumnDefinition Width="*" />
        <ColumnDefinition Width="100" />
    </Grid.ColumnDefinitions>
    
    ...
</Grid>
```

And the equivalent Fluxx, with no need for property elements:
```
<Grid
    RowDefinitions=
        <RowDefinition Height=Auto />
        <RowDefinition Height=* />
        <RowDefinition Height=100 />

    ColumnDefinitions=
        <ColumnDefinition Width=Auto />
        <ColumnDefinition Width=* />
        <ColumnDefinition Width=100 />
    ...
/Grid>
```

In fact, Height is the default property for RowDefinition and Width for ColumnDefinition, so this can be further shortened to:  
```
<Grid
    RowDefinitions=<RowDefinition Auto/> <RowDefinition */> <RowDefinition 100/>
    ColumnDefinitions=<ColumnDefinition Auto/> <ColumnDefinition */> <ColumnDefinition 100/>
    ...
/Grid>

```
