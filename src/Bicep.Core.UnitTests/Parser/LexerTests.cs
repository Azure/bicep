using System;
using Bicep.Core.Parser;
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
            var token = new Token(TokenType.String, new TextSpan(0, literalText.Length), literalText, string.Empty, string.Empty);

            var actual = Lexer.GetStringValue(token);

            actual.Should().Be(expectedValue);
        }

        [TestMethod]
        public void GetStringValue_WrongTokenType_ShouldThrow()
        {
            var token = new Token(TokenType.Number, new TextSpan(0, 2), "12", string.Empty, string.Empty);

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
            var token = new Token(TokenType.String, new TextSpan(0, literalText.Length), literalText, string.Empty, string.Empty);

            Action invalidLiteral = () => Lexer.GetStringValue(token);
            invalidLiteral.Should().Throw<ArgumentException>().WithMessage(expectedExceptionMessage);
        }
    }
}
