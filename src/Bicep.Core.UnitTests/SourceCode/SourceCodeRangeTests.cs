// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceCodeRangeTests
{
    public TestContext? TestContext { get; set; }

    [TestMethod]
    public void SerializesAndDeserializes()
    {
        var range = new SourceCodeRange(new SourceCodePosition(123, 456), new SourceCodePosition(234, 567));
        string serialized = JsonSerializer.Serialize(range);

        serialized.Should().Be("\"[123:456]-[234:567]\"");

        var deserialized = JsonSerializer.Deserialize<SourceCodeRange>(serialized);
        deserialized.Should().Be(range);
    }
}
