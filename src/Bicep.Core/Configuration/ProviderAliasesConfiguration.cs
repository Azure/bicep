// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record ProviderAliases
    {
        [JsonPropertyName("br")]
        public ImmutableSortedDictionary<string, OciArtifactProviderAlias> OciArtifactExtensionAliases { get; init; } = ImmutableSortedDictionary<string, OciArtifactProviderAlias>.Empty;
    }

    public record OciArtifactProviderAlias
    {
        public string? Registry { get; init; }

        public string? ExtensionPath { get; init; }

        public override string ToString() => this.ExtensionPath is not null
            ? $"{Registry}/{this.ExtensionPath}"
            : $"{Registry}";
    }

    public partial class ProviderAliasesConfiguration : ConfigurationSection<ProviderAliases>
    {
        private readonly Uri? configFileUri;

        private ProviderAliasesConfiguration(ProviderAliases data, Uri? configFileUri)
            : base(data)
        {
            this.configFileUri = configFileUri;
        }
        public static ProviderAliasesConfiguration Bind(JsonElement element, Uri? configFileUri) => new(element.ToNonNullObject<ProviderAliases>(), configFileUri);

        public ImmutableSortedDictionary<string, OciArtifactProviderAlias> OciArtifactExtensionAliases => this.Data.OciArtifactExtensionAliases;

        public ResultWithDiagnostic<OciArtifactProviderAlias> TryGetOciArtifactExtensionAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactExtensionAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.OciArtifactProviderAliasNameDoesNotExistInConfiguration(aliasName, configFileUri));
            }

            if (alias.Registry is null)
            {
                return new(x => x.InvalidOciArtifactProviderAliasRegistryNullOrUndefined(aliasName, configFileUri));
            }

            return new(alias);
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out ErrorBuilderDelegate? errorBuilder)
        {
            if (!ProviderAliasNameRegex().IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidProviderAliasName(aliasName);
                return false;
            }

            errorBuilder = null;
            return true;
        }

        [GeneratedRegex("^[a-zA-Z0-9-_]+$", RegexOptions.CultureInvariant)]
        private static partial Regex ProviderAliasNameRegex();


    }
}
