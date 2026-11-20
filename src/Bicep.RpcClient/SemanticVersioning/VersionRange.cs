// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.RpcClient.SemanticVersioning;

/// <summary>
/// An exact Bicep version or a range with at most one lower and one upper bound.
/// </summary>
internal sealed class VersionRange
{
    private VersionRange(VersionComparator? lowerBound, VersionComparator? upperBound)
    {
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public VersionComparator? LowerBound { get; }

    public VersionComparator? UpperBound { get; }

    public bool IsExactVersion
        => LowerBound is { IsInclusive: true } lower &&
           UpperBound is { IsInclusive: true } upper &&
           lower.Version == upper.Version;

    public bool IsSatisfiable
    {
        get
        {
            if (LowerBound is not { } lower || UpperBound is not { } upper)
            {
                return true;
            }

            var comparison = lower.Version.CompareTo(upper.Version);
            if (comparison == 0)
            {
                return lower.IsInclusive && upper.IsInclusive;
            }

            // No release can lie strictly between adjacent patch versions.
            var adjacentExclusiveBounds = !lower.IsInclusive &&
                !upper.IsInclusive &&
                lower.Version.Major == upper.Version.Major &&
                lower.Version.Minor == upper.Version.Minor &&
                (long)lower.Version.Patch + 1 == upper.Version.Patch;

            return comparison < 0 && !adjacentExclusiveBounds;
        }
    }

    public bool IsSatisfiedBy(SemanticVersion version)
        => (LowerBound?.IsSatisfiedBy(version) ?? true) &&
           (UpperBound?.IsSatisfiedBy(version) ?? true);

    public static VersionRange Parse(string value)
        => TryParse(value, out var result) && result is not null
            ? result
            : throw new FormatException($"The provided value '{value}' is not a valid version constraint.");

    public static bool TryParse(string value, out VersionRange? result)
    {
        result = null;
        var parts = value.Split(',');
        if (parts.Length > 2)
        {
            return false;
        }

        VersionComparator? lowerBound = null;
        VersionComparator? upperBound = null;
        foreach (var part in parts)
        {
            if (!VersionComparator.TryParse(part, out var comparator) || comparator is null)
            {
                return false;
            }

            if (comparator.Operator == VersionComparatorOperator.Equal)
            {
                if (lowerBound is not null || upperBound is not null)
                {
                    return false;
                }

                lowerBound = comparator with { Operator = VersionComparatorOperator.GreaterThanOrEqual };
                upperBound = comparator with { Operator = VersionComparatorOperator.LessThanOrEqual };
            }
            else if (comparator.IsLowerBound)
            {
                if (lowerBound is not null)
                {
                    return false;
                }

                lowerBound = comparator;
            }
            else
            {
                if (upperBound is not null)
                {
                    return false;
                }

                upperBound = comparator;
            }
        }

        result = new(lowerBound, upperBound);
        return true;
    }

    public override string ToString()
        => IsExactVersion && LowerBound is { } exact
            ? exact.Version.ToString()
            : string.Join(", ", new[] { LowerBound, UpperBound }.OfType<VersionComparator>());
}
