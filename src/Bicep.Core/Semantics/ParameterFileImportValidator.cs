// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;

namespace Bicep.Core.Semantics;

internal static class ParameterFileImportValidator
{
    public static Diagnostic? ValidateImport(ImportedSymbol importedSymbol)
    {
        if (importedSymbol.Context.SourceFile.FileKind != BicepSourceFileKind.ParamsFile ||
            GetDeploymentContextFunctionNames(importedSymbol) is not { Length: > 0 } functionNames)
        {
            return null;
        }

        return DiagnosticBuilder.ForPosition(importedSymbol.DeclaringImportedSymbolsListItem.OriginalSymbolName)
            .ImportedSymbolDependsOnDeploymentContextFunctions(importedSymbol.Name, functionNames);
    }

    public static Diagnostic? ValidateImport(WildcardImportSymbol wildcardImport)
    {
        if (wildcardImport.Context.SourceFile.FileKind != BicepSourceFileKind.ParamsFile ||
            GetDeploymentContextFunctionNames(wildcardImport) is not { Length: > 0 } functionNames)
        {
            return null;
        }

        return DiagnosticBuilder.ForPosition(wildcardImport.DeclaringSyntax)
            .ImportedSymbolDependsOnDeploymentContextFunctions(wildcardImport.Name, functionNames);
    }

    private static ImmutableArray<string> GetDeploymentContextFunctionNames(ImportedSymbol importedSymbol)
    {
        if (importedSymbol.SourceModel is not SemanticModel sourceModel ||
            TryGetExportedValueDeclaration(sourceModel, importedSymbol) is not { } exportedSymbol)
        {
            return [];
        }

        return GetDeploymentContextFunctionNames(sourceModel, [exportedSymbol]);
    }

    private static ImmutableArray<string> GetDeploymentContextFunctionNames(WildcardImportSymbol wildcardImport)
    {
        if (wildcardImport.SourceModel is not SemanticModel sourceModel)
        {
            return [];
        }

        var exportedSymbols = sourceModel.Exports.Values
            .Select(export => TryGetExportedValueDeclaration(sourceModel, export))
            .WhereNotNull();

        return GetDeploymentContextFunctionNames(sourceModel, exportedSymbols);
    }

    private static ImmutableArray<string> GetDeploymentContextFunctionNames(SemanticModel sourceModel, IEnumerable<DeclaredSymbol> exportedSymbols)
    {
        var visited = new HashSet<DeclaredSymbol>();
        var functionNames = new HashSet<string>(LanguageConstants.IdentifierComparer);

        foreach (var exportedSymbol in exportedSymbols)
        {
            CollectDeploymentContextFunctionNames(sourceModel, exportedSymbol, visited, functionNames);
        }

        return [.. functionNames.Order(StringComparer.OrdinalIgnoreCase)];
    }

    private static void CollectDeploymentContextFunctionNames(SemanticModel model, DeclaredSymbol rootSymbol, ISet<DeclaredSymbol> visited, ISet<string> functionNames)
    {
        foreach (var symbol in model.Binder.GetReferencedSymbolClosureFor(rootSymbol).Add(rootSymbol))
        {
            if (!visited.Add(symbol))
            {
                continue;
            }

            foreach (var syntax in GetValueSyntaxes(symbol))
            {
                CollectDeploymentContextFunctionNames(model, syntax, visited, functionNames);
            }

            switch (symbol)
            {
                case ImportedSymbol importedSymbol when importedSymbol.SourceModel is SemanticModel importedFrom &&
                    TryGetExportedValueDeclaration(importedFrom, importedSymbol) is { } importedDeclaration:
                    CollectDeploymentContextFunctionNames(importedFrom, importedDeclaration, visited, functionNames);
                    break;
            }
        }
    }

    private static void CollectDeploymentContextFunctionNames(SemanticModel model, SyntaxBase syntax, ISet<DeclaredSymbol> visited, ISet<string> functionNames)
    {
        foreach (var functionCall in SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(syntax))
        {
            switch (SymbolHelper.TryGetSymbolInfo(model.Binder, model.TypeManager.GetDeclaredType, functionCall))
            {
                case FunctionSymbol functionSymbol when !CanEvaluateFunctionWhileBuildingParametersFile(model, functionSymbol, functionCall):
                    functionNames.Add(functionSymbol.Name);
                    break;

                case WildcardImportInstanceFunctionSymbol wildcardFunction when wildcardFunction.BaseSymbol.SourceModel is SemanticModel importedFrom &&
                    TryGetExportedDeclaration<DeclaredFunctionSymbol>(importedFrom, wildcardFunction.Name) is { } importedDeclaration:
                    CollectDeploymentContextFunctionNames(importedFrom, importedDeclaration, visited, functionNames);
                    break;
            }
        }

        foreach (var propertyAccess in SyntaxAggregator.AggregateByType<PropertyAccessSyntax>(syntax))
        {
            if (model.Binder.GetSymbolInfo(propertyAccess.BaseExpression) is WildcardImportSymbol wildcardImport &&
                wildcardImport.SourceModel is SemanticModel importedFrom &&
                TryGetExportedDeclaration<VariableSymbol>(importedFrom, propertyAccess.PropertyName.IdentifierName) is { } importedDeclaration)
            {
                CollectDeploymentContextFunctionNames(importedFrom, importedDeclaration, visited, functionNames);
            }
        }
    }

    private static bool CanEvaluateFunctionWhileBuildingParametersFile(SemanticModel model, FunctionSymbol functionSymbol, FunctionCallSyntaxBase functionCall)
    {
        var flags = model.TypeManager.GetMatchedFunctionOverload(functionCall)?.Flags ?? functionSymbol.FunctionFlags;

        return flags.HasFlag(FunctionFlags.Pure);
    }

    private static IEnumerable<SyntaxBase> GetValueSyntaxes(DeclaredSymbol symbol) => symbol switch
    {
        VariableSymbol variable => [variable.DeclaringVariable.Value],
        DeclaredFunctionSymbol function => [function.DeclaringFunction.Lambda],
        _ => [],
    };

    private static DeclaredSymbol? TryGetExportedValueDeclaration(SemanticModel sourceModel, ImportedSymbol importedSymbol)
    {
        if (importedSymbol.OriginalSymbolName is not { } originalSymbolName)
        {
            return null;
        }

        return importedSymbol switch
        {
            ImportedVariableSymbol => TryGetExportedDeclaration<VariableSymbol>(sourceModel, originalSymbolName),
            ImportedFunctionSymbol => TryGetExportedDeclaration<DeclaredFunctionSymbol>(sourceModel, originalSymbolName),
            _ => null,
        };
    }

    private static TSymbol? TryGetExportedDeclaration<TSymbol>(SemanticModel sourceModel, string name)
        where TSymbol : DeclaredSymbol
        => sourceModel.Root.GetDeclarationsByName(name)
            .OfType<TSymbol>()
            .FirstOrDefault(symbol => symbol.IsExported(sourceModel));

    private static DeclaredSymbol? TryGetExportedValueDeclaration(SemanticModel sourceModel, ExportMetadata exportMetadata) => exportMetadata switch
    {
        ExportedVariableMetadata => TryGetExportedDeclaration<VariableSymbol>(sourceModel, exportMetadata.Name),
        ExportedFunctionMetadata => TryGetExportedDeclaration<DeclaredFunctionSymbol>(sourceModel, exportMetadata.Name),
        _ => null,
    };
}
