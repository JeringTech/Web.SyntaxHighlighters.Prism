# Changelog
This project uses [semantic versioning](http://semver.org/spec/v2.0.0.html). Refer to 
*[Semantic Versioning in Practice](https://www.jering.tech/articles/semantic-versioning-in-practice)*
for an overview of semantic versioning.

## [Unreleased](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/4.1.1...HEAD)

## [4.1.1](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/4.1.0...4.1.1) - Jan 19, 2018
### Changes
- Bumped `Jering.Javascript.NodeJS` to `4.1.1`.
### Fixes
- Fixed `StaticPrismService` concurrency issue. ([4e7b872](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/pull/8/commits/4e7b8724dfc105642c06832b50770e3db584b994))
- Fixed NuGet package's `PackageLicenseUrl` metadata.

## [4.1.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/4.0.2...4.1.0) - Dec 3, 2018
### Additions
- Added `StaticPrismService.DisposeServiceProvider`.
### Fixes
- `StaticPrismService.Invoke*` methods are now thread-safe.

## [4.0.2](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/4.0.1...4.0.2) - Nov 30, 2018
### Changes
- Changed project URL (used by NuGet.org) from `jering.tech/utilities/web.syntaxhighlighters.prism` to `jering.tech/utilities/jering.web.syntaxhighlighters.prism` for consistency with other Jering projects.

## [4.0.1](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/4.0.0...4.0.1) - Nov 29, 2018
### Changes
- Bumped `Jering.Javascript.NodeJS` to 4.0.3.
### Fixes
- Fixed inaccurate Nuget package metadata.

## [4.0.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/3.2.0...4.0.0) - Nov 28, 2018
### Additions
- Added `StaticPrismService`.
- Added `Dispose(bool disposed)` and a finalizer to `PrismService`.
### Changes
- **Breaking**: `IPrismService.HighlightAsync` and `IPrismService.IsValidLanguageAliasAsync` now take
a `CancellationToken` as an optional argument.
- Improved XML comments.

## [3.2.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/3.1.0...3.2.0) - Oct 4, 2018
### Changes
- Bumped `Microsoft.Extensions.DependencyInjection` version.

## [3.1.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/3.0.0...3.1.0) - Aug 9, 2018
### Changes
- Bumped `Jering.Javascript.NodeJS` version.

## [3.0.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/2.1.0...3.0.0) - Aug 6, 2018
### Changes
- Renamed project to `Jering.Web.SyntaxHighlighters.Prism` for consistency with other `Jering` packages. Using statements must be updated to reference types from the
namespace `Jering.Web.SyntaxHighlighters.Prism` instead of `Jering.WebUtils.SyntaxHighlighters.Prism`.
- Added .NET Standard 1.3 as a target framework.

## [2.1.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/2.0.3...2.1.0) - Jul 24, 2018
### Additions
- Added XML comments in Nuget package.
### Changes
- Replaced [Microsoft.AspNetCore.NodeServices](https://github.com/aspnet/JavaScriptServices/tree/master/src/Microsoft.AspNetCore.NodeServices) with 
  [JavascriptUtils.NodeJS](https://github.com/JeringTech/JavascriptUtils.NodeJS) for IPC with Node.js.
- Renamed assembly to Jering.WebUtils.SyntaxHighlighters.Prism.

## [2.0.3](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/2.0.2...2.0.3) - Jul 3, 2018
### Fixes
- Fixed dev dependencies not being excluded from Nuget package.

## [2.0.2](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/2.0.1...2.0.2) - Jun 28, 2018
### Fixes
- Fixed javascript bundle not being minified.

## [2.0.1](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/2.0.0...2.0.1) - Jun 28, 2018
### Fixes
- Fixed null argument exception message for `HighlightAsync`.

## [2.0.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/1.0.1...2.0.0) - Jun 14, 2018
### Changes
- Changed async method names.

## [1.0.1](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/1.0.0...1.0.1) - Jun 13, 2018
### Fixes
- Fixed Nuget package properties. Urls were pointing to a non-existent repository.

## [1.0.0](https://github.com/JeringTech/Web.SyntaxHighlighters.Prism/compare/1.0.0-beta.1...1.0.0) - Jun 12, 2018
### Changes
- Renamed Prism to PrismService.
- Cleaned up exception handling in PrismService members.

## 1.0.0-beta.1 - Jun 8, 2018
Initial release.
