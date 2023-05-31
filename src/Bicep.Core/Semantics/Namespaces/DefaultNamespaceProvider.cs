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
    private delegate NamespaceType GetNamespaceDelegate(string aliasName, ResourceScope resourceScope, IFeatureProvider features);
    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;

    public DefaultNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader)
    {
        var azResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoader);
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (alias, scope, features) => SystemNamespaceType.Create(alias, features),
            [AzNamespaceType.BuiltInName] = (alias, scope, features) => AzNamespaceType.Create(alias, scope, azResourceTypeProvider),
            [K8sNamespaceType.BuiltInName] = (alias, scope, features) => K8sNamespaceType.Create(alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope, IFeatureProvider features)
        => providerLookup.TryGetValue(providerName)?.Invoke(aliasName, resourceScope, features);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
