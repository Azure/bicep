// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record ModuleAlias { }

    public record TemplateSpecModuleAlias : ModuleAlias
    {
        public string? Subscription { get; init; }

        public string? ResourceGroup { get; init; }

        public override string ToString()
        {
            Debug.Assert(Subscription is not null, "TemplateSpecModuleAlias.Subscription should already be validated and cannot be null.");

            return this.ResourceGroup is null
                ? $"{Subscription}/{ResourceGroup}"
                : $"{Subscription}";
        }
    }

    public record OciArtifactModuleAlias : ModuleAlias
    {
        public string? Registry { get; init; }

        public string? ModulePath { get; init; }

        public override string ToString()
        {
            Debug.Assert(Registry is not null, "BicepRegistryModuleAlias.Registry should already be validated and cannot be null.");

            return this.ModulePath is null
                ? $"{Registry}/{ModulePath}"
                : $"{Registry}";
        }
    }


    public class ModuleAliasesConfiguration
    {
        private static readonly Regex ModuleAliasNameRegex = new(@"\w-");

        private ModuleAliasesConfiguration(
            ImmutableDictionary<string, TemplateSpecModuleAlias> templateSpecModuleAliases,
            ImmutableDictionary<string, OciArtifactModuleAlias> ociArtifactModuleAliases,
            string resourceName)
        {
            this.TemplateSpecModuleAliases = templateSpecModuleAliases;
            this.OciArtifactModuleAliases = ociArtifactModuleAliases;
            this.ResourceName = resourceName;
        }

        public static ModuleAliasesConfiguration Bind(IConfiguration rawConfiguration, string resourceName)
        {
            var templateSpecModuleAliases = new Dictionary<string, TemplateSpecModuleAlias>();
            rawConfiguration.GetSection(ModuleReferenceSchemes.TemplateSpecs).Bind(templateSpecModuleAliases);

            var bicepRegistryModuleAliases = new Dictionary<string, OciArtifactModuleAlias>();
            rawConfiguration.GetSection(ModuleReferenceSchemes.Oci).Bind(bicepRegistryModuleAliases);

            return new(templateSpecModuleAliases.ToImmutableDictionary(), bicepRegistryModuleAliases.ToImmutableDictionary(), resourceName);
        }

        // TODO: do not expose the two dictionaries as properties.
        public ImmutableDictionary<string, TemplateSpecModuleAlias> TemplateSpecModuleAliases { get; }

        public ImmutableDictionary<string, OciArtifactModuleAlias> OciArtifactModuleAliases { get; }

        /// <summary>
        /// Gets the built-in configuraiton manifest resource name if the configuration is a built-in configuraiton,
        /// or a path to a bicepconfig.json file if the configuration is a custom one.
        /// </summary>
        public string ResourceName { get; }

        public TemplateSpecModuleAlias? TryGetTemplateSpecModuleAlias(string aliasName, out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ModuleAliasNameRegex.IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return null;
            }

            if (!this.TemplateSpecModuleAliases.TryGetValue(aliasName, out var alias))
            {
                errorBuilder = x => x.TemplateSpecModuleAliasNameDoesNotExistInConfiguration(aliasName, this.ResourceName);
                return null;
            }

            if (alias.Subscription is null)
            {
                errorBuilder = x => x.InvalidTemplateSpecAliasSubscriptionNullOrUndefined(aliasName, this.ResourceName);
                return null;
            }

            errorBuilder = null;
            return alias;
        }

        public OciArtifactModuleAlias? TryGetOciArtifactModuleAlias(string aliasName, out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ModuleAliasNameRegex.IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return null;
            }

            if (!this.OciArtifactModuleAliases.TryGetValue(aliasName, out var alias))
            {
                errorBuilder = x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, this.ResourceName);
                return null;
            }

            if (alias.Registry is null)
            {
                errorBuilder = x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, this.ResourceName);
                return null;
            }

            errorBuilder = null;
            return alias;
        }
    }
}
