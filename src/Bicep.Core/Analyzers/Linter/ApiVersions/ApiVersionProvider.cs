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
        private readonly Dictionary<ResourceScope, ApiVersionCache> caches = new();
        private readonly IEnumerable<ResourceTypeReference> resourceTypeReferences;

        public ApiVersionProvider(IEnumerable<ResourceTypeReference> resourceTypeReferences)
        {
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

            if (caches.TryGetValue(scope, out ApiVersionCache? cache))
            {
                return cache;
            }
            else
            {
                var newCache = new ApiVersionCache();
                caches[scope] = newCache;
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

            if (cache.apiVersionsByResourceTypeName.TryGetValue(fullyQualifiedResourceType, out List<AzureResourceApiVersion>? apiVersions))
            {
                return apiVersions;
            }

            return [];
        }

        private class ApiVersionCache
        {
            private static readonly IComparer<AzureResourceApiVersion> ApiVersionComparer = System.Collections.Generic.Comparer<AzureResourceApiVersion>.Create((x, y) =>
            {
                var dateComparison = x.Date.CompareTo(y.Date);

                if (dateComparison != 0)
                {
                    return dateComparison;
                }

                return StringComparer.Ordinal.Compare(x.Suffix, y.Suffix);
            });

            public bool typesCached;
            public ResourceTypeReference[]? injectedTypes;

            public Dictionary<string, List<AzureResourceApiVersion>> apiVersionsByResourceTypeName = new(Comparer);

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

                // Sort versions once at cache build time so lookups can return pre-parsed values.
                foreach (var apiVersions in apiVersionsByResourceTypeName.Values)
                {
                    apiVersions.Sort(ApiVersionComparer);
                }
            }

            private static void AddApiVersionToCache(Dictionary<string, List<AzureResourceApiVersion>> listOfTypes, AzureResourceApiVersion apiVersion, string fullyQualifiedType)
            {
                if (!listOfTypes.TryGetValue(fullyQualifiedType, out var value))
                {
                    value = [];
                    listOfTypes[fullyQualifiedType] = value;
                }
                
                value.Add(apiVersion);
            }
        }
    }
}
