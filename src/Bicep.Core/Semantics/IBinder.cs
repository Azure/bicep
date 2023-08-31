// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System.Collections.Immutable;

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
    }
}
