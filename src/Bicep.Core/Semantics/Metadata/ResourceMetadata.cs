// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.TypeSystem.Az;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Semantics.Metadata
{
    public record ResourceMetadata(
        ResourceType Type,
        ResourceSymbol Symbol,
        ResourceMetadataParent? Parent,
        bool IsExistingResource)
    {
        private readonly ImmutableDictionary<string, SyntaxBase> UniqueIdentifiers = GetUniqueIdentifiers(Type, Symbol);

        public ResourceTypeReference TypeReference => Type.TypeReference;

        public SyntaxBase NameSyntax => TryGetNameSyntax() ?? 
            throw new InvalidOperationException($"Failed to find a 'name' property for resource '{Symbol.Name}'");

        public SyntaxBase? TryGetNameSyntax() => UniqueIdentifiers.TryGetValue(AzResourceTypeProvider.ResourceNamePropertyName);

        public SyntaxBase? TryGetScopeSyntax() => UniqueIdentifiers.TryGetValue(LanguageConstants.ResourceScopePropertyName);

        public bool IsAzResource => Type.DeclaringNamespace.ProviderNameEquals(AzNamespaceType.BuiltInName);

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
