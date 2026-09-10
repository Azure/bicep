// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Documentation;

internal static class BicepDocumentationOrdering
{
    public static readonly IComparer<string> NameComparer = Comparer<string>.Create(Compare);

    public static ImmutableArray<T> SortByName<T>(IEnumerable<T> items, Func<T, string> nameSelector) =>
        [.. items.OrderBy(nameSelector, NameComparer)];

    private static int Compare(string left, string right)
    {
        var caseInsensitive = string.Compare(left, right, StringComparison.OrdinalIgnoreCase);

        return caseInsensitive != 0 ? caseInsensitive : string.CompareOrdinal(left, right);
    }
}
