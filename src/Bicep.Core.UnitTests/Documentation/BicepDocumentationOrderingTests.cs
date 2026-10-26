// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Documentation;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Documentation;

[TestClass]
public class BicepDocumentationOrderingTests
{
    [TestMethod]
    public void SortByName_MixedCaseNames_OrdersCaseInsensitivelyWithOrdinalTieBreak()
    {
        var names = new[] { "zeta", "Alpha", "beta", "alpha" };

        var sorted = BicepDocumentationOrdering.SortByName(names, name => name);

        // "Alpha" and "alpha" are a case-insensitive tie, broken by ordinal comparison (uppercase sorts first).
        sorted.Should().Equal("Alpha", "alpha", "beta", "zeta");
    }

    [TestMethod]
    public void SortByName_EmptyInput_ReturnsEmptyArray()
    {
        var sorted = BicepDocumentationOrdering.SortByName(Array.Empty<string>(), name => name);

        sorted.Should().BeEmpty();
    }

    [TestMethod]
    public void NameComparer_CaseInsensitiveDifference_ReturnsNonZeroWithoutOrdinalTieBreak()
    {
        var result = BicepDocumentationOrdering.NameComparer.Compare("alpha", "beta");

        result.Should().BeLessThan(0);
    }

    [TestMethod]
    public void NameComparer_CaseInsensitiveTie_FallsBackToOrdinalComparison()
    {
        // "Alpha" vs "alpha" are equal under OrdinalIgnoreCase, so the ordinal tie-break must run
        // (uppercase 'A' sorts before lowercase 'a' ordinally).
        var result = BicepDocumentationOrdering.NameComparer.Compare("Alpha", "alpha");

        result.Should().BeLessThan(0);
    }

    [TestMethod]
    public void NameComparer_IdenticalNames_ReturnsZero()
    {
        var result = BicepDocumentationOrdering.NameComparer.Compare("alpha", "alpha");

        result.Should().Be(0);
    }
}
