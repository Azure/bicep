// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Semantics;

public class DeclaredFunctionSymbol : DeclaredSymbol, IFunctionSymbol
{
    public DeclaredFunctionSymbol(ISymbolContext context, string name, FunctionDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        this.overloadsLazy = new(() => GetFunctionOverload() is { } overload ?
            ImmutableArray.Create(overload) :
            ImmutableArray<FunctionOverload>.Empty);
    }

    private readonly Lazy<ImmutableArray<FunctionOverload>> overloadsLazy;

    public FunctionDeclarationSyntax DeclaringFunction => (FunctionDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitDeclaredFunctionSymbol(this);

    public override SymbolKind Kind => SymbolKind.Function;

    public override IEnumerable<Symbol> Descendants => Type.AsEnumerable();

    public ImmutableArray<FunctionOverload> Overloads => overloadsLazy.Value;

    public FunctionFlags FunctionFlags => FunctionFlags.Default;

    private FunctionOverload GetFunctionOverload()
    {
        var builder = new FunctionOverloadBuilder(this.Name);
        if (DescriptionHelper.TryGetFromDecorator(Context.Binder, Context.TypeManager, DeclaringFunction) is { } description)
        {
            builder.WithGenericDescription(description);
        }

        if (this.DeclaringFunction.Lambda is not TypedLambdaSyntax lambdaSyntax ||
            this.Context.TypeManager.GetDeclaredType(this.DeclaringFunction.Lambda) is not LambdaType lambdaType)
        {
            builder.WithReturnType(ErrorType.Empty());

            return builder.Build();
        }

        var localVariables = lambdaSyntax.GetLocalVariables().ToImmutableArray();
        for (var i = 0; i < localVariables.Length; i++)
        {
            builder.WithRequiredParameter(localVariables[i].Name.IdentifierName, lambdaType.ArgumentTypes[i].Type, "");
        }

        builder.WithReturnType(lambdaType.ReturnType.Type);

        return builder.Build();
    }
}