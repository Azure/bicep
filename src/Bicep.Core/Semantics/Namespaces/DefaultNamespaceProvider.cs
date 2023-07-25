// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProvider : INamespaceProvider
{
    private delegate NamespaceType? GetNamespaceDelegate(
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind,
        string? version = null);
    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;

    private readonly IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory;

    public DefaultNamespaceProvider(IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory)
    {
        var builtInAzResourceTypeLoaderVersion = "1.0.0";
        var builtInAzResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoaderFactory.GetBuiltInTypeLoader(), builtInAzResourceTypeLoaderVersion);

        this.azResourceTypeLoaderFactory = azResourceTypeLoaderFactory;
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind, version) => SystemNamespaceType.Create(alias, features, sourceFileKind),
            [AzNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind, version) =>
            {
                AzResourceTypeProvider? provider = builtInAzResourceTypeProvider;
                if (features.DynamicTypeLoadingEnabled && version is not null)
                {
                    // TODO(asilverman): Current implementation of dynamic type loading needs to be refactored to handle caching of the provider to optimize 
                    // performance hit as a result of recreating the provider for each call to TryGetNamespace.
                    // Tracked by - https://msazure.visualstudio.com/One/_sprints/taskboard/Azure-ARM-Deployments/One/Custom/Azure-ARM/Gallium/Jul-2023?workitem=24563440
                    var loader = azResourceTypeLoaderFactory.GetResourceTypeLoader(version, features);
                    if (loader is null)
                    {
                        return null;
                    }
                    var overriddenProviderVersion = builtInAzResourceTypeLoaderVersion;
                    if (features.DynamicTypeLoadingEnabled)
                    {
                        overriddenProviderVersion = version ?? overriddenProviderVersion;
                    }
                    provider = new AzResourceTypeProvider(loader, overriddenProviderVersion);
                }
                return AzNamespaceType.Create(alias, scope, provider);
            },
            [K8sNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind, version) => K8sNamespaceType.Create(alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(
        string providerName,
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind,
        string? version = null)
    //TODO(asilverman): This is the location where we would like to add support for extensibility providers, we want to add a new key and a new loader for the ext. provider
        => providerLookup.TryGetValue(providerName)?.Invoke(aliasName, resourceScope, features, sourceFileKind, version);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
