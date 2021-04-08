// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Snippets
{
    public class ResourceSnippetsProvider : IResourceSnippetsProvider
    {
        private HashSet<ResourceSnippet> resourceSnippets = new HashSet<ResourceSnippet>();

        public ResourceSnippetsProvider()
        {
            Initialize();
        }

        private void Initialize()
        {
            string pathPrefix = "Snippets/Templates/";
            Assembly assembly = typeof(ResourceSnippetsProvider).Assembly;
            IEnumerable<string> manifestResourceNames = assembly.GetManifestResourceNames().Where(p => p.StartsWith(pathPrefix, StringComparison.Ordinal));

            foreach (var manifestResourceName in manifestResourceNames)
            {
                Stream? stream = assembly.GetManifestResourceStream(manifestResourceName);
                StreamReader streamReader = new StreamReader(stream ?? throw new ArgumentNullException("Stream is null"), Encoding.Default);

                (string description, string snippetText) = GetDescriptionAndText(streamReader.ReadToEnd());
                string prefix = Path.GetFileNameWithoutExtension(manifestResourceName);
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
