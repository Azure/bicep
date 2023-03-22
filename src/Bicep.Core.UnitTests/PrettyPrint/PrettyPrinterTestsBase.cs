// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Utils;
using FluentAssertions;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    public class PrettyPrinterTestsBase
    {
        protected static PrettyPrintOptions CommonOptions { get; } = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);

        protected void TestPrintProgram(string programText, string expectedOutput, PrettyPrintOptions? options = null)
        {
            ProgramSyntax? programSyntax = ParserHelper.Parse(programText, out var lexingErrorLookup, out var parsingErrorLookup);
            options ??= CommonOptions;

            string output = PrettyPrinter.PrintProgram(programSyntax, options, lexingErrorLookup, parsingErrorLookup);

            output.Should().Be(expectedOutput);

            programSyntax = ParserHelper.Parse(expectedOutput, out lexingErrorLookup, out parsingErrorLookup);
            output = PrettyPrinter.PrintProgram(programSyntax, options, lexingErrorLookup, parsingErrorLookup);

            // The formatter should produce consistent results.
            output.Should().Be(expectedOutput);
        }
    }
}
