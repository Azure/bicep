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
        TypesProviderDescriptor typesProviderDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind);

    private readonly ImmutableDictionary<string, GetNamespaceDelegate> providerLookup;

    public DefaultNamespaceProvider(IResourceTypeLoaderFactory azResourceTypeLoaderFactory)
    {
        var builtInAzResourceTypeLoaderVersion = "1.0.0";
        var builtInAzResourceTypeProvider = new AzResourceTypeProvider(azResourceTypeLoaderFactory.GetBuiltInTypeLoader(), builtInAzResourceTypeLoaderVersion);

        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = (providerDescriptor, scope, features, sourceFileKind) => SystemNamespaceType.Create(providerDescriptor.Alias, features, sourceFileKind),
            [AzNamespaceType.BuiltInName] = (providerDescriptor, scope, features, sourceFileKind) =>
            {
                AzResourceTypeProvider? provider = builtInAzResourceTypeProvider;
                if (features.DynamicTypeLoadingEnabled && providerDescriptor.Version is not null)
                {
                    // TODO(asilverman): Current implementation of dynamic type loading needs to be refactored to handle caching of the provider to optimize 
                    // performance hit as a result of recreating the provider for each call to TryGetNamespace.
                    // Tracked by - https://msazure.visualstudio.com/One/_sprints/taskboard/Azure-ARM-Deployments/One/Custom/Azure-ARM/Gallium/Jul-2023?workitem=24563440
                    var loader = azResourceTypeLoaderFactory.GetResourceTypeLoader(providerDescriptor, features);
                    if (loader is null)
                    {
                        return null;
                    }
                    var overriddenProviderVersion = builtInAzResourceTypeLoaderVersion;
                    if (features.DynamicTypeLoadingEnabled)
                    {
                        overriddenProviderVersion = providerDescriptor.Version ?? overriddenProviderVersion;
                    }
                    provider = new AzResourceTypeProvider(loader, overriddenProviderVersion);
                }
                return AzNamespaceType.Create(providerDescriptor.Alias, scope, provider, sourceFileKind);
            },
            [K8sNamespaceType.BuiltInName] = (providerDescriptor, scope, features, sourceFileKind) => K8sNamespaceType.Create(providerDescriptor.Alias),
            [MicrosoftGraphNamespaceType.BuiltInName] = (providerDescriptor, scope, features, sourceFileKind) => MicrosoftGraphNamespaceType.Create(providerDescriptor.Alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(
        TypesProviderDescriptor typesProviderDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind)
    //TODO(asilverman): This is the location where we would like to add support for extensibility providers, we want to add a new key and a new loader for the ext. provider
        => providerLookup.TryGetValue(typesProviderDescriptor.Name)?.Invoke(typesProviderDescriptor, resourceScope, features, sourceFileKind);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
