using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xunit;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Tests
{
    public class PrismServiceIntegrationTests : IDisposable
    {
        private ServiceProvider _serviceProvider;

        [Theory]
        [MemberData(nameof(HighlightAsync_HighlightsCode_Data))]
        public async Task HighlightAsync_HighlightsCode(string dummyCode, string dummyLanguageAlias, string expectedResult)
        {
            // Arrange 
            IPrismService prismService = await CreatePrismService().ConfigureAwait(false);

            // Act
            string result = await prismService.HighlightAsync(dummyCode, dummyLanguageAlias).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> HighlightAsync_HighlightsCode_Data()
        {
            return new object[][]
            {
                // javascript
                new object[]
                {
                    @"function exampleFunction(arg) {
    // Example comment
    return arg + 'dummyString';
}",
                    "javascript",
                    @"<span class=""token keyword"">function</span> <span class=""token function"">exampleFunction</span><span class=""token punctuation"">(</span>arg<span class=""token punctuation"">)</span> <span class=""token punctuation"">{</span>
    <span class=""token comment"">// Example comment</span>
    <span class=""token keyword"">return</span> arg <span class=""token operator"">+</span> <span class=""token string"">'dummyString'</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>"
                },

                // csharp
                new object[]
                {
                    @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}",
                    "csharp",
                    @"<span class=""token keyword"">public</span> <span class=""token keyword"">string</span> <span class=""token function"">ExampleFunction</span><span class=""token punctuation"">(</span><span class=""token keyword"">string</span> arg<span class=""token punctuation"">)</span>
<span class=""token punctuation"">{</span>
    <span class=""token comment"">// Example comment</span>
    <span class=""token keyword"">return</span> arg <span class=""token operator"">+</span> <span class=""token string"">""dummyString""</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>"
                }
            };
        }

        [Theory]
        [MemberData(nameof(IsValidLanguageAliasAsync_ChecksIfLanguageAliasIsValid_Data))]
        public async Task IsValidLanguageAliasAsync_ChecksIfLanguageAliasIsValid(string dummyLanguageAlias, bool expectedResult)
        {
            // Arrange
            IPrismService prismService = await CreatePrismService().ConfigureAwait(false);

            // Act
            bool result = await prismService.IsValidLanguageAliasAsync(dummyLanguageAlias).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> IsValidLanguageAliasAsync_ChecksIfLanguageAliasIsValid_Data()
        {
            return new object[][]
            {
                // Alias
                new object[]
                {
                    "html", true
                },

                // Actual language
                new object[]
                {
                    "css", true
                },

                // Non existent language
                new object[]
                {
                    "non-existent-language", false
                }
            };
        }

        private async Task<IPrismService> CreatePrismService()
        {
            // Since a new container is created for each test, a new INodeServices instance is created as well.
            // This means that a new node process is started and then disposed of for each test. 
            // It is cleaner to do things this way, but reconsider if performance becomes an issue.
            var services = new ServiceCollection();

            services.AddPrism();
            if (Debugger.IsAttached)
            {
                // Override INodeServices service registered by AddPrism to enable debugging
                services.AddNodeServices(options =>
                {
                    options.LaunchWithDebugging = true;
                    options.InvocationTimeoutMilliseconds = 99999999; // -1 doesn't work, once a js breakpoint is hit, the debugger disconnects
                });

                _serviceProvider = services.BuildServiceProvider();

                // InvokeAsync implicitly starts up a node instance. Adding a break point after InvokeAsync allows
                // chrome to connect to the debugger
                INodeServices nodeServices = _serviceProvider.GetRequiredService<INodeServices>();
                try
                {
                    int dummy = await nodeServices.InvokeAsync<int>("").ConfigureAwait(false);
                }
                catch
                {
                    // Do nothing
                }
            }
            else
            {
                _serviceProvider = services.BuildServiceProvider();
            }

            return _serviceProvider.GetRequiredService<IPrismService>();
        }

        public void Dispose()
        {
            // Ensure that NodeServices gets disposed
            _serviceProvider?.Dispose();
        }
    }
}