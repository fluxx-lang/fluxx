Fluxx is a Functional *Reactive* Markup Language
---------------

What's a reactive language? Reactivity is often compared to Excel. In Excel you can define a formula
that depends on other cells. When the values in those other cells change, the formula automatically
updates--it *reacts* to the change, automatically updating. Compared to traditional programming,
there's no need to write an event handler, generate property change notifications, decide what
thread(s) the recalculate work should happen on, or worry about locking as the data changes.
Instead, you just declaratively express the value for one cell in terms of another cell and the rest
happens automatically. The difference between the two approaches make Excel a tool accessible to
everyone while traditional programming is for the experts. Reactivity makes development easier,
making experts more productive and non-experts more empowered.

Fluxx favors this app architecture: An app can be viewed as a *function*, that maps a data model to
UI. As the data model (the function input) changes, the UI automatically updates.  Data model
updates generally come from two kinds of places: (1) Users interacting with the UI, making edits or
navigating to a different page and (2) external changes like a network request completing with
updated data from the server.  Both trigger reactive updates.

Here's a simple example (again, based on Xamarin Forms UI controls):

```
employeeDetailsPage{employee: Employee} =
    ContentPage
        StackLayout
            Label Text:{employee.FirstName}
            Label Text:{employee.LastName}
```

When the employee name changes, the UI will automatically update. Note that the dependencies here
are all captured as part of the function abstraction--anything that output can depend is provided
as function parameters.  Contrasted with XAML, there's no notion of a "binding context", set
externally.  That makes things simpler, more explicit, and more flexible with multiple parameters
allowed.

Reactivity can also be tied directly to the state of UI controls, not just parameters:

```
helloPage =
    ContentPage
        StackLayout
            Entry Name:FirstName Placeholder:enter first name
            Entry Name:LastName Placeholder:enter last name
            Label Text:Hello there, {FirstName.Text} {LastName.Text}
```

Here `Name` is an "identifier property", a special kind of property that essentially creates an
identifier, of the specified object type (`Entry` here) that refers to the object. In the Fluxx
version of HTML that property would be called `id` but for Fluxx Xamarin Forms it's called `Name`,
like in regular XAML.

Unlike XAML in Fluxx there's no need to override the BindingContext for controls to do view to view
bindings nor use other complicated binding syntax. You just specify arbitrary expressions, using
your data parameters and UI control names. And it's all strongly typed, with typing errors flagged
immediately in a Fluxx supported editor. No more binding errors that are only caught at runtime.

Here's a more complicated example, contrasting XAML data binding with Fluxx data binding / reactivity.
XAML has the restriction that a control can only have a single binding context, so if multiple
control properties need to be bound to different things you have to reverse the binding with
Mode="OneWayToSource". With Fluxx, there's such restriction and it's arguably much more
straightforward. Fluxx aims to remove much of the complexity and learning curve around XAML binding,
while also being more powerful.

Here's the XAML:
```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="XamlSamples.SliderTransformsPage"
             Title="Slider Transforms Page">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="Auto" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackLayout Grid.Row="0" Grid.Column="0" Grid.ColumnSpan="2">
            <!-- Scaled and rotated Label -->
            <Label x:Name="label"
                    Text="TEXT"
                    HorizontalOptions="Center"
                    VerticalOptions="CenterAndExpand" />
        </StackLayout>

        <!-- Slider and identifying Label for Scale -->
        <Slider x:Name="scaleSlider"
                BindingContext="{x:Reference label}"
                Grid.Row="1" Grid.Column="1"
                Maximum="10"
                Value="{Binding Scale, Mode=TwoWay}" />

        <Label BindingContext="{x:Reference scaleSlider}"
            Text="{Binding Value, StringFormat='Scale = {0:F1}'}"
            Grid.Row="1" Grid.Column="0"
            VerticalTextAlignment="Center" />

        <!-- Slider and identifying Label for Rotation -->
        <Slider x:Name="rotationSlider"
                BindingContext="{x:Reference label}"
                Grid.Row="2" Grid.Column="1"
                Maximum="360"
                Value="{Binding Rotation, Mode=OneWayToSource}" />

        <Label BindingContext="{x:Reference rotationSlider}"
            Text="{Binding Value, StringFormat='Rotation = {0:F0}'}"
            Grid.Row="2" Grid.Column="0"
            VerticalTextAlignment="Center" />

        <!-- Slider and identifying Label for RotationX -->
        <Slider x:Name="rotationXSlider"
                BindingContext="{x:Reference label}"
                Grid.Row="3" Grid.Column="1"
                Maximum="360"
                Value="{Binding RotationX, Mode=OneWayToSource}" />

        <Label BindingContext="{x:Reference rotationXSlider}"
            Text="{Binding Value, StringFormat='RotationX = {0:F0}'}"
            Grid.Row="3" Grid.Column="0"
            VerticalTextAlignment="Center" />

        <!-- Slider and identifying Label for RotationY -->
        <Slider x:Name="rotationYSlider"
                BindingContext="{x:Reference label}"
                Grid.Row="4" Grid.Column="1"
                Maximum="360"
                Value="{Binding RotationY, Mode=OneWayToSource}" />

        <Label BindingContext="{x:Reference rotationYSlider}"
            Text="{Binding Value, StringFormat='RotationY = {0:F0}'}"
            Grid.Row="4" Grid.Column="0"
            VerticalTextAlignment="Center" />
    </Grid>
</ContentPage>
```


And the Fluxx, where the dependencies are set explicitly, in a straightfoward way, on the `label` control: 
```
slidersTransformPage =
    ContentPage
        Title: Slider Transforms Page
        Content:

        Grid
            RowDefinitions:
                RowDefinition *
                RowDefinition Auto
                RowDefinition Auto
                RowDefinition Auto
                RowDefinition Auto

            ColumnDefinitions: ColumenDefinition{Auto} ColumenDefinition{*}

            StackLayout  Grid.Row:0  Grid.Column:0  Grid.ColumnSpan:2
                // Scaled and rotated Label
                Label
                    Name: label
                    Text: TEXT
                    Scale: {scaleSlider.Value}
                    Rotation: {rotationSlider.Value}
                    RotationX: {rotationXSlider.Value}
                    RotationY: {rotationYSlider.Value}
                    HorizontalOptions: Center
                    VerticalOptions: CenterAndExpand

            // Identifying Label and Slider for Scale
            Label
                Grid.Row:1  Grid.Column:0
                Text: Scale = format{value:scaleSlider.Value  precision:1}
                VerticalTextAlignment: Center

            Slider
                Grid.Row:1  Grid.Column:1
                Name: scaleSlider
                Maximum: 10

            // Identifying Label and Slider for Rotation
            Label
                Grid.Row:2  Grid.Column:0
                Text: Rotation = format{value:rotationSlider.Value  precision:0}
                VerticalTextAlignment: Center

            Slider rotationSlider
                Grid.Row:2  Grid.Column:1
                Maximum: 360

            // Identifying Label and Slider for RotationX
            Label
                Grid.Row:3  Grid.Column:0
                Text: RotationX = format{value:rotationXSlider.Value  precision:0}
                VerticalTextAlignment: Center

            Slider rotationXSlider
                Grid.Row:3  Grid.Column:1
                Maximum: 360<!---->

            // Identifying Label and Slider for RotationY
            Label
                Grid.Row:4  Grid.Column:0
                Text: RotationY = format{value:rotationYSlider.Value  precision:0}
                VerticalTextAlignment: Center

            Slider rotationYSlider
                Grid.Row:4  Grid.Column:1
                Maximum: 360
```
