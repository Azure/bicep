// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.Artifacts;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Artifacts.Registries;
using Bicep.Core.Extensions;
using Bicep.Core.RegistryClients.Oci;
using Bicep.Core.UnitTests.Mock;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Artifacts.Registries;

[TestClass]
public class OciArtifactRegistryEntryPointTests
{
    [TestMethod]
    [DataRow("contoso.azurecr.io", true)]
    [DataRow("untrusted.example.com", false)]
    public void A_cached_artifact_is_only_loaded_from_a_trusted_registry(string registry, bool expectRestored)
    {
        var reference = CreateCachedModuleReference(registry);
        IRemoteArtifactRegistry ociRegistry = new OciArtifactRegistry(
            new RegistryConfiguration(PermitUntrustedRegistries: false),
            StrictMock.Of<IOciRegistryTransportFactory>().Object,
            NullLogger<OciArtifactRegistry>.Instance);

        // An untrusted registry's artifact counts as not restored, so that restore runs and reports BCP446.
        var entryPoint = ociRegistry.TryGetEntryPoint(reference);

        if (expectRestored)
        {
            entryPoint.Should().NotBeNull();
            entryPoint!.Uri.Should().Be(reference.ModuleCacheEntry.MainTemplateFile.Uri);
        }
        else
        {
            entryPoint.Should().BeNull();
        }
    }

    [TestMethod]
    public void An_incomplete_cache_entry_is_not_restored()
    {
        var reference = CreateCachedModuleReference("contoso.azurecr.io");
        reference.ModuleCacheEntry.MetadataFile.Delete();
        IRemoteArtifactRegistry ociRegistry = new OciArtifactRegistry(
            new RegistryConfiguration(PermitUntrustedRegistries: false),
            StrictMock.Of<IOciRegistryTransportFactory>().Object,
            NullLogger<OciArtifactRegistry>.Instance);

        ociRegistry.TryGetEntryPoint(reference).Should().BeNull();
    }

    private static OciArtifactReference CreateCachedModuleReference(string registry)
    {
        var cacheRoot = new FileSystemFileExplorer(new MockFileSystem()).GetDirectory(IOUri.FromFilePath("/cache/")).EnsureExists();
        var context = new OciArtifactContext(BicepTestConstants.BuiltInConfiguration.Cloud, cacheRoot, UseOrasSdk: false);
        var reference = new OciArtifactReference(context, ArtifactType.Module, registry, "modules/storage", "v1", null);

        var cacheEntry = reference.ModuleCacheEntry;
        cacheEntry.CacheDirectory.EnsureExists();
        cacheEntry.MainTemplateFile.Write("{}");
        cacheEntry.ManifestFile.Write("{}");
        cacheEntry.MetadataFile.Write("{}");

        return reference;
    }
}
