// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics.Namespaces;

public class DefaultNamespaceProvider : INamespaceProvider
{
    private delegate NamespaceType? GetNamespaceDelegate(
        ResourceTypesProviderDescriptor descriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind);

    private readonly ImmutableDictionary<string, GetNamespaceDelegate> builtInNamespaceLookup;
    private readonly IResourceTypeProviderFactory resourceTypeLoaderFactory;

    public DefaultNamespaceProvider(IResourceTypeProviderFactory resourceTypeLoaderFactory)
    {
        this.resourceTypeLoaderFactory = resourceTypeLoaderFactory;
        this.builtInNamespaceLookup = new Dictionary<string, GetNamespaceDelegate>
        {
            [AzNamespaceType.BuiltInName] = (descriptor, resourceScope, _, sourceFileKind) => AzNamespaceType.Create(descriptor.Alias, resourceScope, resourceTypeLoaderFactory.GetBuiltInAzResourceTypesProvider(), sourceFileKind),
            [SystemNamespaceType.BuiltInName] = (descriptor, _, features, sourceFileKind) => SystemNamespaceType.Create(descriptor.Alias, features, sourceFileKind),
            [K8sNamespaceType.BuiltInName] = (descriptor, _, _, _) => K8sNamespaceType.Create(descriptor.Alias),
            [MicrosoftGraphNamespaceType.BuiltInName] = (descriptor, _, _, _) => MicrosoftGraphNamespaceType.Create(descriptor.Alias),
        }.ToImmutableDictionary();
    }

    public NamespaceType? TryGetNamespace(
        ResourceTypesProviderDescriptor descriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind)
    {
        // If we don't have a types path, we're loading a 'built-in' type
        if (descriptor.TypesBaseUri is null &&
            builtInNamespaceLookup.TryGetValue(descriptor.Name) is {} getProvider)
        {
            return getProvider(descriptor, resourceScope, features, sourceFileKind);
        }

        // Special-case the 'az' provider being loaded from registry - we need add-on functionality delivered via the namespace provider
        if (descriptor.Name == AzNamespaceType.BuiltInName)
        {
            if (resourceTypeLoaderFactory.GetResourceTypeProviderFromFilePath(descriptor).IsSuccess(out var dynamicallyLoadedProvider, out var errorBuilder))
            {
                return AzNamespaceType.Create(descriptor.Alias, resourceScope, dynamicallyLoadedProvider, sourceFileKind);
            }

            Trace.WriteLine($"Failed to load types from {descriptor.TypesBaseUri}: {errorBuilder(DiagnosticBuilder.ForDocumentStart())}");
            return null;
        }

        // TODO: return the 3rd party provider namespace type here
        return null;
    }
}
