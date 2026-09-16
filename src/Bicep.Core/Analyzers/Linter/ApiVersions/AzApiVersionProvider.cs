// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.TypeSystem.Providers.Az;
using ResourceScope = Bicep.Core.TypeSystem.ResourceScope;

namespace Bicep.Core.Analyzers.Linter.ApiVersions;

public class AzApiVersionProvider(AzResourceTypeProvider AzResourceTypeProvider) : IApiVersionProvider
{
    private static readonly IComparer<AzureResourceApiVersion> ApiVersionComparer = Comparer<AzureResourceApiVersion>.Create((x, y) =>
    {
        var dateComparison = x.Date.CompareTo(y.Date);

        if (dateComparison != 0)
        {
            return dateComparison;
        }

        return StringComparer.Ordinal.Compare(x.Suffix, y.Suffix);
    });

    private readonly ConcurrentDictionary<(ResourceScope, string), ImmutableSortedSet<AzureResourceApiVersion>> cache = new();

    public ImmutableSortedSet<AzureResourceApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceType)
    {
        if (!AzResourceTypeProvider.TypeReferencesByType.TryGetValue(fullyQualifiedResourceType, out var resourceTypeReferences))
        {
            return [];
        }

        return cache.GetOrAdd((scope, fullyQualifiedResourceType), key =>
        {
            var apiVersions = ImmutableSortedSet.CreateBuilder(ApiVersionComparer);

            foreach (var resourceTypeReference in resourceTypeReferences)
            {
                if (resourceTypeReference.ApiVersion is { } apiVersionString &&
                    AzureResourceApiVersion.TryParse(apiVersionString, out var apiVersion))
                {
                    apiVersions.Add(apiVersion);
                }
            }

            return apiVersions.ToImmutable();
        });
    }
}
