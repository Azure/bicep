// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Diagnostics;
using Azure.Deployments.Core.Definitions.Schema;
using Azure.Deployments.Core.Entities;
using Azure.Deployments.Templates.Extensions;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Features;
using Bicep.Core.Modules;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Registry;
using Bicep.Core.Registry.Oci;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.SourceGraph;
using Bicep.Core.SourceLink;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using Bicep.LanguageServer.CompilationManager;
using Bicep.LanguageServer.Completions;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Providers;
using Bicep.LanguageServer.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;

namespace Bicep.LanguageServer.Handlers
{
    public class BicepDefinitionHandler(
        ISymbolResolver symbolResolver,
        ICompilationManager compilationManager,
        BicepCompiler bicepCompiler,
        ILanguageServerFacade languageServer,
        IModuleDispatcher moduleDispatcher,
        DocumentSelectorFactory documentSelectorFactory,
        ISourceFileFactory sourceFileFactory
    ) : DefinitionHandlerBase()
    {
        public override async Task<LocationOrLocationLinks?> Handle(DefinitionParams request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var context = compilationManager.GetCompilation(request.TextDocument.Uri);
            if (context is null)
            {
                return null;
            }

            var result = symbolResolver.ResolveSymbol(request.TextDocument.Uri, request.Position);

            // No parent Symbol: ad hoc syntax matching
            return result switch
            {
                null => HandleUnboundSymbolLocation(request, context),

                { Symbol: ParameterAssignmentSymbol param } => HandleParameterAssignment(request, result, context, param),

                // Used for the declaration ONLY of a wildcard import. Other syntax that resolves to a wildcard import will be handled by HandleDeclaredDefinitionLocation
                { Origin: WildcardImportSyntax, Symbol: WildcardImportSymbol wildcardImport }
                    => HandleWildcardImportDeclaration(context, wildcardImport),

                { Symbol: ImportedSymbol imported } => HandleImportedSymbolLocation(result, context, imported),

                { Symbol: WildcardImportInstanceFunctionSymbol instanceFunctionSymbol }
                    => HandleWildcardImportInstanceFunctionLocation(result, context, instanceFunctionSymbol),

                { Symbol: DeclaredSymbol declaration } => HandleDeclaredDefinitionLocation(request, result, declaration),

                // Object property: currently only used for module param goto
                { Origin: ObjectPropertySyntax } => HandleObjectPropertyLocation(request, context),

                // Used for module (name), variable, wildcard import, or resource property access
                { Symbol: PropertySymbol } => HandlePropertyLocation(request, result, context),

                _ => null,
            };
        }

        protected override DefinitionRegistrationOptions CreateRegistrationOptions(DefinitionCapability capability, ClientCapabilities clientCapabilities) => new()
        {
            DocumentSelector = documentSelectorFactory.CreateForBicepAndParams()
        };

        private LocationOrLocationLinks HandleUnboundSymbolLocation(DefinitionParams request, CompilationContext context)
        {
            int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
            { // Definition handler for a non symbol bound to implement module path goto.
                // try to resolve module path syntax from given offset using tail matching.
                if (SyntaxMatcher.IsTailMatch<ModuleDeclarationSyntax, StringSyntax, Token>(
                     matchingNodes,
                     (moduleSyntax, stringSyntax, token) => moduleSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete)
                 && matchingNodes[^3] is ModuleDeclarationSyntax moduleDeclarationSyntax
                 && matchingNodes[^2] is StringSyntax stringToken
                 && context.Compilation.SourceFileGrouping.TryGetSourceFile(moduleDeclarationSyntax).IsSuccess(out var sourceFile)
                 && moduleDispatcher.TryGetArtifactReference(context.Compilation.SourceFileGrouping.EntryPoint, moduleDeclarationSyntax).IsSuccess(out var moduleReference))
                {
                    return HandleModuleReference(context, stringToken, sourceFile, moduleReference);
                }
            }
            {
                // Handle using '<module_path>'
                if (SyntaxMatcher.IsTailMatch<UsingDeclarationSyntax, StringSyntax, Token>(matchingNodes, (usingDeclaration, usingPath, token) => token.Type == TokenType.StringComplete) &&
                    matchingNodes[^3] is UsingDeclarationSyntax usingDeclaration &&
                    matchingNodes[^2] is StringSyntax stringSyntax &&
                    context.Compilation.SourceFileGrouping.TryGetSourceFile(usingDeclaration).IsSuccess(out var sourceFile) &&
                    moduleDispatcher.TryGetArtifactReference(context.Compilation.SourceFileGrouping.EntryPoint, usingDeclaration).IsSuccess(out var moduleReference))
                {
                    return HandleModuleReference(context, stringSyntax, sourceFile, moduleReference);
                }
            }
            { // Definition handler for a non symbol bound to implement import path goto.
                // try to resolve import path syntax from given offset using tail matching.
                if (SyntaxMatcher.IsTailMatch<CompileTimeImportDeclarationSyntax, CompileTimeImportFromClauseSyntax, StringSyntax, Token>(
                     matchingNodes,
                     (_, fromClauseSyntax, stringSyntax, token) => fromClauseSyntax.Path == stringSyntax && token.Type == TokenType.StringComplete)
                 && matchingNodes[^4] is CompileTimeImportDeclarationSyntax importDeclarationSyntax
                 && matchingNodes[^2] is StringSyntax stringToken
                 && context.Compilation.SourceFileGrouping.TryGetSourceFile(importDeclarationSyntax).IsSuccess(out var sourceFile)
                 && moduleDispatcher.TryGetArtifactReference(context.Compilation.SourceFileGrouping.EntryPoint, importDeclarationSyntax).IsSuccess(out var moduleReference))
                {
                    // goto beginning of the module file.
                    return GetFileDefinitionLocation(
                        GetArtifactSourceLinkUri(sourceFile, moduleReference),
                        stringToken,
                        context,
                        new() { Start = new(0, 0), End = new(0, 0) });
                }
            }
            {  // Definition handler for a non symbol bound to implement load* functions file argument path goto.
                if (SyntaxMatcher.IsTailMatch<StringSyntax, Token>(
                        matchingNodes,
                        (stringSyntax, token) => !stringSyntax.IsInterpolated() && token.Type == TokenType.StringComplete)
                    && matchingNodes[^2] is StringSyntax stringToken
                    && context.Compilation.GetEntrypointSemanticModel().GetDeclaredType(stringToken) is { } stringType
                    && stringType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringFilePath)
                    && stringToken.TryGetLiteralValue() is { } stringTokenValue
                    && RelativePath.TryCreate(stringTokenValue).Transform(context.Compilation.SourceFileGrouping.EntryPoint.FileHandle.TryGetRelativeFile).IsSuccess(out var relativeFile)
                    && relativeFile.Exists())
                {
                    return GetFileDefinitionLocation(
                        relativeFile.Uri.ToUri(),
                        stringToken,
                        context,
                        new() { Start = new(0, 0), End = new(0, 0) });
                }
            }
            {
                if (SyntaxMatcher.GetTailMatch<UsingDeclarationSyntax, StringSyntax, Token>(matchingNodes) is (var @using, var path, _) &&
                    @using.Path == path &&
                    context.Compilation.SourceFileGrouping.TryGetSourceFile(@using).IsSuccess(out var sourceFile))
                {
                    return GetFileDefinitionLocation(
                        sourceFile.FileHandle.Uri.ToDocumentUri(),
                        path,
                        context,
                        new() { Start = new(0, 0), End = new(0, 0) });
                }
            }

            // all other unbound syntax nodes return no
            return new();
        }

