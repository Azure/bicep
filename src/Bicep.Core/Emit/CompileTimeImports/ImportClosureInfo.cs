// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit.CompileTimeImports;

internal enum ArmSymbolType { Type, Variable, Function };

internal record ArmIdentifier(ArmSymbolType SymbolType, string Identifier);

public record WildcardImportPropertyReference(WildcardImportSymbol WildcardImport, string PropertyName);

public record ImportedSymbolOriginMetadata(string SourceTemplateIdentifier, string OriginalName);

public record ImportClosureInfo(ImmutableArray<DeclaredTypeExpression> ImportedTypesInClosure,
    ImmutableArray<DeclaredVariableExpression> ImportedVariablesInClosure,
    ImmutableArray<DeclaredFunctionExpression> ImportedFunctionsInClosure,
    ImmutableDictionary<ImportedSymbol, string> ImportedSymbolNames,
    ImmutableDictionary<WildcardImportPropertyReference, string> WildcardImportPropertyNames,
    ImmutableDictionary<string, ImportedSymbolOriginMetadata> ImportedSymbolOriginMetadata)
{
    private const string ArmTypeRefPrefix = "#/definitions/";
    private const string BicepDefinedFunctionNamePrefix = $"{EmitConstants.UserDefinedFunctionsNamespace}.";

    public static ImportClosureInfo Calculate(SemanticModel model)
    {
        var closure = CalculateImportClosure(model);
        var closureMetadata = CalculateImportedSymbolMetadata(model, closure);

        var importedBicepSymbolNames = closureMetadata.Keys.OfType<BicepSymbolicReference>()
            .ToImmutableDictionary(@ref => @ref.Symbol, @ref => closureMetadata[@ref].UniqueNameWithinClosure);
        var importedSymbolNames = closure.ImportedSymbolsToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value].UniqueNameWithinClosure);
        var wildcardImportPropertyNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value].UniqueNameWithinClosure);
        var synthesizedVariableNames = closureMetadata.Keys.OfType<BicepSynthesizedVariableReference>()
            .ToLookup(@ref => @ref.SourceBicepModel)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableDictionary(@ref => @ref.Name, @ref => closureMetadata[@ref].UniqueNameWithinClosure));

        ArmIdentifierEqualityComparer armIdentifierEqualityComparer = new();
        var importedArmSymbolNamesByFile = closureMetadata.Keys.OfType<ArmSymbolicReference>()
            .ToLookup(@ref => @ref.ArmTemplateFile)
            .ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToImmutableDictionary(
                @ref => new ArmIdentifier(@ref.Type, @ref.Identifier),
                @ref => closureMetadata[@ref].UniqueNameWithinClosure,
                armIdentifierEqualityComparer));
        var armDeclarationToExpressionConverters = closureMetadata.Keys.OfType<ArmSymbolicReference>()
            .Select(@ref => @ref.ArmTemplateFile)
            .Distinct()
            .ToImmutableDictionary(templateFile => templateFile,
                templateFile => new ArmDeclarationToExpressionConverter(templateFile.Template ?? throw new InvalidOperationException(),
                    importedArmSymbolNamesByFile[templateFile],
                    sourceSyntax: null));

        Dictionary<string, DeclaredTypeExpression> importedTypes = new();
        Dictionary<string, DeclaredVariableExpression> importedVariables = new();
        Dictionary<string, DeclaredFunctionExpression> importedFunctions = new();
        var importedSymbolMetadata = ImmutableDictionary.CreateBuilder<string, ImportedSymbolOriginMetadata>();

        foreach (var (symbol, (originMetadata, name)) in closureMetadata)
        {
            importedSymbolMetadata.Add(name, originMetadata);
            switch (symbol)
            {
                case ArmSymbolicReference armRef:
                    var converter = armDeclarationToExpressionConverters[armRef.ArmTemplateFile].WithSourceSyntax(closure.SymbolsInImportClosure[armRef]);
                    switch (armRef.Type)
                    {
                        case ArmSymbolType.Type:
                            importedTypes.Add(name, converter.CreateDeclaredTypeExpressionFor(armRef.Identifier));
                            break;
                        case ArmSymbolType.Variable:
                            importedVariables.Add(name, converter.CreateDeclaredVariableExpressionFor(armRef.Identifier));
                            break;
                        case ArmSymbolType.Function:
                            importedFunctions.Add(name, converter.CreateDeclaredFunctionExpressionFor(armRef.Identifier));
                            break;
                        default:
                            throw new UnreachableException($"Unknown ARM symbol type: {armRef.Type}");
                    }
                    break;
                case BicepSymbolicReference bicepRef:
                    var migrator = new ImportedSymbolDeclarationMigrator(bicepRef.SourceBicepModel,
                        importedBicepSymbolNames,
                        synthesizedVariableNames.TryGetValue(bicepRef.SourceBicepModel, out var dict) ? dict : ImmutableDictionary<string, string>.Empty,
                        closure.SymbolsInImportClosure[bicepRef]);
                    ExpressionBuilder expressionBuilder = new(closure.EmitterContexts.GetOrAdd(bicepRef.SourceBicepModel, m => new(m)));
                    switch (bicepRef.Symbol)
                    {
                        case TypeAliasSymbol importedType:
                            importedTypes.Add(name, migrator.RewriteForMigration((DeclaredTypeExpression)expressionBuilder.Convert(importedType.DeclaringType)));
                            break;
                        case VariableSymbol importedVariable:
                            importedVariables.Add(name, migrator.RewriteForMigration((DeclaredVariableExpression)expressionBuilder.Convert(importedVariable.DeclaringVariable)));
                            break;
                        case DeclaredFunctionSymbol importedFunction:
                            importedFunctions.Add(name, migrator.RewriteForMigration((DeclaredFunctionExpression)expressionBuilder.Convert(importedFunction.DeclaringFunction)));
                            break;
                        default:
                            throw new UnreachableException($"Cannot import Bicep symbols of type {bicepRef.Symbol.GetType().Name}");
                    }
                    break;
                case BicepSynthesizedVariableReference synthesizedVariableRef:
                    importedVariables.Add(name, new(closure.SymbolsInImportClosure[synthesizedVariableRef], name, synthesizedVariableRef.Value));
                    break;
                default:
                    throw new UnreachableException($"This switch was expected to exhaustively process all kinds of {nameof(IntraTemplateSymbolicReference)} but did not handle an instance of type {symbol.GetType().Name}");
            }
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            ImmutableArray.CreateRange(importedVariables.Values.OrderBy(dve => dve.Name)),
            ImmutableArray.CreateRange(importedFunctions.Values.OrderBy(dfe => dfe.Name)),
            importedSymbolNames,
            wildcardImportPropertyNames,
            importedSymbolMetadata.ToImmutable());
    }

    private static ImportClosure CalculateImportClosure(SemanticModel model)
    {
        Dictionary<ISemanticModel, ArtifactReference> importedModuleReferences = new();
        Dictionary<IntraTemplateSymbolicReference, SyntaxBase> symbolsInImportClosure = new();
        Dictionary<ImportedSymbol, IntraTemplateSymbolicReference> importedSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> wildcardImportPropertiesToIntraTemplateSymbols = new();
        ConcurrentDictionary<ArmTemplateFile, ArmReferenceCollector> armReferenceCollectors = new();
        ConcurrentDictionary<SemanticModel, EmitterContext> bicepEmitterContexts = new();

        Queue<SearchQueueItem> searchQueue = new(model.Root.ImportedSymbols
            .Select(importedSymbol => new SearchQueueItem(importedSymbol.DeclaringSyntax, new BicepImportedSymbolReference(importedSymbol, model, GetImportReference(importedSymbol))))
            .Concat(model.Root.WildcardImports
                .Select(wildcardImport => new SearchQueueItem(wildcardImport.DeclaringSyntax, new BicepWildcardImportSymbolicReference(wildcardImport, model, GetImportReference(wildcardImport))))));

        while (searchQueue.Count > 0)
        {
            var item = searchQueue.Dequeue();

            if (item.SymbolicReference is BicepSymbolicReference bicepSymbolicReference)
            {
                if (symbolsInImportClosure.TryAdd(bicepSymbolicReference, item.InitiallyDeclaringSyntax))
                {
                    foreach (var reference in CollectReferences(bicepSymbolicReference))
                    {
                        searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, reference));
                    }

                    foreach (var synthesizedVariableReference in CollectSynthesizedVariableReferences(bicepSymbolicReference, bicepEmitterContexts.GetOrAdd(bicepSymbolicReference.SourceBicepModel, m => new(m))))
                    {
                        symbolsInImportClosure.TryAdd(synthesizedVariableReference, item.InitiallyDeclaringSyntax);
                    }
                }
            }
            else if (item.SymbolicReference is ArmSymbolicReference armSymbolicReference)
            {
                if (symbolsInImportClosure.TryAdd(armSymbolicReference, item.InitiallyDeclaringSyntax))
                {
                    ArmIdentifier refTarget = new(armSymbolicReference.Type, armSymbolicReference.Identifier);
                    foreach (var reference in armReferenceCollectors.GetOrAdd(armSymbolicReference.ArmTemplateFile, f => new(f)).EnumerateReferencesUsedInDefinitionOf(refTarget))
                    {
                        searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, new ArmSymbolicReference(reference.SymbolType, reference.Identifier, armSymbolicReference.ArmTemplateFile, armSymbolicReference.SourceModel)));
                    }
                }
            }
            else if (item.SymbolicReference is BicepWildcardImportSymbolicReference wildcardImportSymbolicReference)
            {
                var targetModel = wildcardImportSymbolicReference.Symbol.SourceModel;
                importedModuleReferences[targetModel] = wildcardImportSymbolicReference.ImportTarget;

                foreach (var (propertyName, exportedSymbol) in EnumerateExportedSymbolsAsIntraTemplateSymbols(targetModel))
                {
                    wildcardImportPropertiesToIntraTemplateSymbols[new(wildcardImportSymbolicReference.Symbol, propertyName)] = exportedSymbol;
                    searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, exportedSymbol));
                }
            }
            else if (item.SymbolicReference is BicepImportedSymbolReference importedSymbolReference)
            {
                var targetModel = importedSymbolReference.Symbol.SourceModel;
                importedModuleReferences[targetModel] = importedSymbolReference.ImportTarget;

                var name = importedSymbolReference.Symbol.OriginalSymbolName
                    ?? throw new InvalidOperationException($"The import symbol {importedSymbolReference.Symbol.Name} did not specify what symbol to import");

                if (!targetModel.Exports.TryGetValue(name, out var exportMetadata))
                {
                    throw new InvalidOperationException($"No export named {name} found in {TemplateIdentifier(model, targetModel, importedSymbolReference.ImportTarget)}");
                }

                IntraTemplateSymbolicReference target = targetModel switch
                {
                    SemanticModel targetBicepModel
                        => new BicepSymbolicReference(FindSymbolNamed(name, targetBicepModel), targetBicepModel),
                    ArmTemplateSemanticModel targetArmModel
                        => ReferenceForArmTarget(exportMetadata, targetArmModel.SourceFile, targetModel),
                    TemplateSpecSemanticModel targetTemplateSpecModel
                        => ReferenceForArmTarget(exportMetadata, targetTemplateSpecModel.SourceFile.MainTemplateFile, targetModel),
                    _ => throw new InvalidOperationException($"Unrecognized module type {targetModel.GetType().Name} encountered"),
                };

                importedSymbolsToIntraTemplateSymbols[importedSymbolReference.Symbol] = target;
                searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, target));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected symbolic reference type of {item.SymbolicReference.GetType().Name} encountered.");
            }
        }

        return new(importedModuleReferences,
            symbolsInImportClosure,
            importedSymbolsToIntraTemplateSymbols,
            wildcardImportPropertiesToIntraTemplateSymbols,
            bicepEmitterContexts);
    }

    private static ArtifactReference GetImportReference(ImportedSymbol symbol)
    {
        if (symbol.TryGetArtifactReference().IsSuccess(out var moduleReference))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static ArtifactReference GetImportReference(WildcardImportSymbol symbol)
    {
        if (symbol.TryGetArtifactReference().IsSuccess(out var moduleReference))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static IEnumerable<SymbolicReference> CollectReferences(BicepSymbolicReference typeReference)
        => typeReference.SourceBicepModel.Binder.GetSymbolsReferencedInDeclarationOf(typeReference.Symbol)
            .Select<DeclaredSymbol, SymbolicReference?>(symbol => symbol switch
            {
                ProviderNamespaceSymbol => null, // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
                LocalVariableSymbol => null, // local variables are contained within the expression and are not external references
                TypeAliasSymbol typeAlias => new BicepSymbolicReference(typeAlias, typeReference.SourceBicepModel),
                VariableSymbol variable => new BicepSymbolicReference(variable, typeReference.SourceBicepModel),
                DeclaredFunctionSymbol declaredFunction => new BicepSymbolicReference(declaredFunction, typeReference.SourceBicepModel),
                ImportedSymbol imported => new BicepImportedSymbolReference(imported, typeReference.SourceBicepModel, GetImportReference(imported)),
                WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbolicReference(wildcardImport, typeReference.SourceBicepModel, GetImportReference(wildcardImport)),
                _ => throw new InvalidOperationException($"Invalid symbol {symbol.Name} of type {symbol.GetType().Name} encountered within a export expression"),
            })
            .WhereNotNull();

    private static IEnumerable<BicepSynthesizedVariableReference> CollectSynthesizedVariableReferences(BicepSymbolicReference @ref, EmitterContext emitterContext)
        => SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(@ref.Symbol.DeclaringSyntax)
            .Select(functionCallSyntax => emitterContext.FunctionVariables.TryGetValue(functionCallSyntax, out var result)
                ? new BicepSynthesizedVariableReference(result.Name, result.Value, @ref.SourceBicepModel)
                : null)
            .WhereNotNull();

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(bicepModel),
        ArmTemplateSemanticModel armModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(armModel),
        TemplateSpecSemanticModel templateSpecModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(templateSpecModel),
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(SemanticModel model)
        => model.Root.TypeDeclarations
            .Where(s => model.Exports.ContainsKey(s.Name))
            .Select<TypeAliasSymbol, (string, IntraTemplateSymbolicReference)>(t => (t.Name, new BicepSymbolicReference(t, model)))
            .Concat(model.Root.VariableDeclarations
                .Where(s => model.Exports.ContainsKey(s.Name))
                .Select<VariableSymbol, (string, IntraTemplateSymbolicReference)>(v => (v.Name, new BicepSymbolicReference(v, model))))
            .Concat(model.Root.FunctionDeclarations
                .Where(s => model.Exports.ContainsKey(s.Name))
                .Select<DeclaredFunctionSymbol, (string, IntraTemplateSymbolicReference)>(f => (f.Name, new BicepSymbolicReference(f, model))));

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ArmTemplateSemanticModel model)
        => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile);

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(TemplateSpecSemanticModel model)
        => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile.MainTemplateFile);

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ISemanticModel model, ArmTemplateFile templateFile)
        => model.Exports.Values.Select<ExportMetadata, (string, IntraTemplateSymbolicReference)>(md => (md.Name, ReferenceForArmTarget(md, templateFile, model)));

    private static DeclaredSymbol FindSymbolNamed(string nameOfSymbolSought, SemanticModel model)
        => model.Root.Declarations.Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, nameOfSymbolSought)).Single();

    private static IntraTemplateSymbolicReference ReferenceForArmTarget(ExportMetadata targetMetadata, ArmTemplateFile sourceTemplateFile, ISemanticModel sourceModel)
        => targetMetadata switch
        {
            ExportedTypeMetadata => new ArmSymbolicReference(ArmSymbolType.Type, $"{ArmTypeRefPrefix}{targetMetadata.Name}", sourceTemplateFile, sourceModel),
            ExportedVariableMetadata => new ArmSymbolicReference(ArmSymbolType.Variable, targetMetadata.Name, sourceTemplateFile, sourceModel),
            ExportedFunctionMetadata => new ArmSymbolicReference(ArmSymbolType.Function,
                !targetMetadata.Name.Contains('.') ? $"{EmitConstants.UserDefinedFunctionsNamespace}.{targetMetadata.Name}" : targetMetadata.Name,
                sourceTemplateFile,
                sourceModel),
            _ => throw new InvalidOperationException($"Unrecognized export metadata type: {targetMetadata.GetType().Name}"),
        };

    private static ImmutableDictionary<IntraTemplateSymbolicReference, (ImportedSymbolOriginMetadata OriginMetadata, string UniqueNameWithinClosure)>
    CalculateImportedSymbolMetadata(SemanticModel model, ImportClosure closure)
    {
        var importedSymbolMetadata = ImmutableDictionary.CreateBuilder<IntraTemplateSymbolicReference, (ImportedSymbolOriginMetadata, string)>();

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedSymbol in model.Root.ImportedSymbols)
        {
            var importTarget = closure.ImportedSymbolsToIntraTemplateSymbols[importedSymbol];
            importedSymbolMetadata[closure.ImportedSymbolsToIntraTemplateSymbols[importedSymbol]] = (
                new(TemplateIdentifier(model, importTarget.SourceModel, closure.ImportedModuleReferences[importTarget.SourceModel]), SymbolIdentifier(importTarget)),
                importedSymbol.Name);
        }

        int uniqueIdentifier = 1;
        ConcurrentDictionary<string, int> templateIds = new();

        // Every other symbol in the closure should be assigned a stable identifier that won't conflict with any valid Bicep identifier
        foreach (var (symbolInfo, sourceTemplateIdentifier, originalSymbolName) in closure.SymbolsInImportClosure.Keys
            .Select(symbolInfo => (symbolInfo, TemplateIdentifier(model, symbolInfo.SourceModel, closure.ImportedModuleReferences[symbolInfo.SourceModel]), SymbolIdentifier(symbolInfo)))
            .OrderBy(t => $"{t.Item2}_{t.Item3}"))
        {
            // This symbol was imported by name and should appear in the template using the assigned identifier
            if (importedSymbolMetadata.ContainsKey(symbolInfo))
            {
                continue;
            }

            var templateId = templateIds.GetOrAdd(sourceTemplateIdentifier, _ => uniqueIdentifier++);
            var symbolId = Lexer.IsValidIdentifier(originalSymbolName) ? originalSymbolName : $"_{uniqueIdentifier++}";

            importedSymbolMetadata.Add(symbolInfo, (new(sourceTemplateIdentifier, originalSymbolName), $"_{templateId}.{symbolId}"));
        }

        return importedSymbolMetadata.ToImmutable();
    }

    private static string TemplateIdentifier(SemanticModel entryPointModel, ISemanticModel modelToIdentify, ArtifactReference reference)
        => reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference => entryPointModel.SourceFile.FileUri.MakeRelativeUri(GetSourceFileUri(modelToIdentify)).ToString(),
            ArtifactReference otherwise => otherwise.FullyQualifiedReference,
        };

    private static Uri GetSourceFileUri(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => bicepModel.SourceFile.FileUri,
        ArmTemplateSemanticModel armTemplate => armTemplate.SourceFile.FileUri,
        TemplateSpecSemanticModel templateSpec => templateSpec.SourceFile.FileUri,
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static string SymbolIdentifier(IntraTemplateSymbolicReference reference) => reference switch
    {
        BicepSymbolicReference bicepSymbolicReference => bicepSymbolicReference.Symbol.Name,
        BicepSynthesizedVariableReference synthesizedVariableReference => synthesizedVariableReference.Name,
        // For ARM JSON type references, the name is a JSON pointer
        // If the pointer starts with "#/definitions/" and everything after that is a valid idenfitier (this will be the case for anything compiled from Bicep), use the last path segment
        ArmSymbolicReference armRef when armRef.Type == ArmSymbolType.Type && armRef.Identifier.StartsWith(ArmTypeRefPrefix)
            => armRef.Identifier[ArmTypeRefPrefix.Length..],
        // For ARM user-defined function references, the name will be of the format "<namespace>.<name>"
        // If the namespace is "__bicep" (this will be the case for anything compiled from Bicep), use everything after the '.'
        ArmSymbolicReference armRef when armRef.Type == ArmSymbolType.Function && armRef.Identifier.StartsWith(BicepDefinedFunctionNamePrefix)
            => armRef.Identifier[BicepDefinedFunctionNamePrefix.Length..],
        ArmSymbolicReference armRef => armRef.Identifier,
        _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {reference.GetType().Name} encountered."),
    };

    private record SymbolicReference(ISemanticModel SourceModel);
    private record InterTemplateSymbolicReference(SemanticModel SourceBicepModule) : SymbolicReference(SourceBicepModule);
    private record BicepWildcardImportSymbolicReference(WildcardImportSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel);
    private record BicepImportedSymbolReference(ImportedSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel);

    private record IntraTemplateSymbolicReference(ISemanticModel SourceModel) : SymbolicReference(SourceModel);
    private record BicepSymbolicReference(DeclaredSymbol Symbol, SemanticModel SourceBicepModel) : IntraTemplateSymbolicReference(SourceBicepModel);
    private record BicepSynthesizedVariableReference(string Name, Expression Value, SemanticModel SourceBicepModel) : IntraTemplateSymbolicReference(SourceBicepModel);
    private record ArmSymbolicReference(ArmSymbolType Type, string Identifier, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel) : IntraTemplateSymbolicReference(SourceModel);

    private record ImportClosure(
        IReadOnlyDictionary<ISemanticModel, ArtifactReference> ImportedModuleReferences,
        IReadOnlyDictionary<IntraTemplateSymbolicReference, SyntaxBase> SymbolsInImportClosure,
        IReadOnlyDictionary<ImportedSymbol, IntraTemplateSymbolicReference> ImportedSymbolsToIntraTemplateSymbols,
        IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> WildcardImportPropertiesToIntraTemplateSymbols,
        ConcurrentDictionary<SemanticModel, EmitterContext> EmitterContexts);

    private record SearchQueueItem(SyntaxBase InitiallyDeclaringSyntax, SymbolicReference SymbolicReference);

    private class ArmIdentifierEqualityComparer : IEqualityComparer<ArmIdentifier>
    {
        public bool Equals(ArmIdentifier? x, ArmIdentifier? y)
        {
            if (x is null)
            {
                return y is null;
            }

            return x.SymbolType == y?.SymbolType && StringComparer.OrdinalIgnoreCase.Equals(x.Identifier, y?.Identifier);
        }

        public int GetHashCode([DisallowNull] ArmIdentifier obj)
            => obj.GetHashCode();
    }
}
