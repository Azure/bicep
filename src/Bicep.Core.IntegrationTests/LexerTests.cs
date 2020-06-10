using System;
using System.Collections.Generic;
using System.Text;
using Bicep.Core.IntegrationTests.UnitSamples;
using Bicep.Core.Parser;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Tests
{
    [TestClass]
    public class LexerTests
    {
        [DataTestMethod]
        [UnitSamplesDataSource]
        public void LexerShouldRoundtrip(string displayName, string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var serialized = new StringBuilder();
            WriteTokens(serialized, lexer.GetTokens());

            serialized.ToString().Should().Be(contents, "because the lexer should not lose information");
        }

        [DataTestMethod]
        [UnitSamplesDataSource]
        public void LexerShouldProduceValidTokenLocations(string displayName, string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            foreach (Token token in lexer.GetTokens())
            {
                // lookup the text of the token in original contents by token's span
                string tokenText = contents[new Range(token.Span.Position, token.Span.Position + token.Span.Length)];

                tokenText.Should().Be(token.Text, "because token text at location should match original contents at the same location");
            }
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
