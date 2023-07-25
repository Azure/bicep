// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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

internal record ImportClosureInfo(ImmutableArray<DeclaredTypeExpression> ImportedTypesInClosure,
    ImmutableDictionary<WildcardImportPropertyReference, string> WildcardPropertyReferenceToImportedTypeName)
{
    private const string ArmTypeRefPrefix = "#/definitions/";

    public static ImportClosureInfo Calculate(SemanticModel model)
    {
        var closure = CalculateImportClosure(model);

        Dictionary<IntraTemplateSymbolicTypeReference, string> importedTypeSymbolNames = new();

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedType in model.Root.TypeImports)
        {
            importedTypeSymbolNames[closure.ImportedTypeSymbolsToIntraTemplateSymbols[importedType]] = importedType.Name;
        }

        // Every other symbol in the closure should be assigned a stable identifier that won't conflict with any valid Bicep identifier
        // We have two options here, neither of them perfect:
        //   1. Assign a name that would not be a legal Bicep identifier but *is* a legal ARM identifier (e.g., "foo.bar").
        //     - This guarantees there will never be a conflict between declared symbols and symbols brought over as part of the import closure.
        //     - This will create issues for decompilation, since each one of these symbols will have to be manually renamed.
        //   2. Assign a name that would be a legal Bicep identifier but that is unlikely to be used (e.g., "__bicep_import__foo")
        //     - No decompilation issues.
        //     - There is a chance some templates will use such an identifier (particularly since it would survive decompilation!), introducing the possibility of name conflicts.
        // Going with option 1 for now but will raise this with the team.
        foreach (var symbol in closure.TypeSymbolsInImportClosure.Keys)
        {
            // This symbol was imported by name and should appear in the template using the assigned identifier
            if (importedTypeSymbolNames.ContainsKey(symbol))
            {
                continue;
            }

            importedTypeSymbolNames.Add(symbol,
                $"{AlphanumericTemplateIdentifier(model, symbol.SourceModel, closure.ImportedModuleReferences[symbol.SourceModel])}.{AlphanumericTypeSymbolIdentifier(symbol)}");
        }
        var typeImportSymbolNames = closure.ImportedTypeSymbolsToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => importedTypeSymbolNames[kvp.Value]);
        var wildcardTypeImportSymbolNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => importedTypeSymbolNames[kvp.Value]);

        Dictionary<string, DeclaredTypeExpression> importedTypes = new();
        ConcurrentDictionary<SchemaValidationContext, ArmTypeToExpressionConverter> jsonTemplateTypeConverters = new();
        ConcurrentDictionary<SemanticModel, ExpressionBuilder> bicepExpressionBuilders = new();
        ConcurrentDictionary<SemanticModel, ImportedTypeDeclarationMigrator> importedBicepTypeExpressionRewriters = new();

        foreach (var (symbol, name) in importedTypeSymbolNames)
        {
            importedTypes.Add(name, symbol switch
            {
                ArmSymbolicTypeReference armSymbolRef
                    => jsonTemplateTypeConverters.GetOrAdd(closure.ArmSchemaContexts[armSymbolRef.ArmTemplateFile],
                        context => new(context,
                            importedTypeSymbolNames.Keys.OfType<ArmSymbolicTypeReference>()
                                .Where(@ref => @ref.SourceModel == armSymbolRef.SourceModel)
                                .ToImmutableDictionary(@ref => @ref.TypePointer, @ref => importedTypeSymbolNames[@ref]),
                            closure.TypeSymbolsInImportClosure[armSymbolRef]))
                        .ConvertToExpression(importedTypeSymbolNames[armSymbolRef], armSymbolRef.TypePointer),
                BicepSymbolicTypeReference bicepSymbolRef
                    => importedBicepTypeExpressionRewriters.GetOrAdd(bicepSymbolRef.SourceBicepModel,
                        m => new(m,
                            importedTypeSymbolNames.Keys.OfType<BicepSymbolicTypeReference>()
                                .Where(bicepRef => bicepRef.SourceModel == m)
                                .ToImmutableDictionary(@ref => @ref.Symbol, @ref => importedTypeSymbolNames[@ref]),
                            typeImportSymbolNames,
                            wildcardTypeImportSymbolNames,
                            closure.TypeSymbolsInImportClosure[bicepSymbolRef]))
                        .RewriteForMigration((DeclaredTypeExpression) bicepExpressionBuilders.GetOrAdd(bicepSymbolRef.SourceBicepModel, m => new(new(m)))
                            .Convert(bicepSymbolRef.Symbol.DeclaringType)),
                _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {symbol.GetType().Name} encountered."),
            });
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            model.Root.WildcardImports
                .SelectMany(w => GetImportedModel(w).ExportedTypes.Keys.Select(k => new WildcardImportPropertyReference(w, k)))
                .ToImmutableDictionary(@ref => @ref, @ref => importedTypeSymbolNames[closure.WildcardImportPropertiesToIntraTemplateSymbols[@ref]]));
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
        => EnumerateExportedTypesAsPointers(model)
            .Select<string, (string, IntraTemplateSymbolicTypeReference)>(pointer => (pointer, new ArmSymbolicTypeReference(pointer, model.SourceFile, model)));

    private static IEnumerable<(string symbolName, IntraTemplateSymbolicTypeReference reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(TemplateSpecSemanticModel model)
        => EnumerateExportedTypesAsPointers(model)
            .Select<string, (string, IntraTemplateSymbolicTypeReference)>(
                pointer => (pointer, new ArmSymbolicTypeReference(pointer, model.SourceFile.MainTemplateFile, model)));

    private static IEnumerable<string> EnumerateExportedTypesAsPointers(ISemanticModel model)
        => model.ExportedTypes.Keys.Select(typeName => $"{ArmTypeRefPrefix}{typeName}");

    private static Uri GetSourceFileUri(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => bicepModel.SourceFile.FileUri,
        ArmTemplateSemanticModel armTemplate => armTemplate.SourceFile.FileUri,
        TemplateSpecSemanticModel templateSpec => templateSpec.SourceFile.FileUri,
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private static string AlphanumericTemplateIdentifier(SemanticModel entryPointModel, ISemanticModel modelToIdentify, ModuleReference reference)
        => Sha256Hash(reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference localModule => entryPointModel.SourceFile.FileUri.MakeRelativeUri(GetSourceFileUri(modelToIdentify)).ToString(),
            ModuleReference otherwise => otherwise.FullyQualifiedReference,
        });

    private static string AlphanumericTypeSymbolIdentifier(IntraTemplateSymbolicTypeReference reference) => reference switch
    {
        BicepSymbolicTypeReference bicepSymbolicReference => bicepSymbolicReference.Symbol.Name,
        // For ARM JSON type references, the name is a JSON pointer
        // If the pointer starts with "#/definitions/" and everything after that is a valid idenfitier (this will be the case for anything compiled from Bicep), use the last path segment
        ArmSymbolicTypeReference armTypeRef when armTypeRef.TypePointer.StartsWith(ArmTypeRefPrefix) && Lexer.IsValidIdentifier(armTypeRef.TypePointer.Substring(ArmTypeRefPrefix.Length))
            => armTypeRef.TypePointer.Substring(ArmTypeRefPrefix.Length),
        // for all other ARM type pointers, use a hash of the full pointer
        ArmSymbolicTypeReference armSymbolicTypeReference => Sha256Hash(armSymbolicTypeReference.TypePointer),
        _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {reference.GetType().Name} encountered."),
    };

    private static string Sha256Hash(string input) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(input)));

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
