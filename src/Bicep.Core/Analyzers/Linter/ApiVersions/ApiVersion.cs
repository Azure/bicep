// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Diagnostics;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public record struct ApiVersion
    {
        public ApiVersion(DateTime date, string suffix)
        {
            Date = date;
            Suffix = suffix;

            Debug.Assert(date.Date == date, "Should be a date, not a date/time");
            Debug.Assert(suffix == "" || suffix[0] == '-', "Suffix should be empty or being with a hyphen");
        }

        public ApiVersion(string dateAsString, string? suffix)
            : this(ApiVersionHelper.ParseDateFromString(dateAsString), suffix ?? String.Empty)
        {
        }

        public ApiVersion(string apiVersion)
            : this(ApiVersionHelper.MustParse(apiVersion))
        {
        }

        public static ApiVersion? TryParse(string apiVersion)
        {
            var (date, suffix) = ApiVersionHelper.TryParse(apiVersion);
            if (date is not null)
            {
                return new ApiVersion(date, suffix);
            }

            return null;
        }

        private ApiVersion((string date, string suffix) apiVersion)
            : this(ApiVersionHelper.ParseDateFromString(apiVersion.date), apiVersion.suffix)
        {
        }

        public DateTime Date { get; init; }
        public string Suffix { get; init; }

        public bool IsPreview => Suffix.Length > 0;
        public bool IsStable => Suffix.Length == 0;

        public string Formatted => ApiVersionHelper.Format(Date, Suffix);
    }
}
