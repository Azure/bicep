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
using static Bicep.Core.Registry.Catalog.RegistryModuleMetadata;

namespace Bicep.Core.Registry.Catalog.Implementation.PrivateRegistries;

/// <summary>
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrModuleMetadataProvider : BaseModuleMetadataProvider, IRegistryModuleMetadataProvider
{
    private const int MaxReturnedModules = 10000;

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

    private async Task<RegistryModuleVersionMetadata> TryGetLiveModuleVersionMetadataAsync(string modulePath, string version)
    {
        try
        {
            AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
            var artifactResult = await acrManager.PullArtifactAsync(cloud, new OciArtifactAddressComponents(Registry, modulePath, version, null));
            var manifest = artifactResult.Manifest;

            if (manifest.ArtifactType != BicepMediaTypes.BicepModuleArtifactType)
            {
                return RegistryModuleVersionMetadata.UnexpectedArtifactType(version, manifest.ArtifactType ?? "(null)");
            }

            string? description = null;
            string? documentationUri = null;
            string? title = null;

            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDescriptionAnnotation, out description);
            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageDocumentationAnnotation, out documentationUri);
            manifest.Annotations?.TryGetValue(OciAnnotationKeys.OciOpenContainerImageTitleAnnotation, out title);

            return new RegistryModuleVersionMetadata(
                version,
                IsBicepModule: true,
                new RegistryMetadataDetails(description ?? title, documentationUri));
        }
        catch (InvalidArtifactException ex)
        {
            Trace.WriteLine($"Invalid Bicep module {modulePath}, version {version}: {ex.Message}");
            return RegistryModuleVersionMetadata.InvalidModule(version, ex);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Failed to get version details for module {modulePath} version {version}: {ex.Message}");
            return RegistryModuleVersionMetadata.DownloadError(version, ex);
        }
    }

    protected override async Task<ImmutableArray<IRegistryModuleMetadata>> GetLiveDataCoreAsync()
    {
        Trace.WriteLine($"Retrieving catalog for registry {Registry}...");

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var catalog = await acrManager.GetRepositoryNamesAsync(cloud, Registry, MaxReturnedModules);

        Trace.WriteLine($"Found {catalog.Length} repositories");

        var modules = catalog
            .Reverse() // Reverse to search the latest modules first
            .Select(m => new RegistryModuleMetadata(
                Registry,
                m,
                async () =>
                {
                    var versions = await GetLiveModuleVersionsAsync(m);
                    var moduleDetails = GetModuleDetails(versions);
                    return new ComputedData(moduleDetails, versions);
                })
        ).ToImmutableArray();

        return [.. modules.Cast<IRegistryModuleMetadata>()];
    }

    private RegistryMetadataDetails GetModuleDetails(ImmutableArray<RegistryModuleVersionMetadata> versions)
    {
        // OCI modules don't have a description or documentation URI, we just use the most recent version with valid metadata
        var lastVersion = versions.LastOrDefault(x =>
            x.IsBicepModule == true
            && (!string.IsNullOrWhiteSpace(x.Details.Description) || !string.IsNullOrWhiteSpace(x.Details.DocumentationUri)));
        lastVersion ??= versions.LastOrDefault(x => x.IsBicepModule == true && !string.IsNullOrWhiteSpace(x.Details.DocumentationUri));
        lastVersion ??= versions.LastOrDefault(x => x.IsBicepModule == true);
        lastVersion ??= versions.LastOrDefault();

        if (lastVersion is { })
        {
            return lastVersion.Details;
        }

        return new RegistryMetadataDetails(null, null);
    }
}
