// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;
using Azure.Deployments.Core.Comparers;
using Bicep.Core;
using Bicep.Core.Analyzers.Linter.Common;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Semantics.Metadata;
using Bicep.Core.Semantics.Namespaces;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.Text;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.IO.Abstraction;
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
    public partial class BicepCompletionProvider : ICompletionProvider
    {
        private static readonly Container<string> ResourceSymbolCommitChars = new(":");

        private static readonly Container<string> PropertyAccessCommitChars = new(".");

        private static readonly ResourceTypeSearchKeywords ResourceTypeSearchKeywords = new();

        private readonly IModuleReferenceCompletionProvider moduleReferenceCompletionProvider;
        private readonly ISnippetsProvider snippetsProvider;

        public BicepCompletionProvider(ISnippetsProvider snippetsProvider, IModuleReferenceCompletionProvider moduleReferenceCompletionProvider)
        {
            this.snippetsProvider = snippetsProvider;
            this.moduleReferenceCompletionProvider = moduleReferenceCompletionProvider;
        }

        public async Task<IEnumerable<CompletionItem>> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context, CancellationToken cancellationToken)
        {
            var model = compilation.GetEntrypointSemanticModel();

            return GetDeclarationCompletions(model, context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(model, context))
                .Concat(GetObjectPropertyNameCompletions(model, context))
                .Concat(GetMemberAccessCompletions(compilation, context))
                .Concat(GetTypeMemberAccessCompletions(compilation, context))
                .Concat(GetResourceAccessCompletions(compilation, context))
                .Concat(GetArrayIndexCompletions(compilation, context))
                .Concat(GetPropertyValueCompletions(model, context))
                .Concat(GetArrayItemCompletions(model, context))
                .Concat(GetResourceTypeCompletions(model, context))
                .Concat(GetResourceTypeFollowerCompletions(context))
                .Concat(GetLocalModulePathCompletions(model, context))
                .Concat(GetModuleBodyCompletions(model, context))
                .Concat(GetTestBodyCompletions(model, context))
                .Concat(GetResourceBodyCompletions(model, context))
                .Concat(GetParameterDefaultValueCompletions(model, context))
                .Concat(GetVariableValueCompletions(model, context))
                .Concat(GetOutputValueCompletions(model, context))
                .Concat(GetOutputTypeFollowerCompletions(context))
                .Concat(GetVariableNameFollowerCompletions(context))
                .Concat(GetTargetScopeCompletions(model, context))
                .Concat(GetExtensionCompletions(model, context))
                .Concat(GetCompileTimeImportCompletions(model, context))
                .Concat(GetFunctionParamCompletions(model, context))
                .Concat(GetExpressionCompletions(model, context))
                .Concat(GetDisableNextLineDiagnosticsDirectiveCompletion(context))
                .Concat(GetDisableNextLineDiagnosticsDirectiveCodesCompletion(model, context))
                .Concat(GetParamIdentifierCompletions(model, context))
                .Concat(GetParamValueCompletions(model, context))
                .Concat(GetAssertValueCompletions(model, context))
                .Concat(GetTypeArgumentCompletions(model, context))
                .Concat(GetUsingWithCompletions(model, context))
                .Concat(await moduleReferenceCompletionProvider.GetFilteredCompletions(model.SourceFile, context, cancellationToken));
        }

        public Task<CompletionItem> Resolve(CompletionItem completionItem, CancellationToken cancellationToken)
        {
            return moduleReferenceCompletionProvider.ResolveCompletionItem(completionItem, cancellationToken);
        }

        private static IEnumerable<CompletionItem> GetParamIdentifierCompletions(SemanticModel paramsSemanticModel, BicepCompletionContext paramsCompletionContext)
        {
            if (paramsCompletionContext.Kind.HasFlag(BicepCompletionContextKind.ParamIdentifier) &&
                paramsSemanticModel.Root.TryGetBicepFileSemanticModelViaUsing().IsSuccess(out var usingModel))
            {
                foreach (var metadata in usingModel.Parameters.Values)
                {
                    if (paramsSemanticModel.TryGetParameterAssignment(metadata) is null)
                    {
                        yield return CompletionItemBuilder
                            .Create(
                                GetCompletionItemKind(SymbolKind.ParameterAssignment),
                                metadata.Name)
                            .WithDocumentation(
                                MarkdownHelper.AppendNewline($"Type: `{metadata.TypeReference.Type}`") +
                                MarkdownHelper.AppendNewline(metadata.Description))
                            .WithDetail(!metadata.IsRequired ? metadata.Name : $"{metadata.Name} (Required)")
                            .WithPlainTextEdit(paramsCompletionContext.ReplacementRange, metadata.Name)
                            .Build();
                    }
                }
            }
        }

        private IEnumerable<CompletionItem> GetParamValueCompletions(SemanticModel paramsSemanticModel, BicepCompletionContext paramsCompletionContext)
        {
            if (!paramsCompletionContext.Kind.HasFlag(BicepCompletionContextKind.ParamValue) ||
                paramsCompletionContext.EnclosingDeclaration is not ParameterAssignmentSyntax paramAssignment)
            {
                return [];
            }

            var declaredType = paramsSemanticModel.GetDeclaredType(paramAssignment);

            // loops are not allowed in param files... yet!
            return GetValueCompletionsForType(paramsSemanticModel, paramsCompletionContext, declaredType, paramAssignment.Value, loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.TopLevelDeclarationStart))
            {
                switch (model.SourceFileKind)
                {
                    case BicepSourceFileKind.BicepFile:
                        yield return CreateKeywordCompletion(LanguageConstants.MetadataKeyword, "Metadata keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.VariableKeyword, "Variable keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange, priority: CompletionPriority.High);
                        yield return CreateKeywordCompletion(LanguageConstants.OutputKeyword, "Output keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.ModuleKeyword, "Module keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.TargetScopeKeyword, "Target Scope keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.TypeKeyword, "Type keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.ImportKeyword, "Import keyword", context.ReplacementRange);
                        yield return CreateKeywordCompletion(LanguageConstants.ExtensionKeyword, "Extension keyword", context.ReplacementRange);

                        if (model.Features.TestFrameworkEnabled)
                        {
                            yield return CreateKeywordCompletion(LanguageConstants.TestKeyword, "Test keyword", context.ReplacementRange);
                        }

                        yield return CreateContextualSnippetCompletion(
                            LanguageConstants.FunctionKeyword,
                            "Function declaration",
                            "func ${1:name}() ${2:outputType} => $0",
                            context.ReplacementRange);

                        if (model.Features.AssertsEnabled)
                        {
                            yield return CreateKeywordCompletion(LanguageConstants.AssertKeyword, "Assert keyword", context.ReplacementRange);
                        }

                        foreach (Snippet resourceSnippet in snippetsProvider.GetTopLevelNamedDeclarationSnippets())
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
                                                                           resourceSnippet.CompletionPriority,
                                                                           filterText: ResourceTypeSearchKeywords.TryGetSnippetFilterText(resourceSnippet));
                        }

                        break;

                    case BicepSourceFileKind.ParamsFile:
                        // the using declaration is a singleton
                        // we should not offer completions for it more than once
                        if (model.Root.UsingDeclarationSyntax is null)
                        {
                            yield return CreateKeywordCompletion(LanguageConstants.UsingKeyword, "Using keyword", context.ReplacementRange);
                        }

                        yield return CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter assignment keyword", context.ReplacementRange);

                        if (model.Features.ModuleExtensionConfigsEnabled)
                        {
                            yield return CreateKeywordCompletion(LanguageConstants.ExtensionConfigKeyword, "Extension config assignment keyword", context.ReplacementRange);
                        }

                        yield return CreateKeywordCompletion(LanguageConstants.ExtendsKeyword, "Extends keyword", context.ReplacementRange);

                        break;

                    default:
                        throw new NotImplementedException($"Unexpected source file kind '{model.SourceFileKind}'.");
                }
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.NestedResourceDeclarationStart) && context.EnclosingDeclaration is ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange);

                if (model.GetSymbolInfo(resourceDeclarationSyntax) is ResourceSymbol parentSymbol &&
                    parentSymbol.TryGetResourceTypeReference() is ResourceTypeReference parentTypeReference)
                {
                    foreach (Snippet snippet in snippetsProvider.GetNestedResourceDeclarationSnippets(parentTypeReference))
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
                ? GetValueCompletionsForType(model, context, model.GetDeclaredType(targetScope), targetScope.Assignment, loopsAllowed: false)
                : [];
        }

        private static IEnumerable<CompletionItem> GetSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.Expression) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName))
            {
                return [];
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName | BicepCompletionContextKind.MemberAccess))
            {
                // This is already handled by GetMemberAccessCompletions.
                return [];
            }

            if (context.Property != null && model.GetDeclaredTypeAssignment(context.Property)?.Flags == DeclaredTypeFlags.Constant)
            {
                // the enclosing property's declared type is supposed to be a constant value
                // the constant flag comes from TypeProperty constant flag, so nothing else can really alter it except for another property
                // (in other words constant flag inherits down into the expression tree of the property value)
                return [];
            }

            // when we're inside an expression that is inside a property that expects a compile-time constant value,
            // we should not be emitting accessible symbol completions
            return GetAccessibleSymbolCompletions(model, context);
        }

        private IEnumerable<CompletionItem> GetDeclarationTypeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ParameterType))
            {
                var completions = GetTypeCompletions(model, context)
                    .Concat(GetParameterTypeSnippets(model, context));

                // Only show the resource type as a completion if the resource-typed parameter feature is enabled.
                if (model.Features.ResourceTypedParamsAndOutputsEnabled)
                {
                    completions = completions.Concat(CreateResourceTypeKeywordCompletion(context.ReplacementRange));
                }

                return completions;
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.TypeDeclarationValue))
            {
                return GetAmbientTypeCompletions(model, context)
                    .Concat(GetUserDefinedTypeCompletions(model, context, declaredType => !ReferenceEquals(declaredType.DeclaringType, context.EnclosingDeclaration)))
                    .Concat(GetImportedTypeCompletions(model, context));
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ObjectTypePropertyValue))
            {
                return GetTypeCompletions(model, context);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.UnionTypeMember))
            {
                // union types must be composed of literals, so don't include primitive types or non-literal user defined types
                var cyclableType = CyclableTypeEnclosingDeclaration(model.Binder, context.ReplacementTarget);
                return GetUserDefinedTypeCompletions(model, context, declared => !ReferenceEquals(declared.DeclaringType, cyclableType) && IsTypeLiteralSyntax(declared.DeclaringType.Value));
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.TypedLocalVariableType) ||
                context.Kind.HasFlag(BicepCompletionContextKind.TypedLambdaOutputType))
            {
                return GetTypeCompletions(model, context);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.VariableNameFollower))
            {
                return GetTypeCompletions(model, context);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.OutputType))
            {
                var completions = GetTypeCompletions(model, context);

                // Only show the resource type as a completion if the resource-typed parameter feature is enabled.
                if (model.Features.ResourceTypedParamsAndOutputsEnabled)
                {
                    completions = completions.Concat(CreateResourceTypeKeywordCompletion(context.ReplacementRange));
                }

                return completions;
            }

            return [];
        }

        private static IEnumerable<CompletionItem> GetTypeCompletions(SemanticModel model, BicepCompletionContext context)
            => GetAmbientTypeCompletions(model, context)
                .Concat(GetUserDefinedTypeCompletions(model, context))
                .Concat(GetImportedTypeCompletions(model, context));

        private static IEnumerable<CompletionItem> GetAmbientTypeCompletions(SemanticModel model, BicepCompletionContext context) => model.Binder.NamespaceResolver.GetKnownTypes()
            .Where(ambientType => !ambientType.Flags.HasFlag(TypePropertyFlags.FallbackProperty))
            .ToLookup(ambientType => ambientType.Name)
            .SelectMany(grouping => grouping.Count() > 1 || model.Binder.FileSymbol.Declarations.Any(decl => LanguageConstants.IdentifierComparer.Equals(grouping.Key, decl.Name))
                ? grouping.Select(ambientType => ($"{ambientType.DeclaringNamespace.Name}.{ambientType.Name}", ambientType))
                : grouping.Select(ambientType => (ambientType.Name, ambientType)))
            .Select(tuple => CreateTypeCompletion(tuple.Item1, tuple.ambientType, context.ReplacementRange));

        private static IEnumerable<CompletionItem> GetUserDefinedTypeCompletions(SemanticModel model, BicepCompletionContext context, Func<TypeAliasSymbol, bool>? filter = null)
        {
            IEnumerable<TypeAliasSymbol> declarationsForCompletions = model.Root.TypeDeclarations;

            if (filter is not null)
            {
                declarationsForCompletions = declarationsForCompletions.Where(filter);
            }

            return declarationsForCompletions.Select(declaredType => CreateDeclaredTypeCompletion(model, declaredType, context.ReplacementRange, CompletionPriority.High));
        }

        private static IEnumerable<CompletionItem> GetImportedTypeCompletions(SemanticModel model, BicepCompletionContext context)
            => model.Root.ImportedTypes
                .Select(importedType => CreateImportedCompletion(importedType, context.ReplacementRange, CompletionPriority.High))
                .Concat(model.Root.WildcardImports
                    .SelectMany(wildcardImport => wildcardImport.SourceModel.Exports.Values.OfType<ExportedTypeMetadata>()
                        .Select(exportMetadata => (wildcardImport, exportMetadata)))
                    .Select(t => CreateWildcardPropertyCompletion(t.Item1, t.Item2, context.ReplacementRange, CompletionPriority.High)));

        private static bool IsTypeLiteralSyntax(SyntaxBase syntax) => syntax is BooleanTypeLiteralSyntax
            || syntax is IntegerTypeLiteralSyntax
            || (syntax is StringTypeLiteralSyntax @string && @string.SegmentValues.Length == 1)
            || syntax is UnionTypeSyntax
            || (syntax is ObjectTypeSyntax objectType && objectType.Properties.All(p => IsTypeLiteralSyntax(p.Value)))
            || (syntax is TupleTypeSyntax tupleType && tupleType.Items.All(i => IsTypeLiteralSyntax(i.Value)));

        private static StatementSyntax? CyclableTypeEnclosingDeclaration(IBinder binder, SyntaxBase? syntax) => syntax switch
        {
            StatementSyntax statement => statement,
            // Aggregate types have special rules around cycles and nullability. Stop looking for cycles if you hit one while climbing the syntax hierarchy for a given type declaration
            ArrayTypeMemberSyntax or ObjectTypePropertySyntax or TupleTypeItemSyntax => null,
            SyntaxBase otherwise => CyclableTypeEnclosingDeclaration(binder, binder.GetParent(otherwise)),
            null => null,
        };

        private static string? TryGetSkippedTokenText(SkippedTriviaSyntax skippedTrivia)
        {
            // This method attempts to obtain text from a skipped token - in cases where the user has partially-typed syntax
            // but may be looking for completions.
            if (skippedTrivia.Elements.Length != 1 ||
                skippedTrivia.Elements[0] is not Token token)
            {
                return null;
            }

            switch (token.Type)
            {
                case TokenType.Identifier:
                    return token.Text;

                case TokenType.StringComplete:
                    if (!token.Text.EndsWith('\''))
                    {
                        // An unterminated string will result in skipped trivia containing an unterminated token.
                        // Compensate here by building the expected token before lexing it.
                        token = SyntaxFactory.CreateFreeformToken(token.Type, $"{token.Text}'");
                    }

                    return Lexer.TryGetStringValue(token);

                default:
                    return null;
            }
        }

        private static string? TryGetEnteredTextFromStringOrSkipped(SyntaxBase syntax)
            => syntax switch
            {
                StringTypeLiteralSyntax s when s.SegmentValues.Length == 1 => s.SegmentValues[0],
                StringSyntax s => s.TryGetLiteralValue(),
                SkippedTriviaSyntax s => TryGetSkippedTokenText(s),
                _ => null,
            };

        private static IEnumerable<CompletionItem> GetResourceTypeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ResourceType))
            {
                return [];
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

                    foreach (var resourceType in group.Where(rt => rt.ApiVersion is not null).OrderByDescending(rt => rt.ApiVersion ?? "", ApiVersionComparer.Instance))
                    {
                        items.Add(CreateResourceTypeSegmentCompletion(resourceType, index++, context.ReplacementRange, includeApiVersion: true, displayApiVersion: resourceType.ApiVersion));
                    }
                }

                return items;
            }

            return GetResourceTypeCompletions(model, context, context.EnclosingDeclaration switch
            {
                ResourceDeclarationSyntax resourceSyntax => resourceSyntax.Type,
                ParameterDeclarationSyntax parameterSyntax when parameterSyntax.Type is ResourceTypeSyntax resourceType => resourceType.Type,
                OutputDeclarationSyntax outputSyntax when outputSyntax.Type is ResourceTypeSyntax resourceType => resourceType.Type,
                _ => null,
            });
        }

        private static IEnumerable<CompletionItem> GetResourceTypeCompletions(SemanticModel model, BicepCompletionContext context, SyntaxBase? resourceTypeIdentifierSyntax)
        {
            var fullyQualifiedResourceType = resourceTypeIdentifierSyntax is not null &&
                TryGetEnteredTextFromStringOrSkipped(resourceTypeIdentifierSyntax) is string entered &&
                ResourceTypeReference.HasResourceTypePrefix(entered)
                    ? entered
                    : null;

            // ResourceType completions are divided into 2 parts.
            // If the current value passes the namespace and type notation ("<Namespace>/<type>") format, we return the fully qualified resource types
            if (fullyQualifiedResourceType is not null)
            {
                var resourceType = fullyQualifiedResourceType.Split('@')[0];

                // newest api versions should be shown first
                // strict filtering on type so that we show api versions for only the selected type
                return model.Binder.NamespaceResolver.GetGroupedResourceTypes()[resourceType]
                    .SelectMany(x => x)
                    .OrderByDescending(rt => rt.ApiVersion ?? "", ApiVersionComparer.Instance)
                    .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange, showApiVersion: true));
            }

            // if we do not have the namespace and type notation, we only return unique resource types without their api-versions
            // we need to ensure that Microsoft.Compute/virtualMachines comes before Microsoft.Compute/virtualMachines/extensions
            // we still order by apiVersion first to have consistent indexes
            return model.Binder.NamespaceResolver.GetGroupedResourceTypes()
                .Select(rt => rt.SelectMany(x => x).OrderByDescending(rt => rt.ApiVersion ?? "", ApiVersionComparer.Instance).First())
                .OrderBy(rt => rt.Type, StringComparer.OrdinalIgnoreCase)
                .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange, showApiVersion: false));
        }

        private static IEnumerable<CompletionItem> GetResourceTypeFollowerCompletions(BicepCompletionContext context)
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
            IOUri BicepFileParentUri,
            IOUri EnteredParentUri,
            bool ShowCwdPrefix,
            ImmutableArray<IFileHandle> Files,
            ImmutableArray<IDirectoryHandle> Directories);

        private static FileCompletionInfo? TryGetFilesForPathCompletions(IFileHandle currentFileHandle, string entered)
        {
            try
            {
                var files = new List<IFileHandle>();
                var dirs = new List<IDirectoryHandle>();

                var currentDirectory = currentFileHandle.GetParent();
                var searchDirectory = currentDirectory.GetDirectory(entered);

                if (!searchDirectory.Exists() && !string.IsNullOrEmpty(entered))
                {
                    searchDirectory = currentDirectory.GetFile(entered).GetParent();
                }

                if (searchDirectory.Exists())
                {
                    files = [.. searchDirectory.EnumerateFiles()];
                    dirs = [.. searchDirectory.EnumerateDirectories()];

                    // include the parent folder as a completion if we're not at the file system root
                    if (searchDirectory.GetParent() is { } parentSearchDirectory)
                    {
                        dirs.Add(parentSearchDirectory);
                    }
                }

                return new(
                    BicepFileParentUri: currentDirectory.Uri,
                    EnteredParentUri: searchDirectory.Uri,
                    ShowCwdPrefix: entered.StartsWith("./"),
                    Files: [.. files],
                    Directories: [.. dirs]);
            }
            catch (IOException)
            {
                return null;
            }
        }

        private static IEnumerable<CompletionItem> CreateFileCompletionItems(IFileHandle mainFileHandle, Range replacementRange, FileCompletionInfo info, Predicate<IFileHandle> predicate, CompletionPriority priority)
        {
            foreach (var fileHandle in info.Files)
            {
                if (fileHandle.Uri == mainFileHandle.Uri || !predicate(fileHandle))
                {
                    continue;
                }

                var completionName = fileHandle.Uri.GetPathRelativeTo(info.EnteredParentUri);
                var completionValue = fileHandle.Uri.GetPathRelativeTo(info.BicepFileParentUri);
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

        private static IEnumerable<CompletionItem> CreateDirectoryCompletionItems(Range replacementRange, FileCompletionInfo info, CompletionPriority priority = CompletionPriority.Low)
        {
            foreach (var dir in info.Directories)
            {
                var completionName = dir.Uri.GetPathRelativeTo(info.EnteredParentUri).ToString();
                var completionValue = dir.Uri.GetPathRelativeTo(info.BicepFileParentUri).ToString();
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
                .WithFollowupCompletion("file path completion")
                .Build();
            }
        }

        private IEnumerable<CompletionItem> GetLocalModulePathCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ModulePath) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.TestPath) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.UsingFilePath) &&
                !context.Kind.HasFlag(BicepCompletionContextKind.ExtendsFilePath))
            {
                return [];
            }

            if (IsOciArtifactRegistryReference(context))
            {
                return [];
            }

            // To provide intellisense before the quotes are typed
            if (context.EnclosingDeclaration is not IArtifactReferenceSyntax artifactReference
                || artifactReference.Path is not StringSyntax stringSyntax
                || stringSyntax.TryGetLiteralValue() is not string entered)
            {
                entered = "";
            }

            try
            {
                // These should only fail if we're not able to resolve cwd path or the entered string
                if (TryGetFilesForPathCompletions(model.SourceFile.FileHandle, entered) is not { } fileCompletionInfo)
                {
                    return [];
                }

                var replacementSyntax = (context.EnclosingDeclaration as IArtifactReferenceSyntax)?.Path;
                var replacementRange = replacementSyntax?.ToRange(model.SourceFile.LineStarts) ?? context.ReplacementRange;

                var dirItems = CreateDirectoryCompletionItems(replacementRange, fileCompletionInfo);

                if (context.Kind.HasFlag(BicepCompletionContextKind.ExtendsFilePath))
                {
                    var bicepParamFileItems = CreateFileCompletionItems(model.SourceFile.FileHandle, replacementRange, fileCompletionInfo, x => x.IsBicepParamFile(), CompletionPriority.High);

                    return bicepParamFileItems.Concat(dirItems);
                }

                // Prioritize .bicep files higher than other files.
                var bicepFileItems = CreateFileCompletionItems(model.SourceFile.FileHandle, replacementRange, fileCompletionInfo, x => x.IsBicepFile(), CompletionPriority.High);
                var armTemplateFileItems = CreateFileCompletionItems(model.SourceFile.FileHandle, replacementRange, fileCompletionInfo, IsArmTemplateFile, CompletionPriority.Medium);

                if (model.Features.ExtendableParamFilesEnabled && context.Kind.HasFlag(BicepCompletionContextKind.UsingFilePath))
                {
                    var item = CompletionItemBuilder.Create(CompletionItemKind.Enum, LanguageConstants.NoneKeyword)
                                                    .WithFilterText(LanguageConstants.NoneKeyword)
                                                    .WithSortText(GetSortText(LanguageConstants.NoneKeyword, CompletionPriority.Medium))
                                                    .WithPlainTextEdit(replacementRange, LanguageConstants.NoneKeyword)
                                                    .Build();

                    bicepFileItems = bicepFileItems.Prepend(item);
                }

                return bicepFileItems.Concat(armTemplateFileItems).Concat(dirItems);
            }
            catch (DirectoryNotFoundException)
            {
                return [];
            }

            // Local functions.
            bool IsArmTemplateFile(IFileHandle fileHandle)
            {
                if (!fileHandle.IsArmTemplateLikeFile())
                {
                    return false;
                }

                if (fileHandle.Uri.HasExtension(LanguageConstants.ArmTemplateFileExtension))
                {
                    return true;
                }

                if (model.SourceFileGrouping.SourceFiles.Any(sourceFile =>
                        sourceFile is ArmTemplateFile &&
                        sourceFile.FileHandle.Uri == fileHandle.Uri))
                {
                    return true;
                }

                if (fileHandle.TryPeekText(2000, Encoding.UTF8).IsSuccess(out var fileContents) &&
                    LanguageConstants.ArmTemplateSchemaRegex.IsMatch(fileContents))
                {
                    return true;
                }

                return false;
            }
        }

        private static bool IsOciArtifactRegistryReference(BicepCompletionContext context)
        {
            return context.ReplacementTarget is Token token &&
                token.Text is string text &&
                (ModuleRegistryWithoutAliasPattern().IsMatch(text) || ModuleRegistryWithAliasPattern().IsMatch(text));
        }

        private static IEnumerable<CompletionItem> GetParameterTypeSnippets(SemanticModel model, BicepCompletionContext context)
        {
            if (context.EnclosingDeclaration is ParameterDeclarationSyntax parameterDeclarationSyntax)
            {
                Range enclosingDeclarationRange = parameterDeclarationSyntax.Keyword.ToRange(model.SourceFile.LineStarts);
                TextEdit textEdit = new()
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

                yield return CreateContextualSnippetCompletion("securestring",
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
                return [];
            }

            var declaredType = model.GetDeclaredType(parameter);

            return GetValueCompletionsForType(model, context, declaredType, (parameter.Modifier as ParameterDefaultValueSyntax)?.DefaultValue, loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetVariableValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.VariableValue) || context.EnclosingDeclaration is not VariableDeclarationSyntax variable)
            {
                return [];
            }

            if (model.GetDeclaredType(variable) is { } declaredType)
            {
                return GetValueCompletionsForType(model, context, declaredType, variable.Value, loopsAllowed: true);
            }

            // we don't know what the variable type is, so assume "any"
            return CreateLoopCompletions(context.ReplacementRange, LanguageConstants.Any, filtersAllowed: false);
        }

        private IEnumerable<CompletionItem> GetOutputValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.OutputValue) || context.EnclosingDeclaration is not OutputDeclarationSyntax output)
            {
                return [];
            }

            var declaredType = model.GetDeclaredType(output);

            return GetValueCompletionsForType(model, context, declaredType, output.Value, loopsAllowed: true);
        }

        private static IEnumerable<CompletionItem> GetOutputTypeFollowerCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.OutputTypeFollower))
            {
                const string equals = "=";
                yield return CreateOperatorCompletion(equals, context.ReplacementRange, preselect: true);
            }
        }

        private static IEnumerable<CompletionItem> GetVariableNameFollowerCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.VariableNameFollower))
            {
                const string equals = "=";
                yield return CreateOperatorCompletion(equals, context.ReplacementRange, preselect: true);
            }
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
                var snippets = snippetsProvider.GetResourceBodyCompletionSnippets(resourceType, resourceDeclarationSyntax.IsExistingResource(), isResourceNested);

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
            IEnumerable<Snippet> snippets = snippetsProvider.GetModuleBodyCompletionSnippets(typeSymbol.UnwrapArrayType());

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

        private IEnumerable<CompletionItem> CreateTestBodyCompletions(SemanticModel model, BicepCompletionContext context, TestDeclarationSyntax testDeclarationSyntax)
        {
            TypeSymbol typeSymbol = model.GetTypeInfo(testDeclarationSyntax);
            IEnumerable<Snippet> snippets = snippetsProvider.GetTestBodyCompletionSnippets(typeSymbol.UnwrapArrayType());

            foreach (Snippet snippet in snippets)
            {
                string prefix = snippet.Prefix;
                BicepTelemetryEvent telemetryEvent = BicepTelemetryEvent.CreateTestBodySnippetInsertion(prefix);
                var command = TelemetryHelper.CreateCommand
                (
                    title: "test body completion snippet",
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

        private IEnumerable<CompletionItem> GetAssertValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.AssertValue) || context.EnclosingDeclaration is not AssertDeclarationSyntax assert)
            {
                return [];
            }

            return GetValueCompletionsForType(model, context, LanguageConstants.Bool, assert.Value, loopsAllowed: false);
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

        private IEnumerable<CompletionItem> GetTestBodyCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.TestBody) && context.EnclosingDeclaration is TestDeclarationSyntax testDeclarationSyntax)
            {
                foreach (CompletionItem completionItem in CreateTestBodyCompletions(model, context, testDeclarationSyntax))
                {
                    yield return completionItem;
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

        private static CompletionPriority GetContextualCompletionPriority(Symbol symbol, SemanticModel model, BicepCompletionContext context, Symbol? enclosingDeclarationSymbol)
        {
            // The value type of resource/module.dependsOn items can only be a resource or module symbol so prioritize them higher than anything else.
            // Expressions can also be accepted in this context so other completion items will still be available, just lower in the list.
            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectsResourceSymbolicReference)
                && symbol is ResourceSymbol or ModuleSymbol)
            {
                // parent resource symbols of the current resource should not be prioritized but are still provided for use in expressions
                var enclosingResourceMetadata = model.DeclaredResources.FirstOrDefault((drm) => drm.Symbol == enclosingDeclarationSymbol);
                if (enclosingResourceMetadata != null
                    && model.ResourceAncestors.GetAncestors(enclosingResourceMetadata).Any(ra => ra.Resource.Symbol == symbol))
                {
                    return CompletionPriority.Medium;
                }

                return CompletionPriority.VeryHigh;
            }

            return GetCompletionPriority(symbol.Kind);
        }

        private static bool ShouldSymbolBeIncludedInCompletion(Symbol symbol, SemanticModel model, BicepCompletionContext context, Symbol? enclosingDeclarationSymbol)
        {
            // filter out self references
            if (enclosingDeclarationSymbol != null && ReferenceEquals(symbol, enclosingDeclarationSymbol))
            {
                return false;
            }

            // For nested resource/module symbol completions, don't suggest child symbols for resource.dependsOn symbol completions.
            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectsResourceSymbolicReference) && symbol is ResourceSymbol or ModuleSymbol or TestSymbol)
            {
                // filter out child resource symbols of the enclosing declaration symbol
                var symbolResourceMetadata = model.DeclaredResources.FirstOrDefault((drm) => drm.Symbol == symbol);
                if (symbolResourceMetadata != null
                    && model.ResourceAncestors.GetAncestors(symbolResourceMetadata).Any(ra => ra.Resource.Symbol == enclosingDeclarationSymbol))
                {
                    return false;
                }
            }

            return true;
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
                    if (!result.ContainsKey(symbol.Name) && ShouldSymbolBeIncludedInCompletion(symbol, model, context, enclosingDeclarationSymbol))
                    {
                        // the symbol satisfies the following conditions:
                        // - we have not added a symbol with the same name (avoids duplicate completions)
                        // - the symbol is different than the enclosing declaration (avoids suggesting cycles)
                        // - the symbol name is different than the name of the enclosing declaration (avoids suggesting a duplicate identifier)
                        var priority = GetContextualCompletionPriority(symbol, model, context, enclosingDeclarationSymbol);
                        result.Add(symbol.Name, CreateSymbolCompletion(symbol, context.ReplacementRange, priority: priority, model: model));
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

                result = GetAccessibleDecoratorFunctions(namespaceType,
                    context.EnclosingDecorable is null ? null : model.GetDeclaredType(context.EnclosingDecorable),
                    enclosingDeclarationSymbol);
                accessibleDecoratorFunctionsCache.Add(namespaceType, result);

                return result;
            }

            var nsTypeDict = GetNamespaceTypeBySymbol(model);

            if (!context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName))
            {
                // add namespaces first
                AddSymbolCompletions(completions, nsTypeDict.Keys);
                Func<DeclaredSymbol, bool> symbolFilter = _ => true;

                // add accessible symbols from innermost scope and then move to outer scopes
                // reverse loop iteration
                foreach (var scope in context.ActiveScopes.Reverse())
                {
                    // add referenceable declarations with valid identifiers at current scope
                    AddSymbolCompletions(completions, scope.Declarations
                        .Where(symbolFilter)
                        .Where(decl => decl.NameSource.IsValid && decl.CanBeReferenced()));

                    if (scope.ScopeResolution == ScopeResolution.GlobalsOnly)
                    {
                        // don't inherit outer scope variables
                        break;
                    }

                    if (scope.ScopeResolution == ScopeResolution.InheritFunctionsAndVariablesOnly)
                    {
                        symbolFilter = symbol => symbol is VariableSymbol or ImportedVariableSymbol or DeclaredFunctionSymbol or ImportedFunctionSymbol or WildcardImportSymbol;
                    }
                }
            }
            else
            {
                // Only add the namespaces that contain accessible decorator function symbols.
                AddSymbolCompletions(completions, nsTypeDict.Keys.Where(
                    ns => GetAccessibleDecoratorFunctionsWithCache(nsTypeDict[ns]).Any()));

                // Record the names of referenceable declarations which will be used to check name clashes later.
                declaredNames.UnionWith(model.Root.Declarations.Where(decl => decl.NameSource.IsValid && decl.CanBeReferenced()).Select(decl => decl.Name));
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
                        completions.Add(fullyQualifiedFunctionName, CreateSymbolCompletion(function, context.ReplacementRange, model, insertText: fullyQualifiedFunctionName));
                    }
                    else
                    {
                        // function does not have to be fully qualified
                        completions.Add(function.Name, CreateSymbolCompletion(function, context.ReplacementRange, model));
                    }
                }
            }

            return completions.Values;
        }

        private static IEnumerable<FunctionSymbol> GetAccessibleDecoratorFunctions(NamespaceType namespaceType, TypeSymbol? targetType, Symbol? enclosingDeclarationSymbol)
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
                TypeAliasSymbol declaredTypeSymbol when targetType is not null => GetAccessible(knownDecoratorFunctions, (targetType as TypeType)?.Unwrapped ?? targetType, FunctionFlags.TypeDecorator),
                VariableSymbol variableSymbol => GetAccessible(knownDecoratorFunctions, variableSymbol.Type, FunctionFlags.VariableDecorator),
                DeclaredFunctionSymbol functionSymbol => GetAccessible(knownDecoratorFunctions, functionSymbol.Type, FunctionFlags.FunctionDecorator),
                ResourceSymbol resourceSymbol => GetAccessible(knownDecoratorFunctions, resourceSymbol.Type, FunctionFlags.ResourceDecorator),
                ModuleSymbol moduleSymbol => GetAccessible(knownDecoratorFunctions, moduleSymbol.Type, FunctionFlags.ModuleDecorator),
                OutputSymbol outputSymbol => GetAccessible(knownDecoratorFunctions, outputSymbol.Type, FunctionFlags.OutputDecorator),
                /*
                 * The decorator is dangling if enclosingDeclarationSymbol is null. Return all decorator factory functions since
                 * we don't know which kind of declaration it will attach to.
                 */
                null => knownDecoratorFunctions,
                _ => []
            };
        }

        private static IEnumerable<CompletionItem> GetMemberAccessCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.MemberAccess) || context.PropertyAccess == null)
            {
                return [];
            }

            var declaredType = compilation.GetEntrypointSemanticModel().GetDeclaredType(context.PropertyAccess.BaseExpression);
            var model = compilation.GetEntrypointSemanticModel();

            if (context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName) && declaredType is NamespaceType namespaceType)
            {
                var enclosingDeclarationSymbol = context.EnclosingDeclaration is null ? null : model.GetSymbolInfo(context.EnclosingDeclaration);
                var decoratorTargetType = context.EnclosingDecorable is null ? null : model.GetDeclaredType(context.EnclosingDecorable);

                return GetAccessibleDecoratorFunctions(namespaceType, decoratorTargetType, enclosingDeclarationSymbol)
                    .Select(symbol => CreateSymbolCompletion(symbol, context.ReplacementRange, model));
            }

            if (declaredType is not null && TypeHelper.TryRemoveNullability(declaredType) is TypeSymbol nonNullable)
            {
                declaredType = nonNullable;
            }

            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => CreatePropertyAccessCompletion(p,
                    compilation.SourceFileGrouping.EntryPoint,
                    context.PropertyAccess.IsSafeAccess,
                    context.PropertyAccess.Dot,
                    context.ReplacementRange))
                .Concat(GetMethods(declaredType)
                .Select(m => CreateSymbolCompletion(m, context.ReplacementRange, model)));
        }

        private static IEnumerable<CompletionItem> GetTypeMemberAccessCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.TypeMemberAccess) || context.TypePropertyAccess is null)
            {
                return [];
            }

            var declaredType = compilation.GetEntrypointSemanticModel().GetDeclaredType(context.TypePropertyAccess.BaseExpression);

            if (declaredType is not null && TypeHelper.TryRemoveNullability(declaredType) is TypeSymbol nonNullable)
            {
                declaredType = nonNullable;
            }

            if (declaredType is TypeType typeType)
            {
                declaredType = typeType.Unwrapped;
            }

            var completions = GetProperties(declaredType)
                .Select(p => CreatePropertyAccessCompletion(p, compilation.SourceFileGrouping.EntryPoint, isSafeAccess: false, context.TypePropertyAccess.Dot, context.ReplacementRange));

            if (GetAdditionalPropertiesType(declaredType) is not null)
            {
                completions = completions.Append(CompletionItemBuilder.Create(CompletionItemKind.Property, "*")
                    .WithPlainTextEdit(context.ReplacementRange, "*")
                    .WithCommitCharacters(PropertyAccessCommitChars)
                    .WithSortText(GetSortText("*", CompletionPriority.Low))
                    .Build());
            }

            return completions;
        }

        private static IEnumerable<CompletionItem> GetResourceAccessCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ResourceAccess) || context.ResourceAccess == null)
            {
                return [];
            }

            var symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(context.ResourceAccess.BaseExpression) as ResourceSymbol;
            if (symbol == null)
            {
                return [];
            }
            var model = compilation.GetEntrypointSemanticModel();

            // Find child resources
            var children = symbol.DeclaringResource.TryGetBody()?.Resources ?? [];
            return children
                .Select(r => new { resource = r, symbol = compilation.GetEntrypointSemanticModel().GetSymbolInfo(r) as ResourceSymbol, })
                .Where(entry => entry.symbol != null)
                .Select(entry => CreateSymbolCompletion(entry.symbol!, context.ReplacementRange, model));
        }

        private static IEnumerable<CompletionItem> GetArrayIndexCompletions(Compilation compilation, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayIndex) || context.ArrayAccess == null)
            {
                return [];
            }

            var declaredType = compilation.GetEntrypointSemanticModel().GetDeclaredType(context.ArrayAccess.BaseExpression);

            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.WriteOnly))
                .Select(p => CreatePropertyIndexCompletion(p, context.ReplacementRange, CompletionPriority.High));
        }

        private static IEnumerable<CompletionItem> GetObjectPropertyNameCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ObjectPropertyName) == false || context.Object == null)
            {
                return [];
            }

            // in order to provide completions for property names,
            // we need to establish the type of the object first
            var declaredType = model.GetDeclaredType(context.Object);
            if (declaredType == null)
            {
                return [];
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

        private static IEnumerable<NamedTypeProperty> GetProperties(TypeSymbol? type)
        {
            return (type switch
            {
                ResourceType resourceType => GetProperties(resourceType.Body.Type),
                ModuleType moduleType => GetProperties(moduleType.Body.Type),
                TestType testType => GetProperties(testType.Body.Type),
                ObjectType objectType => objectType.Properties.Values,
                DiscriminatedObjectType discriminated => discriminated.DiscriminatorProperty.AsEnumerable(),
                UnionType unionType => GetPropertiesFromUnionType(unionType),
                _ => [],
            }).Where(p => !p.Flags.HasFlag(TypePropertyFlags.FallbackProperty));
        }

        private static IEnumerable<NamedTypeProperty> GetPropertiesFromUnionType(UnionType unionType)
        {
            var potentiallyCollapsedType = TypeHelper.TryCollapseTypes(unionType.Members);
            if (potentiallyCollapsedType is UnionType)
            {
                // type collapsed into a new or same union type (may have collapsed into itself)
                // get properties from each union members
                return unionType.Members.SelectMany(member => GetProperties(member.Type));
            }

            return GetProperties(potentiallyCollapsedType);
        }

        private static TypeSymbol? GetAdditionalPropertiesType(TypeSymbol? type) => type switch
        {
            ResourceType resourceType => GetAdditionalPropertiesType(resourceType.Body.Type),
            ModuleType moduleType => GetAdditionalPropertiesType(moduleType.Body.Type),
            TestType testType => GetAdditionalPropertiesType(testType.Body.Type),
            ObjectType objectType when objectType.AdditionalProperties is not null && !objectType.AdditionalProperties.Flags.HasFlag(TypePropertyFlags.FallbackProperty)
                => objectType.AdditionalProperties.TypeReference.Type,
            _ => null,
        };

        private static IEnumerable<FunctionSymbol> GetMethods(TypeSymbol? type) => type switch
        {
            ObjectType objectType => objectType.MethodResolver.GetKnownFunctions().Values,
            ResourceType resourceType => GetMethods(resourceType.Body.Type),
            _ => [],
        };

        private static DeclaredTypeAssignment? GetDeclaredTypeAssignment(SemanticModel model, SyntaxBase? syntax) => syntax == null
            ? null
            : model.GetDeclaredTypeAssignment(syntax);

        private IEnumerable<CompletionItem> GetPropertyValueCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.PropertyValue))
            {
                return [];
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Property);
            if (declaredTypeAssignment == null)
            {
                return [];
            }

            // Check if the property name is "identity" and the parent is a resource or module
            // Use a single identity snippet provider for both resources and modules
            IEnumerable<CompletionItem> snippetCompletions = [];
            if (context.Property is { } propertySyntax &&
                propertySyntax.TryGetKeyText() is "identity" &&
                (context.EnclosingDeclaration is ResourceDeclarationSyntax or ModuleDeclarationSyntax))
            {
                snippetCompletions = snippetsProvider.GetIdentitySnippets(context.EnclosingDeclaration is ResourceDeclarationSyntax)
                    .Select(snippet => CreateContextualSnippetCompletion(
                        snippet.Prefix,
                        snippet.Detail,
                        snippet.Text,
                        context.ReplacementRange,
                        snippet.CompletionPriority,
                        preselect: true));
            }

            var loopsAllowed = context.Property is not null && ForSyntaxValidatorVisitor.IsAddingPropertyLoopAllowed(model, context.Property);
            return GetValueCompletionsForType(model, context, declaredTypeAssignment.Reference.Type, context.Property?.Value, loopsAllowed).Concat(snippetCompletions);
        }

        private IEnumerable<CompletionItem> GetArrayItemCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayItem) || context.Array?.Syntax is not ArraySyntax arraySyntax)
            {
                return [];
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, arraySyntax);
            if (declaredTypeAssignment?.Reference.Type is not ArrayType arrayType)
            {
                return [];
            }

            // Special case: there is a distinction in the type system that needs to be made for resource.dependsOn resource collections
            // which are resources defined by a loop vs arrays. For now, check if type is the specific type of resource.dependsOn
            // don't provide value completions as the resource collections completions are handled by GetSymbolCompletions.
            if (ReferenceEquals(arrayType.Item.Type, LanguageConstants.ResourceOrResourceCollectionRefItem))
            {
                return [];
            }

            return GetValueCompletionsForType(model, context, arrayType.Item.Type, arraySyntax.Items.Skip(context.Array.ArgumentIndex).FirstOrDefault(), loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetFunctionParamCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.FunctionArgument)
                || context.FunctionArgument is not { } functionArgument
                || model.GetSymbolInfo(functionArgument.Syntax) is not IFunctionSymbol functionSymbol)
            {
                return [];
            }

            var argType = functionSymbol.GetDeclaredArgumentType(functionArgument.ArgumentIndex);

            return GetValueCompletionsForType(model, context, argType, functionArgument.Syntax.Arguments.Skip(functionArgument.ArgumentIndex).FirstOrDefault(), loopsAllowed: false);
        }

        private IEnumerable<CompletionItem> GetTypeArgumentCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.TypeArgument) ||
                context.TypeArgument is not { } typeArgument ||
                GetSymbolType(model.GetSymbolInfo(typeArgument.Syntax)) is not TypeTemplate typeTemplate ||
                typeTemplate.Parameters.Length <= typeArgument.ArgumentIndex)
            {
                return [];
            }

            return typeTemplate.Parameters[typeArgument.ArgumentIndex].Type is TypeSymbol type
                ? GetValueCompletionsForType(model,
                    context,
                    type,
                    typeArgument.Syntax.Arguments.Skip(typeArgument.ArgumentIndex).Select(arg => arg.Expression).FirstOrDefault(),
                    loopsAllowed: false)
                : GetTypeCompletions(model, context);
        }

        private static TypeSymbol? GetSymbolType(Symbol? symbol) => symbol switch
        {
            ITypeReference typeReference => typeReference.Type,
            DeclaredSymbol declared => declared.Type,
            PropertySymbol property => property.Type,
            _ => null,
        };

        private IEnumerable<CompletionItem> GetFileCompletionPaths(SemanticModel model, BicepCompletionContext context, TypeSymbol argType)
        {
            if (context.FunctionArgument is not { } functionArgument ||
                !argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringFilePath) &&
                !argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringDirectoryPath))
            {
                return [];
            }

            //try get entered text. we need to provide path completions when something else than string is entered and in that case we use the token value to get what's currently entered
            //(token value for string will have single quotes, so we need to avoid it)
            var entered = (functionArgument.Syntax.Arguments.ElementAtOrDefault(functionArgument.ArgumentIndex)?.Expression as StringSyntax)?.TryGetLiteralValue() ?? string.Empty;

            // These should only fail if we're not able to resolve cwd path or the entered string
            if (TryGetFilesForPathCompletions(model.SourceFile.FileHandle, entered) is not { } fileCompletionInfo)
            {
                return [];
            }

            if (argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringDirectoryPath))
            {
                return CreateDirectoryCompletionItems(context.ReplacementRange, fileCompletionInfo, CompletionPriority.High);
            }

            IEnumerable<CompletionItem> fileItems;
            if (argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringJsonFilePath))
            {
                // Prioritize .json or .jsonc files higher than other files.
                var jsonItems = CreateFileCompletionItems(model.SourceFile.FileHandle, context.ReplacementRange, fileCompletionInfo, file => file.Uri.HasExtension(LanguageConstants.JsonFileExtension) || file.Uri.HasExtension(LanguageConstants.JsoncFileExtension), CompletionPriority.High);
                var nonJsonItems = CreateFileCompletionItems(model.SourceFile.FileHandle, context.ReplacementRange, fileCompletionInfo, file => !file.Uri.HasExtension(LanguageConstants.JsonFileExtension) && !file.Uri.HasExtension(LanguageConstants.JsoncFileExtension), CompletionPriority.Medium);
                fileItems = jsonItems.Concat(nonJsonItems);
            }
            else if (argType.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringYamlFilePath))
            {
                // Prioritize .yaml or .yml files higher than other files.
                var yamlItems = CreateFileCompletionItems(model.SourceFile.FileHandle, context.ReplacementRange, fileCompletionInfo, file => file.Uri.HasExtension("yaml") || file.Uri.HasExtension("yml"), CompletionPriority.High);
                var nonYamlItems = CreateFileCompletionItems(model.SourceFile.FileHandle, context.ReplacementRange, fileCompletionInfo, file => !file.Uri.HasExtension("yaml") && !file.Uri.HasExtension("yml"), CompletionPriority.Medium);
                fileItems = yamlItems.Concat(nonYamlItems);
            }
            else
            {
                fileItems = CreateFileCompletionItems(model.SourceFile.FileHandle, context.ReplacementRange, fileCompletionInfo, (_) => true, CompletionPriority.High);
            }

            var dirItems = CreateDirectoryCompletionItems(context.ReplacementRange, fileCompletionInfo, CompletionPriority.Medium);

            return fileItems.Concat(dirItems);
        }

        private IEnumerable<CompletionItem> GetValueCompletionsForType(SemanticModel model, BicepCompletionContext context, TypeSymbol? type, SyntaxBase? currentValue, bool loopsAllowed)
        {
            var replacementRange = context.ReplacementRange;
            switch (type)
            {
                case BooleanType:
                    yield return CreateKeywordCompletion(LanguageConstants.TrueKeyword, LanguageConstants.TrueKeyword, replacementRange, preselect: true, CompletionPriority.High);
                    yield return CreateKeywordCompletion(LanguageConstants.FalseKeyword, LanguageConstants.FalseKeyword, replacementRange, preselect: true, CompletionPriority.High);

                    break;

                case StringType when type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringFilePath) ||
                    type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsStringDirectoryPath):
                    foreach (var completion in GetFileCompletionPaths(model, context, type))
                    {
                        yield return completion;
                    }
                    break;

                case StringType when type.ValidationFlags.HasFlag(TypeSymbolValidationFlags.IsResourceTypeIdentifier):
                    foreach (var completion in GetResourceTypeCompletions(model, context, currentValue))
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

                case IntegerLiteralType integerLiteral:
                    yield return CompletionItemBuilder.Create(CompletionItemKind.EnumMember, integerLiteral.Name)
                        .WithPlainTextEdit(replacementRange, integerLiteral.Name)
                        .WithDetail(integerLiteral.Name)
                        .Preselect()
                        .WithSortText(GetSortText(integerLiteral.Name, CompletionPriority.Medium))
                        .Build();

                    break;

                case BooleanLiteralType booleanLiteral:
                    yield return CompletionItemBuilder.Create(CompletionItemKind.EnumMember, booleanLiteral.Name)
                        .WithPlainTextEdit(replacementRange, booleanLiteral.Name)
                        .WithDetail(booleanLiteral.Name)
                        .Preselect()
                        .WithSortText(GetSortText(booleanLiteral.Name, CompletionPriority.Medium))
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
                    var aggregatedCompletions = union.Members.SelectMany(typeRef => GetValueCompletionsForType(model, context, typeRef.Type, currentValue, loopsAllowed));
                    foreach (var completion in aggregatedCompletions)
                    {
                        yield return completion;
                    }
                    break;

                case LambdaType lambda:
                    // by default, only offer a completion for fixed lambda arguments
                    var fixedArgLength = lambda.ArgumentTypes.Length;
                    var (snippet, label) = fixedArgLength switch
                    {
                        1 => (
                            "${1:arg} => $0",
                            "arg => ..."),
                        _ => (
                            $"({string.Join(", ", Enumerable.Range(0, fixedArgLength).Select(i => $"${{{i + 1}:arg{i + 1}}}"))}) => $0",
                            $"({string.Join(", ", Enumerable.Range(0, fixedArgLength).Select(i => $"arg{i + 1}"))}) => ..."),
                    };

                    yield return CreateContextualSnippetCompletion(label, label, snippet, replacementRange, CompletionPriority.High);
                    break;
            }
        }

        private IEnumerable<CompletionItem> GetObjectBodyCompletions(TypeSymbol typeSymbol, Range replacementRange)
        {
            IEnumerable<Snippet> snippets = snippetsProvider.GetObjectBodyCompletionSnippets(typeSymbol);

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

        private static IEnumerable<CompletionItem> GetExpressionCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.Expression))
            {
                yield return CreateConditionCompletion(context.ReplacementRange);
            }
        }

        private static IEnumerable<CompletionItem> GetDisableNextLineDiagnosticsDirectiveCompletion(BicepCompletionContext context)
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

            if (nextLineSpan.IsNil)
            {
                return [];
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

        private TextSpan GetNextLineSpan(ImmutableArray<int> lineStarts, int line, ProgramSyntax programSyntax)
        {
            var nextLine = line + 1;
            if (lineStarts.Length > nextLine)
            {
                return TextCoordinateConverter.GetLineSpan(lineStarts, programSyntax.GetEndPosition(), nextLine);
            }

            return TextSpan.Nil;
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

        private static CompletionItem CreatePropertyNameCompletion(NamedTypeProperty property, bool includeColon, Range replacementRange)
        {
            var required = TypeHelper.IsRequired(property);

            var escapedPropertyName = StringUtils.EscapeBicepPropertyName(property.Name);
            var suffix = includeColon ? ":" : string.Empty;
            return CompletionItemBuilder.Create(CompletionItemKind.Property, property.Name)
                // property names that match Bicep keywords or contain non-identifier chars need to be escaped
                .WithPlainTextEdit(replacementRange, $"{escapedPropertyName}{suffix}")
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(property.Name, required ? CompletionPriority.High : CompletionPriority.Medium))
                .Preselect(required)
                .Build();
        }

        private static CompletionItem CreatePropertyIndexCompletion(NamedTypeProperty property, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var escaped = StringUtils.EscapeBicepString(property.Name);
            return CompletionItemBuilder.Create(CompletionItemKind.Property, escaped)
                .WithPlainTextEdit(replacementRange, escaped)
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(escaped, priority))
                .Build();
        }

        private static CompletionItem CreatePropertyAccessCompletion(NamedTypeProperty property, BicepSourceFile tree, bool isSafeAccess, Token dot, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
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
                var edit = new StringBuilder("[");
                if (isSafeAccess)
                {
                    edit.Append('?');
                }
                edit.Append(StringUtils.EscapeBicepString(property.Name)).Append(']');
                item
                    .WithPlainTextEdit(replacementRange, edit.ToString())
                    .WithAdditionalEdits(new TextEditContainer(
                        // remove the dot after the main text edit is applied
                        new TextEdit
                        {
                            NewText = string.Empty,
                            Range = dot.ToRange(tree.LineStarts)
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

        private static CompletionItem CreateTypeCompletion(string typeName, AmbientTypeSymbol type, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var builder = CompletionItemBuilder.Create(CompletionItemKind.Class, typeName)
                .WithPlainTextEdit(replacementRange, typeName)
                .WithDetail(type.Description ?? type.Name)
                .WithSortText(GetSortText(typeName, priority));

            if (type.Type is TypeTemplate typeTemplate)
            {
                var needsQuotes = typeTemplate.Parameters.Length > 0 && typeTemplate.Parameters[0].Type?.IsString() == true;
                var quote = needsQuotes ? "'" : string.Empty;
                builder = builder.WithSnippetEdit(replacementRange, $"{typeName}<{quote}$0{quote}>")
                    // parameterized types always require at least one argument, so automatically request signature help
                    .WithCommand(new Command { Name = EditorCommands.SignatureHelp, Title = "signature help" });
            }
            else
            {
                builder = builder.WithPlainTextEdit(replacementRange, typeName);
            }

            return builder.Build();
        }

        private static CompletionItem CreateDeclaredTypeCompletion(SemanticModel model, TypeAliasSymbol declaredType, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var builder = CompletionItemBuilder.Create(CompletionItemKind.Class, declaredType.Name)
                .WithPlainTextEdit(replacementRange, declaredType.Name)
                .WithDetail(declaredType.Type.Name)
                .WithSortText(GetSortText(declaredType.Name, priority));

            if (DescriptionHelper.TryGetFromDecorator(model, declaredType.DeclaringType) is string documentation)
            {
                builder = builder.WithDocumentation(documentation);
            }

            return builder.Build();
        }

        private static CompletionItem CreateImportedCompletion(ImportedSymbol imported, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var builder = CompletionItemBuilder.Create(CompletionItemKind.Class, imported.Name)
                .WithPlainTextEdit(replacementRange, imported.Name)
                .WithDetail(imported.Type.Name)
                .WithSortText(GetSortText(imported.Name, priority));

            if (imported.Description is string documentation)
            {
                builder = builder.WithDocumentation(documentation);
            }

            return builder.Build();
        }

        private static CompletionItem CreateWildcardPropertyCompletion(WildcardImportSymbol wildcardImport, ExportMetadata exportMetadata, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            StringBuilder replacementBuilder = new(wildcardImport.Name);
            if (Lexer.IsValidIdentifier(exportMetadata.Name))
            {
                replacementBuilder.Append('.').Append(exportMetadata.Name);
            }
            else
            {
                replacementBuilder.Append("['").Append(exportMetadata.Name.Replace("'", "\\'")).Append("']");
            }

            var replacement = replacementBuilder.ToString();
            var builder = CompletionItemBuilder.Create(CompletionItemKind.Class, replacement)
                .WithPlainTextEdit(replacementRange, replacement)
                .WithDetail(exportMetadata.TypeReference.Type.Name)
                .WithSortText(GetSortText(replacement, priority));

            if (exportMetadata.Description is string documentation)
            {
                builder = builder.WithDocumentation(documentation);
            }

            return builder.Build();
        }

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
                var insertText = StringUtils.EscapeBicepString(resourceType.Name);
                return CompletionItemBuilder.Create(CompletionItemKind.Class, resourceType.ApiVersion)
                    // Lower-case all resource types in filter text otherwise editor may prefer those with casing that match what the user has already typed (#9168)
                    .WithFilterText(insertText.ToLowerInvariant())
                    .WithPlainTextEdit(replacementRange, insertText)
                    .WithDocumentation(
                        MarkdownHelper.AppendNewline($"Type: `{resourceType.Type}`") +
                        MarkdownHelper.AppendNewline($"API Version: `{resourceType.ApiVersion}`"))
                    .WithSortText(index.ToString("x8"))
                    .Build();
            }
            else
            {
                var insertText = StringUtils.EscapeBicepString(resourceType.Type);
                return CompletionItemBuilder.Create(CompletionItemKind.Class, insertText)
                    .WithSnippetEdit(replacementRange, $"{insertText[..^1]}@$0'")
                    .WithDocumentation(
                        MarkdownHelper.AppendNewline($"Type: `{resourceType.Type}`"))
                    .WithFollowupCompletion("resource type completion")
                    .WithSortText(index.ToString("x8"))
                    .WithFilterText(ResourceTypeSearchKeywords.TryGetResourceTypeFilterText(resourceType, surroundWithSingleQuotes: true))
                    .Build();
            }
        }

        private static CompletionItem CreateResourceTypeSegmentCompletion(ResourceTypeReference resourceType, int index, Range replacementRange, bool includeApiVersion, string? displayApiVersion)
        {
            // We create one completion with and without the API version.
            var insertText = includeApiVersion && (resourceType.ApiVersion is not null) ?
                StringUtils.EscapeBicepString($"{resourceType.TypeSegments[^1]}@{resourceType.ApiVersion}") :
                StringUtils.EscapeBicepString($"{resourceType.TypeSegments[^1]}");

            return CompletionItemBuilder.Create(CompletionItemKind.Class, insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .WithDocumentation(
                    MarkdownHelper.AppendNewline($"Type: `{resourceType.Type}`") +
                    MarkdownHelper.AppendNewline(displayApiVersion is not null ? $"API Version: `{displayApiVersion}`" : null))
                // 8 hex digits is probably overkill :)
                .WithSortText(index.ToString("x8"))
                .Build();
        }

        private static CompletionItemBuilder CreateFilePathCompletionBuilder(string name, string path, Range replacementRange, CompletionItemKind completionItemKind, CompletionPriority priority)
        {
            path = StringUtils.EscapeBicepString(path);
            var item = CompletionItemBuilder.Create(completionItemKind, name)
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
                .WithDocumentation(MarkdownHelper.CodeBlock(new Snippet(snippet).FormatDocumentation()))
                .WithSortText(GetSortText(label, priority))
                .Preselect(preselect)
                .Build();

        /// <summary>
        /// Creates a completion with a contextual snippet with command option. This will look like a snippet to the user.
        /// </summary>
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, Command command, CompletionPriority priority = CompletionPriority.Medium, bool preselect = false, string? filterText = null) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet, label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithCommand(command)
                .WithDetail(detail)
                .WithDocumentation(MarkdownHelper.CodeBlock(new Snippet(snippet).FormatDocumentation()))
                .WithSortText(GetSortText(label, priority))
                .WithFilterText(filterText)
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
                .WithDocumentation(MarkdownHelper.CodeBlock(new Snippet(snippet).FormatDocumentation()))
                .WithSortText(GetSortText(label, priority))
                .Build();

        private static CompletionItem CreateSymbolCompletion(Symbol symbol, Range replacementRange, SemanticModel model, string? insertText = null, CompletionPriority? priority = null)
        {
            insertText ??= symbol.Name;
            var kind = GetCompletionItemKind(symbol.Kind);
            var completion = CompletionItemBuilder.Create(kind, insertText);

            priority ??= GetCompletionPriority(symbol.Kind);
            completion.WithSortText(GetSortText(insertText, priority.Value));

            if (symbol is ResourceSymbol)
            {
                // treat : as a commit character for the resource access operator case
                completion.WithCommitCharacters(ResourceSymbolCommitChars);
            }

            if (symbol is IFunctionSymbol function)
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
                    .WithFollowupCompletion("symbol completion")
                    .Build();
            }

            if (TryGetSymbolDocumentationMarkdown(symbol, model) is { } documentation)
            {
                completion.WithDocumentation(documentation);
            }

            return completion
                .WithDetail(insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .Build();
        }

        private IEnumerable<CompletionItem> GetExtensionCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectingExtensionSpecification))
            {
                var extensionNames = model.Configuration.Extensions.Data.Keys
                    .Concat(SystemNamespaceType.BuiltInName)
                    .ToHashSet();

                foreach (var extensionName in extensionNames.OrderBy(x => x, LanguageConstants.IdentifierComparer))
                {
                    var completionText = extensionName;
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Folder, completionText)
                        .WithSortText(GetSortText(completionText, CompletionPriority.High))
                        .WithDetail(completionText)
                        .WithPlainTextEdit(context.ReplacementRange, completionText)
                        .Build();
                }
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectingExtensionWithOrAsKeyword))
            {
                yield return CreateKeywordCompletion(LanguageConstants.WithKeyword, "With keyword", context.ReplacementRange);
                yield return CreateKeywordCompletion(LanguageConstants.AsKeyword, "As keyword", context.ReplacementRange);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectingExtensionConfig))
            {
                if (context.EnclosingDeclaration is ExtensionDeclarationSyntax importSyntax &&
                    model.GetSymbolInfo(importSyntax) is ExtensionNamespaceSymbol extensionSymbol &&
                    extensionSymbol.TryGetNamespaceType() is { } namespaceType)
                {
                    foreach (var completion in GetValueCompletionsForType(model, context, namespaceType.ConfigurationType, importSyntax.Config, loopsAllowed: false))
                    {
                        yield return completion;
                    }
                }
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectingExtensionAsKeyword))
            {
                yield return CreateKeywordCompletion(LanguageConstants.AsKeyword, "As keyword", context.ReplacementRange);
            }
        }

        private IEnumerable<CompletionItem> GetUsingWithCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!model.Features.DeployCommandsEnabled)
            {
                yield break;
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.UsingFollower))
            {
                yield return CreateKeywordCompletion(LanguageConstants.WithKeyword, "With keyword", context.ReplacementRange);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.UsingWithFollower) &&
                context.EnclosingDeclaration is UsingDeclarationSyntax usingDeclaration &&
                usingDeclaration.WithClause is UsingWithClauseSyntax usingWithClause)
            {
                var configType = model.GetDeclaredType(usingWithClause);
                foreach (var completion in GetValueCompletionsForType(model, context, configType, usingWithClause.Config, loopsAllowed: false))
                {
                    yield return completion;
                }
            }
        }

        private IEnumerable<CompletionItem> GetCompileTimeImportCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ImportIdentifier))
            {
                yield return CompletionItemBuilder.Create(CompletionItemKind.Value, "{}")
                    .WithSortText(GetSortText("{}", CompletionPriority.High))
                    .WithDetail("Import symbols individually from another template")
                    .WithPlainTextEdit(context.ReplacementRange, "{}")
                    .Build();

                yield return CompletionItemBuilder.Create(CompletionItemKind.Value, "* as")
                    .WithSortText(GetSortText("* as", CompletionPriority.High))
                    .WithDetail("Import all symbols from another template under a new namespace")
                    .WithPlainTextEdit(context.ReplacementRange, "* as")
                    .Build();
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ExpectingImportFromKeyword))
            {
                yield return CreateKeywordCompletion(LanguageConstants.FromKeyword, "From keyword", context.ReplacementRange);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.ImportedSymbolIdentifier))
            {
                if (context.EnclosingDeclaration is CompileTimeImportDeclarationSyntax compileTimeImportDeclaration &&
                    compileTimeImportDeclaration.ImportExpression.Span.ContainsInclusive(context.ReplacementTarget.Span.Position))
                {
                    if (model.TryGetReferencedModel(compileTimeImportDeclaration).IsSuccess(out var importedModel))
                    {
                        var claimedNames = model.Root.Declarations.Select(d => d.Name).ToImmutableHashSet();

                        foreach (var exported in importedModel.Exports)
                        {
                            var edit = exported.Key switch
                            {
                                string key when !Lexer.IsValidIdentifier(key) => $"'{key.Replace("'", @"\'")}' as ",
                                string key when claimedNames.Contains(key) => $"{key} as ",
                                string key => key,
                            };

                            yield return CompletionItemBuilder.Create(CompletionItemKind.Variable, exported.Key)
                                .WithSortText(GetSortText(exported.Key, CompletionPriority.High))
                                .WithDetail(exported.Value.Description)
                                .WithPlainTextEdit(context.ReplacementRange, edit)
                                .Build();
                        }
                    }
                }
            }
        }

        // the priority must be a number in the sort text
        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";

        private static CompletionPriority GetCompletionPriority(SymbolKind symbolKind) =>
            symbolKind switch
            {
                SymbolKind.Function => CompletionPriority.Low,
                SymbolKind.Namespace => CompletionPriority.Low,
                SymbolKind.ImportedNamespace => CompletionPriority.Low,
                _ => CompletionPriority.Medium
            };

        private static CompletionItemKind GetCompletionItemKind(SymbolKind symbolKind) =>
            symbolKind switch
            {
                SymbolKind.Function => CompletionItemKind.Function,
                SymbolKind.File => CompletionItemKind.File,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Namespace => CompletionItemKind.Folder,
                SymbolKind.ImportedNamespace => CompletionItemKind.Folder,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.ParameterAssignment => CompletionItemKind.Field,
                SymbolKind.TypeAlias => CompletionItemKind.TypeParameter,
                SymbolKind.Resource => CompletionItemKind.Interface,
                SymbolKind.Module => CompletionItemKind.Module,
                SymbolKind.Test => CompletionItemKind.Keyword,
                SymbolKind.Local => CompletionItemKind.Variable,

                _ => CompletionItemKind.Text
            };

        private static bool IsPropertyNameEscapingRequired(NamedTypeProperty property) =>
            StringUtils.IsPropertyNameEscapingRequired(property.Name);

        private static string FormatPropertyDetail(NamedTypeProperty property) =>
            TypeHelper.IsRequired(property)
                ? $"{property.Name} (Required)"
                : property.Name;

        private static string FormatPropertyDocumentation(TypeProperty property)
        {
            var buffer = new StringBuilder();

            buffer.Append(MarkdownHelper.AppendNewline($"Type: `{property.TypeReference.Type}`"));

            if (property.Flags.HasFlag(TypePropertyFlags.ReadOnly))
            {
                // this case will be used for dot property access completions
                // this flag is not possible in property name completions
                buffer.Append(MarkdownHelper.AppendNewline($"Read-only property"));
            }

            if (property.Flags.HasFlag(TypePropertyFlags.WriteOnly))
            {
                buffer.Append(MarkdownHelper.AppendNewline($"Write-only property"));
            }

            if (property.Flags.HasFlag(TypePropertyFlags.Constant))
            {
                buffer.Append(MarkdownHelper.AppendNewline($"Requires a compile-time constant value."));
            }

            buffer.Append(MarkdownHelper.AppendNewline(property.Description));

            return buffer.ToString();
        }

        private static string? TryGetSymbolDocumentationMarkdown(Symbol symbol, SemanticModel model)
        {
            if (symbol is DeclaredSymbol declaredSymbol && declaredSymbol.DeclaringSyntax is DecorableSyntax decorableSyntax)
            {
                var buffer = new StringBuilder();

                if (declaredSymbol is ParameterSymbol)
                {
                    buffer.Append(MarkdownHelper.AppendNewline($"Type: `{declaredSymbol.Type}`"));
                }

                var documentation = DescriptionHelper.TryGetFromDecorator(model, decorableSyntax);
                buffer.Append(MarkdownHelper.AppendNewline(documentation));

                return buffer.ToString();
            }

            return null;
        }

        [GeneratedRegex(@"'br:(.*?):?'?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex ModuleRegistryWithoutAliasPattern();

        [GeneratedRegex(@"'br/(.*?):(.*?):?'?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex ModuleRegistryWithAliasPattern();
    }
}