        private LocationOrLocationLinks HandleModuleReference(CompilationContext context, StringSyntax stringToken, ISourceFile sourceFile, ArtifactReference reference)
        {
            // Return the correct link format so our language client can display the sources
            return GetFileDefinitionLocation(
                GetArtifactSourceLinkUri(sourceFile, reference),
                stringToken,
                context,
                new() { Start = new(0, 0), End = new(0, 0) });
        }

        private DocumentUri GetArtifactSourceLinkUri(ISourceFile sourceFile, ArtifactReference reference)
        {
            if (!this.CanClientAcceptRegistryContent() || !reference.IsExternal)
            {
                // the client doesn't support the bicep-extsrc scheme or we're dealing with a local module
                // just use the file URI
                return sourceFile.FileHandle.Uri.ToDocumentUri();
            }

            if (reference is OciArtifactReference ociArtifactReference)
            {
                return BicepExternalSourceRequestHandler.GetRegistryModuleSourceLinkUri(ociArtifactReference, ociArtifactReference.TryLoadSourceArchive().TryUnwrap());
            }

            if (reference is TemplateSpecModuleReference templateSpecModuleReference)
            {
                return BicepExternalSourceRequestHandler.GetTemplateSpecSourceLinkUri(templateSpecModuleReference);
            }

            throw new UnreachableException();
        }

