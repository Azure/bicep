// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Binder : IBinder
    {
        private readonly BicepSourceFile bicepFile;
        private readonly ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol;

        public Binder(INamespaceProvider namespaceProvider, IFeatureProvider features, BicepSourceFile sourceFile, ISymbolContext symbolContext)
        {
            // TODO use lazy or some other pattern for init
            this.bicepFile = sourceFile;
            this.TargetScope = SyntaxHelper.GetTargetScope(sourceFile);
            var (declarations, outermostScopes) = DeclarationVisitor.GetDeclarations(namespaceProvider, features, TargetScope, sourceFile, symbolContext);
            var uniqueDeclarations = GetUniqueDeclarations(declarations);
            this.NamespaceResolver = GetNamespaceResolver(features, namespaceProvider, sourceFile, this.TargetScope, uniqueDeclarations);
            this.Bindings = NameBindingVisitor.GetBindings(sourceFile.ProgramSyntax, uniqueDeclarations, NamespaceResolver, outermostScopes);
            this.cyclesBySymbol = CyclicCheckVisitor.FindCycles(sourceFile.ProgramSyntax, this.Bindings);

            this.FileSymbol = new FileSymbol(
                symbolContext,
                sourceFile,
                NamespaceResolver,
                outermostScopes,
                declarations);
        }

        public ResourceScope TargetScope { get; }

        public FileSymbol FileSymbol { get; }

        public NamespaceResolver NamespaceResolver { get; }

        public ImmutableDictionary<SyntaxBase, Symbol> Bindings { get; }

        public SyntaxBase? GetParent(SyntaxBase syntax)
            => bicepFile.Hierarchy.GetParent(syntax);

        public bool IsDescendant(SyntaxBase node, SyntaxBase potentialAncestor)
            => bicepFile.Hierarchy.IsDescendant(node, potentialAncestor);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.Bindings.TryGetValue(syntax);

        public ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol)
            => this.cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle) ? cycle : null;

        private static ImmutableDictionary<string, DeclaredSymbol> GetUniqueDeclarations(IEnumerable<DeclaredSymbol> outermostDeclarations)
        {
            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            return outermostDeclarations
                .OrderBy(x => x is not OutputSymbol && x is not MetadataSymbol ? 0 : 1)
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        private static NamespaceResolver GetNamespaceResolver(IFeatureProvider features, INamespaceProvider namespaceProvider, BicepSourceFile sourceFile, ResourceScope targetScope, ImmutableDictionary<string, DeclaredSymbol> uniqueDeclarations)
        {
            var importedNamespaces = uniqueDeclarations.Values.OfType<ImportedNamespaceSymbol>();

            return NamespaceResolver.Create(features, namespaceProvider, sourceFile, targetScope, importedNamespaces);
        }
    }
}
