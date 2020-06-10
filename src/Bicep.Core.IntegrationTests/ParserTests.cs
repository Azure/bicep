using System.Text;
using Bicep.Core.IntegrationTests.UnitSamples;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
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
