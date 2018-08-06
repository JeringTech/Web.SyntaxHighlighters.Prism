# Changelog
This project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html). Refer to 
[The Semantic Versioning Lifecycle](https://www.jeremytcd.com/articles/the-semantic-versioning-lifecycle)
for an overview of semantic versioning.

## [Unreleased](https://github.com/JeremyTCD/Web.SyntaxHighlighters.Prism/compare/2.1.0...HEAD)

## 2.1.0 - Jul 24, 2018
### Changes
- Replaced [Microsoft.AspNetCore.NodeServices](https://github.com/aspnet/JavaScriptServices/tree/master/src/Microsoft.AspNetCore.NodeServices) with 
  [JavascriptUtils.NodeJS](https://github.com/JeremyTCD/JavascriptUtils.NodeJS) for IPC with Node.js.
- Renamed assembly to Jering.WebUtils.SyntaxHighlighters.Prism.
### Additions
- Added XML comments in Nuget package.


## 2.0.3 - Jul 3, 2018
### Fixes
- Fixed dev dependencies not being excluded from Nuget package.

## 2.0.2 - Jun 28, 2018
### Fixes
- Fixed javascript bundle not being minified.

## 2.0.1 - Jun 28, 2018
### Fixes
- Fixed null argument exception message for `HighlightAsync`.

## 2.0.0 - Jun 14, 2018
### Changes
- Changed async method names.

## 1.0.1 - Jun 13, 2018
### Fixes
- Fixed Nuget package properties. Urls were pointing to a non-existent repository.

## 1.0.0 - Jun 12, 2018
### Changes
- Renamed Prism to PrismService.
- Cleaned up exception handling in PrismService members.

## 1.0.0-beta.1 - Jun 8, 2018
Initial release.