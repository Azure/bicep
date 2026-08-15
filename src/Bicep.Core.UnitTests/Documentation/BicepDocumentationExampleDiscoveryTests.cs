// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Documentation;
using Bicep.IO.Abstraction;
using Bicep.Testing;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
    public void Discover_MetadataDescription_UsesParsedEscapedAndMultilineLiteralValues()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/escaped/main.bicep", "metadata description = 'It\\'s ready'"),
            ("examples/multiline/main.bicep", "metadata description = '''\nLine one.\nLine two.\n'''"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Single(example => example.Name == "escaped").Description.Should().Be("It's ready");
        examples.Single(example => example.Name == "multiline").Description.Should().Be("Line one.\nLine two.\n");
    }

    [TestMethod]
    public void Discover_CommentedOutMetadata_IsIgnored()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "// metadata description = 'Not metadata.'\nparam foo string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Description.Should().Be("metadata description = 'Not metadata.'");
    }

    [TestMethod]
    public void Discover_MetadataName_OverridesFolderName()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("tests/e2e/defaults/main.test.bicep", "metadata name = 'Using only defaults'"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Name.Should().Be("Using only defaults");
    }

    [TestMethod]
    public void Discover_NonStringOrInterpolatedMetadata_UsesFallbacks()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "metadata name = 42\nmetadata description = 'prefix-${name}'"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Should().ContainSingle();
        examples[0].Name.Should().Be("default");
        examples[0].Description.Should().BeNull();
    }

    [TestMethod]
    public void Discover_DuplicateStringMetadata_ThrowsDocumentationException()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/default/main.bicep", "metadata name = 'first'\nmetadata name = 'second'"));

        var action = () => BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        action.Should().Throw<BicepDocumentationException>()
            .WithMessage("*metadata 'name' is declared more than once*");
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
    public void Discover_SkipPredicateExcludesFilesAndDirectories()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/direct.bicep", "param direct string"),
            ("examples/skipped/main.bicep", "param nested string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(
            GetModuleRoot(fileSet),
            uri => uri.Path.Contains("direct.bicep") || uri.Path.Contains("/skipped"));

        examples.Should().BeEmpty();
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
    public void Discover_NestedSiblingTests_UseTheirContainingFolderNames()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("tests/e2e/defaults/main.test.bicep", "param foo string"),
            ("tests/e2e/waf-aligned/main.test.bicep", "param foo string"),
            ("tests/e2e/defaults/dependencies.bicep", "param ignored string"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Select(example => example.Name).Should().Equal("defaults", "waf-aligned");
        examples.Select(example => example.RelativePath).Should().NotContain(path => path.EndsWith("dependencies.bicep"));
    }

    [TestMethod]
    public void Discover_DuplicateDisplayNames_ArePreservedInPathOrder()
    {
        var fileSet = MockFileSystemTestFileSet.Create(
            ("examples/first/main.bicep", "metadata name = 'same'"),
            ("tests/e2e/second/main.test.bicep", "metadata name = 'Same'"));

        var examples = BicepDocumentationExampleDiscovery.Discover(GetModuleRoot(fileSet));

        examples.Select(example => example.Name).Should().Equal("same", "Same");
    }

    [TestMethod]
    public void Discover_ExampleReadFailure_ThrowsActionableDocumentationException()
    {
        var moduleUri = IOUri.FromFilePath(Path.GetFullPath("module"));
        var moduleRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        var examplesRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        var testsRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        var file = new Mock<IFileHandle>(MockBehavior.Strict);
        moduleRoot.SetupGet(handle => handle.Uri).Returns(moduleUri);
        moduleRoot.Setup(handle => handle.GetDirectory("examples")).Returns(examplesRoot.Object);
        examplesRoot.SetupGet(handle => handle.Uri).Returns(moduleUri.Resolve("examples/"));
        examplesRoot.Setup(handle => handle.Exists()).Returns(true);
        examplesRoot.Setup(handle => handle.EnumerateFiles("*")).Returns([file.Object]);
        examplesRoot.Setup(handle => handle.EnumerateDirectories("*")).Returns([]);
        file.SetupGet(handle => handle.Uri).Returns(moduleUri.Resolve("examples/main.bicep"));
        file.Setup(handle => handle.ReadAllText()).Throws(new IOException("disk error"));
        moduleRoot.Setup(handle => handle.GetDirectory("tests")).Returns(testsRoot.Object);
        testsRoot.Setup(handle => handle.Exists()).Returns(false);

        var action = () => BicepDocumentationExampleDiscovery.Discover(moduleRoot.Object);

        action.Should().Throw<BicepDocumentationException>()
            .WithMessage("*examples*main.bicep*disk error*")
            .WithInnerException<IOException>();
    }

    [TestMethod]
    public void Discover_DirectoryEnumerationFailure_ThrowsActionableDocumentationException()
    {
        var moduleUri = IOUri.FromFilePath(Path.GetFullPath("module"));
        var moduleRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        var examplesRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        moduleRoot.SetupGet(handle => handle.Uri).Returns(moduleUri);
        moduleRoot.Setup(handle => handle.GetDirectory("examples")).Returns(examplesRoot.Object);
        examplesRoot.SetupGet(handle => handle.Uri).Returns(moduleUri.Resolve("examples/"));
        examplesRoot.Setup(handle => handle.Exists()).Returns(true);
        examplesRoot.Setup(handle => handle.EnumerateFiles("*")).Throws(new UnauthorizedAccessException("denied"));

        var action = () => BicepDocumentationExampleDiscovery.Discover(moduleRoot.Object);

        action.Should().Throw<BicepDocumentationException>()
            .WithMessage("*Unable to discover usage examples*denied*")
            .WithInnerException<UnauthorizedAccessException>();
    }

    [TestMethod]
    public void Discover_ExcessiveDirectoryDepth_ThrowsActionableDocumentationException()
    {
        var moduleUri = IOUri.FromFilePath(Path.GetFullPath("module"));
        var moduleRoot = new Mock<IDirectoryHandle>(MockBehavior.Strict);
        var directories = Enumerable.Range(0, 102)
            .Select(_ => new Mock<IDirectoryHandle>(MockBehavior.Strict))
            .ToArray();
        moduleRoot.SetupGet(handle => handle.Uri).Returns(moduleUri);
        moduleRoot.Setup(handle => handle.GetDirectory("examples")).Returns(directories[0].Object);
        directories[0].Setup(handle => handle.Exists()).Returns(true);

        for (var index = 0; index < directories.Length; index++)
        {
            directories[index].SetupGet(handle => handle.Uri).Returns(moduleUri.Resolve($"examples/{index}/"));
            directories[index].Setup(handle => handle.EnumerateFiles("*")).Returns([]);
            directories[index].Setup(handle => handle.EnumerateDirectories("*"))
                .Returns(index + 1 < directories.Length ? [directories[index + 1].Object] : []);
        }

        var action = () => BicepDocumentationExampleDiscovery.Discover(moduleRoot.Object);

        action.Should().Throw<BicepDocumentationException>()
            .WithMessage("*maximum directory depth*");
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
