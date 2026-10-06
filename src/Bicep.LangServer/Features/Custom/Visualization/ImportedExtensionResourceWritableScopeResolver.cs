// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Resources;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// Resolves writable scopes for imported extension types one at a time. This includes imported Azure type packages;
    /// the built-in Azure catalog uses <see cref="BuiltInAzureResourceWritableScopeResolver"/> to avoid materializing every type.
    /// </summary>
    internal sealed class ImportedExtensionResourceWritableScopeResolver : IResourceWritableScopeResolver
    {
        private readonly IResourceTypeProvider typeProvider;

        private readonly NamespaceType declaringNamespace;

        public ImportedExtensionResourceWritableScopeResolver(IResourceTypeProvider typeProvider, NamespaceType declaringNamespace)
        {
            this.typeProvider = typeProvider;
            this.declaringNamespace = declaringNamespace;
        }

        public ImmutableDictionary<ResourceTypeReference, ResourceScope> Resolve(IEnumerable<ResourceTypeReference> references)
        {
            var scopes = ImmutableDictionary.CreateBuilder<ResourceTypeReference, ResourceScope>();
            foreach (var reference in references.Distinct())
            {
                if (this.typeProvider.TryGetDefinedType(this.declaringNamespace, reference, ResourceTypeGenerationFlags.None) is { } type)
                {
                    scopes[reference] = type.ValidParentScopes & ~type.ReadOnlyScopes;
                }
            }

            return scopes.ToImmutable();
        }
    }
}
