---
title: Properties and Styles
layout: page
parent: Language
nav_order: 3
---

Properties
----------

Fluxx has first class language support for _properties_, allowing property values for a particular type to
exist and be manipulated indepdently of objects of the type. Properties provide support for styles, but they are a
generic language feature and useful in other contexts too.

Here's XAML based style, as an example:
```
<Style TargetType="TextBlock"
        x:Key="TitleTextStyle">
    <Setter Property="FontSize" Value="26"/>
    <Setter Property="Foreground">
        <Setter.Value>
            <LinearGradientBrush StartPoint="0.5,0" EndPoint="0.5,1">
                <LinearGradientBrush.GradientStops>
                    <GradientStop Offset="0.0" Color="#90DDDD" />
                    <GradientStop Offset="1.0" Color="#5BFFFF" />
                </LinearGradientBrush.GradientStops>
            </LinearGradientBrush>
        </Setter.Value>
    </Setter>
</Style>
```

Fluxx equivalent:
```
let TitleTextStyle:TextBlock.properties =
    FontSize=26
    Foreground=
        <LinearGradientBrush StartPoint=0.5,0; EndPoint=0.5,1;
            <GradientStop Offset=0.0; Color=#90DDDD />
            <GradientStop Offset=1.0; Color=#5BFFFF />
        /LinearGradientBrush>
```

`TextBlock.properties` is a set of 0 or more property values, that
can apply to a `TextBlock` object. These property values live on their
own, independent of any particular `TextBlock`.

Properties can be modified via the `with` expression:
```
let PageTitleTextStyle:TextBlock.properties =
    TitleTextStyle with <
        FontSize=32
        BackgroundColor=Blue
        Foreground=unset
    />
```

Similar to CSS, assigning a property to `unset`, like `Foreground=unset` above, removes the property.
