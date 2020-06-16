using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Samples
{
    [TestClass]
    public class DataSetsTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void DataSetShouldBeValid(DataSet dataSet)
        {
            dataSet.Name.Should().NotBeNullOrWhiteSpace();
            dataSet.DisplayName.Should().NotBeNullOrWhiteSpace();
            
            // Bicep files may be empty
            dataSet.Bicep.Should().NotBeNull();

            // tokens are serialized in JSON format, so can't be whitespace
            dataSet.Tokens.Should().NotBeNullOrWhiteSpace();

            dataSet.Errors.Should().NotBeNullOrWhiteSpace();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void DataSetBicepLineEndingsShouldMatchDataSetNameSuffix(DataSet dataSet)
        {
            var lineEndingTokens = GetLineEndingTokens(dataSet.Bicep);
            lineEndingTokens.Select(token => token.Type).Should().AllBeEquivalentTo(TokenType.NewLine);

            string? expectedLineEnding = null;
            if (dataSet.Name.EndsWith("_CRLF"))
            {
                expectedLineEnding = "\r\n";
            }

            if (dataSet.Name.EndsWith("_LF"))
            {
                expectedLineEnding = "\n";
            }

            if (expectedLineEnding != null)
            {
                lineEndingTokens.All(token => string.Equals(token.Text, expectedLineEnding, StringComparison.Ordinal)).Should().BeTrue();
            }
        }

        private IEnumerable<Token> GetLineEndingTokens(string contents)
        {
            var lexer = new Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            return lexer.GetTokens().Where(token => token.Type == TokenType.NewLine);
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
