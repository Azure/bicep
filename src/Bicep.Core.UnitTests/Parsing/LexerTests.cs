// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing
{
    [TestClass]
    public class LexerTests
    {
        [DataTestMethod]
        [DataRow(@"''", "")]
        [DataRow(@"'test'", "test")]
        [DataRow(@"'hello there'", "hello there")]
        [DataRow(@"'\r\n\t\\\$\''", "\r\n\t\\$'")]
        [DataRow("'First line\\nSecond\\ttabbed\\tline'", "First line\nSecond\ttabbed\tline")]
        // escape ascii
        [DataRow(@"'\u{0}'", "\0")]
        [DataRow(@"'\u{20}'", " ")]
        [DataRow(@"'hello\u{20}world\u{3f}'", "hello world?")]
        [DataRow(@"'new\u{0d}\u{A}line'", "new\r\nline")]
        // surrogate pairs (various options)
        [DataRow(@"'\u{10437}'", "\U00010437")]
        [DataRow(@"'\u{D801}\u{DC37}'", "\U00010437")]
        [DataRow(@"'\u{D801}\u{DC37}'", "\uD801\uDC37")]
        public void TryGetStringValue_ValidStringLiteralToken_ShouldCalculateValueCorrectly(string literalText, string expectedValue)
        {
            var token = new Token(TokenType.StringComplete, new TextSpan(0, literalText.Length), literalText, Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

            var actual = Lexer.TryGetStringValue(token);

            actual.Should().Be(expectedValue);
        }

        [TestMethod]
        public void TryGetStringValue_WrongTokenType_ShouldReturnNull()
        {
            var token = new Token(TokenType.Integer, new TextSpan(0, 2), "12", Enumerable.Empty<SyntaxTrivia>(), Enumerable.Empty<SyntaxTrivia>());

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
        [DataRow(@"'\u'")]
        [DataRow(@"'\u{'")]
        [DataRow(@"'\u}'")]
        [DataRow(@"'\u{}'")]
        [DataRow(@"'\u{110000}'")]
        [DataRow(@"'\u{FFFFFFFF}'")]
        [DataRow(@"'prefix\usufffix'")]
        [DataRow(@"'prefix\u{sufffix'")]
        [DataRow(@"'prefix\u}sufffix'")]
        [DataRow(@"'prefix\u{}sufffix'")]
        [DataRow(@"'prefix\u{110000}sufffix'")]
        [DataRow(@"'prefix\u{FFFFFFFF}sufffix'")]
        [DataRow(@"'prefix\u{FFFFFFFFsufffix'")]
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
        public void MissingCodesInDisableNextLineStatement_ShouldBeRecognizedWithError()
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow("#disable-next-line"), diagnosticWriter);
            lexer.Lex();

            var diagnostics = diagnosticWriter.GetDiagnostics();

            diagnostics.Should().SatisfyRespectively(
                x =>
                {
                    x.Level.Should().Be(DiagnosticLevel.Error);
                    x.Code.Should().Be("BCP226");
                    x.Message.Should().Be("Expected at least one diagnostic at this location. Valid format is #disable-next-line diagnosticCode1 diagnosticCode2 ...");
                });
        }

        [TestMethod]
        public void ValidDisableNextLineStatement_ShouldLexCorrectly()
        {
            string text = "#disable-next-line BCP226";
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            diagnosticWriter.GetDiagnostics().Should().BeEmpty();

            var tokens = lexer.GetTokens();
            tokens.Count().Should().Be(1);

            var leadingTrivia = tokens.First().LeadingTrivia;
            leadingTrivia.Count().Should().Be(1);

            var disableNextLineSyntaxTrivia = leadingTrivia.First();
            disableNextLineSyntaxTrivia.Type.Should().Be(SyntaxTriviaType.DisableNextLineStatement);
            disableNextLineSyntaxTrivia.Text.Should().Be(text);
        }

        [TestMethod]
        public void ValidDisableNextLineStatement_FollowedByComment_ShouldLexCorrectly()
        {
            string text = "#disable-next-line BCP226 // test";
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            diagnosticWriter.GetDiagnostics().Should().BeEmpty();

            var tokens = lexer.GetTokens();
            tokens.Count().Should().Be(1);

            var leadingTrivia = tokens.First().LeadingTrivia;
            leadingTrivia.Count().Should().Be(2);

            leadingTrivia.Should().SatisfyRespectively(
                x =>
                {
                    x.Text.Should().Be("#disable-next-line BCP226 ");
                    x.Type.Should().Be(SyntaxTriviaType.DisableNextLineStatement);
                },
                x =>
                {
                    x.Text.Should().Be("// test");
                    x.Type.Should().Be(SyntaxTriviaType.SingleLineComment);
                });
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
                "The specified escape sequence is not recognized. Only the following escape sequences are allowed: \"\\$\", \"\\'\", \"\\\\\", \"\\n\", \"\\r\", \"\\t\", \"\\u{...}\".",
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

        [DataRow(@"'hello\u{20}world\u{3F}'")]
        [DataRow(@"'\u{0}'")]
        [DataRow(@"'\u{00}'")]
        [DataRow(@"'\u{1}'")]
        [DataRow(@"'\u{4}'")]
        [DataRow(@"'\u{9}'")]
        [DataRow(@"'\u{A}'")]
        [DataRow(@"'\u{a}'")]
        [DataRow(@"'\u{F}'")]
        [DataRow(@"'\u{10}'")]
        [DataRow(@"'\u{1f}'")]
        [DataRow(@"'\u{FfFf}'")]
        [DataRow(@"'\u{10000}'")]
        [DataRow(@"'\u{10FFFF}'")]
        [DataTestMethod]
        public void CompleteStringsWithUnicodeEscapes_ShouldLexCorrectly(string text)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            diagnosticWriter.GetDiagnostics().Should().BeEmpty();

            lexer.GetTokens().Select(t => t.Type).Should().Equal(TokenType.StringComplete, TokenType.EndOfFile);
        }

        [DataRow(@"'\u'", @"\u")]
        [DataRow(@"'\u{'", @"\u{")]
        [DataRow(@"'\u{}'", @"\u{")]
        [DataRow(@"'\u}'", @"\u")]
        [DataRow(@"'\u{110000}'", @"\u{110000}")]
        [DataRow(@"'\u{10Z'", @"\u{10")]
        [DataTestMethod]
        public void InvalidUnicodeEscapes_ShouldProduceExpectedDiagnostic(string text, string expectedSpanText)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            lexer.GetTokens().Select(t => t.Type).Should().Equal(TokenType.StringComplete, TokenType.EndOfFile);
            var diagnostics = diagnosticWriter.GetDiagnostics().ToList();

            diagnostics.Should().HaveCount(1);
            var diagnostic = diagnostics.Single();

            diagnostic.Code.Should().Be("BCP133");
            diagnostic.Message.Should().Be(@"The unicode escape sequence is not valid. Valid unicode escape sequences range from \u{0} to \u{10FFFF}.");

            var spanText = text[new Range(diagnostic.Span.Position, diagnostic.Span.GetEndPosition())];
            spanText.Should().Be(expectedSpanText);
        }

        // empty
        [DataRow("''''''", "")]
        [DataRow("'''\r\n'''", "")]
        [DataRow("'''\n'''", "")]
        // basic
        [DataRow("'''abc'''", "abc")]
        // first preceding newline should be stripped
        [DataRow("'''\r\nabc'''", "abc")]
        [DataRow("'''\nabc'''", "abc")]
        [DataRow("'''\rabc'''", "abc")]
        // only the first should be stripped!
        [DataRow("'''\n\nabc'''", "\nabc")]
        [DataRow("'''\n\rabc'''", "\rabc")]
        // no escaping necessary
        [DataRow("''' \n \r \t \\ ' ${ } '''", " \n \r \t \\ ' ${ } ")]
        // leading and terminating ' characters
        [DataRow("''''a''''", "'a'")]
        [DataTestMethod]
        public void Multiline_strings_should_lex_correctly(string text, string expectedValue)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            lexer.GetTokens().Select(t => t.Type).Should().Equal(TokenType.MultilineString, TokenType.EndOfFile);

            var multilineToken = lexer.GetTokens().First();
            multilineToken.Text.Should().Be(text);
            Lexer.TryGetMultilineStringValue(multilineToken).Should().Be(expectedValue);
        }

        [DataRow("'''abc")]
        [DataRow("'''abc''")]
        [DataTestMethod]
        public void Unterminated_multiline_strings_should_attach_a_diagnostic(string text)
        {
            var diagnosticWriter = ToListDiagnosticWriter.Create();
            var lexer = new Lexer(new SlidingTextWindow(text), diagnosticWriter);
            lexer.Lex();

            lexer.GetTokens().Select(t => t.Type).Should().Equal(TokenType.MultilineString, TokenType.EndOfFile);
            var diagnostics = diagnosticWriter.GetDiagnostics().ToList();

            diagnostics.Should().HaveCount(1);
            var diagnostic = diagnostics.Single();

            diagnostic.Code.Should().Be("BCP140");
            diagnostic.Message.Should().Be($"The multi-line string at this location is not terminated. Terminate it with \"'''\".");
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

