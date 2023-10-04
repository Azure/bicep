// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface IBinder : ISyntaxHierarchy
    {
        ResourceScope TargetScope { get; }

        NamespaceResolver NamespaceResolver { get; }

        FileSymbol FileSymbol { get; }

        Symbol? GetSymbolInfo(SyntaxBase syntax);

        ImmutableDictionary<SyntaxBase, Symbol> Bindings { get; }

        ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol);

        ImmutableHashSet<DeclaredSymbol> GetSymbolsReferencedInDeclarationOf(DeclaredSymbol symbol);

        ImmutableHashSet<DeclaredSymbol> GetReferencedSymbolClosureFor(DeclaredSymbol symbol);
    }
}
