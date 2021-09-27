// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;

namespace Bicep.Core.Semantics
{
    public class Binder : IBinder
    {
        private readonly BicepFile bicepFile;
        private readonly ImmutableDictionary<SyntaxBase, Symbol> bindings;
        private readonly ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol;

        public Binder(INamespaceProvider namespaceProvider, BicepFile bicepFile, ISymbolContext symbolContext)
        {
            // TODO use lazy or some other pattern for init
            this.bicepFile = bicepFile;
            this.TargetScope = SyntaxHelper.GetTargetScope(bicepFile);
            var (declarations, outermostScopes) = DeclarationVisitor.GetDeclarations(namespaceProvider, TargetScope, bicepFile, symbolContext);
            var uniqueDeclarations = GetUniqueDeclarations(declarations);
            this.NamespaceResolver = GetNamespaceResolver(namespaceProvider, this.TargetScope, uniqueDeclarations);
            this.bindings = NameBindingVisitor.GetBindings(bicepFile.ProgramSyntax, uniqueDeclarations, NamespaceResolver, outermostScopes);
            this.cyclesBySymbol = GetCyclesBySymbol(bicepFile, this.bindings);

            this.FileSymbol = new FileSymbol(
                bicepFile.FileUri.LocalPath,
                bicepFile.ProgramSyntax,
                NamespaceResolver,
                outermostScopes,
                declarations,
                bicepFile.FileUri);
        }

        public ResourceScope TargetScope { get; }

        public FileSymbol FileSymbol { get; }

        public NamespaceResolver NamespaceResolver { get; }

        public SyntaxBase? GetParent(SyntaxBase syntax)
            => bicepFile.Hierarchy.GetParent(syntax);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.bindings.TryGetValue(syntax);

        public ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol)
            => this.cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle) ? cycle : null;

        private static ImmutableDictionary<string, DeclaredSymbol> GetUniqueDeclarations(IEnumerable<DeclaredSymbol> outermostDeclarations)
        {
            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            return outermostDeclarations
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        private static NamespaceResolver GetNamespaceResolver(INamespaceProvider namespaceProvider, ResourceScope targetScope, ImmutableDictionary<string, DeclaredSymbol> uniqueDeclarations)
        {
            var importedNamespaces = uniqueDeclarations.Values.OfType<ImportedNamespaceSymbol>();

            return NamespaceResolver.Create(namespaceProvider, targetScope, importedNamespaces);
        }

        private static ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> GetCyclesBySymbol(BicepFile bicepFile, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            return CyclicCheckVisitor.FindCycles(bicepFile.ProgramSyntax, bindings);
        }
    }
}
