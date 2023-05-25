// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProvider : INamespaceProvider
{
    private delegate NamespaceType? GetNamespaceDelegate(
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider features,
        string? version = null);
    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;
    private readonly IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory;

    public DefaultNamespaceProvider(IAzResourceTypeLoaderFactory azResourceTypeLoaderFactory)
    {
        this.azResourceTypeLoaderFactory = azResourceTypeLoaderFactory;
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (alias, scope, features, ids) => SystemNamespaceType.Create(alias, features),
            [AzNamespaceType.BuiltInName] = (alias, scope, features, version) =>
            {
                var loader = azResourceTypeLoaderFactory.GetResourceTypeLoader(version, features);
                if (loader is null)
                {
                    return null;
                }
                var overriddenProviderVersion = "1.0.0"; //builtin version
                if (features.DynamicTypeLoading)
                {
                    overriddenProviderVersion = version ?? overriddenProviderVersion;
                }
                return AzNamespaceType.Create(alias, scope, new AzResourceTypeProvider(loader, overriddenProviderVersion));

            },
            [K8sNamespaceType.BuiltInName] = (alias, scope, features, ids) => K8sNamespaceType.Create(alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(
        string providerName,
        string aliasName,
        ResourceScope resourceScope,
        IFeatureProvider features,
        string? version = null)
    //TODO(asilverman): This is the location where we would like to add support for extensibility providers, we want to add a new key and a new loader for the ext. provider
        => providerLookup.TryGetValue(providerName)?.Invoke(aliasName, resourceScope, features, version);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
