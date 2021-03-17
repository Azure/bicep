// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Snippets
{
    public class ResourceSnippetsProvider : IResourceSnippetsProvider
    {
        private static readonly HashSet<string> _snippetPrefixes = new HashSet<string>()
        {
            "res-aks-cluster",
            "res-app-security-group",
            "res-automation-account",
            "res-availability-set",
            "res-container-group",
            "res-container-registry",
            "res-cosmos-account",
            "res-data-lake",
            "res-dns-zone",
            "res-ip",
            "res-ip-prefix"
        };

        private HashSet<ResourceSnippet> resourceSnippets = new HashSet<ResourceSnippet>();

        public ResourceSnippetsProvider()
        {
            Initialize();
        }

        private void Initialize()
        {
            string? currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            string templatesFolder = Path.Combine(currentDirectory ?? throw new ArgumentNullException("Path is null"),
                                                  "Snippets",
                                                  "Templates");

            foreach (string prefix in _snippetPrefixes)
            {
                string template = Path.Combine(templatesFolder, prefix + ".bicep");

                (string description, string snippetText) = GetDescriptionAndText(File.ReadAllText(template));
                ResourceSnippet resourceSnippet = new ResourceSnippet(prefix, description, snippetText);

                resourceSnippets.Add(resourceSnippet);
            }
        }

        public (string, string) GetDescriptionAndText(string? template)
        {
            string description = string.Empty;
            string text = string.Empty;

            if (!string.IsNullOrWhiteSpace(template))
            {
                Parser parser = new Parser(template);
                ProgramSyntax programSyntax = parser.Program();
                IEnumerable<SyntaxBase> declarations = programSyntax.Declarations;

                if (declarations.Any() && declarations.First() is ResourceDeclarationSyntax resourceDeclarationSyntax)
                {
                    text = template.Substring(resourceDeclarationSyntax.Span.Position);

                    ImmutableArray<SyntaxBase> children = programSyntax.Children;

                    if (children.Length > 0 &&
                        children[0] is Token firstToken &&
                        firstToken is not null &&
                        firstToken.LeadingTrivia[0] is SyntaxTrivia syntaxTrivia &&
                        syntaxTrivia.Type is SyntaxTriviaType.SingleLineComment)
                    {
                        description = syntaxTrivia.Text.Substring("// ".Length);
                    }
                }
            }

            return (description, text);
        }

        public IEnumerable<ResourceSnippet> GetResourceSnippets() => resourceSnippets;
    }
}
