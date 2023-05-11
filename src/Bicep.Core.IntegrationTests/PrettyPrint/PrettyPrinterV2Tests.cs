// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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
        public void Print_GivenWidth_ShouldOptimizeOutputAccordingly(int width)
        {
            var dataSet = DataSets.PrettyPrint_LF;
            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);
            var options = new PrettyPrinterV2Options(Width: width);
            var writer = new StringWriter();

            PrettyPrinterV2.PrintTo(writer, program, options, lexingErrorLookup, parsingErrorLookup);
            var output = writer.ToString();

            var outputFileName = $"main.pprint.w{width}.bicep";
            var outputFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, outputFileName), output);

            var expected = dataSet.ReadDataSetFile(outputFileName);

            output.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                expected,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, outputFileName),
                actualLocation: outputFile);
        }

        [DataTestMethod]
        [DataRow(40)]
        [DataRow(80)]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void Print_FormattedOutput_ProducesTheSameOutput(int width)
        {
            var dataSet = DataSets.PrettyPrint_LF;
            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);
            var options = new PrettyPrinterV2Options(Width: width);
            var writer = new StringWriter();

            PrettyPrinterV2.PrintTo(writer, program, options, lexingErrorLookup, parsingErrorLookup);
            var output = writer.ToString();

            program = ParserHelper.Parse(output, out lexingErrorLookup, out parsingErrorLookup);
            writer = new StringWriter();
            PrettyPrinterV2.PrintTo(writer, program, options, lexingErrorLookup, parsingErrorLookup);

            var newOutput = writer.ToString();

            newOutput.Should().Be(output);
        }
    }
}
