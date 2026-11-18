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
public class ConfigurationChainResolverTests
{
    private static ConfigurationChainResolution Resolve(params (string Path, string Contents)[] files)
    {
        var fileSet = InMemoryTestFileSet.Create([.. files.Select(file => (file.Path, (TestFileData)file.Contents))]);

        return ConfigurationChainResolver.Resolve(
            fileSet.GetUri("bicepconfig.json"),
            uri => fileSet.FileExplorer.GetFile(uri) is var handle && handle.Exists() ? ConfigurationFileReader.Read(handle) : null);
    }

    private static IOUri Uri(string path) => TestFileUri.FromInMemoryPath(path);

    [TestMethod]
    public void Resolve_ReturnsFilesLeafFirst()
    {
        var resolution = Resolve(
            ("bicepconfig.json", """{ "extends": "./a/base.json" }"""),
            ("a/base.json", """{ "extends": "../b/base.json" }"""),
            ("b/base.json", """{}"""));

        resolution.Error.Should().BeNull();
        resolution.Files.Select(file => file.Uri).Should().Equal(Uri("bicepconfig.json"), Uri("a/base.json"), Uri("b/base.json"));
    }

    [TestMethod]
    public void Resolve_MissingBase_IsReportedOnTheFileThatExtendsIt()
    {
        var resolution = Resolve(
            ("bicepconfig.json", """{ "extends": "./a/base.json" }"""),
            ("a/base.json", """{ "extends": "./missing.json" }"""));

        resolution.Error!.File.Should().Be(Uri("a/base.json"));
        resolution.Error.Diagnostic.Code.Should().Be("BCP272");
        resolution.Error.Diagnostic.Message.Should().Contain(Uri("a/missing.json").ToString());
        resolution.Files.Should().HaveCount(2);
    }

    [TestMethod]
    public void Resolve_UnparsableBase_IsReportedOnTheBase()
    {
        var resolution = Resolve(
            ("bicepconfig.json", """{ "extends": "./a/base.json" }"""),
            ("a/base.json", """{"""));

        resolution.Error!.File.Should().Be(Uri("a/base.json"));
        resolution.Error.Diagnostic.Code.Should().Be("BCP271");
    }

    [TestMethod]
    [DataRow("""{ "extends": 5 }""")]
    [DataRow("""{ "extends": "/absolute/base.json" }""")]
    public void Resolve_InvalidExtendsInBase_IsReportedOnTheBase(string baseContents)
    {
        var resolution = Resolve(
            ("bicepconfig.json", """{ "extends": "./a/base.json" }"""),
            ("a/base.json", baseContents));

        resolution.Error!.File.Should().Be(Uri("a/base.json"));
        resolution.Error.Diagnostic.Code.Should().Be("BCP453");
    }

    [TestMethod]
    public void Resolve_Cycle_IsReportedOnTheLeaf()
    {
        var resolution = Resolve(
            ("bicepconfig.json", """{ "extends": "./a/base.json" }"""),
            ("a/base.json", """{ "extends": "../bicepconfig.json" }"""));

        resolution.Error!.File.Should().Be(Uri("bicepconfig.json"));
        resolution.Error.Diagnostic.Code.Should().Be("BCP454");
    }
}
