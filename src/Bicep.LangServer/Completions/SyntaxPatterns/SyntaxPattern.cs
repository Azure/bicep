// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Bicep.LanguageServer.Completions.SyntaxPatterns
{
    public class SyntaxPattern
    {
        // Pattern nodes: ancestors + left siblings + overlapping node.
        private readonly ImmutableArray<SyntaxBase?> patternNodes;

        private SyntaxPattern(ImmutableArray<SyntaxBase?> patternNodes)
        {
            this.patternNodes = patternNodes;
        }

        public static SyntaxPattern Create(char cursor, string textWithCursor) =>
            Create(parser => parser.Program(), cursor, textWithCursor);

        public static SyntaxPattern Create(Func<Parser, SyntaxBase> syntaxFactory, char cursor, string textWithCursor)
        {
            var (text, offset) = ProcessTextWithCursor(cursor, textWithCursor);

            var parser = new Parser(text);
            var syntax = syntaxFactory(parser);
            var patternNodes = GetPatternNodes(syntax, offset);

            return new(patternNodes);
        }

        public static SyntaxPattern Create(SyntaxBase syntax, int offset)
        {
            var patternNodes = GetPatternNodes(syntax, offset);

            return new(patternNodes);
        }

        public bool TailMatch(SyntaxPattern other)
        {
            if (this.patternNodes.Length > other.patternNodes.Length)
            {
                return false;
            }

            var tailNodes = other.patternNodes.Skip(other.patternNodes.Length - patternNodes.Length);

            return this.patternNodes.SequenceEqual(tailNodes, new SyntaxTypeComparer());
        }

        public bool TailMatch(ImmutableArray<SyntaxBase?> nodes)
        {
            if (this.patternNodes.Length > nodes.Length)
            {
                return false;
            }

            var tailNodes = nodes.Skip(nodes.Length - patternNodes.Length);

            return tailNodes.SequenceEqual(this.patternNodes, new SyntaxTypeComparer());
        }

        private static ImmutableArray<SyntaxBase?> GetPatternNodes(SyntaxBase syntax, int offset)
        {
            var ancestors = AncestorsCollector.CollectAncestors(syntax, offset);
            var (leftSiblings, overlappingNode) = LeftSiblingsCollector.CollectLeftSiblings(ancestors[^1], offset);

            // pattern nodes = ancestors + left siblings + overlapping node.
            return ancestors.Concat(leftSiblings).Append(overlappingNode).ToImmutableArray();
        }

        private static (string text, int offset) ProcessTextWithCursor(char cursor, string textWithCursor)
        {
            var text = textWithCursor.Replace(cursor.ToString(), "");
            var offsets = new List<int>();

            for (var i = 0; i < textWithCursor.Length; i++)
            {
                if (textWithCursor[i] == cursor)
                {
                    offsets.Add(i - offsets.Count);
                }
            }

            return (text, offsets.Single());
        }
    }
}
