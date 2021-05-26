// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Bicep.Core.Extensions;

namespace Bicep.Core.Resources
{
    public class ResourceTypeReference
    {
        private static readonly RegexOptions PatternRegexOptions = RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.CultureInvariant;
        private static readonly Regex ResourceTypePattern = new Regex(@"^(?<namespace>[a-z0-9][a-z0-9\.]*)(/(?<type>[a-z0-9\-]+))+@(?<version>[a-z0-9\-])*?", PatternRegexOptions);

        private static readonly Regex VersionedResourceTypePattern = new Regex(@"^(?<namespace>[a-z0-9][a-z0-9\.]*)(/(?<type>[a-z0-9\-]+))+@(?<version>(\d{4}-\d{2}-\d{2})(-(preview|alpha|beta|rc|privatepreview))?$)", PatternRegexOptions);

        private static readonly Regex SingleTypePattern = new Regex(@"^(?<type>[a-z0-9\-]+)(@(?<version>(\d{4}-\d{2}-\d{2})(-(preview|alpha|beta|rc|privatepreview))?))?$", PatternRegexOptions);

        public ResourceTypeReference(string @namespace, IEnumerable<string> types, string apiVersion)
        {
            if (String.IsNullOrWhiteSpace(@namespace))
            {
                throw new ArgumentException("Namespace must not be null, empty or whitespace.");
            }

            if (String.IsNullOrWhiteSpace(apiVersion))
            {
                throw new ArgumentException("API Version must not be null, empty or whitespace.");
            }

            this.Namespace = @namespace;
            this.Types = types.ToImmutableArray();
            if (this.Types.Length <= 0)
            {
                throw new ArgumentException("At least one type must be specified.");
            }

            this.ApiVersion = apiVersion;
        }

        public string Namespace { get; }

        public ImmutableArray<string> Types { get; }

        public string TypesString => this.Types.ConcatString("/");

        public string ApiVersion { get; }

        public string FullyQualifiedType => $"{this.Namespace}/{this.TypesString}";

        public string FormatName()
            => $"{this.FullyQualifiedType}@{this.ApiVersion}";

        public bool IsRootType => Types.Length == 1;

        public bool IsParentOf(ResourceTypeReference other)
        {
            return 
                StringComparer.OrdinalIgnoreCase.Equals(this.Namespace, other.Namespace) &&

                // Parent should have N types, child should have N+1, first N types should be equal
                this.Types.Length + 1 == other.Types.Length &&
                Enumerable.SequenceEqual(this.Types, other.Types.Take(this.Types.Length), StringComparer.OrdinalIgnoreCase);
        }

        public static ResourceTypeReference? TryCombine(ResourceTypeReference baseType, IEnumerable<string> typeSegments)
        {
            var types = new List<string>(baseType.Types);

            var bestVersion = baseType.ApiVersion;
            foreach (var typeSegment in typeSegments)
            {
                if (!TryParseSingleTypeSegment(typeSegment, out var type, out var version))
                {
                    return null;
                }

                types.Add(type);

                if (!string.IsNullOrEmpty(version))
                {
                    bestVersion = version;
                }
            }

            return new ResourceTypeReference(baseType.Namespace, types, bestVersion);
        }

        public static ResourceTypeReference? TryParse(string resourceType)
        {
            var match = VersionedResourceTypePattern.Match(resourceType);
            if (match.Success == false)
            {
                return null;
            }

            var ns = match.Groups["namespace"].Value;
            var types = match.Groups["type"].Captures.Cast<Capture>().Select(c => c.Value);
            var version = match.Groups["version"].Value;

            return new ResourceTypeReference(ns, types, version);
        }

        public static ResourceTypeReference Parse(string resourceType)
            => TryParse(resourceType) ?? throw new ArgumentException($"Unable to parse '{resourceType}'", nameof(resourceType));

        public static bool TryParseSingleTypeSegment(string typeSegment, [NotNullWhen(true)] out string? type, out string? version)
        {
            var match = SingleTypePattern.Match(typeSegment);
            if (match.Success == false)
            {
                type = null;
                version = null;
                return false;
            }

            type = match.Groups["type"].Value;
            version = match.Groups["version"].Value;
            if (version == "")
            {
                version = null;
            }
            
            return true;
        }

        public static bool IsNamespaceAndTypeSegment(string segment)
        {
            return ResourceTypePattern.Match(segment).Success;
        }
    }
}
