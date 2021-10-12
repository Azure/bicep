// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.ApiVersion
{
    public class ApiVersionProvider : IApiVersionProvider
    {
        private Dictionary<string, List<string>> previewVersions = new();
        private Dictionary<string, List<string>> nonPreviewVersions = new();

        public ApiVersionProvider()
        {
            CacheApiVersions();
        }

        private void CacheApiVersions()
        {
            DefaultNamespaceProvider defaultNamespaceProvider = new DefaultNamespaceProvider(new AzResourceTypeLoader(), new FeatureProvider());
            NamespaceResolver namespaceResolver = NamespaceResolver.Create(defaultNamespaceProvider, TypeSystem.ResourceScope.ResourceGroup, Enumerable.Empty<ImportedNamespaceSymbol>());
            IEnumerable<ResourceTypeReference> resourceTypeReferences = namespaceResolver.GetAvailableResourceTypes();

            foreach (var resourceTypeReference in resourceTypeReferences)
            {
                if (resourceTypeReference.ApiVersion.Contains("preview"))
                {
                    UpdateCache(previewVersions, resourceTypeReference);
                }
                else
                {
                    UpdateCache(nonPreviewVersions, resourceTypeReference);
                }
            }

            previewVersions = previewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
            nonPreviewVersions = nonPreviewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y).ToList());
        }

        private void UpdateCache(Dictionary<string, List<string>> cache, ResourceTypeReference resourceTypeReference)
        {
            string apiVersion = resourceTypeReference.ApiVersion.Split("-preview").ElementAt(0);
            if (cache.TryGetValue(resourceTypeReference.FullyQualifiedType, out List<string> value))
            {
                value.Add(apiVersion);
                cache[resourceTypeReference.FullyQualifiedType] = value;
            }
            else
            {
                cache.Add(resourceTypeReference.FullyQualifiedType, new List<string> { apiVersion });
            }
        }

        public string? GetRecentApiVersion(string fullyQualifiedName, bool useNonApiVersionCache = true)
        {
            if (useNonApiVersionCache)
            {
                if (nonPreviewVersions.TryGetValue(fullyQualifiedName, out List<string> nonPreviewVersionDates) &&
                    nonPreviewVersionDates.Any())
                {
                    return nonPreviewVersionDates.First();
                }
            }
            else if (previewVersions.TryGetValue(fullyQualifiedName, out List<string> previewVersionDates)
                && previewVersionDates.Any())
            {
                return previewVersionDates.First();
            }

            return null;
        }
    }
}
