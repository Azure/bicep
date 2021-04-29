// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Bicep.Core.Diagnostics;
using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        // Used to cache resource declarations. Maps resource type to body text
        private ConcurrentDictionary<string, string> resourceTypeToBodyMap = new ConcurrentDictionary<string, string>();
        // Used to cache resource dependencies. Maps resource type to it's dependencies
        private ConcurrentDictionary<string, string> resourceTypeToDependentsMap = new ConcurrentDictionary<string, string>();
        // Used to cache resource body snippets
        private ConcurrentDictionary<string, IEnumerable<Snippet>> resourceBodySnippetsCache = new ConcurrentDictionary<string, IEnumerable<Snippet>>();
        // Used to cache top level declarations
        private HashSet<Snippet> topLevelNamedDeclarationSnippets = new HashSet<Snippet>();

        public SnippetsProvider()
        {
            Initialize();
        }

        private void Initialize()
        {
            string pathPrefix = "Snippets/Templates/";
            Assembly assembly = typeof(SnippetsProvider).Assembly;
            IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
                StreamReader streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

                (string description, string snippetText) = GetDescriptionAndText(streamReader.ReadToEnd(), manifestResourceName);
                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
                CompletionPriority completionPriority = CompletionPriority.Medium;

                if (prefix.StartsWith("resource"))
                {
                    completionPriority = CompletionPriority.High;
                }

                Snippet snippet = new Snippet(snippetText, completionPriority, prefix, description);

                topLevelNamedDeclarationSnippets.Add(snippet);
            }
        }

        public (string, string) GetDescriptionAndText(string? template, string manifestResourceName)
        {
            string description = string.Empty;
            string text = string.Empty;

            if (!string.IsNullOrWhiteSpace(template))
            {
                Parser parser = new Parser(template);
                ProgramSyntax programSyntax = parser.Program();
                IEnumerable<SyntaxBase> declarations = programSyntax.Declarations;

                if (declarations.Any() && declarations.First() is StatementSyntax statementSyntax)
                {
                    text = template.Substring(statementSyntax.Span.Position);

                    ImmutableArray<SyntaxBase> children = programSyntax.Children;

                    if (children.Length > 0 &&
                        children[0] is Token firstToken &&
                        firstToken is not null &&
                        firstToken.LeadingTrivia[0] is SyntaxTrivia syntaxTrivia &&
                        syntaxTrivia.Type is SyntaxTriviaType.SingleLineComment)
                    {
                        description = syntaxTrivia.Text.Substring("// ".Length);
                    }

                    CacheResourceDeclarationAndDependencies(template, manifestResourceName);
                }
            }

            return (description, text);
        }

        public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets() => topLevelNamedDeclarationSnippets;

        private void CacheResourceDeclarationAndDependencies(string template, string manifestResourceName)
        {
            ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> dependencies = GetResourceDependencies(template, manifestResourceName);

            foreach (KeyValuePair<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> kvp in dependencies)
            {
                DeclaredSymbol declaredSymbol = kvp.Key;

                if (declaredSymbol.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax)
                {
                    string type = declaredSymbol.Type.Name;

                    CacheResourceDeclaration(resourceDeclarationSyntax, type, template);
                    CacheResourceDependencies(kvp.Value, template, type);
                }
            }
        }

        private void CacheResourceDeclaration(ResourceDeclarationSyntax resourceDeclarationSyntax, string type, string template)
        {
            if (!resourceTypeToBodyMap.ContainsKey(type))
            {
                TextSpan bodySpan = resourceDeclarationSyntax.Value.Span;
                string bodyText = template.Substring(bodySpan.Position, bodySpan.Length);

                resourceTypeToBodyMap.TryAdd(type, bodyText);
            }
        }

        private void CacheResourceDependencies(ImmutableHashSet<ResourceDependency> resourceDependencies, string template, string resourceType)
        {
            if (resourceDependencies.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (ResourceDependency resourceDependency in resourceDependencies)
                {
                    TextSpan span = resourceDependency.Resource.DeclaringSyntax.Span;
                    sb.AppendLine(template.Substring(span.Position, span.Length));
                }

                resourceTypeToDependentsMap.TryAdd(resourceType, sb.ToString());
            }
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(string template, string manifestResourceName)
        {
            // Snippets with prefix resource will not have valid type, so there can't be any dependencies
            if (manifestResourceName.Contains("resource"))
            {
                return ImmutableDictionary.Create<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();
            }

            string path = Path.GetFullPath(manifestResourceName);
            SyntaxTree syntaxTree = SyntaxTree.Create(new Uri(path), template);
            SyntaxTreeGrouping syntaxTreeGrouping = new SyntaxTreeGrouping(
                syntaxTree,
                ImmutableHashSet.Create(syntaxTree),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, SyntaxTree>(),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>());

            Compilation compilation = new Compilation(AzResourceTypeProvider.CreateWithAzTypes(), syntaxTreeGrouping);
            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();

            return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public IEnumerable<Snippet> GetResourceBodyCompletionSnippets(TypeSymbol typeSymbol)
        {
            if (resourceBodySnippetsCache.TryGetValue(typeSymbol.Name, out IEnumerable<Snippet>? cachedSnippets) && cachedSnippets.Any())
            {
                return cachedSnippets;
            }

            List<Snippet> snippets = new List<Snippet>();

            snippets.Add(GetEmptySnippet());

            Snippet? snippetFromExistingTemplate = GetResourceBodyCompletionSnippetFromTemplate(typeSymbol);
            if (snippetFromExistingTemplate is not null)
            {
                snippets.Add(snippetFromExistingTemplate);
            }

            Snippet? snippetFromAzTypes = GetResourceBodyCompletionSnippetFromAzTypes(typeSymbol);
            if (snippetFromAzTypes is not null)
            {
                snippets.Add(snippetFromAzTypes);
            }

            // Add to cache
            resourceBodySnippetsCache.TryAdd(typeSymbol.Name, snippets);

            return snippets;
        }

        private Snippet? GetResourceBodyCompletionSnippetFromTemplate(TypeSymbol typeSymbol)
        {
            string label = "insert-snippet";
            string description = "Snippet";
            string type = typeSymbol.Name;

            StringBuilder sb = new StringBuilder();

            // Get resource body completion snippet from checked in static template file, if available
            if (resourceTypeToBodyMap.TryGetValue(type, out string? resourceBody))
            {
                sb.AppendLine(resourceBody);

                if (resourceTypeToDependentsMap.TryGetValue(type, out string? resourceDependencies))
                {
                    sb.Append(resourceDependencies);
                }

                return new Snippet(sb.ToString(), CompletionPriority.Medium, label, description);
            }

            return null;
        }

        private Snippet? GetResourceBodyCompletionSnippetFromAzTypes(TypeSymbol typeSymbol)
        {
            string label = "insert-required";
            string description = "Required properties";

            if (typeSymbol is ResourceType resourceType && resourceType.Body is ObjectType objectType)
            {
                int index = 1;
                StringBuilder sb = new StringBuilder();

                foreach (KeyValuePair<string, TypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                {
                    string? snippetText = GetSnippetText(kvp.Value, ref index);

                    if (snippetText is not null)
                    {
                        sb.Append(snippetText);
                    }
                }

                if (sb.Length > 0)
                {
                    // Insert open curly at the beginning
                    sb.Insert(0, "{\n");

                    // Append final tab stop
                    sb.Append("\t$0\n}");

                    return new Snippet(sb.ToString(), CompletionPriority.Medium, label, description);
                }
            }

            return null;
        }

        private string? GetSnippetText(TypeProperty typeProperty, ref int index)
        {
            if (typeProperty.Flags.HasFlag(TypePropertyFlags.Required))
            {
                StringBuilder sb = new StringBuilder();

                if (typeProperty.TypeReference.Type is ObjectType objectType)
                {
                    sb.AppendLine("\t" + typeProperty.Name + ": {");

                    foreach (KeyValuePair<string, TypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                    {
                        string? snippetText = GetSnippetText(kvp.Value, ref index);
                        if (snippetText is not null)
                        {
                            sb.Append(snippetText);
                        }
                    }

                    sb.AppendLine("\t}");
                }
                else
                {
                    sb.AppendLine("\t" + typeProperty.Name + ": $" + (index).ToString());
                    index++;
                }

                return sb.ToString();
            }

            return null;
        }

        private Snippet GetEmptySnippet()
        {
            string label = "{}";

            return new Snippet("{\n\t$0\n}", CompletionPriority.Medium, label, label);
        }
    }
}
