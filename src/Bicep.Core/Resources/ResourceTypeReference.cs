// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace Bicep.Core.Resources
{
    public partial class ResourceTypeReference : IEquatable<ResourceTypeReference>
    {
        private readonly int hashCode;
        private ImmutableArray<string> typeSegments;

        public ResourceTypeReference(string type, string? version)
            : this(version is null ? type : $"{type}@{version}", type, version)
        {
        }

        private ResourceTypeReference(string name, string type, string? version)
        {
            if (type.Length <= 0)
            {
                throw new ArgumentException("Type must be non-empty.");
            }

            Name = name;
            Type = type;
            ApiVersion = version;
            hashCode = HashCode.Combine(
                LanguageConstants.ResourceTypeComparer.GetHashCode(type),
                version is null ? 0 : LanguageConstants.ResourceTypeComparer.GetHashCode(version));
        }

        public string FormatName() => Name;

        public string FormatType() => Type;

        public ImmutableArray<string> TypeSegments
        {
            get
            {
                if (typeSegments.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(
                        ref typeSegments,
                        SplitTypeSegments(Type));
                }

                return typeSegments;
            }
        }

        public string Name { get; }

        public string Type { get; }

        public string? ApiVersion { get; }

        public bool IsParentOf(ResourceTypeReference other)
        {
            if (this.TypeSegments.Length + 1 != other.TypeSegments.Length)
            {
                return false;
            }

            for (var i = 0; i < this.TypeSegments.Length; i++)
            {
                if (!StringComparer.OrdinalIgnoreCase.Equals(this.TypeSegments[i], other.TypeSegments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static ResourceTypeReference? TryParse(string resourceType)
        {
            var separatorIndex = resourceType.IndexOf('@');
            var typeSpan = separatorIndex < 0 ? resourceType.AsSpan() : resourceType.AsSpan(0, separatorIndex);
            if (!IsValidType(typeSpan))
            {
                return null;
            }

            if (separatorIndex < 0)
            {
                return new ResourceTypeReference(resourceType, null);
            }

            var versionSpan = resourceType.AsSpan(separatorIndex + 1);
            if (!IsValidVersion(versionSpan))
            {
                return null;
            }

            return new ResourceTypeReference(
                resourceType,
                resourceType[..separatorIndex],
                resourceType[(separatorIndex + 1)..]);
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

        public override bool Equals(object? other) => Equals(other as ResourceTypeReference);

        public bool Equals(ResourceTypeReference? other)
            => other is not null &&
            LanguageConstants.ResourceTypeComparer.Equals(this.Type, other.Type) &&
            LanguageConstants.ResourceTypeComparer.Equals(this.ApiVersion, other.ApiVersion);

        public override int GetHashCode()
            => hashCode;

        private static bool IsValidType(ReadOnlySpan<char> type)
        {
            var segmentStart = true;
            foreach (var character in type)
            {
                if (segmentStart)
                {
                    if (!char.IsAsciiLetterOrDigit(character))
                    {
                        return false;
                    }

                    segmentStart = false;
                }
                else if (character == '/')
                {
                    segmentStart = true;
                }
                else if (!char.IsAsciiLetterOrDigit(character) && character is not '-' and not '.')
                {
                    return false;
                }
            }

            return !segmentStart;
        }

        private static bool IsValidVersion(ReadOnlySpan<char> version)
        {
            if (version.Length < 2 || !char.IsAsciiLetterOrDigit(version[0]))
            {
                return false;
            }

            foreach (var character in version[1..])
            {
                if (!char.IsAsciiLetterOrDigit(character) && character is not '-' and not '.')
                {
                    return false;
                }
            }

            return true;
        }

        private static ImmutableArray<string> SplitTypeSegments(string type)
        {
            var builder = ImmutableArray.CreateBuilder<string>(type.AsSpan().Count('/') + 1);
            foreach (var range in type.AsSpan().Split('/'))
            {
                builder.Add(type[range]);
            }

            return builder.MoveToImmutable();
        }

        [GeneratedRegex("^(?<types>[a-z0-9][a-z0-9-.]*(/[a-z0-9][a-z0-9-.]*)*)@", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex ResourceTypePrefixPattern();
    }
}
