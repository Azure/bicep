// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Resources;

namespace Bicep.Core.TypeSystem.Providers;

public abstract class ResourceTypeProviderBase
{
    protected readonly IEnumerable<ResourceTypeReference> availableResourceTypes;
    protected readonly Lazy<ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>>> typeReferencesByTypeLazy;

    public ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> TypeReferencesByType => typeReferencesByTypeLazy.Value;

    protected ResourceTypeProviderBase(IEnumerable<ResourceTypeReference> availableResourceTypes)
    {
        this.availableResourceTypes = availableResourceTypes;
        typeReferencesByTypeLazy = new(() => availableResourceTypes
            .GroupBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
            .ToImmutableDictionary(x => x.Key, x => x.ToImmutableArray(), StringComparer.OrdinalIgnoreCase));
    }
}
