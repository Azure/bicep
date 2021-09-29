// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Bicep.Core.Configuration
{
    public record TemplateSpecModuleAlias
    {
        public string? Subscription { get; init; }

        public string? ResourceGroup { get; init; }
    }

    public record BicepRegistryModuleAlias
    {
        public string? Registry { get; init; }

        public string? ModulePath { get; init; }
    }


    public class ModuleAliasesConfiguration
    {
        private ModuleAliasesConfiguration(
            ImmutableDictionary<string, TemplateSpecModuleAlias> templateSpecModuleAliases,
            ImmutableDictionary<string, BicepRegistryModuleAlias> bicepRegistryModuleAliases)
        {
            this.TemplateSpecModuleAliases = templateSpecModuleAliases;
            this.BicepRegistryModuleAliases = bicepRegistryModuleAliases;
        }

        public static ModuleAliasesConfiguration Bind(IConfiguration rawConfiguration)
        {
            var templateSpecModuleAliases = new Dictionary<string, TemplateSpecModuleAlias>();
            rawConfiguration.GetSection("ts").Bind(templateSpecModuleAliases);

            var bicepRegistryModuleAliases = new Dictionary<string, BicepRegistryModuleAlias>();
            rawConfiguration.GetSection("br").Bind(bicepRegistryModuleAliases);

            return new(templateSpecModuleAliases.ToImmutableDictionary(), bicepRegistryModuleAliases.ToImmutableDictionary());
        }

        public ImmutableDictionary<string, TemplateSpecModuleAlias> TemplateSpecModuleAliases { get; }

        public ImmutableDictionary<string, BicepRegistryModuleAlias> BicepRegistryModuleAliases { get; }
    }
}
