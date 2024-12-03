// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;
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
        private readonly IOUri? configFileUri;

        private ModuleAliasesConfiguration(ModuleAliases data, IOUri? configFileUri)
            : base(data)
        {
            this.configFileUri = configFileUri;
        }

        public static ModuleAliasesConfiguration Bind(JsonElement element, IOUri? configFileUri) => new(element.ToNonNullObject<ModuleAliases>(), configFileUri);

        public ImmutableSortedDictionary<string, OciArtifactModuleAlias> GetOciArtifactModuleAliases()
        {
            return this.Data.OciArtifactModuleAliases;
        }

        public ImmutableSortedDictionary<string, TemplateSpecModuleAlias> GetTemplateSpecModuleAliases()
        {
            return this.Data.TemplateSpecModuleAliases;
        }

        public ResultWithDiagnosticBuilder<TemplateSpecModuleAlias> TryGetTemplateSpecModuleAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.TemplateSpecModuleAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.TemplateSpecModuleAliasNameDoesNotExistInConfiguration(aliasName, configFileUri));
            }

            if (alias.Subscription is null)
            {
                return new(x => x.InvalidTemplateSpecAliasSubscriptionNullOrUndefined(aliasName, configFileUri));
            }

            if (alias.ResourceGroup is null)
            {
                return new(x => x.InvalidTemplateSpecAliasResourceGroupNullOrUndefined(aliasName, configFileUri));
            }

            return new(alias);
        }

        public ResultWithDiagnosticBuilder<OciArtifactModuleAlias> TryGetOciArtifactModuleAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactModuleAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, configFileUri));
            }

            if (alias.Registry is null)
            {
                return new(x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, configFileUri));
            }

            return new(alias);
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out DiagnosticBuilderDelegate? errorBuilder)
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
