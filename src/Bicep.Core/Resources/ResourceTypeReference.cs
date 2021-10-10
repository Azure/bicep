// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;
using Bicep.Core.TypeSystem;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReference
    {
        private const string TypeSegmentPattern = "[a-z0-9][a-z0-9-.]*";
        private const string VersionPattern = "[a-z0-9][a-z0-9-]+";

        private const RegexOptions PatternRegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private static readonly Regex ResourceTypePattern = new Regex(@$"^(?<type>{TypeSegmentPattern})(/(?<type>{TypeSegmentPattern}))*(@(?<version>{VersionPattern}))?", PatternRegexOptions);

        public ResourceTypeReference(ImmutableArray<string> typeSegments, string? version)
        {
            if (typeSegments.Length <= 0)
            {
                throw new ArgumentException("At least one type must be specified.");
            }

            TypeSegments = typeSegments;
            Version = version;
        }

        public string FormatName()
            => $"{FormatType()}{(this.Version == null ? "" : $"@{this.Version}")}";

        public string FormatType()
            => string.Join('/', this.TypeSegments);

        public ImmutableArray<string> TypeSegments { get; }

        public string? Version { get; }

        public bool IsParentOf(ResourceTypeReference other)
        {
            // Parent should have N types, child should have N+1, first N types should be equal
            return 
                // TODO ensure we're comparing within the same namespace
                this.TypeSegments.Length + 1 == other.TypeSegments.Length &&
                Enumerable.SequenceEqual(this.TypeSegments, other.TypeSegments.Take(this.TypeSegments.Length), StringComparer.OrdinalIgnoreCase);
        }

        public static ResourceTypeReference? TryParse(string resourceType)
        {
            var match = ResourceTypePattern.Match(resourceType);
            if (match.Success == false)
            {
                return null;
            }

            var types = match.Groups["type"].Captures.Cast<Capture>()
                .Select(c => c.Value)
                .ToImmutableArray();
            var version = match.Groups["version"].Value;

            return new ResourceTypeReference(types, version);
        }

        public static ResourceTypeReference Combine(ResourceTypeReference baseType, ResourceTypeReference nestedType)
        {
            return new ResourceTypeReference(
                baseType.TypeSegments.AddRange(nestedType.TypeSegments),
                nestedType.Version ?? baseType.Version);
         }

        public static ResourceTypeReference Parse(string resourceType)
            => TryParse(resourceType) ?? throw new ArgumentException($"Unable to parse '{resourceType}'", nameof(resourceType));

        public static bool IsNamespaceAndTypeSegment(string segment)
        {
            return ResourceTypePattern.Match(segment).Success;
        }
    }
}
