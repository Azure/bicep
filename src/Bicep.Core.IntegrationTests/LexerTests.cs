// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class LexerTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldRoundtrip(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep), ToListDiagnosticWriter.Create());
            lexer.Lex();

            var serialized = new StringBuilder();
            new TokenWriter(serialized).WriteTokens(lexer.GetTokens());

            serialized.ToString().Should().Be(dataSet.Bicep, "because the lexer should not lose information");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldProduceValidTokenLocations(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep), ToListDiagnosticWriter.Create());
            lexer.Lex();

            foreach (Token token in lexer.GetTokens())
            {
                // lookup the text of the token in original contents by token's span
                string tokenText = dataSet.Bicep[new Range(token.Span.Position, token.GetEndPosition())];

                tokenText.Should().Be(token.Text, "because token text at location should match original contents at the same location");
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldProduceContiguousSpans(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep), ToListDiagnosticWriter.Create());
            lexer.Lex();

            int visitedPosition = 0;

            // local function
            void VisitSpan(TextSpan span, string text)
            {
                // the token should begin at the position where the previous token ended
                span.Position.Should().Be(visitedPosition, $"because token or trivia '{text}' at span '{span}' should begin at position {visitedPosition}.");

                // cover the span of the token
                visitedPosition += span.Length;
            }

            // local function
            void VisitTrivia(IEnumerable<SyntaxTrivia> trivia)
            {
                foreach (var trivium in trivia)
                {
                    VisitSpan(trivium.Span, trivium.Text);
                }
            }

            var tokens = lexer.GetTokens();
            foreach (var token in tokens)
            {
                VisitTrivia(token.LeadingTrivia);
                VisitSpan(token.Span, token.Text);
                VisitTrivia(token.TrailingTrivia);
            }

            // when we're done visited position should match the length of the file
            visitedPosition.Should().Be(dataSet.Bicep.Length);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void LexerShouldProduceExpectedTokens(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep), ToListDiagnosticWriter.Create());
            lexer.Lex();

            string getLoggingString(Token token)
            {
                return $"{token.Type} |{token.Text}|";
            }

            var sourceTextWithDiags = DataSet.AddDiagsToSourceText(dataSet, lexer.GetTokens(), getLoggingString);
            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainTokens), sourceTextWithDiags);

            sourceTextWithDiags.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Tokens,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainTokens),
                actualLocation: resultsFile);

            lexer.GetTokens().Count(token => token.Type == TokenType.EndOfFile).Should().Be(1, "because there should only be 1 EOF token");
            lexer.GetTokens().Last().Type.Should().Be(TokenType.EndOfFile, "because the last token should always be EOF.");
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void ParamsFile_LexerShouldProduceExpectedTokens(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);

            var lexer = new Lexer(new SlidingTextWindow(data.Parameters.EmbeddedFile.Contents), ToListDiagnosticWriter.Create());
            lexer.Lex();

            string getLoggingString(Token token)
            {
                return $"{token.Type} |{token.Text}|";
            }

            var sourceTextWithDiags = OutputHelper.AddDiagsToSourceText(data.Parameters.EmbeddedFile.Contents, "\n", lexer.GetTokens(), getLoggingString);

            data.Tokens.WriteToOutputFolder(sourceTextWithDiags);
            data.Tokens.ShouldHaveExpectedValue();

            lexer.GetTokens().Count(token => token.Type == TokenType.EndOfFile).Should().Be(1, "because there should only be 1 EOF token");
            lexer.GetTokens().Last().Type.Should().Be(TokenType.EndOfFile, "because the last token should always be EOF.");
        }

        [DataTestMethod]
        [DynamicData(nameof(GetValidData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void LexerShouldProduceValidStringLiteralTokensOnValidFiles(DataSet dataSet)
        {
            var lexer = new Lexer(new SlidingTextWindow(dataSet.Bicep), ToListDiagnosticWriter.Create());
            lexer.Lex();

            foreach (Token stringToken in lexer.GetTokens().Where(token => token.Type == TokenType.StringComplete))
            {
                Lexer.TryGetStringValue(stringToken).Should().NotBeNull($"because string token at span {stringToken.Span} should have a string value. Token Text = {stringToken.Text}");
            }
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();

        private static IEnumerable<object[]> GetValidData() => DataSets.AllDataSets.Where(ds => ds.IsValid).ToDynamicTestData();
    }
}

