// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using Bicep.Core.Syntax;

namespace Bicep.Core.UnitTests.Utils
{
    public static class PrintHelper
    {
        private static PrettyPrintOptions DefaultOptions { get; } = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);

        public static string PrettyPrint(ProgramSyntax programSyntax)
        {
            return PrettyPrinter.PrintProgram(programSyntax, DefaultOptions);
        }
    }
}