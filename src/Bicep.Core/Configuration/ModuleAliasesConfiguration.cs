// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

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

    public class ModuleAliasesConfiguration : ConfigurationSection<ModuleAliases>
    {
        private static readonly Regex ModuleAliasNameRegex = new(@"^[a-zA-Z0-9-_]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly string? configurationPath;

        private ModuleAliasesConfiguration(ModuleAliases data, string? configurationPath)
            : base(data)
        {
            this.configurationPath = configurationPath;
        }

        public static ModuleAliasesConfiguration Bind(JsonElement element, string? configurationPath) => new(element.ToNonNullObject<ModuleAliases>(), configurationPath);

        public bool TryGetTemplateSpecModuleAlias(string aliasName, [NotNullWhen(true)] out TemplateSpecModuleAlias? alias, [NotNullWhen(false)] out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ValidateAliasName(aliasName, out errorBuilder))
            {
                alias = null;
                return false;
            }

            if (!this.Data.TemplateSpecModuleAliases.TryGetValue(aliasName, out alias))
            {
                errorBuilder = x => x.TemplateSpecModuleAliasNameDoesNotExistInConfiguration(aliasName, this.configurationPath);
                return false;
            }

            if (alias.Subscription is null)
            {
                errorBuilder = x => x.InvalidTemplateSpecAliasSubscriptionNullOrUndefined(aliasName, this.configurationPath);
                return false;
            }

            if (alias.ResourceGroup is null)
            {
                errorBuilder = x => x.InvalidTemplateSpecAliasResourceGroupNullOrUndefined(aliasName, this.configurationPath);
                return false;
            }

            errorBuilder = null;
            return true;
        }

        public bool TryGetOciArtifactModuleAlias(string aliasName, [NotNullWhen(true)] out OciArtifactModuleAlias? alias, [NotNullWhen(false)] out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ValidateAliasName(aliasName, out errorBuilder))
            {
                alias = null;
                return false;
            }

            if (!this.Data.OciArtifactModuleAliases.TryGetValue(aliasName, out alias))
            {
                errorBuilder = x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, this.configurationPath);
                return false;
            }

            if (alias.Registry is null)
            {
                errorBuilder = x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, this.configurationPath);
                return false;
            }

            errorBuilder = null;
            return true;
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ModuleAliasNameRegex.IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return false;
            }

            errorBuilder = null;
            return true;
        }
    }
}
