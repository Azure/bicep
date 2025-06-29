// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Bicep.Core;
using Bicep.Core.Emit;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Resources;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Types;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets;

public class SnippetCacheBuilder
{
    private static readonly Regex SnippetPlaceholderCommentPattern = new(@"\/\*(?<snippetPlaceholder>(.*?))\*\/('(.*?)'|\w+|-\d+|.*?)", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

    // Used to cache resource declaration information. Maps resource type reference to prefix, identifier, body text and description
    private readonly ConcurrentDictionary<ResourceTypeReference, SnippetCache.SnippetCacheSnippet> resourceTypeReferenceInfoMap = new();
    // Used to cache resource dependencies. Maps resource type reference to it's dependencies
    private readonly ConcurrentDictionary<ResourceTypeReference, string> resourceTypeReferenceToDependentsMap = new();
    // Used to cache information about child type symbols in nested resource scenario. Maps resource type reference to nested type symbols
    private readonly ConcurrentDictionary<ResourceTypeReference, ImmutableArray<ResourceTypeReference>> resourceTypeReferenceToChildTypeSymbolsMap = new();
    // Used to cache top level declarations
    private readonly HashSet<Snippet> topLevelNamedDeclarationSnippets = new();

    private readonly BicepCompiler bicepCompiler;

    public SnippetCacheBuilder(BicepCompiler bicepCompiler)
    {
        this.bicepCompiler = bicepCompiler;
    }

    public async Task<SnippetCache> Build()
    {
        string pathPrefix = "Files/SnippetTemplates/";
        Assembly assembly = typeof(SnippetsProvider).Assembly;
        IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal));

        foreach (var manifestResourceName in manifestResourceNames)
        {
            Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
            var streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

            var (description, snippetText) = await GetDescriptionAndSnippetText(await streamReader.ReadToEndAsync(), manifestResourceName);
            string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
            CompletionPriority completionPriority = CompletionPriority.Medium;

            if (prefix.StartsWith("resource"))
            {
                completionPriority = CompletionPriority.High;
            }

            var snippet = new Snippet(snippetText, completionPriority, prefix, description);

            topLevelNamedDeclarationSnippets.Add(snippet);
        }

