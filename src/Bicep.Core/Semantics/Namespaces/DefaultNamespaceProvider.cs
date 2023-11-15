// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProvider : INamespaceProvider
{
    private delegate NamespaceType? GetNamespaceDelegate(
        ResourceTypesProviderDescriptor typesProviderDescriptor,
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
            [AzNamespaceType.BuiltInName] = TryCreateAzNamespace,
            [SystemNamespaceType.BuiltInName] = (providerDescriptor, resourceScope, features, sourceFileKind) => SystemNamespaceType.Create(providerDescriptor.Alias, features, sourceFileKind),
            [K8sNamespaceType.BuiltInName] = (providerDescriptor, resourceScope, features, sourceFileKind) => K8sNamespaceType.Create(providerDescriptor.Alias),
            [MicrosoftGraphNamespaceType.BuiltInName] = (providerDescriptor, resourceScope, features, sourceFileKind) => MicrosoftGraphNamespaceType.Create(providerDescriptor.Alias),
        }.ToImmutableDictionary();
    }

    private NamespaceType? TryCreateAzNamespace(ResourceTypesProviderDescriptor providerDescriptor, ResourceScope scope, IFeatureProvider features, BicepSourceFileKind sourceFileKind)
    {
        if (!features.DynamicTypeLoadingEnabled)
        {
            providerDescriptor = new(AzNamespaceType.BuiltInName, AzNamespaceType.Settings.ArmTemplateProviderVersion);
        }

        if (resourceTypeLoaderFactory.GetResourceTypeProvider(providerDescriptor, features).IsSuccess(out var dynamicallyLoadedProvider, out var errorBuilder))
        {
            return AzNamespaceType.Create(providerDescriptor.Alias, scope, dynamicallyLoadedProvider, sourceFileKind);
        }

        Trace.WriteLine($"Failed to load types from {providerDescriptor.Path}: {errorBuilder(DiagnosticBuilder.ForPosition(providerDescriptor.Span))}");
        return null;
    }

    public NamespaceType? TryGetNamespace(
        ResourceTypesProviderDescriptor typesProviderDescriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind)
        => providerLookup.TryGetValue(typesProviderDescriptor.Name)?
        .Invoke(
            typesProviderDescriptor,
            resourceScope,
            features,
            sourceFileKind);

    public IEnumerable<string> AvailableNamespaces
        => providerLookup.Keys;
}
