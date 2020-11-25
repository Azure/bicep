// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics
{
    public interface IBinder
    {
        ResourceScopeType TargetScope { get; }

        FileSymbol FileSymbol { get; }

        SyntaxBase? GetParent(SyntaxBase syntax);

        IEnumerable<SyntaxBase> FindReferences(Symbol symbol);

        Symbol? GetSymbolInfo(SyntaxBase syntax);

        ImmutableArray<DeclaredSymbol>? TryGetCycle(DeclaredSymbol declaredSymbol);
    }
}
