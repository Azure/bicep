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

    private readonly IBicepCloudConfiguration cloud;
    private readonly IOciRegistryTransportFactory transportFactory;
    private readonly bool useOrasSdk;

    public PrivateAcrModuleMetadataProvider(
        IBicepCloudConfiguration cloud,
        string registry,
        IOciRegistryTransportFactory transportFactory,
        bool useOrasSdk
    ) : base(registry)
    {
        this.cloud = cloud;
        this.transportFactory = transportFactory;
        this.useOrasSdk = useOrasSdk;
    }

    protected override async Task<ImmutableArray<RegistryModuleVersionMetadata>> GetLiveModuleVersionsAsync(string modulePath)
    {
        var registry = Registry;
        var repository = modulePath;

        var transport = transportFactory.GetTransport(registry, useOrasSdk);

        // For this part we want to throw on errors
        var tags = await transport.GetRepositoryTagsAsync(cloud, registry, repository);

        // For the rest, we'll be resilient
        return [.. await Task.WhenAll(
            tags.Select(async tag => await TryGetLiveModuleVersionMetadataAsync(modulePath, tag))
        )];
    }

    private async Task<RegistryModuleVersionMetadata> TryGetLiveModuleVersionMetadataAsync(string modulePath, string version)
    {
        try
        {
            var transport = transportFactory.GetTransport(Registry, useOrasSdk);
            var artifactResult = await transport.PullArtifactAsync(cloud, new OciArtifactAddressComponents(Registry, modulePath, version, null));
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

        var transport = transportFactory.GetTransport(Registry, useOrasSdk);
        var catalog = await transport.GetRepositoryNamesAsync(cloud, Registry, MaxReturnedModules);

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
        // Use the last version with valid metadata in the transport's ordering.
        // ORAS returns tags in ordinal order, not publication or semantic-version order.
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
