// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Bicep.Core.Registry.Oci
{
    public static partial class OciArtifactReferenceFacts
    {
        public const string Scheme = "br";

        public const int MaxRegistryLength = 255;

        // must be kept in sync with TagNameRegex
        public const int MaxTagLength = 128;

        public const int MaxRepositoryLength = 255;

        // the registry component is equivalent to a host in a URI, which are case-insensitive
        public static readonly IEqualityComparer<string> RegistryComparer = StringComparer.OrdinalIgnoreCase;

        // repository component is case-sensitive (although regex blocks upper case)
        public static readonly IEqualityComparer<string> RepositoryComparer = StringComparer.Ordinal;

        // tags are case-sensitive and may contain upper and lowercase characters
        public static readonly IEqualityComparer<string?> TagComparer = StringComparer.Ordinal;

        // digests are case sensitive
        public static readonly IEqualityComparer<string?> DigestComparer = StringComparer.Ordinal;

        public static bool IsOciNamespaceSegment(string value) => OciNamespaceSegmentRegex().IsMatch(value);

        public static bool IsOciDigest(string value) => DigestRegex().IsMatch(value);

        public static bool IsOciTag(string value) => TagNameRegex().IsMatch(value);

        // obtained from https://github.com/opencontainers/distribution-spec/blob/main/spec.md#pull
        [GeneratedRegex("^[a-z0-9]+([._-][a-z0-9]+)*$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex OciNamespaceSegmentRegex();
        [GeneratedRegex("^sha256:[a-f0-9]{64}$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex DigestRegex();

        [GeneratedRegex("^[a-zA-Z0-9_][a-zA-Z0-9._-]{0,127}$", RegexOptions.ExplicitCapture | RegexOptions.CultureInvariant)]
        private static partial Regex TagNameRegex();
    }
}
