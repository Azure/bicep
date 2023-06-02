// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core.Diagnostics;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.Core.IntegrationTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrinterV2Tests
    {
        public TestContext TestContext { get; set; } = null!;

        [DataTestMethod]
        [DataRow(40)]
        [DataRow(80)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Print_VariousWidths_OptimizesLayoutAccordingly(int width)
        {
            var dataSet = DataSets.PrettyPrint_LF;
            var options = new PrettyPrinterV2Options(Width: width);

            var output = Print(dataSet.Bicep, options);

            var outputFileName = $"main.pprint.w{width}.bicep";
            var outputFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, outputFileName), output);
            var expected = dataSet.ReadDataSetFile(outputFileName);

            output.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                expected,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, outputFileName),
                actualLocation: outputFile);

            AssertConsistentOutput(output, options);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void Print_DataSet_ProducesExpectedOutput(DataSet dataSet)
        {
            var output = Print(dataSet.Bicep, PrettyPrinterV2Options.Default);
            var outputFileName = $"main.pprint.bicep";
            var outputFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, outputFileName), output);

            var expected = dataSet.ReadDataSetFile(outputFileName);

            output.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                expected,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, outputFileName),
                actualLocation: outputFile);

            AssertConsistentOutput(output, PrettyPrinterV2Options.Default);
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Print_ParamDataSet_ProducesExpectedOutput(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var output = Print(data.Parameters.EmbeddedFile.Contents, PrettyPrinterV2Options.Default, isParamFile: true);

            data.PrettyPrinted.WriteToOutputFolder(output);
            data.PrettyPrinted.ShouldHaveExpectedValue();

            AssertConsistentParamsOutput(output, PrettyPrinterV2Options.Default);
        }

        private static void AssertConsistentOutput(string formatted, PrettyPrinterV2Options options) =>
            Print(formatted, options).Should().Be(formatted);

        private static void AssertConsistentParamsOutput(string formatted, PrettyPrinterV2Options options) =>
            Print(formatted, options, isParamFile: true).Should().Be(formatted);

        private static string Print(string programText, PrettyPrinterV2Options options, bool isParamFile = false)
        {
            var program = isParamFile
                ? ParserHelper.ParamsParse(programText, out var lexingErrorLookup, out var parsingErrorLookup)
                : ParserHelper.Parse(programText, out lexingErrorLookup, out parsingErrorLookup);

            var context = PrettyPrinterV2Context.Create(program, options, lexingErrorLookup, parsingErrorLookup);

            return PrettyPrinterV2.Print(context);
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets
            .Where(x => !x.Name.Equals(DataSets.PrettyPrint_LF.Name, StringComparison.Ordinal))
            .ToDynamicTestData();
    }
}
