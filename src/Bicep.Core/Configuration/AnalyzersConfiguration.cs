// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;

namespace Bicep.Core.Configuration
{
    public class AnalyzersConfiguration
    {
        private readonly IConfiguration? rawConfiguration;

        public AnalyzersConfiguration(IConfiguration? rawConfiguration)
        {
            this.rawConfiguration = rawConfiguration;
        }

        public T GetValue<T>(string key, T defaultValue) => this.rawConfiguration is not null
            ? this.rawConfiguration.GetSection(key).Get<T>() ?? defaultValue
            : defaultValue;
    }
}
