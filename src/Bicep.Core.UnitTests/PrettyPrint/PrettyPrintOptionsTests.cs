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
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(-100)]
        [DataRow(-10000)]
        public void PrettyPrintOptions_SetIndentSizeSmallerThanOne_ShouldSetToOne(long indentSize)
        {
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, indentSize, false);

            options.IndentSize.Should().Be(1);
        }

        [DataTestMethod]
        [DataRow(1001)]
        [DataRow(1002)]
        [DataRow(10000)]
        [DataRow(1000000)]
        public void PrettyPrintOptions_SetIndentSizeGreaterThanOneThousand_ShouldSetToOneThousand(long indentSize)
        {
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, indentSize, false);

            options.IndentSize.Should().Be(1000);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(10)]
        [DataRow(100)]
        [DataRow(1000)]
        public void PrettyPrintOptions_SetIndentSizeBetweenOneAndOneThousand_ShouldSetToSpecifiedValue(long indentSize)
        {
            var options = new PrettyPrintOptions(NewlineOption.Auto, IndentKindOption.Space, indentSize, false);

            options.IndentSize.Should().Be((int)indentSize);
        }
    }
}
