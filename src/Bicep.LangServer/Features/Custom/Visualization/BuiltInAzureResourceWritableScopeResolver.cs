// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using Azure.Bicep.Types;
using Azure.Bicep.Types.Az;
using Azure.Bicep.Types.Serialization;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using SerializedResourceType = Azure.Bicep.Types.Concrete.ResourceType;
using SerializedScopeType = Azure.Bicep.Types.Concrete.ScopeType;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Resolves writable scopes of the built-in Azure resource types straight from their serialized type files.
    /// </summary>
    /// <remarks>
    /// Only the palette needs scopes for the whole catalog at once; compilation reads them just for declared resources.
    /// Materializing every type is too slow for that (tens of seconds), because each load deserializes the type's whole
    /// file. This resolver deserializes each file once, in parallel, and reads only the scope flags. It derives from
    /// <see cref="AzTypeLoader"/> because <see cref="TypeLoader"/> exposes file content only to subclasses.
    /// </remarks>
    internal sealed class BuiltInAzureResourceWritableScopeResolver : AzTypeLoader, IResourceWritableScopeResolver
    {
        public static readonly Lazy<BuiltInAzureResourceWritableScopeResolver> Instance = new(() => new(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly ImmutableDictionary<string, CrossFileTypeReference> locations;

        private BuiltInAzureResourceWritableScopeResolver()
        {
            this.locations = this.LoadTypeIndex().Resources.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase);
        }

        public ImmutableDictionary<ResourceTypeReference, ResourceScope> Resolve(IEnumerable<ResourceTypeReference> references)
        {
            var locatedReferences = new List<(ResourceTypeReference Reference, CrossFileTypeReference Location)>();
            foreach (var reference in references.Distinct())
            {
                if (this.locations.TryGetValue(reference.FormatName(), out var location))
                {
                    locatedReferences.Add((reference, location));
                }
            }

            var scopes = new ConcurrentDictionary<ResourceTypeReference, ResourceScope>();
            Parallel.ForEach(locatedReferences.GroupBy(entry => entry.Location.RelativePath, StringComparer.Ordinal), file =>
            {
                using var stream = this.GetContentStreamAtPath(file.Key);
                var types = TypeSerializer.Deserialize(stream);

                foreach (var (reference, location) in file)
                {
                    if (location.Index >= 0 && location.Index < types.Length && types[location.Index] is SerializedResourceType resourceType)
                    {
                        scopes[reference] = ToResourceScope(resourceType.WritableScopes);
                    }
                }
            });

            return scopes.ToImmutableDictionary();
        }

        // Mirrors AzResourceTypeFactory's private conversion so this reader agrees with materialized types.
        private static ResourceScope ToResourceScope(SerializedScopeType scopes)
        {
            if (scopes == SerializedScopeType.All)
            {
                return ResourceScope.Tenant | ResourceScope.ManagementGroup | ResourceScope.Subscription | ResourceScope.ResourceGroup | ResourceScope.Resource;
            }

            var output = ResourceScope.None;
            output |= scopes.HasFlag(SerializedScopeType.Extension) ? ResourceScope.Resource : ResourceScope.None;
            output |= scopes.HasFlag(SerializedScopeType.Tenant) ? ResourceScope.Tenant : ResourceScope.None;
            output |= scopes.HasFlag(SerializedScopeType.ManagementGroup) ? ResourceScope.ManagementGroup : ResourceScope.None;
            output |= scopes.HasFlag(SerializedScopeType.Subscription) ? ResourceScope.Subscription : ResourceScope.None;
            output |= scopes.HasFlag(SerializedScopeType.ResourceGroup) ? ResourceScope.ResourceGroup : ResourceScope.None;
            return output;
        }
    }
}
