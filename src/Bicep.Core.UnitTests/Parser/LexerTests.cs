// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parser
{
    [TestClass]
    public class LexerTests
    {
        [DataTestMethod]
        [DataRow(@"''", "")]
        [DataRow(@"''", "")]
        [DataRow(@"'test'", "test")]
        [DataRow(@"'hello there'", "hello there")]
        [DataRow(@"'\r\n\t\\\$\''", "\r\n\t\\$'")]
        [DataRow("'First line\\nSecond\\ttabbed\\tline'", "First line\nSecond\ttabbed\tline")]
        public void TryGetStringValue_ValidStringLiteralToken_ShouldCalculateValueCorrectly(string literalText, string expectedValue)
        {
            var token = new Token(TokenType.StringComplete, new TextSpan(0, literalText.Length), literalText, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

            var actual = Lexer.TryGetStringValue(token);

            actual.Should().Be(expectedValue);
        }

        [TestMethod]
        public void TryGetStringValue_WrongTokenType_ShouldReturnNull()
        {
            var token = new Token(TokenType.Number, new TextSpan(0, 2), "12", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

            Lexer.TryGetStringValue(token).Should().BeNull();
        }

        [DataTestMethod]
        [DataRow(@"")]
        [DataRow(@"hi")]
        [DataRow(@"'hello'there")]
        [DataRow(@"'test\!'")]
        [DataRow(@"'")]
        [DataRow(@"'test")]
        [DataRow(@"'hi\")]
        public void GetStringValue_InvalidStringLiteralToken_ShouldReturnNull(string literalText)
        {
            var token = new Token(TokenType.StringComplete, new TextSpan(0, literalText.Length), literalText, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

            Lexer.TryGetStringValue(token).Should().BeNull();
        }

        [TestMethod]
        public void UnrecognizedTokens_ShouldNotBeRecognized()
        {
            RunSingleTokenTest("^", TokenType.Unrecognized, "The following token is not recognized: \"^\".", "BCP001");
        }

        [TestMethod]
        public void UnterminatedMultiLineComment_ShouldBeRecognizedWithError()
        {
            const string expectedTokenText = "'test'";
            RunSingleTokenTest(
                "'test'/* unfinished comment *",
                TokenType.StringComplete,
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.",
                "BCP002",
                expectedTokenText: expectedTokenText,
                expectedStartPosition: expectedTokenText.Length);

            RunSingleTokenTest(
                "'test'/* unfinished comment",
                TokenType.StringComplete,
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.",
                "BCP002",
                expectedTokenText: expectedTokenText,
                expectedStartPosition: expectedTokenText.Length);
        }

        [TestMethod]
        public void UnterminatedString_ShouldBeRecognizedWithError()
        {
            RunSingleTokenTest("'string does not end", TokenType.StringComplete, "The string at this location is not terminated. Terminate the string with a single quote character.", "BCP003");
            RunSingleTokenTest("'beginning an escape\\", TokenType.StringComplete, "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.", "BCP005");
        }

        [TestMethod]
        public void UnrecognizedEscapeSequence_ShouldBeRecognizedWithError()
        {
            RunSingleTokenTest(
                "'bad \\escape'",
                TokenType.StringComplete,
                "The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \"\\$\", \"\\'\", \"\\\\\", \"\\n\", \"\\r\", \"\\t\".",
                "BCP006",
                expectedStartPosition: 5,
                expectedLength: 2);
        }

        [TestMethod]
        public void UnterminatedStringUnexpectedNewline_ShouldBeRecognizedWithError()
        {
            var text = "'unfinished\n'even more unfinished\r\n'test'";
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            var tokens = lexer.GetTokens().ToImmutableArray();
            tokens.Should().HaveCount(6);

            tokens.Select(t => t.Type).Should().Equal(TokenType.StringComplete, TokenType.NewLine, TokenType.StringComplete, TokenType.NewLine, TokenType.StringComplete, TokenType.EndOfFile);

            var expectedTexts = new[]
            {
                "'unfinished",
                "\n",
                "'even more unfinished",
                "\r\n",
                "'test'",
                string.Empty
            };

            tokens.Select(t => t.Text).Should().Equal(expectedTexts);

            tokens.Select(t => t.Span.Position).Should().Equal(expectedTexts.Select((s, i) => expectedTexts.Take(i).Select(s => s.Length).Sum()));
            tokens.Select(t => t.Span.Length).Should().Equal(expectedTexts.Select(s => s.Length));
        }

        [DataRow("a")]
        [DataRow("ab")]
        [DataRow("a0")]
        [DataRow("resourceGroup")]
        [DataTestMethod]
        public void ValidIdentifier_IsValidIdentifier_ShouldReturnTrue(string value)
        {
            Lexer.IsValidIdentifier(value).Should().BeTrue();
        }

        [DataRow("")]
        [DataRow("2")]
        [DataRow("2a")]
        [DataRow("a-b")]
        [DataRow("abz-b")]
        [DataTestMethod]
        public void InvalidIdentifier_IsValidIdentifier_ShouldReturnFalse(string value)
        {
            Lexer.IsValidIdentifier(value).Should().BeFalse();
        }

        private static void RunSingleTokenTest(string text, TokenType expectedTokenType, string expectedMessage, string expectedCode, int expectedStartPosition = 0, int? expectedLength = null, string? expectedTokenText = null)
        {
            expectedTokenText ??= text;
            expectedLength ??= text.Length - expectedStartPosition;

            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            var tokens = lexer.GetTokens().ToImmutableArray();
            tokens.Should().HaveCount(2);
            tokens.Select(t => t.Type).Should().Equal(expectedTokenType, TokenType.EndOfFile);

            tokens.First().Text.Should().Be(expectedTokenText);

            var errors = diagnosticWriter.GetDiagnostics();
            errors.Should().HaveCount(1);

            var error = errors.Single();
            error.Message.Should().Be(expectedMessage);
            error.Code.Should().Be(expectedCode);
            error.Span.Position.Should().Be(expectedStartPosition);
            error.Span.Length.Should().Be(expectedLength);
        }
    }
}

