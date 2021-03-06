// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Bicep.Core.Text
{
    public static class SpellChecker
    {
        public static string? GetSpellingSuggestion(string name, IEnumerable<string> candidates)
        {
            if (candidates.Any() != true)
            {
                return null;
            }

            // 0.34 allows 1 insertion/deletion/substitution for every 3 characters.
            int maxLengthDifference = Min((int)Math.Floor(name.Length * 0.34), 2);
            int maxDistance = (int)Math.Floor(name.Length * 0.34) + 1;

            string? bestCandidate = null;
            bool checkCaseInsensitiveEqualityOnly = false;
            string nameUpperCase = name.ToUpperInvariant();

            foreach (string candidate in candidates)
            {
                if (string.IsNullOrEmpty(candidate) || Math.Abs(name.Length - candidate.Length) > maxLengthDifference)
                {
                    continue;
                }

                string candidateUpperCase = candidate.ToUpperInvariant();

                if (nameUpperCase.Equals(candidateUpperCase))
                {
                    return candidate;
                }

                if (checkCaseInsensitiveEqualityOnly)
                {
                    continue;
                }

                if (candidate.Length < 3)
                {
                    // Don't bother for such a short candidate name.
                    continue;
                }

                int distance = GetLevenshteinDistanceWithMax(nameUpperCase, candidateUpperCase, maxDistance - 1);

                if (distance < 0)
                {
                    continue;
                }

                if (distance < 2)
                {
                    /*
                     * There won't be any better result with a smaller Levenshtein distance,
                     * so check case-insensitive equality for the rest of the candidates.
                     */
                    checkCaseInsensitiveEqualityOnly = true;
                    bestCandidate = candidate;
                }
                else
                {
                    Debug.Assert(distance < maxDistance);

                    maxDistance = distance;
                    bestCandidate = candidate;
                }
            }

            return bestCandidate;
        }

        private static int GetLevenshteinDistanceWithMax(string first, string second, int maxDistance)
        {
            if (Math.Abs(first.Length - second.Length) > maxDistance)
            {
                return -1;
            }

            /*
             * Lev(i, j): Levenshtein distance between first.Substring(0, i) and second.Substring(0, j).
             * Using two 1D arrays instead of one 2d array to reduce space complexity.
             */
            var previous = new int[second.Length + 1];
            var current = new int[second.Length + 1];
            var boundary = maxDistance + 1;

            // Initialize Lev(0, j).
            for (int j = 0; j <= second.Length; j++)
            {
                previous[j] = j;
            }

            // Start DP to fill Lev(i, j).
            for (int i = 1; i <= first.Length; i++)
            {
                int minJ = i > maxDistance ? i - maxDistance : 1;
                int maxJ = second.Length > maxDistance + i ? maxDistance + i : second.Length;
                int minDistance = i;

                // Lev(i, 0).
                current[0] = i;

                // Lev(i, 1) to Lev(i, minJ - 1).
                for (int j = 1; j < minJ; j++)
                {
                    current[j] = boundary;
                }

                // Lev(i, minJ) to Lev(i, maxJ).
                for (int j = minJ; j <= maxJ; j++)
                {
                    // See: https://en.wikipedia.org/wiki/Levenshtein_distance#Definition
                    current[j] = first[i - 1] == second[j - 1]
                        ? previous[j - 1]
                        : 1 + Min(previous[j], current[j - 1], previous[j - 1]);

                    minDistance = Min(minDistance, current[j]);
                }

                // Lev(i, maxJ + 1) to Lev(i, second.Length).
                for (int j = maxJ + 1; j <= second.Length; j++)
                {
                    current[j] = boundary;
                }

                if (minDistance > maxDistance)
                {
                    // Short circuit since we will not get any better result later for longer strings.
                    return -1;
                }

                var temp = current;
                current = previous;
                previous = temp;
            }

            /*
             * When the DP completes, previous stores
             * Lev(first.Length, 0), Lev(first.Length, 1), ..., Lev(first.Length, second.Length).
             */
            return previous[second.Length] > maxDistance ? -1 : previous[second.Length];
        }

        private static int Min(params int[] values) => values.Min();
    }
}
