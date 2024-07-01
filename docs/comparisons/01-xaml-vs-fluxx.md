---
title: XAML vs Fluxx
layout: default
parent: Comparisons
nav_order: 1
---

XAML vs Fluxx
----------

Below are examples of XAML - especially verbose XAML - and how that would be expressed in Fluxx.
These examples taken from real apps.

### Hide Content Based on enum Values

##### XAML
```xml
<ConventView>
    <ContentView.Triggers>
        <DataTrigger
            Binding="{Binding Item.Type, Converter={StaticResource EnumToIntConverter}}"
            TargetType="ContentView"
            Value="7">
            <Setter Property="IsVisible" Value="False">
        </DataTrigger>
        <DataTrigger
            Binding="{Binding Item.Type, Converter={StaticResource EnumToIntConverter}}"
            TargetType="ContentView"
            Value="8">
            <Setter Property="IsVisible" Value="False">
        </DataTrigger>
    </ConventView.Triggers>
<ConventView>
```

##### Fluxx
```fluxx
<ConventView
    if Item.Type is MyEnum.EnumValue1 or MyEnum.EnumValue2
        IsVisible=false
    /if
/ConventView>
```

### If User Profile Notes are Empty Set Row Height 0, Else 38

##### XAML
```xml
<Grid>
    <Grid.RowDefinitions>
        <RowDefinition Height="Auto" />
        <RowDefinition Height="{Binding Profile.Notes, Converter={StaticResource ZeroGridLengthIfStringEmptyConverter}, ConverterParameter=38}" />
    </Grid.RowDefinitions>
</Grid>
```

##### Fluxx
```fluxx
<Grid
    RowDefinitions =
        <RowDefinition Height=Auto />
        <RowDefinition Height=if Profile.Notes.Length == 0 => 0; else => 38 /if />
/Grid>
```

### If Switched Count >= 1, Use Gradient

##### XAML
```xml
<svg:SvgIcon>
    <svg:SvgIcon.Triggers>
        <DataTrigger
            Binding="{Binding Source={x:Reference ThisControl}, Path=SwitchedCount, Converter={converters:IntIsEqualOrMoreConverter}, ConverterParameter=1}"
            TargetType="svg:SvgIcon"
            Value="True">
            <Setter Property="UseGradient" Value="True" />
        </DataTrigger>
    </svg:SvgIcon.Triggers>
</svg:SvgIcon>
```

##### Fluxx
```fluxx
<svg.SvgIcon
    if ThisControl.SwitchedCount >= 1
        UseGradient=True
    /if
/svg.SvgIcon>
```

### Set End Color to 10% Lighter

##### XAML
```xml
<svg1:GradientBox
    EndColor="{Binding AvatarFormsColor, Converter={converters:MakeLighter}, ConverterParameter=10}"
    HorizontalOptions="Fill"
    StartColor="{Binding AvatarFormsColor}"
    VerticalOptions="Fill" />
```

##### Fluxx
```fluxx
<svg1.GradientBox
    EndColor=@AvatarFormsColor.MakeLighter(10)
    HorizontalOptions=Fill
    StartColor=@AvatarFormsColor
    VerticalOptions=Fill />
```


### Add Small Indicator Over Avator with Online Status

##### XAML
```xml
<Ellipse
    Margin="5,5,0,0"
    Fill="{xaml:SetBrush SolidColor={x:StaticResource ColorPrimary}}"
    HeightRequest="12"
    HorizontalOptions="Start"
    StrokeThickness="0.0"
    VerticalOptions="Start"
    WidthRequest="12">
    <Ellipse.Triggers>
        <DataTrigger
            Binding="{Binding OtherUser.OnlineStatus}"
            TargetType="Ellipse"
            Value="0">
            <Setter Property="Fill" Value="{xaml:SetBrush SolidColor={x:StaticResource ColorPaperSecondary}}" />
        </DataTrigger>
        <DataTrigger
            Binding="{Binding OtherUser.OnlineStatus}"
            TargetType="Ellipse"
            Value="1">
            <Setter Property="Fill" Value="#35D057" />
        </DataTrigger>
        <DataTrigger
            Binding="{Binding OtherUser.OnlineStatus}"
            TargetType="Ellipse"
            Value="2">
            <Setter Property="Fill" Value="{xaml:SetBrush SolidColor={x:StaticResource ColorDanger}}" />
        </DataTrigger>
    </Ellipse.Triggers>
</Ellipse>
```

##### Fluxx
```fluxx
<Ellipse
    Margin=5,5,0,0
    Fill=<SolidColorBrush @ColorPrimary/>
    HeightRequest=12
    HorizontalOptions=Start
    StrokeThickness=0.0
    VerticalOptions=Start
    WidthRequest=12
    Fill=
        if OtherUser.OnlineStatus is
            0 => <SolidColorBrush @ColorPaperSecondary/>
            1 => <SolidColorBrush #35D057/>
            2 => <SolidColorBrush @ColorDanger/>
        /if
/Ellipse>
```

### Update UI Based on Busy Status and If Options Provided

##### XAML
```xml
<input:ButtonMedium.Triggers>
    <DataTrigger
        Binding="{Binding IsBusy, Converter={converters:NotConverter}}"
        TargetType="input:ButtonMedium"
        Value="True">
        <Setter Property="Look" Value="Default" />
    </DataTrigger>
    <DataTrigger
        Binding="{Binding IsBusy}"
        TargetType="input:ButtonMedium"
        Value="True">
        <Setter Property="Look" Value="Disabled" />
    </DataTrigger>
    <DataTrigger
        Binding="{Binding Item.TestOptions, Converter={StaticResource StringNotEmptyConverter}}"
        TargetType="input:ButtonMedium"
        Value="True">
        <Setter Property="Text" Value="{x:Static resX:ResStrings.TestVariants }" />
    </DataTrigger>
    <DataTrigger
        Binding="{Binding Item.TestOptions, Converter={StaticResource StringNotEmptyConverter}}"
        TargetType="input:ButtonMedium"
        Value="False">
        <Setter Property="Text" Value="{x:Static resX:ResStrings.NeedSpecify }" />
    </DataTrigger>
</input:ButtonMedium.Triggers>
...
```

##### Fluxx
```fluxx
<ButtonMedium
    Look=if !IsBusy => Default; else => Disabled /if
    Text=<uitext id=if Item.TestOptions.Length > 0 => TestVariants; else => NeedSpecify /if />
    ...
/ButtonMedium>
```
