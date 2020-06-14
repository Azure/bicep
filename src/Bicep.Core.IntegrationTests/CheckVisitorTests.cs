using System.Collections.Generic;
using System.Linq;
using Bicep.Core.IntegrationTests.Utils;
using Bicep.Core.Parser;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
using Bicep.Core.Visitors;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests
{
    [TestClass]
    public class CheckVisitorTests
    {
        [DataTestMethod]
        [DynamicData(nameof(GetValidData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void ValidProgram_ShouldProduceNoErrors(DataSet dataSet)
        {
            GetErrors(dataSet.Bicep).Should().BeEmpty();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetInvalidData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void InvalidProgram_ShouldProduceErrors(DataSet dataSet)
        {
            var errors = GetErrors(dataSet.Bicep);
            errors.Should().NotBeEmpty();
        }

        private static IEnumerable<object[]> GetValidData() => DataSets.AllDataSets.Where(ds=>ds.IsValid).Select(ds => new object[] { ds });

        private static IEnumerable<object[]> GetInvalidData() => DataSets.AllDataSets.Where(ds => ds.IsValid == false).Select(ds => new object[] {ds});

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
