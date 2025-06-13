// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using ResourceScope = Bicep.Core.TypeSystem.ResourceScope;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public class ApiVersionProvider : IApiVersionProvider
    {
        private static StringComparer Comparer = LanguageConstants.ResourceTypeComparer;

        // One cache per target scope type
        private readonly Dictionary<ResourceScope, ApiVersionCache> _caches = new();
        private readonly IFeatureProvider features;
        private readonly IEnumerable<ResourceTypeReference> resourceTypeReferences;
        public ApiVersionProvider(IFeatureProvider features, IEnumerable<ResourceTypeReference> resourceTypeReferences)
        {
            this.features = features;
            this.resourceTypeReferences = resourceTypeReferences;
        }

        // for unit testing
        public void InjectTypeReferences(ResourceScope scope, IEnumerable<ResourceTypeReference> resourceTypeReferences)
        {
            var cache = GetCache(scope);
            Debug.Assert(!cache.typesCached, $"{nameof(InjectTypeReferences)} Types have already been cached for scope {scope}");
            cache.injectedTypes = [.. resourceTypeReferences];
        }

        private ApiVersionCache GetCache(ResourceScope scope)
        {
            switch (scope)
            {
                case ResourceScope.Tenant:
                case ResourceScope.ManagementGroup:
                case ResourceScope.Subscription:
                case ResourceScope.ResourceGroup:
                    break;
                default:
                    throw new ArgumentException($"Unexpected ResourceScope {scope}");
            }

            if (_caches.TryGetValue(scope, out ApiVersionCache? cache))
            {
                return cache;
            }
            else
            {
                var newCache = new ApiVersionCache();
                _caches[scope] = newCache;
                return newCache;
            }
        }

        private ApiVersionCache EnsureCached(ResourceScope scope)
        {
            var cache = GetCache(scope);
            if (cache.typesCached)
            {
                return cache;
            }
            cache.typesCached = true;
            var resourceTypesToCache = cache.injectedTypes ?? this.resourceTypeReferences;

            cache.CacheApiVersions(resourceTypesToCache);
            return cache;
        }

        public IEnumerable<string> GetResourceTypeNames(ResourceScope scope)
        {
            var cache = EnsureCached(scope);
            return cache.apiVersionsByResourceTypeName.Keys;
        }

        public IEnumerable<AzureResourceApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceType)
        {
            var cache = EnsureCached(scope);

            if (!cache.apiVersionsByResourceTypeName.Any())
            {
                throw new InvalidCastException($"ApiVersionProvider was unable to find any resource types for scope {scope}");
            }

            if (cache.apiVersionsByResourceTypeName.TryGetValue(fullyQualifiedResourceType, out List<string>? apiVersions))
            {
                return apiVersions.Select(AzureResourceApiVersion.Parse);
            }

            return [];
        }

        private class ApiVersionCache
        {
            public bool typesCached;
            public ResourceTypeReference[]? injectedTypes;

            public Dictionary<string, List<string>> apiVersionsByResourceTypeName = new(Comparer);

            public void CacheApiVersions(IEnumerable<ResourceTypeReference> resourceTypeReferences)
            {
                this.typesCached = true;

                foreach (var resourceTypeReference in resourceTypeReferences)
                {
                    if (resourceTypeReference.ApiVersion is string apiVersionString &&
                        AzureResourceApiVersion.TryParse(apiVersionString, out var apiVersion))
                    {
                        string fullyQualifiedType = resourceTypeReference.FormatType();
                        AddApiVersionToCache(apiVersionsByResourceTypeName, apiVersion /* suffix will have been lower-cased */, fullyQualifiedType);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid resource type and apiVersion found: {resourceTypeReference.FormatType()}");
                    }
                }

                // Sort the lists of api versions for each resource type
                apiVersionsByResourceTypeName = apiVersionsByResourceTypeName.ToDictionary(x => x.Key, x => x.Value.OrderBy(y => y).ToList(), Comparer);
            }

            private static void AddApiVersionToCache(Dictionary<string, List<string>> listOfTypes, string apiVersion, string fullyQualifiedType)
            {
                if (listOfTypes.TryGetValue(fullyQualifiedType, out List<string>? value))
                {
                    value.Add(apiVersion);
                    listOfTypes[fullyQualifiedType] = value;
                }
                else
                {
                    listOfTypes.Add(fullyQualifiedType, new List<string> { apiVersion });
                }
            }
        }
    }
}
