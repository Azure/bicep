// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Comparers;

namespace Bicep.Core.Semantics;

internal class SymbolicReferenceCollector : AstVisitor
{
    private readonly ConcurrentDictionary<DeclaredSymbol, ImmutableSortedSet<SyntaxBase>.Builder> references = new();
    private readonly IBinder binder;

    private SymbolicReferenceCollector(IBinder binder)
    {
        this.binder = binder;
    }

    internal static ImmutableDictionary<DeclaredSymbol, ImmutableSortedSet<SyntaxBase>> CollectSymbolsReferenced(IBinder binder, SyntaxBase syntaxToSearch)
    {
        SymbolicReferenceCollector collector = new(binder);
        syntaxToSearch.Accept(collector);
        return collector.references.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutable());
    }

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        if (binder.GetSymbolInfo(syntax) is DeclaredSymbol signified)
        {
            references.GetOrAdd(signified, _ => ImmutableSortedSet.CreateBuilder<SyntaxBase>(SyntaxSourceOrderComparer.Instance))
                .Add(syntax);
        }
    }

    public override void VisitFunctionCallSyntax(FunctionCallSyntax syntax)
    {
        if (binder.GetSymbolInfo(syntax) is DeclaredSymbol signified)
        {
            references.GetOrAdd(signified, _ => ImmutableSortedSet.CreateBuilder<SyntaxBase>(SyntaxSourceOrderComparer.Instance))
                .Add(syntax);
        }

        base.VisitFunctionCallSyntax(syntax);
    }

    public override void VisitTypeVariableAccessSyntax(TypeVariableAccessSyntax syntax)
    {
        if (binder.GetSymbolInfo(syntax) is DeclaredSymbol signified)
        {
            references.GetOrAdd(signified, _ => ImmutableSortedSet.CreateBuilder<SyntaxBase>(SyntaxSourceOrderComparer.Instance))
                .Add(syntax);
        }
    }
}
