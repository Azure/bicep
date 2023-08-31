// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Parsing;
using System;
using System.Collections.Generic;

namespace Bicep.Core.Syntax.Comparers;

public class TokenIgnoringTriviaComparer : IEqualityComparer<Token>
{
    private TokenIgnoringTriviaComparer()
    {
    }

    public static readonly IEqualityComparer<Token> Instance = new TokenIgnoringTriviaComparer();

    public bool Equals(Token? x, Token? y)
    {
        if (x is null || y is null)
        {
            return x == y;
        }

        return x.Type == y.Type &&
            x.Text == y.Text;
    }

    public int GetHashCode(Token obj)
    {
        var hc = new HashCode();
        hc.Add(obj.Type);
        hc.Add(obj.Text);
        return hc.ToHashCode();
    }
}
