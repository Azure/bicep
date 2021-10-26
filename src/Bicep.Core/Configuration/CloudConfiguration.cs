// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bicep.Core.Configuration
{
    public record Cloud
    {
        [JsonPropertyName("currentProfile")]
        public string CurrentProfileName { get; init; } = "";

        public ImmutableSortedDictionary<string, CloudProfile> Profiles { get; init; } = ImmutableSortedDictionary<string, CloudProfile>.Empty;

        public ImmutableArray<CredentialType> CredentialPrecedence { get; init; } = ImmutableArray<CredentialType>.Empty;
    }

    public record CloudProfile
    {
        public string? ResourceManagerEndpoint { get; init; }

        public string? ActiveDirectoryAuthority { get; init; }
    }

    public class CloudConfiguration : ConfigurationSection<Cloud>, IEquatable<CloudConfiguration>
    {
        public CloudConfiguration(Cloud data, Uri resourceManagerEndpointUri, Uri activeDirectoryAuthorityUri)
            : base(data)
        {
            this.ResourceManagerEndpointUri = resourceManagerEndpointUri;
            this.ActiveDirectoryAuthorityUri = activeDirectoryAuthorityUri;
        }

        public ImmutableArray<CredentialType> CredentialPrecedence => this.Data.CredentialPrecedence;

        public Uri ResourceManagerEndpointUri { get; }

        public Uri ActiveDirectoryAuthorityUri { get; }

        // this is needed for all track 1 SDKs and track 2 management plane SDKs
        public string AuthenticationScope => ResourceManagerEndpointUri.AbsoluteUri + ".default";

        // this is needed for track 2 data plane SDKs
        // this does not work with regional ARM endpoints
        public string ResourceManagerAudience => ResourceManagerEndpointUri.AbsoluteUri.TrimEnd('/');

        public static CloudConfiguration Bind(JsonElement element, string? configurationPath)
        {
            var cloud = element.ToNonNullObject<Cloud>();
            var (endpointUri, authorityUri) = ValidateCurrentProfile(cloud, configurationPath);

            return new(cloud, endpointUri, authorityUri);
        }

        private static (Uri resourceManagerEndpointUri, Uri activeDirectoryAuthorityUri) ValidateCurrentProfile(Cloud cloud, string? configurationPath)
        {
            static string ToCamelCase(string name) => char.ToLowerInvariant(name[0]) + name[1..];

            string BuildAvailableProfileNamesClause() => cloud.Profiles.Keys.Any() ? $"\"{cloud.Profiles.Keys.OrderBy(name => name).ConcatString("\", \"")}\"" : "";

            string BuildConfigurationClause() => configurationPath is not null ? $"Bicep configuration \"{configurationPath}\"" : "built-in Bicep configuration";

            Uri ValidateUri(string? value, string propertyName)
            {
                if (value is null)
                {
                    throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" in the {BuildConfigurationClause()}. The \"{propertyName}\" property cannot be null or undefined.");
                }

                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" in the {BuildConfigurationClause()} is invalid. The value of the \"{propertyName}\" property \"{value}\" is not a valid URL.");
                }

                return uri;
            }

            if (!cloud.Profiles.TryGetValue(cloud.CurrentProfileName, out var currentProfile))
            {
                throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" does not exist in the {BuildConfigurationClause()}. Available profiles include {BuildAvailableProfileNamesClause()}.");
            }

            var endpointUri = ValidateUri(currentProfile.ResourceManagerEndpoint, ToCamelCase(nameof(currentProfile.ResourceManagerEndpoint)));
            var authorityUri = ValidateUri(currentProfile.ActiveDirectoryAuthority, ToCamelCase(nameof(currentProfile.ActiveDirectoryAuthority)));

            return (endpointUri, authorityUri);
        }

        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(this.ResourceManagerEndpointUri);
            hashCode.Add(this.ActiveDirectoryAuthorityUri);

            foreach (var credentialType in this.CredentialPrecedence)
            {
                hashCode.Add(credentialType);
            }

            return hashCode.ToHashCode();
        }

        public override bool Equals(object? obj) => obj is CloudConfiguration other && this.Equals(other);

        public bool Equals(CloudConfiguration? other) =>
            other is not null &&
            this.ResourceManagerEndpointUri.Equals(other.ResourceManagerEndpointUri) &&
            this.ActiveDirectoryAuthorityUri.Equals(other.ActiveDirectoryAuthorityUri) &&
            this.CredentialPrecedence.SequenceEqual(other.CredentialPrecedence);
    }
}
