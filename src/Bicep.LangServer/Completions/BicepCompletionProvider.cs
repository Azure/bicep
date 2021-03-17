// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azure.Deployments.Core.Comparers;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.LanguageServer.Extensions;
using Bicep.LanguageServer.Snippets;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Range = OmniSharp.Extensions.LanguageServer.Protocol.Models.Range;
using SymbolKind = Bicep.Core.Semantics.SymbolKind;

namespace Bicep.LanguageServer.Completions
{
    public class BicepCompletionProvider : ICompletionProvider
    {
        private const string MarkdownNewLine = "  \n";

        private static readonly Container<string> PropertyCommitChars = new Container<string>(":");

        private static readonly Container<string> ResourceSymbolCommitChars = new Container<string>(":");

        private static readonly Container<string> PropertyAccessCommitChars = new Container<string>(".");

        private IFileResolver FileResolver;

        public BicepCompletionProvider(IFileResolver fileResolver)
        {
            this.FileResolver = fileResolver;
        }

        public IEnumerable<CompletionItem> GetFilteredCompletions(Compilation compilation, BicepCompletionContext context)
        {
            var model = compilation.GetEntrypointSemanticModel();

            return GetDeclarationCompletions(context)
                .Concat(GetSymbolCompletions(model, context))
                .Concat(GetDeclarationTypeCompletions(context))
                .Concat(GetObjectPropertyNameCompletions(model, context))
                .Concat(GetMemberAccessCompletions(compilation, context))
                .Concat(GetResourceAccessCompletions(compilation, context))
                .Concat(GetArrayIndexCompletions(compilation, context))
                .Concat(GetPropertyValueCompletions(model, context))
                .Concat(GetArrayItemCompletions(model, context))
                .Concat(GetResourceTypeCompletions(model, context))
                .Concat(GetModulePathCompletions(model, context))
                .Concat(GetResourceOrModuleBodyCompletions(context))
                .Concat(GetTargetScopeCompletions(model, context));
        }

