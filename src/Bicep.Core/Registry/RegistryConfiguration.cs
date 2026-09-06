// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;

namespace Bicep.Core.Registry
{
    public class RegistryConfiguration
    {
        /// <summary>
        /// Built-in trusted registries. Hardcoded — users cannot remove these.
        /// Wildcard entries use the form "*.suffix" meaning any subdomain of that suffix.
        /// Exact entries match only the specified hostname.
        /// </summary>
        private static readonly ImmutableArray<string> BuiltInTrustedRegistries =
        [
            "*.azurecr.io",
            "*.azurecr.cn",
            "*.azurecr.us",
            "mcr.microsoft.com",
            "mcr.azure.cn",
            "ghcr.io",
        ];

        private readonly bool permitUntrustedRegistries;

        /// <summary>
        /// Additional trusted registries supplied by the user (e.g. via the
        /// BICEP_TRUSTED_REGISTRIES environment variable). Uses the same matching rules as
        /// <see cref="BuiltInTrustedRegistries"/>: "*.suffix" for subdomain wildcards, otherwise exact match.
        /// </summary>
        private readonly ImmutableArray<string> additionalTrustedRegistries;

        public RegistryConfiguration(bool PermitUntrustedRegistries, IEnumerable<string>? additionalTrustedRegistries = null)
        {
            this.permitUntrustedRegistries = PermitUntrustedRegistries;
            this.additionalTrustedRegistries = additionalTrustedRegistries?.ToImmutableArray() ?? [];
        }

        /// <summary>
        /// Returns true if <paramref name="registryHostname"/> is trusted by the built-in list
        /// or the user-supplied list.
        /// </summary>
        public bool IsRegistryTrusted(string registryHostname)
        {
            if (permitUntrustedRegistries)
            {
                return true;
            }

            var normalized = Normalize(registryHostname);
            if (normalized is null)
            {
                // URL forms, paths, ports — not a bare hostname
                return false;
            }

            return IsMatchedByAny(normalized, BuiltInTrustedRegistries)
                || IsMatchedByAny(normalized, additionalTrustedRegistries);
        }

        /// <summary>
        /// Parses a delimited list of registry hostnames/patterns (comma or semicolon separated),
        /// e.g. the value of the BICEP_TRUSTED_REGISTRIES environment variable.
        /// </summary>
        public static IEnumerable<string> ParseTrustedRegistries(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return [];
            }

            return value
                .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        // ── Helpers ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Normalizes a registry hostname for comparison.
        /// Returns null if the input looks like a URL or path (not a bare hostname).
        /// </summary>
        private static string? Normalize(string hostname)
        {
            // Reject URL forms
            if (hostname.Contains("://") || hostname.Contains('@'))
            {
                return null;
            }

            // Reject paths
            if (hostname.Contains('/'))
            {
                return null;
            }

            string normalized;

            // Handle IPv6 addresses in bracket notation: "[::1]" or "[::1]:5000"
            if (hostname.StartsWith('['))
            {
                var closingBracket = hostname.IndexOf(']');
                if (closingBracket < 0)
                {
                    return null; // malformed IPv6
                }
                // Everything up to and including the closing bracket is the address
                normalized = hostname[..(closingBracket + 1)];
            }
            else
            {
                // Strip port (e.g. "contoso.azurecr.io:443" → "contoso.azurecr.io")
                // This allows registry references like "localhost:5000" to be evaluated by their host
                var colonIndex = hostname.IndexOf(':');
                normalized = colonIndex >= 0 ? hostname[..colonIndex] : hostname;
            }

            // Strip trailing dot (FQDN normalization)
            if (normalized.EndsWith('.'))
            {
                normalized = normalized[..^1];
            }

            return normalized.ToLowerInvariant();
        }

        private static bool IsMatchedByAny(string normalizedHostname, ImmutableArray<string> patterns)
        {
            foreach (var pattern in patterns)
            {
                if (IsMatch(normalizedHostname, pattern))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsMatch(string normalizedHostname, string pattern)
        {
            if (pattern.StartsWith("*."))
            {
                // Constrained wildcard: *.azurecr.io → suffix ".azurecr.io"
                var suffix = pattern[1..]; // keep the leading dot: ".azurecr.io"
                return normalizedHostname.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) &&
                       normalizedHostname.Length > suffix.Length;
            }

            // Exact match (case-insensitive)
            return string.Equals(normalizedHostname, pattern, StringComparison.OrdinalIgnoreCase);
        }
    }
}
