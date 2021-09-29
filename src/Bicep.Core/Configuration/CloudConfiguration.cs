// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Configuration
{
    public record CloudProfile
    {
        public string ResourceManagerEndpoint { get; init; } = "";
    }

    public class CloudConfiguration
    {
        private CloudConfiguration(string currentProfile, IReadOnlyDictionary<string, CloudProfile> profiles)
        {
            this.CurrentProfile = profiles.GetValueOrDefault(currentProfile);
        }

        public static CloudConfiguration Bind(IConfiguration rawConfiguration)
        {
            string currentProfile = rawConfiguration.GetValue("currentProfile", "AzureCloud");

            var profiles = new Dictionary<string, CloudProfile>();
            rawConfiguration.GetSection("profiles").Bind(profiles);

            return new(currentProfile, profiles);
        }

        public CloudProfile? CurrentProfile { get; }
    }
}
