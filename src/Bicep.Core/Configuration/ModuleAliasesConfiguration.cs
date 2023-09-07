// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record ModuleAliases
    {
        [JsonPropertyName("ts")]
        public ImmutableSortedDictionary<string, TemplateSpecModuleAlias> TemplateSpecModuleAliases { get; init; } = ImmutableSortedDictionary<string, TemplateSpecModuleAlias>.Empty;

        [JsonPropertyName("br")]
        public ImmutableSortedDictionary<string, OciArtifactModuleAlias> OciArtifactModuleAliases { get; init; } = ImmutableSortedDictionary<string, OciArtifactModuleAlias>.Empty;
    }

    public record TemplateSpecModuleAlias
    {
        public string? Subscription { get; init; }

        public string? ResourceGroup { get; init; }

        public override string ToString() => $"{Subscription}/{ResourceGroup}";
    }

    public record OciArtifactModuleAlias
    {
        public string? Registry { get; init; }

        public string? ModulePath { get; init; }

        public override string ToString() => this.ModulePath is not null
            ? $"{Registry}/{ModulePath}"
            : $"{Registry}";
    }

    public partial class ModuleAliasesConfiguration : ConfigurationSection<ModuleAliases>
    {
        private readonly string? configurationPath;

        private ModuleAliasesConfiguration(ModuleAliases data, string? configurationPath)
            : base(data)
        {
            this.configurationPath = configurationPath;
        }

        public static ModuleAliasesConfiguration Bind(JsonElement element, string? configurationPath) => new(element.ToNonNullObject<ModuleAliases>(), configurationPath);

        public ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetOciArtifactModuleAliases()
        {
            return this.Data.OciArtifactModuleAliases;
        }

        public ImmutableSortedDictionary<string, TemplateSpecModuleAlias> GetTemplateSpecModuleAliases()
        {
            return this.Data.TemplateSpecModuleAliases;
        }

        public ResultWithDiagnostic<TemplateSpecModuleAlias> TryGetTemplateSpecModuleAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.TemplateSpecModuleAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.TemplateSpecModuleAliasNameDoesNotExistInConfiguration(aliasName, this.configurationPath));
            }

            if (alias.Subscription is null)
            {
                return new(x => x.InvalidTemplateSpecAliasSubscriptionNullOrUndefined(aliasName, this.configurationPath));
            }

            if (alias.ResourceGroup is null)
            {
                return new(x => x.InvalidTemplateSpecAliasResourceGroupNullOrUndefined(aliasName, this.configurationPath));
            }

            return new(alias);
        }

        public ResultWithDiagnostic<OciArtifactModuleAlias> TryGetOciArtifactModuleAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactModuleAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, this.configurationPath));
            }

            if (alias.Registry is null)
            {
                return new(x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, this.configurationPath));
            }

            return new(alias);
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ModuleAliasNameRegex().IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return false;
            }

            errorBuilder = null;
            return true;
        }

        [GeneratedRegex("^[a-zA-Z0-9-_]+$", RegexOptions.CultureInvariant)]
        private static partial Regex ModuleAliasNameRegex();
    }
}
