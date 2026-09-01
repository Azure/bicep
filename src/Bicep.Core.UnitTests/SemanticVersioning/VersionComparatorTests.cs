// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.SemanticVersioning;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.SemanticVersioning;

[TestClass]
public class VersionComparatorTests
{
    [DataTestMethod]
    [DataRow("1.2.3", VersionComparatorOperator.Equal, "1.2.3")]
    [DataRow("=1.2.3", VersionComparatorOperator.Equal, "1.2.3")]
    [DataRow(">1.2.3", VersionComparatorOperator.GreaterThan, "1.2.3")]
    [DataRow(">=1.2.3", VersionComparatorOperator.GreaterThanOrEqual, "1.2.3")]
    [DataRow("<1.2.3", VersionComparatorOperator.LessThan, "1.2.3")]
    [DataRow("<=1.2.3", VersionComparatorOperator.LessThanOrEqual, "1.2.3")]
    [DataRow("  >=  1.2.3  ", VersionComparatorOperator.GreaterThanOrEqual, "1.2.3")]
    [DataRow("v1.2.3", VersionComparatorOperator.Equal, "1.2.3")]
    [DataRow(">=V1.2.3", VersionComparatorOperator.GreaterThanOrEqual, "1.2.3")]
    [DataRow(">=1.2", VersionComparatorOperator.GreaterThanOrEqual, "1.2.0")]
    [DataRow(">=1", VersionComparatorOperator.GreaterThanOrEqual, "1.0.0")]
    [DataRow("1.2.3-preview", VersionComparatorOperator.Equal, "1.2.3-preview")]
    [DataRow("1.2.3+abc123", VersionComparatorOperator.Equal, "1.2.3+abc123")]
    public void TryParse_ValidComparator_ReturnsTrueAndParsedComparator(string value, VersionComparatorOperator expectedOperator, string expectedVersion)
    {
        var result = VersionComparator.TryParse(value, out var comparator);

        result.Should().BeTrue();
        comparator.Should().NotBeNull();
        comparator!.Operator.Should().Be(expectedOperator);
        comparator.Version.ToString().Should().Be(expectedVersion);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow(">=")]
    [DataRow(">")]
    [DataRow("=")]
    [DataRow("whatever")]
    [DataRow("1.2.3.4")]
    [DataRow("01.2.3")]
    [DataRow("^1.2.3")]
    [DataRow("~1.2.3")]
    [DataRow("*")]
    [DataRow("=>1.2.3")]
    [DataRow("(>=1.2.3)")]
    public void TryParse_InvalidComparator_ReturnsFalseAndNull(string value)
    {
        var result = VersionComparator.TryParse(value, out var comparator);

        result.Should().BeFalse();
        comparator.Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("1.2.3", false, false, true)]
    [DataRow(">1.2.3", true, false, false)]
    [DataRow(">=1.2.3", true, false, true)]
    [DataRow("<1.2.3", false, true, false)]
    [DataRow("<=1.2.3", false, true, true)]
    public void BoundProperties_ReturnExpectedValues(string value, bool isLowerBound, bool isUpperBound, bool isInclusive)
    {
        var comparator = ParseComparator(value);

        comparator.IsLowerBound.Should().Be(isLowerBound);
        comparator.IsUpperBound.Should().Be(isUpperBound);
        comparator.IsInclusive.Should().Be(isInclusive);
    }

    [DataTestMethod]
    [DataRow("1.2.3", "1.2.3")]
    [DataRow("=1.2.3", "1.2.3")]
    [DataRow("v1.2.3", "1.2.3")]
    [DataRow(">=1.2", ">=1.2.0")]
    [DataRow("  <  1.2.3", "<1.2.3")]
    public void ToString_NormalizesTheComparator(string value, string expected)
    {
        ParseComparator(value).ToString().Should().Be(expected);
    }

    private static VersionComparator ParseComparator(string value)
    {
        VersionComparator.TryParse(value, out var comparator).Should().BeTrue();

        return comparator!;
    }
}
