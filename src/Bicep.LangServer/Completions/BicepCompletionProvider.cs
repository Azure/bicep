// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Azure.Deployments.Core.Comparers;
using Bicep.Core;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.Workspaces;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Snippets;
using Bicep.LanguageServer.Telemetry;
using Bicep.LanguageServer.Utils;
using Newtonsoft.Json.Linq;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        private const string MarkdownNewLine = "  \n";

        private static readonly Container<string> ResourceSymbolCommitChars = new(":");

        private static readonly Container<string> PropertyAccessCommitChars = new(".");

        private readonly IFileResolver FileResolver;
        private readonly ISnippetsProvider SnippetsProvider;
        private readonly ITelemetryProvider TelemetryProvider;
        private readonly INamespaceProvider namespaceProvider;

        public BicepCompletionProvider(IFileResolver fileResolver, ISnippetsProvider snippetsProvider, ITelemetryProvider telemetryProvider, INamespaceProvider namespaceProvider)
        {
            this.FileResolver = fileResolver;
            this.SnippetsProvider = snippetsProvider;
            this.TelemetryProvider = telemetryProvider;
            this.namespaceProvider = namespaceProvider;
        }

        public IEnumerable<CompletionItem> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context)
        {
            var model = compilation.GetEntrypointSemanticModel();

            return GetDeclarationCompletions(model, context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(model, context))
                .Concat(GetObjectPropertyNameCompletions(model, context))
                .Concat(GetMemberAccessCompletions(compilation, context))
                .Concat(GetResourceAccessCompletions(compilation, context))
                .Concat(GetArrayIndexCompletions(compilation, context))
                .Concat(GetPropertyValueCompletions(model, context))
                .Concat(GetArrayItemCompletions(model, context))
                .Concat(GetResourceTypeCompletions(model, context))
                .Concat(GetResourceTypeFollowerCompletions(context))
                .Concat(GetModulePathCompletions(model, context))
                .Concat(GetModuleBodyCompletions(model, context))
                .Concat(GetResourceBodyCompletions(model, context))
                .Concat(GetParameterDefaultValueCompletions(model, context))
                .Concat(GetVariableValueCompletions(context))
                .Concat(GetOutputValueCompletions(model, context))
                .Concat(GetTargetScopeCompletions(model, context))
                .Concat(GetImportCompletions(model, context))
                .Concat(GetFunctionParamCompletions(model, context))
                .Concat(GetExpressionCompletions(model, context))
                .Concat(GetDisableNextLineDiagnosticsDirectiveCompletion(context))
                .Concat(GetDisableNextLineDiagnosticsDirectiveCodesCompletion(model, context));
        }


        public IEnumerable<CompletionItem> GetFilteredParamsCompletions(ParamsSemanticModel paramsSemanticModel, ParamsCompletionContext paramsCompletionContext)
        {
            return GetParamIdentifierCompletions(paramsSemanticModel, paramsCompletionContext)
                  .Concat(GetParamAllowedValueCompletions(paramsSemanticModel, paramsCompletionContext))
                  .Concat(GetUsingDeclarationPathCompletions(paramsSemanticModel, paramsCompletionContext));
        }

        private IEnumerable<CompletionItem> GetParamIdentifierCompletions(ParamsSemanticModel paramsSemanticModel, ParamsCompletionContext paramsCompletionContext)
        {
            if(paramsCompletionContext.Kind.HasFlag(ParamsCompletionContextKind.ParamIdentifier) && paramsSemanticModel.BicepCompilation is {} bicepCompilation)
            {
                var bicepSemanticModel = bicepCompilation.GetEntrypointSemanticModel();
                var bicepFileParamDeclarations = bicepSemanticModel.Root.ParameterDeclarations;

                foreach(var declaration in bicepFileParamDeclarations)
                {
                    if(!IsParamAssigned(declaration))
                    {
                        yield return CreateSymbolCompletion(declaration, paramsCompletionContext.ReplacementRange);
                    }
                }
            }

            bool IsParamAssigned(ParameterSymbol declaration) => paramsSemanticModel.ParamBinder.ParamFileSymbol.ParameterAssignmentSymbols.Any(paramDeclaration => paramDeclaration.Name == declaration.Name);
        }

        private IEnumerable<CompletionItem> GetParamAllowedValueCompletions(ParamsSemanticModel paramsSemanticModel, ParamsCompletionContext paramsCompletionContext)
        {

            if(paramsCompletionContext.Kind.HasFlag(ParamsCompletionContextKind.ParamValue) && paramsSemanticModel.BicepCompilation is {} bicepCompilation)
            {
                var bicepSemanticModel = bicepCompilation.GetEntrypointSemanticModel();
                var bicepFileParamDeclaration = bicepSemanticModel.Parameters.Where(parameterMetaData => parameterMetaData.Name == paramsCompletionContext.ParamAssignment).FirstOrDefault();
                if(bicepFileParamDeclaration?.TypeReference.Type is UnionType union)
                {
                    foreach(var typeRef in union.Members)
                    {
                        if(typeRef.Type is StringLiteralType stringLiteral)
                        {
                            var returnVal = CompletionItemBuilder.Create(CompletionItemKind.EnumMember, stringLiteral.Name)
                                        .WithPlainTextEdit(paramsCompletionContext.ReplacementRange, stringLiteral.Name)
                                        .WithDetail(stringLiteral.Name)
                                        .Preselect()
                                        .WithSortText(GetSortText(stringLiteral.Name, CompletionPriority.Medium))
                                        .Build();
                            yield return returnVal;
                        }
                    }
                }
            }
        }

        private IEnumerable<CompletionItem> GetUsingDeclarationPathCompletions(ParamsSemanticModel paramsSemanticModel, ParamsCompletionContext paramsCompletionContext)
        {
            if (!paramsCompletionContext.Kind.HasFlag(ParamsCompletionContextKind.UsingFilePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if(paramsCompletionContext.UsingDeclaration is not UsingDeclarationSyntax usingDeclarationSyntax ||
               usingDeclarationSyntax.Path is not StringSyntax stringSyntax ||
               stringSyntax.TryGetLiteralValue() is not string entered)
            {
                entered = "";
            }


             // These should only fail if we're not able to resolve cwd path or the entered string
            if (TryGetFilesForPathCompletions(paramsSemanticModel.BicepParamFile.FileUri, entered) is not {} fileCompletionInfo)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // Prioritize .bicep files higher than other files.
            var bicepFileItems = CreateFileCompletionItems(paramsSemanticModel.BicepParamFile.FileUri, paramsCompletionContext.ReplacementRange, fileCompletionInfo, IsBicepFile, CompletionPriority.High);
            var dirItems = CreateDirectoryCompletionItems(paramsCompletionContext.ReplacementRange, fileCompletionInfo);

            return bicepFileItems.Concat(dirItems);

            bool IsBicepFile(Uri fileUri) => PathHelper.HasBicepExtension(fileUri);
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.TopLevelDeclarationStart))
            {
                yield return CreateKeywordCompletion(LanguageConstants.MetadataKeyword, "Metadata keyword", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter keyword", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.VariableKeyword, "Variable keyword", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange, priority: CompletionPriority.High);

                yield return CreateKeywordCompletion(LanguageConstants.OutputKeyword, "Output keyword", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.ModuleKeyword, "Module keyword", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.TargetScopeKeyword, "Target Scope keyword", context.ReplacementRange);

                if (model.Features.ImportsEnabled)
                {
                    yield return CreateKeywordCompletion(LanguageConstants.ImportKeyword, "Import keyword", context.ReplacementRange);
                }

                foreach (Snippet resourceSnippet in SnippetsProvider.GetTopLevelNamedDeclarationSnippets())
                {
                    string prefix = resourceSnippet.Prefix;
                    BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateTopLevelDeclarationSnippetInsertion(prefix);
                    var command = TelemetryHelper.CreateCommand
                    (
                        title: "top level snippet completion",
                        name: TelemetryConstants.CommandName,
                        args: JArray.FromObject(new List<object> { telemetryEvent })
                    );

                    yield return CreateContextualSnippetCompletion(prefix,
                                                                   resourceSnippet.Detail,
                                                                   resourceSnippet.Text,
                                                                   context.ReplacementRange,
                                                                   command,
                                                                   resourceSnippet.CompletionPriority);
                }
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.NestedResourceDeclarationStart) && context.EnclosingDeclaration is ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange);

                if (model.GetSymbolInfo(resourceDeclarationSyntax) is ResourceSymbol parentSymbol &&
                    parentSymbol.TryGetResourceTypeReference() is ResourceTypeReference parentTypeReference)
                {
                    foreach (Snippet snippet in SnippetsProvider.GetNestedResourceDeclarationSnippets(parentTypeReference))
                    {
                        string prefix = snippet.Prefix;
                        BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateNestedResourceDeclarationSnippetInsertion(prefix);
                        var command = TelemetryHelper.CreateCommand
                        (
                            title: "nested resource declaration completion snippet",
                            name: TelemetryConstants.CommandName,
                            args: JArray.FromObject(new List<object> { telemetryEvent })
                        );
                        yield return CreateContextualSnippetCompletion(prefix,
                            snippet.Detail,
                            snippet.Text,
                            context.ReplacementRange,
                            command,
                            snippet.CompletionPriority,
                            preselect: true);
                    }
                }
            }
        }

        private IEnumerable<CompletionItem> GetTargetScopeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            return context.Kind.HasFlag(BicepCompletionContextKind.TargetScope) && context.TargetScope is { } targetScope
                ? GetValueCompletionsForType(model, context, model.GetDeclaredType(targetScope), loopsAllowed: false)
                : Enumerable.Empty<CompletionItem>();
        }

        private IEnumerable<CompletionItem> GetSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.Expression) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName | BicepCompletionContextKind.MemberAccess))
            {
                // This is already handled by GetMemberAccessCompletions.
                return Enumerable.Empty<CompletionItem>();
            }

            if (context.Property != null && model.GetDeclaredTypeAssignment(context.Property)?.Flags == DeclaredTypeFlags.Constant)
            {
                // the enclosing property's declared type is supposed to be a constant value
                // the constant flag comes from TypeProperty constant flag, so nothing else can really alter it except for another property
                // (in other words constant flag inherits down into the expression tree of the property value)
                return Enumerable.Empty<CompletionItem>();
            }

            // when we're inside an expression that is inside a property that expects a compile-time constant value,
            // we should not be emitting accessible symbol completions
            return GetAccessibleSymbolCompletions(model, context);
        }

        private IEnumerable<CompletionItem> GetDeclarationTypeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            // local function
            IEnumerable<CompletionItem> GetPrimitiveTypeCompletions() =>
                LanguageConstants.DeclarationTypes.Values.Select(type => CreateTypeCompletion(type, context.ReplacementRange));

            if (context.Kind.HasFlag(BicepCompletionContextKind.ParameterType))
            {
                // Only show the resource type as a completion if the resource-typed parameter feature is enabled.
                var completions = GetPrimitiveTypeCompletions().Concat(GetParameterTypeSnippets(model.Compilation, context));
                if (model.Features.ResourceTypedParamsAndOutputsEnabled)
                {
                    completions = completions.Concat(CreateResourceTypeKeywordCompletion(context.ReplacementRange));
                }

                return completions;
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.OutputType))
            {
                // Only show the resource type as a completion if the resource-typed parameter feature is enabled.
                var completions = GetPrimitiveTypeCompletions();
                if (model.Features.ResourceTypedParamsAndOutputsEnabled)
                {
                    completions = completions.Concat(CreateResourceTypeKeywordCompletion(context.ReplacementRange));
                }

                return completions;
            }

            return Enumerable.Empty<CompletionItem>();
        }

        private IEnumerable<CompletionItem> GetResourceTypeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ResourceType))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // For a nested resource, we want to filter the set of types.
            //
            // The strategy when *can't* filter - due to errors - to fallback to the main path and offer full completions
            // then once the user corrects whatever's cause the error, they will be told to simplify the type.
            if (context.EnclosingDeclaration is SyntaxBase &&
                model.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(context.EnclosingDeclaration) is ResourceDeclarationSyntax parentSyntax &&
                model.GetSymbolInfo(parentSyntax) is ResourceSymbol parentSymbol &&
                parentSymbol.TryGetResourceType() is { } parentResourceType)
            {
                // This is more complex because we allow the API version to be omitted, so we want to make a list of unique values
                // for the FQT, and then create a "no version" completion + a completion for each version.
                var filtered = parentResourceType.DeclaringNamespace.ResourceTypeProvider.GetAvailableTypes()
                    .Where(rt => parentResourceType.TypeReference.IsParentOf(rt))
                    .ToLookup(rt => rt.FormatType());

                var index = 0;
                var items = new List<CompletionItem>();
                foreach (var group in filtered.OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
                {
                    // Doesn't matter which one of the group we take, we're leaving out the version.
                    items.Add(CreateResourceTypeSegmentCompletion(group.First(), index++, context.ReplacementRange, includeApiVersion: false, displayApiVersion: parentResourceType.TypeReference.ApiVersion));

                    foreach (var resourceType in group.Where(rt => rt.ApiVersion is not null).OrderByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance))
                    {
                        items.Add(CreateResourceTypeSegmentCompletion(resourceType, index++, context.ReplacementRange, includeApiVersion: true, displayApiVersion: resourceType.ApiVersion));
                    }
                }

                return items;
            }

            static string? TryGetFullyQualifiedType(StringSyntax? stringSyntax)
            {
                if (stringSyntax?.TryGetLiteralValue() is string entered && ResourceTypeReference.HasResourceTypePrefix(entered))
                {
                    return entered;
                }

                return null;
            }

            static string? TryGetFullyQualfiedResourceType(SyntaxBase? enclosingDeclaration)
            {
                return enclosingDeclaration switch
                {
                    ResourceDeclarationSyntax resourceSyntax => TryGetFullyQualifiedType(resourceSyntax.TypeString),
                    ParameterDeclarationSyntax parameterSyntax when parameterSyntax.Type is ResourceTypeSyntax resourceType => TryGetFullyQualifiedType(resourceType.TypeString),
                    OutputDeclarationSyntax outputSyntax when outputSyntax.Type is ResourceTypeSyntax resourceType => TryGetFullyQualifiedType(resourceType.TypeString),
                    _ => null,
                };
            }

            // ResourceType completions are divided into 2 parts.
            // If the current value passes the namespace and type notation ("<Namespace>/<type>") format, we return the fully qualified resource types
            if (TryGetFullyQualfiedResourceType(context.EnclosingDeclaration) is string qualified)
            {
                // newest api versions should be shown first
                // strict filtering on type so that we show api versions for only the selected type
                return model.Binder.NamespaceResolver.GetAvailableResourceTypes()
                    .Where(rt => StringComparer.OrdinalIgnoreCase.Equals(qualified.Split('@')[0], rt.FormatType()))
                    .OrderBy(rt => rt.FormatType(), StringComparer.OrdinalIgnoreCase)
                    .ThenByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance)
                    .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange, showApiVersion: true))
                    .ToList();
            }

            // if we do not have the namespace and type notation, we only return unique resource types without their api-versions
            // we need to ensure that Microsoft.Compute/virtualMachines comes before Microsoft.Compute/virtualMachines/extensions
            // we still order by apiVersion first to have consistent indexes
            return model.Binder.NamespaceResolver.GetAvailableResourceTypes()
                .OrderByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance)
                .GroupBy(rt => rt.FormatType())
                .Select(rt => rt.First())
                .OrderBy(rt => rt.FormatType(), StringComparer.OrdinalIgnoreCase)
                .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange, showApiVersion: false))
                .ToList();
        }

        private IEnumerable<CompletionItem> GetResourceTypeFollowerCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ResourceTypeFollower))
            {
                // Only when there is no existing assignment sign
                if (context.EnclosingDeclaration is ResourceDeclarationSyntax { Assignment: SkippedTriviaSyntax { Elements: { IsDefaultOrEmpty: true } } })
                {
                    const string equals = "=";
                    yield return CreateOperatorCompletion(equals, context.ReplacementRange, preselect: true);
                }

                if (context.EnclosingDeclaration is ResourceDeclarationSyntax { ExistingKeyword: null })
                {
                    const string existing = "existing";
                    yield return CreateKeywordCompletion(existing, existing, context.ReplacementRange);
                }
            }
        }

        private record FileCompletionInfo(
            Uri BicepFileParentUri,
            Uri EnteredParentUri,
            bool ShowCwdPrefix,
            ImmutableArray<Uri> Files,
            ImmutableArray<Uri> Directories);

        private FileCompletionInfo? TryGetFilesForPathCompletions(Uri baseUri, string entered)
        {
            var files = new List<Uri>();
            var dirs = new List<Uri>();

            if (FileResolver.TryResolveFilePath(baseUri, ".") is not { } cwdUri
                || FileResolver.TryResolveFilePath(cwdUri, entered) is not { } query)
            {
                return null;
            }

            // technically bicep files do not have to follow the bicep extension, so
            // we are not enforcing *.bicep get files command
            var queryParent = (FileResolver.DirExists(query) ? query : FileResolver.TryResolveFilePath(query, "."));

            if (queryParent is not null)
            {
                files = FileResolver.GetFiles(queryParent, string.Empty).ToList();
                dirs = FileResolver.GetDirectories(queryParent, string.Empty).ToList();

                // include the parent folder as a completion if we're not at the file system root
                if (FileResolver.TryResolveFilePath(queryParent, "..") is {} parentDir &&
                    parentDir != queryParent)
                {
                    dirs.Add(parentDir);
                }
            }

            return new(
                BicepFileParentUri: cwdUri,
                EnteredParentUri: query,
                ShowCwdPrefix: entered.StartsWith("./"),
                Files: files.ToImmutableArray(),
                Directories: dirs.ToImmutableArray());
        }

        private IEnumerable<CompletionItem> CreateFileCompletionItems(Uri mainFileUri, Range replacementRange, FileCompletionInfo info, Predicate<Uri> predicate, CompletionPriority priority)
        {
            foreach (var fileUri in info.Files)
            {
                if (fileUri == mainFileUri || !predicate(fileUri))
                {
                    continue;
                }

                var completionName = info.EnteredParentUri.MakeRelativeUri(fileUri).ToString();
                var completionValue = info.BicepFileParentUri.MakeRelativeUri(fileUri).ToString();
                if (info.ShowCwdPrefix && !completionValue.StartsWith("../", StringComparison.Ordinal))
                {
                    // "./" will not be preserved when making relative Uris. We have to go and manually add it.
                    completionValue = "./" + completionValue;
                }

                yield return CreateFilePathCompletionBuilder(
                    completionName,
                    completionValue,
                    replacementRange,
                    CompletionItemKind.File,
                    priority)
                .Build();
            }
        }

        private IEnumerable<CompletionItem> CreateDirectoryCompletionItems(Range replacementRange, FileCompletionInfo info, CompletionPriority priority = CompletionPriority.Low)
        {
            foreach (var dirUri in info.Directories)
            {
                var completionName = info.EnteredParentUri.MakeRelativeUri(dirUri).ToString();
                var completionValue = info.BicepFileParentUri.MakeRelativeUri(dirUri).ToString();
                if (info.ShowCwdPrefix && !completionValue.StartsWith("../", StringComparison.Ordinal))
                {
                    // "./" will not be preserved when making relative Uris. We have to go and manually add it.
                    completionValue = "./" + completionValue;
                }

                yield return CreateFilePathCompletionBuilder(
                    completionName,
                    completionValue,
                    replacementRange,
                    CompletionItemKind.Folder,
                    priority)
                .WithCommand(new Command { Name = EditorCommands.RequestCompletions, Title = "file path completion" })
                .Build();
            }
        }

        private IEnumerable<CompletionItem> GetModulePathCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // To provide intellisense before the quotes are typed
            if (context.EnclosingDeclaration is not ModuleDeclarationSyntax declarationSyntax
                || declarationSyntax.Path is not StringSyntax stringSyntax
                || stringSyntax.TryGetLiteralValue() is not string entered)
            {
                entered = "";
            }

            // These should only fail if we're not able to resolve cwd path or the entered string
            if (TryGetFilesForPathCompletions(model.SourceFile.FileUri, entered) is not {} fileCompletionInfo)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var replacementRange = context.EnclosingDeclaration is ModuleDeclarationSyntax module ? module.Path.ToRange(model.SourceFile.LineStarts) : context.ReplacementRange;

            // Prioritize .bicep files higher than other files.
            var bicepFileItems = CreateFileCompletionItems(model.SourceFile.FileUri, replacementRange, fileCompletionInfo, IsBicepFile, CompletionPriority.High);
            var armTemplateFileItems = CreateFileCompletionItems(model.SourceFile.FileUri, replacementRange, fileCompletionInfo, IsArmTemplateFileLike, CompletionPriority.Medium);
            var dirItems = CreateDirectoryCompletionItems(replacementRange, fileCompletionInfo);

            return bicepFileItems.Concat(armTemplateFileItems).Concat(dirItems);

            // Local functions.


            bool IsBicepFile(Uri fileUri) => PathHelper.HasBicepExtension(fileUri);

            bool IsArmTemplateFileLike(Uri fileUri)
            {
                if (PathHelper.HasExtension(fileUri, LanguageConstants.ArmTemplateFileExtension))
                {
                    return true;
                }

                if (model.Compilation.SourceFileGrouping.SourceFiles.Any(sourceFile =>
                        sourceFile is ArmTemplateFile &&
                        sourceFile.FileUri.LocalPath.Equals(fileUri.LocalPath, PathHelper.PathComparison)))
                {
                    return true;
                }

                if (!PathHelper.HasExtension(fileUri, LanguageConstants.JsonFileExtension) &&
                    !PathHelper.HasExtension(fileUri, LanguageConstants.JsoncFileExtension))
                {
                    return false;
                }

                if (FileResolver.TryReadAtMostNCharaters(fileUri, Encoding.UTF8, 2000, out var fileContents) &&
                    LanguageConstants.ArmTemplateSchemaRegex.IsMatch(fileContents))
                {
                    return true;
                }

                return false;
            }
        }

        private static IEnumerable<CompletionItem> GetParameterTypeSnippets(Compilation compitation, BicepCompletionContext context)
        {
            if (context.EnclosingDeclaration is ParameterDeclarationSyntax parameterDeclarationSyntax)
            {
                BicepFile bicepFile = compitation.SourceFileGrouping.EntryPoint;
                Range enclosingDeclarationRange = parameterDeclarationSyntax.Keyword.ToRange(bicepFile.LineStarts);
                TextEdit textEdit = new TextEdit()
                {
                    Range = new Range()
                    {
                        Start = enclosingDeclarationRange.Start,
                        End = enclosingDeclarationRange.Start
                    },
                    NewText = "@secure()\n"
                };

                yield return CreateContextualSnippetCompletion("secureObject",
                                                               "Secure object",
                                                               "object",
                                                               context.ReplacementRange,
                                                               new TextEdit[] { textEdit });

                yield return CreateContextualSnippetCompletion("secureString",
                                                               "Secure string",
                                                               "string",
                                                               context.ReplacementRange,
                                                               new TextEdit[] { textEdit });
            }
        }

        private IEnumerable<CompletionItem> GetParameterDefaultValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ParameterDefaultValue) || context.EnclosingDeclaration is not ParameterDeclarationSyntax parameter)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredType = model.GetDeclaredType(parameter);

            return GetValueCompletionsForType(model, context, declaredType, loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetVariableValueCompletions(BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.VariableValue))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // we don't know what the variable type is, so assume "any"
            return CreateLoopCompletions(context.ReplacementRange, LanguageConstants.Any, filtersAllowed: false);
        }

        private IEnumerable<CompletionItem> GetOutputValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OutputValue) || context.EnclosingDeclaration is not OutputDeclarationSyntax output)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredType = model.GetDeclaredType(output);

            return GetValueCompletionsForType(model, context, declaredType, loopsAllowed: true);
        }

        private IEnumerable<CompletionItem> GetResourceBodyCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ResourceBody) && context.EnclosingDeclaration is ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                foreach (CompletionItem completionItem in CreateResourceBodyCompletions(model, context, resourceDeclarationSyntax))
                {
                    yield return completionItem;
                }

                yield return CreateResourceOrModuleConditionCompletion(context.ReplacementRange);

                // loops are always allowed as long as we're not already in a loop
                if (resourceDeclarationSyntax.Value is not ForSyntax)
                {
                    foreach (var completion in CreateLoopCompletions(context.ReplacementRange, LanguageConstants.Object, filtersAllowed: true))
                    {
                        yield return completion;
                    }
                }
            }
        }

        private IEnumerable<CompletionItem> CreateResourceBodyCompletions(SemanticModel model, BicepCompletionContext context, ResourceDeclarationSyntax resourceDeclarationSyntax)
        {
            if (model.GetDeclaredType(resourceDeclarationSyntax)?.UnwrapArrayType() is ResourceType resourceType)
            {
                var isResourceNested = model.Binder.GetNearestAncestor<ResourceDeclarationSyntax>(resourceDeclarationSyntax) is { };
                var snippets = SnippetsProvider.GetResourceBodyCompletionSnippets(resourceType, resourceDeclarationSyntax.IsExistingResource(), isResourceNested);

                foreach (Snippet snippet in snippets)
                {
                    string prefix = snippet.Prefix;
                    BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateResourceBodySnippetInsertion(prefix, resourceType.Type.Name);
                    Command command = TelemetryHelper.CreateCommand
                    (
                        title: "resource body completion snippet",
                        name: TelemetryConstants.CommandName,
                        args: JArray.FromObject(new List<object> { telemetryEvent })
                    );

                    yield return CreateContextualSnippetCompletion(prefix,
                        snippet.Detail,
                        snippet.Text,
                        context.ReplacementRange,
                        command,
                        snippet.CompletionPriority,
                        preselect: true);
                }
            }
        }

        private IEnumerable<CompletionItem> CreateModuleBodyCompletions(SemanticModel model, BicepCompletionContext context, ModuleDeclarationSyntax moduleDeclarationSyntax)
        {
            TypeSymbol typeSymbol = model.GetTypeInfo(moduleDeclarationSyntax);
            IEnumerable<Snippet> snippets = SnippetsProvider.GetModuleBodyCompletionSnippets(typeSymbol.UnwrapArrayType());

            foreach (Snippet snippet in snippets)
            {
                string prefix = snippet.Prefix;
                BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateModuleBodySnippetInsertion(prefix);
                var command = TelemetryHelper.CreateCommand
                (
                    title: "module body completion snippet",
                    name: TelemetryConstants.CommandName,
                    args: JArray.FromObject(new List<object> { telemetryEvent })
                );
                yield return CreateContextualSnippetCompletion(prefix,
                    snippet.Detail,
                    snippet.Text,
                    context.ReplacementRange,
                    command,
                    snippet.CompletionPriority,
                    preselect: true);
            }
        }

        private IEnumerable<CompletionItem> GetModuleBodyCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ModuleBody) && context.EnclosingDeclaration is ModuleDeclarationSyntax moduleDeclarationSyntax)
            {
                foreach (CompletionItem completionItem in CreateModuleBodyCompletions(model, context, moduleDeclarationSyntax))
                {
                    yield return completionItem;
                }

                yield return CreateResourceOrModuleConditionCompletion(context.ReplacementRange);

                // loops are always allowed in a resource/module if we're not inside another loop
                if (moduleDeclarationSyntax.Value is not ForSyntax)
                {
                    foreach (var completion in CreateLoopCompletions(context.ReplacementRange, LanguageConstants.Object, filtersAllowed: true))
                    {
                        yield return completion;
                    }
                }
            }
        }

        private static ImmutableDictionary<Symbol, NamespaceType> GetNamespaceTypeBySymbol(SemanticModel model)
        {
            return model.Root.Namespaces
                .Select(ns => (symbol: ns, type: (ns as INamespaceSymbol)?.TryGetNamespaceType()))
                .Where(x => x.type is not null)
                .ToImmutableDictionary(x => x.symbol, x => x.type!);
        }

        private static IEnumerable<CompletionItem> GetAccessibleSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            // maps insert text to the completion item
            var completions = new Dictionary<string, CompletionItem>();

            var declaredNames = new HashSet<string>();

            var accessibleDecoratorFunctionsCache = new Dictionary<NamespaceType, IEnumerable<FunctionSymbol>>();

            var enclosingDeclarationSymbol = context.EnclosingDeclaration == null
                ? null
                : model.GetSymbolInfo(context.EnclosingDeclaration);

            // local function
            void AddSymbolCompletions(IDictionary<string, CompletionItem> result, IEnumerable<Symbol> symbols)
            {
                foreach (var symbol in symbols)
                {
                    if (!result.ContainsKey(symbol.Name) && !ReferenceEquals(symbol, enclosingDeclarationSymbol))
                    {
                        // the symbol satisfies the following conditions:
                        // - we have not added a symbol with the same name (avoids duplicate completions)
                        // - the symbol is different than the enclosing declaration (avoids suggesting cycles)
                        // - the symbol name is different than the name of the enclosing declaration (avoids suggesting a duplicate identifier)
                        result.Add(symbol.Name, CreateSymbolCompletion(symbol, context.ReplacementRange));
                    }
                }
            }

            // local function
            IEnumerable<FunctionSymbol> GetAccessibleDecoratorFunctionsWithCache(NamespaceType namespaceType)
            {
                if (accessibleDecoratorFunctionsCache.TryGetValue(namespaceType, out var result))
                {
                    return result;
                }

                result = GetAccessibleDecoratorFunctions(namespaceType, enclosingDeclarationSymbol);
                accessibleDecoratorFunctionsCache.Add(namespaceType, result);

                return result;
            }

            var nsTypeDict = GetNamespaceTypeBySymbol(model);

            if (!context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName))
            {
                // add namespaces first
                AddSymbolCompletions(completions, nsTypeDict.Keys);

                // add accessible symbols from innermost scope and then move to outer scopes
                // reverse loop iteration
                for (int depth = context.ActiveScopes.Length - 1; depth >= 0; depth--)
                {
                    // add the non-output declarations with valid identifiers at current scope
                    var currentScope = context.ActiveScopes[depth];
                    AddSymbolCompletions(completions, currentScope.Declarations.Where(decl => decl.NameSyntax.IsValid && !(decl is OutputSymbol)));
                }
            }
            else
            {
                // Only add the namespaces that contain accessible decorator function symbols.
                AddSymbolCompletions(completions, nsTypeDict.Keys.Where(
                    ns => GetAccessibleDecoratorFunctionsWithCache(nsTypeDict[ns]).Any()));

                // Record the names of the non-output declarations which will be used to check name clashes later.
                declaredNames.UnionWith(model.Root.Declarations.Where(decl => decl.NameSyntax.IsValid && decl is not OutputSymbol).Select(decl => decl.Name));
            }

            // get names of functions that always require to be fully qualified due to clashes between namespaces
            var alwaysFullyQualifiedNames = nsTypeDict.Values
                .SelectMany(ns => context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName)
                    ? GetAccessibleDecoratorFunctionsWithCache(ns)
                    : ns.MethodResolver.GetKnownFunctions().Values)
                .GroupBy(func => func.Name, (name, functionSymbols) => (name, count: functionSymbols.Count()), LanguageConstants.IdentifierComparer)
                .Where(tuple => tuple.count > 1)
                .Select(tuple => tuple.name)
                .ToHashSet(LanguageConstants.IdentifierComparer);

            foreach (var (symbol, namespaceType) in nsTypeDict)
            {
                var functionSymbols = context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName)
                    ? GetAccessibleDecoratorFunctionsWithCache(namespaceType)
                    : namespaceType.MethodResolver.GetKnownFunctions().Values;

                foreach (var function in functionSymbols)
                {
                    if (function.FunctionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly) && !(enclosingDeclarationSymbol is ParameterSymbol))
                    {
                        // this function is only allowed in param defaults but the enclosing declaration is not bound to a parameter symbol
                        // therefore we should not suggesting this function as a viable completion
                        continue;
                    }

                    if (completions.ContainsKey(function.Name) || alwaysFullyQualifiedNames.Contains(function.Name) || declaredNames.Contains(function.Name))
                    {
                        // either there is a declaration with the same name as the function or the function is ambiguous between the imported namespaces
                        // either way the function must be fully qualified in the completion
                        var fullyQualifiedFunctionName = $"{symbol.Name}.{function.Name}";
                        completions.Add(fullyQualifiedFunctionName, CreateSymbolCompletion(function, context.ReplacementRange, insertText: fullyQualifiedFunctionName));
                    }
                    else
                    {
                        // function does not have to be fully qualified
                        completions.Add(function.Name, CreateSymbolCompletion(function, context.ReplacementRange));
                    }
                }
            }

            return completions.Values;
        }

        private static IEnumerable<FunctionSymbol> GetAccessibleDecoratorFunctions(NamespaceType namespaceType, Symbol? enclosingDeclarationSymbol)
        {
            // Local function.
            IEnumerable<FunctionSymbol> GetAccessible(IEnumerable<FunctionSymbol> symbols, TypeSymbol targetType, FunctionFlags flags) =>
                symbols.Where(functionSymbol => functionSymbol.Overloads.Any(overload =>
                    overload.Flags.HasFlag(flags) &&
                    namespaceType.DecoratorResolver.TryGetDecorator(overload)?.CanAttachTo(targetType) == true));

            var knownDecoratorFunctions = namespaceType.DecoratorResolver.GetKnownDecoratorFunctions().Values;

            return enclosingDeclarationSymbol switch
            {
                MetadataSymbol metadataSymbol => GetAccessible(knownDecoratorFunctions, metadataSymbol.Type, FunctionFlags.MetadataDecorator),
                ParameterSymbol parameterSymbol => GetAccessible(knownDecoratorFunctions, parameterSymbol.Type, FunctionFlags.ParameterDecorator),
                VariableSymbol variableSymbol => GetAccessible(knownDecoratorFunctions, variableSymbol.Type, FunctionFlags.VariableDecorator),
                ResourceSymbol resourceSymbol => GetAccessible(knownDecoratorFunctions, resourceSymbol.Type, FunctionFlags.ResourceDecorator),
                ModuleSymbol moduleSymbol => GetAccessible(knownDecoratorFunctions, moduleSymbol.Type, FunctionFlags.ModuleDecorator),
                OutputSymbol outputSymbol => GetAccessible(knownDecoratorFunctions, outputSymbol.Type, FunctionFlags.OutputDecorator),
                /*
                 * The decorator is dangling if enclosingDeclarationSymbol is null. Return all decorator factory functions since
                 * we don't know which kind of declaration it will attach to.
                 */
                null => knownDecoratorFunctions,
                _ => Enumerable.Empty<FunctionSymbol>()
            };
        }

        private IEnumerable<CompletionItem> GetMemberAccessCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.MemberAccess) || context.PropertyAccess == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredType = compilation.GetEntrypointSemanticModel().GetDeclaredType(context.PropertyAccess.BaseExpression);

            if (context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName) && declaredType is NamespaceType namespaceType)
            {
                var model = compilation.GetEntrypointSemanticModel();
                var enclosingDeclarationSymbol = context.EnclosingDeclaration is null ? null : model.GetSymbolInfo(context.EnclosingDeclaration);

                return GetAccessibleDecoratorFunctions(namespaceType, enclosingDeclarationSymbol)
                    .Select(symbol => CreateSymbolCompletion(symbol, context.ReplacementRange));
            }

            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => CreatePropertyAccessCompletion(p, compilation.SourceFileGrouping.EntryPoint, context.PropertyAccess, context.ReplacementRange))
                .Concat(GetMethods(declaredType)
                    .Select(m => CreateSymbolCompletion(m, context.ReplacementRange)));
        }

        private IEnumerable<CompletionItem> GetResourceAccessCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ResourceAccess) || context.ResourceAccess == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(context.ResourceAccess.BaseExpression) as ResourceSymbol;
            if (symbol == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // Find child resources
            var children = symbol.DeclaringResource.TryGetBody()?.Resources ?? Enumerable.Empty<ResourceDeclarationSyntax>();
            return children
                .Select(r => new { resource = r, symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(r) as ResourceSymbol, })
                .Where(entry => entry.symbol != null)
                .Select(entry => CreateSymbolCompletion(entry.symbol!, context.ReplacementRange));
        }

        private IEnumerable<CompletionItem> GetArrayIndexCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayIndex) || context.ArrayAccess == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredType = compilation.GetEntrypointSemanticModel().GetDeclaredType(context.ArrayAccess.BaseExpression);

            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => CreatePropertyIndexCompletion(p, context.ReplacementRange, CompletionPriority.High));
        }

        private IEnumerable<CompletionItem> GetObjectPropertyNameCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ObjectPropertyName) == false || context.Object == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // in order to provide completions for property names,
            // we need to establish the type of the object first
            var declaredType = model.GetDeclaredType(context.Object);
            if (declaredType == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var specifiedPropertyNames = context.Object.ToNamedPropertyDictionary();

            // exclude read-only properties as they can't be set
            // exclude properties whose name has been specified in the object already
            var includeColon = !context.Kind.HasFlag(BicepCompletionContextKind.ObjectPropertyColonExists);
            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly)
                            && specifiedPropertyNames.ContainsKey(p.Name) == false)
                .Select(p => CreatePropertyNameCompletion(p, includeColon, context.ReplacementRange));
        }

        private static IEnumerable<TypeProperty> GetProperties(TypeSymbol? type)
        {
            return (type switch
            {
                ResourceType resourceType => GetProperties(resourceType.Body.Type),
                ModuleType moduleType => GetProperties(moduleType.Body.Type),
                ObjectType objectType => objectType.Properties.Values,
                DiscriminatedObjectType discriminated => discriminated.DiscriminatorProperty.AsEnumerable(),
                _ => Enumerable.Empty<TypeProperty>(),
            }).Where(p => !p.Flags.HasFlag(TypePropertyFlags.FallbackProperty));
        }

        private static IEnumerable<FunctionSymbol> GetMethods(TypeSymbol? type) =>
            type is ObjectType objectType
                ? objectType.MethodResolver.GetKnownFunctions().Values
                : Enumerable.Empty<FunctionSymbol>();

        private static DeclaredTypeAssignment? GetDeclaredTypeAssignment(SemanticModel model, SyntaxBase? syntax) => syntax == null
            ? null
            : model.GetDeclaredTypeAssignment(syntax);

        private IEnumerable<CompletionItem> GetPropertyValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.PropertyValue))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Property);
            if (declaredTypeAssignment == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var loopsAllowed = context.Property is not null && ForSyntaxValidatorVisitor.IsAddingPropertyLoopAllowed(model, context.Property);
            return GetValueCompletionsForType(model, context, declaredTypeAssignment.Reference.Type, loopsAllowed);
        }

        private IEnumerable<CompletionItem> GetArrayItemCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayItem))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Array);
            if (declaredTypeAssignment?.Reference.Type is not ArrayType arrayType)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return GetValueCompletionsForType(model, context, arrayType.Item.Type, loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetFunctionParamCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.FunctionArgument)
                || context.FunctionArgument is not { } functionArgument
                || model.GetSymbolInfo(functionArgument.Function) is not FunctionSymbol functionSymbol)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var argType = functionSymbol.GetDeclaredArgumentType(functionArgument.ArgumentIndex);

            return GetValueCompletionsForType(model, context, argType, loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetFileCompletionPaths(SemanticModel model,BicepCompletionContext context, TypeSymbol argType)
        {
            if (context.FunctionArgument is not { } functionArgument || !argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringFilePath))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            //try get entered text. we need to provide path completions when something else than string is entered and in that case we use the token value to get what's currently entered
            //(token value for string will have single quotes, so we need to avoid it)
            var entered = (functionArgument.Function.Arguments.ElementAtOrDefault(functionArgument.ArgumentIndex)?.Expression as StringSyntax)?.TryGetLiteralValue() ?? string.Empty;

            // These should only fail if we're not able to resolve cwd path or the entered string
            if (TryGetFilesForPathCompletions(model.SourceFile.FileUri, entered) is not {} fileCompletionInfo)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            IEnumerable<CompletionItem> fileItems;
            if (argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringJsonFilePath))
            {
                // Prioritize .json or .jsonc files higher than other files.
                var jsonItems = CreateFileCompletionItems(model.SourceFile.FileUri, context.ReplacementRange, fileCompletionInfo, (file) => PathHelper.HasExtension(file, "json") || PathHelper.HasExtension(file, "jsonc"), CompletionPriority.High);
                var nonJsonItems = CreateFileCompletionItems(model.SourceFile.FileUri, context.ReplacementRange, fileCompletionInfo, (file) => !PathHelper.HasExtension(file, "json") && !PathHelper.HasExtension(file, "jsonc"), CompletionPriority.Medium);
                fileItems = jsonItems.Concat(nonJsonItems);
            }
            else
            {
                fileItems = CreateFileCompletionItems(model.SourceFile.FileUri, context.ReplacementRange, fileCompletionInfo, (_) => true, CompletionPriority.High);
            }

            var dirItems = CreateDirectoryCompletionItems(context.ReplacementRange, fileCompletionInfo, CompletionPriority.Medium);

            return fileItems.Concat(dirItems);
        }

        private IEnumerable<CompletionItem> GetValueCompletionsForType(SemanticModel model, BicepCompletionContext context, TypeSymbol? type, bool loopsAllowed)
        {
            var replacementRange = context.ReplacementRange;
            switch (type)
            {
                case PrimitiveType _ when ReferenceEquals(type, LanguageConstants.Bool):
                    yield return CreateKeywordCompletion(LanguageConstants.TrueKeyword, LanguageConstants.TrueKeyword, replacementRange, preselect: true, CompletionPriority.High);
                    yield return CreateKeywordCompletion(LanguageConstants.FalseKeyword, LanguageConstants.FalseKeyword, replacementRange, preselect: true, CompletionPriority.High);

                    break;

                case PrimitiveType _ when type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringFilePath):
                    foreach (var completion in GetFileCompletionPaths(model, context, type))
                    {
                        yield return completion;
                    }
                    break;

                case StringLiteralType stringLiteral:
                    yield return CompletionItemBuilder.Create(CompletionItemKind.EnumMember, stringLiteral.Name)
                        .WithPlainTextEdit(replacementRange, stringLiteral.Name)
                        .WithDetail(stringLiteral.Name)
                        .Preselect()
                        .WithSortText(GetSortText(stringLiteral.Name, CompletionPriority.Medium))
                        .Build();

                    break;

                case ArrayType arrayType:
                    const string arrayLabel = "[]";
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Value, arrayLabel)
                        .WithSnippetEdit(replacementRange, "[\n\t$0\n]")
                        .WithDetail(arrayLabel)
                        .Preselect()
                        .WithSortText(GetSortText(arrayLabel, CompletionPriority.High))
                        .Build();

                    if (loopsAllowed)
                    {
                        // property loop is allowed here
                        foreach (var completion in CreateLoopCompletions(replacementRange, arrayType.Item.Type, filtersAllowed: false))
                        {
                            yield return completion;
                        }
                    }

                    break;

                case DiscriminatedObjectType _:
                case ObjectType _:
                    foreach (CompletionItem completionItem in GetObjectBodyCompletions(type, replacementRange))
                    {
                        yield return completionItem;
                    }

                    break;

                case UnionType union:
                    var aggregatedCompletions = union.Members.SelectMany(typeRef => GetValueCompletionsForType(model, context, typeRef.Type, loopsAllowed));
                    foreach (var completion in aggregatedCompletions)
                    {
                        yield return completion;
                    }
                    break;

                case LambdaType lambda:
                    var (snippet, label) = lambda.ArgumentTypes.Length switch {
                        1 => (
                            "${1:arg} => $0",
                            "arg => ..."),
                        _ => (
                            $"({string.Join(", ", lambda.ArgumentTypes.Select((x, i) => $"${{{i + 1}:arg{i + 1}}}"))}) => $0",
                            $"({string.Join(", ", lambda.ArgumentTypes.Select((x, i) => $"arg{i + 1}"))}) => ..."),
                    };

                    yield return CreateContextualSnippetCompletion(label, label, snippet, replacementRange, CompletionPriority.High);
                    break;
            }
        }

        private IEnumerable<CompletionItem> GetObjectBodyCompletions(TypeSymbol typeSymbol, Range replacementRange)
        {
            IEnumerable<Snippet> snippets = SnippetsProvider.GetObjectBodyCompletionSnippets(typeSymbol);

            foreach (Snippet snippet in snippets)
            {
                string prefix = snippet.Prefix;
                BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateObjectBodySnippetInsertion(prefix);
                var command = TelemetryHelper.CreateCommand
                (
                    title: "object body completion snippet",
                    name: TelemetryConstants.CommandName,
                    args: JArray.FromObject(new List<object> { telemetryEvent })
                );
                yield return CreateContextualSnippetCompletion(prefix,
                    snippet.Detail,
                    snippet.Text,
                    replacementRange,
                    command: command,
                    snippet.CompletionPriority,
                    preselect: true);
            }
        }

        private IEnumerable<CompletionItem> GetExpressionCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.Expression))
            {
                yield return CreateConditionCompletion(context.ReplacementRange);
            }
        }

        private IEnumerable<CompletionItem> GetDisableNextLineDiagnosticsDirectiveCompletion(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.DisableNextLineDiagnosticsDirectiveStart))
            {
                yield return CreateKeywordCompletion(LanguageConstants.DisableNextLineDiagnosticsKeyword, "Disable next line diagnostics directive", context.ReplacementRange);
            }
        }

        private IEnumerable<CompletionItem> GetDisableNextLineDiagnosticsDirectiveCodesCompletion(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.DisableNextLineDiagnosticsCodes))
            {
                foreach (var diagnostic in GetDiagnosticCodes(context.ReplacementRange, model))
                {
                    yield return CreateKeywordCompletion(diagnostic.Code, diagnostic.Message, context.ReplacementRange);
                }
            }
        }

        private IEnumerable<IDiagnostic> GetDiagnosticCodes(Range range, SemanticModel model)
        {
            var lineStarts = model.SourceFile.LineStarts;
            var position = GetPosition(range, lineStarts);
            (var line, _) = TextCoordinateConverter.GetPosition(lineStarts, position);
            var nextLineSpan = GetNextLineSpan(lineStarts, line, model.SourceFile.ProgramSyntax);

            if (nextLineSpan is null)
            {
                return Enumerable.Empty<IDiagnostic>();
            }

            return model.GetAllDiagnostics()
                .Where(diagnostic => nextLineSpan.ContainsInclusive(diagnostic.Span.Position) && diagnostic.CanBeSuppressed())
                .DistinctBy(x => x.Code);
        }

        private int GetPosition(Range range, ImmutableArray<int> lineStarts)
        {
            var rangeStart = range.Start;
            return lineStarts[rangeStart.Line] + rangeStart.Character;
        }

        private TextSpan? GetNextLineSpan(ImmutableArray<int> lineStarts, int line, ProgramSyntax programSyntax)
        {
            var nextLine = line + 1;
            if (lineStarts.Length > nextLine)
            {
                var nextLineStart = lineStarts[nextLine];

                int nextLineEnd;

                if (lineStarts.Length > nextLine + 1)
                {
                    nextLineEnd = lineStarts[nextLine + 1] - 1;
                }
                else
                {
                    nextLineEnd = programSyntax.GetEndPosition();
                }

                return new TextSpan(nextLineStart, nextLineEnd - nextLineStart);
            }

            return null;
        }

        private static CompletionItem CreateResourceOrModuleConditionCompletion(Range replacementRange)
        {
            const string conditionLabel = "if";
            return CompletionItemBuilder.Create(CompletionItemKind.Snippet, conditionLabel)
                .WithSnippetEdit(replacementRange, "if (${1:condition}) {\n\t$0\n}")
                .WithDetail(conditionLabel)
                .WithSortText(GetSortText(conditionLabel, CompletionPriority.High))
                .Build();
        }

        private static CompletionItem CreateConditionCompletion(Range replacementRange)
        {
            const string conditionLabel = "if-else";
            return CompletionItemBuilder.Create(CompletionItemKind.Snippet, conditionLabel)
                .WithSnippetEdit(replacementRange, "${1:condition} ? ${2:TrueValue} : ${3:FalseValue}")
                .WithDetail(conditionLabel)
                .WithSortText(GetSortText(conditionLabel, CompletionPriority.High))
                .Build();
        }

        private static IEnumerable<CompletionItem> CreateLoopCompletions(Range replacementRange, TypeSymbol arrayItemType, bool filtersAllowed)
        {
            const string loopLabel = "for";
            const string indexedLabel = "for-indexed";
            const string filteredLabel = "for-filtered";

            var assignableToObject = TypeValidator.AreTypesAssignable(arrayItemType, LanguageConstants.Object);
            var assignableToArray = TypeValidator.AreTypesAssignable(arrayItemType, LanguageConstants.Array);

            var (itemSnippet, indexedSnippet) = (assignableToObject, assignableToArray) switch
            {
                (true, false) => ("[for ${2:item} in ${1:list}: {\n\t$0\n}]", "[for (${2:item}, ${3:index}) in ${1:list}: {\n\t$0\n}]"),
                (false, true) => ("[for ${2:item} in ${1:list}: [\n\t$0\n]]", "[for (${2:item}, ${3:index}) in ${1:list}: [\n\t$0\n]]"),
                _ => ("[for ${2:item} in ${1:list}: $0]", "[for (${2:item}, ${3:index}) in ${1:list}: $0]")
            };

            yield return CreateContextualSnippetCompletion(loopLabel, loopLabel, itemSnippet, replacementRange, CompletionPriority.High);
            yield return CreateContextualSnippetCompletion(indexedLabel, indexedLabel, indexedSnippet, replacementRange, CompletionPriority.High);

            if (filtersAllowed && assignableToObject && !assignableToArray)
            {
                yield return CreateContextualSnippetCompletion(filteredLabel, filteredLabel, "[for (${2:item}, ${3:index}) in ${1:list}: if (${4:condition}) {\n\t$0\n}]", replacementRange, CompletionPriority.High);
            }
        }

        private static CompletionItem CreatePropertyNameCompletion(TypeProperty property, bool includeColon, Range replacementRange)
        {
            var required = property.Flags.HasFlag(TypePropertyFlags.Required);

            var escapedPropertyName = IsPropertyNameEscapingRequired(property) ? StringUtils.EscapeBicepString(property.Name) : property.Name;
            var suffix = includeColon ? ":" : string.Empty;
            return CompletionItemBuilder.Create(CompletionItemKind.Property, property.Name)
                // property names that much Bicep keywords or containing non-identifier chars need to be escaped
                .WithPlainTextEdit(replacementRange, $"{escapedPropertyName}{suffix}")
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(property.Name, required ? CompletionPriority.High : CompletionPriority.Medium))
                .Preselect(required)
                .Build();
        }

        private static CompletionItem CreatePropertyIndexCompletion(TypeProperty property, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var escaped = StringUtils.EscapeBicepString(property.Name);
            return CompletionItemBuilder.Create(CompletionItemKind.Property, escaped)
                .WithPlainTextEdit(replacementRange, escaped)
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(escaped, priority))
                .Build();
        }

        private static CompletionItem CreatePropertyAccessCompletion(TypeProperty property, BicepFile tree, PropertyAccessSyntax propertyAccess, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var item = CompletionItemBuilder.Create(CompletionItemKind.Property, property.Name)
                .WithCommitCharacters(PropertyAccessCommitChars)
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(property.Name, priority));

            if (IsPropertyNameEscapingRequired(property))
            {
                // the property requires escaping because it does not comply with bicep identifier rules
                // in bicep those types of properties are accessed via array indexer using a string as an index
                // if we update the main edit of the completion, vs code will not show such a completion at all
                // thus we will append additional text edits to replace the . with a [ and to insert the closing ]
                item
                    .WithPlainTextEdit(replacementRange, $"[{StringUtils.EscapeBicepString(property.Name)}]")
                    .WithAdditionalEdits(new TextEditContainer(
                        // remove the dot after the main text edit is applied
                        new TextEdit
                        {
                            NewText = string.Empty,
                            Range = propertyAccess.Dot.ToRange(tree.LineStarts)
                        }));
            }
            else
            {
                item.WithPlainTextEdit(replacementRange, property.Name);
            }

            return item.Build();
        }

        private static CompletionItem CreateKeywordCompletion(string keyword, string detail, Range replacementRange, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Keyword, keyword)
                .WithPlainTextEdit(replacementRange, keyword)
                .WithDetail(detail)
                .Preselect(preselect)
                .WithSortText(GetSortText(keyword, priority))
                .Build();

        private static CompletionItem CreateTypeCompletion(TypeSymbol type, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Class, type.Name)
                .WithPlainTextEdit(replacementRange, type.Name)
                .WithDetail(type.Name)
                .WithSortText(GetSortText(type.Name, priority))
                .Build();

        private static CompletionItem CreateResourceTypeKeywordCompletion(Range replacementRange, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Class, LanguageConstants.ResourceKeyword)
                .WithPlainTextEdit(replacementRange, LanguageConstants.ResourceKeyword)
                .WithDetail(LanguageConstants.ResourceKeyword)
                .WithSortText(GetSortText(LanguageConstants.ResourceKeyword, priority))
                .Build();

        private static CompletionItem CreateOperatorCompletion(string op, Range replacementRange, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Operator, op)
                .WithPlainTextEdit(replacementRange, op)
                .Preselect(preselect)
                .WithSortText(GetSortText(op, priority))
                .Build();

        private static CompletionItem CreateResourceTypeCompletion(ResourceTypeReference resourceType, int index, Range replacementRange, bool showApiVersion)
        {
            // Splitting ResourceType Completion in to two pieces, one for the 'Namespace/type', the second for '@<api-version>'
            if (showApiVersion && resourceType.ApiVersion is not null)
            {
                var insertText = StringUtils.EscapeBicepString($"{resourceType.FormatType()}@{resourceType.ApiVersion}");
                return CompletionItemBuilder.Create(CompletionItemKind.Class, resourceType.ApiVersion)
                    .WithFilterText(insertText)
                    .WithPlainTextEdit(replacementRange, insertText)
                    .WithDocumentation($"Type: `{resourceType.FormatType()}`{MarkdownNewLine}API Version: `{resourceType.ApiVersion}`")
                    // 8 hex digits is probably overkill :)
                    .WithSortText(index.ToString("x8"))
                    .Build();
            }
            else
            {
                var insertText = StringUtils.EscapeBicepString($"{resourceType.FormatType()}");
                return CompletionItemBuilder.Create(CompletionItemKind.Class, insertText)
                    .WithSnippetEdit(replacementRange, $"{insertText.Substring(0, insertText.Length - 1)}@$0'")
                    .WithDocumentation($"Type: `{resourceType.FormatType()}`{MarkdownNewLine}`")
                    .WithCommand(new Command { Name = EditorCommands.RequestCompletions, Title = "resource type completion" })
                    // 8 hex digits is probably overkill :)
                    .WithSortText(index.ToString("x8"))
                    .Build();
            }
        }

        private static CompletionItem CreateResourceTypeSegmentCompletion(ResourceTypeReference resourceType, int index, Range replacementRange, bool includeApiVersion, string? displayApiVersion)
        {
            // We create one completion with and without the API version.
            var insertText = includeApiVersion && (resourceType.ApiVersion is not null) ?
                StringUtils.EscapeBicepString($"{resourceType.TypeSegments[^1]}@{resourceType.ApiVersion}") :
                StringUtils.EscapeBicepString($"{resourceType.TypeSegments[^1]}");

            var apiVersionMarkdown = displayApiVersion is not null ? $"{MarkdownNewLine}API Version: `{displayApiVersion}`" : "";

            return CompletionItemBuilder.Create(CompletionItemKind.Class, insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .WithDocumentation($"Type: `{resourceType.FormatType()}`{apiVersionMarkdown}")
                // 8 hex digits is probably overkill :)
                .WithSortText(index.ToString("x8"))
                .Build();
        }

        private static CompletionItemBuilder CreateFilePathCompletionBuilder(string name, string path, Range replacementRange, CompletionItemKind completionItemKind, CompletionPriority priority)
        {
            // unescape string according to https://stackoverflow.com/questions/575440/url-encoding-using-c-sharp
            path = StringUtils.EscapeBicepString(Uri.UnescapeDataString(path));
            var item = CompletionItemBuilder.Create(completionItemKind, Uri.UnescapeDataString(name))
                .WithFilterText(path)
                .WithSortText(GetSortText(name, priority));
            // Folder completions should keep us within the completion string
            if (completionItemKind.Equals(CompletionItemKind.Folder))
            {
                item.WithSnippetEdit(replacementRange, $"{path.Substring(0, path.Length - 1)}$0'");
            }
            else
            {
                item = item.WithPlainTextEdit(replacementRange, path);
            }

            return item;
        }

        /// <summary>
        /// Creates a completion with a contextual snippet. This will look like a snippet to the user.
        /// </summary>
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium, bool preselect = false) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithDetail(detail)
                .WithDocumentation($"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```")
                .WithSortText(GetSortText(label, priority))
                .Preselect(preselect)
                .Build();

        /// <summary>
        /// Creates a completion with a contextual snippet with command option. This will look like a snippet to the user.
        /// </summary>
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, Command command, CompletionPriority priority = CompletionPriority.Medium, bool preselect = false) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithCommand(command)
                .WithDetail(detail)
                .WithDocumentation($"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```")
                .WithSortText(GetSortText(label, priority))
                .Preselect(preselect)
                .Build();

        /// <summary>
        /// Creates a completion with a contextual snippet. This will look like a snippet to the user.
        /// </summary>
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, TextEditContainer additionalTextEdits, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithAdditionalEdits(additionalTextEdits)
                .WithDetail(detail)
                .WithDocumentation($"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```")
                .WithSortText(GetSortText(label, priority))
                .Build();

        private static CompletionItem CreateSymbolCompletion(Symbol symbol, Range replacementRange, string? insertText = null)
        {
            insertText ??= symbol.Name;
            var kind = GetCompletionItemKind(symbol);
            var priority = GetCompletionPriority(symbol);

            var completion = CompletionItemBuilder.Create(kind, insertText)
                .WithSortText(GetSortText(insertText, priority));

            if (symbol is ResourceSymbol)
            {
                // treat : as a commit character for the resource access operator case
                completion.WithCommitCharacters(ResourceSymbolCommitChars);
            }

            if (symbol is FunctionSymbol function)
            {
                // for functions without any parameters on all the overloads, we should be placing the cursor after the parentheses
                // for all other functions, the cursor should land between the parentheses so the user can specify the arguments
                bool hasParameters = function.Overloads.Any(overload => overload.HasParameters);
                var snippet = hasParameters
                    ? $"{insertText}($0)"
                    : $"{insertText}()$0";

                if (hasParameters)
                {
                    // if parameters may need to be specified, automatically request signature help
                    completion.WithCommand(new Command { Name = EditorCommands.SignatureHelp, Title = "signature help" });
                }

                ImmutableArray<FunctionOverload> overloads = function.Overloads;
                var genericDescription = overloads.First().GenericDescription;
                var description = overloads.First().Description;
                string codeBlock = string.Empty;

                foreach (var overload in overloads)
                {
                    codeBlock = $"{codeBlock}{function.Name}{overload.TypeSignature}\n";
                }

                return completion
                    .WithDocumentation(MarkdownHelper.CodeBlockWithDescription(codeBlock, genericDescription.ToString()))
                    .WithSnippetEdit(replacementRange, snippet)
                    .Build();
            }

            if (symbol is INamespaceSymbol)
            {
                // trigger follow up completions
                return completion
                    .WithDetail(insertText)
                    .WithPlainTextEdit(replacementRange, insertText + ".")
                    .WithCommand(new Command { Name = EditorCommands.RequestCompletions, Title = "symbol completion" })
                    .Build();
            }

            return completion
                .WithDetail(insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .Build();
        }

        private IEnumerable<CompletionItem> GetImportCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ImportProviderFollower))
            {
                yield return CreateKeywordCompletion(LanguageConstants.AsKeyword, "As keyword", context.ReplacementRange);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ImportFollower))
            {
                foreach (var builtInNamespace in namespaceProvider.AvailableNamespaces.OrderBy(x => x, LanguageConstants.IdentifierComparer))
                {
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Folder, builtInNamespace)
                        .WithSortText(GetSortText(builtInNamespace, CompletionPriority.High))
                        .WithDetail(builtInNamespace)
                        .WithPlainTextEdit(context.ReplacementRange, builtInNamespace)
                        .Build();
                }
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ImportAliasFollower))
            {
                if (context.EnclosingDeclaration is ImportDeclarationSyntax importSyntax &&
                    model.GetSymbolInfo(importSyntax) is ImportedNamespaceSymbol importSymbol &&
                    importSymbol.TryGetNamespaceType() is {} namespaceType)
                {
                    foreach (var completion in GetValueCompletionsForType(model, context, namespaceType.ConfigurationType, loopsAllowed: false))
                    {
                        yield return completion;
                    }
                }
            }
        }

        // the priority must be a number in the sort text
        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";

        private static CompletionPriority GetCompletionPriority(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionPriority.Low,
                SymbolKind.Namespace => CompletionPriority.Low,
                SymbolKind.ImportedNamespace => CompletionPriority.Low,
                _ => CompletionPriority.Medium
            };

        private static CompletionItemKind GetCompletionItemKind(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionItemKind.Function,
                SymbolKind.File => CompletionItemKind.File,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Namespace => CompletionItemKind.Folder,
                SymbolKind.ImportedNamespace => CompletionItemKind.Folder,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.Resource => CompletionItemKind.Interface,
                SymbolKind.Module => CompletionItemKind.Module,
                SymbolKind.Local => CompletionItemKind.Variable,

                _ => CompletionItemKind.Text
            };

        private static bool IsPropertyNameEscapingRequired(TypeProperty property) =>
            !Lexer.IsValidIdentifier(property.Name) || LanguageConstants.Keywords.ContainsKey(property.Name);

        private static string FormatPropertyDetail(TypeProperty property) =>
            property.Flags.HasFlag(TypePropertyFlags.Required)
                ? $"{property.Name} (Required)"
                : property.Name;

        private static string FormatPropertyDocumentation(TypeProperty property)
        {
            var buffer = new StringBuilder();

            buffer.Append($"Type: `{property.TypeReference.Type}`{MarkdownNewLine}");

            if (property.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            {
                // this case will be used for dot property access completions
                // this flag is not possible in property name completions
                buffer.Append($"Read-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
            {
                buffer.Append($"Write-only property{MarkdownNewLine}");
            }

            if (property.Flags.HasFlag(TypePropertyFlags.Constant))
            {
                buffer.Append($"Requires a compile-time constant value.{MarkdownNewLine}");
            }

            if (property.Description is not null)
            {
                buffer.Append($"{property.Description}{MarkdownNewLine}");
            }

            return buffer.ToString();
        }
    }
}
