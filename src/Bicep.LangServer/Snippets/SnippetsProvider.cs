// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
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
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        private Dictionary<string, string> resourceTypeToBodyMap = new Dictionary<string, string>();
        private Dictionary<string, string> resourceTypeToDependentsMap = new Dictionary<string, string>();
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
                (ResourceDeclarationSyntax? resourceDeclarationSyntax, string? resourceType) = GetResourceDeclarationSyntaxWithType(kvp.Key);

                if (resourceDeclarationSyntax is not null && resourceType is not null)
                {
                    CacheResourceDeclaration(resourceDeclarationSyntax, resourceType, template);
                    CacheResourceDependencies(kvp.Value, template, resourceType);
                }
            }
        }

        private void CacheResourceDeclaration(ResourceDeclarationSyntax resourceDeclarationSyntax, string type, string template)
        {
            if (!resourceTypeToBodyMap.ContainsKey(type))
            {
                TextSpan bodySpan = resourceDeclarationSyntax.Value.Span;
                string bodyText = template.Substring(bodySpan.Position, bodySpan.Length);

                resourceTypeToBodyMap.Add(type, bodyText);
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

                resourceTypeToDependentsMap.Add(resourceType, sb.ToString());
            }
        }

        private (ResourceDeclarationSyntax?, string?) GetResourceDeclarationSyntaxWithType(DeclaredSymbol declaredSymbol)
        {
            if (declaredSymbol.DeclaringSyntax is ResourceDeclarationSyntax resourceDeclarationSyntax &&
                resourceDeclarationSyntax.TypeString is StringSyntax stringSyntax)
            {
                return (resourceDeclarationSyntax, stringSyntax.StringTokens.First().Text);
            }

            return (null, null);
        }

        private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(string template, string manifestResourceName)
        {
            string path = Path.GetFullPath(manifestResourceName);
            SyntaxTree syntaxTree = SyntaxTree.Create(new Uri(path), template);
            SyntaxTreeGrouping syntaxTreeGrouping = new SyntaxTreeGrouping(
                syntaxTree,
                ImmutableHashSet.Create(syntaxTree),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, SyntaxTree>(),
                ImmutableDictionary.Create<ModuleDeclarationSyntax, DiagnosticBuilder.ErrorBuilderDelegate>());

            Compilation compilation = new Compilation(new AzResourceTypeProvider(), syntaxTreeGrouping);
            SemanticModel semanticModel = compilation.GetEntrypointSemanticModel();

            return ResourceDependencyVisitor.GetResourceDependencies(semanticModel);
        }

        public Snippet? GetResourceBodyCompletionSnippet(string type)
        {
            Snippet? snippet = topLevelNamedDeclarationSnippets.Where(x => x.Text.Contains(type)).FirstOrDefault();

            if (snippet is null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();

            if (resourceTypeToBodyMap.TryGetValue(type, out string? resourceBody))
            {
                sb.AppendLine(resourceBody);

                if (resourceTypeToDependentsMap.TryGetValue(type, out string? resourceDependencies))
                {
                    sb.Append(resourceDependencies);
                }

                return new Snippet(sb.ToString(), snippet.CompletionPriority, snippet.Prefix, snippet.Detail);
            }

            return null;
        }
    }
}
