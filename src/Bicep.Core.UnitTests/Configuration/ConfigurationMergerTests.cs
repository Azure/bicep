// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Json;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Configuration;

[TestClass]
public class ConfigurationMergerTests
{
    private static readonly IOUri BaseUri = TestFileUri.FromInMemoryPath("base/bicepconfig.json");
    private static readonly IOUri LeafUri = TestFileUri.FromInMemoryPath("bicepconfig.json");

    private static MergedConfiguration Merge(string builtIn, string baseContents, string leafContents) =>
        ConfigurationMerger.Merge(
            JsonElementFactory.CreateElement(builtIn),
            [new(LeafUri, JsonElementFactory.CreateElement(leafContents)), new(BaseUri, JsonElementFactory.CreateElement(baseContents))]);

    [TestMethod]
    [DataRow("objects merge deeply",
        """{ "a": { "x": 1, "y": 1 } }""", """{ "a": { "y": 2, "z": 2 } }""", """{ "a": { "z": 3 } }""",
        """{"a":{"x":1,"y":2,"z":3}}""")]
    [DataRow("arrays are replaced",
        """{ "a": [1, 2] }""", """{ "a": [3] }""", """{}""",
        """{"a":[3]}""")]
    [DataRow("values are replaced, whatever their type",
        """{ "a": { "x": 1 }, "b": 1 }""", """{ "a": 5 }""", """{ "b": { "y": 2 } }""",
        """{"a":5,"b":{"y":2}}""")]
    [DataRow("null is ignored, so the inherited value stays",
        """{ "a": 1, "b": { "x": 1 } }""", """{ "a": 2 }""", """{ "a": null, "b": null, "c": { "x": null, "y": 1 } }""",
        """{"a":2,"b":{"x":1},"c":{"y":1}}""")]
    [DataRow("extends and $schema are not merged",
        """{}""", """{ "$schema": "s", "extends": "x" }""", """{ "extends": "./base/bicepconfig.json", "a": { "extends": 1 } }""",
        """{"a":{"extends":1}}""")]
    public void Merge_AppliesMergeRules(string description, string builtIn, string baseContents, string leafContents, string expected)
    {
        var merged = Merge(builtIn, baseContents, leafContents);

        merged.Root.GetRawText().Should().Be(expected, description);
    }

    [TestMethod]
    [DataRow("/a", null)]
    [DataRow("/b", "base")]
    [DataRow("/b/x", "base")]
    [DataRow("/c/x", null)]
    [DataRow("/c/y", "base")]
    [DataRow("/c/z", "leaf")]
    [DataRow("/d", "leaf")]
    [DataRow("/d/x", "leaf")]
    [DataRow("/e/0/x", "base")]
    [DataRow("/f", "leaf")]
    [DataRow("/f/x", "leaf")]
    [DataRow("/g.h", "base")]
    [DataRow("/g.h/x", "base")]
    [DataRow("/g/h", null)]
    public void Merge_RecordsTheFileThatDeclaredEachValue(string path, string? expectedFile)
    {
        var merged = Merge(
            builtIn: """{ "a": 1, "c": { "x": 1 }, "f": { "x": 1 } }""",
            baseContents: """{ "b": { "x": 1 }, "c": { "y": 1 }, "d": { "x": 1 }, "e": [{ "x": 1 }], "f": { "x": 2 }, "g.h": { "x": 1 } }""",
            leafContents: """{ "c": { "z": 1 }, "d": 5, "f": 3 }""");

        var expectedUri = expectedFile switch
        {
            "base" => BaseUri,
            "leaf" => LeafUri,
            _ => null,
        };

        // Property names are separated by "/" here, so that a name can contain a dot.
        merged.Provenance.GetDeclaringFile(ConfigurationPropertyPath.Create(path.Split('/', StringSplitOptions.RemoveEmptyEntries))).Should().Be(expectedUri);
    }
}
