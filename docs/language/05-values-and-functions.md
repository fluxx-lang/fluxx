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

```Fluxx
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

