using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Visitors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CheckVisitorTests
    {
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ProgramsShouldProduceExpectedErrors(DataSet dataSet)
        {
            var errors = GetErrors(dataSet.Bicep);

            var buffer = new StringBuilder();
            new ErrorWriter(buffer).WriteErrors(errors.OrderBy(s => s.Message));

            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Actual.err", buffer.ToString());

            buffer.ToString().Should().Be(dataSet.Errors);
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.Select(ds => new object[] { ds });

        private static List<Error> GetErrors(string contents)
        {
            var program = ParserHelper.Parse(contents);
            program.Should().BeOfType<ProgramSyntax>();

            var errors = new List<Error>();
            var typeChecker = new CheckVisitor(errors);

            typeChecker.Visit(program);

            return errors;
        }
    }
}
