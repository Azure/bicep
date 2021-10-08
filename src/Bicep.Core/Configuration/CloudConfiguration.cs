// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Extensions;
using System;
using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

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
    }

    public class CloudConfiguration : ConfigurationSection<Cloud>
    {
        private readonly string? configurationPath;

        public CloudConfiguration(Cloud data, string? configurationPath)
            : base(data)
        {
            this.configurationPath = configurationPath;
        }

        public ImmutableArray<CredentialType> CredentialPrecedence => this.Data.CredentialPrecedence;

        public static CloudConfiguration Bind(JsonElement element, string? configurationPath) => new(element.ToNonNullObject<Cloud>(), configurationPath);

        public string? TryGetCurrentResourceManagerEndpoint(out ErrorBuilderDelegate? errorBuilder)
        {
            if (!this.Data.Profiles.TryGetValue(this.Data.CurrentProfileName, out var currentProfile))
            {
                errorBuilder = x => x.CloudProfileDoesNotExistInConfiguration(this.Data.CurrentProfileName, this.configurationPath, this.Data.Profiles.Keys);
                return null;
            }

            if (currentProfile.ResourceManagerEndpoint is null)
            {
                errorBuilder = x => x.InvalidCloudProfileResourceManagerEndpointNullOrUndefined(this.Data.CurrentProfileName, this.configurationPath);
                return null;
            }

            if (!Uri.TryCreate(currentProfile.ResourceManagerEndpoint, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                errorBuilder = x => x.InvalidCloudProfileInvalidResourceManagerEndpoint(this.Data.CurrentProfileName, currentProfile.ResourceManagerEndpoint, this.configurationPath);
                return null;
            }

            errorBuilder = null;
            return currentProfile.ResourceManagerEndpoint;
        }
    }
}
