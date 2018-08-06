# Jering.Web.SyntaxHighlighters.Prism
[![Build status](https://ci.appveyor.com/api/projects/status/swmelqsuwnw41d0h?svg=true)](https://ci.appveyor.com/project/JeremyTCD/web-syntaxhighlighters-prism)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/Pkcs11Interop/Pkcs11Interop/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/vpre/Jering.Web.SyntaxHighlighters.Prism.svg?label=nuget)](https://www.nuget.org/packages/Jering.Web.SyntaxHighlighters.Prism/)
<!-- TODO tests badge, this service should work -  [![Tests status](https://appveyor-shields-badge.herokuapp.com/api/testResults/jeremytcd/web-syntaxhighlighters-prism/badge.svg)](https://ci.appveyor.com/project/jeremytcd/web-syntaxhighlighters-prism) -->

## Table of Contents
[Overview](#overview)  
[Target Frameworks](#target-frameworks)  
[Prerequisites](#prerequisites)  
[Installation](#installation)  
[Concepts](#concepts)  
[Usage](#usage)  
[API](#api)  
[Building](#building)  
[Related Projects](#related-projects)  
[Contributing](#contributing)  
[About](#about)

## Overview
This library provides a way to perform syntax highlighting in .Net applications using the javascript library, [Prism](https://github.com/PrismJS/prism). 

## Target Frameworks
- .NET Standard 1.3
- .NET Standard 2.0
 
## Prerequisites
[NodeJS](https://nodejs.org/en/) must be installed and node.exe's directory must be added to the `Path` environment variable.

## Installation
Using Package Manager:
```
PM> Install-Package Jering.Web.SyntaxHighlighters.Prism
```
Using .Net CLI:
```
> dotnet add package Jering.Web.SyntaxHighlighters.Prism
```

## Concepts
### What is a Syntax Highlighter?
Syntax highlighters add markup to code to facilitate styling. For example, the following code:

```csharp
public string ExampleFunction(string arg)
{
    // Example comment
    return arg + "dummyString";
}
```

is transformed into the following markup by the syntax highlighter Prism:

```html
<span class="token keyword">public</span> <span class="token keyword">string</span> <span class="token function">ExampleFunction</span><span class="token punctuation">(</span><span class="token keyword">string</span> arg<span class="token punctuation">)</span>
<span class="token punctuation">{</span>
    <span class="token comment">// Example comment</span>
    <span class="token keyword">return</span> arg <span class="token operator">+</span> <span class="token string">"dummyString"</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
```

Prism is a a javascript library, which is ideal since syntax highlighting is often done client-side. There are however, situations where syntax highlighting can't or shouldn't be done client-side, for example:
- When generating [AMP](https://www.ampproject.org/) pages, since AMP pages cannot run scripts.
- When page load time is critical.
- When page size is critical.

This library allows syntax highlighting to be done by .Net server-side applications and tools like static site generators.

## Usage
### Creating IPrismService
This library uses depedency injection (DI) to facilitate extensibility and testability.
You can use any DI framework that has adapters for [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection).
Here, we'll use the vanilla Microsoft.Extensions.DependencyInjection framework:
```csharp
var services = new ServiceCollection();
services.AddPrism();
ServiceProvider serviceProvider = services.BuildServiceProvider();
IPrismService prismService = serviceProvider.GetRequiredService<IPrismService>();
```

`IPrismService` is a singleton service and `IPrismService`'s members are thread safe.
Where possible, inject `IPrismService` into your types or keep a reference to a shared `IPrismService` instance. 
Try to avoid creating multiple `IPrismService` instances, since each instance spawns a NodeJS process. 

When you're done, you can manually dispose of an `IPrismService` instance by calling
```csharp
prismService.Dispose();
```
or 
```csharp
serviceProvider.Dispose(); // Calls Dispose on objects it has instantiated that are disposable
```
`Dispose` kills the spawned NodeJS process.
Note that even if `Dispose` isn't called manually, the service that manages the NodeJS process, `INodeJSService` from [Jering.Javascript.NodeJS](https://github.com/JeremyTCD/Javascript.NodeJS), will kill the 
NodeJS process when the application shuts down - if the application shuts down gracefully. If the application does not shutdown gracefully, the NodeJS process will kill 
itself when it detects that its parent has been killed. 
Essentially, manually disposing of `IPrismService` instances is not mandatory.

### Using IPrismService
Code can be highlighted using [`IPrismService.HighlightAsync`](#iprismservice.highlightasync):
```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

string highlightedCode = await prismService.HighlightAsync(code, "csharp");
```
Note the second parameter of `IPrismService.HighlightAsync`. It must be a valid [Prism language alias](https://prismjs.com/index.html#languages-list) representing the 
code's language.

## API
### IPrismService.HighlightAsync
#### Signature
```csharp
Task<string> HighlightAsync(string code, string languageAlias)
```
#### Description
Highlights code of a specified language.
#### Parameters
- `code`
  - Type: `string`
  - Description: Code to highlight.
- `languageAlias`
  - Type: `string`
  - Description: A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.
#### Returns
Highlighted code.
#### Exceptions
- `ArgumentNullException`
  - Thrown if `code` is null.
- `ArgumentException`
  - Thrown if `languageAlias` is not a valid Prism language alias.
- `InvocationException`
  - Thrown if a NodeJS error occurs.
#### Example
```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

string highlightedCode = await prismService.HighlightAsync(code, "csharp");
```
### IPrismService.IsValidLanguageAliasAsync
#### Signature
```csharp
ValueTask<bool> IsValidLanguageAliasAsync(string languageAlias)
```
#### Description
Determines whether a language alias is valid.
#### Parameters
- `languageAlias`
  - Type: `string`
  - Description: Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.
#### Returns
`true` if `languageAlias` is a valid Prism language alias. Otherwise, `false`.
#### Exceptions
- `InvocationException`
  - Thrown if a NodeJS error occurs.
#### Example
```csharp
bool isValid = await prismService.IsValidLanguageAliasAsync("csharp");
```

## Building
This project can be built using Visual Studio 2017.

## Related Projects
#### Similar Projects
[Jering.Web.SyntaxHighlighters.HighlightJS](https://github.com/JeremyTCD/Web.SyntaxHighlighters.HighlightJS) - 
A C# Wrapper for the Syntax Highlighter, HighlightJS.
#### Projects Using this Library
[Jering.Markdig.Extensions.FlexiBlocks](https://github.com/JeremyTCD/Markdig.Extensions.FlexiBlocks) - A Collection of Flexible Markdig Extensions.
#### Projects this Library Uses
[Jering.Javascript.NodeJS](https://github.com/JeremyTCD/Javascript.NodeJS) - Invoke Javascript in NodeJS from C#.

## Contributing
Contributions are welcome!  

## About
Follow [@JeremyTCD](https://twitter.com/JeremyTCD) for updates and more.
