// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Providers.Az;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Metadata
{
    // Represents a resource that is declared with Bicep code.
    public record DeclaredResourceMetadata(
        ResourceType Type,
        bool IsExistingResource,
        ResourceSymbol Symbol,
        ResourceMetadataParent? Parent)
        : ResourceMetadata(Type, IsExistingResource)
    {
        private readonly ImmutableDictionary<string, SyntaxBase> UniqueIdentifiers = GetUniqueIdentifiers(Type, Symbol);

        public SyntaxBase NameSyntax => TryGetNameSyntax() ??
            throw new InvalidOperationException($"Failed to find a 'name' property for resource '{Symbol.Name}'");

        public SyntaxBase? TryGetNameSyntax() => this.IsAzResourceType() ? this.UniqueIdentifiers.TryGetValue(AzResourceTypeProvider.ResourceNamePropertyName) : null;

        public SyntaxBase? TryGetScopeSyntax() => this.IsAzResourceType() ? UniqueIdentifiers.TryGetValue(LanguageConstants.ResourceScopePropertyName) : null;

        private static ImmutableDictionary<string, SyntaxBase> GetUniqueIdentifiers(ResourceType type, ResourceSymbol symbol)
        {
            if (symbol.DeclaringResource.TryGetBody() is not { } bodySyntax)
            {
                return [];
            }

            var identifiersBuilder = ImmutableDictionary.CreateBuilder<string, SyntaxBase>(LanguageConstants.IdentifierComparer);
            foreach (var propertySyntax in bodySyntax.Properties)
            {
                if (propertySyntax.TryGetKeyText() is { } propertyKey &&
                    type.UniqueIdentifierProperties.Contains(propertyKey))
                {
                    identifiersBuilder[propertyKey] = propertySyntax.Value;
                }
            }

            return identifiersBuilder.ToImmutable();
        }

        private bool IsAzResourceType() => LanguageConstants.IdentifierComparer.Equals(this.Type.DeclaringNamespace.ExtensionName, AzNamespaceType.BuiltInName);
    }
}
