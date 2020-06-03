using System.Text;
using FluentAssertions;
using Bicep.Core.Parser;
using Bicep.Core.Tests.UnitSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Tests
{
    [TestClass]
    public class ParserTests
    {
        [DataTestMethod]
        [UnitSamplesDataSource]
        public void RoundTripTest(string displayName, string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);
            
            var program = parser.Parse();

            var buffer = new StringBuilder();
            var visitor = new PrintVisitor(buffer);

            visitor.Visit(program);

            buffer.ToString().Should().Be(contents);
        }
    }
}
