// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.RpcClient.SemanticVersioning;

/// <summary>
/// One comparison operator and version. A bare version is an equality comparison.
/// </summary>
internal sealed record VersionComparator(VersionComparatorOperator Operator, SemanticVersion Version)
{
    private static readonly ImmutableArray<(string Token, VersionComparatorOperator Operator)> OperatorTokens =
    [
        (">=", VersionComparatorOperator.GreaterThanOrEqual),
        ("<=", VersionComparatorOperator.LessThanOrEqual),
        (">", VersionComparatorOperator.GreaterThan),
        ("<", VersionComparatorOperator.LessThan),
        ("=", VersionComparatorOperator.Equal),
    ];

    public bool IsLowerBound => Operator is VersionComparatorOperator.GreaterThan or VersionComparatorOperator.GreaterThanOrEqual;

    public bool IsUpperBound => Operator is VersionComparatorOperator.LessThan or VersionComparatorOperator.LessThanOrEqual;

    public bool IsInclusive => Operator is not (VersionComparatorOperator.GreaterThan or VersionComparatorOperator.LessThan);

    public bool IsSatisfiedBy(SemanticVersion version)
    {
        var comparison = version.CompareTo(Version);
        return Operator switch
        {
            VersionComparatorOperator.Equal => comparison == 0,
            VersionComparatorOperator.GreaterThan => comparison > 0,
            VersionComparatorOperator.GreaterThanOrEqual => comparison >= 0,
            VersionComparatorOperator.LessThan => comparison < 0,
            VersionComparatorOperator.LessThanOrEqual => comparison <= 0,
            _ => throw new InvalidOperationException($"Unrecognized version comparator operator: {Operator}."),
        };
    }

    public static bool TryParse(string value, out VersionComparator? result)
    {
        result = null;
        var remainder = value.Trim();
        var @operator = VersionComparatorOperator.Equal;
        foreach (var (token, candidate) in OperatorTokens)
        {
            if (remainder.StartsWith(token, StringComparison.Ordinal))
            {
                @operator = candidate;
                remainder = remainder.Substring(token.Length).TrimStart();
                break;
            }
        }

        if (!SemanticVersion.TryParse(remainder, out var version) || version is null)
        {
            return false;
        }

        result = new(@operator, version);
        return true;
    }

    public override string ToString() => Operator switch
    {
        VersionComparatorOperator.Equal => Version.ToString(),
        VersionComparatorOperator.GreaterThan => $">{Version}",
        VersionComparatorOperator.GreaterThanOrEqual => $">={Version}",
        VersionComparatorOperator.LessThan => $"<{Version}",
        VersionComparatorOperator.LessThanOrEqual => $"<={Version}",
        _ => throw new InvalidOperationException($"Unrecognized version comparator operator: {Operator}."),
    };
}
