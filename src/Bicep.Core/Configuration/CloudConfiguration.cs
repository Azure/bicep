// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record CloudProfile
    {
        public string? ResourceManagerEndpoint { get; init; }
    }

    public class CloudConfiguration
    {
        // this is a workaround for unusual array merging behavior in the underlying config library
        private static readonly ImmutableArray<CredentialType> DefaultCredentialPrecedence = new[]{
            CredentialType.AzureCLI,
            CredentialType.AzurePowerShell
        }.ToImmutableArray();

        private CloudConfiguration(string currentProfile, ImmutableDictionary<string, CloudProfile> profiles, string? configurationPath, ImmutableArray<CredentialType> credentialPrecedence)
        {
            this.CurrentProfileName = currentProfile;
            this.Profiles = profiles;
            this.ConfigurationPath = configurationPath;
            this.CurrentProfile = profiles.GetValueOrDefault(currentProfile);
            this.CredentialPrecedence = credentialPrecedence.ToImmutableArray();
        }

        public static CloudConfiguration Bind(IConfiguration rawConfiguration, string? configurationPath)
        {
            string currentProfile = rawConfiguration.GetValue("currentProfile", "AzureCloud");

            var profiles = new Dictionary<string, CloudProfile>();
            rawConfiguration.GetSection("profiles").Bind(profiles);

            var credentialPrecedence = rawConfiguration.GetSection("credentialPrecedence")?.Get<CredentialType[]>()?.ToImmutableArray() ?? DefaultCredentialPrecedence;

            return new(currentProfile, profiles.ToImmutableDictionary(), configurationPath, credentialPrecedence);
        }

        public CloudProfile? CurrentProfile { get; }

        public string CurrentProfileName { get; }

        public ImmutableDictionary<string, CloudProfile> Profiles { get; }

        public string? ConfigurationPath { get; }

        public ImmutableArray<CredentialType> CredentialPrecedence { get; }

        public string? TryGetCurrentResourceManagerEndpoint(out ErrorBuilderDelegate? errorBuilder)
        {
            if (!this.Profiles.TryGetValue(this.CurrentProfileName, out var currentProfile))
            {
                errorBuilder = x => x.CloudProfileDoesNotExistInConfiguration(this.CurrentProfileName, this.ConfigurationPath, this.Profiles.Keys);
                return null;
            }

            if (currentProfile.ResourceManagerEndpoint is null)
            {
                errorBuilder = x => x.InvalidCloudProfileResourceManagerEndpointNullOrUndefined(this.CurrentProfileName, this.ConfigurationPath);
                return null;
            }

            if (!Uri.TryCreate(currentProfile.ResourceManagerEndpoint, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                errorBuilder = x => x.InvalidCloudProfileInvalidResourceManagerEndpoint(this.CurrentProfileName, currentProfile.ResourceManagerEndpoint, this.ConfigurationPath);
                return null;
            }

            errorBuilder = null;
            return currentProfile.ResourceManagerEndpoint;
        }
    }
}