        private LocationOrLocationLinks HandleWildcardImportDeclaration(CompilationContext context, WildcardImportSymbol wildcardImport)
        {
            if (context.Compilation.SourceFileGrouping.TryGetSourceFile(wildcardImport.EnclosingDeclaration).IsSuccess(out var sourceFile) &&
                moduleDispatcher.TryGetArtifactReference(context.Compilation.SourceFileGrouping.EntryPoint, wildcardImport.EnclosingDeclaration).IsSuccess(out var moduleReference))
            {
                return GetFileDefinitionLocation(
                    GetArtifactSourceLinkUri(sourceFile, moduleReference),
                    wildcardImport.DeclaringSyntax,
                    context,
                    new() { Start = new(0, 0), End = new(0, 0) });
            }

            return new();
        }

        private static LocationOrLocationLinks HandleDeclaredDefinitionLocation(DefinitionParams request, SymbolResolutionResult result, DeclaredSymbol declaration)
        {
            return new(new LocationOrLocationLink(new LocationLink
            {
                // source of the link. Underline only the symbolic name
                OriginSelectionRange = (result.Origin is ITopLevelNamedDeclarationSyntax named ? named.Name : result.Origin).ToRange(result.Context.LineStarts),
                TargetUri = request.TextDocument.Uri,

                // entire span of the declaredSymbol
                TargetRange = declaration.DeclaringSyntax.ToRange(result.Context.LineStarts),
                TargetSelectionRange = declaration.NameSource.ToRange(result.Context.LineStarts)
            }));
        }

        private LocationOrLocationLinks HandleObjectPropertyLocation(DefinitionParams request, CompilationContext context)
        {
            int offset = PositionHelper.GetOffset(context.LineStarts, request.Position);
            var matchingNodes = SyntaxMatcher.FindNodesMatchingOffset(context.Compilation.SourceFileGrouping.EntryPoint.ProgramSyntax, offset);
            // matchingNodes[0] should be ProgramSyntax
            if (matchingNodes[1] is ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                // capture the property accesses leading to this specific property access
                var propertyAccesses = matchingNodes.OfType<ObjectPropertySyntax>().ToList();
                // only two level of traversals: mod { params: { <outputName1>: ...}}
                if (propertyAccesses.Count == 2 &&
                    propertyAccesses[0].TryGetKeyText() is { } propertyType &&
                    propertyAccesses[1].TryGetKeyText() is { } propertyName)
                {
                    // underline only the key of the object property access
                    return GetModuleSymbolLocation(
                        propertyAccesses.Last().Key,
                        context,
                        moduleDeclarationSyntax,
                        propertyType,
                        propertyName);
                }
            }

            return new();
        }

