using Jering.Javascript.NodeJS;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Jering.Web.SyntaxHighlighters.Prism.Tests
{
    public class PrismServiceIntegrationTests : IDisposable
    {
        private IServiceProvider _serviceProvider;

        [Theory]
        [MemberData(nameof(HighlightAsync_HighlightsCode_Data))]
        public async Task HighlightAsync_HighlightsCode(string dummyCode, string dummyLanguageAlias, string expectedResult)
        {
            // Arrange 
            IPrismService prismService = CreatePrismService();

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

        [Fact]
        public void HighlightAsync_IsThreadSafe()
        {
            // Arrange
            const string dummyCode = @"public string ExampleFunction(string arg)
{
    // Example comment
    return arg + ""dummyString"";
}";
            const string dummyLanguageAlias = "csharp";
            IPrismService prismService = CreatePrismService();

            // Act
            var results = new ConcurrentQueue<string>();
            const int numThreads = 5;
            var threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(() => results.Enqueue(prismService.HighlightAsync(dummyCode, dummyLanguageAlias).GetAwaiter().GetResult()));
                threads.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            Assert.Equal(numThreads, results.Count);
            foreach (string result in results)
            {
                Assert.Equal(@"<span class=""token keyword"">public</span> <span class=""token keyword"">string</span> <span class=""token function"">ExampleFunction</span><span class=""token punctuation"">(</span><span class=""token keyword"">string</span> arg<span class=""token punctuation"">)</span>
<span class=""token punctuation"">{</span>
    <span class=""token comment"">// Example comment</span>
    <span class=""token keyword"">return</span> arg <span class=""token operator"">+</span> <span class=""token string"">""dummyString""</span><span class=""token punctuation"">;</span>
<span class=""token punctuation"">}</span>",
                result);
            }
        }

        [Theory]
        [MemberData(nameof(IsValidLanguageAliasAsync_ChecksIfLanguageAliasIsValid_Data))]
        public async Task IsValidLanguageAliasAsync_ChecksIfLanguageAliasIsValid(string dummyLanguageAlias, bool expectedResult)
        {
            // Arrange
            IPrismService prismService = CreatePrismService();

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

        [Fact]
        public void IsValidLanguageAliasAsync_IsThreadSafe()
        {
            // Arrange
            const string dummyLanguageAlias = "csharp";
            IPrismService prismService = CreatePrismService();

            // Act
            var results = new ConcurrentQueue<bool>();
            const int numThreads = 5;
            var threads = new List<Thread>();
            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(() => results.Enqueue(prismService.IsValidLanguageAliasAsync(dummyLanguageAlias).GetAwaiter().GetResult()));
                threads.Add(thread);
                thread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Assert
            Assert.Equal(numThreads, results.Count);
            foreach (bool result in results)
            {
                Assert.True(result);
            }
        }

        private IPrismService CreatePrismService()
        {
            var services = new ServiceCollection();

            services.AddPrism();
            if (Debugger.IsAttached)
            {
                services.Configure<NodeJSProcessOptions>(options => options.NodeAndV8Options = "--inspect-brk");
                services.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = -1);
            }
            _serviceProvider = services.BuildServiceProvider();

            return _serviceProvider.GetRequiredService<IPrismService>();
        }

        public void Dispose()
        {
            // Ensure that NodeJSService gets disposed
            ((IDisposable)_serviceProvider).Dispose();
        }
    }
}