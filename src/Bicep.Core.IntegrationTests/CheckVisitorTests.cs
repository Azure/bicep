using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Json;
using Bicep.Core.UnitTests.Serialization;
using Bicep.Core.UnitTests.Utils;
using Bicep.Core.Visitors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
            var errors = GetErrors(dataSet.Bicep).Select(error => new ErrorItem(error, dataSet.Bicep));

            var actual = JToken.FromObject(errors, DataSetSerialization.CreateSerializer());
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Errors_Actual.json", actual.ToString(Formatting.Indented));

            var expected = JToken.Parse(dataSet.Errors);
            FileHelper.SaveResultFile(this.TestContext!, $"{dataSet.Name}_Errors_Expected.json", expected.ToString(Formatting.Indented));
            JsonAssert.AreEqual(expected, actual, this.TestContext!, $"{dataSet.Name}_Errors_Delta.json");
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();

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
