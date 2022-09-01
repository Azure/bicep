// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProvider : INamespaceProvider
{
    private delegate NamespaceType GetNamespaceDelegate(string aliasName, ResourceScope resourceScope);

    private readonly IFeatureProvider featureProvider;
    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;

    public DefaultNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader, IFeatureProvider featureProvider)
    {
        this.featureProvider = featureProvider;
        var azResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoader);
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (alias, scope) => SystemNamespaceType.Create(alias, featureProvider),
            [AzNamespaceType.BuiltInName] = (alias, scope) => AzNamespaceType.Create(alias, scope, azResourceTypeProvider),
            [K8sNamespaceType.BuiltInName] = (alias, scope) => K8sNamespaceType.Create(alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope)
        => providerLookup.TryGetValue(providerName)?.Invoke(aliasName, resourceScope);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;

    public bool AllowImportStatements
        => featureProvider.ImportsEnabled;
}
