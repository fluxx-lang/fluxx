---
title: Styles and Properties
layout: page
parent: Language
---

Styles and properties
-------------



XAML:
```
<Style BasedOn="{StaticResource {x:Type TextBlock}}"
        TargetType="TextBlock"
        x:Key="TitleText">
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

Fluxx:
```
let TitleText:TextBlock.properties =
    FontSize=26;
    Foreground=
        <LinearGradientBrush StartPoint=0.5,0; EndPoint=0.5,1;
            <GradientStop Offset=0.0; Color=#90DDDD />
            <GradientStop Offset=1.0; Color=#5BFFFF />
        /LinearGradientBrush>
```



Styles also become functions.


XAML (WPF):
```
<Style BasedOn="{StaticResource {x:StaticResource  DefaultTextStyle}}"
        TargetType="TextBlock"
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

Fluxx:
```
let <TitleTextStyle/> =
    <DefaultTextStyle
        FontSize=26
        Foreground=
            <LinearGradientBrush StartPoint=0.5,0; EndPoint=0.5,1
                GradientStops=
                    <GradientStop Offset=0.0; Color=#90DDDD />
                    <GradientStop Offset=1.0; Color=#5BFFFF />
            /LinearGradientBrush>
    /DefaultText>
```
