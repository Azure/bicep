// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Bicep.Core.Emit;
using Bicep.Core.Parsing;
using Bicep.Core.Semantics;
using Bicep.Core.Syntax;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        private HashSet<Snippet> topLevelNamedDeclarationSnippets = new HashSet<Snippet>();
        private Dictionary<string, string> resourceTypeToBodyMap = new Dictionary<string, string>();
        public Dictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> Dependencies = new Dictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>>();
        private Dictionary<string, string> resourceTypeToDependentsMap = new Dictionary<string, string>();

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

                    CacheResourceDependencies(template, manifestResourceName);
                }
            }

            return (description, text);
        }

        public IEnumerable<Snippet> GetTopLevelNamedDeclarationSnippets() => topLevelNamedDeclarationSnippets;

        private void CacheResourceDependencies(string template, string manifestResourceName)
        {
            ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> dependencies = GetResourceDependencies(template, manifestResourceName);

            foreach (KeyValuePair<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> kvp in dependencies)
            {
                CacheResourceDeclarations(template, kvp.Key.DeclaringSyntax);

                if (kvp.Value.Any())
                {
                    if (kvp.Key.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax &&
                        resourceDeclarationSyntax.TypeString is StringSyntax stringSyntax)
                    {
                        string resourceDependencies = string.Empty;
                        foreach (ResourceDependency resourceDependency in kvp.Value)
                        {
                            TextSpan span = resourceDependency.Resource.DeclaringSyntax.Span;
                            resourceDependencies = template.Substring(span.Position, span.Length) + "\n";
                        }

                        string type = stringSyntax.StringTokens.First().Text;

                        resourceTypeToDependentsMap.Add(type, resourceDependencies);
                    }
                }
            }
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(string template, string manifestResourceName)
        {
            string path = Path.GetFullPath(manifestResourceName);
            SyntaxTree syntaxTree = SyntaxTree.Create(new Uri(path), template);
            SyntaxTreeGrouping syntaxTreeGrouping = new SyntaxTreeGrouping(
                syntaxTree,
                ImmutableHashSet.Create(syntaxTree),
                null!,
                null!);

            Compilation compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();

            return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        private void CacheResourceDeclarations(string template, SyntaxBase syntaxBase)
        {
            if (syntaxBase is ResourceDeclarationSyntax resourceDeclarationSyntax &&
                resourceDeclarationSyntax.TypeString is StringSyntax stringSyntax)
            {
                string type = stringSyntax.StringTokens.First().Text;

                TextSpan bodySpan = resourceDeclarationSyntax.Value.Span;

                string bodyText = template.Substring(bodySpan.Position, bodySpan.Length);

                if (!resourceTypeToBodyMap.ContainsKey(type))
                {
                    resourceTypeToBodyMap.Add(type, bodyText);
                }
            }
        }

        public Snippet? GetResourceBodyCompletionSnippet(string type)
        {
            Snippet? snippet = topLevelNamedDeclarationSnippets.Where(x => x.Text.Contains(type)).FirstOrDefault();

            if (snippet is null)
            {
                return null;
            }

            string text = string.Empty;

            if (resourceTypeToBodyMap.TryGetValue(type, out string? resourceBody))
            {
                text = resourceBody;

                if (resourceTypeToDependentsMap.TryGetValue(type, out string? resourceDependencies))
                {
                    text += "\n" + resourceDependencies;
                }
            }

            return new Snippet(text, snippet.CompletionPriority, snippet.Prefix, snippet.Detail);
        }
    }
}
