// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core;
using Bicep.Core.Extensions;
using Bicep.Core.Parsing;

namespace Bicep.Core.Configuration
{
    public record Cloud
    {
        [JsonPropertyName("currentProfile")]
        public string CurrentProfileName { get; init; } = "";

        public ImmutableSortedDictionary<string, CloudProfile> Profiles { get; init; } = ImmutableSortedDictionary<string, CloudProfile>.Empty;

        public ImmutableArray<CredentialType> CredentialPrecedence { get; init; } = [];

        public CredentialOptions? CredentialOptions { get; init; }
    }

    public record CloudProfile(string? ResourceManagerEndpoint, string? ActiveDirectoryAuthority);

    public record CredentialOptions(ManagedIdentity? ManagedIdentity);

    public record ManagedIdentity(ManagedIdentityType Type, string? ClientId, string? ResourceId);

    public class CloudConfiguration : ConfigurationSection<Cloud>, IEquatable<CloudConfiguration>
    {
        public CloudConfiguration(Cloud data, Uri resourceManagerEndpointUri, Uri activeDirectoryAuthorityUri)
            : base(data)
        {
            this.ResourceManagerEndpointUri = resourceManagerEndpointUri;
            this.ActiveDirectoryAuthorityUri = activeDirectoryAuthorityUri;
        }

        public ImmutableArray<CredentialType> CredentialPrecedence => this.Data.CredentialPrecedence;

        public CredentialOptions? CredentialOptions => this.Data.CredentialOptions;

        public Uri ResourceManagerEndpointUri { get; }

        public Uri ActiveDirectoryAuthorityUri { get; }

        // this is needed for all track 1 SDKs and track 2 management plane SDKs
        public string AuthenticationScope => ResourceManagerEndpointUri.AbsoluteUri;

        // this is needed for track 2 data plane SDKs
        // this does not work with regional ARM endpoints
        public string ResourceManagerAudience => ResourceManagerEndpointUri.AbsoluteUri.TrimEnd('/');

        public static CloudConfiguration Bind(JsonElement element)
        {
            var cloud = element.ToNonNullObject<Cloud>();
            var (endpointUri, authorityUri) = ValidateCurrentProfile(cloud);
            ValidateCredentialOptions(cloud);

            return new(cloud, endpointUri, authorityUri);
        }

        private static void ValidateCredentialOptions(Cloud cloud)
        {
            if (cloud.CredentialOptions is null ||
                cloud.CredentialOptions.ManagedIdentity is null ||
                cloud.CredentialOptions.ManagedIdentity.Type is ManagedIdentityType.SystemAssigned)
            {
                return;
            }

            var clientId = cloud.CredentialOptions.ManagedIdentity.ClientId;
            var resourceId = cloud.CredentialOptions.ManagedIdentity.ResourceId;

            if (clientId is null && resourceId is null)
            {
                throw new ConfigurationException($@"The managed-identity configuration is invalid. Either ""{nameof(clientId)}"" or ""{nameof(resourceId)}"" must be set for user-assigned identity.");
            }

            if (clientId is not null && resourceId is not null)
            {
                throw new ConfigurationException($@"The managed-identity configuration is invalid. ""{nameof(clientId)}"" and ""{nameof(resourceId)}"" cannot be set at the same time for user-assigned identity.");
            }

            if (clientId is not null && !Guid.TryParse(clientId, out _))
            {
                throw new ConfigurationException($@"The managed-identity configuration is invalid. ""{nameof(clientId)}"" must be a GUID.");
            }

            if (resourceId is not null && !ResourceIdentifier.TryParse(resourceId, out _))
            {
                throw new ConfigurationException($@"The managed-identity configuration is invalid. ""{nameof(resourceId)}"" must be a valid Azure resource identifier.");
            }
        }

        private static (Uri resourceManagerEndpointUri, Uri activeDirectoryAuthorityUri) ValidateCurrentProfile(Cloud cloud)
        {
            string BuildAvailableProfileNamesClause() => cloud.Profiles.Keys.Any() ? $"\"{cloud.Profiles.Keys.OrderBy(name => name).ConcatString("\", \"")}\"" : "";

            Uri ValidateUri(string? value, string propertyName)
            {
                if (value is null)
                {
                    throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" is invalid. The \"{propertyName}\" property cannot be null or undefined.");
                }

                if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" is invalid. The value of the \"{propertyName}\" property \"{value}\" is not a valid URL.");
                }

                return uri;
            }

            if (!cloud.Profiles.TryGetValue(cloud.CurrentProfileName, out var currentProfile))
            {
                throw new ConfigurationException($"The cloud profile \"{cloud.CurrentProfileName}\" does not exist. Available profiles include {BuildAvailableProfileNamesClause()}.");
            }

            var endpointUri = ValidateUri(currentProfile.ResourceManagerEndpoint, StringUtils.ToCamelCase(nameof(currentProfile.ResourceManagerEndpoint)));
            var authorityUri = ValidateUri(currentProfile.ActiveDirectoryAuthority, StringUtils.ToCamelCase(nameof(currentProfile.ActiveDirectoryAuthority)));

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