        private LocationOrLocationLinks HandlePropertyLocation(DefinitionParams request, SymbolResolutionResult result, CompilationContext context)
        {
            var semanticModel = context.Compilation.GetEntrypointSemanticModel();

            // Find the underlying VariableSyntax being accessed
            var syntax = result.Origin;
            var propertyAccesses = new List<IdentifierSyntax>();
            while (true)
            {
                if (syntax is PropertyAccessSyntax propertyAccessSyntax)
                {
                    // since we are traversing bottom up, add this access to the beginning of the list
                    propertyAccesses.Insert(0, propertyAccessSyntax.PropertyName);
                    syntax = propertyAccessSyntax.BaseExpression;

                    continue;
                }

                if (syntax is TypePropertyAccessSyntax typePropertyAccessSyntax)
                {
                    // since we are traversing bottom up, add this access to the beginning of the list
                    propertyAccesses.Insert(0, typePropertyAccessSyntax.PropertyName);
                    syntax = typePropertyAccessSyntax.BaseExpression;

                    continue;
                }

                if (syntax is ParenthesizedExpressionSyntax parenthesized)
                {
                    syntax = parenthesized.Expression;

                    continue;
                }

                break;
            }

            if (syntax is VariableAccessSyntax or TypeVariableAccessSyntax
                && semanticModel.GetSymbolInfo(syntax) is DeclaredSymbol ancestorSymbol)
            {
                // If the symbol is a module, we need to redirect the user to the module file
                // note: module.name doesn't follow this: it should refer to the declaration of the module in the current file, like regular variable and resource property accesses
                if (propertyAccesses.Count == 2
                && ancestorSymbol.DeclaringSyntax is ModuleDeclarationSyntax moduleDeclarationSyntax)
                {
                    // underline only the last property access
                    return GetModuleSymbolLocation(
                        propertyAccesses.Last(),
                        context,
                        moduleDeclarationSyntax,
                        propertyAccesses[0].IdentifierName,
                        propertyAccesses[1].IdentifierName);
                }

                // The user should be redirected to the import target file if the symbol is a wildcard import
                if (propertyAccesses.Count == 1 && ancestorSymbol is WildcardImportSymbol wildcardImport)
                {
                    return HandleImportedSymbolLocation(result.Origin.ToRange(context.LineStarts), context, wildcardImport.SourceModel, propertyAccesses.Single().IdentifierName, wildcardImport.EnclosingDeclaration);
                }

                // Otherwise, we redirect user to the specified module, variable, or resource declaration
                if (GetObjectSyntaxFromDeclaration(ancestorSymbol.DeclaringSyntax) is ObjectSyntax objectSyntax
                    && ObjectSyntaxExtensions.TryGetPropertyByNameRecursive(objectSyntax, propertyAccesses) is ObjectPropertySyntax resultingSyntax)
                {
                    // underline only the last property access
                    return new(new LocationOrLocationLink(new LocationLink
                    {
                        OriginSelectionRange = propertyAccesses.Last().ToRange(result.Context.LineStarts),
                        TargetUri = request.TextDocument.Uri,
                        TargetRange = resultingSyntax.ToRange(result.Context.LineStarts),
                        TargetSelectionRange = resultingSyntax.ToRange(result.Context.LineStarts)
                    }));
                }
            }

            return new();
        }

        private LocationOrLocationLinks HandleParameterAssignment(DefinitionParams request, SymbolResolutionResult result, CompilationContext context, ParameterAssignmentSymbol param)
        {
            if (param.NameSource is not { } nameSyntax)
            {
                return new();
            }

            var paramsModel = context.Compilation.GetEntrypointSemanticModel();
            if (!paramsModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel) ||
                usingModel is not SemanticModel bicepModel)
            {
                return new();
            }

            if (bicepModel.Root.ParameterDeclarations
                .FirstOrDefault(x => x.DeclaringParameter.Name.NameEquals(param.Name)) is not ParameterSymbol parameterSymbol)
            {
                return new();
            }

            var range = PositionHelper.GetNameRange(bicepModel.SourceFile.LineStarts, parameterSymbol.DeclaringSyntax);
            var documentUri = bicepModel.SourceFile.FileHandle.Uri.ToDocumentUri();

