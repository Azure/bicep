// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.SourceLink;
using Bicep.Core.Text;
using Bicep.Core.UnitTests.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SourceCode;

[TestClass]
public class ArchivedSourceFileLinkTests
{
    [TestMethod]
    public void SerializesAndDeserializes()
    {
        var link = new ArchivedSourceFileLink(
            new TextRange(123, 456, 234, 567),
            "../modules/target.bicep");
        string serialized = JsonSerializer.Serialize(link);

        serialized.Should().Be("{\"Range\":\"[123:456]-[234:567]\",\"Target\":\"../modules/target.bicep\"}");

        var deserialized = JsonSerializer.Deserialize<ArchivedSourceFileLink>(serialized);
        deserialized.Should().Be(link);
    }
}
