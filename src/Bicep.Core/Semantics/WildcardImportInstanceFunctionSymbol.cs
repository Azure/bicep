// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class WildcardImportInstanceFunctionSymbol(WildcardImportSymbol baseSymbol, string name, ExportedFunctionMetadata exportMetadata) : Symbol(name), IFunctionSymbol
{
    public override void Accept(SymbolVisitor visitor) => visitor.VisitWildcardImportInstanceFunctionSymbol(this);

    public override SymbolKind Kind => SymbolKind.Function;

    public ImmutableArray<FunctionOverload> Overloads { get; } = ImmutableArray.Create(TypeHelper.OverloadWithResolvedTypes(new(baseSymbol.Context.Binder), exportMetadata));

    public FunctionFlags FunctionFlags => FunctionFlags.Default;

    public WildcardImportSymbol BaseSymbol { get; } = baseSymbol;
}
