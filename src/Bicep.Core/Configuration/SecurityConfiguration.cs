// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;

namespace Bicep.Core.Configuration
{
    public class SecurityConfiguration
    {
        public const string SecurityKey = "security";
        public const string TrustedRegistriesKey = "trustedRegistries";

        /// <summary>
        /// Built-in trusted registries. Hardcoded — users cannot remove these.
        /// Wildcard entries use the form "*.suffix" meaning any subdomain of that suffix.
        /// Exact entries match only the specified hostname.
        /// </summary>
        public static readonly ImmutableArray<string> BuiltInTrustedRegistries =
        [
            "*.azurecr.io",
            "*.azurecr.cn",
            "*.azurecr.us",
            "mcr.microsoft.com",
            "mcr.azure.cn",
            "ghcr.io",
        ];

        private SecurityConfiguration(
            ImmutableArray<string> trustedRegistries,
            ImmutableArray<(string Pattern, string Reason)> invalidRegistryPatterns)
        {
            TrustedRegistries = trustedRegistries;
            InvalidRegistryPatterns = invalidRegistryPatterns;
        }

        /// <summary>
        /// User-supplied trusted registry entries (valid patterns only).
        /// </summary>
        public ImmutableArray<string> TrustedRegistries { get; }

        /// <summary>
        /// Invalid registry patterns found in user config, along with the validation reason.
        /// If non-empty, a BCP447 warning is emitted for each (up to a cap)
        /// but restore proceeds for modules whose registry matches a valid trusted entry.
        /// </summary>
        public ImmutableArray<(string Pattern, string Reason)> InvalidRegistryPatterns { get; }

        /// <summary>
        /// True if any invalid registry patterns were found. A BCP447 warning is emitted for each,
        /// but restore is not blocked for modules whose registry matches a valid trusted entry.
        /// </summary>
        public bool HasInvalidRegistryPatterns => !InvalidRegistryPatterns.IsEmpty;

        /// <summary>
        /// Returns a default (empty user list, no invalid patterns) instance.
        /// Built-in registries are always in effect.
        /// </summary>
        public static readonly SecurityConfiguration Default = new([], []);

        /// <summary>
        /// Parses the "security" JSON element from bicepconfig.json.
        /// Invalid patterns are captured in <see cref="InvalidRegistryPatterns"/> rather than thrown.
        /// </summary>
        public static SecurityConfiguration Bind(JsonElement element)
        {
            if (!element.TryGetProperty(TrustedRegistriesKey, out var registriesElement) ||
                registriesElement.ValueKind != JsonValueKind.Array)
            {
                return Default;
            }

            var valid = ImmutableArray.CreateBuilder<string>();
            var invalid = ImmutableArray.CreateBuilder<(string Pattern, string Reason)>();

            foreach (var entry in registriesElement.EnumerateArray())
            {
                var pattern = entry.GetString();
                if (pattern is null)
                {
                    continue;
                }

                var error = ValidateRegistryPattern(pattern);
                if (error is null)
                {
                    valid.Add(pattern);
                }
                else
                {
                    invalid.Add((pattern, error));
                }
            }

            return new(valid.ToImmutable(), invalid.ToImmutable());
        }

        /// <summary>
        /// Returns true if <paramref name="registryHostname"/> is trusted by the built-in list
        /// or the user-supplied list.
        /// </summary>
        public bool IsRegistryTrusted(string registryHostname)
        {
            var normalized = Normalize(registryHostname);
            if (normalized is null)
            {
                // URL forms, paths, ports — not a bare hostname
                return false;
            }

            return IsMatchedByAny(normalized, BuiltInTrustedRegistries) ||
                   IsMatchedByAny(normalized, TrustedRegistries);
        }

        /// <summary>
        /// Validates a single trusted registry pattern string.
        /// Returns null if the pattern is valid, or an error message string if it is invalid.
        /// </summary>
        public static string? ValidateRegistryPattern(string pattern)
        {
            if (string.IsNullOrWhiteSpace(pattern))
            {
                return "Pattern must not be empty or whitespace.";
            }

            // Reject URL forms
            if (pattern.Contains("://") || pattern.Contains('@'))
            {
                return $"Pattern '{pattern}' must be a bare hostname, not a URL. Remove the scheme (e.g. 'https://') and any credentials.";
            }

            // Reject path segments
            if (pattern.Contains('/'))
            {
                return $"Pattern '{pattern}' must not contain a path. Use a bare hostname or '*.suffix' form.";
            }

            // Reject port specifiers in patterns (ports in registry references are handled separately).
            // Exception: IPv6 addresses use ':' inside brackets — "[::1]" is valid.
            if (pattern.Contains(':') && !pattern.StartsWith('['))
            {
                return $"Pattern '{pattern}' must not contain a port. Omit the port number from the pattern.";
            }

            // Reject port specifiers in IPv6 patterns (e.g. "[::1]:5000" — use "[::1]")
            if (pattern.StartsWith('[') && pattern.Contains("]:"))
            {
                return $"Pattern '{pattern}' must not contain a port. Use just the IPv6 address without a port number (e.g. '[::1]').";
            }

            if (pattern.StartsWith('.'))
            {
                return $"Pattern '{pattern}' must not start with a dot.";
            }

            if (pattern.Contains(".."))
            {
                return $"Pattern '{pattern}' contains an empty label (consecutive dots).";
            }

            if (pattern == "*")
            {
                return $"Pattern '*' is not allowed because it trusts all registries. Use a specific hostname or '*.example.com' form.";
            }

            if (pattern.StartsWith("*."))
            {
                var baseDomain = pattern[2..]; // everything after "*."

                // Reject multiple wildcards: *.*.example.com
                if (baseDomain.Contains('*'))
                {
                    return $"Pattern '{pattern}' contains multiple wildcards. Only a single '*.' prefix is allowed.";
                }

                // Reject over-broad TLD wildcards: *.com, *.io, *.net etc.
                // A valid base domain must contain at least one dot (e.g., "example.com", "azurecr.io" is the base for "*.azurecr.io")
                // Actually for *.io: base = "io", no dot → reject
                if (!baseDomain.Contains('.'))
                {
                    return $"Pattern '{pattern}' is too broad. Wildcards over a top-level domain suffix (e.g. '*.com', '*.io') are not permitted.";
                }

                return null; // valid wildcard
            }

            if (pattern.Contains('*'))
            {
                return $"Pattern '{pattern}' uses a wildcard in an unsupported position. The wildcard '*' is only allowed as the entire left-most label (e.g. '*.example.com').";
            }

            // Exact hostname — no further validation needed beyond the checks above
            return null;
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

        /// <summary>
        /// Writes the security section as JSON to the provided writer.
        /// Only user-supplied <see cref="TrustedRegistries"/> entries are written
        /// (built-in entries are implicit and not persisted).
        /// </summary>
        public void WriteTo(System.Text.Json.Utf8JsonWriter writer)
        {
            writer.WriteStartObject();
            writer.WriteStartArray(TrustedRegistriesKey);
            foreach (var entry in TrustedRegistries)
            {
                writer.WriteStringValue(entry);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
