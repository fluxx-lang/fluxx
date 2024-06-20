---
title: Home (New)
layout: home
---

Fluxx is a functional markup language. You can think of it as functional XML - XML enhanced with simple but powerful elements of functional reactive programming.  

Fluxx Language
------------
Fluxx is a general purpose language, intended to be especially good at building UIs.

It's driven by a couple of realizations:

1. Today UIs are either built in a markup language (e.g. XAML, Razor, Angular markup, HTML/CSS) or in code (e.g. Dart, SwiftUI, Jetpack Compose, JavaScript). Markup languages give a clean & elegant syntax and UI tooling friendliness. While coded UIs bring more power & code based abstractions. Both approaches have their proponents, but neither is wholly satisying.

Fluxx aims to be the best of both worlds, combining the clean declarative syntax of a modern markup language with a simple but powerful strongly typed functional programming language. You can kind of think of it as a better, more concise, XML - with functions.

2. There's a growing realization that the Functional Reactive Programming (FRP) model is especially well suited for building UIs. Fluxx supports FRP natively. In particular, all Fluxx functions are automatically reactive with no extra syntax magic required (no callbacks, no @observable annotations, no weird binding syntax,). Reactivity works best, and simplest, when its a core part of the language.
    
{% include_relative contents.md %}

Evolved XAML
-----------

XAML is great, but it has some point points. We've learned some things in the 13 years since XAML was created.
Fluxx is in many ways an experiemental evolved XAML - it aims to take those learnings to create something better.

- Much less verbose, friendlier syntax.
- Functions as core abstraction mechanism, for UI components and more
- Support for conditionals, pattern matching, arbitrary expressions, all automatically reactive
- More concise, intutive, and powerful data binding and behaviors, through reactivity
- See updates to your UI immediately, as you type, running in your real app
- Easy upgrade path: XAML to Fluxx auto converter; mix Fluxx and XAML pages in same app
- Unified syntax: Use for Xamarin Forms, UWP, and WPF (and more, even web eventually--Fluxx is generic)
- Supports innovative visual tooling, like the `example` feature for in editor playgrounds
- Fluxx is translated to human friendly C# code, making for easier debugging than XAML 
- Build more of your app declaratively: decalarative navigation, state updates, and more
- Not just programmers: easy enough so designers / less technical users can build working whole app UI prototypes

Xamarin + Fluxx vs React Native and Flutter
----------------------------
Xamarin + Fluxx has similar concepts to React Native and Flutter, but is, well, better.
- Better languages (vs JavaScript/JSX): strongly typed; natively complied performance; reactivity supported directly in core language; cleaner markup + functional syntax than JSX glued on top of JavaScript 
- Better languages (vs Dart): reactivity supported directly in core language; cleaner markup syntax than Dart since Fluxx is designed to be a markup language
- Better tooling: innovative visual UI tooling with immediate feedback on your live app coupled with the world class Visual Studio IDE
- Better native interop: call directly to native platform APIs from a single language; no hand written Objective-C / Java wrappers required

MVU Architeture
------------
Fluxx encourages use of the MVU, or Module-View-Update, architecture, similar to other modern frameworks like [Elm](https://guide.elm-lang.org/architecture), React, and F#'s [Fabulous](https://fsprojects.github.io/Fabulous/).
- Model is a data model or view model. It's similar to the Model/View Model in today's MVVM architecture, except Fluxx allows more flexibility as functions can have multiple parameters and so aren't restricted to a single view model. UI function parameters correspond to `props` in React apps, but in Fluxx these are explicitly declared and strongly typed. 
- The View transforms function parameters (e.g. model data) to UI markup. Fluxx is a functional language, better supporting UI that is dynamic, not just static markup. In fact, an entire Fluxx app can be thought of as a single big function (which calls others functions).
- Updates are declarative expressions of how state should change in response to events.
  React calls these "Actions" and some Microsoft technologies call them "Commands", but the concept is essentially the same.
  Ideally, Updates are routed to a managed state component for the app, similar to Redux or MobX in React apps. But use of Updates isn't required - traditional code behind callbacks are supported as well, for easy compatibility with today's XAML based apps.    
  See [Event Handling](language/event-handling.md) for details.
