// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.LanguageServer.Completions.SyntaxPatterns;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class SyntaxPattern
    {
        private readonly ImmutableArray<SyntaxBase> expectedNodes;

        private SyntaxPattern(ImmutableArray<SyntaxBase> expectedNodes)
        {
            this.expectedNodes = expectedNodes;
        }

        public static SyntaxPattern Create(string textWithCursor) =>
            Create(parser => parser.Program(), textWithCursor);

        public static SyntaxPattern Create(Func<Parser, SyntaxBase> syntaxFactory, string textWithCursor)
        {
            var (text, offset) = ProcessTextWithCursor(textWithCursor);

            var parser = new Parser(text);
            var syntax = syntaxFactory(parser);
            var expectedTypes = GetAncestorsAndLeftSiblings(syntax, offset);

            return new(expectedTypes);
        }

        public static ImmutableArray<SyntaxBase> GetAncestorsAndLeftSiblings(SyntaxBase syntax, int offset)
        {
            var ancestors = AncestorsCollector.CollectAncestors(syntax, offset);
            var leftSiblings = LeftSiblingsCollector.CollectLeftSiblings(ancestors[^1], offset);

            return ancestors.Concat(leftSiblings).ToImmutableArray();
        }

        public bool TailMatch(ImmutableArray<SyntaxBase> nodes)
        {
            if (this.expectedNodes.Length > nodes.Length)
            {
                return false;
            }

            var tailNodes = nodes.Skip(nodes.Length - expectedNodes.Length);

            return tailNodes.SequenceEqual(this.expectedNodes, new SyntaxTypeComparer());
        }

        private static (string text, int offset) ProcessTextWithCursor(string textWithCursor)
        {
            var text = textWithCursor.Replace("|", "");
            var offsets = new List<int>();

            for (var i = 0; i < textWithCursor.Length; i++)
            {
                if (textWithCursor[i] == '|')
                {
                    offsets.Add(i - offsets.Count);
                }
            }

            return (text, offsets.Single());
        }
    }
}
