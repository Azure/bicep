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
public class ConfigurationFileReaderTests
{
    private static ResultWithDiagnostic<ConfigurationFile> Read(string contents)
    {
        var fileSet = InMemoryTestFileSet.Create(("bicepconfig.json", contents));

        return ConfigurationFileReader.Read(fileSet.FileExplorer.GetFile(fileSet.GetUri("bicepconfig.json")));
    }

    [TestMethod]
    public void Read_WithComments_Succeeds()
    {
        var file = Read("""
            {
              // A comment.
              "a": /* another */ 1
            }
            """).Unwrap();

        file.Root.GetProperty("a").GetInt32().Should().Be(1);
    }

    [TestMethod]
    [DataRow("""{ "a": 1, }""", "BCP271")]
    [DataRow("not json", "BCP271")]
    [DataRow("[]", "BCP273")]
    public void Read_InvalidContents_ReturnsError(string contents, string expectedCode)
    {
        Read(contents).IsSuccess(out _, out var error).Should().BeFalse();

        error!.Code.Should().Be(expectedCode);
    }

    [TestMethod]
    public void Read_UnreadableFile_ReturnsError()
    {
        var fileSet = InMemoryTestFileSet.Create();

        ConfigurationFileReader.Read(fileSet.FileExplorer.GetFile(fileSet.GetUri("missing.json"))).IsSuccess(out _, out var error).Should().BeFalse();

        error!.Code.Should().Be("BCP272");
    }

    [TestMethod]
    public void Extends_IsNullWhenAbsentOrNull()
    {
        Read("""{ "extends": null }""").Unwrap().Extends.Should().BeNull();
        Read("""{}""").Unwrap().Extends.Should().BeNull();
        Read("""{ "extends": 5 }""").Unwrap().Extends!.Value.ValueKind.Should().Be(JsonValueKind.Number);
    }
}
