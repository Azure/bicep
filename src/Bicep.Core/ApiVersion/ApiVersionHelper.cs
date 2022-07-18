// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bicep.Core.ApiVersion
{
    public static class ApiVersionHelper
    {
        public static StringComparer Comparer = LanguageConstants.ResourceTypeComparer;

        private static readonly int ApiVersionDateLength = "2000-01-01".Length;

        private static readonly Regex VersionPattern = new Regex(@"^((?<version>(\d{4}-\d{2}-\d{2}))(?<suffix>-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase);

        public static (string? date, string? suffixWithHypen) TryParse(string apiVersion)
        {
            MatchCollection matches = VersionPattern.Matches(apiVersion);

            foreach (Match match in matches)
            {
                string? version = match.Groups["version"].Value;
                string? suffix = match.Groups["suffix"].Value;

                if (version is not null)
                {
                    return (version, string.IsNullOrEmpty(suffix) ? null : suffix.ToLowerInvariant());//using Bicep.Core.ApiVersion; testpoint
                }
            }

            return (null, null);//using Bicep.Core.ApiVersion; testpoint
        }

        public static string Format(DateTime date, string? suffix = null)
        {
            var result = string.Format(CultureInfo.InvariantCulture, "{0:yyyy-MM-dd}", date);//using Bicep.Core.ApiVersion; testpoint
            return string.IsNullOrEmpty(suffix) ? result : result + suffix;
        }

        // Assumes a and b are valid api-version strings
        // Compares just the date in an API version
        // Positive means a > b
        public static int CompareApiVersionDates(string a, string b)
        {
            // Since apiVersions are in the form yyyy-MM-dd{-*}, we can do a simple string comparison against the
            // date portion.
            return a.Substring(0, ApiVersionDateLength).CompareTo(b.Substring(0, ApiVersionDateLength));//using Bicep.Core.ApiVersion; testpoint
        }

        // Assumes apiVersion is a valid api-version string
        public static bool IsPreviewVersion(string apiVersion)
        {
            return apiVersion.Length > ApiVersionDateLength;//using Bicep.Core.ApiVersion; testpoint
        }

        public static DateTime ParseDate(string apiVersion) //using Bicep.Core.ApiVersion; testpoint
        {
            (string? date, string? _) = TryParse(apiVersion);
            if (date is null)
            {
                throw new Exception($"Invalid API version '{apiVersion}'");//using Bicep.Core.ApiVersion; testpoint
            }

            return DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);//using Bicep.Core.ApiVersion; testpoint
        }

        public static IEnumerable<string> FilterPreview(IEnumerable<string> apiVersions)
        {
            return apiVersions.Where(v => ApiVersionHelper.IsPreviewVersion(v)).ToArray();//using Bicep.Core.ApiVersion; testpoint
        }

        public static IEnumerable<string> FilterNonPreview(IEnumerable<string> apiVersions)
        {
            return apiVersions.Where(v => !ApiVersionHelper.IsPreviewVersion(v)).ToArray();//using Bicep.Core.ApiVersion; testpoint
        }
    }
}
