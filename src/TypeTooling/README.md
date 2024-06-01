# Overview

TypeTooling provides a standard API for querying metadata associated with data types to power great developer tooling experiences.

It's intended to work especially well with UI widgets embedded in some kind of UI markup language, but can operate with any data type.

It provides, or plans to provide, APIs like these:
- For a given object type, what are all the properties it exposes? What is the help text for those properties?
- Is this literal text a valid for a particular type (e.g. is "24dp" valid for a width type)? If not, what's a user friendly error/warning/info message to show?
- Should the type show up in the toolbox? If so, what are its icon and default properties?
- Pop up a visual editor for the type (e.g. a color picker for a color) if one exists.
- Can objects of the type be visualized in some way (e.g. as a PNG)? If so, render it.

TypeTooling wraps access to XAML data types (for UWP/WPF/Forms XAML), custom data types, and Android and iOS UI widgets (planned). 3rd parties can plug in providers as well, for other data types. By providing a single standard API for this info, it's easier to build generic tools that
use it.

Project goals here include:
- Improve XAML text editor experience (e.g. linting).
- Extend some Proppy functionality (e.g. color picker) into the source editor as well.
- Provide good source editor support for Android and iOS native widgets being embedded in Forms XAML, as well as AXML and XIB.
- Increase code share.
- Support tooling partners, as they help grow the Microsoft ecosystem too.
- Support UI tooling innovation.

This is still a (somewhat) experimental project, which started as a side project, put here to make discussion and review easier by
interested parties (e.g. the XAML Experiences Team), before we make a final decision on use and open sourcing.

# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.
