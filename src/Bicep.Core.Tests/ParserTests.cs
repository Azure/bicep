using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.Tests.UnitSamples;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.Tests
{
    [TestClass]
    public class ParserTests
    {
        [DataTestMethod]
        [UnitSamplesDataSource]
        public void Files_ShouldRoundTripSuccessfully(string displayName, string contents)
        {
            RunRoundTripTest(contents);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("parameter")]
        [DataRow("parameter ")]
        [DataRow("parameter foo")]
        [DataRow("parameter foo bar")]
        [DataRow("parameter foo bar =")]
        [DataRow("parameter foo bar = 1")]
        public void Oneliners_ShouldRoundTripSuccessfully(string contents)
        {
            RunRoundTripTest(contents);
        }

        private static void RunRoundTripTest(string contents)
        {
            var program = Parse(contents);

            var buffer = new StringBuilder();
            var visitor = new PrintVisitor(buffer);

            visitor.Visit(program);

            buffer.ToString().Should().Be(contents);
        }


        private static SyntaxBase Parse(string contents)
        {
            var lexer = new Parser.Lexer(new SlidingTextWindow(contents));
            lexer.Lex();

            var tokens = lexer.GetTokens();
            var parser = new Parser.Parser(tokens);

            var program = parser.Parse();

            var errors = new List<Error>();
            var collector = new ParseErrorCollector(errors);

            collector.Visit(program);

            return program;
        }
    }
}
