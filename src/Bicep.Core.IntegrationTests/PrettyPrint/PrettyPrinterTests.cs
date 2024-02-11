// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Samples;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.IntegrationTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrinterTests
    {
        [NotNull]
        public TestContext? TestContext { get; set; }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void PrintProgram_AnyProgram_ShouldProduceExpectedOutput(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var formattedOutput = PrettyPrinter.PrintProgram(program, options, lexingErrorLookup, parsingErrorLookup);
            formattedOutput.Should().NotBeNull();

            var resultsFile = FileHelper.SaveResultFile(this.TestContext, Path.Combine(dataSet.Name, DataSet.TestFileMainFormatted), formattedOutput!);

            formattedOutput.Should().EqualWithLineByLineDiffOutput(
                TestContext,
                dataSet.Formatted,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainFormatted),
                actualLocation: resultsFile);
        }

        [DataTestMethod]
        [BaselineData_Bicepparam.TestData()]
        [TestCategory(BaselineHelper.BaselineTestCategory)]
        public void PrintProgram_ParamsFile_ShouldProduceExpectedOutput(BaselineData_Bicepparam baselineData)
        {
            var data = baselineData.GetData(TestContext);
            var program = ParserHelper.ParamsParse(data.Parameters.EmbeddedFile.Contents, out var lexingErrorLookup, out var parsingErrorLookup);
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var formattedOutput = PrettyPrinter.PrintProgram(program, options, lexingErrorLookup, parsingErrorLookup);
            formattedOutput.Should().NotBeNull();

            data.Formatted.WriteToOutputFolder(formattedOutput);
            data.Formatted.ShouldHaveExpectedValue();
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void PrintProgram_PrintTwice_ReturnsConsistentResults(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep, out var lexingErrorLookup, out var parsingErrorLookup);
            var syntaxErrors = lexingErrorLookup.Concat(parsingErrorLookup);
            var syntaxErrroMessages = syntaxErrors.Select(d => d.Message);

            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);
            var formattedOutput = PrettyPrinter.PrintProgram(program, options, lexingErrorLookup, parsingErrorLookup);
            var formattedProgram = ParserHelper.Parse(formattedOutput!, out var newLexingErrorLookup, out var newParsingErrorLookup);
            var newSyntaxErrors = newLexingErrorLookup.Concat(newParsingErrorLookup);
            var newSyntaxErrorMessages = newSyntaxErrors.Select(d => d.Message);

            // Diagnostic messages should remain the same after formatting.
            newSyntaxErrorMessages.Should().BeEquivalentTo(syntaxErrroMessages);

            // Normalize formatting
            var regex = new Regex("[\\r\\n\\s]+");
            string programText = regex.Replace(program.ToString(), "");
            string formattedProgramText = regex.Replace(formattedProgram.ToString(), "");

            formattedProgramText.Should().Be(programText);
        }

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();
    }
}
