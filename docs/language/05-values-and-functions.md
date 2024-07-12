---
title: Values and Functions
layout: default
parent: Language
nav_order: 5
---


Values
------

Fluxx values are defined with `let`. Types are inferred but can be specified explicitly:

```fluxx
let myValue1 = 42
let myValue2:float = 43
```

Values are immutable. As Fluxx is a pure functional language, it doesn't have mutable variables.

Functions
------

Functions are also defined with `let`. Functions allow for abstraction and reuse. Here's a WPF
based example, abstracting icon images:

```fluxx
let <IconImage Icon:FileName /> =
    <Image Width=32 Height=32
        Source=
            <BitmapImage UriSource=@IconFile />
    /Image>
```

The function definition lists the function's properties (parameters), using the `<name>:<type>`
syntax, with the return value after the `=`.

Here's another function, making use of (calling) the function above:

```fluxx
let <MyPageHeader Label:uitext> = 
    <StackLayout Orientation=Horizontal;
        <IconImage Icon=pageIcon.png />
        <TextBlock Text=@Label />
    /StackLayout>
```

Note that functions are invoked with exactly the same syntax used to instantiate an object and
set properties on it. You can think of instantiating an object as calling a "constructor" function.

In XAML UI frameworks (like WPF or MAUI) functions can be used in two different ways.
The `MyPageHeader` function above acts like a macro - using it results in the same visual UI hierarchy
as if the `StackLayout` was used directly, just with less typing. That function has a return
type of `StackLayout` - Fluxx uses type inferrence for function return values, like values.

However, if the function is defined with an explicit return type that's a custom type,
then it acts like control:

```fluxx
let <MyPageHeader Label:uitext>:MyControls.MyPageHeader = 
    <StackLayout Orientation=Horizontal;
        <IconImage Icon=pageIcon.png />
        <TextBlock Text=@Label />
    /StackLayout>
```

Here `MyControls` is expected to be a C# class, in the `MyControls` namespace.
That class can contain code behind. The Fluxx markup above is the "XAML" part of the control.
When used there's a `MyPageHeader` control in the visual UI hierarchy.
