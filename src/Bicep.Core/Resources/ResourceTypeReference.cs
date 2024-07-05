// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Bicep.Core.Resources
{
    public partial class ResourceTypeReference
    {
        public ResourceTypeReference(string type, string? version)
        {
            if (type.Length <= 0)
            {
                throw new ArgumentException("Type must be non-empty.");
            }

            Name = version is null ? type : $"{type}@{version}";
            Type = type;
            TypeSegments = [.. type.Split('/')];
            ApiVersion = version;
        }

        public string FormatName() => Name;

        public string FormatType() => Type;

        public ImmutableArray<string> TypeSegments { get; }

        public string Name { get; }

        public string Type { get; }

        public string? ApiVersion { get; }

        public bool IsParentOf(ResourceTypeReference other)
        {
            // Parent should have N types, child should have N+1, first N types should be equal
            return
                this.TypeSegments.Length + 1 == other.TypeSegments.Length &&
                Enumerable.SequenceEqual(this.TypeSegments, other.TypeSegments.Take(this.TypeSegments.Length), StringComparer.OrdinalIgnoreCase);
        }

        public static ResourceTypeReference? TryParse(string resourceType)
        {
            var match = ResourceTypePattern().Match(resourceType);
            if (match.Success == false)
            {
                return null;
            }

            var types = match.Groups["types"].Value;
            var version = match.Groups["version"].Value;

            if (string.IsNullOrEmpty(version))
            {
                return new ResourceTypeReference(types, null);
            }

            return new ResourceTypeReference(types, version);
        }

        public static ResourceTypeReference Combine(ResourceTypeReference baseType, ResourceTypeReference nestedType)
        {
            return new ResourceTypeReference(
                $"{baseType.Type}/{nestedType.Type}",
                nestedType.ApiVersion ?? baseType.ApiVersion);
        }

        public static ResourceTypeReference Parse(string resourceType)
            => TryParse(resourceType) ?? throw new ArgumentException($"Unable to parse '{resourceType}'", nameof(resourceType));

        public static bool HasResourceTypePrefix(string segment)
            => ResourceTypePrefixPattern().IsMatch(segment);

        public override string ToString()
            => this.FormatName();

        public override bool Equals(object? other)
        {
            if (other is not ResourceTypeReference otherReference)
            {
                return false;
            }

            return Enumerable.SequenceEqual(this.TypeSegments, otherReference.TypeSegments, LanguageConstants.ResourceTypeComparer) &&
                LanguageConstants.ResourceTypeComparer.Equals(this.ApiVersion, otherReference.ApiVersion);
        }

        public override int GetHashCode()
            => HashCode.Combine(
                Enumerable.Select(this.TypeSegments, x => LanguageConstants.ResourceTypeComparer.GetHashCode(x)).Aggregate((a, b) => a ^ b),
                (this.ApiVersion is null ? 0 : LanguageConstants.ResourceTypeComparer.GetHashCode(this.ApiVersion)));

        [GeneratedRegex("^(?<types>[a-z0-9][a-z0-9-.]*(/[a-z0-9][a-z0-9-.]*)*)@", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex ResourceTypePrefixPattern();

        [GeneratedRegex(@"^(?<types>[a-z0-9][a-z0-9-.]*(/[a-z0-9][a-z0-9-.]*)*)(@(?<version>[a-z0-9][a-z0-9-\.]+))?$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex ResourceTypePattern();
    }
}
