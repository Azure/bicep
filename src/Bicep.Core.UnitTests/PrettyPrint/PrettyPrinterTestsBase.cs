// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    public class PrettyPrinterTestsBase
    {
        protected static PrettyPrintOptions CommonOptions { get; } = new PrettyPrintOptions(
            NewlineOption.Auto,
            IndentKindOption.Space,
            2,
            false);
    }
}
