# Jering.Web.SyntaxHighlighters.Prism
[![Build Status](https://dev.azure.com/JeringTech/Web.SyntaxHighlighters.Prism/_apis/build/status/Jering.Web.SyntaxHighlighters.Prism-CI?branchName=master)](https://dev.azure.com/JeringTech/Web.SyntaxHighlighters.Prism/_build/latest?definitionId=3?branchName=master)
[![codecov](https://codecov.io/gh/JeringTech/Web.SyntaxHighlighters.Prism/branch/master/graph/badge.svg)](https://codecov.io/gh/JeringTech/Web.SyntaxHighlighters.Prism)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/Pkcs11Interop/Pkcs11Interop/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/vpre/Jering.Web.SyntaxHighlighters.Prism.svg?label=nuget)](https://www.nuget.org/packages/Jering.Web.SyntaxHighlighters.Prism/)

## Table of Contents
[Overview](#overview)  
[Target Frameworks](#target-frameworks)  
[Prerequisites](#prerequisites)  
[Installation](#installation)  
[Usage](#usage)  
[API](#api)  
[Building and Testing](#building-and-testing)  
[Related Jering Projects](#related-jering-projects)  
[Related Concepts](#related-concepts)  
[Contributing](#contributing)  
[About](#about)

## Overview
Jering.Web.SyntaxHighlighters.Prism enables you to perform syntax highlighting from C# projects using [Prism](https://github.com/PrismJS/prism).
This library is built to be flexible; you can use a dependency injection (DI) based API or a static API.

Here is an example of syntax highlighting using the static API:

```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

// Highlight code
string result = await StaticPrismService.HighlightAsync(code, "csharp");

string syntaxHighlightedCode = @"<span class=""token keyword"">public</span> <span class=""token keyword"">string</span> <span class=""token function"">ExampleFunction</span><span class=""token punctuation"">(</span><span class=""token keyword"">string</span> arg<span class=""token punctuation"">)</span>
<span class=""token punctuation"">{</span>
    <span class=""token comment"">// Example comment</span>
    <span class=""token keyword"">return</span> arg <span class=""token operator"">+</span> <span class=""token string"">""dummyString""</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>";

// result == syntax highlighted code
Assert.Equal(syntaxHighlightedCode, result);
```

And here is an example of syntax highlighting using the DI based API:

```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

// Highlight code
var services = new ServiceCollection();
services.AddPrism();
ServiceProvider serviceProvider = services.BuildServiceProvider();
IPrismService prismService = serviceProvider.GetRequiredService<IPrismService>();
string result = await prismService.HighlightAsync(code, "csharp");

string syntaxHighlightedCode = @"<span class=""token keyword"">public</span> <span class=""token keyword"">string</span> <span class=""token function"">ExampleFunction</span><span class=""token punctuation"">(</span><span class=""token keyword"">string</span> arg<span class=""token punctuation"">)</span>
<span class=""token punctuation"">{</span>
    <span class=""token comment"">// Example comment</span>
    <span class=""token keyword"">return</span> arg <span class=""token operator"">+</span> <span class=""token string"">""dummyString""</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>";

// result == syntax highlighted code
Assert.Equal(syntaxHighlightedCode, result);
```

## Target Frameworks
- .NET Standard 2.0
- .NET Framework 4.6.1
 
## Prerequisites
[NodeJS](https://nodejs.org/en/) must be installed and node.exe's directory must be added to the `Path` environment variable. This library has been
tested with NodeJS 10.5.2 - 12.13.0.

## Installation
Using Package Manager:
```
PM> Install-Package Jering.Web.SyntaxHighlighters.Prism
```
Using .Net CLI:
```
> dotnet add package Jering.Web.SyntaxHighlighters.Prism
```

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
Note that even if `Dispose` isn't called manually, the service that manages the NodeJS process, `INodeJSService` from [Jering.Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS), will kill the 
NodeJS process when the application shuts down - if the application shuts down gracefully. If the application does not shutdown gracefully, the NodeJS process will kill 
itself when it detects that its parent has been killed. 
Essentially, manually disposing of `IPrismService` instances is not mandatory.

#### Static API
This library also provides a static API as an alternative. The `StaticPrismService` type wraps an `IPrismService` instance, exposing most of its [public members](#api) statically.
Whether you use the static API or the DI based API depends on your development needs. If you are already using DI, if you want to mock 
out syntax highlighting in your tests or if you want to overwrite services, use the DI based API. Otherwise,
use the static API. An example usage:

```csharp
string result = await StaticPrismService.HighlightAsync(code, "csharp");
```

The following section on using `IPrismService` applies to usage of `StaticPrismService`.

### Using IPrismService
Code can be highlighted using [`IPrismService.HighlightAsync`](#iprismservicehighlightasync):
```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

string highlightedCode = await prismService.HighlightAsync(code, "csharp");
```
The second parameter of `IPrismService.HighlightAsync` must be a valid [Prism language alias](https://prismjs.com/index.html#languages-list).

## API
### IPrismService.HighlightAsync
#### Signature
```csharp
Task<string> HighlightAsync(string code, string languageAlias, CancellationToken cancellationToken = default)
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
- `cancellationToken`
  - Type: `CancellationToken`
  - Description: The cancellation token for the asynchronous operation.
#### Returns
Highlighted code.
#### Exceptions
- `ArgumentNullException`
  - Thrown if `code` is null.
- `ArgumentException`
  - Thrown if `languageAlias` is not a valid Prism language alias.
- `InvocationException`
  - Thrown if a NodeJS error occurs.
- `ObjectDisposedException`
  - Thrown if this instance has been disposed or if an attempt is made to use one of its dependencies that has been disposed.
- `OperationCanceledException`
  - Thrown if `cancellationToken` is cancelled.
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
Task<bool> IsValidLanguageAliasAsync(string languageAlias, CancellationToken cancellationToken = default)
```
#### Description
Determines whether a language alias is valid.
#### Parameters
- `languageAlias`
  - Type: `string`
  - Description: Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.
- `cancellationToken`
  - Type: `CancellationToken`
  - Description: The cancellation token for the asynchronous operation.
#### Returns
`true` if `languageAlias` is a valid Prism language alias. Otherwise, `false`.
#### Exceptions
- `InvocationException`
  - Thrown if a NodeJS error occurs.
- `ObjectDisposedException`
  - Thrown if this instance has been disposed or if an attempt is made to use one of its dependencies that has been disposed.
- `OperationCanceledException`
  - Thrown if `cancellationToken` is cancelled.
#### Example
```csharp
bool isValid = await prismService.IsValidLanguageAliasAsync("csharp");
```

## Building and Testing
You can build and test this project in Visual Studio 2017/2019.

## Related Jering Projects
#### Similar Projects
[Jering.Web.SyntaxHighlighters.HighlightJS](https://github.com/JeringTech/Web.SyntaxHighlighters.HighlightJS) - Use the Syntax Highlighter, HighlightJS, from C#.
#### Projects Using this Library
[Jering.Markdig.Extensions.FlexiBlocks](https://github.com/JeringTech/Markdig.Extensions.FlexiBlocks) - A Collection of Flexible Markdig Extensions.
#### Projects this Library Uses
[Jering.Javascript.NodeJS](https://github.com/JeringTech/Javascript.NodeJS) - Invoke Javascript in NodeJS, from C#.

## Related Concepts
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


## Contributing
Contributions are welcome!  

## About
Follow [@JeringTech](https://twitter.com/JeringTech) for updates and more.
