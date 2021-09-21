// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Resources;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Semantics.Namespaces
{
    public class NamespaceResolver
    {
        private readonly ImmutableDictionary<string, NamespaceType> namespaceTypes;

        private NamespaceResolver(ImmutableDictionary<string, NamespaceType> namespaceTypes, ImmutableDictionary<string, BuiltInNamespaceSymbol> builtIns)
        {
            this.namespaceTypes = namespaceTypes;
            this.BuiltIns = builtIns;
        }

        public static NamespaceResolver Create(INamespaceProvider namespaceProvider, ResourceScope targetScope, IEnumerable<ImportedNamespaceSymbol> importedNamespaces)
        {
            var builtInNamespaceSymbols = new Dictionary<string, BuiltInNamespaceSymbol>(LanguageConstants.IdentifierComparer);
            var namespaceTypes = importedNamespaces
                .Select(x => x.DeclaredType)
                .OfType<NamespaceType>()
                .ToImmutableDictionary(x => x.Name, LanguageConstants.IdentifierComparer);

            if (!namespaceTypes.Values.Any(x => LanguageConstants.IdentifierComparer.Equals(x.ProviderName, SystemNamespaceType.BuiltInName)) &&
                namespaceProvider.TryGetNamespace(SystemNamespaceType.BuiltInName, SystemNamespaceType.BuiltInName, targetScope) is { } sysNamespaceType)
            {
                var symbol = new BuiltInNamespaceSymbol(SystemNamespaceType.BuiltInName, sysNamespaceType);
                builtInNamespaceSymbols[symbol.Name] = symbol;
                namespaceTypes = namespaceTypes.Add(symbol.Name, sysNamespaceType);
            }

            if (!namespaceTypes.Values.Any(x => LanguageConstants.IdentifierComparer.Equals(x.ProviderName, AzNamespaceType.BuiltInName)) &&
                namespaceProvider.TryGetNamespace(AzNamespaceType.BuiltInName, AzNamespaceType.BuiltInName, targetScope) is { } azNamespaceType)
            {
                var symbol = new BuiltInNamespaceSymbol(AzNamespaceType.BuiltInName, azNamespaceType);
                builtInNamespaceSymbols[symbol.Name] = symbol;
                namespaceTypes = namespaceTypes.Add(symbol.Name, azNamespaceType);
            }

            return new(namespaceTypes, builtInNamespaceSymbols.ToImmutableDictionary(LanguageConstants.IdentifierComparer));
        }

        public ImmutableDictionary<string, BuiltInNamespaceSymbol> BuiltIns { get; }

        public IEnumerable<Symbol> ResolveGlobalFunction(IdentifierSyntax identifierSyntax, bool includeDecorators)
        {
            // attempt to find function in all imported namespaces
            var symbols = this.namespaceTypes.Values
                .Select(type => includeDecorators
                    ? type.MethodResolver.TryGetSymbol(identifierSyntax) ?? type.DecoratorResolver.TryGetSymbol(identifierSyntax)
                    : type.MethodResolver.TryGetSymbol(identifierSyntax));

            foreach (var symbol in symbols)
            {
                if (symbol is not null)
                {
                    yield return symbol;
                }
            }
        }

        public IEnumerable<string> GetKnownFunctionNames(bool includeDecorators)
        {
            return this.namespaceTypes.Values
                .SelectMany(type => includeDecorators
                    ? type.MethodResolver.GetKnownFunctions().Keys.Concat(type.DecoratorResolver.GetKnownDecoratorFunctions().Keys)
                    : type.MethodResolver.GetKnownFunctions().Keys);
        }

        public IEnumerable<string> GetKnownPropertyNames()
        {
            return this.namespaceTypes.Values.SelectMany(type => type.Properties.Keys);
        }

        public IEnumerable<string> GetNamespaceNames()
        {
            return this.namespaceTypes.Keys;
        }

        public ResourceType GetResourceType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            var resourceTypes = namespaceTypes.Values
                .Select(type => type.ResourceTypeProvider.TryGetType(reference, flags))
                .WhereNotNull();

            // The az provider is currently always imported, and will always return a result
            // So it's safe to use 'First' here.
            return resourceTypes.First();
        }

        public bool HasResourceType(ResourceTypeReference reference)
            => namespaceTypes.Values.Any(type => type.ResourceTypeProvider.HasType(reference));

        public IEnumerable<ResourceTypeReference> GetAvailableResourceTypes()
            => namespaceTypes.Values.SelectMany(type => type.ResourceTypeProvider.GetAvailableTypes());
    }
}
