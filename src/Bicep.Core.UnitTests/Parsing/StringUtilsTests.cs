// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Parsing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Parsing;

[TestClass]
public class StringUtilsTests
{
    [TestMethod]
    [DataRow("\nhello\r\nthere", "\nhello\nthere")]
    [DataRow("\n  hello\r\n  there", "\nhello\nthere")]
    [DataRow("\n  hello\n there", "\n hello\nthere")]
    [DataRow("\n    hello\n\n\n    there\n    ", "\nhello\n\n\nthere\n")]
    [DataRow("hello\n    there\n    ", "hello\nthere\n")]
    public void NormalizeIndent_strips_leading_whitespace(string input, string expectedOutput)
    {
        var output = StringUtils.NormalizeIndent(input);
        output.Should().Be(expectedOutput);
    }
}
