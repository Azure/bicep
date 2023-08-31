// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using System;
using System.Collections.Immutable;

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

        public SyntaxBase? TryGetNameSyntax() => UniqueIdentifiers.TryGetValue(AzResourceTypeProvider.ResourceNamePropertyName);

        public SyntaxBase? TryGetScopeSyntax() => UniqueIdentifiers.TryGetValue(LanguageConstants.ResourceScopePropertyName);

        private static ImmutableDictionary<string, SyntaxBase> GetUniqueIdentifiers(ResourceType type, ResourceSymbol symbol)
        {
            if (symbol.DeclaringResource.TryGetBody() is not { } bodySyntax)
            {
                return ImmutableDictionary<string, SyntaxBase>.Empty;
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
    }
}
