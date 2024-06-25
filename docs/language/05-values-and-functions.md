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

Values can be of any type, not just primitive types:

```fluxx
let logoImg =
    <img src=logo.jpg; alt=<uitext Acme company logo/> />
```

Expressions
------


```fluxx
let myValue1 = 42
<element prop=@myValue />
```


```fluxx
let myValue2 = @(myValue1 * 2)
<element prop=@myValue />
```
