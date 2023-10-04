// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        private readonly ConcurrentDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> symbolsDirectlyReferencedInDeclarations = new();
        private readonly ConcurrentDictionary<DeclaredSymbol, ImmutableHashSet<DeclaredSymbol>> referencedSymbolClosures = new();
        private readonly Stack<DeclaredSymbol> closureCalculationStack = new();

        public Binder(INamespaceProvider namespaceProvider, IFeatureProvider features, BicepSourceFile sourceFile, ISymbolContext symbolContext)
        {
            // TODO use lazy or some other pattern for init
            this.bicepFile = sourceFile;
            this.TargetScope = SyntaxHelper.GetTargetScope(sourceFile);
            var fileScope = DeclarationVisitor.GetDeclarations(namespaceProvider, features, TargetScope, sourceFile, symbolContext);
            this.NamespaceResolver = GetNamespaceResolver(features, namespaceProvider, sourceFile, this.TargetScope, fileScope);
            this.Bindings = NameBindingVisitor.GetBindings(sourceFile.ProgramSyntax, NamespaceResolver, fileScope);
            this.cyclesBySymbol = CyclicCheckVisitor.FindCycles(sourceFile.ProgramSyntax, this.Bindings);

            this.FileSymbol = new FileSymbol(
                symbolContext,
                sourceFile,
                NamespaceResolver,
                fileScope);
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

        public ImmutableHashSet<DeclaredSymbol> GetSymbolsReferencedInDeclarationOf(DeclaredSymbol symbol)
            => symbolsDirectlyReferencedInDeclarations.GetOrAdd(symbol,
                s => SymbolicReferenceCollector.CollectSymbolsReferenced(this, s.DeclaringSyntax).Keys.ToImmutableHashSet());

        public ImmutableHashSet<DeclaredSymbol> GetReferencedSymbolClosureFor(DeclaredSymbol symbol)
            => referencedSymbolClosures.GetOrAdd(symbol, CalculateReferencedSymbolClosure);

        private ImmutableHashSet<DeclaredSymbol> CalculateReferencedSymbolClosure(DeclaredSymbol symbol)
        {
            closureCalculationStack.Push(symbol);

            var builder = ImmutableHashSet.CreateBuilder<DeclaredSymbol>();
            foreach (var symbolReferencedInDeclaration in GetSymbolsReferencedInDeclarationOf(symbol))
            {
                builder.Add(symbolReferencedInDeclaration);
                if (!closureCalculationStack.Contains(symbolReferencedInDeclaration))
                {
                    builder.UnionWith(GetReferencedSymbolClosureFor(symbolReferencedInDeclaration));
                }
            }

            closureCalculationStack.Pop();

            return builder.ToImmutable();
        }

        private static NamespaceResolver GetNamespaceResolver(IFeatureProvider features, INamespaceProvider namespaceProvider, BicepSourceFile sourceFile, ResourceScope targetScope, ILanguageScope fileScope)
        {
            return NamespaceResolver.Create(features, namespaceProvider, sourceFile, targetScope, fileScope);
        }
    }
}
