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

            void TryAddBuiltInNamespace(string @namespace)
            {
                if (namespaceTypes.ContainsKey(@namespace))
                {
                    // we already have an imported namespace with this symbolic name
                    return;
                }

                if (namespaceProvider.TryGetNamespace(@namespace, @namespace, targetScope) is not { } namespaceType)
                {
                    // this namespace doesn't match a known built-in namespace
                    return;
                }

                if (namespaceTypes.Values.Any(x => LanguageConstants.IdentifierComparer.Equals(x.ProviderName, @namespace)))
                {
                    // the namespace has already been explicitly imported. don't register it as a built-in.
                    return;
                }

                var symbol = new BuiltInNamespaceSymbol(@namespace, namespaceType);
                builtInNamespaceSymbols[@namespace] = symbol;
                namespaceTypes = namespaceTypes.Add(@namespace, namespaceType);
            }

            TryAddBuiltInNamespace(SystemNamespaceType.BuiltInName);
            TryAddBuiltInNamespace(AzNamespaceType.BuiltInName);

            return new(namespaceTypes, builtInNamespaceSymbols.ToImmutableDictionary(LanguageConstants.IdentifierComparer));
        }

        public ImmutableDictionary<string, BuiltInNamespaceSymbol> BuiltIns { get; }

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

        public IEnumerable<string> GetKnownFunctionNames(bool includeDecorators)
            => this.namespaceTypes.Values
                .SelectMany(type => includeDecorators
                    ? type.MethodResolver.GetKnownFunctions().Keys.Concat(type.DecoratorResolver.GetKnownDecoratorFunctions().Keys)
                    : type.MethodResolver.GetKnownFunctions().Keys);

        public IEnumerable<string> GetKnownPropertyNames()
            => this.namespaceTypes.Values.SelectMany(type => type.Properties.Keys);

        public IEnumerable<string> GetNamespaceNames()
            => this.namespaceTypes.Keys;

        public ResourceType GetResourceType(ResourceTypeReference reference, ResourceTypeGenerationFlags flags)
        {
            var definedTypes = namespaceTypes.Values
                .Select(type => type.ResourceTypeProvider.TryGetDefinedType(reference, flags))
                .WhereNotNull();

            if (definedTypes.FirstOrDefault() is {} definedType)
            {
                return definedType;
            }

            var generatedTypes = namespaceTypes.Values
                .Select(type => type.ResourceTypeProvider.TryGenerateDefaultType(reference, flags))
                .WhereNotNull();

            // Here we are assuming that one of the namespaces will always return at least one result with TryGenerateDefaultType.
            // This is a fair assumption at present, because the "az" namespace is always imported.
            return generatedTypes.First();
        }

        public bool HasResourceType(ResourceTypeReference reference)
            => namespaceTypes.Values.Any(type => type.ResourceTypeProvider.HasDefinedType(reference));

        public IEnumerable<ResourceTypeReference> GetAvailableResourceTypes()
        {
            // Here we are not handling any deduplication between namespaces. This is OK for now, because there
            // are only two supported namespaces ("az" & "sys"), both singletons. "sys" does not contain any resource types.
            return namespaceTypes.Values.SelectMany(type => type.ResourceTypeProvider.GetAvailableTypes());
        }
    }
}
