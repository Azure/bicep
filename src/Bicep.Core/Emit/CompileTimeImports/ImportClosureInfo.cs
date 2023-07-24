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

    public static ImportClosureInfo Calculate(SemanticModel model)
    {
        Dictionary<ISemanticModel, ModuleReference> importedModuleReferences = new();
        Dictionary<IntraTemplateSymbolicTypeReference, SyntaxBase> typeSymbolsInImportClosure = new();

        Dictionary<ImportedTypeSymbol, IntraTemplateSymbolicTypeReference> importedTypeSymbolsToIntraTemplateSymbols = new();
        Dictionary<WildcardImportPropertyReference, IntraTemplateSymbolicTypeReference> wildcardImportPropertiesToIntraTemplateSymbols = new();

        Queue<ImmutableList<SymbolicReference>> searchQueue = new(model.Root.TypeImports
            .Select(t => ImmutableList.Create<SymbolicReference>(new BicepImportedTypeSymbolicReference(t, model, GetImportReference(t))))
            .Concat(model.Root.WildcardImports
                .Select(w => ImmutableList.Create<SymbolicReference>(new BicepWildcardImportSymbolicReference(w, model, GetImportReference(w))))));

        while (searchQueue.Count > 0)
        {
            var current = searchQueue.Dequeue();
            var symbolicReferenceToProcess = current.Last();

            if (symbolicReferenceToProcess is BicepSymbolicTypeReference bicepSymbolicReference)
            {
                if (typeSymbolsInImportClosure.TryAdd(bicepSymbolicReference, GetInitiallyDeclaringSyntax(current)))
                {
                    foreach (var reference in SymbolicReferenceCollector.CollectReferences(bicepSymbolicReference))
                    {
                        searchQueue.Enqueue(current.Add(reference));
                    }
                }
            }
            else if (symbolicReferenceToProcess is BicepWildcardImportSymbolicReference wildcardImportSymbolicReference)
            {
                var targetModel = GetImportedModel(wildcardImportSymbolicReference.Symbol);
                importedModuleReferences[targetModel] = wildcardImportSymbolicReference.ImportTarget;

                foreach (var (propertyName, exportedSymbol) in EnumerateExportedSymbolsAsIntraTemplateSymbols(targetModel))
                {
                    wildcardImportPropertiesToIntraTemplateSymbols[new(wildcardImportSymbolicReference.Symbol, propertyName)] = exportedSymbol;
                    searchQueue.Enqueue(current.Add(exportedSymbol));
                }
            }
            else if (symbolicReferenceToProcess is BicepImportedTypeSymbolicReference importedTypeSymbolicReference)
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
                searchQueue.Enqueue(current.Add(target));
            }
            else if (symbolicReferenceToProcess is ArmSymbolicTypeReference armSymbolicTypeReference)
            {
                if (typeSymbolsInImportClosure.TryAdd(armSymbolicTypeReference, GetInitiallyDeclaringSyntax(current)))
                {
                    foreach (var reference in EnumerateTypeReferences(armSymbolicTypeReference.ArmTemplateFile, armSymbolicTypeReference.TypePointer))
                    {
                        searchQueue.Enqueue(current.Add(
                            new ArmSymbolicTypeReference(reference, armSymbolicTypeReference.ArmTemplateFile, armSymbolicTypeReference.SourceModel)));
                    }
                }
            }
            else
            {
                throw new InvalidOperationException($"Unexpected symbolic reference type of {symbolicReferenceToProcess.GetType().Name} encountered.");
            }
        }

        Dictionary<IntraTemplateSymbolicTypeReference, string> importedTypeSymbolNames = new();

        // Every symbol explicitly imported into the model by name should keep that name in the compiled template.
        foreach (var importedType in model.Root.TypeImports)
        {
            importedTypeSymbolNames[importedTypeSymbolsToIntraTemplateSymbols[importedType]] = importedType.Name;
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
        foreach (var symbol in typeSymbolsInImportClosure.Keys)
        {
            // This symbol was imported by name and should appear in the template using the assigned identifier
            if (importedTypeSymbolNames.ContainsKey(symbol))
            {
                continue;
            }

            importedTypeSymbolNames.Add(symbol,
                $"{AlphanumericTemplateIdentifier(model, symbol.SourceModel, importedModuleReferences[symbol.SourceModel])}.{AlphanumericTypeSymbolIdentifier(symbol)}");
        }

        Dictionary<string, DeclaredTypeExpression> importedTypes = new();
        ConcurrentDictionary<Template, ArmTypeToExpressionConverter> jsonTemplateTypeConverters = new();
        ConcurrentDictionary<SemanticModel, ExpressionBuilder> bicepExpressionBuilders = new();
        ConcurrentDictionary<SemanticModel, ImportedTypeDeclarationMigrator> importedBicepTypeExpressionRewriters = new();

        foreach (var (symbol, name) in importedTypeSymbolNames)
        {
            importedTypes.Add(name, symbol switch
            {
                ArmSymbolicTypeReference armSymbolRef
                    => jsonTemplateTypeConverters.GetOrAdd(armSymbolRef.ArmTemplateFile.Template ?? throw new InvalidOperationException($"Unable to parse {armSymbolRef.ArmTemplateFile.FileUri} as ARM template"),
                        t => new(SchemaValidationContext.ForTemplate(t),
                            importedTypeSymbolNames.Keys.OfType<ArmSymbolicTypeReference>()
                                .Where(@ref => @ref.SourceModel == armSymbolRef.SourceModel)
                                .ToImmutableDictionary(@ref => @ref.TypePointer, @ref => importedTypeSymbolNames[@ref]),
                            typeSymbolsInImportClosure[armSymbolRef]))
                        .ConvertToExpression(importedTypeSymbolNames[armSymbolRef], armSymbolRef.TypePointer),
                BicepSymbolicTypeReference bicepSymbolRef
                    => importedBicepTypeExpressionRewriters.GetOrAdd(bicepSymbolRef.SourceBicepModel,
                        m => new(m,
                            importedTypeSymbolNames,
                            importedTypeSymbolsToIntraTemplateSymbols,
                            wildcardImportPropertiesToIntraTemplateSymbols,
                            typeSymbolsInImportClosure[bicepSymbolRef]))
                        .RewriteForMigration((DeclaredTypeExpression) bicepExpressionBuilders.GetOrAdd(bicepSymbolRef.SourceBicepModel, m => new(new(m)))
                            .Convert(bicepSymbolRef.Symbol.DeclaringType)),
                _ => throw new InvalidOperationException($"Unexpected symbolic reference type of {symbol.GetType().Name} encountered."),
            });
        }

        return new(ImmutableArray.CreateRange(importedTypes.Values.OrderBy(dte => dte.Name)),
            model.Root.WildcardImports
                .SelectMany(w => GetImportedModel(w).ExportedTypes.Keys.Select(k => new WildcardImportPropertyReference(w, k)))
                .ToImmutableDictionary(@ref => @ref, @ref => importedTypeSymbolNames[wildcardImportPropertiesToIntraTemplateSymbols[@ref]]));
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

    private static SyntaxBase GetInitiallyDeclaringSyntax(ImmutableList<SymbolicReference> referenceChain) => referenceChain.First() switch
    {
        BicepWildcardImportSymbolicReference wildcardImport => wildcardImport.Symbol.DeclaringSyntax,
        BicepImportedTypeSymbolicReference importedType => importedType.Symbol.DeclaringSyntax,
        var otherwise => throw new ArgumentException($"Reference chains must start with a reference to an imported symbol, but chain started with reference of type {otherwise.GetType().Name}"),
    };

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

    private static IEnumerable<string> EnumerateTypeReferences(ArmTemplateFile templateFile, string entryPointTypeRef)
        => EnumerateTypeReferences(DerefArmType(templateFile, entryPointTypeRef));

    private static ITemplateSchemaNode DerefArmType(ArmTemplateFile templateFile, string typePointer)
    {
        if (templateFile.Template is not {} template)
        {
            throw new InvalidOperationException($"Source template of {templateFile.FileUri} is not valid");
        }

        return DerefArmType(template, typePointer);
    }

    private static ITemplateSchemaNode DerefArmType(Template template, string typePointer)
    {
        // TODO make LocalSchemaRefResolver in Azure.Deployments.Templates public
        if (!typePointer.StartsWith(ArmTypeRefPrefix) ||
            typePointer.Substring(ArmTypeRefPrefix.Length).Contains('/') ||
            !template.Definitions.TryGetValue(typePointer.Substring(ArmTypeRefPrefix.Length), out var typeDefinition))
        {
            throw new InvalidOperationException($"Invalid ARM template type reference ({typePointer}) encountered");
        }

        return typeDefinition;
    }

    private static IEnumerable<string> EnumerateTypeReferences(ITemplateSchemaNode schemaNode)
    {
        if (schemaNode.Ref?.Value is string @ref)
        {
            yield return @ref;
        }

        if (schemaNode.AdditionalProperties?.SchemaNode is {} addlPropertiesType)
        {
            foreach (var nested in EnumerateTypeReferences(addlPropertiesType))
            {
                yield return nested;
            }
        }

        if (schemaNode.Properties is {} properties)
        {
            foreach (var nested in properties.Values.SelectMany(EnumerateTypeReferences))
            {
                yield return nested;
            }
        }

        if (schemaNode.Items?.SchemaNode is {} itemsType)
        {
            foreach (var nested in EnumerateTypeReferences(itemsType))
            {
                yield return nested;
            }
        }

        if (schemaNode.PrefixItems is {} prefixItemTypes)
        {
            foreach (var nested in prefixItemTypes.SelectMany(EnumerateTypeReferences))
            {
                yield return nested;
            }
        }
    }

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

    private class SymbolicReferenceCollector : AstVisitor
    {
        private readonly HashSet<SymbolicReference> references = new();
        private readonly SemanticModel model;

        private SymbolicReferenceCollector(SemanticModel model)
        {
            this.model = model;
        }

        internal static IEnumerable<SymbolicReference> CollectReferences(BicepSymbolicTypeReference symbolicReference)
        {
            SymbolicReferenceCollector collector = new(symbolicReference.SourceBicepModel);
            symbolicReference.Symbol.DeclaringSyntax.Accept(collector);
            return collector.references;
        }

        public override void VisitVariableAccessSyntax(VariableAccessSyntax syntax)
        {
            if (ConvertToSymbolicReference(syntax) is {} symbolicReference)
            {
                references.Add(symbolicReference);
            }
        }

        private SymbolicReference? ConvertToSymbolicReference(VariableAccessSyntax variableAccess) => model.GetSymbolInfo(variableAccess) switch
        {
            // ambient types are always available; no need to migrate a definition into the target template
            AmbientTypeSymbol => null,
            // this was the base expression of a fully qualified ambient type reference (e.g., sys.string)
            BuiltInNamespaceSymbol or ProviderNamespaceSymbol => null,
            TypeAliasSymbol typeAlias => new BicepSymbolicTypeReference(typeAlias, model),
            ImportedTypeSymbol importedType => new BicepImportedTypeSymbolicReference(importedType, model, GetImportReference(importedType)),
            WildcardImportSymbol wildcardImport => new BicepWildcardImportSymbolicReference(wildcardImport, model, GetImportReference(wildcardImport)),
            Symbol otherwise => throw new InvalidOperationException($"Invalid symbol {otherwise.Name} of type {otherwise.GetType().Name} encountered within a type expression"),
            _ => throw new InvalidOperationException($"Unable to find symbol named {variableAccess.Name.IdentifierName}"),
        };
    }

    private class ImportedTypeDeclarationMigrator : ExpressionRewriteVisitor
    {
        private readonly SemanticModel sourceModel;
        private readonly IReadOnlyDictionary<IntraTemplateSymbolicTypeReference, string> importedTypeSymbolNames;
        private readonly IReadOnlyDictionary<ImportedTypeSymbol, IntraTemplateSymbolicTypeReference> importedTypeSymbolsToIntraTemplateSymbols;
        private readonly IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicTypeReference> wildcardImportPropertiesToIntraTemplateSymbols;
        private readonly SyntaxBase? sourceSyntax;

        public ImportedTypeDeclarationMigrator(SemanticModel sourceModel,
            IReadOnlyDictionary<IntraTemplateSymbolicTypeReference, string> importedTypeSymbolNames,
            IReadOnlyDictionary<ImportedTypeSymbol, IntraTemplateSymbolicTypeReference> importedTypeSymbolsToIntraTemplateSymbols,
            IReadOnlyDictionary<WildcardImportPropertyReference, IntraTemplateSymbolicTypeReference> wildcardImportPropertiesToIntraTemplateSymbols,
            SyntaxBase? sourceSyntax)
        {
            this.sourceModel = sourceModel;
            this.importedTypeSymbolNames = importedTypeSymbolNames;
            this.importedTypeSymbolsToIntraTemplateSymbols = importedTypeSymbolsToIntraTemplateSymbols;
            this.wildcardImportPropertiesToIntraTemplateSymbols = wildcardImportPropertiesToIntraTemplateSymbols;
            this.sourceSyntax = sourceSyntax;
        }

        public TExpression RewriteForMigration<TExpression>(TExpression expression) where TExpression : Expression
        {
            var processed = Replace(expression);
            if (processed is not TExpression newExpressionTyped)
            {
                throw new InvalidOperationException($"Expected {nameof(processed)} to be of type {typeof(TExpression)}");
            }

            return newExpressionTyped;
        }

        protected override Expression Replace(Expression current)
            => base.Replace(current) with { SourceSyntax = sourceSyntax };

        public override Expression ReplaceDeclaredTypeExpression(DeclaredTypeExpression expression)
            => new DeclaredTypeExpression(sourceSyntax,
                importedTypeSymbolNames[new BicepSymbolicTypeReference(LookupTypeAliasByName(expression.Name), sourceModel)],
                RewriteForMigration(expression.Value),
                expression.Description,
                expression.Metadata,
                expression.Secure,
                expression.MinLength,
                expression.MaxLength,
                expression.MinValue,
                expression.MaxValue,
                expression.Sealed,
                // An imported type is never automatically re-exported
                Exported: null);

        public override Expression ReplaceTypeAliasReferenceExpression(TypeAliasReferenceExpression expression)
            => new TypeAliasReferenceExpression(sourceSyntax,
                importedTypeSymbolNames[new BicepSymbolicTypeReference(LookupTypeAliasByName(expression.Name), sourceModel)],
                expression.ExpressedType);

        public override Expression ReplaceImportedTypeReferenceExpression(ImportedTypeReferenceExpression expression)
            => new TypeAliasReferenceExpression(sourceSyntax,
                importedTypeSymbolNames[importedTypeSymbolsToIntraTemplateSymbols[expression.Symbol]],
                expression.ExpressedType);

        public override Expression ReplaceWildcardImportPropertyReferenceExpression(WildcardImportPropertyReferenceExpression expression)
            => new TypeAliasReferenceExpression(sourceSyntax,
                importedTypeSymbolNames[wildcardImportPropertiesToIntraTemplateSymbols[new(expression.ImportSymbol, expression.PropertyName)]],
                expression.ExpressedType);

        private TypeAliasSymbol LookupTypeAliasByName(string name) => sourceModel.Root.TypeDeclarations
            .Where(t => LanguageConstants.IdentifierComparer.Equals(t.Name, name))
            .Single();
    }
}
