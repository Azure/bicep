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
        private CloudConfiguration(string currentProfile, ImmutableDictionary<string, CloudProfile> profiles, string resourceName)
        {
            this.CurrentProfileName = currentProfile;
            this.Profiles = profiles;
            this.ResourceName = resourceName;
            this.CurrentProfile = profiles.GetValueOrDefault(currentProfile);
        }

        public static CloudConfiguration Bind(IConfiguration rawConfiguration, string resourceName)
        {
            string currentProfile = rawConfiguration.GetValue("currentProfile", "AzureCloud");

            var profiles = new Dictionary<string, CloudProfile>();
            rawConfiguration.GetSection("profiles").Bind(profiles);

            return new(currentProfile, profiles.ToImmutableDictionary(), resourceName);
        }

        public CloudProfile? CurrentProfile { get; }

        public string CurrentProfileName { get; }

        public ImmutableDictionary<string, CloudProfile> Profiles { get; }

        public string ResourceName { get; }

        public string? TryGetCurrentResourceManagerEndpoint(out ErrorBuilderDelegate? errorBuilder)
        {
            if (!this.Profiles.TryGetValue(this.CurrentProfileName, out var currentProfile))
            {
                errorBuilder = x => x.CloudProfileDoesNotExistInConfiguration(this.CurrentProfileName, this.ResourceName, this.Profiles.Keys);
                return null;
            }

            if (currentProfile.ResourceManagerEndpoint is null)
            {
                errorBuilder = x => x.InvalidCloudProfileResourceManagerEndpointNullOrUndefined(this.CurrentProfileName, this.ResourceName);
                return null;
            }

            if (!Uri.TryCreate(currentProfile.ResourceManagerEndpoint, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                errorBuilder = x => x.InvalidCloudProfileInvalidResourceManagerEndpoint(this.CurrentProfileName, currentProfile.ResourceManagerEndpoint, this.ResourceName);
                return null;
            }

            errorBuilder = null;
            return currentProfile.ResourceManagerEndpoint;
        }
    }
}
