// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Semantics;
using Bicep.Core.TypeSystem;

namespace Bicep.LanguageServer.Utils;

public class ActiveScopesVisitor : SymbolVisitor
{
    private readonly int offset;

    private ActiveScopesVisitor(int offset)
    {
        this.offset = offset;
    }

    public override void VisitFileSymbol(FileSymbol symbol)
    {
        // global scope is always active
        this.ActiveScopes.Add(symbol);

        base.VisitFileSymbol(symbol);
    }

    public override void VisitLocalScope(LocalScope symbol)
    {
        // use binding syntax because this is used to find accessible symbols
        // in a child scope
        if (symbol.BindingSyntax.Span.ContainsInclusive(this.offset))
        {
            // the offset is inside the binding scope
            // this scope is active
            this.ActiveScopes.Add(symbol);

            // visit children to find more active scopes within
            base.VisitLocalScope(symbol);
        }
    }

    private IList<ILanguageScope> ActiveScopes { get; } = new List<ILanguageScope>();

    public static ImmutableArray<ILanguageScope> GetActiveScopes(FileSymbol file, int offset)
    {
        var visitor = new ActiveScopesVisitor(offset);
        visitor.Visit(file);

        return [.. visitor.ActiveScopes];
    }
}
