// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Semantics;

public class ImportedFunctionSymbol : ImportedSymbol<ExportedFunctionMetadata>, IFunctionSymbol
{
    public ImportedFunctionSymbol(ISymbolContext context, ImportedSymbolsListItemSyntax declaringSyntax, CompileTimeImportDeclarationSyntax enclosingDeclartion, ISemanticModel sourceModel, ExportedFunctionMetadata exportedFunctionMetadata)
        : base(context, declaringSyntax, enclosingDeclartion, sourceModel, exportedFunctionMetadata) { }

    public override SymbolKind Kind => SymbolKind.Function;

    public FunctionOverload Overload => ExportMetadata.Overload;

    public ImmutableArray<FunctionOverload> Overloads => ImmutableArray.Create(Overload);

    public FunctionFlags FunctionFlags => FunctionFlags.Default;

    public override void Accept(SymbolVisitor visitor) => visitor.VisitImportedFunctionSymbol(this);
}
