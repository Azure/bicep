// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.SourceGraph;
using Bicep.Core.UnitTests.Assertions;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Registry.Oci
{
    [TestClass]
    public class OciArtifactEmulatedReferenceTests
    {
        [TestMethod]
        [DataRow("keyvault:1.0.0", "keyvault")]
        [DataRow("storage/queue:v2.0", "storage/queue")]
        [DataRow("keyvault@sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4", "keyvault")]
        [DataRow("keyvault", "keyvault")]
        [DataRow("a/b/c:latest", "a/b/c")]
        [DataRow("mymodule:v1", "mymodule")]
        [DataRow("", "")]
        [DataRow(":", "")]
        [DataRow("@sha256:abc", "")]
        public void ExtractModulePath_ShouldExtractCorrectPath(string input, string expected)
        {
            OciArtifactEmulatedReference.ExtractModulePath(input).Should().Be(expected);
        }

        [TestMethod]
        public void TryParse_EmptyModulePath_ShouldFail()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result = OciArtifactEmulatedReference.TryParse(
                referencingFile,
                "./modules",
                configFileUri,
                ":1.0.0",
                fileExplorer);

            result.IsSuccess(out _, out var failureBuilder).Should().BeFalse();
            var diagnostic = failureBuilder!(DiagnosticBuilder.ForDocumentStart());
            diagnostic.Code.Should().Be("BCP090");
        }

        [TestMethod]
        public void TryParse_ValidModulePath_ShouldSucceed()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result = OciArtifactEmulatedReference.TryParse(
                referencingFile,
                "../bicepModules",
                configFileUri,
                "keyvault:1.0.0",
                fileExplorer);

            result.IsSuccess(out var reference, out _).Should().BeTrue();
            reference!.UnqualifiedReference.Should().Be("keyvault");
            reference!.FullyQualifiedReference.Should().Be("br:keyvault");
            reference!.IsExternal.Should().BeFalse();
            reference.Scheme.Should().Be("br-fs");
        }

        [TestMethod]
        public void TryParse_MultiSegmentModulePath_ShouldSucceed()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result = OciArtifactEmulatedReference.TryParse(
                referencingFile,
                "./modules",
                configFileUri,
                "storage/queue:v2.0",
                fileExplorer);

            result.IsSuccess(out var reference, out _).Should().BeTrue();
            reference!.UnqualifiedReference.Should().Be("storage/queue");
            reference!.FullyQualifiedReference.Should().Be("br:storage/queue");
        }

        [TestMethod]
        public void TryParse_DigestReference_ShouldIgnoreDigestAndSucceed()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result = OciArtifactEmulatedReference.TryParse(
                referencingFile,
                "./modules",
                configFileUri,
                "keyvault@sha256:e207a69d02b3de40d48ede9fd208d80441a9e590a83a0bc915d46244c03310d4",
                fileExplorer);

            result.IsSuccess(out var reference, out _).Should().BeTrue();
            reference!.UnqualifiedReference.Should().Be("keyvault");
            reference!.FullyQualifiedReference.Should().Be("br:keyvault");
        }

        [TestMethod]
        public void TryGetEntryPointFileHandle_ShouldReturnFileHandle()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var parseResult = OciArtifactEmulatedReference.TryParse(
                referencingFile,
                "../bicepModules",
                configFileUri,
                "keyvault:1.0.0",
                fileExplorer);

            parseResult.IsSuccess(out var reference, out _).Should().BeTrue();

            var entryPointResult = reference!.TryGetEntryPointFileHandle();

            entryPointResult.IsSuccess(out var fileHandle, out _).Should().BeTrue();
            fileHandle.Should().NotBeNull();
            fileHandle!.Uri.Path.Should().Contain("keyvault.bicep");
        }

        [TestMethod]
        public void Equals_SameReferences_ShouldBeEqual()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result1 = OciArtifactEmulatedReference.TryParse(
                referencingFile, "../bicepModules", configFileUri, "keyvault:1.0.0", fileExplorer);
            var result2 = OciArtifactEmulatedReference.TryParse(
                referencingFile, "../bicepModules", configFileUri, "keyvault:2.0.0", fileExplorer);

            result1.IsSuccess(out var ref1, out _).Should().BeTrue();
            result2.IsSuccess(out var ref2, out _).Should().BeTrue();

            ref1!.Equals(ref2).Should().BeTrue();
            ref1.GetHashCode().Should().Be(ref2!.GetHashCode());
        }

        [TestMethod]
        public void Equals_DifferentModulePaths_ShouldNotBeEqual()
        {
            var fileExplorer = new FileSystemFileExplorer(new MockFileSystem());
            var referencingFile = BicepTestConstants.CreateDummyBicepFile();
            var configFileUri = new IOUri("file", "", "/repo/bicepconfig.json");

            var result1 = OciArtifactEmulatedReference.TryParse(
                referencingFile, "../bicepModules", configFileUri, "keyvault:1.0.0", fileExplorer);
            var result2 = OciArtifactEmulatedReference.TryParse(
                referencingFile, "../bicepModules", configFileUri, "storage:1.0.0", fileExplorer);

            result1.IsSuccess(out var ref1, out _).Should().BeTrue();
            result2.IsSuccess(out var ref2, out _).Should().BeTrue();

            ref1!.Equals(ref2).Should().BeFalse();
        }

        [TestMethod]
        public void TryGetOciArtifactModuleAlias_BothRegistryAndFileSystemSet_ShouldFail()
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(
                new()
                {
                    ["moduleAliases.br.myAlias.registry"] = "example.azurecr.io",
                    ["moduleAliases.br.myAlias.fileSystem"] = "../bicepModules",
                });

            var result = configuration.ModuleAliases.TryGetOciArtifactModuleAlias("myAlias");

            result.IsSuccess(out _, out var failureBuilder).Should().BeFalse();
            var diagnostic = failureBuilder!(DiagnosticBuilder.ForDocumentStart());
            diagnostic.Code.Should().Be("BCP446");
            diagnostic.Message.Should().Contain("mutually exclusive");
        }

        [TestMethod]
        public void TryGetOciArtifactModuleAlias_NeitherRegistryNorFileSystemSet_ShouldFail()
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(
                new()
                {
                    ["moduleAliases.br.myAlias.modulePath"] = "path",
                });

            var result = configuration.ModuleAliases.TryGetOciArtifactModuleAlias("myAlias");

            result.IsSuccess(out _, out var failureBuilder).Should().BeFalse();
            var diagnostic = failureBuilder!(DiagnosticBuilder.ForDocumentStart());
            diagnostic.Code.Should().Be("BCP216");
            diagnostic.Message.Should().Contain("fileSystem");
        }

        [TestMethod]
        public void TryGetOciArtifactModuleAlias_OnlyFileSystemSet_ShouldSucceed()
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(
                new()
                {
                    ["moduleAliases.br.myAlias.fileSystem"] = "../bicepModules",
                });

            var result = configuration.ModuleAliases.TryGetOciArtifactModuleAlias("myAlias");

            result.IsSuccess(out var alias, out _).Should().BeTrue();
            alias!.FileSystem.Should().Be("../bicepModules");
            alias.Registry.Should().BeNull();
        }

        [TestMethod]
        public void TryGetOciArtifactModuleAlias_OnlyRegistrySet_ShouldSucceed()
        {
            var configuration = BicepTestConstants.CreateMockConfiguration(
                new()
                {
                    ["moduleAliases.br.myAlias.registry"] = "example.azurecr.io",
                });

            var result = configuration.ModuleAliases.TryGetOciArtifactModuleAlias("myAlias");

            result.IsSuccess(out var alias, out _).Should().BeTrue();
            alias!.Registry.Should().Be("example.azurecr.io");
            alias.FileSystem.Should().BeNull();
        }
    }
}
