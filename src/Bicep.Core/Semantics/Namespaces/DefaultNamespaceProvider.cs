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
    private delegate NamespaceType GetNamespaceDelegate(
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

    public ResultWithDiagnostic<NamespaceType> TryGetNamespace(
        ResourceTypesProviderDescriptor descriptor,
        ResourceScope resourceScope,
        IFeatureProvider features,
        BicepSourceFileKind sourceFileKind)
    {
        // If we don't have a types path, we're loading a 'built-in' type
        if (descriptor.TypesBaseUri is null &&
            builtInNamespaceLookup.TryGetValue(descriptor.Name) is { } getProviderFn)
        {
            return new(getProviderFn(descriptor, resourceScope, features, sourceFileKind));
        }

        // dynamic types are not supported for namespaces other than 'az' for now
        if (descriptor.Name != AzNamespaceType.BuiltInName)
        {
            return new(x => x.UnrecognizedProvider(descriptor.Name));
        }

        if (!resourceTypeLoaderFactory.GetResourceTypeProviderFromFilePath(descriptor).IsSuccess(out var dynamicallyLoadedProvider, out var errorBuilder))
        {
            Trace.WriteLine($"Failed to load types from {descriptor.TypesBaseUri}");
            return new(errorBuilder);
        }
        
        return new(AzNamespaceType.Create(descriptor.Alias, resourceScope, dynamicallyLoadedProvider, sourceFileKind));
    }
}
