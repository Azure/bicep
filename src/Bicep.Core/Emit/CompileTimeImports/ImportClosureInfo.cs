// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Expression.Engines;
using Azure.Deployments.Expression.Expressions;
using Bicep.Core.ArmHelpers;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;
using Newtonsoft.Json.Linq;

namespace Bicep.Core.Emit.CompileTimeImports;

internal record WildcardImportPropertyReference(WildcardImportSymbol WildcardImport, string PropertyName) {}

internal record ImportedSymbolOriginMetadata(string SourceTemplateIdentifier, string OriginalName) {}

internal record ImportClosureInfo(ImmutableArray<DeclaredTypeExpression> ImportedTypesInClosure,
    ImmutableArray<DeclaredVariableExpression> ImportedVariablesInClosure,
    ImmutableDictionary<WildcardImportPropertyReference, string> WildcardPropertyReferenceToImportedSymbolName,
    ImmutableDictionary<string, ImportedSymbolOriginMetadata> ImportedSymbolOriginMetadata)
{
    private const string ArmTypeRefPrefix = "#/definitions/";

    public static ImportClosureInfo Calculate(SemanticModel model)
    {
        var closure = CalculateImportClosure(model);
        var closureMetadata = CalculateImportedSymbolMetadata(model, closure);

        var importedBicepSymbolNames = closureMetadata.Keys.OfType<BicepSymbolicReference>()
            .ToImmutableDictionary(@ref => @ref.Symbol, @ref => closureMetadata[@ref].UniqueNameWithinClosure);
        var importedArmTypeNamesByFile = closureMetadata.Keys.OfType<ArmSymbolicTypeReference>()
            .ToLookup(@ref => @ref.ArmTemplateFile)
            .ToImmutableDictionary(grouping => grouping.Key,
                grouping => grouping.ToImmutableDictionary(@ref => @ref.TypePointer, @ref => closureMetadata[@ref].UniqueNameWithinClosure));
        var importedArmVariableNamesByFile = closureMetadata.Keys.OfType<ArmSymbolicVariableReference>()
            .ToLookup(@ref => @ref.ArmTemplateFile)
            .ToImmutableDictionary(grouping => grouping.Key,
                grouping => grouping.ToImmutableDictionary(@ref => @ref.VariableName, @ref => closureMetadata[@ref].UniqueNameWithinClosure));
        var importSymbolNames = closure.ImportedSymbolsToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value].UniqueNameWithinClosure);
        var wildcardImportPropertyNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value].UniqueNameWithinClosure);

        Dictionary<string, DeclaredTypeExpression> importedTypes = new();
        Dictionary<string, DeclaredVariableExpression> importedVariables = new();
        var importedSymbolMetadata = ImmutableDictionary.CreateBuilder<string, ImportedSymbolOriginMetadata>();

        ConcurrentDictionary<SemanticModel, ExpressionBuilder> bicepExpressionBuilders = new();

        foreach (var (symbol, (originMetadata, name)) in closureMetadata)
        {
            importedSymbolMetadata.Add(name, originMetadata);
            switch (symbol)
            {
                case ArmSymbolicTypeReference armTypeRef:
                    importedTypes.Add(name, new ArmTypeToExpressionConverter(closure.ArmSchemaContexts[armTypeRef.ArmTemplateFile], importedArmTypeNamesByFile[armTypeRef.ArmTemplateFile], closure.SymbolsInImportClosure[armTypeRef])
                        .ConvertToExpression(closureMetadata[armTypeRef].UniqueNameWithinClosure, armTypeRef.TypePointer));
                    break;
                case BicepSymbolicReference bicepSymbolRef when bicepSymbolRef.Symbol is TypeAliasSymbol importedType:
                    importedTypes.Add(name, new ImportedSymbolDeclarationMigrator(bicepSymbolRef.SourceBicepModel, importedBicepSymbolNames, importSymbolNames, wildcardImportPropertyNames, closure.SymbolsInImportClosure[bicepSymbolRef])
                        .RewriteForMigration((DeclaredTypeExpression) bicepExpressionBuilders.GetOrAdd(bicepSymbolRef.SourceBicepModel, m => new(new(m))).Convert(importedType.DeclaringType)));
                    break;
                case ArmSymbolicVariableReference armVariableRef:
                    importedVariables.Add(name, new ArmVariableToExpressionConverter(closure.ArmVariablesEvaluators[armVariableRef.ArmTemplateFile], importedArmVariableNamesByFile[armVariableRef.ArmTemplateFile], closure.SymbolsInImportClosure[armVariableRef])
                        .ConvertToExpression(closureMetadata[armVariableRef].UniqueNameWithinClosure, armVariableRef.VariableName));
                    break;
                case BicepSymbolicReference bicepSymbolRef when bicepSymbolRef.Symbol is VariableSymbol importedVariable:
                    importedVariables.Add(name, new ImportedSymbolDeclarationMigrator(bicepSymbolRef.SourceBicepModel, importedBicepSymbolNames, importSymbolNames, wildcardImportPropertyNames, closure.SymbolsInImportClosure[bicepSymbolRef])
                        .RewriteForMigration((DeclaredVariableExpression) bicepExpressionBuilders.GetOrAdd(bicepSymbolRef.SourceBicepModel, m => new(new(m))).Convert(importedVariable.DeclaringVariable)));
                    break;
            }
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            ImmutableArray.CreateRange(importedVariables.Values.OrderBy(dve => dve.Name)),
            model.Root.WildcardImports
                .SelectMany(w => GetImportedModel(w).Exports.Keys.Select(k => new WildcardImportPropertyReference(w, k)))
                .ToImmutableDictionary(@ref => @ref, @ref => closureMetadata[closure.WildcardImportPropertiesToIntraTemplateSymbols[@ref]].UniqueNameWithinClosure),
            importedSymbolMetadata.ToImmutable());
    }

    private static ImportClosure CalculateImportClosure(SemanticModel model)
    {
        Dictionary<ISemanticModel, ArtifactReference> importedModuleReferences = new();
        Dictionary<IntraTemplateSymbolicReference, SyntaxBase> symbolsInImportClosure = new();
        Dictionary<ImportedSymbol, IntraTemplateSymbolicReference> importedSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> wildcardImportPropertiesToIntraTemplateSymbols = new();
        ConcurrentDictionary<ArmTemplateFile, SchemaValidationContext> armSchemaContextsByFile = new();
        ConcurrentDictionary<ArmTemplateFile, TemplateVariablesEvaluator> armVariablesEvaluatorsByFile = new();

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
                }
            }
            else if (item.SymbolicReference is BicepWildcardImportSymbolicReference wildcardImportSymbolicReference)
            {
                var targetModel = GetImportedModel(wildcardImportSymbolicReference.Symbol);
                importedModuleReferences[targetModel] = wildcardImportSymbolicReference.ImportTarget;

                foreach (var (propertyName, exportedSymbol) in EnumerateExportedSymbolsAsIntraTemplateSymbols(targetModel))
                {
                    wildcardImportPropertiesToIntraTemplateSymbols[new(wildcardImportSymbolicReference.Symbol, propertyName)] = exportedSymbol;
                    searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, exportedSymbol));
                }
            }
            else if (item.SymbolicReference is BicepImportedSymbolReference importedSymbolReference)
            {
                var targetModel = GetImportedModel(importedSymbolReference.Symbol);
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
                        => ReferenceForArmTarget(name, exportMetadata, targetArmModel.SourceFile, targetModel),
                    TemplateSpecSemanticModel targetTemplateSpecModel
                        => ReferenceForArmTarget(name, exportMetadata, targetTemplateSpecModel.SourceFile.MainTemplateFile, targetModel),
                    _ => throw new InvalidOperationException($"Unrecognized module type {targetModel.GetType().Name} encountered"),
                };

                importedSymbolsToIntraTemplateSymbols[importedSymbolReference.Symbol] = target;
                searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, target));
            }
            else if (item.SymbolicReference is ArmSymbolicTypeReference armSymbolicTypeReference)
            {
                if (symbolsInImportClosure.TryAdd(armSymbolicTypeReference, item.InitiallyDeclaringSyntax))
                {
                    foreach (var reference in ArmTemplateHelpers.EnumerateTypeReferencesUsedIn(
                        armSchemaContextsByFile.GetOrAdd(armSymbolicTypeReference.ArmTemplateFile, ArmTemplateHelpers.ContextFor),
                        armSymbolicTypeReference.TypePointer))
                    {
                        searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax,
                            new ArmSymbolicTypeReference(reference, armSymbolicTypeReference.ArmTemplateFile, armSymbolicTypeReference.SourceModel)));
                    }
                }
            }
            else if (item.SymbolicReference is ArmSymbolicVariableReference armSymbolicVariableReference)
            {
                if (symbolsInImportClosure.TryAdd(armSymbolicVariableReference, item.InitiallyDeclaringSyntax))
                {
                    foreach (var variableReferenced in ArmTemplateHelpers.EnumerateVariableReferencesUsedIn(
                        armVariablesEvaluatorsByFile.GetOrAdd(armSymbolicVariableReference.ArmTemplateFile, ArmTemplateHelpers.VariablesEvaluatorFor),
                        armSymbolicVariableReference.VariableName))
                    {
                        searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax,
                            new ArmSymbolicVariableReference(variableReferenced, armSymbolicVariableReference.ArmTemplateFile, armSymbolicVariableReference.SourceModel)));
                    }
                }
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
            armSchemaContextsByFile,
            armVariablesEvaluatorsByFile);
    }

    private static ArtifactReference GetImportReference(ImportedSymbol symbol)
    {
        if (symbol.TryGetModuleReference().IsSuccess(out var moduleReference))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static ArtifactReference GetImportReference(WildcardImportSymbol symbol)
    {
        if (symbol.TryGetModuleReference().IsSuccess(out var moduleReference))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static IEnumerable<SymbolicReference> CollectReferences(BicepSymbolicReference typeReference)
        => SymbolicReferenceCollector.CollectSymbolsReferenced(typeReference.SourceBicepModel.Binder, typeReference.Symbol)
            .Select<DeclaredSymbol, SymbolicReference?>(symbol => symbol switch
            {
                ProviderNamespaceSymbol => null, // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
                LocalVariableSymbol => null, // local variables are contained within the expression and are not external references
                TypeAliasSymbol typeAlias => new BicepSymbolicReference(typeAlias, typeReference.SourceBicepModel),
                VariableSymbol variable => new BicepSymbolicReference(variable, typeReference.SourceBicepModel),
                ImportedSymbol imported => new BicepImportedSymbolReference(imported, typeReference.SourceBicepModel, GetImportReference(imported)),
                WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbolicReference(wildcardImport, typeReference.SourceBicepModel, GetImportReference(wildcardImport)),
                _ => throw new InvalidOperationException($"Invalid symbol {symbol.Name} of type {symbol.GetType().Name} encountered within a export expression"),
            })
            .WhereNotNull();

    private static ISemanticModel GetImportedModel(ImportedSymbol symbol)
    {
        if (symbol.TryGetSemanticModel() is ISemanticModel model)
        {
            return model;
        }

        throw new InvalidOperationException("Unable to load model for import statement");
    }

    private static ISemanticModel GetImportedModel(WildcardImportSymbol symbol)
    {
        if (symbol.TryGetSemanticModel() is ISemanticModel model)
        {
            return model;
        }

        throw new InvalidOperationException("Unable to load model for import statement");
    }

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
                .Select<VariableSymbol, (string, IntraTemplateSymbolicReference)>(v => (v.Name, new BicepSymbolicReference(v, model))));

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ArmTemplateSemanticModel model)
        => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile);

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(TemplateSpecSemanticModel model)
        => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile.MainTemplateFile);

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ISemanticModel model, ArmTemplateFile templateFile)
        => model.Exports.Values.Select<ExportMetadata, (string, IntraTemplateSymbolicReference)>(md => md switch
        {
            ExportedTypeMetadata => (md.Name, new ArmSymbolicTypeReference($"{ArmTypeRefPrefix}{md.Name}", templateFile, model)),
            ExportedVariableMetadata => (md.Name, new ArmSymbolicVariableReference(md.Name, templateFile, model)),
            _ => throw new InvalidOperationException($"Unrecognized export metadata type: {md.GetType().Name}"),
        });

    private static DeclaredSymbol FindSymbolNamed(string nameOfSymbolSought, SemanticModel model)
        => model.Root.Declarations.Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, nameOfSymbolSought)).Single();

    private static IntraTemplateSymbolicReference ReferenceForArmTarget(string targetName, ExportMetadata targetMetadata, ArmTemplateFile sourceTemplateFile, ISemanticModel sourceModel)
        => targetMetadata switch
        {
            ExportedTypeMetadata => new ArmSymbolicTypeReference($"{ArmTypeRefPrefix}{targetName}", sourceTemplateFile, sourceModel),
            ExportedVariableMetadata => new ArmSymbolicVariableReference(targetName, sourceTemplateFile, sourceModel),
            _ => throw new InvalidOperationException($"Unrecognized export metadata type: {targetMetadata.GetType().Name}"),
        };

    private static ImmutableDictionary<IntraTemplateSymbolicReference, (ImportedSymbolOriginMetadata OriginMetadata, string UniqueNameWithinClosure)>
    CalculateImportedSymbolMetadata(SemanticModel model, ImportClosure closure)
    {
        var importedSymbolMetadata = ImmutableDictionary.CreateBuilder<IntraTemplateSymbolicReference, (ImportedSymbolOriginMetadata, string)>();

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedType in model.Root.ImportedSymbols)
        {
            var importTarget = closure.ImportedSymbolsToIntraTemplateSymbols[importedType];
            importedSymbolMetadata[closure.ImportedSymbolsToIntraTemplateSymbols[importedType]] = (
                new(TemplateIdentifier(model, importTarget.SourceModel, closure.ImportedModuleReferences[importTarget.SourceModel]), SymbolIdentifier(importTarget)),
                importedType.Name);
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
            var symbolId = Lexer.IsValidIdentifier(originalSymbolName) ? originalSymbolName : $"{uniqueIdentifier++}";

            importedSymbolMetadata.Add(symbolInfo, (new(sourceTemplateIdentifier, originalSymbolName), $"{templateId}.{symbolId}"));
        }

        return importedSymbolMetadata.ToImmutable();
    }

    private static string TemplateIdentifier(SemanticModel entryPointModel, ISemanticModel modelToIdentify, ArtifactReference reference)
        => reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference localModule => entryPointModel.SourceFile.FileUri.MakeRelativeUri(GetSourceFileUri(modelToIdentify)).ToString(),
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
        ArmSymbolicVariableReference armSymbolicVariableReference => armSymbolicVariableReference.VariableName,
        // For ARM JSON type references, the name is a JSON pointer
        // If the pointer starts with "#/definitions/" and everything after that is a valid idenfitier (this will be the case for anything compiled from Bicep), use the last path segment
        ArmSymbolicTypeReference armTypeRef when armTypeRef.TypePointer.StartsWith(ArmTypeRefPrefix) && Lexer.IsValidIdentifier(armTypeRef.TypePointer[ArmTypeRefPrefix.Length..])
            => armTypeRef.TypePointer[ArmTypeRefPrefix.Length..],
        // for all other ARM type pointers, use the full pointer
        ArmSymbolicTypeReference armSymbolicTypeReference => armSymbolicTypeReference.TypePointer,
        _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {reference.GetType().Name} encountered."),
    };

    private record SymbolicReference(ISemanticModel SourceModel) {}
    private record InterTemplateSymbolicReference(SemanticModel SourceBicepModule) : SymbolicReference(SourceBicepModule) {}
    private record BicepWildcardImportSymbolicReference(WildcardImportSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel) {}
    private record BicepImportedSymbolReference(ImportedSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel) {}

    private record IntraTemplateSymbolicReference(ISemanticModel SourceModel) : SymbolicReference(SourceModel) {}
    private record BicepSymbolicReference(DeclaredSymbol Symbol, SemanticModel SourceBicepModel) : IntraTemplateSymbolicReference(SourceBicepModel) {}
    private record ArmSymbolicTypeReference(string TypePointer, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel)
        : IntraTemplateSymbolicReference(SourceModel) {}
    private record ArmSymbolicVariableReference(string VariableName, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel)
        : IntraTemplateSymbolicReference(SourceModel) {}

    private record ImportClosure(
        IReadOnlyDictionary<ISemanticModel, ArtifactReference> ImportedModuleReferences,
        IReadOnlyDictionary<IntraTemplateSymbolicReference, SyntaxBase> SymbolsInImportClosure,
        IReadOnlyDictionary<ImportedSymbol, IntraTemplateSymbolicReference> ImportedSymbolsToIntraTemplateSymbols,
        IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicReference> WildcardImportPropertiesToIntraTemplateSymbols,
        IReadOnlyDictionary<ArmTemplateFile, SchemaValidationContext> ArmSchemaContexts,
        IReadOnlyDictionary<ArmTemplateFile, TemplateVariablesEvaluator> ArmVariablesEvaluators) {}

    private record SearchQueueItem(SyntaxBase InitiallyDeclaringSyntax, SymbolicReference SymbolicReference) {}
}
