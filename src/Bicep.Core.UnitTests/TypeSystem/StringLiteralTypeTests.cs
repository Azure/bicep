// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.TypeSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.TypeSystem
{
    [TestClass]
    public class StringLiteralTypeTests
    {
        [TestMethod]
        public void StringLiteral_name_should_be_correctly_formatted()
        {
            var literal = new StringLiteralType("a'b\\c$d\re\nf\tg${h");

            literal.Name.Should().StartWith("'", "literal type name should be quoted");
            literal.Name.Should().EndWith("'", "literal type name should be quoted");
            literal.Name.Should().Be("'a\\'b\\\\c$d\\re\\nf\\tg\\${h'", "literal type name should be escaped");
        }
    }
}