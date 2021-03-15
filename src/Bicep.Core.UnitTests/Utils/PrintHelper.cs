// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.UnitTests.Utils
{
    public static class PrintHelper
    {
        private static PrettyPrintOptions DefaultOptions { get; } = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);

        public static string PrintAndCheckForParseErrors(ProgramSyntax programSyntax)
        {
            var asString = PrettyPrinter.PrintProgram(programSyntax, DefaultOptions);

            var parsed = ParserHelper.Parse(asString);
            parsed.GetParseDiagnostics().Should().BeEmpty();

            return asString;
        }
    }
}