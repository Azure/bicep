// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.SourceLink;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Text;

[TestClass]
public class TextRangeTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void SerializesAndDeserializes()
    {
        var range = new TextRange(new TextPosition(123, 456), new TextPosition(234, 567));
        string serialized = JsonSerializer.Serialize(range);

        serialized.Should().Be("\"[123:456]-[234:567]\"");

        var deserialized = JsonSerializer.Deserialize<TextRange>(serialized);
        deserialized.Should().Be(range);
    }
}
