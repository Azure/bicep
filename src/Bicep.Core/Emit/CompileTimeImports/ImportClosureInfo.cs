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
        SymbolFromForeignTemplateFactory referenceFactory = new(model.SourceFile.FileUri);
        var closure = CalculateImportClosure(model, referenceFactory);
        var closureMetadata = CalculateImportedSymbolMetadata(model, closure);

        var importedBicepSymbolNames = closureMetadata.Keys.OfType<SymbolFromForeignBicepTemplate>()
            .ToImmutableDictionary(@ref => @ref.Symbol, @ref => closureMetadata[@ref]);
        var importedSymbolNames = closure.ImportedSymbolsToForeignTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value]);
        var wildcardImportPropertyNames = closure.WildcardImportPropertiesToIntraTemplateSymbols
            .ToImmutableDictionary(kvp => kvp.Key, kvp => closureMetadata[kvp.Value]);
        var synthesizedVariableNames = closureMetadata.Keys.OfType<SynthesizedVariableFromForeignBicepTemplate>()
            .ToLookup(@ref => @ref.SourceBicepModel)
            .ToImmutableDictionary(g => g.Key, g => g.ToImmutableDictionary(@ref => @ref.Name, @ref => closureMetadata[@ref]));

        var importedArmSymbolNamesByFile = closureMetadata.Keys.OfType<SymbolFromForeignArmTemplate>()
            .ToLookup(@ref => @ref.ArmTemplateFile)
            .ToImmutableDictionary(grouping => grouping.Key, grouping => grouping.ToImmutableDictionary(
                @ref => new ArmIdentifier(@ref.Type, @ref.Identifier),
                @ref => closureMetadata[@ref],
                ArmIdentifierEqualityComparer.Instance));
        var armDeclarationToExpressionConverters = closureMetadata.Keys.OfType<SymbolFromForeignArmTemplate>()
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
                case SymbolFromForeignArmTemplate armRef:
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
                case SymbolFromForeignBicepTemplate bicepRef:
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
                case SynthesizedVariableFromForeignBicepTemplate synthesizedVariableRef:
                    importedVariables.Add(name, new(closure.SymbolsInImportClosure[synthesizedVariableRef], name, synthesizedVariableRef.Value));
                    break;
                default:
                    throw new UnreachableException($"This switch was expected to exhaustively process all kinds of {nameof(SymbolImportedFromForeignTemplate)} but did not handle an instance of type {symbol.GetType().Name}");
            }
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            ImmutableArray.CreateRange(importedVariables.Values.OrderBy(dve => dve.Name)),
            ImmutableArray.CreateRange(importedFunctions.Values.OrderBy(dfe => dfe.Name)),
            importedSymbolNames,
            wildcardImportPropertyNames,
            importedSymbolMetadata.ToImmutable());
    }

    private static ImportClosure CalculateImportClosure(
        SemanticModel model,
        SymbolFromForeignTemplateFactory referenceFactory)
    {
        Dictionary<SymbolImportedFromForeignTemplate, SyntaxBase> symbolsInImportClosure = new(SymbolImportedFromForeignTemplateComparer.Instance);
        Dictionary<ImportedSymbol, SymbolImportedFromForeignTemplate> importedSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, SymbolImportedFromForeignTemplate> wildcardImportPropertiesToIntraTemplateSymbols = new();
        ConcurrentDictionary<ArmTemplateFile, ArmReferenceCollector> armReferenceCollectors = new();
        ConcurrentDictionary<SemanticModel, EmitterContext> bicepEmitterContexts = new();

        Queue<SearchQueueItem> searchQueue = new(model.Root.ImportedSymbols
            .Select(importedSymbol => new SearchQueueItem(importedSymbol.DeclaringSyntax, new BicepImportedSymbol(importedSymbol, model, GetImportReference(importedSymbol))))
            .Concat(model.Root.WildcardImports
                .Select(wildcardImport => new SearchQueueItem(wildcardImport.DeclaringSyntax, new BicepWildcardImportSymbol(wildcardImport, model, GetImportReference(wildcardImport))))));

        while (searchQueue.Count > 0)
        {
            var item = searchQueue.Dequeue();

            if (item.Symbol is SymbolFromForeignBicepTemplate bicepSymbol)
            {
                if (symbolsInImportClosure.TryAdd(bicepSymbol, item.InitiallyDeclaringSyntax))
                {
                    foreach (var reference in CollectReferences(bicepSymbol))
                    {
                        searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, reference));
                    }

                    foreach (var synthesizedVariableReference in CollectSynthesizedVariableReferences(
                        bicepSymbol,
                        bicepEmitterContexts.GetOrAdd(bicepSymbol.SourceBicepModel, m => new(m))))
                    {
                        symbolsInImportClosure.TryAdd(synthesizedVariableReference, item.InitiallyDeclaringSyntax);
                    }
                }
            }
            else if (item.Symbol is SymbolFromForeignArmTemplate armSymbol)
            {
                if (symbolsInImportClosure.TryAdd(armSymbol, item.InitiallyDeclaringSyntax))
                {
                    ArmIdentifier refTarget = new(armSymbol.Type, armSymbol.Identifier);
                    foreach (var reference in armReferenceCollectors.GetOrAdd(
                        armSymbol.ArmTemplateFile,
                        f => new(f)).EnumerateReferencesUsedInDefinitionOf(refTarget))
                    {
                        searchQueue.Enqueue(new(
                            item.InitiallyDeclaringSyntax,
                            SymbolFromForeignTemplateFactory.SymbolFor(
                                reference.SymbolType,
                                reference.Identifier,
                                armSymbol)));
                    }
                }
            }
            else if (item.Symbol is BicepWildcardImportSymbol wildcardImportSymbol)
            {
                foreach (var (propertyName, exportedSymbol) in EnumerateExportedSymbolsAsIntraTemplateSymbols(
                    wildcardImportSymbol.Symbol.SourceModel,
                    wildcardImportSymbol,
                    referenceFactory))
                {
                    wildcardImportPropertiesToIntraTemplateSymbols[new(wildcardImportSymbol.Symbol, propertyName)] = exportedSymbol;
                    searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, exportedSymbol));
                }
            }
            else if (item.Symbol is BicepImportedSymbol importedSymbol)
            {
                var targetModel = importedSymbol.Symbol.SourceModel;

                var name = importedSymbol.Symbol.OriginalSymbolName
                    ?? throw new InvalidOperationException($"The import symbol {importedSymbol.Symbol.Name} did not specify what symbol to import");

                if (!targetModel.Exports.TryGetValue(name, out var exportMetadata))
                {
                    throw new InvalidOperationException($"No export named {name} found in {TemplateIdentifier(
                        model.SourceFile.FileUri,
                        targetModel,
                        importedSymbol.ImportTarget)}");
                }

                SymbolImportedFromForeignTemplate target = targetModel switch
                {
                    SemanticModel targetBicepModel => referenceFactory.SymbolFor(
                        FindSymbolNamed(name, targetBicepModel),
                        targetBicepModel,
                        importedSymbol),
                    ArmTemplateSemanticModel targetArmModel => ReferenceForArmTarget(
                        exportMetadata,
                        targetArmModel.SourceFile,
                        targetModel,
                        importedSymbol,
                        referenceFactory),
                    TemplateSpecSemanticModel targetTemplateSpecModel => ReferenceForArmTarget(
                        exportMetadata,
                        targetTemplateSpecModel.SourceFile.MainTemplateFile,
                        targetModel,
                        importedSymbol,
                        referenceFactory),
                    _ => throw new InvalidOperationException($"Unrecognized module type {targetModel.GetType().Name} encountered"),
                };

                importedSymbolsToIntraTemplateSymbols[importedSymbol.Symbol] = target;
                searchQueue.Enqueue(new(item.InitiallyDeclaringSyntax, target));
            }
            else
            {
                throw new InvalidOperationException($"Unexpected symbolic reference type of {item.Symbol.GetType().Name} encountered.");
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

    private static IEnumerable<EnclosedSymbol> CollectReferences(SymbolFromForeignBicepTemplate typeReference)
        => typeReference.SourceBicepModel.Binder.GetSymbolsReferencedInDeclarationOf(typeReference.Symbol)
            .Select<DeclaredSymbol, EnclosedSymbol?>(symbol => symbol switch
            {
                ProviderNamespaceSymbol => null, // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
                LocalVariableSymbol => null, // local variables are contained within the expression and are not external references
                TypeAliasSymbol typeAlias => SymbolFromForeignTemplateFactory.SymbolFor(typeAlias, typeReference),
                VariableSymbol variable => SymbolFromForeignTemplateFactory.SymbolFor(variable, typeReference),
                DeclaredFunctionSymbol func => SymbolFromForeignTemplateFactory.SymbolFor(func, typeReference),
                ImportedSymbol imported => new BicepImportedSymbol(
                    imported,
                    typeReference.SourceBicepModel,
                    GetImportReference(imported)),
                WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbol(
                    wildcardImport,
                    typeReference.SourceBicepModel,
                    GetImportReference(wildcardImport)),
                _ => throw new InvalidOperationException(
                    $"Invalid symbol {symbol.Name} of type {symbol.GetType().Name} encountered within a export expression"),
            })
            .WhereNotNull();

    private static IEnumerable<SynthesizedVariableFromForeignBicepTemplate> CollectSynthesizedVariableReferences(
        SymbolFromForeignBicepTemplate @ref,
        EmitterContext emitterContext
    ) => SyntaxAggregator.AggregateByType<FunctionCallSyntaxBase>(@ref.Symbol.DeclaringSyntax)
        .Select(functionCallSyntax => emitterContext.FunctionVariables.TryGetValue(functionCallSyntax, out var result)
            ? SymbolFromForeignTemplateFactory.SymbolFor(result.Name, result.Value, @ref)
            : null)
        .WhereNotNull();

    private static IEnumerable<
        (string symbolName, SymbolImportedFromForeignTemplate reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ISemanticModel model,
        BicepWildcardImportSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
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
        (string symbolName, SymbolImportedFromForeignTemplate reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        SemanticModel model,
        BicepWildcardImportSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
    ) => model.Root.TypeDeclarations
        .Concat<DeclaredSymbol>(model.Root.VariableDeclarations)
        .Concat(model.Root.FunctionDeclarations)
        .Where(t => t.IsExported())
        .Select(s => (s.Name, referenceFactory.SymbolFor(s, model, referrer)));

    private static IEnumerable<
        (string symbolName, SymbolImportedFromForeignTemplate reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ArmTemplateSemanticModel model,
        BicepWildcardImportSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
    ) => EnumerateExportedSymbolsAsIntraTemplateSymbols(model, model.SourceFile, referrer, referenceFactory);

    private static IEnumerable<
        (string symbolName, SymbolImportedFromForeignTemplate reference)
    > EnumerateExportedSymbolsAsIntraTemplateSymbols(
        TemplateSpecSemanticModel model,
        BicepWildcardImportSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
    ) => EnumerateExportedSymbolsAsIntraTemplateSymbols(
        model,
        model.SourceFile.MainTemplateFile,
        referrer,
        referenceFactory);

    private static IEnumerable<(string symbolName, SymbolImportedFromForeignTemplate reference)> EnumerateExportedSymbolsAsIntraTemplateSymbols(
        ISemanticModel model,
        ArmTemplateFile templateFile,
        BicepWildcardImportSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
    ) => model.Exports.Values.Select<ExportMetadata, (string, SymbolImportedFromForeignTemplate)>(
        md => (md.Name, ReferenceForArmTarget(md, templateFile, model, referrer, referenceFactory)));

    private static DeclaredSymbol FindSymbolNamed(string nameOfSymbolSought, SemanticModel model)
        => model.Root.Declarations.Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, nameOfSymbolSought)).Single();

    private static SymbolImportedFromForeignTemplate ReferenceForArmTarget(
        ExportMetadata targetMetadata,
        ArmTemplateFile sourceTemplateFile,
        ISemanticModel sourceModel,
        InterTemplateSymbol referrer,
        SymbolFromForeignTemplateFactory referenceFactory
    )
        => targetMetadata switch
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

    private static ImmutableDictionary<SymbolImportedFromForeignTemplate, string> CalculateImportedSymbolMetadata(
        SemanticModel model,
        ImportClosure closure)
    {
        var importedSymbolNames = ImmutableDictionary.CreateBuilder<SymbolImportedFromForeignTemplate, string>(
            SymbolImportedFromForeignTemplateComparer.Instance);

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedSymbol in model.Root.ImportedSymbols)
        {
            importedSymbolNames[closure.ImportedSymbolsToForeignTemplateSymbols[importedSymbol]] = importedSymbol.Name;
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

    private static string TemplateIdentifier(Uri entryPointUri, ISemanticModel modelToIdentify, ArtifactReference reference)
        => reference switch
        {
            // for local modules, use the path on disk relative to the entry point template
            LocalModuleReference => entryPointUri.MakeRelativeUri(GetSourceFileUri(modelToIdentify)).ToString(),
            ArtifactReference otherwise => otherwise.FullyQualifiedReference,
        };

    private static Uri GetSourceFileUri(ISemanticModel model) => model switch
    {
        SemanticModel bicepModel => bicepModel.SourceFile.FileUri,
        ArmTemplateSemanticModel armTemplate => armTemplate.SourceFile.FileUri,
        TemplateSpecSemanticModel templateSpec => templateSpec.SourceFile.FileUri,
        _ => throw new InvalidOperationException($"Unrecognized module type {model.GetType().Name} encountered"),
    };

    private abstract record EnclosedSymbol(ISemanticModel SourceModel);
    private abstract record InterTemplateSymbol(SemanticModel SourceBicepModule, ArtifactReference ImportTarget)
        : EnclosedSymbol(SourceBicepModule);
    private record BicepWildcardImportSymbol(WildcardImportSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbol(SourceBicepModel, ImportTarget);
    private record BicepImportedSymbol(ImportedSymbol Symbol, SemanticModel SourceBicepModel, ArtifactReference ImportTarget)
        : InterTemplateSymbol(SourceBicepModel, ImportTarget);

    private abstract record SymbolImportedFromForeignTemplate(ISemanticModel SourceModel, string SourceTemplateIdentifier)
        : EnclosedSymbol(SourceModel)
    {
        public abstract string OriginalSymbolName { get; }
    }

    private class SymbolImportedFromForeignTemplateComparer : IEqualityComparer<SymbolImportedFromForeignTemplate>
    {
        internal static readonly SymbolImportedFromForeignTemplateComparer Instance = new();

        public bool Equals(SymbolImportedFromForeignTemplate? x, SymbolImportedFromForeignTemplate? y)
        {
            if (x is null)
            {
                return y is null;
            }

            return x.SourceTemplateIdentifier.Equals(y?.SourceTemplateIdentifier) &&
                x.OriginalSymbolName.Equals(y.OriginalSymbolName);
        }

        public int GetHashCode(SymbolImportedFromForeignTemplate obj)
            => HashCode.Combine(obj.SourceTemplateIdentifier, obj.OriginalSymbolName);
    }

    private record SymbolFromForeignBicepTemplate(DeclaredSymbol Symbol, SemanticModel SourceBicepModel, string SourceTemplateIdentifier)
        : SymbolImportedFromForeignTemplate(SourceBicepModel, SourceTemplateIdentifier)
    {
        public override string OriginalSymbolName => Symbol.Name;
    }

    private record SynthesizedVariableFromForeignBicepTemplate(string Name, Expression Value, SemanticModel SourceBicepModel, string SourceTemplateIdentifier)
        : SymbolImportedFromForeignTemplate(SourceBicepModel, SourceTemplateIdentifier)
    {
        public override string OriginalSymbolName => Name;
    }

    private record SymbolFromForeignArmTemplate(ArmSymbolType Type, string Identifier, ArmTemplateFile ArmTemplateFile, ISemanticModel SourceModel, string SourceTemplateIdentifier)
        : SymbolImportedFromForeignTemplate(SourceModel, SourceTemplateIdentifier)
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

    private class SymbolFromForeignTemplateFactory
    {
        private readonly Uri entryPointUri;

        internal SymbolFromForeignTemplateFactory(Uri entryPointUri)
        {
            this.entryPointUri = entryPointUri;
        }

        internal SymbolImportedFromForeignTemplate SymbolFor(
            DeclaredSymbol symbol,
            SemanticModel sourceBicepModel,
            InterTemplateSymbol referrer
        ) => new SymbolFromForeignBicepTemplate(
            symbol,
            sourceBicepModel,
            TemplateIdentifier(entryPointUri, sourceBicepModel, referrer.ImportTarget));

        internal static SymbolFromForeignBicepTemplate SymbolFor(
            DeclaredSymbol symbol,
            SymbolFromForeignBicepTemplate enclosedBy
        ) => new(symbol, enclosedBy.SourceBicepModel, enclosedBy.SourceTemplateIdentifier);

        internal static SynthesizedVariableFromForeignBicepTemplate SymbolFor(
            string name,
            Expression value,
            SymbolFromForeignBicepTemplate enclosedBy
        ) => new(name, value, enclosedBy.SourceBicepModel, enclosedBy.SourceTemplateIdentifier);

        internal SymbolImportedFromForeignTemplate SymbolFor(
            ArmSymbolType type,
            string identifier,
            ArmTemplateFile armTemplateFile,
            ISemanticModel sourceModel,
            InterTemplateSymbol referrer
        ) => new SymbolFromForeignArmTemplate(
            type,
            identifier,
            armTemplateFile,
            sourceModel,
            TemplateIdentifier(entryPointUri, sourceModel, referrer.ImportTarget));

        internal static SymbolFromForeignArmTemplate SymbolFor(
            ArmSymbolType type,
            string identifier,
            SymbolFromForeignArmTemplate enclosedBy
        ) => new(
            type,
            identifier,
            enclosedBy.ArmTemplateFile,
            enclosedBy.SourceModel,
            enclosedBy.SourceTemplateIdentifier);
    }

    private record ImportClosure(
        IReadOnlyDictionary<SymbolImportedFromForeignTemplate, SyntaxBase> SymbolsInImportClosure,
        IReadOnlyDictionary<ImportedSymbol, SymbolImportedFromForeignTemplate> ImportedSymbolsToForeignTemplateSymbols,
        IReadOnlyDictionary<WildcardImportPropertyReference, SymbolImportedFromForeignTemplate> WildcardImportPropertiesToIntraTemplateSymbols,
        ConcurrentDictionary<SemanticModel, EmitterContext> EmitterContexts);

    private record SearchQueueItem(SyntaxBase InitiallyDeclaringSyntax, EnclosedSymbol Symbol);

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
