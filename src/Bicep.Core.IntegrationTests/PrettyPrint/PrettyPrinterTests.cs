// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using Bicep.Core.Parser;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Samples;
using Bicep.Core.Syntax;
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
        public void PrintProgram_ProgramWithoutDiagnostics_ShouldProduceExpectedOutput(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep);
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var formattedOutput = PrettyPrinter.PrintProgram(program, options);
            formattedOutput.Should().NotBeNull();

            var resultsFile = FileHelper.SaveResultFile(this.TestContext!, Path.Combine(dataSet.Name, DataSet.TestFileMainFormatted), formattedOutput!);

            formattedOutput.Should().EqualWithLineByLineDiffOutput(
                formattedOutput!,
                expectedLocation: OutputHelper.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainFormatted),
                actualLocation: resultsFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void PrintProgram_ProgramWithoutDiagnostics_ShouldRoundTrip(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep);
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var formattedOutput = PrettyPrinter.PrintProgram(program, options);
            formattedOutput.Should().NotBeNull();

            // The program should still be parsed without any errors after formatting.
            var formattedProgram = ParserHelper.Parse(formattedOutput!);
            formattedProgram.GetParseDiagnostics().Should().BeEmpty();

            var buffer = new StringBuilder();
            var printVisitor = new PrintVisitor(buffer,x =>
                // Remove newlines and whitespaces.
                (x is Token token && token.Type == TokenType.NewLine) ||
                (x is SyntaxTrivia trivia && trivia.Type == SyntaxTriviaType.Whitespace));

            printVisitor.Visit(program);
            string programText = buffer.ToString();

            buffer.Clear();
            printVisitor.Visit(program);
            string formattedProgramText = buffer.ToString();

            formattedProgramText.Should().Be(programText);
        }

        private static IEnumerable<object[]> GetData() => DataSets.DataSetsWithNoDiagnostics.ToDynamicTestData();
    }
}
