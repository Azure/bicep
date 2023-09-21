// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.PrettyPrintV2;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.PrettyPrintV2
{
    [TestClass]
    public class PrettyPrinterV2OptionsTests
    {
        [DataTestMethod]
        [DataRow(-10, 0)]
        [DataRow(-1, 0)]
        [DataRow(20, 20)]
        [DataRow(1001, 1000)]
        [DataRow(3000, 1000)]
        public void Create_VariousIndentSizes_NormalizesSizes(int indentSize, int expectedIndentSize)
        {
            var options = new PrettyPrinterV2Options(IndentSize: indentSize);

            options.IndentSize.Should().BeGreaterThanOrEqualTo(expectedIndentSize);

            options = options with { IndentSize = indentSize };

            options.IndentSize.Should().Be(expectedIndentSize);
        }

        [DataTestMethod]
        [DataRow(-10, 0)]
        [DataRow(-1, 0)]
        [DataRow(0, 0)]
        [DataRow(20, 20)]
        [DataRow(100, 100)]
        public void Create_VariousWidths_NormalizesWidths(int width, int expectedWidth)
        {
            var options = new PrettyPrinterV2Options(Width: width);

            options.Width.Should().Be(expectedWidth);

            options = options with { Width = width };

            options.Width.Should().Be(expectedWidth);
        }
    }
}
