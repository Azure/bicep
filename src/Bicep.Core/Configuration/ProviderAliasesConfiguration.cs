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
        public ImmutableSortedDictionary<string, OciArtifactProviderAlias> OciArtifactProviderAliases { get; init; } = ImmutableSortedDictionary<string, OciArtifactProviderAlias>.Empty;
    }

    public record OciArtifactProviderAlias
    {
        public string? Registry { get; init; }
        public string? ProviderPath { get; init; }
        public override string ToString() => this.ProviderPath is not null
            ? $"{Registry}/{ProviderPath}"
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

        public ImmutableSortedDictionary<string, OciArtifactProviderAlias> GetOciArtifactProviderAliases()
        {
            return this.Data.OciArtifactProviderAliases;
        }

        public ResultWithDiagnostic<OciArtifactProviderAlias> TryGetOciArtifactProviderAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactProviderAliases.TryGetValue(aliasName, out var alias))
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
