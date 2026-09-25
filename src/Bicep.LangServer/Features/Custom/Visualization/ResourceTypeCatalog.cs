// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Globalization;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// The resource palette's view of a resource type catalog: each type at its default API version, optionally limited
    /// to the types that can be deployed at a target scope.
    /// </summary>
    internal sealed class ResourceTypeCatalog
    {
        private readonly string id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);

        private readonly ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> versionsByType;

        private readonly Lazy<ImmutableArray<VisualResourceTypeCatalogEntry>> defaultTypes;

        private readonly Lazy<ImmutableDictionary<ResourceTypeReference, ResourceScope>> writableScopes;

        private readonly ConcurrentDictionary<ResourceScope, ImmutableArray<VisualResourceTypeCatalogEntry>> filteredTypes = new();

        public ResourceTypeCatalog(
            ImmutableDictionary<string, ImmutableArray<ResourceTypeReference>> versionsByType,
            IResourceWritableScopeResolver scopeResolver)
        {
            this.versionsByType = versionsByType;
            this.defaultTypes = new(this.SelectDefaultVersions, LazyThreadSafetyMode.ExecutionAndPublication);

            // Reading scopes can touch every type file, so it waits for the first query that filters by scope and is
            // shared by every scope.
            this.writableScopes = new(
                () => scopeResolver.Resolve(this.defaultTypes.Value.Select(ToReference)),
                LazyThreadSafetyMode.ExecutionAndPublication);
        }

        /// <summary>
        /// The catalog identity includes the target scope, so a scope change reads as a new catalog to the client: stale
        /// responses are discarded and version choices made for the previous scope are reset.
        /// </summary>
        public string GetId(ResourceScope? scope) => scope is { } targetScope ? $"{this.id}-{targetScope}" : this.id;

        public ImmutableArray<VisualResourceTypeNamespace> GetNamespaces(ResourceScope? scope) =>
            [.. this.GetResourceTypes(scope)
                .GroupBy(type => GetProviderNamespace(type.FullyQualifiedType), StringComparer.OrdinalIgnoreCase)
                .Select(group => new VisualResourceTypeNamespace(group.Key, group.Count()))
                .OrderBy(providerNamespace => providerNamespace.Name, StringComparer.OrdinalIgnoreCase)];

        /// <summary>All types, sorted by name.</summary>
        public ImmutableArray<VisualResourceTypeCatalogEntry> GetResourceTypes(ResourceScope? scope) =>
            scope is { } targetScope
                ? this.filteredTypes.GetOrAdd(targetScope, scope => [.. this.defaultTypes.Value.Where(type => this.IsDeployableAt(type, scope))])
                : this.defaultTypes.Value;

        public ImmutableArray<VisualResourceTypeCatalogEntry> GetResourceTypes(string providerNamespace, ResourceScope? scope) =>
            [.. this.GetResourceTypes(scope)
                .Where(type => string.Equals(GetProviderNamespace(type.FullyQualifiedType), providerNamespace, StringComparison.OrdinalIgnoreCase))];

        public ImmutableArray<VisualResourceTypeCatalogEntry> Search(string query, ResourceScope? scope) =>
            [.. this.GetResourceTypes(scope)
                .Where(type => type.FullyQualifiedType.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase))];

        /// <summary>Every API version of a type, newest first. Versions are not filtered by scope.</summary>
        public ImmutableArray<string> GetApiVersions(string fullyQualifiedType)
        {
            if (string.IsNullOrWhiteSpace(fullyQualifiedType) || !this.versionsByType.TryGetValue(fullyQualifiedType, out var references))
            {
                throw new VisualResourceCreationException($"Resource type \"{fullyQualifiedType}\" was not found.");
            }

            return ResourceApiVersionPolicy.SortNewestFirst(references.Select(reference => reference.ApiVersion));
        }

        // A type whose scopes could not be read is kept rather than silently hidden.
        private bool IsDeployableAt(VisualResourceTypeCatalogEntry type, ResourceScope targetScope) =>
            !this.writableScopes.Value.TryGetValue(ToReference(type), out var writableScopes) || writableScopes.HasFlag(targetScope);

        private ImmutableArray<VisualResourceTypeCatalogEntry> SelectDefaultVersions()
        {
            var types = new List<VisualResourceTypeCatalogEntry>();
            foreach (var (type, references) in this.versionsByType)
            {
                if (type.IndexOf('/') > 0 &&
                    ResourceApiVersionPolicy.SelectDefault(references.Select(reference => reference.ApiVersion)) is { } apiVersion)
                {
                    types.Add(new(type, apiVersion, ResourceApiVersionPolicy.IsPreview(apiVersion)));
                }
            }

            return [.. types.OrderBy(type => type.FullyQualifiedType, StringComparer.OrdinalIgnoreCase)];
        }

        private static string GetProviderNamespace(string fullyQualifiedType) => fullyQualifiedType[..fullyQualifiedType.IndexOf('/')];

        private static ResourceTypeReference ToReference(VisualResourceTypeCatalogEntry type) => new(type.FullyQualifiedType, type.ApiVersion);
    }
}
