---
title: XAML Comparisons
layout: default
parent: Language
---


XAML
----

Fluxx aims to be more concise, intuitive, and powerful than XAML. Below are some syntax comparisons,
showing how XAML constructs are expressed in Fluxx.

#### Triggers

XAML:
```
<Style TargetType="ListBoxItem">
    <Setter Property="Opacity" Value="0.5" />
    <Setter Property="MaxHeight" Value="75" />
    <Style.Triggers>
        <Trigger Property="IsSelected" Value="True">
            <Trigger.Setters>
                <Setter Property="Opacity" Value="1.0" />
            </Trigger.Setters>
        </Trigger>
    </Style.Triggers>
</Style>
```


Fluxx:
```
let ListBoxItem.properties =
    Opacity=0.5;
    MaxHeight=75;
    if IsSelected =>
        Opacity=1.0
    /if
```