            return new(new LocationOrLocationLink(new LocationLink
            {
                // source of the link. Underline only the symbolic name
                OriginSelectionRange = nameSyntax.ToRange(context.LineStarts),
                TargetUri = documentUri,

                // entire span of the declaredSymbol
                TargetRange = range,
                TargetSelectionRange = range
            }));
        }

        private LocationOrLocationLinks HandleImportedSymbolLocation(SymbolResolutionResult result, CompilationContext context, ImportedSymbol imported)
            => HandleImportedSymbolLocation(result.Origin.ToRange(context.LineStarts), context, imported.SourceModel, imported.OriginalSymbolName, imported.EnclosingDeclaration);

        private LocationOrLocationLinks HandleWildcardImportInstanceFunctionLocation(SymbolResolutionResult result, CompilationContext context, WildcardImportInstanceFunctionSymbol symbol)
            => HandleImportedSymbolLocation(result.Origin.ToRange(context.LineStarts), context, symbol.BaseSymbol.SourceModel, symbol.Name, symbol.BaseSymbol.EnclosingDeclaration);

        private LocationOrLocationLinks HandleImportedSymbolLocation(Range originSelectionRange, CompilationContext context, ISemanticModel sourceModel, string? originalSymbolName, IArtifactReferenceSyntax enclosingDeclaration)
        {
            if (TryHandleImportedSymbolLocationInBicepSource(originSelectionRange, context, sourceModel, originalSymbolName, enclosingDeclaration) is { } bicepSourceLink)
            {
                return bicepSourceLink;
            }

            var (armTemplate, armTemplateUri) = GetArmSourceTemplateInfo(context, enclosingDeclaration);

            if (armTemplateUri is not null && originalSymbolName is string nonNullName && sourceModel.Exports.TryGetValue(nonNullName, out var exportMetadata))
            {
                if (exportMetadata.Kind == ExportMetadataKind.Type &&
                    armTemplate?.Definitions?.TryGetValue(nonNullName, out var originalTypeDefinition) is true &&
                    ToRange(originalTypeDefinition) is Range typeDefinitionRange)
                {
                    return new(new LocationOrLocationLink(new LocationLink
                    {
                        OriginSelectionRange = originSelectionRange,
                        TargetUri = armTemplateUri,
                        TargetRange = typeDefinitionRange,
                        TargetSelectionRange = typeDefinitionRange,
                    }));
                }

                if (exportMetadata.Kind == ExportMetadataKind.Variable)
                {
                    if (armTemplate?.Variables?.TryGetValue(nonNullName, out var variableDeclaration) is true && ToRange(variableDeclaration) is Range variableDefinitionRange)
                    {
                        return new(new LocationOrLocationLink(new LocationLink
                        {
                            OriginSelectionRange = originSelectionRange,
                            TargetUri = armTemplateUri,
                            TargetRange = variableDefinitionRange,
                            TargetSelectionRange = variableDefinitionRange,
                        }));
                    }

                    if (armTemplate?.Variables?.TryGetValue("copy", out var copyVariablesDeclaration) is true &&
                        copyVariablesDeclaration.Value is JArray copyVariablesArray &&
                        copyVariablesArray.Where(e => e is JObject objectElement &&
                            objectElement.TryGetValue("name", StringComparison.OrdinalIgnoreCase, out var nameToken) &&
                            nameToken is JValue { Value: string nameString } &&
                            StringComparer.OrdinalIgnoreCase.Equals(nameString, nonNullName))
                            .FirstOrDefault() is JToken copyVariableToken &&
                        ToRange(copyVariableToken) is Range copyVariableDefinitionRange)
                    {
                        return new(new LocationOrLocationLink(new LocationLink
                        {
                            OriginSelectionRange = originSelectionRange,
                            TargetUri = armTemplateUri,
                            TargetRange = copyVariableDefinitionRange,
                            TargetSelectionRange = copyVariableDefinitionRange,
                        }));
                    }
                }

                if (exportMetadata.Kind == ExportMetadataKind.Function)
                {
                    var fullyQualifiedFunctionName = nonNullName.Contains('.')
                        ? nonNullName
                        : $"{EmitConstants.UserDefinedFunctionsNamespace}.{nonNullName}";

                    if (armTemplate?.GetFunctionDefinitions()
                        .Where(fd => StringComparer.OrdinalIgnoreCase.Equals(fd.Key, fullyQualifiedFunctionName))
                        .FirstOrDefault() is FunctionDefinition functionDefinition &&
                        ToRange(functionDefinition.Function) is Range functionDefinitionRange)
                    {
                        return new(new LocationOrLocationLink(new LocationLink
                        {
                            OriginSelectionRange = originSelectionRange,
                            TargetUri = armTemplateUri,
                            TargetRange = functionDefinitionRange,
                            TargetSelectionRange = functionDefinitionRange,
                        }));
                    }
                }
            }

            return new();
        }

        private LocationOrLocationLinks? TryHandleImportedSymbolLocationInBicepSource(Range originSelectionRange, CompilationContext context, ISemanticModel sourceModel, string? originalSymbolName, IArtifactReferenceSyntax enclosingDeclaration)
        {
            Uri? externalSourceUri = null;
            SemanticModel? bicepModel = sourceModel as SemanticModel; // if the source model is a local Bicep file

            // CONSIDER using only the syntax tree, not the semantic model
            if (sourceModel is ArmTemplateSemanticModel &&
                moduleDispatcher.TryGetArtifactReference(context.Compilation.SourceFileGrouping.EntryPoint, enclosingDeclaration).IsSuccess(out var artifactReference)
                && artifactReference is OciArtifactReference ociArtifactReference
                && ociArtifactReference.TryLoadSourceArchive().TryUnwrap() is SourceArchive sourceArchive)
            {
                // An imported remote artifact was published with source, so we want to take the user to the definition in the source archive.
                // Long term, it would be best to package up symbol information with the source that we could query here to find the location of the definition in the source file.
                // But for this purpose, doing a compilation of the source entrypoint file should be enough to find the definition for most scenarios, even if the source file
                // does not compile error-free (which is likely). We will compile the entrypoint file only, not any of its dependencies or referenced modules.
                var importedSourceBicep = sourceArchive.FindSourceFile(sourceArchive.EntrypointRelativePath).Contents;
                var bicepFile = sourceFileFactory.CreateBicepFile(DummyFileHandle.Default, importedSourceBicep);

                var workspace = new ActiveSourceFileSet();
                workspace.UpsertSourceFile(bicepFile);
                var compilation = bicepCompiler.CreateCompilationWithoutRestore(bicepFile.FileHandle.Uri, workspace);

                bicepModel = compilation.GetEntrypointSemanticModel();
                externalSourceUri = BicepExternalSourceRequestHandler.GetRegistryModuleSourceLinkUri(ociArtifactReference, sourceArchive);
            }

            if (bicepModel?.Root.Declarations.Where(type => LanguageConstants.IdentifierComparer.Equals(type.Name, originalSymbolName)).FirstOrDefault() is { } originalDeclaration)
            {
                // entire span of the declaredSymbol
                var targetRange = PositionHelper.GetNameRange(bicepModel.SourceFile.LineStarts, originalDeclaration.DeclaringSyntax);

                return new(new LocationOrLocationLink(new LocationLink
                {
                    OriginSelectionRange = originSelectionRange,
                    TargetUri = externalSourceUri ?? bicepModel.SourceFile.FileHandle.Uri.ToDocumentUri(),
                    TargetRange = targetRange,
                    TargetSelectionRange = targetRange,
                }));
            }

            return null;
        }

        private static (Template?, DocumentUri?) GetArmSourceTemplateInfo(CompilationContext context, IArtifactReferenceSyntax foreignTemplateReference)
            => context.Compilation.SourceFileGrouping.TryGetSourceFile(foreignTemplateReference).TryUnwrap() switch
            {
                TemplateSpecFile templateSpecFile => (templateSpecFile.MainTemplateFile.Template, templateSpecFile.FileHandle.Uri.ToDocumentUri()),
                ArmTemplateFile armTemplateFile => (armTemplateFile.Template, armTemplateFile.FileHandle.Uri.ToDocumentUri()),
                _ => (null, null),
            };

        private static Range? ToRange(JTokenMetadata jToken)
            => jToken.LineNumber.HasValue && jToken.LinePosition.HasValue
                ? new(jToken.LineNumber.Value - 1, jToken.LinePosition.Value, jToken.LineNumber.Value - 1, jToken.LinePosition.Value)
                : null;

        private static Range? ToRange(IJsonLineInfo jsonLineInfo)
            => jsonLineInfo.LineNumber > 0
                ? new(jsonLineInfo.LineNumber - 1, jsonLineInfo.LinePosition, jsonLineInfo.LineNumber - 1, jsonLineInfo.LinePosition)
                : null;

        private LocationOrLocationLinks GetModuleSymbolLocation(
            SyntaxBase underlinedSyntax,
            CompilationContext context,
            ModuleDeclarationSyntax moduleDeclarationSyntax,
            string propertyType,
            string propertyName)
        {
            if (context.Compilation.SourceFileGrouping.TryGetSourceFile(moduleDeclarationSyntax).IsSuccess(out var sourceFile) && sourceFile is BicepFile bicepFile
            && context.Compilation.GetSemanticModel(bicepFile) is SemanticModel moduleModel)
            {
                switch (propertyType)
                {
                    case LanguageConstants.ModuleOutputsPropertyName:
                        if (moduleModel.Root.OutputDeclarations
                            .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is OutputSymbol outputSymbol)
                        {
                            return GetFileDefinitionLocation(
                                bicepFile.FileHandle.Uri.ToDocumentUri(),
                                underlinedSyntax,
                                context,
                                outputSymbol.DeclaringOutput.Name.ToRange(bicepFile.LineStarts));
                        }
                        break;
                    case LanguageConstants.ModuleParamsPropertyName:
                        if (moduleModel.Root.ParameterDeclarations
                            .FirstOrDefault(d => string.Equals(d.Name, propertyName)) is ParameterSymbol parameterSymbol)
                        {
                            return GetFileDefinitionLocation(
                                bicepFile.FileHandle.Uri.ToDocumentUri(),
                                underlinedSyntax,
                                context,
                                parameterSymbol.DeclaringParameter.Name.ToRange(bicepFile.LineStarts));
                        }
                        break;
                }

            }

            return new();
        }

        private static LocationOrLocationLinks GetFileDefinitionLocation(
            DocumentUri fileUri,
            SyntaxBase originalSelectionSyntax,
            CompilationContext context,
            Range targetRange)
        {
            return new LocationOrLocationLinks(new LocationOrLocationLink(new LocationLink
            {
                OriginSelectionRange = originalSelectionSyntax.ToRange(context.LineStarts),
                TargetUri = fileUri,
                TargetRange = targetRange,
                TargetSelectionRange = targetRange
            }));
        }

        private static ObjectSyntax? GetObjectSyntaxFromDeclaration(SyntaxBase syntax) => syntax switch
        {
            ResourceDeclarationSyntax resourceDeclarationSyntax when resourceDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            ModuleDeclarationSyntax moduleDeclarationSyntax when moduleDeclarationSyntax.TryGetBody() is ObjectSyntax objectSyntax => objectSyntax,
            VariableDeclarationSyntax variableDeclarationSyntax when variableDeclarationSyntax.Value is ObjectSyntax objectSyntax => objectSyntax,
            _ => null,
        };

        // True if the client knows how (like our vscode extension) to handle the "bicep-extsrc:" schema
        private bool CanClientAcceptRegistryContent()
        {
            if (languageServer.ClientSettings.InitializationOptions is not JObject obj ||
                obj.Property("enableRegistryContent") is not { } property ||
                property.Value.Type != JTokenType.Boolean)
            {
                return false;
            }

            return property.Value.Value<bool>();
        }
    }
}
