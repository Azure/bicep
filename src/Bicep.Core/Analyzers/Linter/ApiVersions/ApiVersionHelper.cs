// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public static class ApiVersionHelper
    {
        // Resource types are case-insensitive in ARM
        public static StringComparer Comparer = LanguageConstants.ResourceTypeComparer;

        private static readonly Regex VersionPattern = new Regex(@"^((?<version>(\d{4}-\d{2}-\d{2}))(?<suffix>-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public static (string? date, string? suffixWithHypen) TryParse(string apiVersion)
        {
            MatchCollection matches = VersionPattern.Matches(apiVersion);
            if (matches.Count == 1)
            {
                Match match = matches[0];
                string? version = match.Groups["version"].Value;
                string? suffix = match.Groups["suffix"].Value;

                if (version is not null)
                {
                    return (version, string.IsNullOrEmpty(suffix) ? null : suffix.ToLowerInvariant());
                }
            }

            return (null, null);
        }

        public static (string date, string suffix) MustParse(string apiVersion)
        {
            var (date, suffix) = TryParse(apiVersion);

            if (date is null)
            {
                throw new ArgumentException($"Unexpected API version {apiVersion}");
            }

            return (date, suffix ?? String.Empty);
        }

        public static string Format(DateTime date, string? suffix = null)
        {
            var result = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", date);
            return string.IsNullOrEmpty(suffix) ? result : result + suffix;
        }

        public static DateTime ParseDateFromString(string dateString)
        {
            return DateTime.ParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime ParseDateFromApiVersion(string apiVersion)
        {
            (string? date, string? _) = TryParse(apiVersion);
            if (date is null)
            {
                throw new Exception($"Invalid API version '{apiVersion}'");
            }

            return ParseDateFromString(date);
        }
    }
}
