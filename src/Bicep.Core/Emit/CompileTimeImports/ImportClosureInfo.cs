// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Azure.Deployments.Core.Definitions.Schema;
using Bicep.Core.Extensions;
using Bicep.Core.Intermediate;
using Bicep.Core.Modules;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.Workspaces;
using Microsoft.WindowsAzure.ResourceStack.Common.Extensions;

namespace Bicep.Core.Emit.CompileTimeImports;

internal record WildcardImportPropertyReference(WildcardImportSymbol WildcardImport, string PropertyName) {}

internal record ImportedSymbolOriginMetadata(string SourceTemplateIdentifier, string OriginalName) {}

internal record ImportClosureInfo(ImmutableArray<DeclaredTypeExpression> ImportedTypesInClosure,
    ImmutableDictionary<WildcardImportPropertyReference, string> WildcardPropertyReferenceToImportedTypeName,
    ImmutableDictionary<string, ImportedSymbolOriginMetadata> ImportedSymbolOriginMetadata)
{
    private const string ArmTypeRefPrefix = "#/definitions/";

    public static ImportClosureInfo Calculate(SemanticModel model)
    {
        var closure = CalculateImportClosure(model);
        var importedTypeSymbolMetadata = CalculateImportedTypeMetadata(model, closure);

        var importedBicepTypeNames = importedTypeSymbolMetadata.Keys.OfType<BicepSymbolicTypeReference>()
            .ToImmutableDictionary(@ref => @ref.Symbol, @ref => importedTypeSymbolMetadata[@ref].UniqueNameWithinClosure);
        var importedArmTypeNames = importedTypeSymbolMetadata.Keys.OfType<ArmSymbolicTypeReference>()
            .ToImmutableDictionary(@ref => @ref.TypePointer, @ref => importedTypeSymbolMetadata[@ref].UniqueNameWithinClosure);
        var typeImportSymbolNames = closure.ImportedTypeSymbolsToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => importedTypeSymbolMetadata[kvp.Value].UniqueNameWithinClosure);
        var wildcardImportTypePropertyNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => importedTypeSymbolMetadata[kvp.Value].UniqueNameWithinClosure);

        Dictionary<string, DeclaredTypeExpression> importedTypes = new();
        var importedSymbolMetadata = ImmutableDictionary.CreateBuilder<string, ImportedSymbolOriginMetadata>();

        ConcurrentDictionary<SemanticModel, ExpressionBuilder> bicepExpressionBuilders = new();

        foreach (var (symbol, (originMetadata, name)) in importedTypeSymbolMetadata)
        {
            importedSymbolMetadata.Add(name, originMetadata);
            importedTypes.Add(name, symbol switch
            {
                ArmSymbolicTypeReference armSymbolRef
                    => new ArmTypeToExpressionConverter(closure.ArmSchemaContexts[armSymbolRef.ArmTemplateFile], importedArmTypeNames, closure.TypeSymbolsInImportClosure[armSymbolRef])
                        .ConvertToExpression(importedTypeSymbolMetadata[armSymbolRef].UniqueNameWithinClosure, armSymbolRef.TypePointer),

                BicepSymbolicTypeReference bicepSymbolRef
                    => new ImportedTypeDeclarationMigrator(bicepSymbolRef.SourceBicepModel, importedBicepTypeNames, typeImportSymbolNames, wildcardImportTypePropertyNames, closure.TypeSymbolsInImportClosure[bicepSymbolRef])
                        .RewriteForMigration((DeclaredTypeExpression) bicepExpressionBuilders.GetOrAdd(bicepSymbolRef.SourceBicepModel, m => new(new(m)))
                            .Convert(bicepSymbolRef.Symbol.DeclaringType)),

                _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {symbol.GetType().Name} encountered."),
            });
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            model.Root.WildcardImports
                .SelectMany(w => GetImportedModel(w).ExportedTypes.Keys.Select(k => new WildcardImportPropertyReference(w, k)))
                .ToImmutableDictionary(@ref => @ref, @ref => importedTypeSymbolMetadata[closure.WildcardImportPropertiesToIntraTemplateSymbols[@ref]].UniqueNameWithinClosure),
            importedSymbolMetadata.ToImmutable());
    }

    private static ImportClosure CalculateImportClosure(SemanticModel model)
    {
        Dictionary<ISemanticModel, ModuleReference> importedModuleReferences = new();
        Dictionary<IntraTemplateSymbolicTypeReference, SyntaxBase> typeSymbolsInImportClosure = new();
        Dictionary<ImportedTypeSymbol, IntraTemplateSymbolicTypeReference> importedTypeSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, IntraTemplateSymbolicTypeReference> wildcardImportPropertiesToIntraTemplateSymbols = new();
        ConcurrentDictionary<ArmTemplateFile, SchemaValidationContext> armSchemaContextsByFile = new();

        Queue<SearchQueueItem> searchQueue = new(model.Root.TypeImports
            .Select(t => new SearchQueueItem(t.DeclaringSyntax, new BicepImportedTypeSymbolicReference(t, model, GetImportReference(t))))
            .Concat(model.Root.WildcardImports
                .Select(w => new SearchQueueItem(w.DeclaringSyntax, new BicepWildcardImportSymbolicReference(w, model, GetImportReference(w))))));

        while (searchQueue.Count > 0)
        {
            var item = searchQueue.Dequeue();

            if (item.SymbolicReference is BicepSymbolicTypeReference bicepSymbolicReference)
            {
                if (typeSymbolsInImportClosure.TryAdd(bicepSymbolicReference, item.InitiallyDeclaringSyntax))
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
            else if (item.SymbolicReference is BicepImportedTypeSymbolicReference importedTypeSymbolicReference)
            {
                var targetModel = GetImportedModel(importedTypeSymbolicReference.Symbol);
                importedModuleReferences[targetModel] = importedTypeSymbolicReference.ImportTarget;

                IntraTemplateSymbolicTypeReference target = targetModel switch
                {
                    SemanticModel targetBicepModel => new BicepSymbolicTypeReference(
                        targetBicepModel.Root.TypeDeclarations
                            .Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, importedTypeSymbolicReference.Symbol.OriginalSymbolName)).Single(),
                        targetBicepModel),
                    ArmTemplateSemanticModel targetArmModel => new ArmSymbolicTypeReference(
                        $"{ArmTypeRefPrefix}{importedTypeSymbolicReference.Symbol.OriginalSymbolName}",
                        targetArmModel.SourceFile,
                        targetArmModel),
                    TemplateSpecSemanticModel targetTemplateSpecModel => new ArmSymbolicTypeReference(
                        $"{ArmTypeRefPrefix}{importedTypeSymbolicReference.Symbol.OriginalSymbolName}",
                        targetTemplateSpecModel.SourceFile.MainTemplateFile,
                        targetTemplateSpecModel),
                    _ => throw new InvalidOperationException($"Unrecognized module type {targetModel.GetType().Name} encountered"),
                };

                importedTypeSymbolsToIntraTemplateSymbols[importedTypeSymbolicReference.Symbol] = target;
                searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, target));
            }
            else if (item.SymbolicReference is ArmSymbolicTypeReference armSymbolicTypeReference)
            {
                if (typeSymbolsInImportClosure.TryAdd(armSymbolicTypeReference, item.InitiallyDeclaringSyntax))
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
            else
            {
                throw new InvalidOperationException($"Unexpected symbolic reference type of {item.SymbolicReference.GetType().Name} encountered.");
            }
        }

        return new(importedModuleReferences, typeSymbolsInImportClosure, importedTypeSymbolsToIntraTemplateSymbols, wildcardImportPropertiesToIntraTemplateSymbols, armSchemaContextsByFile);
    }

    private static ModuleReference GetImportReference(ImportedTypeSymbol symbol)
    {
        if (symbol.TryGetModuleReference(out var moduleReference, out _))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static ModuleReference GetImportReference(WildcardImportSymbol symbol)
    {
        if (symbol.TryGetModuleReference(out var moduleReference, out _))
        {
            return moduleReference;
        }

        throw new InvalidOperationException("Unable to load module reference for import statement");
    }

    private static IEnumerable<SymbolicReference> CollectReferences(BicepSymbolicTypeReference typeReference)
        => SymbolicReferenceCollector.CollectSymbolsReferenced(typeReference.SourceBicepModel, typeReference.Symbol)
            .Select<DeclaredSymbol, SymbolicReference?>(symbol => symbol switch
            {
                ProviderNamespaceSymbol => null, // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
                TypeAliasSymbol typeAlias => new BicepSymbolicTypeReference(typeAlias, typeReference.SourceBicepModel),
                ImportedTypeSymbol importedType => new BicepImportedTypeSymbolicReference(importedType, typeReference.SourceBicepModel, GetImportReference(importedType)),
                WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbolicReference(wildcardImport, typeReference.SourceBicepModel, GetImportReference(wildcardImport)),
                _ => throw new InvalidOperationException($"Invalid symbol {symbol.Name} of type {symbol.GetType().Name} encountered within a type expression"),
            })
            .WhereNotNull();

    private static ISemanticModel GetImportedModel(ImportedTypeSymbol symbol)
    {
        if (symbol.TryGetSemanticModel(out var model, out _))
        {
            return model;
        }

        throw new InvalidOperationException("Unable to load model for import statement");
    }

    private static ISemanticModel GetImportedModel(WildcardImportSymbol symbol)
    {
        if (symbol.TryGetSemanticModel(out var model, out _))
        {
            return model;
        }

        throw new InvalidOperationException("Unable to load model for import statement");
    }

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicTypeReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(bicepModel),
        ArmTemplateSemanticModel armModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(armModel),
        TemplateSpecSemanticModel templateSpecModel => EnumerateExportedSymbolsAsIntraTemplateSymbols(templateSpecModel),
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicTypeReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(SemanticModel model)
        => model.Root.TypeDeclarations
            .Where(s => model.ExportedTypes.ContainsKey(s.Name))
            .Select<TypeAliasSymbol, (string, IntraTemplateSymbolicTypeReference)>(t => (t.Name, new BicepSymbolicTypeReference(t, model)));

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicTypeReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(ArmTemplateSemanticModel model)
        => model.ExportedTypes.Keys
            .Select<string, (string, IntraTemplateSymbolicTypeReference)>(typeName => (typeName, new ArmSymbolicTypeReference($"{ArmTypeRefPrefix}{typeName}", model.SourceFile, model)));

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicTypeReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(TemplateSpecSemanticModel model)
        => model.ExportedTypes.Keys
            .Select<string, (string, IntraTemplateSymbolicTypeReference)>(
                typeName => (typeName, new ArmSymbolicTypeReference($"{ArmTypeRefPrefix}{typeName}", model.SourceFile.MainTemplateFile, model)));

    private static IReadOnlyDictionary<IntraTemplateSymbolicTypeReference, (ImportedSymbolOriginMetadata OriginMetadata, string UniqueNameWithinClosure)>
    CalculateImportedTypeMetadata(SemanticModel model, ImportClosure closure)
    {
        Dictionary<IntraTemplateSymbolicTypeReference, (ImportedSymbolOriginMetadata, string)> importedTypeSymbolMetadata = new();

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedType in model.Root.TypeImports)
        {
            var importTarget = closure.ImportedTypeSymbolsToIntraTemplateSymbols[importedType];
            importedTypeSymbolMetadata[closure.ImportedTypeSymbolsToIntraTemplateSymbols[importedType]] = (
                new(TemplateIdentifier(model, importTarget.SourceModel, closure.ImportedModuleReferences[importTarget.SourceModel]), TypeSymbolIdentifier(importTarget)),
                importedType.Name);
        }

        int uniqueIdentifier = 1;
        ConcurrentDictionary<string, string> templateIdentifiers = new();

        // Every other symbol in the closure should be assigned a stable identifier that won't conflict with any valid Bicep identifier
        foreach (var (symbolInfo, sourceTemplateIdentifier, originalSymbolName) in closure.TypeSymbolsInImportClosure.Keys
            .Select(symbolInfo => (symbolInfo, TemplateIdentifier(model, symbolInfo.SourceModel, closure.ImportedModuleReferences[symbolInfo.SourceModel]), TypeSymbolIdentifier(symbolInfo)))
            .OrderBy(t => $"{t.Item2}_{t.Item3}"))
        {
            // This symbol was imported by name and should appear in the template using the assigned identifier
            if (importedTypeSymbolMetadata.ContainsKey(symbolInfo))
            {
                continue;
            }

            importedTypeSymbolMetadata.Add(symbolInfo, (new(sourceTemplateIdentifier, originalSymbolName),
                $"{templateIdentifiers.GetOrAdd(sourceTemplateIdentifier, $"{uniqueIdentifier++}")}.{uniqueIdentifier++}"));
        }

        return importedTypeSymbolMetadata;
    }

    private static string TemplateIdentifier(SemanticModel entryPointModel, ISemanticModel modelToIdentify, ModuleReference reference)
        => reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference localModule => entryPointModel.SourceFile.FileUri.MakeRelativeUri(GetSourceFileUri(modelToIdentify)).ToString(),
            ModuleReference otherwise => otherwise.FullyQualifiedReference,
        };

    private static Uri GetSourceFileUri(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => bicepModel.SourceFile.FileUri,
        ArmTemplateSemanticModel armTemplate => armTemplate.SourceFile.FileUri,
        TemplateSpecSemanticModel templateSpec => templateSpec.SourceFile.FileUri,
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static string TypeSymbolIdentifier(IntraTemplateSymbolicTypeReference reference) => reference switch
    {
        BicepSymbolicTypeReference bicepSymbolicReference => bicepSymbolicReference.Symbol.Name,
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
    private record BicepWildcardImportSymbolicReference(WildcardImportSymbol Symbol, SemanticModel SourceBicepModel, ModuleReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel) {}
    private record BicepImportedTypeSymbolicReference(ImportedTypeSymbol Symbol, SemanticModel SourceBicepModel, ModuleReference ImportTarget)
        : InterTemplateSymbolicReference(SourceBicepModel) {}

    private record IntraTemplateSymbolicTypeReference(ISemanticModel SourceModel) : SymbolicReference(SourceModel) {}
    private record BicepSymbolicTypeReference(TypeAliasSymbol Symbol, SemanticModel SourceBicepModel) : IntraTemplateSymbolicTypeReference(SourceBicepModel) {}
    private record ArmSymbolicTypeReference(string TypePointer, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel)
        : IntraTemplateSymbolicTypeReference(SourceModel) {}

    private record ImportClosure(
        IReadOnlyDictionary<ISemanticModel, ModuleReference> ImportedModuleReferences,
        IReadOnlyDictionary<IntraTemplateSymbolicTypeReference, SyntaxBase> TypeSymbolsInImportClosure,
        IReadOnlyDictionary<ImportedTypeSymbol, IntraTemplateSymbolicTypeReference> ImportedTypeSymbolsToIntraTemplateSymbols,
        IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicTypeReference> WildcardImportPropertiesToIntraTemplateSymbols,
        IReadOnlyDictionary<ArmTemplateFile, SchemaValidationContext> ArmSchemaContexts) {}

    private record SearchQueueItem(SyntaxBase InitiallyDeclaringSyntax, SymbolicReference SymbolicReference) {}
}
