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
using Bicep.Core.TypeSystem;
using Bicep.Core.TypeSystem.Az;
using Bicep.LanguageServer.Completions;

namespace Bicep.LanguageServer.Snippets
{
    public class SnippetsProvider : ISnippetsProvider
    {
        // Used to cache resource declarations. Maps resource type to body text
        private Dictionary<string, string> resourceTypeToBodyMap = new Dictionary<string, string>();
        // Used to cache resource dependencies. Maps resource type to it's dependencies
        private Dictionary<string, string> resourceTypeToDependentsMap = new Dictionary<string, string>();
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

        private ImmutableDictionary<DeclaredSymbol, ImmutableHashSet<ResourceDependency>> GetResourceDependencies(string template, string manifestResourceName)
        {
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

        public Snippet GetResourceBodyCompletionSnippet(TypeSymbol typeSymbol)
        {
            string label = "{}";
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

                return new Snippet(sb.ToString(), CompletionPriority.Medium, label, label);
            }

            // Get resource body completion snippet from swagger spec
            return GetResourceBodyCompletionSnippetFromSwaggerSpec(typeSymbol, label);
        }

        private Snippet GetResourceBodyCompletionSnippetFromSwaggerSpec(TypeSymbol typeSymbol, string label)
        {
            if (typeSymbol is ResourceType resourceType && resourceType.Body is ObjectType objectType)
            {
                int index = 1;
                string? snippetText = null;

                foreach (KeyValuePair<string, TypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                {
                    snippetText = GetSnippetText(kvp.Value, snippetText, ref index);
                }

                if (snippetText is not null)
                {
                    // Define final tab stop
                    snippetText += "\t$0\n}";

                    // Add to cache
                    resourceTypeToBodyMap.Add(typeSymbol.Name, snippetText);

                    return new Snippet(snippetText, CompletionPriority.Medium, label, label);
                }
            }

            return new Snippet("{\n\t$0\n}", CompletionPriority.Medium, label, label);
        }

        private string? GetSnippetText(TypeProperty typeProperty, string? snippetText, ref int index)
        {
            if (typeProperty.Flags.HasFlag(TypePropertyFlags.Required))
            {
                if (snippetText is null)
                {
                    snippetText = "{\n";
                }

                if (typeProperty.TypeReference.Type is ObjectType objectType)
                {
                    snippetText += "\t" + typeProperty.Name + ": {\n";

                    foreach (KeyValuePair<string, TypeProperty> kvp in objectType.Properties.OrderBy(x => x.Key))
                    {
                        snippetText = GetSnippetText(kvp.Value, snippetText, ref index);
                    }

                    snippetText += "\t}\n";
                }
                else
                {
                    snippetText += "\t" + typeProperty.Name + ": $" + (index).ToString() + "\n";
                    index++;
                }

                return snippetText;
            }

            return snippetText;
        }
    }
}
