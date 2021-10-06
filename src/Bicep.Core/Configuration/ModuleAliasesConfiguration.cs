// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Modules;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record ModuleAlias { }

    public record TemplateSpecModuleAlias : ModuleAlias
    {
        public string? Subscription { get; init; }

        public string? ResourceGroup { get; init; }

        public override string ToString() => $"{Subscription}/{ResourceGroup}";
    }

    public record OciArtifactModuleAlias : ModuleAlias
    {
        public string? Registry { get; init; }

        public string? ModulePath { get; init; }

        public override string ToString() => this.ModulePath is not null
            ? $"{Registry}/{ModulePath}"
            : $"{Registry}";
    }

    public class ModuleAliasesConfiguration
    {
        private static readonly Regex ModuleAliasNameRegex = new(@"^[a-zA-Z0-9-_]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private ModuleAliasesConfiguration(
            ImmutableDictionary<string, TemplateSpecModuleAlias> templateSpecModuleAliases,
            ImmutableDictionary<string, OciArtifactModuleAlias> ociArtifactModuleAliases,
            string? configurationPath)
        {
            this.TemplateSpecModuleAliases = templateSpecModuleAliases;
            this.OciArtifactModuleAliases = ociArtifactModuleAliases;
            this.ConfigurationPath = configurationPath;
        }

        public static ModuleAliasesConfiguration Bind(IConfiguration rawConfiguration, string? configurationPath)
        {
            var templateSpecModuleAliases = new Dictionary<string, TemplateSpecModuleAlias>();
            rawConfiguration.GetSection(ModuleReferenceSchemes.TemplateSpecs).Bind(templateSpecModuleAliases);

            var bicepRegistryModuleAliases = new Dictionary<string, OciArtifactModuleAlias>();
            rawConfiguration.GetSection(ModuleReferenceSchemes.Oci).Bind(bicepRegistryModuleAliases);

            return new(templateSpecModuleAliases.ToImmutableDictionary(), bicepRegistryModuleAliases.ToImmutableDictionary(), configurationPath);
        }

        // TODO: do not expose the two dictionaries as properties.
        public ImmutableDictionary<string, TemplateSpecModuleAlias> TemplateSpecModuleAliases { get; }

        public ImmutableDictionary<string, OciArtifactModuleAlias> OciArtifactModuleAliases { get; }

        public string? ConfigurationPath { get; }

        public ModuleAlias? TryGetModuleAlias(string scheme, string aliasName, out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ModuleAliasNameRegex.IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return null;
            }

            return scheme switch
            {
                ModuleReferenceSchemes.TemplateSpecs => TryGetTemplateSpecModuleAlias(aliasName, out errorBuilder),
                ModuleReferenceSchemes.Oci => TryGetOciArtifactModuleAlias(aliasName, out errorBuilder),
                _ => throw new ArgumentException("Unknown module reference scheme {}."),
            };
        }

        private TemplateSpecModuleAlias? TryGetTemplateSpecModuleAlias(string aliasName, out ErrorBuilderDelegate? errorBuilder)
        {
            if (!this.TemplateSpecModuleAliases.TryGetValue(aliasName, out var alias))
            {
                errorBuilder = x => x.TemplateSpecModuleAliasNameDoesNotExistInConfiguration(aliasName, this.ConfigurationPath);
                return null;
            }

            if (alias.Subscription is null)
            {
                errorBuilder = x => x.InvalidTemplateSpecAliasSubscriptionNullOrUndefined(aliasName, this.ConfigurationPath);
                return null;
            }

            if (alias.ResourceGroup is null)
            {
                errorBuilder = x => x.InvalidTemplateSpecAliasResourceGroupNullOrUndefined(aliasName, this.ConfigurationPath);
                return null;
            }

            errorBuilder = null;
            return alias;
        }

        private OciArtifactModuleAlias? TryGetOciArtifactModuleAlias(string aliasName, out ErrorBuilderDelegate? errorBuilder)
        {
            if (!this.OciArtifactModuleAliases.TryGetValue(aliasName, out var alias))
            {
                errorBuilder = x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, this.ConfigurationPath);
                return null;
            }

            if (alias.Registry is null)
            {
                errorBuilder = x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, this.ConfigurationPath);
                return null;
            }

            errorBuilder = null;
            return alias;
        }
    }
}
