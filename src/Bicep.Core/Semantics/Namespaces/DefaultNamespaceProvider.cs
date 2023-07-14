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
    private delegate NamespaceType GetNamespaceDelegate(string aliasName, ResourceScope resourceScope, IFeatureProvider features, BicepSourceFileKind sourceFileKind);
    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;

    public DefaultNamespaceProvider(IAzResourceTypeLoader azResourceTypeLoader)
    {
        var azResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoader);
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind) => SystemNamespaceType.Create(alias, features, sourceFileKind),
            [AzNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind) => AzNamespaceType.Create(alias, scope, azResourceTypeProvider, sourceFileKind),
            [K8sNamespaceType.BuiltInName] = (alias, scope, features, sourceFileKind) => K8sNamespaceType.Create(alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(string providerName, string aliasName, ResourceScope resourceScope, IFeatureProvider features, BicepSourceFileKind sourceFileKind)
        => providerLookup.TryGetValue(providerName)?.Invoke(aliasName, resourceScope, features, sourceFileKind);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
