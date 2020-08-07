using System;
using System.Collections.Immutable;
using System.Linq;
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
        [DataRow(@"'", "")]
        [DataRow(@"''", "")]
        [DataRow(@"''", "")]
        [DataRow(@"'test", "test")]
        [DataRow(@"'test'", "test")]
        [DataRow(@"'hello there'", "hello there")]
        [DataRow(@"'\r\n\t\\\$\''", "\r\n\t\\$'")]
        [DataRow(@"'hi\", "hi")]
        [DataRow("'First line\\nSecond\\ttabbed\\tline'", "First line\nSecond\ttabbed\tline")]
        public void GetStringValue_ValidStringLiteralToken_ShouldCalculateValueCorrectly(string literalText, string expectedValue)
        {
            var token = new Token(TokenType.String, new TextSpan(0, literalText.Length), literalText, ImmutableArray.Create<SyntaxTrivia>(), ImmutableArray.Create<SyntaxTrivia>());

            var actual = Lexer.GetStringValue(token);

            actual.Should().Be(expectedValue);
        }

        [TestMethod]
        public void GetStringValue_WrongTokenType_ShouldThrow()
        {
            var token = new Token(TokenType.Number, new TextSpan(0, 2), "12", ImmutableArray.Create<SyntaxTrivia>(), ImmutableArray.Create<SyntaxTrivia>());

            Action wrongType = () => Lexer.GetStringValue(token);
            wrongType.Should().Throw<ArgumentException>().WithMessage("The specified token must be of type 'String' but is of type 'Number'.");
        }

        [DataTestMethod]
        [DataRow(@"", "The text of the specified token must start with a single quote. Text = ")]
        [DataRow(@"hi", "The text of the specified token must start with a single quote. Text = hi")]
        [DataRow(@"'hello'there", "String token must not contain additional characters after the string-terminating single quote. Text = 'hello'there")]
        [DataRow(@"'test\!'", "String token contains an invalid escape character. Text = 'test\\!'")]
        public void GetStringValue_InvalidStringLiteralToken_ShouldThrow(string literalText, string expectedExceptionMessage)
        {
            var token = new Token(TokenType.String, new TextSpan(0, literalText.Length), literalText, ImmutableArray.Create<SyntaxTrivia>(), ImmutableArray.Create<SyntaxTrivia>());

            Action invalidLiteral = () => Lexer.GetStringValue(token);
            invalidLiteral.Should().Throw<ArgumentException>().WithMessage(expectedExceptionMessage);
        }

        [TestMethod]
        public void UnrecognizedTokens_ShouldNotBeRecognized()
        {
            RunSingleTokenTest("^", TokenType.Unrecognized, "The following token is not recognized: '^'.", "BCP001");
        }

        [TestMethod]
        public void UnterminatedMultiLineComment_ShouldBeRecognizedWithError()
        {
            const string expectedTokenText = "'test'";
            RunSingleTokenTest(
                "'test'/* unfinished comment *",
                TokenType.String,
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.",
                "BCP002",
                expectedTokenText: expectedTokenText,
                expectedStartPosition: expectedTokenText.Length);

            RunSingleTokenTest(
                "'test'/* unfinished comment",
                TokenType.String,
                "The multi-line comment at this location is not terminated. Terminate it with the */ character sequence.",
                "BCP002",
                expectedTokenText: expectedTokenText,
                expectedStartPosition: expectedTokenText.Length);
        }

        [TestMethod]
        public void UnterminatedString_ShouldBeRecognizedWithError()
        {
            RunSingleTokenTest("'string does not end", TokenType.String, "The string at this location is not terminated. Terminate the string with a single quote character.", "BCP003");
            RunSingleTokenTest("'beginning an escape\\", TokenType.String, "The string at this location is not terminated. Complete the escape sequence and terminate the string with a single unescaped quote character.", "BCP005");
        }

        [TestMethod]
        public void UnrecognizedEscapeSequence_ShouldBeRecognizedWithError()
        {
            RunSingleTokenTest(
                "'bad \\escape'",
                TokenType.String,
                "The specified escape sequence is not recognized. Only the following characters can be escaped with a backslash: \\$, \\', \\\\, \\n, \\r, \\t.",
                "BCP006",
                expectedStartPosition: 5,
                expectedLength: 2);
        }

        [TestMethod]
        public void UnterminatedStringUnexpectedNewline_ShouldBeRecognizedWithError()
        {
            var text = "'unfinished\n'even more unfinished\r\n'test'";
            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            var tokens = lexer.GetTokens().ToImmutableArray();
            tokens.Should().HaveCount(6);

            tokens.Select(t => t.Type).Should().Equal(TokenType.String, TokenType.NewLine, TokenType.String, TokenType.NewLine, TokenType.String, TokenType.EndOfFile);

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

        private static void RunSingleTokenTest(string text, TokenType expectedTokenType, string expectedMessage, string expectedCode, int expectedStartPosition = 0, int? expectedLength = null, string? expectedTokenText = null)
        {
            expectedTokenText ??= text;
            expectedLength ??= text.Length - expectedStartPosition;

            var lexer = new Lexer(new SlidingTextWindow(text));
            lexer.Lex();

            var tokens = lexer.GetTokens().ToImmutableArray();
            tokens.Should().HaveCount(2);
            tokens.Select(t => t.Type).Should().Equal(expectedTokenType, TokenType.EndOfFile);

            tokens.First().Text.Should().Be(expectedTokenText);

            var errors = lexer.GetErrors().ToImmutableArray();
            errors.Should().HaveCount(1);

            var error = errors.Single();
            error.Message.Should().Be(expectedMessage);
            error.Code.Should().Be(expectedCode);
            error.Span.Position.Should().Be(expectedStartPosition);
            error.Span.Length.Should().Be(expectedLength);
        }
    }
}
