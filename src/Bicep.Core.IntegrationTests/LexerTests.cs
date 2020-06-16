using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class LexerTests
    {
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldRoundtrip(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep));
            lexer.Lex();

            var serialized = new StringBuilder();
            new TokenWriter(serialized).WriteTokens(lexer.GetTokens());

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

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldProduceExpectedTokens(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep));
            lexer.Lex();

            var actual = JToken.FromObject(lexer.GetTokens().Select(token => new TokenItem(token)), DataSetSerialization.CreateSerializer());
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Tokens_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(dataSet.Tokens);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Tokens_Expected.json", expected.ToString(Formatting.Indented));

            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Tokens_Delta.json");
        }

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.ToDynamicTestData();
        }
    }
}
