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
    private readonly IResourceTypeProviderFactory resourceTypeLoaderFactory;

    public DefaultNamespaceProvider(IResourceTypeProviderFactory resourceTypeLoaderFactory)
    {
        this.resourceTypeLoaderFactory = resourceTypeLoaderFactory;
        this.providerLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [SystemNamespaceType.BuiltInName] = CreateSystemNamespace,
            [AzNamespaceType.BuiltInName] = CreateAzNamespace,
            [K8sNamespaceType.BuiltInName] = CreateK8sNamespace,
            [MicrosoftGraphNamespaceType.BuiltInName] = CreateMicrosoftGraphNamespace
        }.ToImmutableDictionary();
    }

    // Define delegate functions for each namespace type
    private readonly GetNamespaceDelegate CreateSystemNamespace = (providerDescriptor, resourceScope, features, sourceFileKind)
        => SystemNamespaceType.Create(providerDescriptor.Alias, features, sourceFileKind);

    private readonly GetNamespaceDelegate CreateK8sNamespace = (providerDescriptor, resourceScope, features, sourceFileKind)
        => K8sNamespaceType.Create(providerDescriptor.Alias);

    private readonly GetNamespaceDelegate CreateMicrosoftGraphNamespace = (providerDescriptor, resourceScope, features, sourceFileKind)
        => MicrosoftGraphNamespaceType.Create(providerDescriptor.Alias);


    private NamespaceType CreateAzNamespace(TypesProviderDescriptor providerDescriptor, ResourceScope scope, IFeatureProvider features, BicepSourceFileKind sourceFileKind)
    {
        IResourceTypeProvider provider = resourceTypeLoaderFactory.GetBuiltInAzResourceTypesProvider();

        if (features.DynamicTypeLoadingEnabled && resourceTypeLoaderFactory.GetResourceTypeProvider(providerDescriptor, features).IsSuccess(out var dynamicallyLoadedProvider))
        {
            provider = dynamicallyLoadedProvider;
        }

        return AzNamespaceType.Create(providerDescriptor.Alias, scope, provider, sourceFileKind);
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
