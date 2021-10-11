// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;

namespace Bicep.Core.ApiVersion
{
    public class ApiVersionProvider
    {
        private Dictionary<string, List<DateTime>> previewVersions = new();
        private Dictionary<string, List<DateTime>> nonPreviewVersions = new();

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

            previewVersions = previewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y.Date).ToList());
            nonPreviewVersions = nonPreviewVersions.ToDictionary(x => x.Key, x => x.Value.OrderByDescending(y => y.Date).ToList());
        }

        private void UpdateCache(Dictionary<string, List<DateTime>> cache, ResourceTypeReference resourceTypeReference)
        {
            string apiVersion = resourceTypeReference.ApiVersion.Split("-preview").ElementAt(0);
            if (cache.TryGetValue(resourceTypeReference.FullyQualifiedType, out List<DateTime> value))
            {
                value.Add(DateTime.ParseExact(apiVersion, "yyyy-MM-dd", CultureInfo.InvariantCulture));
                cache[resourceTypeReference.FullyQualifiedType] = value;
            }
            else
            {
                cache.Add(resourceTypeReference.FullyQualifiedType, new List<DateTime> { DateTime.ParseExact(apiVersion, "yyyy-MM-dd", CultureInfo.InvariantCulture) });
            }
        }

        public DateTime? ConvertApiVersionToDateTime(string apiVersion)
        {
            if (apiVersion.Split("-preview") is string[] words &&
                words is not null)
            {
                return DateTime.ParseExact(words[0], "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            return null;
        }

        public DateTime? GetRecentApiVersionDate(string fullyQualifiedName, bool useNonApiVersionCache = true)
        {
            if (useNonApiVersionCache &&
                nonPreviewVersions.TryGetValue(fullyQualifiedName, out List<DateTime> nonPreviewVersionDates) &&
                nonPreviewVersionDates.Any())
            {
                return nonPreviewVersionDates.First();
            }
            else if (previewVersions.TryGetValue(fullyQualifiedName, out List<DateTime> previewVersionDates)
                && previewVersionDates.Any())
            {
                return previewVersionDates.First();
            }

            return null;
        }
    }
}
