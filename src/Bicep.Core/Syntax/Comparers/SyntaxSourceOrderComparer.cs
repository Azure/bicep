// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Bicep.Core.Navigation;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax.Visitors;

namespace Bicep.Core.Syntax.Comparers;

public class SyntaxSourceOrderComparer : IComparer<SyntaxBase?>
{
    private SyntaxSourceOrderComparer()
    {
    }

    public static readonly IComparer<SyntaxBase?> Instance = new SyntaxSourceOrderComparer();

    int IComparer<SyntaxBase?>.Compare(SyntaxBase? x, SyntaxBase? y)
    {
        if (x is null)
        {
            return y is null ? 0 : int.MaxValue;
        }

        if (y is null)
        {
            return int.MinValue;
        }

        return x.Span.Position - y.Span.Position;
    }
}
