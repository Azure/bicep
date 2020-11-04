// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Bicep.Core.Syntax;

namespace Bicep.LanguageServer.Completions
{
    public static class SyntaxMatcher
    {
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
