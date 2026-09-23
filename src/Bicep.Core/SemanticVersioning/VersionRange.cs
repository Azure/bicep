// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;

namespace Bicep.Core.SemanticVersioning;

/// <summary>
/// A version constraint, expressed either as an exact version ("1.2.3") or as a range pairing a lower bound with an
/// upper bound . Whitespace around comparators is ignored.
/// </summary>
public sealed class VersionRange
{
    /// <summary>
    /// A range may pair at most one lower bound with one upper bound.
    /// </summary>
    private const int MaxComparatorCount = 2;

    private const char ComparatorSeparator = ',';

    private VersionRange(VersionComparator? lowerBound, VersionComparator? upperBound)
    {
        this.LowerBound = lowerBound;
        this.UpperBound = upperBound;
    }

    /// <summary>
    /// The lower end of the range, if the constraint has one.
    /// </summary>
    public VersionComparator? LowerBound { get; }

    /// <summary>
    /// The upper end of the range, if the constraint has one.
    /// </summary>
    public VersionComparator? UpperBound { get; }

    /// <summary>
    /// Whether this constraint accepts a single version only. An exact version is held as a pair of inclusive bounds.
    /// </summary>
    public bool IsExactVersion
        => this.LowerBound is { IsInclusive: true } lower &&
           this.UpperBound is { IsInclusive: true } upper &&
           lower.Version == upper.Version;

    /// <summary>
    /// Whether the bounds are ordered such that a version could fall between them. A range such as
    /// "&gt;=2.0.0, &lt;1.0.0" parses successfully but can never be satisfied. Because versions are discrete
    /// (there is no version between "1.2.3" and "1.2.4"), a range such as "&gt;1.2.3, &lt;1.2.4" is also
    /// unsatisfiable even though its bounds are correctly ordered.
    /// </summary>
    public bool IsSatisfiable
    {
        get
        {
            if (this.LowerBound is not { } lower || this.UpperBound is not { } upper)
            {
                return true;
            }

            return lower.Version.CompareTo(upper.Version) switch
            {
                < 0 => !AreAdjacentExclusiveBounds(lower, upper),
                // The bounds coincide, so the range describes a single version only if both ends include it.
                0 => lower.IsInclusive && upper.IsInclusive,
                _ => false,
            };
        }
    }

    /// <summary>
    /// Whether the lower and upper bounds are both exclusive and exactly one patch version apart, e.g.
    /// "&gt;1.2.3, &lt;1.2.4". No version can satisfy such a pair, since patch numbers are whole integers and
    /// there is nothing strictly between them. The comparison uses <see langword="long"/> so that a patch value
    /// of <see cref="int.MaxValue"/> cannot overflow and mask a genuine adjacency.
    /// </summary>
    private static bool AreAdjacentExclusiveBounds(VersionComparator lower, VersionComparator upper)
        => !lower.IsInclusive &&
           !upper.IsInclusive &&
           lower.Version.Major == upper.Version.Major &&
           lower.Version.Minor == upper.Version.Minor &&
           (long)lower.Version.Patch + 1 == upper.Version.Patch;

    /// <summary>
    /// Determines whether the supplied version satisfies both ends of this range.
    /// </summary>
    public bool IsSatisfiedBy(SemanticVersion version)
        => (this.LowerBound?.IsSatisfiedBy(version) ?? true) &&
           (this.UpperBound?.IsSatisfiedBy(version) ?? true);

    /// <summary>
    /// Compute the range of versions that satisfy both this range and <paramref name="other"/>, by
    /// combining whichever lower bound is more restrictive with whichever upper bound is more restrictive. Returns
    /// <see langword="false"/> if the two ranges are disjoint, i.e. no version can satisfy both.
    /// </summary>
    public bool TryGetOverlap(VersionRange other, [NotNullWhen(true)] out VersionRange? overlap)
    {
        var candidate = new VersionRange(
            StricterLowerBound(this.LowerBound, other.LowerBound),
            StricterUpperBound(this.UpperBound, other.UpperBound));

        if (candidate.IsSatisfiable)
        {
            overlap = candidate;
            return true;
        }

        overlap = null;
        return false;
    }

    /// <summary>
    /// Determines whether every version that satisfies this range also satisfies <paramref name="other"/>. When
    /// this is <see langword="false"/>, this range is "looser" than <paramref name="other"/>: it admits at least one
    /// version that <paramref name="other"/> would reject.
    /// </summary>
    public bool IsSubsetOf(VersionRange other)
        => StricterLowerBound(this.LowerBound, other.LowerBound) == this.LowerBound &&
           StricterUpperBound(this.UpperBound, other.UpperBound) == this.UpperBound;

    /// <summary>
    /// Returns whichever lower bound admits fewer versions. A missing bound is treated as unbounded, i.e. the least
    /// restrictive option. When both bounds sit at the same version, the exclusive comparator is stricter.
    /// </summary>
    private static VersionComparator? StricterLowerBound(VersionComparator? a, VersionComparator? b)
    {
        if (a is null)
        {
            return b;
        }

        if (b is null)
        {
            return a;
        }

        return a.Version.CompareTo(b.Version) switch
        {
            > 0 => a,
            < 0 => b,
            _ => a.IsInclusive ? b : a,
        };
    }

    /// <summary>
    /// Returns whichever upper bound admits fewer versions. A missing bound is treated as unbounded, i.e. the least
    /// restrictive option. When both bounds sit at the same version, the exclusive comparator is stricter.
    /// </summary>
    private static VersionComparator? StricterUpperBound(VersionComparator? a, VersionComparator? b)
    {
        if (a is null)
        {
            return b;
        }

        if (b is null)
        {
            return a;
        }

        return a.Version.CompareTo(b.Version) switch
        {
            < 0 => a,
            > 0 => b,
            _ => a.IsInclusive ? b : a,
        };
    }

    public static VersionRange Parse(string value)
        => TryParse(value, out var result)
            ? result
            : throw new FormatException($"The provided value '{value}' is not a valid version constraint.");

    public static bool TryParse(string value, [NotNullWhen(true)] out VersionRange? result)
    {
        result = null;

        var parts = value.Split(ComparatorSeparator);

        if (parts.Length > MaxComparatorCount)
        {
            return false;
        }

        VersionComparator? lowerBound = null;
        VersionComparator? upperBound = null;

        // Assigning each comparator to the end of the range it constrains rejects redundant constraints such as
        // ">=1.0.0, >=2.0.0", and makes the order the parts were written in irrelevant.
        foreach (var part in parts)
        {
            if (!VersionComparator.TryParse(part, out var comparator))
            {
                return false;
            }

            if (comparator.Operator is VersionComparatorOperator.Equal)
            {
                // An exact version is the degenerate range that admits only itself, so it occupies both ends. This
                // also rejects any constraint combining an exact version with a bound, such as "=1.0.0, <2.0.0".
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

        result = new VersionRange(lowerBound, upperBound);
        return true;
    }

    /// <summary>
    /// Renders the constraint in its normalized form. Diagnostics should report constraints through this method, so
    /// that an exact version is shown the way a user would write it rather than as a pair of bounds.
    /// </summary>
    public override string ToString()
        => this.IsExactVersion && this.LowerBound is { } exact
            ? exact.Version.ToString()
            : string.Join($"{ComparatorSeparator} ", new[] { this.LowerBound, this.UpperBound }.OfType<VersionComparator>());
}
