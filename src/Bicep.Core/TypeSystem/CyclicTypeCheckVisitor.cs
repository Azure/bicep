// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Utils;

namespace Bicep.Core.TypeSystem;

public sealed class CyclicTypeCheckVisitor : AstVisitor
{
    private readonly SemanticModel model;
    private readonly TypeAliasSymbol currentSymbol;
    private readonly List<SyntaxBase> declarationAccesses;
    private bool enteredTypeContainer = false;

    public static ImmutableDictionary<TypeAliasSymbol, ImmutableArray<TypeAliasSymbol>> FindCycles(SemanticModel model)
    {
        Dictionary<TypeAliasSymbol, IEnumerable<SyntaxBase>> declarationAccessDict = new();
        foreach (var typeAliasSymbol in model.Binder.FileSymbol.TypeDeclarations)
        {
            var visitor = new CyclicTypeCheckVisitor(model, typeAliasSymbol);
            visitor.VisitTypeDeclarationSyntax(typeAliasSymbol.DeclaringType);
            declarationAccessDict[typeAliasSymbol] = visitor.DeclarationAccesses;
        }

        return FindCycles(model, declarationAccessDict);
    }

    private static ImmutableDictionary<TypeAliasSymbol, ImmutableArray<TypeAliasSymbol>>
    FindCycles(SemanticModel model, IDictionary<TypeAliasSymbol, IEnumerable<SyntaxBase>> declarationAccessDict)
    {
        var symbolGraph = declarationAccessDict
            .SelectMany(kvp => kvp.Value.Select(model.Binder.GetSymbolInfo).OfType<TypeAliasSymbol>().Select(x => (kvp.Key, x)))
            .ToLookup(x => x.Item1, x => x.Item2);

        return CycleDetector<TypeAliasSymbol>.FindCycles(symbolGraph);
    }

    private CyclicTypeCheckVisitor(SemanticModel model, TypeAliasSymbol currentSymbol)
    {
        this.model = model;
        this.currentSymbol = currentSymbol;
        declarationAccesses = new();
    }

    private IEnumerable<SyntaxBase> DeclarationAccesses => declarationAccesses;

    public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
    {
        // If this reference is not nested within a type container, it would have been detected based on syntax alone in CyclicCheckVisitor.
        // To avoid doubling up on diagnostics, skip recording cycles on top-level accesses
        if (enteredTypeContainer)
        {
            declarationAccesses.Add(syntax);
        }

        base.VisitVariableAccessSyntax(syntax);
    }

    public override void VisitArrayTypeSyntax(ArrayTypeSyntax syntax)
    {
        enteredTypeContainer = true;
        base.VisitArrayTypeSyntax(syntax);
    }

    public override void VisitArrayTypeMemberSyntax(ArrayTypeMemberSyntax syntax)
        => VisitContainedTypeSyntax(syntax, base.VisitArrayTypeMemberSyntax);

    public override void VisitObjectTypeSyntax(ObjectTypeSyntax syntax)
    {
        enteredTypeContainer = true;
        base.VisitObjectTypeSyntax(syntax);
    }

    public override void VisitObjectTypePropertySyntax(ObjectTypePropertySyntax syntax)
        => VisitContainedTypeSyntax(syntax, base.VisitObjectTypePropertySyntax);

    public override void VisitTupleTypeSyntax(TupleTypeSyntax syntax)
    {
        enteredTypeContainer = true;
        base.VisitTupleTypeSyntax(syntax);
    }

    public override void VisitTupleTypeItemSyntax(TupleTypeItemSyntax syntax)
        => VisitContainedTypeSyntax(syntax, base.VisitTupleTypeItemSyntax);

    private void VisitContainedTypeSyntax<TSyntax>(TSyntax syntax, Action<TSyntax> visitBaseFunc) where TSyntax : SyntaxBase
    {
        var containedType = model.GetTypeInfo(syntax);
        if (containedType is ErrorType)
        {
            // if the contained type could not be resolved, stop visiting
            return;
        }

        if (TypeHelper.TryRemoveNullability(containedType) is not null)
        {
            // if the contained type is nullable, any cycle would be *recursive*, not cyclic
            return;
        }

        visitBaseFunc(syntax);
    }
}
