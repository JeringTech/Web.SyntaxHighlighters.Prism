# Jering.WebUtils.SyntaxHighlighters.Prism
[![Build status](https://ci.appveyor.com/api/projects/status/swmelqsuwnw41d0h?svg=true)](https://ci.appveyor.com/project/JeremyTCD/webutils-syntaxhighlighters-prism)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/Pkcs11Interop/Pkcs11Interop/blob/master/LICENSE.md)
[![NuGet](https://img.shields.io/nuget/vpre/Jering.WebUtils.SyntaxHighlighters.Prism.svg?label=nuget)](https://www.nuget.org/packages/Jering.WebUtils.SyntaxHighlighters.Prism/)
<!-- TODO tests badge, this service should work - https://github.com/monkey3310/appveyor-shields-badges/blob/master/README.md -->

Perform Syntax Highlighting in .Net Applications Using the Javascript Library, [Prism](https://github.com/PrismJS/prism). 

## Table of Contents
[Overview](#overview)  
[Prerequisites](#prerequisites)  
[Installation](#installation)  
[Usage](#usage)  
[Building](#building)  
[Contributing](#contributing)  
[About](#about)

## Overview
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

WebUtils.SyntaxHighlighters.Prism allows syntax highlighting to be done by .Net server-side applications and tools like static site generators.

## Prerequisites
[Node.js](https://nodejs.org/en/) must be installed and node.exe's directory must be added to the `Path` environment variable.

## Installation
Using Package Manager:
```
PM> Install-Package Jering.WebUtils.SyntaxHighlighters.Prism
```
Using .Net CLI:
```
> dotnet add package Jering.WebUtils.SyntaxHighlighters.Prism
```

## Usage
### Creating `IPrismService` in ASP.NET Apps
ASP.NET has a built in dependency injection (DI) system. This system can handle instantiation and disposal of `IPrismService` instances.
Call `AddPrism` in `Startup.ConfigureServices` to register a service for `IPrismService`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddPrism();
}
```
You can then inject `IPrismService` into controllers:
```csharp
public MyController(IPrismService prismService)
{
    _prismService = prismService;
}
```

### Creating `IPrismService` in non-ASP.NET Apps
In non-ASP.NET projects, you'll have to create your own DI container. For example, using [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection):
```csharp
var services = new ServiceCollection();
services.AddPrism();
ServiceProvider serviceProvider = services.BuildServiceProvider();
IPrismService prismService = serviceProvider.GetRequiredService<IPrismService>();
```
`IPrismService` is a singleton service and `IPrismService`'s members are thread safe.
Where possible, inject `IPrismService` into your types or keep a reference to a shared `IPrismService` instance. 
Try to avoid creating multiple `IPrismService` instances, since each instance spawns a Node.js process. 

When you're done, you can manually dispose of an `IPrismService` instance by calling
```csharp
prismService.Dispose();
```
or 
```csharp
serviceProvider.Dispose(); // Calls Dispose on objects it has instantiated that are disposable
```
`Dispose` kills the spawned Node.js process.
Note that even if `Dispose` isn't called manually, the service that manages the Node.js process, `INodeJSService` from [Jering.JavascriptUtils.NodeJS](https://github.com/JeremyTCD/JavascriptUtils.NodeJS), will kill the 
Node.js process when the application shuts down - if the application shuts down gracefully. If the application does not shutdown gracefully, the Node.js process will kill 
itself when it detects that its parent has been killed. 
Essentially, manually disposing of `IPrismService` instances is not mandatory.

### API
#### IPrismService.HighlightAsync
##### Signature
```csharp
Task<string> HighlightAsync(string code, string languageAlias)
```
##### Description
Highlights code of a specified language.
##### Parameters
- `code`
  - Type: `string`
  - Description: Code to highlight.
- `languageAlias`
  - Type: `string`
  - Description: A Prism language alias. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.
##### Returns
Highlighted code.
##### Exceptions
- `ArgumentNullException`
  - Thrown if `code` is null.
- `ArgumentException`
  - Thrown if `languageAlias` is not a valid Prism language alias.
- `InvocationException`
  - Thrown if a NodeJS error occurs.
##### Example
```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

string highlightedCode = await prismService.HighlightAsync(code, "csharp");
```
#### IPrismService.IsValidLanguageAliasAsync
##### Signature
```csharp
ValueTask<bool> IsValidLanguageAliasAsync(string languageAlias)
```
##### Description
Determines whether a language alias is valid.
##### Parameters
- `languageAlias`
  - Type: `string`
  - Description: Language alias to validate. Visit https://prismjs.com/index.html#languages-list for the list of valid language aliases.
##### Returns
`true` if `languageAlias` is a valid Prism language alias. Otherwise, `false`.
##### Exceptions
- `InvocationException`
  - Thrown if a NodeJS error occurs.
##### Example
```csharp
bool isValid = await prismService.IsValidLanguageAliasAsync("csharp");
```

## Building
This project can be built using Visual Studio 2017.

## Contributing
Contributions are welcome!  

## About
Follow [@JeremyTCD](https://twitter.com/JeremyTCD) for updates and more.
