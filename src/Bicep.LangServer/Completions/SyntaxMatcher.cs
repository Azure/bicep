// Copyright (c) Microsoft Corporation.
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
        public static bool IsTailMatch<T1>(IList<SyntaxBase> nodes, Func<T1, bool>? predicate = null)
            where T1 : SyntaxBase
        {
            return nodes.Count >= 1 &&
                   nodes[^1] is T1 one &&
                   (predicate is null || predicate.Invoke(one));
        }

        public static bool IsTailMatch<T1, T2>(IList<SyntaxBase> nodes, Func<T1, T2, bool>? predicate = null)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
        {
            return nodes.Count >= 2 &&
                   nodes[^2] is T1 one &&
                   nodes[^1] is T2 two &&
                   (predicate is null || predicate(one, two));
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

        public static bool IsTailMatch<T1, T2, T3, T4>(IList<SyntaxBase> nodes, Func<T1, T2, T3, T4, bool> predicate, Action<T1, T2, T3, T4>? actionOnMatch = null)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
            where T4 : SyntaxBase
        {
            if (nodes.Count >= 4 &&
                   nodes[^4] is T1 one &&
                   nodes[^3] is T2 two &&
                   nodes[^2] is T3 three &&
                   nodes[^1] is T4 four &&
                   predicate(one, two, three, four))
            {
                actionOnMatch?.Invoke(one, two, three, four);
                return true;
            }

            return false;
        }

        public static bool IsTailMatch<T1, T2, T3, T4, T5>(IList<SyntaxBase> nodes, Func<T1, T2, T3, T4, T5, bool> predicate)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
            where T4 : SyntaxBase
            where T5 : SyntaxBase
        {
            return nodes.Count >= 5 &&
                   nodes[^5] is T1 one &&
                   nodes[^4] is T2 two &&
                   nodes[^3] is T3 three &&
                   nodes[^2] is T4 four &&
                   nodes[^1] is T5 five &&
                   predicate(one, two, three, four, five);
        }

        public static bool IsTailMatch<T1, T2, T3, T4, T5, T6>(IList<SyntaxBase> nodes, Func<T1, T2, T3, T4, T5, T6, bool> predicate)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
            where T4 : SyntaxBase
            where T5 : SyntaxBase
            where T6 : SyntaxBase
        {
            return nodes.Count >= 6 &&
                   nodes[^6] is T1 one &&
                   nodes[^5] is T2 two &&
                   nodes[^4] is T3 three &&
                   nodes[^3] is T4 four &&
                   nodes[^2] is T5 five &&
                   nodes[^1] is T6 six &&
                   predicate(one, two, three, four, five, six);
        }

        public static bool IsTailMatch<T1, T2, T3, T4, T5, T6, T7>(IList<SyntaxBase> nodes, Func<T1, T2, T3, T4, T5, T6, T7, bool> predicate)
            where T1 : SyntaxBase
            where T2 : SyntaxBase
            where T3 : SyntaxBase
            where T4 : SyntaxBase
            where T5 : SyntaxBase
            where T6 : SyntaxBase
            where T7 : SyntaxBase
        {
            return nodes.Count >= 7 &&
                   nodes[^7] is T1 one &&
                   nodes[^6] is T2 two &&
                   nodes[^5] is T3 three &&
                   nodes[^4] is T4 four &&
                   nodes[^3] is T5 five &&
                   nodes[^2] is T6 six &&
                   nodes[^1] is T7 seven &&
                   predicate(one, two, three, four, five, six, seven);
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
                // we will pick the node to the left as the winner if it's not a newline. Otherwise, pick the right one instead.
                if (!nodes.Any())
                {
                    nodes.Add(current);
                }
                else
                {
                    var lastNode = nodes[^1];

                    if (TextSpan.AreNeighbors(lastNode, current) && lastNode is Token { Type: TokenType.NewLine })
                    {
                        nodes[^1] = current;
                    }
                    else if (!TextSpan.AreNeighbors(lastNode, current))
                    {
                        nodes.Add(current);
                    }
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
