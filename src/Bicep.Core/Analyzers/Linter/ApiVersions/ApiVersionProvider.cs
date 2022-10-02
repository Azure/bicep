// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using ResourceScope = Bicep.Core.TypeSystem.ResourceScope;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public class ApiVersionProvider : IApiVersionProvider
    {
        private static StringComparer Comparer = LanguageConstants.ResourceTypeComparer;

        // One cache per target scope type
        private readonly Dictionary<ResourceScope, ApiVersionCache> _caches = new();
        private readonly IFeatureProvider features;
        private readonly INamespaceProvider namespaceProvider;

        public ApiVersionProvider(IFeatureProvider features, INamespaceProvider namespaceProvider)
        {
            this.features = features;
            this.namespaceProvider = namespaceProvider;
        }

        // for unit testing
        public void InjectTypeReferences(ResourceScope scope, IEnumerable<ResourceTypeReference> resourceTypeReferences)
        {
            var cache = GetCache(scope);
            Debug.Assert(!cache.typesCached, $"{nameof(InjectTypeReferences)} Types have already been cached for scope {scope}");
            cache.injectedTypes = resourceTypeReferences.ToArray();
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

            IEnumerable<ResourceTypeReference> resourceTypeReferences;
            if (cache.injectedTypes is null)
            {
                NamespaceResolver namespaceResolver = NamespaceResolver.Create(features, namespaceProvider, scope, Enumerable.Empty<ImportedNamespaceSymbol>());
                resourceTypeReferences = namespaceResolver.GetAvailableResourceTypes();
            }
            else
            {
                resourceTypeReferences = cache.injectedTypes;
            }

            cache.CacheApiVersions(resourceTypeReferences);
            return cache;
        }

        public IEnumerable<string> GetResourceTypeNames(ResourceScope scope)
        {
            var cache = EnsureCached(scope);
            return cache.apiVersionsByResourceTypeName.Keys;
        }

        public IEnumerable<ApiVersion> GetApiVersions(ResourceScope scope, string fullyQualifiedResourceType)
        {
            var cache = EnsureCached(scope);

            if (!cache.apiVersionsByResourceTypeName.Any())
            {
                throw new InvalidCastException($"ApiVersionProvider was unable to find any resource types for scope {scope}");
            }

            if (cache.apiVersionsByResourceTypeName.TryGetValue(fullyQualifiedResourceType, out List<string>? apiVersions))
            {
                return apiVersions.Select((string version) => new ApiVersion(version));
            }

            return Enumerable.Empty<ApiVersion>();
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
                    var (apiVersion, suffix) =
                        resourceTypeReference.ApiVersion != null ?
                        ApiVersionHelper.TryParse(resourceTypeReference.ApiVersion) :
                        (null, null);
                    if (apiVersion is not null)
                    {
                        string fullyQualifiedType = resourceTypeReference.FormatType();
                        AddApiVersionToCache(apiVersionsByResourceTypeName, suffix == null ? apiVersion : (apiVersion + suffix) /* suffix will have been lower-cased */, fullyQualifiedType);
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid resource type and apiVersion found: {resourceTypeReference.FormatType()}");
                    }
                }

                // Sort the lists of api versions for each resource type
                apiVersionsByResourceTypeName = apiVersionsByResourceTypeName.ToDictionary(x => x.Key, x => x.Value.OrderBy(y => y).ToList(), Comparer);
            }

            private void AddApiVersionToCache(Dictionary<string, List<string>> listOfTypes, string apiVersion, string fullyQualifiedType)
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
