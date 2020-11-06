// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.PrettyPrint;
using Bicep.Core.PrettyPrint.Options;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrint
{
    [TestClass]
    public class PrettyPrintOptionsTests
    {
        [DataTestMethod]
        [DataRow(-100000)]
        [DataRow(-100)]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(4)]
        [DataRow(100)]
        [DataRow(1000)]
        [DataRow(2000)]
        [DataRow(1000000)]
        public void PrettyPrintOptions_SetIndentSize_ShouldEnsureValueDoesNotExceedLimit(long indentSize)
        {
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, indentSize, false);

            options.IndentSize.Should().BeGreaterOrEqualTo(1);
            options.IndentSize.Should().BeLessOrEqualTo(1000);
        }
    }
}
