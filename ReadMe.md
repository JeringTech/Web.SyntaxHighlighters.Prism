# SyntaxHighlighters
[![Build status](https://ci.appveyor.com/api/projects/status/ytdm1ft3s5gsmcdp?svg=true)](https://ci.appveyor.com/project/JeremyTCD/webutils-syntaxhighlighters)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](https://github.com/Pkcs11Interop/Pkcs11Interop/blob/master/LICENSE.md)
<!-- TODO tests badge, this service should work - https://github.com/monkey3310/appveyor-shields-badges/blob/master/README.md -->
<!-- TODO nuget badge, this service should work - 
[![NuGet][main-nuget-badge]][main-nuget]
[main-nuget]: https://www.nuget.org/packages/JeremyTCD.WebUtils.SyntaxHighlighters.Prism/
[main-nuget-badge]: https://img.shields.io/nuget/vpre/JeremyTCD.WebUtils.SyntaxHighlighters.Prism.svg?label=nuget
--> 

## Overview
Syntax highlighters add markup to code to facilitate styling. For example, the following code:

```csharp
public string ExampleFunction(string arg)
{
    // Example comment
    return arg + "dummyString";
}
```

is transformed into the following markup by [Prism](https://github.com/PrismJS/prism):

```html
<span class="token keyword">public</span> <span class="token keyword">string</span> <span class="token function">ExampleFunction</span><span class="token punctuation">(</span><span class="token keyword">string</span> arg<span class="token punctuation">)</span>
<span class="token punctuation">{</span>
    <span class="token comment">// Example comment</span>
    <span class="token keyword">return</span> arg <span class="token operator">+</span> <span class="token string">"dummyString"</span><span class="token punctuation">;</span>
<span class="token punctuation">}</span>
```

Syntax highlighting is often done client-side. There are however, situations where syntax highlighting can't or shouldn't be done client-side, for example:
- When generating pages for [AMP](https://www.ampproject.org/), since AMP does not allow scripts.
- When page load time is critical.
- When page size is critical.

This repository is a collection of .Net wrapper libraries for javascript syntax highlighters. These wrapper libraries allow syntax highlighting to be done by .Net server-side applications and static site generators.  

This repository contains the following wrapper libraries:
- [JeremyTCD.WebUtils.SyntaxHighlighters.Prism](#JeremyTCD.WebUtils.SyntaxHighlighters.Prism) wraps [Prism](https://github.com/PrismJS/prism)
- JeremyTCD.WebUtils.SyntaxHighlighters.Highlightjs wraps [highlight.js](https://github.com/isagalaev/highlight.js/) (*coming soon*)

## JeremyTCD.WebUtils.SyntaxHighlighters.Prism
### Prerequisites
[Node.js](https://nodejs.org/en/) must be installed and node.exe's directory must be added to the `Path` environment variable.

### Installation
Using Package Manager:
```
PM> Install-Package JeremyTCD.WebUtils.SyntaxHighlighters.Prism
```
Using .Net CLI:
```
> dotnet add package JeremyTCD.WebUtils.SyntaxHighlighters.Prism
```

### Usage
#### Creating `IPrism` in ASP.NET Apps
ASP.Net Core has a built in dependency injection (DI) system. This system will handle instantiation and disposal of `IPrism` instances.
Call `AddPrism` in `Startup.ConfigureServices` to register a service for `IPrism`:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add Prism
    services.AddPrism();
}
```
You can then inject IPrism into a controller:
```csharp
public MyController(IPrism prism)
{
    _prism = prism;
}
```

#### Creating IPrism in non-ASP.NET Apps
In non-ASP.NET projects, you'll have to create your own DI container. For example, using types from [Microsoft.Extensions.DependencyInjection](https://github.com/aspnet/DependencyInjection):
```csharp
var services = new ServiceCollection();
services.AddPrism();
ServiceProvider serviceProvider = services.BuildServiceProvider();
IPrism prism = serviceProvider.GetRequiredService<IPrism>();
```
The `IPrism` instance should be reused: it spawns a Node.js process to execute javascript, so creating multiple `IPrism` instances will result in multiple Node.js processes. 
Where possible, inject `IPrism` into your types or keep a reference to the `IPrism` instance. `IPrism`'s members are thread safe.

When you're done, you can manually dispose of the `IPrism` instance by calling
```csharp
prism.Dispose();
```
or 
```csharp
serviceProvider.Dispose(); // Calls Dispose on objects it has instantiated that are disposable
```
`Dispose` kills the spawned Node.js process.
Note that even if `Dispose` isn't called manually, the service that manages the Node.js process, `INodeService` from [Microsoft.AspNetCore.NodeServices](https://github.com/aspnet/JavaScriptServices/tree/dev/src/Microsoft.AspNetCore.NodeServices), will kill the 
Node.js process when the application shuts down - if the application shuts down gracefully. If the application does not shutdown gracefully, a [script](https://github.com/aspnet/JavaScriptServices/blob/dev/src/Microsoft.AspNetCore.NodeServices/TypeScript/Util/ExitWhenParentExits.ts)
running in the Node.js process will kill the process when it detects that its parent has been killed. Essentially, calling `Dispose` is not mandatory.

#### API
##### Prism.Highlight
###### Signature
```csharp
public virtual async Task<string> Highlight(string code, string languageAlias)
```
###### Parameters
- `code`
  - Type: `string`
  - Description: Code to highlight.
- `languageAlias`
  - Type: `string`
  - Description: A Prism language alias. Visit https://prismjs.com/index.html#languages-list for a list of language aliases.
###### Returns
A `string` containing highlighted code.
###### Exceptions
- `ArgumentNullException`
  - Thrown if `code` is null.
- `ArgumentException`
  - Thrown if `languageAlias` is not a valid Prism language alias.
- `NodeInvocationException`
  - Thrown if a Node error occurs.
###### Example
```csharp
string code = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";

string highlightedCode = await prism.Highlight(code, "csharp");
```
##### Prism.IsValidLanguageAlias
###### Signature
```csharp
public virtual async Task<bool> IsValidLanguageAlias(string languageAlias)
```
###### Parameters
- `languageAlias`
  - Type: `string`
  - Description: Language alias to validate. Visit https://prismjs.com/index.html#languages-list for a list of language aliases.
###### `Returns`
Returns true if `languageAlias` is a valid Prism language alias. Otherwise, returns false.
###### Exceptions
- `NodeInvocationException`
  - Thrown if a Node error occurs.
###### Example
```csharp
bool isValid = await prism.IsValidLanguageAlias("csharp");
```

## Building
The solution in this repository can be built in Visual Studio 2017.

## Contributing
Contributions are welcome! Follow [@JeremyTCD](https://twitter.com/intent/user?screen_name=JeremyTCD) for updates.