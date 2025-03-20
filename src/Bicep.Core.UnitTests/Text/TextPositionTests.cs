// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Text;

[TestClass]
public class TextPositionTests
{
    public TestContext? TestContext { get; set; }

    [DataTestMethod]
    [DataRow(123, 456, "\"[123:456]\"")]
    [DataRow(0, 1, "\"[0:1]\"")]
    [DataRow(0, 0, "\"[0:0]\"")]
    public void Serialization(int x, int y, string expectedSerialization)
    {
        var position = new TextPosition(x, y);
        string serialized = JsonSerializer.Serialize(position);

        serialized.Should().Be(expectedSerialization);

        var deserialized = JsonSerializer.Deserialize<TextPosition>(serialized);
        deserialized.Should().Be(position);
    }

    [DataTestMethod]
    [DataRow(-1, 0)]
    [DataRow(0, -1)]
    public void BadValues(int line, int character)
    {
        ((Action)(() => new TextPosition(line, character))).Should().Throw<ArgumentOutOfRangeException>();
    }

    [DataTestMethod]
    [DataRow("\"[0:0]\"", 0, 0, null)]
    [DataRow("\"[0,0]\"", null, null, "Invalid input format for deserialization of TextPosition")]
    [DataRow("\":0:0]\"", null, null, "Invalid input format for deserialization of TextPosition")]
    public void Deserialization(string input, int? expectedX, int? expectedY, string? expectedMessage)
    {
        try
        {
            var deserialized = JsonSerializer.Deserialize<TextPosition>(input);
            expectedMessage.Should().BeNull();

            expectedX.Should().NotBeNull();
            expectedY.Should().NotBeNull();
            deserialized.Should().Be(new TextPosition(expectedX!.Value, expectedY!.Value));
        }
        catch (Exception ex)
        {
            ex.Message.Should().Be(expectedMessage);
        }
    }
}
