// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Documentation;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationExampleDiscoveryTests
{
    [TestMethod]
    public void Discover_NoExamplesOrTestsFolders_ReturnsEmpty()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("main.bicep", "param foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().BeEmpty();
    }

    [TestMethod]
    public void Discover_MetadataDescription_ExtractsLiteralValue()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "metadata description = 'From metadata.'\nparam foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Description.Should().Be("From metadata.");
    }

    [TestMethod]
    public void Discover_LeadingCommentBlock_ExtractsJoinedCommentText()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "// Line one.\n// Line two.\nparam foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Description.Should().Be("Line one. Line two.");
    }

    [TestMethod]
    public void Discover_NoMetadataOrLeadingComment_DescriptionIsNull()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("examples/default/main.bicep", "param foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Description.Should().BeNull();
    }

    [TestMethod]
    public void Discover_NonBicepFilesInCategoryFolders_AreIgnored()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "param foo string"),
            ("examples/notes.md", "Not an example."));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Select(e => e.RelativePath).Should().Equal("examples/default/main.bicep");
    }

    [TestMethod]
    public void Discover_TestsFolderOnly_DiscoversExamplesFromTestsCategory()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("tests/e2e/main.test.bicep", "param foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Name.Should().Be("e2e");
    }

    [TestMethod]
    public void Discover_DirectFileWithUppercaseExtension_UsesTheSameRulesOnEveryPlatform()
    {
        var fileSet = MockFileSystemTestFileSet.Create(("examples/Main.BICEP", "param foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Name.Should().Be("Main");
    }

    private static Bicep.IO.Abstraction.IDirectoryHandle GetModuleRoot(MockFileSystemTestFileSet fileSet) =>
        fileSet.FileExplorer.GetDirectory(fileSet.GetUri(""));
}