        return new(
            resourceTypeReferenceInfoMap.ToImmutableDictionary(),
            resourceTypeReferenceToDependentsMap.ToImmutableDictionary(),
            resourceTypeReferenceToChildTypeSymbolsMap.ToImmutableDictionary(k => k.Key, k => k.Value.OrderBy(r => r.ToString()).ToImmutableArray()),
            [.. topLevelNamedDeclarationSnippets]);
    }

    public async Task<(string description, string snippet)> GetDescriptionAndSnippetText(string template, string manifestResourceName)
    {
        var description = string.Empty;
        var parser = new Parser(template);
        var programSyntax = parser.Program();
        var declarations = programSyntax.Declarations;

        if (declarations.Any() && declarations.First() is StatementSyntax statementSyntax)
        {
            template = template.Substring(statementSyntax.Span.Position);

            var children = programSyntax.Children;

            if (children.Length > 0 &&
                children[0] is Token firstToken &&
                firstToken.LeadingTrivia[0] is SyntaxTrivia syntaxTrivia &&
                syntaxTrivia.Type is SyntaxTriviaType.SingleLineComment)
            {
                description = syntaxTrivia.Text.Substring("// ".Length);
            }

            await CacheResourceDeclarationAndDependencies(template, manifestResourceName, description);

            return (description, RemoveSnippetPlaceholderComments(template));
        }

        return (string.Empty, string.Empty);
    }

    private async Task CacheResourceDeclarationAndDependencies(string template, string manifestResourceName, string description)
    {
        var dependencies = await GetResourceDependencies(template, manifestResourceName);

        foreach (var (declaredSymbol, syntax) in dependencies)
        {
            if (declaredSymbol.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax)
            {
                if (declaredSymbol.Type is ResourceType resourceType && resourceType.TypeKind != TypeKind.Error)
                {
                    var resourceTypeReference = resourceType.TypeReference;
                    CacheResourceDeclaration(resourceDeclarationSyntax, resourceTypeReference, template, description, manifestResourceName);
                    CacheResourceDependencies(resourceTypeReference, syntax, template);
                }
            }
        }
    }

    private void CacheResourceDeclaration(ResourceDeclarationSyntax resourceDeclaration, ResourceTypeReference resourceTypeReference, string template, string description, string manifestResourceName)
    {
        if (!resourceTypeReferenceInfoMap.ContainsKey(resourceTypeReference))
        {
            var bodySpan = resourceDeclaration.Value.Span;
            var bodyText = template.Substring(bodySpan.Position, bodySpan.Length);
            bodyText = RemoveSnippetPlaceholderComments(bodyText);

            // snippet placeholders are authored using a multi-line comment syntax. To include this when fetching the identifier,
            // we have to fetch everything from the end of the preceding syntax (as multi-line comments are stored in trailing trivia on the previous token).
            var nameStart = resourceDeclaration.Keyword.Span.Position + resourceDeclaration.Keyword.Span.Length;
            var nameEnd = resourceDeclaration.Name.Span.Position + resourceDeclaration.Name.Span.Length;
            var identifier = template.Substring(nameStart, nameEnd - nameStart).Trim();
            identifier = RemoveSnippetPlaceholderComments(identifier);

            var prefix = Path.GetFileNameWithoutExtension(manifestResourceName);

            resourceTypeReferenceInfoMap.TryAdd(resourceTypeReference, new(prefix, identifier, bodyText, description));
        }
    }

    private void CacheResourceDependencies(ResourceTypeReference childResourceTypeReference, ImmutableHashSet<ResourceDependency> resourceDependencies, string template)
    {
        if (resourceDependencies.Any())
        {
            var sb = new StringBuilder();

            foreach (var resourceDependency in resourceDependencies)
            {
                if (resourceDependency.Resource is ResourceSymbol resourceSymbol &&
                    resourceSymbol.TryGetResourceTypeReference() is ResourceTypeReference resourceTypeReference)
                {
                    resourceTypeReferenceToChildTypeSymbolsMap.AddOrUpdate(
                        resourceTypeReference,
                        _ => [childResourceTypeReference],
                        (_, children) => children.Add(childResourceTypeReference));
                }

                var span = resourceDependency.Resource.DeclaringSyntax.Span;
                var dependentTemplate = template.Substring(span.Position, span.Length);

                sb.AppendLine(RemoveSnippetPlaceholderComments(dependentTemplate));
            }

            var value = StringUtils.ReplaceNewlines(sb.ToString(), "\n");
            resourceTypeReferenceToDependentsMap.TryAdd(childResourceTypeReference, value);
        }
    }

    private async Task<ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>> GetResourceDependencies(string template, string manifestResourceName)
    {
        // Snippets with prefix resource will not have valid type, so there can't be any dependencies
        if (manifestResourceName.Contains("resource"))
        {
            return ImmutableDictionary.Create<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();
        }

        // We need to provide uri for syntax tree creation, but it's not used anywhere. In order to avoid
        // cross platform issues, we'll provide a placeholder uri.
        var bicepFile = this.bicepCompiler.SourceFileFactory.CreateBicepFile(new Uri($"inmemory://{manifestResourceName}"), template);
        var workspace = new Workspace();
        workspace.UpsertSourceFiles(bicepFile.AsEnumerable());

        var compilation = await bicepCompiler.CreateCompilation(bicepFile.Uri, workspace, skipRestore: true);
        var semanticModel = compilation.GetEntrypointSemanticModel();

        return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
    }

    private string? GetSnippetText(NamedTypeProperty typeProperty, int indentLevel, ref int index, string? discrimatedObjectKey = null)
    {
        if (typeProperty.Flags.HasFlag(TypePropertyFlags.Required))
        {
            StringBuilder sb = new();

            if (typeProperty.TypeReference.Type is ObjectType objectType)
            {
                sb.AppendLine(GetIndentString(indentLevel) + typeProperty.Name + ": {");

                indentLevel++;

                foreach (KeyValuePair<string, NamedTypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                {
                    string? snippetText = GetSnippetText(kvp.Value, indentLevel, ref index);
                    if (snippetText is not null)
                    {
                        sb.Append(snippetText);
                    }
                }

                indentLevel--;
                sb.AppendLine(GetIndentString(indentLevel) + "}");
            }
            else
            {
                string value = $": ${index}";
                bool shouldIncrementIndent = true;

                if (discrimatedObjectKey is not null &&
                    typeProperty.TypeReference.Type is TypeSymbol typeSymbol &&
                    typeSymbol.Name == discrimatedObjectKey)
                {
                    value = ": " + discrimatedObjectKey;
                    shouldIncrementIndent = false;
                }

                sb.AppendLine(GetIndentString(indentLevel) + typeProperty.Name + value);

                if (shouldIncrementIndent)
                {
                    index++;
                }
            }

            return StringUtils.ReplaceNewlines(sb.ToString(), "\n");
        }

        return null;
    }

    private string GetIndentString(int indentLevel)
    {
        return new string('\t', indentLevel);
    }

    public static string RemoveSnippetPlaceholderComments(string text)
    {
        var matches = SnippetPlaceholderCommentPattern.Matches(text);
        // We will be performing multiple string replacements, better to do it in-place
        var sb = new StringBuilder(text);

        // To avoid recomputing spans, we will perform the replacements in reverse order
        foreach (var match in matches.OrderByDescending(x => x.Index))
        {
            sb.Replace(oldValue: match.Value,
                            newValue: match.Groups["snippetPlaceholder"].Value,
                            startIndex: match.Index,
                            count: match.Length);
        }

        return StringUtils.ReplaceNewlines(sb.ToString(), "\n");
    }
}
