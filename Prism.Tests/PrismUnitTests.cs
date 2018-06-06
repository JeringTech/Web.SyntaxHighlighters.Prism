using Microsoft.AspNetCore.NodeServices;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace JeremyTCD.WebUtils.SyntaxHighlighters.Prism.Tests
{
    public class PrismUnitTests
    {
        private readonly MockRepository _mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };

        [Fact]
        public async Task Highlight_ThrowsExceptionIfCodeIsNull()
        {
            // Arrange
            Mock<Prism> mockPrism = CreateMockPrism();
            mockPrism.CallBase = true;

            // Act and assert
            ArgumentException result = await Assert.ThrowsAsync<ArgumentException>(() => mockPrism.Object.Highlight(null, null)).ConfigureAwait(false);
        }

        [Theory]
        [MemberData(nameof(Highlight_ReturnsCodeIfCodeIsEmptyOrWhitespace_Data))]
        public async Task Highlight_ReturnsCodeIfCodeIsEmptyOrWhitespace(string dummyCode)
        {
            // Arrange
            Mock<Prism> mockPrism = CreateMockPrism();
            mockPrism.CallBase = true;

            // Act
            string result = await mockPrism.Object.Highlight(dummyCode, null).ConfigureAwait(false);

            // Assert
            Assert.Equal(dummyCode, result);
        }

        public static IEnumerable<object[]> Highlight_ReturnsCodeIfCodeIsEmptyOrWhitespace_Data()
        {
            return new object[][]
            {
                new object[]
                {
                    string.Empty
                },
                new object[]
                {
                    " "
                }
            };
        }

        [Fact]
        public async Task Highlight_ThrowsExceptionIfLanguageAliasIsNotAValidPrismLanguageAlias()
        {
            // Arrange
            const string dummyCode = "dummyCode";
            const string dummyLanguageAlias = "dummyLanguageAlias";
            Mock<Prism> mockPrism = CreateMockPrism();
            mockPrism.CallBase = true;
            mockPrism.Setup(p => p.IsValidLanguageAlias(dummyLanguageAlias)).ReturnsAsync(false);

            // Act and assert
            ArgumentException result = await Assert.ThrowsAsync<ArgumentException>(() => mockPrism.Object.Highlight(dummyCode, dummyLanguageAlias)).ConfigureAwait(false);
            Assert.Equal(result.Message, string.Format(Strings.Exception_InvalidPrismLanguageAlias, dummyLanguageAlias));
        }

        [Fact]
        public async Task Highlight_ThrowsExceptionIfANodeErrorOccurs()
        {
            // Arrange
            const string dummyCode = "dummyCode";
            const string dummyLanguageAlias = "dummyLanguageAlias";
            var dummyAggregateException = new AggregateException();
            Mock<INodeServices> mockNodeServices = _mockRepository.Create<INodeServices>();
            mockNodeServices.Setup(n => n.InvokeExportAsync<string>(Prism.INTEROP_FILE, "highlight", dummyCode, dummyLanguageAlias)).ThrowsAsync(dummyAggregateException);
            Mock<Prism> mockPrism = CreateMockPrism(mockNodeServices.Object);
            mockPrism.CallBase = true;
            mockPrism.Setup(p => p.IsValidLanguageAlias(dummyLanguageAlias)).ReturnsAsync(true);

            // Act and assert
            AggregateException result = await Assert.ThrowsAsync<AggregateException>(() => mockPrism.Object.Highlight(dummyCode, dummyLanguageAlias)).ConfigureAwait(false);
            Assert.Same(dummyAggregateException, result);
        }

        [Fact]
        public async Task Highlight_IfSuccessfulInvokesHighlightInInteropJSAndReturnsHighlightedCode()
        {
            // Arrange
            const string dummyCode = "dummyCode";
            const string dummyHighlightedCode = "dummyHighlightedCode";
            const string dummyLanguageAlias = "dummyLanguageAlias";
            Mock<INodeServices> mockNodeServices = _mockRepository.Create<INodeServices>();
            mockNodeServices.Setup(n => n.InvokeExportAsync<string>(Prism.INTEROP_FILE, "highlight", dummyCode, dummyLanguageAlias)).ReturnsAsync(dummyHighlightedCode);
            Mock<Prism> mockPrism = CreateMockPrism(mockNodeServices.Object);
            mockPrism.CallBase = true;
            mockPrism.Setup(p => p.IsValidLanguageAlias(dummyLanguageAlias)).ReturnsAsync(true);

            // Act
            string result = await mockPrism.Object.Highlight(dummyCode, dummyLanguageAlias).ConfigureAwait(false);

            // Assert
            Assert.Equal(dummyHighlightedCode, result);
        }

        [Theory]
        [MemberData(nameof(IsValidLanguageAlias_ReturnsFalseIfLanguageAliasIsNullOrWhitespace_Data))]
        public async Task IsValidLanguageAlias_ReturnsFalseIfLanguageAliasIsNullOrWhitespace(string dummyLanguageAlias)
        {
            // Arrange
            Mock<Prism> mockPrism = CreateMockPrism();
            mockPrism.CallBase = true;

            // Act
            bool result = await mockPrism.Object.IsValidLanguageAlias(dummyLanguageAlias).ConfigureAwait(false);

            // Assert
            Assert.False(result);
        }

        public static IEnumerable<object[]> IsValidLanguageAlias_ReturnsFalseIfLanguageAliasIsNullOrWhitespace_Data()
        {
            return new object[][]
            {
                new object[]
                {
                    string.Empty
                },
                new object[]
                {
                    " "
                }
            };
        }

        [Theory]
        [MemberData(nameof(IsValidLanguageAlias_IfSuccessfulReturnsTrueIfAliasesContainsLanguageAliasAndFalseIfItDoesNot_Data))]
        public async Task IsValidLanguageAlias_IfSuccessfulReturnsTrueIfAliasesContainsLanguageAliasAndFalseIfItDoesNot(
            string dummyLanguageAlias, 
            string[] dummyAliases, 
            bool expectedResult)
        {
            // Arrange
            Mock<Prism> mockPrism = CreateMockPrism();
            mockPrism.CallBase = true;
            mockPrism.Setup(p => p.GetAliases()).ReturnsAsync(dummyAliases);

            // Act
            bool result = await mockPrism.Object.IsValidLanguageAlias(dummyLanguageAlias).ConfigureAwait(false);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        public static IEnumerable<object[]> IsValidLanguageAlias_IfSuccessfulReturnsTrueIfAliasesContainsLanguageAliasAndFalseIfItDoesNot_Data()
        {
            const string dummyLanguageAlias = "dummyLanguageAlias";

            return new object[][]
            {
                // If aliases contains language alias, should return true
                new object[]
                {
                    dummyLanguageAlias,
                    new string[]{ dummyLanguageAlias },
                    true
                },
                // Otherwise, should return false
                new object[]
                {
                    dummyLanguageAlias,
                    new string[0],
                    false
                }
            };
        }

        private Mock<Prism> CreateMockPrism(INodeServices nodeServices = null)
        {
            return _mockRepository.Create<Prism>(nodeServices);
        }
    }
}
