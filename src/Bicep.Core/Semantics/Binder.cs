// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public class Binder : IBinder
    {
        private readonly SyntaxTree syntaxTree;
        private readonly ImmutableDictionary<SyntaxBase, Symbol> bindings;
        private readonly ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> cyclesBySymbol;

        public Binder(SyntaxTree syntaxTree, ISymbolContext symbolContext)
        {
            // TODO use lazy or some other pattern for init
            this.syntaxTree = syntaxTree;
            this.TargetScope = SyntaxHelper.GetTargetScope(syntaxTree);
            var (allDeclarations, outermostScopes) = GetAllDeclarations(syntaxTree, symbolContext);
            var uniqueDeclarations = GetUniqueDeclarations(allDeclarations);
            var builtInNamespacs = GetBuiltInNamespaces(this.TargetScope);
            this.bindings = GetBindings(syntaxTree, uniqueDeclarations, builtInNamespacs, outermostScopes);
            this.cyclesBySymbol = GetCyclesBySymbol(syntaxTree, uniqueDeclarations, this.bindings);

            // TODO: Avoid looping 5 times?
            this.FileSymbol = new FileSymbol(
                syntaxTree.FileUri.LocalPath,
                syntaxTree.ProgramSyntax,
                builtInNamespacs,
                outermostScopes,
                allDeclarations.OfType<ParameterSymbol>(),
                allDeclarations.OfType<VariableSymbol>(),
                allDeclarations.OfType<ResourceSymbol>(),
                allDeclarations.OfType<ModuleSymbol>(),
                allDeclarations.OfType<OutputSymbol>());
        }

        public ResourceScope TargetScope { get; }

        public FileSymbol FileSymbol { get; }

        public SyntaxBase? GetParent(SyntaxBase syntax)
            => syntaxTree.Hierarchy.GetParent(syntax);

        /// <summary>
        /// Returns the symbol that was bound to the specified syntax node. Will return null for syntax nodes that never get bound to symbols. Otherwise,
        /// a symbol will always be returned. Binding failures are represented with a non-null error symbol.
        /// </summary>
        /// <param name="syntax">the syntax node</param>
        public Symbol? GetSymbolInfo(SyntaxBase syntax) => this.bindings.TryGetValue(syntax);

        /// <summary>
        /// Returns all syntax nodes that represent a reference to the specified symbol. This includes the definitions of the symbol as well.
        /// Unusued declarations will return 1 result. Unused and undeclared symbols (functions, namespaces, for example) may return an empty list.
        /// </summary>
        /// <param name="symbol">The symbol</param>
        public IEnumerable<SyntaxBase> FindReferences(Symbol symbol) => this.bindings
            .Where(binding => ReferenceEquals(binding.Value, symbol))
            .Select(binding => binding.Key);

        public ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol)
            => this.cyclesBySymbol.TryGetValue(declaredSymbol, out var cycle) ? cycle : null;

        private static (ImmutableArray<DeclaredSymbol>, ImmutableArray<LocalScopeSymbol>) GetAllDeclarations(SyntaxTree syntaxTree, ISymbolContext symbolContext)
        {
            // collect declarations
            var declarations = new List<DeclaredSymbol>();
            var outermostScopes = new List<LocalScopeSymbol>();
            var declarationVisitor = new DeclarationVisitor(symbolContext, declarations, outermostScopes);
            declarationVisitor.Visit(syntaxTree.ProgramSyntax);

            return (declarations.ToImmutableArray(), outermostScopes.ToImmutableArray());
        }

        private static ImmutableDictionary<string, DeclaredSymbol> GetUniqueDeclarations(IEnumerable<DeclaredSymbol> allDeclarations)
        {
            // in cases of duplicate declarations we will see multiple declaration symbols in the result list
            // for simplicitly we will bind to the first one
            // it may cause follow-on type errors, but there will also be errors about duplicate identifiers as well
            return allDeclarations
                .ToLookup(x => x.Name, LanguageConstants.IdentifierComparer)
                .ToImmutableDictionary(x => x.Key, x => x.First(), LanguageConstants.IdentifierComparer);
        }

        private static ImmutableDictionary<string, NamespaceSymbol> GetBuiltInNamespaces(ResourceScope targetScope)
        {
            var namespaces = new NamespaceSymbol[] { new SystemNamespaceSymbol(), new AzNamespaceSymbol(targetScope) };

            return namespaces.ToImmutableDictionary(property => property.Name, property => property, LanguageConstants.IdentifierComparer);
        }

        private static ImmutableDictionary<SyntaxBase, Symbol> GetBindings(SyntaxTree syntaxTree, IReadOnlyDictionary<string, DeclaredSymbol> uniqueDeclarations, ImmutableDictionary<string, NamespaceSymbol> builtInNamespaces, ImmutableArray<LocalScopeSymbol> outermostScopes)
        {
            // bind identifiers to declarations
            var bindings = new Dictionary<SyntaxBase, Symbol>();
            var binder = new NameBindingVisitor(uniqueDeclarations, bindings, builtInNamespaces, outermostScopes);
            binder.Visit(syntaxTree.ProgramSyntax);

            return bindings.ToImmutableDictionary();
        }

        private static ImmutableDictionary<DeclaredSymbol, ImmutableArray<DeclaredSymbol>> GetCyclesBySymbol(SyntaxTree syntaxTree, IReadOnlyDictionary<string, DeclaredSymbol> uniqueDeclarations, IReadOnlyDictionary<SyntaxBase, Symbol> bindings)
        {
            return CyclicCheckVisitor.FindCycles(syntaxTree.ProgramSyntax, uniqueDeclarations, bindings);
        }
    }
}
