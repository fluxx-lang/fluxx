Values
------

Fluxx values are defined with `let`. Types are inferred but can be specified explicitly:

```
let myValue1 = 42
let myValue2:float = 43
```

Values are immutable. As Fluxx is a pure functional language, it doesn't have mutable variables.

Values can be of any type, not just primitive types:

```
let logoImg =
    <img src=logo.jpg; alt=<uitext Acme company logo/> />
```

Expressions
------



```
let myValue1 = 42
<element prop=@myValue />
```


```
let myValue2 = @(myValue1 * 2)
<element prop=@myValue />
```
