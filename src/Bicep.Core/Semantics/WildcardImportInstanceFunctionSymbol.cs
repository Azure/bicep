// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class WildcardImportInstanceFunctionSymbol : Symbol, IFunctionSymbol
{
    public WildcardImportInstanceFunctionSymbol(WildcardImportSymbol baseSymbol, string name, ExportedFunctionMetadata exportMetadata)
        : base(name)
    {
        BaseSymbol = baseSymbol;
        Overloads = [TypeHelper.OverloadWithResolvedTypes(new(baseSymbol.Context.Binder), exportMetadata)];
    }

    public override void Accept(SymbolVisitor visitor) => visitor.VisitWildcardImportInstanceFunctionSymbol(this);

    public override SymbolKind Kind => SymbolKind.Function;

    public ImmutableArray<FunctionOverload> Overloads { get; }

    public FunctionFlags FunctionFlags => FunctionFlags.Default;

    public WildcardImportSymbol BaseSymbol { get; }
}
