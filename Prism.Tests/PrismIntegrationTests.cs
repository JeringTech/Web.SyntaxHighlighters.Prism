using Microsoft.AspNetCore.NodeServices.HostingModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Tests
{
    public class PrismIntegrationTests : IDisposable
    {
        private ServiceProvider _serviceProvider;

        [Theory]
        [MemberData(nameof(Highlight_HighlightsCode_Data))]
        public void Highlight_HighlightsCode(string dummyCode, string dummyLanguageAlias, string expectedResult)
        {
            // Arrange 
            IPrism prism = CreatePrism();

            // Act
            string result = prism.Highlight(dummyCode, dummyLanguageAlias).Result;

            // Assert
            Assert.Equal(expectedResult, result);
        }
        
        public static IEnumerable<object[]> Highlight_HighlightsCode_Data()
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

        private IPrism CreatePrism()
        {
            // Since a new container is created for each test, a new INodeServices instance is created as well.
            // This means that a new node process is started and then disposed of for each test. 
            // It is cleaner to do things this way, but reconsider if performance becomes an issue.
            var services = new ServiceCollection();
            services.AddPrism();

            _serviceProvider = services.BuildServiceProvider();

            return _serviceProvider.GetRequiredService<IPrism>();
        }

        public void Dispose()
        {
            // Ensure that NodeServices gets disposed
            _serviceProvider?.Dispose();
        }
    }
}