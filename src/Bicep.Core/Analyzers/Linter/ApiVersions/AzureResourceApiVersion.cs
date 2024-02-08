// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Bicep.Core.Analyzers.Linter.ApiVersions
{
    public readonly partial record struct AzureResourceApiVersion
    {
        public const string DateFormat = "yyyy-MM-dd";

        private AzureResourceApiVersion(DateOnly date, string suffix)
        {
            Date = date;
            Suffix = suffix;
        }

        public DateOnly Date { get; }
        public string Suffix { get; }

        public bool IsPreview => Suffix.Length > 0;
        public bool IsStable => Suffix.Length == 0;

        public static implicit operator string(AzureResourceApiVersion apiVersion) => apiVersion.ToString();

        public static AzureResourceApiVersion Parse(string value)
        {
            if (TryParse(value, out var result))
            {
                return result;
            }

            throw new FormatException($"The provided value '{value}' is not a valid Azure resource API version.");
        }

        public static bool TryParse(string value, out AzureResourceApiVersion result)
        {
            var match = ApiVersionPattern().Match(value);

            if (match.Success)
            {
                var date = DateOnly.ParseExact(match.Groups["date"].Value, DateFormat, CultureInfo.InvariantCulture);
                var suffix = match.Groups["suffix"].Value.ToLowerInvariant();

                result = new AzureResourceApiVersion(date, suffix);
                return true;
            }

            result = default;
            return false;
        }

        public override string ToString() => $"{this.Date.ToString(DateFormat, CultureInfo.InvariantCulture)}{this.Suffix.ToLowerInvariant()}";

        [GeneratedRegex(@"^((?<date>(\d{4}-\d{2}-\d{2}))(?<suffix>-(preview|alpha|beta|rc|privatepreview))?$)", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture)]
        private static partial Regex ApiVersionPattern();
    }
}
