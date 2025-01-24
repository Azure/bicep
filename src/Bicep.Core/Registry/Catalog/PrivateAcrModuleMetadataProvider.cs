// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.IO.Packaging;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using Bicep.Core.Configuration;
using Bicep.Core.Extensions;
using Bicep.Core.Registry.Oci;
using Microsoft.Extensions.DependencyInjection;

namespace Bicep.Core.Registry.Catalog; //asdfg should this be in core?

/// <summary>
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrModuleMetadataProvider : BaseModuleMetadataProvider, IRegistryModuleMetadataProvider
{
    private readonly CloudConfiguration cloud;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;

    public PrivateAcrModuleMetadataProvider(
        CloudConfiguration cloud,
        string registry,
        IContainerRegistryClientFactory containerRegistryClientFactory
    ) : base(registry)
    {
        this.cloud = cloud;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
    }

    protected override async Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        var registry = Registry;
        var repository = modulePath;

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);

        // For this part we want to throw on errors
        var tags = await acrManager.GetRepositoryTagsAsync(cloud, registry, repository);

        // For the rest, we'll be resilient
        return [.. await Task.WhenAll(
            tags.Select(async tag => await TryGetLiveModuleVersionMetadataAsync(modulePath, tag))
        )];
    }

    private async Task<RegistryModuleVersionMetadata> TryGetLiveModuleVersionMetadataAsync(string modulePath, string version) //asdfg just get details
    {
        try
        {
            AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
            var artifactResult = await acrManager.PullArtifactAsync(cloud, new OciArtifactReference(ArtifactType.Module, Registry, modulePath, version, null, new Uri("file://asdfg")));
            var manifest = artifactResult.Manifest;
            string? description = null;
            string? documentationUri = null;
            string? title = null;

            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation, out description);
            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, out documentationUri);
            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageTitleAnnotation, out title);

            return new RegistryModuleVersionMetadata(version,
                new RegistryMetadataDetails(description ?? title, documentationUri));
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to get version details for module {modulePath} version {version}: {ex.Message}");
            return new RegistryModuleVersionMetadata(version, new RegistryMetadataDetails(null, null));
        }
    }

    //asdfg bug: br:sawbicep.azurecr.io/de| => br:sawbicep.azurecr.io/dedemo

    protected override async Task<ImmutableArray<IRegistryModuleMetadata>> GetLiveDataCoreAsync()
    {
        Trace.WriteLine($"Retrieving catalog for registry {Registry}...");

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var catalog = await acrManager.GetRepositoryNamesAsync(cloud, Registry); //asdfg limit?

        Trace.WriteLine($"Found {catalog.Length} repositories");

        var modules = catalog
            .Reverse() // Reverse to get the latest modules first
            .Select(m =>
            {
                var getVersionsAsyncFunc = () => this.GetLiveModuleVersionsAsync(m);
                var getDetailsAsyncFunc = () => this.GetLiveModuleDetails(getVersionsAsyncFunc);
                return new DefaultRegistryModuleMetadata(Registry, m, getDetailsAsyncFunc, getVersionsAsyncFunc);
            }
        ).ToImmutableArray();

        return [.. modules.Cast<IRegistryModuleMetadata>()];
    }

    private async Task<RegistryMetadataDetails> GetLiveModuleDetails(
        Func<Task<ImmutableArray<RegistryModuleVersionMetadata>>> getVersionsAsyncFunc)
    {
        // OCI modules don't have a description or documentation URI, we just use the most recent version's metadata
        var versions = await getVersionsAsyncFunc();
        if (versions.LastOrDefault() is { } lastVersion)
        {
            return lastVersion.Details;
        }

        return new RegistryMetadataDetails(null, null);
    }
}
