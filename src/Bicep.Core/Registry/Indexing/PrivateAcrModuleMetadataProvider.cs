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

namespace Bicep.Core.Registry.Indexing; //asdfg should this be in core?

/// <summary>
/// Provider to get modules metadata from a private ACR registry
/// </summary>
public class PrivateAcrModuleMetadataProvider : BaseModuleMetadataProvider, IRegistryModuleMetadataProvider
{
    private readonly CloudConfiguration cloud;
    private readonly IContainerRegistryClientFactory containerRegistryClientFactory;

    // TODO: Allow configuration (note that the default allows bicep anywhere in the module path) //asdfg test filter
    private string filterExpression = "";//asdfg "bicep"; 

    public PrivateAcrModuleMetadataProvider(
        CloudConfiguration cloud,
        string registry,
        IContainerRegistryClientFactory containerRegistryClientFactory
    ) : base(registry)
    {
        this.cloud = cloud;
        this.containerRegistryClientFactory = containerRegistryClientFactory;
    }

    protected override async Task<ImmutableArray<string>> GetLiveModuleVersionsAsync(string modulePath)
    {
        var registry = Registry;
        var repository = modulePath;

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var tags = await acrManager.GetRepositoryTags(cloud, registry, repository);
        return tags != null ? [.. tags] : [];
    }

    protected override async Task<RegistryModuleVersionMetadata?> GetLiveModuleVersionMetadataAsync(string modulePath, string version) //asdfg try?
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

        return new RegistryModuleVersionMetadata(version, description ?? title, documentationUri);
    }

    //asdfg bug: br:sawbicep.azurecr.io/de| => br:sawbicep.azurecr.io/dedemo

    protected override async Task<ImmutableArray<CachableModule>> GetLiveDataCoreAsync()
    {
        var filterRegex = new Regex(filterExpression, RegexOptions.IgnoreCase, matchTimeout: TimeSpan.FromMilliseconds(1));

        Trace.WriteLine($"Retrieving catalog for registry {Registry}...");

        AzureContainerRegistryManager acrManager = new(containerRegistryClientFactory);
        var catalog = await acrManager.GetCatalogAsync(cloud, Registry);
        var filteredCatalog = catalog.Where(m => filterRegex.IsMatch(m)).ToImmutableArray();

        Trace.WriteLine($"Found {catalog.Length} repositories, of which {filteredCatalog.Length} matched the filter (\"{filterExpression}\")");

        var modules = filteredCatalog
            .Select(m =>
            new CachableModule(
                new RegistryModuleMetadata(Registry, m, "asdfg description", "asdfg documentation uri"),
                null
            )
        ).ToImmutableArray();

        return modules;
    }
}
