// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

public class DeclaredFunctionSymbol : DeclaredSymbol, IFunctionSymbol
{
    private readonly Lazy<FunctionOverload> overloadLazy;
    public DeclaredFunctionSymbol(ISymbolContext context, string name, FunctionDeclarationSyntax declaringSyntax)
        : base(context, name, declaringSyntax, declaringSyntax.Name)
    {
        this.overloadLazy = new(GetFunctionOverload);
    }

    public FunctionDeclarationSyntax DeclaringFunction => (FunctionDeclarationSyntax)this.DeclaringSyntax;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitDeclaredFunctionSymbol(this);

    public override SymbolKind Kind => SymbolKind.Function;

    public override IEnumerable<Symbol> Descendants => Type.AsEnumerable();

    // Unlike functions defined in the ARM engine, user-defined functions do not support multiple dispatch and will always have exactly one overload.
    public FunctionOverload Overload => overloadLazy.Value;

    public ImmutableArray<FunctionOverload> Overloads => [Overload];

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
            builder.WithRequiredParameter(localVariables[i].Name.IdentifierName, lambdaType.GetArgumentType(i).Type, "");
        }

        builder.WithReturnType(lambdaType.ReturnType.Type);

        return builder.Build();
    }
}
