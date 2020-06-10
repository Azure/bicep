using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Bicep.Core.Parser;
using Bicep.Core.Syntax;
using Bicep.Core.Tests.UnitSamples;
using Bicep.Core.Tests.Utils;
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
            var program = ParserHelper.Parse(contents);
            program.Should().BeOfType<ProgramSyntax>();

            var buffer = new StringBuilder();
            var visitor = new PrintVisitor(buffer);

            visitor.Visit(program);

            buffer.ToString().Should().Be(contents);
        }
    }
}
