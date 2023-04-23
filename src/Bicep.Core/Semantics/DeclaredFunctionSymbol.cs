// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class DeclaredFunctionSymbol : DeclaredSymbol, IFunctionSymbol
{
    public DeclaredFunctionSymbol(ISymbolContext context, string name, FunctionDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        this.overloadsLazy = new(() => TryGetFunctionOverload() is {} overload ? 
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

    private FunctionOverload? TryGetFunctionOverload()
    {
        if (this.DeclaringFunction.Lambda is not TypedLambdaSyntax lambdaSyntax ||
            this.Context.TypeManager.GetDeclaredType(this.DeclaringFunction.Lambda) is not LambdaType lambdaType)
        {
            return null;
        }

        var builder = new FunctionOverloadBuilder(this.Name);

        var localVariables = lambdaSyntax.GetLocalVariables().ToImmutableArray();
        for (var i = 0; i < localVariables.Length; i++)
        {
            builder.WithRequiredParameter(localVariables[i].Name.IdentifierName, lambdaType.ArgumentTypes[i].Type, "");
        }

        builder.WithReturnType(lambdaType.ReturnType.Type);

        return builder.Build();
    }
}