// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.SourceCode;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class SourceCodeDocumentPathLinkTests
{
    [TestMethod]
    public void SerializesAndDeserializes()
    {
        var link = new SourceCodeDocumentPathLink(
            new SourceCodeRange(123, 456, 234, 567),
            "../modules/target.bicep");
        string serialized = JsonSerializer.Serialize(link);

        serialized.Should().Be("{\"Range\":\"[123:456]-[234:567]\",\"Target\":\"../modules/target.bicep\"}");

        var deserialized = JsonSerializer.Deserialize<SourceCodeDocumentPathLink>(serialized);
        deserialized.Should().Be(link);
    }
}
