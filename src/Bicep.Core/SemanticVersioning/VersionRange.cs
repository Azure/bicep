// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Semver;

namespace Bicep.Core.SemanticVersioning;

/// <summary>
/// A version constraint, expressed either as an exact version ("1.2.3") or as a range of up to two
/// comparators separated by a comma ("&gt;=1.2.3, &lt;2.0.0"). Whitespace around comparators is ignored.
/// </summary>
public sealed class VersionRange
{
    /// <summary>
    /// A range may pair at most one lower bound with one upper bound.
    /// </summary>
    public const int MaxComparatorCount = 2;

    private const char ComparatorSeparator = ',';

    private VersionRange(ImmutableArray<VersionComparator> comparators)
    {
        this.Comparators = comparators;
    }

    /// <summary>
    /// The comparators making up this range. When two are present, the lower bound is always first.
    /// </summary>
    public ImmutableArray<VersionComparator> Comparators { get; }

    public VersionComparator? LowerBound => this.Comparators.FirstOrDefault(comparator => comparator.IsLowerBound);

    public VersionComparator? UpperBound => this.Comparators.FirstOrDefault(comparator => comparator.IsUpperBound);

    /// <summary>
    /// Whether any version at all can satisfy this range. A range such as "&gt;=2.0.0, &lt;1.0.0" parses
    /// successfully but can never be satisfied.
    /// </summary>
    public bool IsSatisfiable
    {
        get
        {
            if (this.LowerBound is not { } lower || this.UpperBound is not { } upper)
            {
                return true;
            }

            return lower.Version.ComparePrecedenceTo(upper.Version) switch
            {
                < 0 => true,
                // The bounds coincide, so the range describes a single version only if both ends include it.
                0 => lower.IsInclusive && upper.IsInclusive,
                _ => false,
            };
        }
    }

    /// <summary>
    /// Determines whether the supplied version satisfies every comparator in this range.
    /// </summary>
    public bool Satisfies(SemVersion version) => this.Comparators.All(comparator => comparator.Satisfies(version));

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

        var comparators = new VersionComparator[parts.Length];

        for (var i = 0; i < parts.Length; i++)
        {
            if (!VersionComparator.TryParse(parts[i], out var comparator))
            {
                return false;
            }

            comparators[i] = comparator;
        }

        if (comparators.Length == MaxComparatorCount)
        {
            var (first, second) = (comparators[0], comparators[1]);

            // A two-part range must pair one lower bound with one upper bound. This rejects redundant ranges
            // such as ">=1.0.0, >=2.0.0" as well as any range combining an exact version with a bound.
            if (first.IsUpperBound && second.IsLowerBound)
            {
                // Normalize so that the lower bound always comes first.
                comparators = [second, first];
            }
            else if (!first.IsLowerBound || !second.IsUpperBound)
            {
                return false;
            }
        }

        result = new VersionRange([.. comparators]);
        return true;
    }

    public override string ToString() => string.Join($"{ComparatorSeparator} ", this.Comparators);
}
