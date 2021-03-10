// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using Bicep.Core.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Text
{
    [TestClass]
    public class SpellCheckerTests
    {
        [DataTestMethod]
        [DataRow(new string[0])]
        public void GetSpellingSuggestion_NullOrEmptyCandidatesEnumerable_ReturnsNull(string[] candidates)
        {
            string? result = SpellChecker.GetSpellingSuggestion("foo", candidates);

            result.Should().BeNull();
        }

        [DataTestMethod]
        public void GetSpellingSuggestion_EmptyCandidate_ReturnsNull()
        {
            var candidates = new [] { "", "" };

            string? result = SpellChecker.GetSpellingSuggestion("foo", candidates);

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetSpellingSuggestion_CandidateHasLessThanThreeCharacters_ReturnsNull()
        {
            var candidates = new [] { "o", "oo", "oO", "OO" };

            string? result = SpellChecker.GetSpellingSuggestion("ooo", candidates);

            result.Should().BeNull();
        }

        [DataTestMethod]
        // maxLengthDifference = 1
        [DataRow("ooo", "ooooo", "oooooo")]
        [DataRow("ooooo", "ooo", "ooooooo")]
        // maxLengthDifference = 2
        [DataRow("oooooo", "ooo", "ooooooooo", "oooooooooooooo")]
        [DataRow("oooooooo", "oooo", "ooooo", "ooooooooooo")]
        public void GetSpellingSuggestion_LengthDifferenceExceedsMax_ReturnsNull(string name, params string[] candidates)
        {
            string? result = SpellChecker.GetSpellingSuggestion(name, candidates);

            result.Should().BeNull();
        }

        [DataTestMethod]
        // maxLengthDifference = 1, maxDistance = 1
        [DataRow("ooo", "ooooo", "oooooo", "oxo", "oOO")]
        [DataRow("ooooo", "ooo", "ooooooo", "oooooo", "oOoOO")]
        // maxLengthDifference = 2, maxDistance = 2
        [DataRow("oooooo", "ooo", "ooooooooo", "ooooox", "OOOOOO")]
        [DataRow("oooooooo", "oooo", "ooooo", "ooooooooooo", "ooooooo", "OoOooOOo")]
        public void GetSpellingSuggestion_CandidateMatchesNameCaseInsensitively_ReturnsCandidate(string name, params string[] candidates)
        {
            string? result = SpellChecker.GetSpellingSuggestion(name, candidates);

            result.Should().Be(candidates[^1]);
        }

        [DataTestMethod]
        // maxLengthDifference = 1, maxDistance = 1
        [DataRow("ooo", "ooooo", "oooooo", "oxx", "oooo", "oxo", "xoo")]
        [DataRow("ooo", "ooooo", "oooooo", "oxx", "oxo", "xoo", "oooo")]
        [DataRow("ooo", "ooooo", "oooooo", "oxx", "xoo", "oooo", "oxo")]
        [DataRow("ooooo", "ooo", "ooooooo", "oxxoo", "oooo", "oooooo", "oxooo")]
        [DataRow("ooooo", "ooo", "ooooooo", "oxxoo", "oooooo", "oxooo", "oooo")]
        [DataRow("ooooo", "ooo", "ooooooo", "oxxoo", "oxooo", "oooo", "oooooo")]
        // maxLengthDifference = 2, maxDistance = 2
        [DataRow("oooooo", "ooo", "ooooooooo", "oooo", "ooooo", "ooooooo", "ooooox")]
        [DataRow("oooooo", "ooo", "ooooooooo", "oooo", "ooooooo", "ooooox", "ooooo")]
        [DataRow("oooooo", "ooo", "ooooooooo", "oooo", "ooooox", "ooooo", "ooooooo")]
        [DataRow("oooooooo", "oooo", "ooooo", "ooooooooooo", "oooooo", "ooooooo", "ooooooooo", "ooooooox")]
        [DataRow("oooooooo", "oooo", "ooooo", "ooooooooooo", "oooooo", "ooooooooo", "ooooooox", "ooooooo")]
        [DataRow("oooooooo", "oooo", "ooooo", "ooooooooooo", "oooooo", "ooooooox", "ooooooo", "ooooooooo")]
        public void GetSpellingSuggestion_NoCaseInsensitiveMatches_ReturnsFirstCandidateWithSmallestEditDistance(string name, params string[] candidates)
        {
            string? result = SpellChecker.GetSpellingSuggestion(name, candidates);

            result.Should().Be(candidates[^3]);
        }
    }
}
