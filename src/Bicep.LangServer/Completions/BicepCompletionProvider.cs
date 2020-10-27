﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bicep.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Parser;
using Bicep.Core.Resources;
using Bicep.Core.SemanticModel;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using SymbolKind = Bicep.Core.SemanticModel.SymbolKind;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        private const string MarkdownNewLine = "  \n";

        private static readonly Container<string> PropertyCommitChars = new Container<string>(":");

        public IEnumerable<CompletionItem> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context)
        {
            var model = compilation.GetEntrypointSemanticModel();

            return GetDeclarationCompletions(context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(context))
                .Concat(GetObjectPropertyNameCompletions(model, context))
                .Concat(GetPropertyValueCompletions(model, context))
                .Concat(GetArrayItemCompletions(model, context))
                .Concat(GetResourceTypeCompletions(model, context));
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.DeclarationStart))
            {
                yield return CreateKeywordCompletion(LanguageConstants.ParameterKeyword, "Parameter keyword", context.ReplacementRange);
                
                yield return CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration", "param ${1:Identifier} ${2:Type}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default value", "param ${1:Identifier} ${2:Type} = ${3:DefaultValue}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with default and allowed values", @"param ${1:Identifier} ${2:Type} {
  default: $3
  allowed: [
    $4
  ]
}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Parameter declaration with options", @"param ${1:Identifier} ${2:Type} {
  $0
}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ParameterKeyword, "Secure string parameter", @"param ${1:Identifier} string {
  secure: true
}", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.VariableKeyword, "Variable keyword", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.VariableKeyword, "Variable declaration", "var ${1:Identifier} = $0", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  location: $6
  properties: {
    $0
  }
}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource with defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  properties: {
    $0
  }
}", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:Type}@${4:Version}' = {
  name: $5
  $0
}
", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Child Resource without defaults", @"resource ${1:Identifier} 'Microsoft.${2:Provider}/${3:ParentType}/${4:ChildType}@${5:Version}' = {
  name: $6
  $0
}", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.OutputKeyword, "Output keyword", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.OutputKeyword, "Output declaration", "output ${1:Identifier} ${2:Type} = $0", context.ReplacementRange);

                yield return CreateKeywordCompletion(LanguageConstants.ModuleKeyword, "Module keyword", context.ReplacementRange);
                yield return CreateContextualSnippetCompletion(LanguageConstants.ModuleKeyword, "Module declaration", @"module ${1:Identifier} '${2:Path}' = {
  name: $3
  $0
}", context.ReplacementRange);
            }
        }

        private IEnumerable<CompletionItem> GetSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind != BicepCompletionContextKind.None)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // when we're inside an expression that is inside a property that expects a compile-time constant value,
            // we should not be emitting accessible symbol completions
            return GetAccessibleSymbolCompletions(model, context);
        }

        private IEnumerable<CompletionItem> GetDeclarationTypeCompletions(BicepCompletionContext context)
        {
            // local function
            IEnumerable<CompletionItem> GetPrimitiveTypeCompletions() =>
                LanguageConstants.DeclarationTypes.Values.Select(type => CreateTypeCompletion(type, context.ReplacementRange));

            if (context.Kind.HasFlag(BicepCompletionContextKind.ParameterType))
            {
                return GetPrimitiveTypeCompletions().Concat(GetParameterTypeSnippets(context.ReplacementRange));
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.OutputType))
            {
                return GetPrimitiveTypeCompletions();
            }

            return Enumerable.Empty<CompletionItem>();
        }

        private IEnumerable<CompletionItem> GetResourceTypeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind != BicepCompletionContextKind.ResourceType)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            // we need to ensure that Microsoft.Compute/virtualMachines@whatever comes before Microsoft.Compute/virtualMachines/extensions@whatever
            // similarly, newest api versions should be shown first
            return model.ResourceTypeProvider.GetAvailableTypes()
                .OrderBy(rt => rt.FullyQualifiedType, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(rt => rt.ApiVersion)
                .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange))
                .ToList();
        }

        private static IEnumerable<CompletionItem> GetParameterTypeSnippets(Range replacementRange)
        {
            yield return CreateContextualSnippetCompletion("secureObject", "Secure object", @"object {
  secure: true
}", replacementRange);

            yield return CreateContextualSnippetCompletion("secureString", "Secure string", @"string {
  secure: true
}", replacementRange);
        }

        private static IEnumerable<CompletionItem> GetAccessibleSymbolCompletions(SemanticModel model, BicepCompletionContext context)
        {
            // maps insert text to the completion item
            var completions = new Dictionary<string, CompletionItem>();

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
                        // we have not added a symbol with the same name (avoids duplicate completions)
                        // and the symbol is different than the enclosing declaration (avoids suggesting cycles)
                        result.Add(symbol.Name, CreateSymbolCompletion(symbol, context.ReplacementRange));
                    }
                }
            }

            // add namespaces first
            AddSymbolCompletions(completions, model.Root.ImportedNamespaces.Values);

            // add the non-output declarations with valid identifiers 
            AddSymbolCompletions(completions, model.Root.AllDeclarations.Where(decl => decl.NameSyntax.IsValid && !(decl is OutputSymbol)));

            // get names of functions that always require to be fully qualified due to clashes between namespaces
            var alwaysFullyQualifiedNames = model.Root.ImportedNamespaces
                .SelectMany(pair => pair.Value.Descendants.OfType<FunctionSymbol>())
                .GroupBy(func => func.Name, (name, functionSymbols) => (name, count: functionSymbols.Count()), LanguageConstants.IdentifierComparer)
                .Where(tuple => tuple.count > 1)
                .Select(tuple => tuple.name)
                .ToHashSet(LanguageConstants.IdentifierComparer);

            foreach (var @namespace in model.Root.ImportedNamespaces.Values)
            {
                foreach (var function in @namespace.Descendants.OfType<FunctionSymbol>())
                {
                    if (function.FunctionFlags.HasFlag(FunctionFlags.ParamDefaultsOnly) && !(enclosingDeclarationSymbol is ParameterSymbol))
                    {
                        // this function is only allowed in param defaults but the enclosing declaration is not bound to a parameter symbol
                        // therefore we should not suggesting this function as a viable completion
                        continue;
                    }

                    if (completions.ContainsKey(function.Name) || alwaysFullyQualifiedNames.Contains(function.Name))
                    {
                        // either there is a declaration with the same name as the function or the function is ambiguous between the imported namespaces
                        // either way the function must be fully qualified in the completion
                        var fullyQualifiedFunctionName = $"{@namespace.Name}.{function.Name}";
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

        private IEnumerable<CompletionItem> GetObjectPropertyNameCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.PropertyName) == false || context.Object == null)
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

            var specifiedPropertyNames = context.Object.ToKnownPropertyNames();

            // exclude read-only properties as they can't be set
            // exclude properties whose name has been specified in the object already
            return GetProperties(declaredType)
                .Where(p => p.Flags.HasFlag(TypePropertyFlags.ReadOnly) == false && specifiedPropertyNames.Contains(p.Name) == false)
                .Select(p => CreatePropertyNameCompletion(p, context.ReplacementRange));
        }

        private static IEnumerable<TypeProperty> GetProperties(TypeSymbol type)
        {
            switch (type)
            {
                case ObjectType objectType:
                    return objectType.Properties.Values;

                case DiscriminatedObjectType discriminated:
                    return discriminated.DiscriminatorProperty.AsEnumerable();

                default:
                    return Enumerable.Empty<TypeProperty>();
            }
        }

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
            if(declaredTypeAssignment == null)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return GetValueCompletions(model, context, declaredTypeAssignment.Reference.Type, declaredTypeAssignment.Flags);
        }

        private IEnumerable<CompletionItem> GetArrayItemCompletions(SemanticModel model, BicepCompletionContext context)
        {
            if (!context.Kind.HasFlag(BicepCompletionContextKind.ArrayItem))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var declaredTypeAssignment = GetDeclaredTypeAssignment(model, context.Array);
            if (declaredTypeAssignment == null || !(declaredTypeAssignment.Reference.Type is ArrayType arrayType))
            {
                return Enumerable.Empty<CompletionItem>();
            }

            return GetValueCompletions(model, context, arrayType.Item.Type, declaredTypeAssignment.Flags);
        }

        private static IEnumerable<CompletionItem> GetValueCompletions(SemanticModel model, BicepCompletionContext context, TypeSymbol type, DeclaredTypeFlags flags)
        {
            var completions = GetValueCompletionsForType(type, context.ReplacementRange);

            if (flags != DeclaredTypeFlags.Constant)
            {
                completions = completions.Concat(GetAccessibleSymbolCompletions(model, context));
            }

            return completions;
        }

        private static IEnumerable<CompletionItem> GetValueCompletionsForType(TypeSymbol? propertyType, Range replacementRange)
        {
            switch (propertyType)
            {
                case PrimitiveType _ when ReferenceEquals(propertyType, LanguageConstants.Bool):
                    yield return CreateKeywordCompletion(LanguageConstants.TrueKeyword, LanguageConstants.TrueKeyword, replacementRange, preselect: true, CompletionPriority.High);
                    yield return CreateKeywordCompletion(LanguageConstants.FalseKeyword, LanguageConstants.FalseKeyword, replacementRange, preselect: true, CompletionPriority.High);
                    
                    break;

                case StringLiteralType stringLiteral:
                    yield return CompletionItemBuilder.Create(CompletionItemKind.EnumMember)
                        .WithLabel(stringLiteral.Name)
                        .WithPlainTextEdit(replacementRange, stringLiteral.Name)
                        .WithDetail(stringLiteral.Name)
                        .Preselect()
                        .WithSortText(GetSortText(stringLiteral.Name, CompletionPriority.Medium));

                    break;

                case ArrayType _:
                    const string arrayLabel = "[]";
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Value)
                        .WithLabel(arrayLabel)
                        .WithSnippetEdit(replacementRange, "[$0]")
                        .WithDetail(arrayLabel)
                        .Preselect()
                        .WithSortText(GetSortText(arrayLabel, CompletionPriority.High));
                    
                    break;

                case ObjectType _:
                    const string objectLabel = "{}";
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Value)
                        .WithLabel(objectLabel)
                        .WithSnippetEdit(replacementRange, "{$0}")
                        .WithDetail(objectLabel)
                        .Preselect()
                        .WithSortText(GetSortText(objectLabel, CompletionPriority.High));

                    break;

                case UnionType union:
                    var aggregatedCompletions = union.Members.SelectMany(typeRef => GetValueCompletionsForType(typeRef.Type, replacementRange));
                    foreach (var completion in aggregatedCompletions)
                    {
                        yield return completion;
                    }

                    break;
            }
        }

        private static CompletionItem CreatePropertyNameCompletion(TypeProperty property, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Property)
                .WithLabel(property.Name)
                // property names containg spaces need to be escaped
                .WithPlainTextEdit(replacementRange, IsPropertyNameEscapingRequired(property) ? StringUtils.EscapeBicepString(property.Name) : property.Name)
                .WithCommitCharacters(PropertyCommitChars)
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(property.Name, priority));

        private static CompletionItem CreateKeywordCompletion(string keyword, string detail, Range replacementRange, bool preselect = false, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Keyword)
                .WithLabel(keyword)
                .WithPlainTextEdit(replacementRange, keyword)
                .WithDetail(detail)
                .Preselect(preselect)
                .WithSortText(GetSortText(keyword, priority));

        private static CompletionItem CreateTypeCompletion(TypeSymbol type, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithLabel(type.Name)
                .WithPlainTextEdit(replacementRange, type.Name)
                .WithDetail(type.Name)
                .WithSortText(GetSortText(type.Name, priority));

        private static CompletionItem CreateResourceTypeCompletion(ResourceTypeReference resourceType, int index, Range replacementRange)
        {
            var insertText = StringUtils.EscapeBicepString($"{resourceType.FullyQualifiedType}@{resourceType.ApiVersion}");
            return CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithLabel(insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .WithDocumentation($"Namespace: `{resourceType.Namespace}`{MarkdownNewLine}Type: `{resourceType.TypesString}`{MarkdownNewLine}API Version: `{resourceType.ApiVersion}`")
                // 8 hex digits is probably overkill :)
                .WithSortText(index.ToString("x8"));
        }

        /// <summary>
        /// Creates a completion with a contextual snippet. This will look like a snippet to the user.
        /// </summary>
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet)
                .WithLabel(label)
                .WithSnippetEdit(replacementRange, snippet)
                .WithDetail(detail)
                .WithDocumentation($"```bicep\n{new Snippet(snippet).FormatDocumentation()}\n```")
                .WithSortText(GetSortText(label, priority));

        private static CompletionItem CreateSymbolCompletion(Symbol symbol, Range replacementRange, string? insertText = null)
        {
            insertText ??= symbol.Name;
            var kind = GetCompletionItemKind(symbol);
            var priority = GetCompletionPriority(symbol);

            var completion = CompletionItemBuilder.Create(kind)
                .WithLabel(insertText)
                .WithSortText(GetSortText(insertText, priority));

            if (symbol is FunctionSymbol function)
            {
                // for functions withouth any parameters on all the overloads, we should be placing the cursor after the parentheses
                // for all other functions, the cursor should land between the parentheses so the user can specify the arguments
                bool hasParameters = function.Overloads.Any(overload => overload.HasParameters);
                var snippet = hasParameters
                    ? $"{insertText}($0)"
                    : $"{insertText}()$0";

                return completion
                    .WithDetail($"{insertText}()")
                    .WithSnippetEdit(replacementRange, snippet);
            }

            return completion
                .WithDetail(insertText)
                .WithPlainTextEdit(replacementRange, insertText);
        }

        // the priority must be a number in the sort text
        private static string GetSortText(string label, CompletionPriority priority) => $"{(int)priority}_{label}";

        private static CompletionPriority GetCompletionPriority(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionPriority.Low,
                SymbolKind.Namespace => CompletionPriority.Low,
                _ => CompletionPriority.Medium
            };

        private static CompletionItemKind GetCompletionItemKind(Symbol symbol) =>
            symbol.Kind switch
            {
                SymbolKind.Function => CompletionItemKind.Function,
                SymbolKind.File => CompletionItemKind.File,
                SymbolKind.Variable => CompletionItemKind.Variable,
                SymbolKind.Namespace => CompletionItemKind.Folder,
                SymbolKind.Output => CompletionItemKind.Value,
                SymbolKind.Parameter => CompletionItemKind.Field,
                SymbolKind.Resource => CompletionItemKind.Interface,
                SymbolKind.Module => CompletionItemKind.Module,
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

            return buffer.ToString();
        }
    }
}