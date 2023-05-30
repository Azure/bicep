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
        public void Print_DifferentWidth_ShouldOptimizeLayoutAccordingly(int width)
        {
            var dataSet = DataSets.PrettyPrint_LF;
            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);

            var writer = new StringWriter();
            var context = PrettyPrinterV2Context.Create(program, new(Width: width), lexingErrorLookup, parsingErrorLookup);

            PrettyPrinterV2.PrintTo(writer, context);
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
            var options = new PrettyPrinterV2Options(Width: width);

            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);

            var writer = new StringWriter();
            var context = PrettyPrinterV2Context.Create(program, options, lexingErrorLookup, parsingErrorLookup);

            PrettyPrinterV2.PrintTo(writer, context);
            var output = writer.ToString();

            writer = new StringWriter();
            program = ParserHelper.Parse(output, out lexingErrorLookup, out parsingErrorLookup);
            context = PrettyPrinterV2Context.Create(program, options, lexingErrorLookup, parsingErrorLookup);

            PrettyPrinterV2.PrintTo(writer, context);

            var newOutput = writer.ToString();

            newOutput.Should().Be(output);
        }
    }
}
