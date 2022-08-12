// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Syntax.Visitors;
using System;

namespace Bicep.Core.Syntax
{
    public static class SyntaxLinqExtensions
    {
        /// <summary>
        /// Walks the specified syntax subtree and returns the first node found that matches a given  query
        /// </summary>
        /// <typeparam name="TSyntaxType">[optional] The type of tree node that is being searched for and returned</typeparam>
        /// <param name="source">The starting tree node</param>
        /// <param name="query">The function that indicates whether a node is the one being searched for</param>
        /// <returns>The first matching node found</returns>
        public static TSyntaxType? FirstOrDefault<TSyntaxType>(
            this SyntaxBase source,
            Func<SyntaxBase, bool> query)
        where TSyntaxType : SyntaxBase
        {
            return SyntaxAggregator.Aggregate<TSyntaxType?, TSyntaxType?>(
                source,
                seed: null,
                function: (found, syntax) => syntax is TSyntaxType tResult && query(tResult) ? tResult : null,
                resultSelector: (found) => found,
                continuationFunction: (found, _) => found is null);
        }

        /// <summary>
        /// Walks the specified syntax subtree and returns the first node found that matches a given query
        /// </summary>
        /// <param name="source">The starting tree node</param>
        /// <param name="query">The function that indicates whether a node is the one being searched for</param>
        /// <returns>The first matching node found</returns>
        public static SyntaxBase? FirstOrDefault(
            this SyntaxBase source,
            Func<SyntaxBase, bool> query)
        {
            return source.FirstOrDefault<SyntaxBase>(query);
        }

        /// <summary>
        /// Walks the specified syntax subtree and returns true only if any node is found that matches a given query
        /// </summary>
        /// <param name="source">The starting tree node</param>
        /// <param name="query">The function that indicates whether a node one being searched for</param>
        /// <returns>True if any matching node is found</returns>
        public static bool Any(
            this SyntaxBase source,
            Func<SyntaxBase, bool> query)
        {
            return source.FirstOrDefault(query) is not null;
        }
    }
}
