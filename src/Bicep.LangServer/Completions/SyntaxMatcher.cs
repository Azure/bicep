﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions
{
    public static class SyntaxMatcher
    {
        public static bool IsTailMatch<T1>(IList<SyntaxBase> nodes, Func<T1, bool> predicate)
            where T1 : SyntaxBase
        {
            return nodes.Count >= 1 &&
                   nodes[^1] is T1 one &&
                   predicate(one);
        }

        public static bool IsTailMatch<T1, T2>(IList<SyntaxBase> nodes, Func<T1, T2, bool> predicate)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
        {
            return nodes.Count >= 2 &&
                   nodes[^2] is T1 one &&
                   nodes[^1] is T2 two &&
                   predicate(one, two);
        }

        public static bool IsTailMatch<T1, T2, T3>(IList<SyntaxBase> nodes, Func<T1, T2, T3, bool> predicate) 
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
        {
            return nodes.Count >= 3 &&
                   nodes[^3] is T1 one &&
                   nodes[^2] is T2 two &&
                   nodes[^1] is T3 three &&
                   predicate(one, two, three);
        }

        public static bool IsTailMatch<T1, T2, T3, T4>(IList<SyntaxBase> nodes, Func<T1, T2, T3, T4, bool> predicate)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
            where T4 : SyntaxBase
        {
            return nodes.Count >= 4 &&
                   nodes[^4] is T1 one &&
                   nodes[^3] is T2 two &&
                   nodes[^2] is T3 three &&
                   nodes[^1] is T4 four &&
                   predicate(one, two, three, four);
        }

        /// <summary>
        /// Returns nodes whose span contains the specified offset from least specific to the most specific.
        /// </summary>
        /// <param name="syntax">The program node</param>
        /// <param name="offset">The offset</param>
        public static List<SyntaxBase> FindNodesMatchingOffset(ProgramSyntax syntax, int offset)
        {
            var nodes = new List<SyntaxBase>();
            syntax.TryFindMostSpecificNodeInclusive(offset, current =>
            {
                // callback is invoked only if node span contains the offset
                // in inclusive mode, 2 nodes can be returned if cursor is between end of one node and beginning of another
                // we will pick the node to the left as the winner
                if (nodes.Any() == false || TextSpan.AreNeighbors(nodes.Last(), current) == false)
                {
                    nodes.Add(current);
                }

                // don't filter out the nodes
                return true;
            });

            return nodes;
        }

        public static List<SyntaxBase> FindNodesMatchingOffsetExclusive(ProgramSyntax syntax, int offset)
        {
            var nodes = new List<SyntaxBase>();
            syntax.TryFindMostSpecificNodeExclusive(offset, current =>
            {
                nodes.Add(current);
                return true;
            });

            return nodes;
        }

        public static (TResult? node, int index) FindLastNodeOfType<TPredicate, TResult>(List<SyntaxBase> matchingNodes) where TResult : SyntaxBase
        {
            var index = matchingNodes.FindLastIndex(matchingNodes.Count - 1, n => n is TPredicate);
            var node = index < 0 ? null : matchingNodes[index] as TResult;

            return (node, index);
        }
    }
}
