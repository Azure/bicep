// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
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
    }
}
