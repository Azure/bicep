// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using Bicep.Core.Syntax.Visitors;
using System.Collections.Generic;
using System.Linq;

namespace Bicep.Core.Syntax.Comparers;

public class SyntaxIgnoringTriviaComparer : IEqualityComparer<SyntaxBase?>
{
    private SyntaxIgnoringTriviaComparer()
    {
    }

    public static readonly IEqualityComparer<SyntaxBase?> Instance = new SyntaxIgnoringTriviaComparer();

    public bool Equals(SyntaxBase? x, SyntaxBase? y)
    {
        if (x is null || y is null)
        {
            return x == y;
        }

        // TODO this can be made more efficient by iterating lazily
        var xTokens = SyntaxAggregator.AggregateByType<Token>(x);
        var yTokens = SyntaxAggregator.AggregateByType<Token>(y);

        return Enumerable.SequenceEqual(xTokens, yTokens, TokenIgnoringTriviaComparer.Instance);
    }

    public int GetHashCode(SyntaxBase obj)
    {
        return SyntaxAggregator.AggregateByType<Token>(obj)
            .Select(TokenIgnoringTriviaComparer.Instance.GetHashCode)
            .Aggregate((a, b) => a ^ b);
    }
}
