// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Configuration;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.FileSystem;
using Bicep.Core.TypeSystem;
using Bicep.Core.Registry;
using Bicep.Core.TypeSystem.ThirdParty;
using System.Collections.Concurrent;
using System.Linq;
using System.Diagnostics;

namespace Bicep.Core.Semantics.Namespaces;

public class RegistryAwareNamespaceProvider : INamespaceProvider
{
    private const string ProvidersRegistry = "bicepprovidersregistry.azurecr.io";

    private readonly INamespaceProvider baseProvider;
    private readonly IConfigurationManager configurationManager;
    private readonly OciModuleRegistry moduleRegistry;

    private record TypeProviderCacheKey(string ProviderName, string? ProviderVersion);
    private readonly ConcurrentDictionary<TypeProviderCacheKey, ThirdPartyResourceTypeProvider> typeProviderCache;
    private readonly Lazy<ImmutableArray<string>> registryNamespacesLazy;

    public RegistryAwareNamespaceProvider(DefaultNamespaceProvider baseProvider, IFileResolver fileResolver, IContainerRegistryClientFactory clientFactory, IFeatureProvider features, IConfigurationManager configurationManager)
    {
        this.baseProvider = baseProvider;
        this.configurationManager = configurationManager;
        this.moduleRegistry = new(fileResolver, clientFactory, features);
        this.typeProviderCache = new();
        this.registryNamespacesLazy = new(GetRegistryNamespaces);
    }

    public IEnumerable<string> AvailableNamespaces
        => baseProvider.AvailableNamespaces.Concat(registryNamespacesLazy.Value);

    public bool AllowImportStatements
        => true;

    public NamespaceType? TryGetNamespace(string providerName, string? providerVersion, string aliasName, ResourceScope resourceScope)
    {
        if (baseProvider.TryGetNamespace(providerName, providerVersion, aliasName, resourceScope) is {} namespaceType)
        {
            return namespaceType;
        }

        var cacheKey = new TypeProviderCacheKey(providerName, providerVersion);
        if (!typeProviderCache.TryGetValue(cacheKey, out var typeProvider))
        {
            var typeLoader = TryRestoreTypesFromRegistry(providerName, providerVersion, out _);
            if (typeLoader is null)
            {
                return null;
            }

            typeProvider = typeProviderCache.GetOrAdd(cacheKey, _ => new(typeLoader));
        }

        return new NamespaceType(
            aliasName: aliasName,
            new NamespaceSettings(
                true,
                providerName,
                (ObjectType)LanguageConstants.Object,
                providerName,
                providerVersion!),
            ImmutableArray<TypeProperty>.Empty,
            ImmutableArray<FunctionOverload>.Empty,
            ImmutableArray<BannedFunction>.Empty,
            ImmutableArray<Decorator>.Empty,
            typeProvider);
    }

    private FileSystemTypeLoader? TryRestoreTypesFromRegistry(string providerName, string? providerVersion, out DiagnosticBuilder.ErrorBuilderDelegate? failureBuilder)
    {
        var configuration = configurationManager.GetBuiltInConfiguration();
        if (providerVersion is null)
        {
            failureBuilder = x => x.UnrecognizedImportProvider(providerVersion is null ? providerName : $"{providerName}@{providerVersion}");
            return null;
        }

        var reference = $"{ProvidersRegistry}/{providerName}/types:{providerVersion}"; // e.g. bicepprovidersregistry.azurecr.io/github/types:v1
        if (OciArtifactModuleReference.TryParse(null, reference, configuration, out failureBuilder) is not {} moduleReference)
        {
            return null;
        }

        if (moduleRegistry.IsTypesRestoreRequired(moduleReference))
        {
            var restoreResult = moduleRegistry.RestoreTypes(configuration, moduleReference.AsEnumerable()).GetAwaiter().GetResult();
            if (restoreResult.TryGetValue(moduleReference, out failureBuilder))
            {
                return null;
            }
        }

        var typesPath = moduleRegistry.GetTypesPath(moduleReference);
        return new FileSystemTypeLoader(typesPath);
    }

    private ImmutableArray<string> GetRegistryNamespaces()
    {
        try
        {
            var configuration = configurationManager.GetBuiltInConfiguration();
            var providers = moduleRegistry.GetAvailableTypeProviders(configuration, new($"https://{ProvidersRegistry}")).GetAwaiter().GetResult();

            return providers.Select(x => $"{x.Name}@{x.Version}").ToImmutableArray();
        }
        catch (Exception e)
        {
            Trace.WriteLine($"Caught error fetching registry namespaces: {e}");
            return ImmutableArray<string>.Empty;
        }
    }
}