        private IEnumerable<CompletionItem> GetDeclarationCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.TopLevelDeclarationStart))
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

                yield return CreateKeywordCompletion(LanguageConstants.TargetScopeKeyword, "Target Scope keyword", context.ReplacementRange);
            }

            if (context.Kind.HasFlag(BicepCompletionContextKind.NestedResourceDeclarationStart))
            {
                yield return CreateKeywordCompletion(LanguageConstants.ResourceKeyword, "Resource keyword", context.ReplacementRange);

                // leaving out the API version on this, because we expect its more common to inherit from the containing resource.
                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Nested resource with defaults", @"resource ${1:Identifier} '${2:Type}' = {
  name: $3
  properties: {
    $0
  }
}", context.ReplacementRange, insertTextMode: InsertTextMode.AdjustIndentation);

                yield return CreateContextualSnippetCompletion(LanguageConstants.ResourceKeyword, "Nested resource without defaults", @"resource ${1:Identifier} '${2:Type}' = {
  name: $3
  $0
}
", context.ReplacementRange, insertTextMode: InsertTextMode.AdjustIndentation);
            }
        }

        private IEnumerable<CompletionItem> GetTargetScopeCompletions(SemanticModel model, BicepCompletionContext context)
        {
            return context.Kind.HasFlag(BicepCompletionContextKind.TargetScope) && context.TargetScope is { } targetScope
                ? GetValueCompletionsForType(model.GetDeclaredType(targetScope), context.ReplacementRange, model, context)
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
                parentSymbol.TryGetResourceTypeReference() is ResourceTypeReference parentTypeReference)
            {
                // This is more complex because we allow the API version to be omitted, so we want to make a list of unique values
                // for the FQT, and then create a "no version" completion + a completion for each version.
                var filtered = model.Compilation.ResourceTypeProvider.GetAvailableTypes()
                    .Where(rt => parentTypeReference.IsParentOf(rt))
                    .ToLookup(rt => rt.FullyQualifiedType);

                var index = 0;
                var items = new List<CompletionItem>();
                foreach (var group in filtered.OrderBy(group => group.Key, StringComparer.OrdinalIgnoreCase))
                {
                    // Doesn't matter which one of the group we take, we're leaving out the version.
                    items.Add(CreateResourceTypeSegmentCompletion(group.First(), index++, context.ReplacementRange, includeApiVersion: false, displayApiVersion: parentTypeReference.ApiVersion));

                    foreach (var resourceType in group.OrderByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance))
                    {
                        items.Add(CreateResourceTypeSegmentCompletion(resourceType, index++, context.ReplacementRange, includeApiVersion: true, displayApiVersion: resourceType.ApiVersion));
                    }
                }

                return items;
            }
            // we need to ensure that Microsoft.Compute/virtualMachines@whatever comes before Microsoft.Compute/virtualMachines/extensions@whatever
            // similarly, newest api versions should be shown first
            return model.Compilation.ResourceTypeProvider.GetAvailableTypes()
                .OrderBy(rt => rt.FullyQualifiedType, StringComparer.OrdinalIgnoreCase)
                .ThenByDescending(rt => rt.ApiVersion, ApiVersionComparer.Instance)
                .Select((reference, index) => CreateResourceTypeCompletion(reference, index, context.ReplacementRange))
                .ToList();
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
            if (FileResolver.TryResolveModulePath(model.SyntaxTree.FileUri, ".") is not { } cwdUri
                || FileResolver.TryResolveModulePath(cwdUri, entered) is not { } query)
            {
                return Enumerable.Empty<CompletionItem>();
            }

            var files = Enumerable.Empty<Uri>();
            var dirs = Enumerable.Empty<Uri>();


            // technically bicep files do not have to follow the bicep extension, so 
            // we are not enforcing *.bicep get files command
            if (FileResolver.TryDirExists(query))
            {
                files = FileResolver.GetFiles(query, string.Empty);
                dirs = FileResolver.GetDirectories(query, string.Empty);
            }
            else if (FileResolver.TryResolveModulePath(query, ".") is { } queryParent)
            {
                files = FileResolver.GetFiles(queryParent, "");
                dirs = FileResolver.GetDirectories(queryParent, "");
            }
            // "./" will not be preserved when making relative Uris. We have to go and manually add it.
            // Prioritize .bicep files higher than other files.
            var fileItems = files
                .Where(file => file != model.SyntaxTree.FileUri)
                .Where(file => file.Segments.Last().EndsWith(LanguageConstants.LanguageFileExtension))
                .Select(file => CreateModulePathCompletion(
                    file.Segments.Last(),
                    (entered.StartsWith("./") ? "./" : "") + cwdUri.MakeRelativeUri(file).ToString(),
                    context.ReplacementRange,
                    CompletionItemKind.File,
                    file.Segments.Last().EndsWith(LanguageConstants.LanguageId) ? CompletionPriority.High : CompletionPriority.Medium))
                .ToList();

            var dirItems = dirs
                .Select(dir => CreateModulePathCompletion(
                    dir.Segments.Last(),
                    (entered.StartsWith("./") ? "./" : "") + cwdUri.MakeRelativeUri(dir).ToString(),
                    context.ReplacementRange,
                    CompletionItemKind.Folder,
                    CompletionPriority.Medium)
                .WithCommand(new Command { Name = EditorCommands.RequestCompletions }))
                .ToList();
            return fileItems.Concat(dirItems);
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

        private IEnumerable<CompletionItem> GetResourceOrModuleBodyCompletions(BicepCompletionContext context)
        {
            if (context.Kind.HasFlag(BicepCompletionContextKind.ResourceBody) || context.Kind.HasFlag(BicepCompletionContextKind.ModuleBody))
            {
                yield return CreateObjectBodyCompletion(context.ReplacementRange);

                yield return CreateResourceOrModuleConditionCompletion(context.ReplacementRange);

                // loops are always allowed in a resource/module
                foreach (var completion in CreateLoopCompletions(context.ReplacementRange, LanguageConstants.Object, filtersAllowed: true))
                {
                    yield return completion;
                }
            }
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

            if (!context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName))
            {
                // add namespaces first
                AddSymbolCompletions(completions, model.Root.ImportedNamespaces.Values);

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
                AddSymbolCompletions(completions, model.Root.ImportedNamespaces.Values.Where(
                    @namespace => GetAccessibleDecoratorFunctionsWithCache(@namespace.Type).Any()));

                // Record the names of the non-output declarations which will be used to check name clashes later.
                declaredNames.UnionWith(model.Root.Declarations.Where(decl => decl.NameSyntax.IsValid && decl is not OutputSymbol).Select(decl => decl.Name));
            }

            // get names of functions that always require to be fully qualified due to clashes between namespaces
            var alwaysFullyQualifiedNames = model.Root.ImportedNamespaces
                .SelectMany(pair => context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName)
                    ? GetAccessibleDecoratorFunctionsWithCache(pair.Value.Type)
                    : pair.Value.Type.MethodResolver.GetKnownFunctions().Values)
                .GroupBy(func => func.Name, (name, functionSymbols) => (name, count: functionSymbols.Count()), LanguageConstants.IdentifierComparer)
                .Where(tuple => tuple.count > 1)
                .Select(tuple => tuple.name)
                .ToHashSet(LanguageConstants.IdentifierComparer);

            foreach (var @namespace in model.Root.ImportedNamespaces.Values)
            {
                var functionSymbols = context.Kind.HasFlag(BicepCompletionContextKind.DecoratorName)
                    ? GetAccessibleDecoratorFunctionsWithCache(@namespace.Type)
                    : @namespace.Type.MethodResolver.GetKnownFunctions().Values;

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
                .Select(p => CreatePropertyAccessCompletion(p, compilation.SyntaxTreeGrouping.EntryPoint, context.PropertyAccess, context.ReplacementRange))
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

            var specifiedPropertyNames = context.Object.ToKnownPropertyNames();

            // exclude read-only properties as they can't be set
            // exclude properties whose name has been specified in the object already
            return GetProperties(declaredType)
                .Where(p => !p.Flags.HasFlag(TypePropertyFlags.ReadOnly) && specifiedPropertyNames.Contains(p.Name) == false)
                .Select(p => CreatePropertyNameCompletion(p, context.ReplacementRange));
        }

        private static IEnumerable<TypeProperty> GetProperties(TypeSymbol? type)
        {
            switch (type)
            {
                case ResourceType resourceType:
                    return GetProperties(resourceType.Body.Type);

                case ModuleType moduleType:
                    return GetProperties(moduleType.Body.Type);

                case ObjectType objectType:
                    return objectType.Properties.Values;

                case DiscriminatedObjectType discriminated:
                    return discriminated.DiscriminatorProperty.AsEnumerable();

                default:
                    return Enumerable.Empty<TypeProperty>();
            }
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

            return GetValueCompletionsForType(declaredTypeAssignment.Reference.Type, context.ReplacementRange, model, context);
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

            return GetValueCompletionsForType(arrayType.Item.Type, context.ReplacementRange, model, context);
        }

        private static IEnumerable<CompletionItem> GetValueCompletionsForType(TypeSymbol? propertyType, Range replacementRange, SemanticModel semanticModel, BicepCompletionContext context)
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

                case ArrayType arrayType:
                    const string arrayLabel = "[]";
                    yield return CompletionItemBuilder.Create(CompletionItemKind.Value)
                        .WithLabel(arrayLabel)
                        .WithSnippetEdit(replacementRange, "[\n\t$0\n]", InsertTextMode.AdjustIndentation)
                        .WithDetail(arrayLabel)
                        .Preselect()
                        .WithSortText(GetSortText(arrayLabel, CompletionPriority.High));

                    if (context.Kind.HasFlag(BicepCompletionContextKind.PropertyValue) &&
                        context.Property is not null &&
                        ForSyntaxValidatorVisitor.IsAddingPropertyLoopAllowed(semanticModel, context.Property))
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
                    yield return CreateObjectBodyCompletion(replacementRange);
                    break;

                case UnionType union:
                    var aggregatedCompletions = union.Members.SelectMany(typeRef => GetValueCompletionsForType(typeRef.Type, replacementRange, semanticModel, context));
                    foreach (var completion in aggregatedCompletions)
                    {
                        yield return completion;
                    }

                    break;
            }
        }

        private static CompletionItem CreateObjectBodyCompletion(Range replacementRange)
        {
            const string objectLabel = "{}";
            return CompletionItemBuilder.Create(CompletionItemKind.Value)
                .WithLabel(objectLabel)
                .WithSnippetEdit(replacementRange, "{\n\t$0\n}", InsertTextMode.AdjustIndentation)
                .WithDetail(objectLabel)
                .Preselect()
                .WithSortText(GetSortText(objectLabel, CompletionPriority.High));
        }

        private static CompletionItem CreateResourceOrModuleConditionCompletion(Range replacementRange)
        {
            const string conditionLabel = "if";
            return CompletionItemBuilder.Create(CompletionItemKind.Snippet)
                .WithLabel(conditionLabel)
                .WithSnippetEdit(replacementRange, "if (${1:condition}) {\n\t$0\n}", InsertTextMode.AdjustIndentation)
                .WithDetail(conditionLabel)
                .WithSortText(GetSortText(conditionLabel, CompletionPriority.High));
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

            yield return CreateContextualSnippetCompletion(loopLabel, loopLabel, itemSnippet, replacementRange, CompletionPriority.High, InsertTextMode.AdjustIndentation);
            yield return CreateContextualSnippetCompletion(indexedLabel, indexedLabel, indexedSnippet, replacementRange, CompletionPriority.High, InsertTextMode.AdjustIndentation);

            if(filtersAllowed && assignableToObject && !assignableToArray)
            {
                yield return CreateContextualSnippetCompletion(filteredLabel, filteredLabel, "[for (${2:item}, ${3:index}) in ${1:list}: if (${4:condition}) {\n\t$0\n}]", replacementRange, CompletionPriority.High, InsertTextMode.AdjustIndentation);
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

        private static CompletionItem CreatePropertyIndexCompletion(TypeProperty property, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var escaped = StringUtils.EscapeBicepString(property.Name);
            return CompletionItemBuilder.Create(CompletionItemKind.Property)
                .WithLabel(escaped)
                .WithPlainTextEdit(replacementRange, escaped)
                .WithDetail(FormatPropertyDetail(property))
                .WithDocumentation(FormatPropertyDocumentation(property))
                .WithSortText(GetSortText(escaped, priority));
        }

        private static CompletionItem CreatePropertyAccessCompletion(TypeProperty property, SyntaxTree tree, PropertyAccessSyntax propertyAccess, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium)
        {
            var item = CompletionItemBuilder.Create(CompletionItemKind.Property)
                .WithLabel(property.Name)
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

            return item;
        }

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

        private static CompletionItem CreateResourceTypeSegmentCompletion(ResourceTypeReference resourceType, int index, Range replacementRange, bool includeApiVersion, string displayApiVersion)
        {
            // We create one completion with and without the API version.
            var insertText = includeApiVersion ?
                StringUtils.EscapeBicepString($"{resourceType.Types[^1]}@{resourceType.ApiVersion}") :
                StringUtils.EscapeBicepString($"{resourceType.Types[^1]}");
            return CompletionItemBuilder.Create(CompletionItemKind.Class)
                .WithLabel(insertText)
                .WithPlainTextEdit(replacementRange, insertText)
                .WithDocumentation($"Namespace: `{resourceType.Namespace}`{MarkdownNewLine}Type: `{resourceType.TypesString}`{MarkdownNewLine}API Version: `{displayApiVersion}`")
                // 8 hex digits is probably overkill :)
                .WithSortText(index.ToString("x8"));
        }

        private static CompletionItem CreateModulePathCompletion(string name, string path, Range replacementRange, CompletionItemKind completionItemKind, CompletionPriority priority)
        {
            path = StringUtils.EscapeBicepString(path);
            var item = CompletionItemBuilder.Create(completionItemKind)
                .WithLabel(name)
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
        private static CompletionItem CreateContextualSnippetCompletion(string label, string detail, string snippet, Range replacementRange, CompletionPriority priority = CompletionPriority.Medium, InsertTextMode insertTextMode = InsertTextMode.AsIs) =>
            CompletionItemBuilder.Create(CompletionItemKind.Snippet)
                .WithLabel(label)
                .WithSnippetEdit(replacementRange, snippet, insertTextMode)
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
                    completion.WithCommand(new Command { Name = EditorCommands.SignatureHelp });
                }

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

            return buffer.ToString();
        }
    }
}
