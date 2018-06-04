using Microsoft.AspNetCore.NodeServices.HostingModels;
using System;
using Xunit;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Tests
{
    public class PrismUnitTests
    {
        [Fact]
        public void Highlight_ThrowsInvalidOperationExceptionIfEngineOptionsIsOfTypeColorCodeOptionsButLanguageAliasHasNoCorrespondingColorCodeLanguage()
        {
        }

        [Fact]
        public void Highlight_ThrowsAggregateExceptionIfNodeErrorOccurs()
        {
            // Arrange
            //const string dummyCode = "dummyCode";
            //const string dummyLanguage = "dummyLanguage";
            //ISyntaxHighlighter highlighter = null;// TODO CreateHighlighter();

            //// Act and assert
            //AggregateException result = Assert.Throws<AggregateException>(() => highlighter.Highlight(dummyCode, dummyLanguage).Result);
            //Assert.IsType<NodeInvocationException>(result.InnerException);
        }
    }
}
