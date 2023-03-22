// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;

namespace Bicep.Core.TypeSystem;

internal class TypeComparer : IComparer<TypeSymbol>
{
    public int Compare(TypeSymbol? a, TypeSymbol? b) => (a, b) switch
    {
        (ArrayType arrayA, ArrayType arrayB) when arrayA.Name == arrayB.Name => arrayA.MinLength == arrayB.MinLength
            ? CompareRefinements(arrayA.MaxLength, arrayB.MaxLength)
            : CompareRefinements(arrayA.MinLength, arrayB.MinLength),
        (IntegerType intA, IntegerType intB) => intA.MinValue == intB.MinValue
            ? CompareRefinements(intA.MaxValue, intB.MaxValue)
            : CompareRefinements(intA.MinValue, intB.MinValue),
        (StringType stringA, StringType stringB) => stringA.MinLength == stringB.MinLength
            ? CompareRefinements(stringA.MaxLength, stringB.MaxLength)
            : CompareRefinements(stringA.MinLength, stringB.MinLength),
        _ => StringComparer.Ordinal.Compare(a?.Name, b?.Name),
    };

    private int CompareRefinements(long? a, long? b) => (a, b) switch
    {
        (long, null) => -1,
        (null, long) => 1,
        (long nonNullA, long nonNullB) => (nonNullA - nonNullB) switch
        {
            < 0 => -1,
            > 0 => 1,
            _ => 0,
        },
        _ => 0,
    };
}
