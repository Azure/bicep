// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceCodePositionTests
{
    public TestContext? TestContext { get; set; }

    [DataTestMethod]
    [DataRow(123, 456, "\"[123:456]\"")]
    [DataRow(0, 1, "\"[0:1]\"")]
    [DataRow(0, 0, "\"[0:0]\"")]
    public void Serialization(int x, int y, string expectedSerialization)
    {
        var position = new SourceCodePosition(x, y);
        string serialized = JsonSerializer.Serialize(position);

        serialized.Should().Be(expectedSerialization);

        var deserialized = JsonSerializer.Deserialize<SourceCodePosition>(serialized);
        deserialized.Should().Be(position);
    }

    [DataTestMethod]
    [DataRow(-1, 0, "Line must be non-negative")]
    [DataRow(0, -1, "Column must be non-negative")]
    public void BadValues(int x, int y, string expectedMessage)
    {
        ((Action)(() => new SourceCodePosition(x, y))).Should().Throw<ArgumentException>().WithMessage(expectedMessage);
    }

    [DataTestMethod]
    [DataRow("\"[0:0]\"", 0, 0, null)]
    [DataRow("\"[0,0]\"", null, null, "Invalid input format for deserialization of SourceCodePosition")]
    [DataRow("\":0:0]\"", null, null, "Invalid input format for deserialization of SourceCodePosition")]
    [DataRow("\"[-1:0]\"", null, null, "Line must be non-negative")]
    public void Deserialization(string input, int? expectedX, int? expectedY, string? expectedMessage)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<SourceCodePosition>(input);
            expectedMessage.Should().BeNull();

            expectedX.Should().NotBeNull();
            expectedY.Should().NotBeNull();
            deserialized.Should().Be(new SourceCodePosition(expectedX!.Value, expectedY!.Value));
        }
        catch (Exception ex)
        {
            ex.Message.Should().Be(expectedMessage);
        }
    }
}
