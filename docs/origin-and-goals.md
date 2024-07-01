---
title: Origin and Goals
layout: page
---


Fluxx Origin and Goals
=======

Fluxx arose mainly from the desire to solve this problem: make it as easy as possible to build UIs.

UI today is generally created in some kind of declarative markup language. That can be HTML, Android XML, iOS XiB,
Windows XAML, NativeScript XML, React JSX, etc.  Those examples are all XML based. But XML has its limitations. It lacks any kind
abstraction mechanism, to use similar markup in multiple places without having to repeat it.  And for UIs there typically needs to be some way
to do data binding, call to/from imperative code, support dynamic UIs that change based on platform or state, support localization, and
interoperate with great interactive tooling. So these UI markup languages add various kinds of extensions to deal to deal with all of that. 
Some embed another language in XML attributes or use a template language preprocessor to mix two languages together.  In most cases, these
markup enhancements solve the same core problems (like the lack of abstraction in XML) over & over, in different ways.  

But all those language extensions on top of XML (already a fairly verbose language to start with) add complexity, which goes against the goal of Fluxx
to make UI dev as easy as possible, removing gratuitous ceremony it's possible to remove. 
A better approach, arguably, is to improve the core markup language itself--XML isn't good enough, which is why people keep extending it,
so instead we should create a better markup language.  So that's one way to think of Fluxx--as a better markup language, a better XML.

Fluxx is a *functional* *reactive* *markup* language.  It's a *general purpose* markup language, but especially
intended to be make it easy to create *UIs*. Let's break that down a bit. First and foremost, Fluxx is a markup language, 
much like XML, JSON, and YAML.  It allows you to express hierarchical data in a straightforward, declarative
way. As a markup language, it's syntax is much like JSON/CSS, albeit a bit more concise. Simple use
cases only use Fluxx for markup, expressing a single static tree of objects. But Fluxx is also a functional
language, allowing repeated markup to be abstracted into a function and functions to transform data/markup
into other markup. Fluxx is a much simpler functional language than say F# or Haskell, but it brings
some of those same core functional concepts. In technical terms, Fluxx is a strongly typed (with type inference)
pure functional language. And Fluxx is a functional reactive language, which in brief means that when the input
changes the output updates automatically; see more about reactivity below.

As a general purpose markup language, Fluxx can be used wherever other markup languages, like XML or JSON or YAML or in some cases custom DSLs,
are used today. It's not really intended for data transfer, but wherever markup is human authored, and is declarative but could benefit from a bit more
traditional programming language like power, like functions, Fluxx is a good candidate.  Think config files, like Puppet / Chef / Azure Resource
Manager server config. Or Maven/Gradle style build scripts.

But the main use case for Fluxx is intended to be defining UIs.  So that's another way to think of Fluxx--as a better UI markup language, a better
(and much less verbose) XAML.  If you need convincing that customers would appreciate XAML syntax innovations, check out these links (including
the comment sections): http://paulstovell.com/blog/six-years-of-wpf and http://www.itwriting.com/blog/8166-entering-microsofts-xaml-labyrinth-is-it-worth-it.html.  Fluxx addresses several of the complaints and suggestions.  For instance, there are these two comments (one from each link):
> I wish the XAML team would take a look at QML and take some ideas from their. QML is much easier on the eyes and works crossplatform.
> A QML/C# combination together with first class support by MS would be a developer’s paradise.

and 
> What if MS had actually valued the programmer's time in writing xaml and xaml looked like this? QtQuick [aka QML] looks like this but in this case, it can be attached to C# instead of C++.
> why not? Why was XML such a compelling choice for them? Why was the uri namespaces a compelling choice for them? Did they really expect people to learn how to program this?
>
> ...
>
> And that's it. The syntax is shorter and more comfortable, and easy to remember. Again... Why Microsoft!?!? Why?!?!

Fluxx is largely is what those two users suggest: It's syntax is very QML-like (though more powerful than QML).  And it's intended to be paired
with C# (with other languages, like JavaScript, possible).

Finally, Fluxx can express more than just single app UI screens in a declarative way--it can express page navigation and links to data connectors
(server or client side) declaratively too.  In simpler cases, whole apps can be done in Fluxx with no C#/JavaScript code behind needed.
In this way, Fluxx can be used for UI prototypes and (at least in the future, potentially) as a "low code" app development tool.  [Note: I need to
expand the doc on the data connector / low code stuff--it's not there yet.] 
