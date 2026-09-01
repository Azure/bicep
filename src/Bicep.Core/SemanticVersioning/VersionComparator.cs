// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Semver;

namespace Bicep.Core.SemanticVersioning;

/// <summary>
/// A single component of a version constraint, A bare version such as "1.2.0"
/// is equivalent to "=1.2.0".
/// </summary>
public sealed record VersionComparator(VersionComparatorOperator Operator, SemVersion Version)
{
    /// <summary>
    /// The version styles accepted within a constraint. A leading "v" is allowed and the minor and patch components may
    /// be omitted (">=1.2" is equivalent to ">=1.2.0", and ">=1" to ">=1.0.0").
    /// </summary>
    private const SemVersionStyles VersionStyles = SemVersionStyles.AllowV | SemVersionStyles.OptionalMinorPatch;

    /// <summary>
    /// The recognized operator tokens. Two-character tokens must be tested first so that ">=" is not
    /// mistaken for ">" followed by a version starting with "=".
    /// </summary>
    private static readonly ImmutableArray<(string Token, VersionComparatorOperator Operator)> OperatorTokens =
    [
        (">=", VersionComparatorOperator.GreaterThanOrEqual),
        ("<=", VersionComparatorOperator.LessThanOrEqual),
        (">", VersionComparatorOperator.GreaterThan),
        ("<", VersionComparatorOperator.LessThan),
        ("=", VersionComparatorOperator.Equal),
    ];

    /// <summary>
    /// Whether this comparator constrains how low a version may go.
    /// </summary>
    public bool IsLowerBound => this.Operator is VersionComparatorOperator.GreaterThan or VersionComparatorOperator.GreaterThanOrEqual;

    /// <summary>
    /// Whether this comparator constrains how high a version may go.
    /// </summary>
    public bool IsUpperBound => this.Operator is VersionComparatorOperator.LessThan or VersionComparatorOperator.LessThanOrEqual;

    /// <summary>
    /// Whether the bound itself is an accepted version.
    /// </summary>
    public bool IsInclusive => this.Operator is not (VersionComparatorOperator.GreaterThan or VersionComparatorOperator.LessThan);

    /// <summary>
    /// Determines whether the supplied version satisfies this comparator. Comparison follows semantic version precedence.
    /// </summary>
    public bool Satisfies(SemVersion version)
    {
        var comparison = version.ComparePrecedenceTo(this.Version);

        return this.Operator switch
        {
            VersionComparatorOperator.Equal => comparison == 0,
            VersionComparatorOperator.GreaterThan => comparison > 0,
            VersionComparatorOperator.GreaterThanOrEqual => comparison >= 0,
            VersionComparatorOperator.LessThan => comparison < 0,
            VersionComparatorOperator.LessThanOrEqual => comparison <= 0,
            _ => throw new UnreachableException($"Unrecognized {nameof(VersionComparatorOperator)}: {this.Operator}."),
        };
    }

    public static bool TryParse(string value, [NotNullWhen(true)] out VersionComparator? result)
    {
        var remainder = value.Trim();
        var @operator = VersionComparatorOperator.Equal;

        foreach (var (token, candidate) in OperatorTokens)
        {
            if (remainder.StartsWith(token, StringComparison.Ordinal))
            {
                @operator = candidate;
                remainder = remainder[token.Length..].TrimStart();
                break;
            }
        }

        if (!SemVersion.TryParse(remainder, VersionStyles, out var version))
        {
            result = null;
            return false;
        }

        result = new VersionComparator(@operator, version);
        return true;
    }

    public override string ToString() => this.Operator switch
    {
        // An exact constraint is rendered without an operator, matching the syntax users are expected to write.
        VersionComparatorOperator.Equal => this.Version.ToString(),
        VersionComparatorOperator.GreaterThan => $">{this.Version}",
        VersionComparatorOperator.GreaterThanOrEqual => $">={this.Version}",
        VersionComparatorOperator.LessThan => $"<{this.Version}",
        VersionComparatorOperator.LessThanOrEqual => $"<={this.Version}",
        _ => throw new UnreachableException($"Unrecognized {nameof(VersionComparatorOperator)}: {this.Operator}."),
    };
}
