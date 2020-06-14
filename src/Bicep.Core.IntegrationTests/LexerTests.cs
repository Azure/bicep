using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class LexerTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldRoundtrip(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep));
            lexer.Lex();

            var serialized = new StringBuilder();
            WriteTokens(serialized, lexer.GetTokens());

            serialized.ToString().Should().Be(dataSet.Bicep, "because the lexer should not lose information");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldProduceValidTokenLocations(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep));
            lexer.Lex();

            foreach (Token token in lexer.GetTokens())
            {
                // lookup the text of the token in original contents by token's span
                string tokenText = dataSet.Bicep[new Range(token.Span.Position, token.Span.Position + token.Span.Length)];

                tokenText.Should().Be(token.Text, "because token text at location should match original contents at the same location");
            }
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.Select(ds => new object[] { ds });
        }

        private void WriteTokens(StringBuilder buffer, IEnumerable<Token> tokens)
        {
            foreach (Token token in tokens)
            {
                WriteToken(buffer, token);
            }
        }
        
        private void WriteToken(StringBuilder buffer, Token token)
        {
            buffer.Append(token.LeadingTrivia);
            buffer.Append(token.Text);
            buffer.Append(token.TrailingTrivia);
        }
    }
}
