// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Azure.Deployments.Core.Comparers;
using Bicep.Core.Analyzers.Linter.ApiVersions;

namespace Bicep.LanguageServer.Features.Custom.Visualization
{
    /// <summary>
    /// API version rules for the resource palette.
    /// </summary>
    internal static class ResourceApiVersionPolicy
    {
        public static bool IsPreview(string apiVersion) =>
            AzureResourceApiVersion.TryParse(apiVersion, out var parsed) && parsed.IsPreview;

        /// <summary>
        /// Picks the version a new resource gets by default: stable versions win over preview versions, and within the
        /// same stability the newest wins.
        /// </summary>
        public static string? SelectDefault(IEnumerable<string?> apiVersions)
        {
            string? selected = null;
            foreach (var apiVersion in apiVersions)
            {
                if (apiVersion is not null && (selected is null || ComparePreference(apiVersion, selected) > 0))
                {
                    selected = apiVersion;
                }
            }

            return selected;
        }

        public static ImmutableArray<string> SortNewestFirst(IEnumerable<string?> apiVersions) =>
            [.. apiVersions
                .OfType<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderByDescending(apiVersion => apiVersion, ApiVersionComparer.Instance)];

        private static int ComparePreference(string left, string right)
        {
            var leftIsPreview = IsPreview(left);

            return leftIsPreview == IsPreview(right)
                ? ApiVersionComparer.Instance.Compare(left, right)
                : leftIsPreview ? -1 : 1;
        }
    }
}
