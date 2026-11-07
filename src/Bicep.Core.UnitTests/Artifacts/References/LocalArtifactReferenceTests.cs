// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Resolution;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts.References
{
    [TestClass]
    public class LocalArtifactReferenceTests
    {
        private static readonly IOUri ReferencingUri = CreateUri("/repo/sub/main.bicep");

        [DataRow("test.bicep", "test.bicep")]
        [DataRow("test.bicep", "./test.bicep")]
        [DataRow("../bar/foo.bicep", "../bar/foo.bicep")]
        [DataRow("../bar/foo.bicep", "../sub/../bar/foo.bicep")]
        [TestMethod]
        public void References_to_the_same_file_should_be_equal(string first, string second)
        {
            var resolver = CreateResolver(out _);
            var firstReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, first);
            var secondReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, second);

            firstReference.Equals(secondReference).Should().BeTrue();
            firstReference.GetHashCode().Should().Be(secondReference.GetHashCode());
        }

        [DataRow("test.bicep", "other.bicep")]
        [DataRow("../bar/foo.bicep", "foo.bicep")]
        [DataRow("./t.json", "./t.bicep")]
        [TestMethod]
        public void References_to_different_files_should_not_be_equal(string first, string second)
        {
            var resolver = CreateResolver(out _);
            var firstReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, first);
            var secondReference = Resolve(resolver, ReferencingUri, ArtifactType.Module, second);

            firstReference.Equals(secondReference).Should().BeFalse();
        }

        [TestMethod]
        public void The_same_path_written_in_different_folders_should_not_be_equal()
        {
            var resolver = CreateResolver(out _);
            var first = Resolve(resolver, CreateUri("/repo/a/main.bicep"), ArtifactType.Module, "mod.bicep");
            var second = Resolve(resolver, CreateUri("/repo/b/main.bicep"), ArtifactType.Module, "mod.bicep");

            first.Equals(second).Should().BeFalse();
        }

        [TestMethod]
        public void The_same_file_as_a_module_and_an_extension_should_not_be_equal()
        {
            var resolver = CreateResolver(out _);
            var module = Resolve(resolver, ReferencingUri, ArtifactType.Module, "ext.tgz");
            var extension = Resolve(resolver, ReferencingUri, ArtifactType.Extension, "ext.tgz");

            module.Equals(extension).Should().BeFalse();
        }

        [DataRow("./test.bicep", "/repo/sub/test.bicep")]
        [DataRow("foo/bar/test.bicep", "/repo/sub/foo/bar/test.bicep")]
        [DataRow("../bar/test.bicep", "/repo/bar/test.bicep")]
        [TestMethod]
        public void Relative_paths_should_be_anchored_on_the_referencing_file(string path, string expectedFilePath)
        {
            var reference = Resolve(CreateResolver(out _), ReferencingUri, ArtifactType.Module, path);

            reference.FullyQualifiedReference.Should().Be(path);
            reference.File.Uri.Should().Be(CreateUri(expectedFilePath));
            reference.TryGetEntryPointFileHandle().Unwrap().Uri.Should().Be(CreateUri(expectedFilePath));
        }

        [TestMethod]
        public async Task Local_extension_should_be_unpacked_on_first_use()
        {
            var resolver = CreateResolver(out var fileExplorer);
            var types = BinaryData.FromString("types");
            fileExplorer.GetFile(CreateUri("/repo/sub/ext.tgz")).Write(await ExtensionArtifactArchiveCodec.Encode(new(types, LocalDeployEnabled: false, [])));

            var reference = Resolve(resolver, ReferencingUri, ArtifactType.Extension, "./ext.tgz");
            var entryPointResult = reference.TryGetEntryPointFileHandle();
            entryPointResult.IsSuccess(out var entryPointOrNull, out var errorBuilder).Should().BeTrue(errorBuilder?.Invoke(DiagnosticBuilder.ForDocumentStart()).Message);
            var entryPoint = entryPointOrNull!;

            entryPoint.Uri.Path.Should().MatchRegex("/\\.bicep/local/sha256_[0-9a-f]{64}/types\\.tgz$");
            entryPoint.Exists().Should().BeTrue();
            entryPoint.TryReadBinaryData().Unwrap().ToString().Should().Be("types");

            // The cache is content-addressed, so another reference to the same content reuses the unpacked package.
            fileExplorer.GetFile(CreateUri("/repo/copy.tgz")).Write(fileExplorer.GetFile(CreateUri("/repo/sub/ext.tgz")).TryReadBinaryData().Unwrap());
            var copy = Resolve(resolver, ReferencingUri, ArtifactType.Extension, "../copy.tgz");
            copy.TryGetEntryPointFileHandle().Unwrap().Uri.Should().Be(entryPoint.Uri);
        }

        [TestMethod]
        public void Invalid_local_extension_should_report_an_error()
        {
            var resolver = CreateResolver(out var fileExplorer);
            fileExplorer.GetFile(CreateUri("/repo/sub/ext.tgz")).Write("not an archive");

            var reference = Resolve(resolver, ReferencingUri, ArtifactType.Extension, "./ext.tgz");

            reference.TryGetEntryPointFileHandle().IsSuccess(out _, out var errorBuilder).Should().BeFalse();
            var diagnostic = errorBuilder!(DiagnosticBuilder.ForDocumentStart());
            diagnostic.Code.Should().Be("BCP192");
            diagnostic.Message.Should().StartWith("Unable to restore the artifact with reference \"./ext.tgz\": The file is not a valid extension package.");
        }

        [TestMethod]
        public void Missing_local_extension_should_report_an_error()
        {
            var reference = Resolve(CreateResolver(out _), ReferencingUri, ArtifactType.Extension, "./missing.tgz");

            reference.TryGetEntryPointFileHandle().IsSuccess().Should().BeFalse();
        }

        private static IOUri CreateUri(string path) => IOUri.FromFilePath(OperatingSystem.IsWindows() ? "C:" + path : path);

        // The file system is mocked rather than in-memory because the in-memory file store does not preserve binary content.
        private static IArtifactReferenceResolver CreateResolver(out IFileExplorer fileExplorer)
        {
            fileExplorer = new FileSystemFileExplorer(new MockFileSystem());

            return new ServiceCollection()
                .AddSingleton(fileExplorer)
                .AddBicepCore()
                .BuildServiceProvider()
                .GetRequiredService<IArtifactReferenceResolver>();
        }

        private static LocalArtifactReference Resolve(IArtifactReferenceResolver resolver, IOUri referencingUri, ArtifactType type, string path) =>
            resolver.TryResolve(referencingUri, type, path).Unwrap().Should().BeOfType<LocalArtifactReference>().Subject;
    }
}
