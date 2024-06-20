Fluxx - Guide
=======

Tooling
-------

There are two parts to the developer experience: the language and the tooling. And Fluxx aims to make
both of those a better experience. Fluxx the language is designed to
enable a great tooling experience.

Live Update
----------

First off, Fluxx can operate in two modes--interpreted and code gen. Interpreted mode is best for
developer inner loop. A key part of the Fluxx developer experience is that your app is updated live,
as you type. That's great for making UI changes and immediately seeing their effects in your real
app, with real data.

The way that works is that your app is running on an emulator / real device (or even multiple
devices, with say iOS and Android emulators by side).  The app hosts the Fluxx runtime and
interpreter. As you type, updated Fluxx source is sent over to the app, via the MQTT protocol (more
below on that) and the UI redrawn on the device.  In the future, we might update C# code in a
similar way, either with a C# interpreter or recompiling. But live update is most useful for
immediately seeing UI changes.

Interpreted mode is the only mode implemented today, but the concept is that Fluxx can also do code
gen, generating C# (or potentially other languages).  Code gen makes for somewhat faster UI code at
runtime, especially on slow Android hardware (though interpreted code is fine performance wise in
many cases). It also avoids the need for the Fluxx runtime / interpreter (not that it's huge, it's
about 100K currently).

And here's another interesting option: Fluxx could also do code gen to JavaScript instead of C# for
Xamarin apps. JavaScript isn't better technically but it does have one key advantage: it would
enable app UI (and anything coded in Fluxx) to be hot updated via CodePush, without needing to
resubmit to the Apple App Store.  The Apple developer agreement only allows hot updates of
JavaScript code.  So that's a potential way to bring CodePush functionality to Xamarin apps (at
least for UI). 

CodePush updates for Android apps and enterprise stores don't have the JavaScript-only restriction,
but if we want CodePush functionality for public iOS App Store Xamarin apps, something like this
seems like the only way to do it. JavaScript code gen could also help support web based apps
(eventually).

As for MQTT, it's a nice protocol to use for tooling talking to device(s).  At first I used http
here, but MQTT has the key advantage that it's outbound only on the device. Normally devices don't
allow inbound connections, at least not without jumping through special hoops (adb connect to
Android emulator, etc). UWP desktop apps don't allow inbound network connections either. MQTT solves
all of that and makes it easier to have multiple devices connect to the tooling. MQTT is the
protocol Visual Studio uses to talk the Xamarin mac agent today. I use the same MQTT library for
Fluxx, written by Daniel Cazzulino and Mauro Agnoletti.

Examples
--------

Another innovative part of the Fluxx developer experience are examples, showing what a UI component
or other expression value looks like, right there the editor.  It's kind of like a playground/workbork,
but part part of your app's source. One thing that makes programming conceptually harder than say direct
manipulation UIs is that you have to imagine the results that something produces rather than being
about to _see_ it. Examples helps bridge that gap.  They are also good for documentation and
testing (more below on those).

For instance (using WPF here):
```
MyGradientButton{Label:UIText  BaseColor:Color} = 
    Button Content:{Label}  Height:23
      Background:
        LinearGradientBrush  StartPoint:0,0  EndPoint:1,1
          GradientStop  Color:BaseColor  Offset:0
          GradientStop  Color:BaseColor.Darken{24%}  Offset:0.445
          GradientStop  Color:BaseColor.Darken{45%}  Offset:0.53

example MyGradientButton  Label:Click here  BaseColor:LightSteelBlue
example MyGradientButton  Label:This is very long label  BaseColor:LemonChiffon
example MyGradientButton  Label:Hit me!  BaseColor:#FFFFFACD
```

The example values are shown right there in the editor, so you can see the three button variants.
When the value is a single line of text they appear on the right, if < 200 pixels or so high
(like here) the appear below the example, and if bigger are shown via popup (or something like that--
we still need to play with this more in action to find the display rules that works best in
practice).

As MyGradientButton or its examples are edited, the results are updated live.
Examples are kind of like the live unit testing features in Visual Studio 2017,
except they display the actual value not just pass/fail.

Examples that are mobile app GUI objects are rendered on the device/emulator (which
the editor communicates with via MQTT) as that's the only way to get pixel perfect results.
Examples that aren't visual are rendered on the PC.

Examples can be used for non GUI data as well, like here:

```
example QueryWeatherForecast  PostalCode:10356  Country:US
```

This queries a REST API and shows the resulting forecast data object (JSON-ish) there in
the editor.

The intended workflow with examples is that you build your app in pieces,
creating UI components and using examples to help visualize them,
sometimes creating UI logic or data connectors that go along with that UI and
using examples to help more easily do that, then using the live update fuctionality
(previous section) to see it all work together in the context of the running app.

After things settle out you can leave the examples there as visual documentation.

When used for unit testing, examples work similar to the Jest testing framework:
https://facebook.github.io/jest/blog/2016/07/27/jest-14.html  The idea is that
there's a persistent snapshot automatically generated initially for each example, that
can be added to source control.  Then later when the example is evaluated it's
compared against the saved snapshot.  If they are different, that's a potential
"test failure", with visual indication of that (say in red) in the editor.
Maybe the difference is due to a bug, in which case you should fix the bug.
Or maybe the difference is due to a legitimate change in functionality, in
which case you can regenerate the snapshot (via some actionable popup you
see when clicking on the error indicator in the editor).  Overall, examples
provide an easier kind of testing, especially good for UI component trees,
where there's no need to hand write the "expected" part of the tests, as
that's generated automatically via the snapshots.


Visual Editing
--------

To maximize discovery and ease of editing I envisioned Fluxx providing other kinds of
visual tooling, all to enable more of a hybrid experience between a code editor and
GUI UI builder.  Those features include:

In place propery editing UI affordances:  When appropriate, property values popup direct manipulation
UI to change them when you click on the text for them.  Clicking on a color value pops up
a color picker right next to the text. Clicking on a continuous numberical value (e.g. dimension,
x/y value, percentage) pops up a little slider under the number so you can drag it rather
than having to type it in number.

Property sheet UI:  Clicking anywhere in a UI control or other function shows a property sheet,
off to the side, that lists all possible properties and their current values (either default or
explicitly set).  That's much like our XAML editors do today and some other React Native IDEs do
something similar.  It's all to promote discovery of all properties & their current defaults.
Maybe we list exaplicitly set properties at the top though.

Drag & drop UI controls:  There's a UI control palette off to the side that can be dragged &
dropped into the text of the markup, to add controls.  Again, that's to promote discovery.

All of the ideas above could stand some mockups & customer testing, to evaluate
usability and customer priority.  But they seem like the right idea.

And note that all of the above is still in the context of Fluxx being a generic language--it's not
just for Xamarin Forms XAML or WPF or HTML; it can work with any kind of object trees, but some
objects & object properties are naturally visual.  The Fluxx TypeInfoProvider class allows providing
metadata to supplement plain old C# (or other language) objects to provide extra info that
helps drive the visual tooling (e.g. provide min/max values for continuous numerical properties).
The TypeInfoProvider can be packaged with the core components it describes or as a separate thing,
provided by a third party.
