// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2;
using Bicep.Core.Syntax;

namespace Bicep.Testing;

public static class TestPrinter
{
    public static string Print(ProgramSyntax programSyntax) =>
        PrettyPrinterV2.PrintValid(programSyntax, PrettyPrinterV2Options.Default);
}