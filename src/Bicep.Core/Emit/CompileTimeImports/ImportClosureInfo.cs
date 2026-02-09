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
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Syntax.Visitors;
using Bicep.IO.Abstraction;
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
        IntraTemplateSymbolicReferenceFactory referenceFactory = new(model.SourceFile.FileHandle.Uri);
        var closure = CalculateImportClosure(model, referenceFactory);
        var closureMetadata = CalculateImportedSymbolNames(model, closure);

        var importedBicepSymbolNames = closureMetadata.Keys.OfType<BicepSymbolicReference>()
            .ToImmutableDictionary(@ref => @ref.Symbol, @ref => closureMetadata[@ref]);
        var importedSymbolNames = closure.ImportedSymbolsToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value]);
        var wildcardImportPropertyNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value]);
        var synthesizedVariableNames = closureMetadata.Keys.OfType<BicepSynthesizedVariableReference>()
            .ToLookup(@ref => @ref.SourceBicepModel)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableDictionary(@ref => @ref.Name, @ref => closureMetadata[@ref]));

        var importedArmSymbolNamesByFile = closureMetadata.Keys.OfType<ArmSymbolicReference>()
            .ToLookup(@ref => @ref.ArmTemplateFile)
            .ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToImmutableDictionary(
                @ref => new ArmIdentifier(@ref.Type, @ref.Identifier),
                @ref => closureMetadata[@ref],
                ArmIdentifierEqualityComparer.Instance));
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

        foreach (var (symbol, name) in closureMetadata)
        {
            importedSymbolMetadata.Add(name, new(symbol.SourceTemplateIdentifier, symbol.OriginalSymbolName));
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
                        synthesizedVariableNames.TryGetValue(bicepRef.SourceBicepModel, out var dict) ? dict : [],
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
                    importedVariables.Add(name, new(closure.SymbolsInImportClosure[synthesizedVariableRef], name, null, synthesizedVariableRef.Value));
                    break;
                default:
                    throw new UnreachableException($"This switch was expected to exhaustively process all kinds of {nameof(IntraTemplateSymbolicReference)} but did not handle an instance of type {symbol.GetType().Name}");
            }
        }

        return new([.. importedTypes.Values.OrderBy(dte => dte.Name)],
            [.. importedVariables.Values.OrderBy(dve => dve.Name)],
            [.. importedFunctions.Values.OrderBy(dfe => dfe.Name)],
            importedSymbolNames,
            wildcardImportPropertyNames,
            importedSymbolMetadata.ToImmutable());
    }

    private static ImportClosure CalculateImportClosure(
        SemanticModel model,
        IntraTemplateSymbolicReferenceFactory referenceFactory)
    {
        Dictionary<IntraTemplateSymbolicReference, SyntaxBase> symbolsInImportClosure = new(IntraTemplateSymbolicReferenceComparer.Instance);
        Dictionary<ImportedSymbol, IntraTemplateSymbolicReference> importedSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> wildcardImportPropertiesToIntraTemplateSymbols = new();
        ConcurrentDictionary<ArmTemplateFile, ArmReferenceCollector> armReferenceCollectors = new();
        ConcurrentDictionary<SemanticModel, EmitterContext> bicepEmitterContexts = new();

        var initialSearchItems = model.Root.ImportedSymbols
            .Select(importedSymbol => new SearchQueueItem(importedSymbol.DeclaringSyntax, new BicepImportedSymbolReference(importedSymbol, model, GetImportReference(importedSymbol))))
            .Concat(model.Root.WildcardImports
                .Select(wildcardImport => new SearchQueueItem(wildcardImport.DeclaringSyntax, new BicepWildcardImportSymbolicReference(wildcardImport, model, GetImportReference(wildcardImport)))));

        Queue<SearchQueueItem> searchQueue = new(initialSearchItems.Concat(GetImportsFromExtendedFiles(model)));

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

                    foreach (var synthesizedVariableReference in CollectSynthesizedVariableReferences(
                        bicepSymbolicReference,
                        bicepEmitterContexts.GetOrAdd(bicepSymbolicReference.SourceBicepModel, m => new(m))))
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
                    foreach (var reference in armReferenceCollectors.GetOrAdd(
                        armSymbolicReference.ArmTemplateFile,
                        f => new(f)).EnumerateReferencesUsedInDefinitionOf(refTarget))
                    {
                        searchQueue.Enqueue(new(
                            item.InitiallyDeclaringSyntax,
                            IntraTemplateSymbolicReferenceFactory.SymbolFor(
                                reference.SymbolType,
                                reference.Identifier,
                                armSymbolicReference)));
                    }
                }
            }
            else if (item.SymbolicReference is BicepWildcardImportSymbolicReference wildcardImportSymbolicReference)
            {
                foreach (var (propertyName, exportedSymbol) in EnumerateExportedSymbolsAsIntraTemplateSymbols(
                    wildcardImportSymbolicReference.Symbol.SourceModel,
                    wildcardImportSymbolicReference,
                    referenceFactory))
                {
                    wildcardImportPropertiesToIntraTemplateSymbols[new(wildcardImportSymbolicReference.Symbol, propertyName)] = exportedSymbol;
                    searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, exportedSymbol));
                }
            }
            else if (item.SymbolicReference is BicepImportedSymbolReference importedSymbolReference)
            {
                var targetModel = importedSymbolReference.Symbol.SourceModel;

                var name = importedSymbolReference.Symbol.OriginalSymbolName
                    ?? throw new InvalidOperationException($"The import symbol {importedSymbolReference.Symbol.Name} did not specify what symbol to import");

                if (!targetModel.Exports.TryGetValue(name, out var exportMetadata))
                {
                    throw new InvalidOperationException($"No export named {name} found in {TemplateIdentifier(
                        model.SourceFile.FileHandle.Uri,
                        targetModel,
                        importedSymbolReference.ImportTarget)}");
                }

                IntraTemplateSymbolicReference target = targetModel switch
                {
                    SemanticModel targetBicepModel => referenceFactory.SymbolFor(
                        FindExportedSymbol(exportMetadata, targetBicepModel),
                        targetBicepModel,
                        importedSymbolReference),
                    ArmTemplateSemanticModel targetArmModel => ReferenceForArmTarget(
                        exportMetadata,
                        targetArmModel.SourceFile,
                        targetModel,
                        importedSymbolReference,
                        referenceFactory),
                    TemplateSpecSemanticModel targetTemplateSpecModel => ReferenceForArmTarget(
                        exportMetadata,
                        targetTemplateSpecModel.SourceFile.MainTemplateFile,
                        targetModel,
                        importedSymbolReference,
                        referenceFactory),
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

        return new(
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

    private static IEnumerable<SearchQueueItem> GetImportsFromExtendedFiles(SemanticModel model)
    {
        // Only parameter files can have extends declarations
        if (model.SourceFile is not BicepParamFile)
        {
            yield break;
        }

        var extendsDeclarations = model.SourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>();
        var visitedModels = new HashSet<ISemanticModel>();
        var modelsToProcess = new Queue<ISemanticModel>();

        // Add all directly extended models to the queue
        foreach (var extendsDeclaration in extendsDeclarations)
        {
            if (model.TryGetReferencedModel(extendsDeclaration).IsSuccess(out var extendedModel))
            {
                if (visitedModels.Add(extendedModel))
                {
                    modelsToProcess.Enqueue(extendedModel);
                }
            }
        }

        // Process extended models recursively
        while (modelsToProcess.TryDequeue(out var extendedModel))
        {
            // Only process Bicep semantic models (not ARM templates)
            if (extendedModel is not SemanticModel extendedSemanticModel)
            {
                continue;
            }

            // Return all imported symbols from this extended model
            foreach (var importedSymbol in extendedSemanticModel.Root.ImportedSymbols)
            {
                yield return new SearchQueueItem(
                    importedSymbol.DeclaringSyntax,
                    new BicepImportedSymbolReference(importedSymbol, extendedSemanticModel, GetImportReference(importedSymbol)));
            }

            // Return all wildcard imports from this extended model
            foreach (var wildcardImport in extendedSemanticModel.Root.WildcardImports)
            {
                yield return new SearchQueueItem(
                    wildcardImport.DeclaringSyntax,
                    new BicepWildcardImportSymbolicReference(wildcardImport, extendedSemanticModel, GetImportReference(wildcardImport)));
            }

            // Check if this extended model also has extends declarations (transitive extends)
            var nestedExtendsDeclarations = extendedSemanticModel.SourceFile.ProgramSyntax.Declarations.OfType<ExtendsDeclarationSyntax>();
            foreach (var nestedExtendsDeclaration in nestedExtendsDeclarations)
            {
                if (extendedSemanticModel.TryGetReferencedModel(nestedExtendsDeclaration).IsSuccess(out var nestedExtendedModel))
                {
                    if (visitedModels.Add(nestedExtendedModel))
                    {
                        modelsToProcess.Enqueue(nestedExtendedModel);
                    }
                }
            }
        }
    }

    private static IEnumerable<SymbolicReference> CollectReferences(BicepSymbolicReference typeReference)
        => typeReference.SourceBicepModel.Binder.GetSymbolsReferencedInDeclarationOf(typeReference.Symbol)
            .Select<DeclaredSymbol, SymbolicReference?>(symbol => symbol switch
            {
                ExtensionNamespaceSymbol => null, // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
                LocalVariableSymbol => null, // local variables are contained within the expression and are not external references
                TypeAliasSymbol typeAlias => IntraTemplateSymbolicReferenceFactory.SymbolFor(typeAlias, typeReference),
                VariableSymbol variable => IntraTemplateSymbolicReferenceFactory.SymbolFor(variable, typeReference),
                DeclaredFunctionSymbol func => IntraTemplateSymbolicReferenceFactory.SymbolFor(func, typeReference),
                ImportedSymbol imported => new BicepImportedSymbolReference(
                    imported,
                    typeReference.SourceBicepModel,
                    GetImportReference(imported)),
                WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbolicReference(
                    wildcardImport,
                    typeReference.SourceBicepModel,
                    GetImportReference(wildcardImport)),
                _ => throw new InvalidOperationException(
                    $"Invalid symbol {symbol.Name} of type {symbol.GetType().Name} encountered within a export expression"),
            })
            .WhereNotNull();

    private static IEnumerable<BicepSynthesizedVariableReference> CollectSynthesizedVariableReferences(
        BicepSymbolicReference @ref,
        EmitterContext emitterContext
    ) => SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(@ref.Symbol.DeclaringSyntax)
        .Select(functionCallSyntax => emitterContext.FunctionVariables.TryGetValue(functionCallSyntax, out var result)
            ? IntraTemplateSymbolicReferenceFactory.SymbolFor(result.Name, result.Value, @ref)
            : null)
        .WhereNotNull();

    private static IEnumerable<
        (string symbolName, IntraTemplateSymbolicReference reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ISemanticModel model,
        BicepWildcardImportSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory
    ) => model switch
    {
        SemanticModel bicepModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(
            bicepModel,
            referrer,
            referenceFactory),
        ArmTemplateSemanticModel armModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(
            armModel,
            referrer,
            referenceFactory),
        TemplateSpecSemanticModel templateSpecModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(
            templateSpecModel,
            referrer,
            referenceFactory),
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static IEnumerable<
        (string symbolName, IntraTemplateSymbolicReference reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        SemanticModel model,
        BicepWildcardImportSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory
    ) => model.Root.TypeDeclarations
        .Concat<DeclaredSymbol>(model.Root.VariableDeclarations)
        .Concat(model.Root.FunctionDeclarations)
        .Where(t => t.IsExported(model))
        .Select(s => (s.Name, referenceFactory.SymbolFor(s, model, referrer)));

    private static IEnumerable<
        (string symbolName, IntraTemplateSymbolicReference reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ArmTemplateSemanticModel model,
        BicepWildcardImportSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory
    ) => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile, referrer, referenceFactory);

    private static IEnumerable<
        (string symbolName, IntraTemplateSymbolicReference reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        TemplateSpecSemanticModel model,
        BicepWildcardImportSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory
    ) => EnumerateExportedSymbolsAsIntraTemplateSymbols(
        model,
        model.SourceFile.MainTemplateFile,
        referrer,
        referenceFactory);

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ISemanticModel model,
        ArmTemplateFile templateFile,
        BicepWildcardImportSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory
    ) => model.Exports.Values
        .Where(export => export.Kind != ExportMetadataKind.Error)
        .Select<ExportMetadata, (string, IntraTemplateSymbolicReference)>(
            md => (md.Name, ReferenceForArmTarget(md, templateFile, model, referrer, referenceFactory)));

    private static DeclaredSymbol FindExportedSymbol(ExportMetadata target, SemanticModel model)
    {
        var source = target.Kind switch
        {
            ExportMetadataKind.Type => model.Root.TypeDeclarations,
            ExportMetadataKind.Variable => model.Root.VariableDeclarations,
            ExportMetadataKind.Function => model.Root.FunctionDeclarations,
            _ => model.Root.Declarations.Where(s => s is not OutputSymbol),
        };

        return source.Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, target.Name)).Single();
    }

    private static IntraTemplateSymbolicReference ReferenceForArmTarget(
        ExportMetadata targetMetadata,
        ArmTemplateFile sourceTemplateFile,
        ISemanticModel sourceModel,
        InterTemplateSymbolicReference referrer,
        IntraTemplateSymbolicReferenceFactory referenceFactory) => targetMetadata switch
        {
            ExportedTypeMetadata => referenceFactory.SymbolFor(
                ArmSymbolType.Type,
                $"{ArmTypeRefPrefix}{targetMetadata.Name}",
                sourceTemplateFile,
                sourceModel,
                referrer),
            ExportedVariableMetadata => referenceFactory.SymbolFor(
                ArmSymbolType.Variable,
                targetMetadata.Name,
                sourceTemplateFile,
                sourceModel,
                referrer),
            ExportedFunctionMetadata => referenceFactory.SymbolFor(
                ArmSymbolType.Function,
                !targetMetadata.Name.Contains('.') ? $"{EmitConstants.UserDefinedFunctionsNamespace}.{targetMetadata.Name}" : targetMetadata.Name,
                sourceTemplateFile,
                sourceModel,
                referrer),
            _ => throw new InvalidOperationException($"Unrecognized export metadata type: {targetMetadata.GetType().Name}"),
        };

    private static ImmutableDictionary<IntraTemplateSymbolicReference, string> CalculateImportedSymbolNames(
        SemanticModel model,
        ImportClosure closure)
    {
        var importedSymbolNames = ImmutableDictionary.CreateBuilder<IntraTemplateSymbolicReference, string>(
            IntraTemplateSymbolicReferenceComparer.Instance);

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedSymbol in model.Root.ImportedSymbols)
        {
            importedSymbolNames[closure.ImportedSymbolsToIntraTemplateSymbols[importedSymbol]] = importedSymbol.Name;
        }

        int uniqueIdentifier = 1;
        ConcurrentDictionary<string, int> templateIds = new();

        // Every other symbol in the closure should be assigned a stable identifier that won't conflict with any valid Bicep identifier
        foreach (var symbol in closure.SymbolsInImportClosure.Keys
            .OrderBy(s => $"{s.SourceTemplateIdentifier}_{s.OriginalSymbolName}"))
        {
            // This symbol was imported by name and should appear in the template using the assigned identifier
            if (importedSymbolNames.ContainsKey(symbol))
            {
                continue;
            }

            var templateId = templateIds.GetOrAdd(symbol.SourceTemplateIdentifier, _ => uniqueIdentifier++);
            var symbolId = Lexer.IsValidIdentifier(symbol.OriginalSymbolName)
                ? symbol.OriginalSymbolName
                : $"_{uniqueIdentifier++}";

            importedSymbolNames.Add(symbol, $"_{templateId}.{symbolId}");
        }

        return importedSymbolNames.ToImmutable();
    }

    private static string TemplateIdentifier(IOUri entryPointUri, ISemanticModel modelToIdentify, ArtifactReference reference)
        => reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference => GetSourceFileUri(modelToIdentify).GetPathRelativeTo(entryPointUri),
            ArtifactReference otherwise => otherwise.FullyQualifiedReference,
        };

    private static IOUri GetSourceFileUri(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => bicepModel.SourceFile.FileHandle.Uri,
        ArmTemplateSemanticModel armTemplate => armTemplate.SourceFile.FileHandle.Uri,
        TemplateSpecSemanticModel templateSpec => templateSpec.SourceFile.FileHandle.Uri,
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private abstract record SymbolicReference(ISemanticModel SourceModel);
    private abstract record InterTemplateSymbolicReference(SemanticModel SourceBicepModule, ArtifactReference ImportTarget)
        : SymbolicReference(SourceBicepModule);
    private record BicepWildcardImportSymbolicReference(WildcardImportSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel, ImportTarget);
    private record BicepImportedSymbolReference(ImportedSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel, ImportTarget);

    private abstract record IntraTemplateSymbolicReference(ISemanticModel SourceModel, string SourceTemplateIdentifier)
        : SymbolicReference(SourceModel)
    {
        public abstract string OriginalSymbolName { get; }
    }

    private class IntraTemplateSymbolicReferenceComparer : IEqualityComparer<IntraTemplateSymbolicReference>
    {
        internal static readonly IntraTemplateSymbolicReferenceComparer Instance = new();

        public bool Equals(IntraTemplateSymbolicReference? x, IntraTemplateSymbolicReference? y)
        {
            if (x is null)
            {
                return y is null;
            }

            return x.SourceTemplateIdentifier.Equals(y?.SourceTemplateIdentifier) &&
                x.OriginalSymbolName.Equals(y.OriginalSymbolName);
        }

        public int GetHashCode(IntraTemplateSymbolicReference obj)
            => HashCode.Combine(obj.SourceTemplateIdentifier, obj.OriginalSymbolName);
    }

    private record BicepSymbolicReference(DeclaredSymbol Symbol, SemanticModel SourceBicepModel, string SourceTemplateIdentifier)
        : IntraTemplateSymbolicReference(SourceBicepModel, SourceTemplateIdentifier)
    {
        public override string OriginalSymbolName => Symbol.Name;
    }

    private record BicepSynthesizedVariableReference(string Name, Expression Value, SemanticModel SourceBicepModel, string SourceTemplateIdentifier)
        : IntraTemplateSymbolicReference(SourceBicepModel, SourceTemplateIdentifier)
    {
        public override string OriginalSymbolName => Name;
    }

    private record ArmSymbolicReference(ArmSymbolType Type, string Identifier, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel, string SourceTemplateIdentifier)
        : IntraTemplateSymbolicReference(SourceModel, SourceTemplateIdentifier)
    {
        public override string OriginalSymbolName => Type switch
        {
            // For ARM JSON type references, the name is a JSON pointer
            // If the pointer starts with "#/definitions/" and everything after that is a valid idenfitier
            // (this will be the case for anything compiled from Bicep), use the last path segment
            ArmSymbolType.Type when Identifier.StartsWith(ArmTypeRefPrefix) &&
                    Lexer.IsValidIdentifier(Identifier[ArmTypeRefPrefix.Length..])
                => Identifier[ArmTypeRefPrefix.Length..],
            // For ARM user-defined function references, the name will be of the format "<namespace>.<name>"
            // If the namespace is "__bicep" (this will be the case for anything compiled from Bicep),
            // use everything after the '.'
            ArmSymbolType.Function when Identifier.StartsWith(BicepDefinedFunctionNamePrefix) &&
                    Lexer.IsValidIdentifier(Identifier[BicepDefinedFunctionNamePrefix.Length..])
                => Identifier[BicepDefinedFunctionNamePrefix.Length..],
            _ => Identifier,
        };
    }

    private class IntraTemplateSymbolicReferenceFactory
    {
        private readonly IOUri entryPointUri;

        internal IntraTemplateSymbolicReferenceFactory(IOUri entryPointUri)
        {
            this.entryPointUri = entryPointUri;
        }

        internal IntraTemplateSymbolicReference SymbolFor(
            DeclaredSymbol symbol,
            SemanticModel sourceBicepModel,
            InterTemplateSymbolicReference referrer
        ) => new BicepSymbolicReference(
            symbol,
            sourceBicepModel,
            TemplateIdentifier(entryPointUri, sourceBicepModel, referrer.ImportTarget));

        internal static BicepSymbolicReference SymbolFor(
            DeclaredSymbol symbol,
            BicepSymbolicReference enclosedBy
        ) => new(symbol, enclosedBy.SourceBicepModel, enclosedBy.SourceTemplateIdentifier);

        internal static BicepSynthesizedVariableReference SymbolFor(
            string name,
            Expression value,
            BicepSymbolicReference enclosedBy
        ) => new(name, value, enclosedBy.SourceBicepModel, enclosedBy.SourceTemplateIdentifier);

        internal IntraTemplateSymbolicReference SymbolFor(
            ArmSymbolType type,
            string identifier,
            ArmTemplateFile armTemplateFile,
            ISemanticModel sourceModel,
            InterTemplateSymbolicReference referrer
        ) => new ArmSymbolicReference(
            type,
            identifier,
            armTemplateFile,
            sourceModel,
            TemplateIdentifier(entryPointUri, sourceModel, referrer.ImportTarget));

        internal static ArmSymbolicReference SymbolFor(
            ArmSymbolType type,
            string identifier,
            ArmSymbolicReference enclosedBy
        ) => new(
            type,
            identifier,
            enclosedBy.ArmTemplateFile,
            enclosedBy.SourceModel,
            enclosedBy.SourceTemplateIdentifier);
    }

    private record ImportClosure(
        IReadOnlyDictionary<IntraTemplateSymbolicReference, SyntaxBase> SymbolsInImportClosure,
        IReadOnlyDictionary<ImportedSymbol, IntraTemplateSymbolicReference> ImportedSymbolsToIntraTemplateSymbols,
        IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> WildcardImportPropertiesToIntraTemplateSymbols,
        ConcurrentDictionary<SemanticModel, EmitterContext> EmitterContexts);

    private record SearchQueueItem(SyntaxBase InitiallyDeclaringSyntax, SymbolicReference SymbolicReference);

    private class ArmIdentifierEqualityComparer : IEqualityComparer<ArmIdentifier>
    {
        internal static readonly ArmIdentifierEqualityComparer Instance = new();

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
