﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Bicep.Core.Parsing;
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
        public void PrintProgram_AnyProgram_ShouldProduceExpectedOutput(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep);
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);

            var formattedOutput = PrettyPrinter.PrintProgram(program, options);
            formattedOutput.Should().NotBeNull();

            var resultsFile = FileHelper.SaveResultFile(this.TestContext!, Path.Combine(dataSet.Name, DataSet.TestFileMainFormatted), formattedOutput!);

            formattedOutput.Should().EqualWithLineByLineDiffOutput(
                TestContext, 
                dataSet.Formatted,
                expectedLocation: DataSet.GetBaselineUpdatePath(dataSet, DataSet.TestFileMainFormatted),
                actualLocation: resultsFile);
        }

        [DataTestMethod]
        [DynamicData(nameof(GetData), DynamicDataSourceType.Method, DynamicDataDisplayNameDeclaringType = typeof(DataSet), DynamicDataDisplayName = nameof(DataSet.GetDisplayName))]
        public void PrintProgram_AnyProgram_ShouldRoundTrip(DataSet dataSet)
        {
            var program = ParserHelper.Parse(dataSet.Bicep);
            var diagnostics = program.GetParseDiagnostics();
            var diagnosticMessages = diagnostics.Select(d => d.Message);

            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, 2, true);
            var formattedOutput = PrettyPrinter.PrintProgram(program, options);
            var formattedProgram = ParserHelper.Parse(formattedOutput!);

            var newDiagnostics = formattedProgram.GetParseDiagnostics();
            var newDiagnosticMessages = newDiagnostics.Select(d => d.Message);

            // Diagnostic messages should remain the same after formatting.
            newDiagnostics.Should().HaveSameCount(diagnostics);
            newDiagnosticMessages.Should().BeEquivalentTo(diagnosticMessages);

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

        private static IEnumerable<object[]> GetData() => DataSets.AllDataSets.ToDynamicTestData();
    }
}
