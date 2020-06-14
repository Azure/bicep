using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class ParserTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void FilesShouldRoundTripSuccessfully(DataSet dataSet)
        {
            RunRoundTripTest(dataSet.Bicep);
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

        private static IEnumerable<object[]> GetData()
        {
            return DataSets.AllDataSets.Select(ds => new object[] {ds});
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
