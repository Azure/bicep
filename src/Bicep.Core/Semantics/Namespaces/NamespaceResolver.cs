// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Resources;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Providers;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics.Namespaces
{
    public class NamespaceResolver
    {
        private readonly ImmutableDictionary<string, NamespaceType> namespaceTypes;
        public ImmutableDictionary<string, BuiltInNamespaceSymbol> ImplicitNamespaces { get; }

        public static NamespaceResolver Create(ImmutableArray<NamespaceResult> namespaceResults)
        {
            var namespaceTypes = ImmutableDictionary.CreateBuilder<string, NamespaceType>(LanguageConstants.IdentifierComparer);
            var implicitNamespaces = ImmutableDictionary.CreateBuilder<string, BuiltInNamespaceSymbol>(LanguageConstants.IdentifierComparer);

            foreach (var result in namespaceResults)
            {
                if (result.Origin is null)
                {
                    implicitNamespaces[result.Name] = new BuiltInNamespaceSymbol(result.Name, result.Type);
                }

                if (result.Type is NamespaceType namespaceType)
                {
                    namespaceTypes[result.Name] = namespaceType;
                    continue;
                }
            }

            return new(
                namespaceTypes.ToImmutable(),
                implicitNamespaces.ToImmutable());
        }

        private NamespaceResolver(ImmutableDictionary<string, NamespaceType> namespaceTypes, ImmutableDictionary<string, BuiltInNamespaceSymbol> implicitNamespaces)
        {
            this.namespaceTypes = namespaceTypes;
            this.ImplicitNamespaces = implicitNamespaces;
        }

        public IEnumerable<Symbol> ResolveUnqualifiedFunction(IdentifierSyntax identifierSyntax, bool includeDecorators)
        {
            // attempt to find function in all imported namespaces
            var symbols = this.namespaceTypes.Values
                .Select(type => includeDecorators
                    ? type.MethodResolver.TryGetSymbol(identifierSyntax) ?? type.DecoratorResolver.TryGetSymbol(identifierSyntax)
                    : type.MethodResolver.TryGetSymbol(identifierSyntax));

            foreach (var symbol in symbols)
            {
                // The caller is responsible for deduplicating this list and returning an error if there is ambiguity, so we simply return everything.
                if (symbol is not null)
                {
                    yield return symbol;
                }
            }
        }

        /// <summary>
        /// Attempt to find ambient type in all imported namespaces. As Namespaces are themselves ObjectTypes, their properties can only be types, not values.
        /// </summary>
        public IEnumerable<AmbientTypeSymbol> ResolveUnqualifiedTypeSymbol(IdentifierSyntax identifierSyntax) => this.namespaceTypes.Values
            .Select(@namespace => @namespace.Properties.TryGetValue(identifierSyntax.IdentifierName, out var found)
                ? new AmbientTypeSymbol(identifierSyntax.IdentifierName, found.TypeReference.Type, @namespace, found.Flags, found.Description)
                : null)
            .WhereNotNull();

        public IEnumerable<FunctionSymbol> GetKnownFunctions(string functionName, bool includeDecorators)
            => this.namespaceTypes.Values
                .Select(type => type.MethodResolver.TryGetFunctionSymbol(functionName) ??
                    (includeDecorators ? type.DecoratorResolver.TryGetDecoratorFunctionSymbol(functionName) : null))
                .OfType<FunctionSymbol>();

        public IEnumerable<string> GetKnownFunctionNames(bool includeDecorators)
            => this.namespaceTypes.Values
                .SelectMany(type => includeDecorators
                    ? type.MethodResolver.GetKnownFunctions().Keys.Concat(type.DecoratorResolver.GetKnownDecoratorFunctions().Keys)
                    : type.MethodResolver.GetKnownFunctions().Keys);

        public IEnumerable<string> GetKnownPropertyNames()
            => this.namespaceTypes.Values.SelectMany(type => type.Properties.Keys);

        public IEnumerable<AmbientTypeSymbol> GetKnownTypes() => this.namespaceTypes.Values
            .SelectMany(@namespace => @namespace.Properties.Select(p => new AmbientTypeSymbol(p.Key, p.Value.TypeReference.Type, @namespace, p.Value.Flags, p.Value.Description)));

        public IEnumerable<string> GetNamespaceNames()
            => this.namespaceTypes.Keys;

        public NamespaceType? TryGetNamespace(string name)
            => this.namespaceTypes.TryGetValue(name);

        public ImmutableArray<ResourceType> GetMatchingResourceTypes(ResourceTypeReference typeReference, ResourceTypeGenerationFlags flags)
        {
            var definedTypes = namespaceTypes.Values
                .Select(type => type.ResourceTypeProvider.TryGetDefinedType(type, typeReference, flags))
                .WhereNotNull()
                .ToImmutableArray();

            if (definedTypes.Any())
            {
                return definedTypes;
            }

            var fallbackTypes = namespaceTypes.Values
                .Select(type => type.ResourceTypeProvider.TryGenerateFallbackType(type, typeReference, flags))
                .WhereNotNull()
                .ToImmutableArray();

            return fallbackTypes;
        }

        public IEnumerable<ResourceTypeReference> GetAvailableAzureResourceTypes() =>
            namespaceTypes.Values.SingleOrDefault(x => x.Name.Equals(AzNamespaceType.BuiltInName, StringComparison.Ordinal))?.ResourceTypeProvider.GetAvailableTypes() ??
            [];

        public ILookup<string, ImmutableArray<ResourceTypeReference>> GetGroupedResourceTypes()
        {
            return namespaceTypes.Values
                .SelectMany(x => x.ResourceTypeProvider.TypeReferencesByType)
                .ToLookup(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
        }
    }
}
