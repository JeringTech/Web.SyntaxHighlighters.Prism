using Jering.Javascript.NodeJS;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Jering.Web.SyntaxHighlighters.Prism.Tests
{
    public class StaticPrismServiceIntegrationTests
    {
        [Fact]
        public async void Configure_ConfiguresOptions()
        {
            // Act
            // Highlight once to ensure that an initial PrismService is created. The invocation after configuration should properly dispose of this initial instance and create a new one with the
            // specified options.
            await StaticPrismService.IsValidLanguageAliasAsync("csharp").ConfigureAwait(false);
            StaticPrismService.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = 0);

            // Assert
            // Since we set timeout to 0, the NodeJS invocation is gauranteed to timeout. The NodeJS connection attempt is likely to timeout. Both throw an InvocationException.
            await Assert.ThrowsAsync<InvocationException>(async () => await StaticPrismService.IsValidLanguageAliasAsync("csharp").ConfigureAwait(false)).ConfigureAwait(false);

            // Reset so other tests aren't affected
            StaticPrismService.Configure<OutOfProcessNodeJSServiceOptions>(options => options.TimeoutMS = 60000);
        }

        [Theory]
        [MemberData(nameof(HighlightAsync_HighlightsCode_Data))]
        public async Task HighlightAsync_HighlightsCode(string dummyCode, string dummyLanguageAlias, string expectedResult)
        {
            // Act
            string result = await StaticPrismService.HighlightAsync(dummyCode, dummyLanguageAlias).ConfigureAwait(false);

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
            // Act
            bool result = await StaticPrismService.IsValidLanguageAliasAsync(dummyLanguageAlias).ConfigureAwait(false);

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
    }
}