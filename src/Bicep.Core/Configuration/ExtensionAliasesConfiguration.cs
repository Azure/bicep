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
    public record ExtensionAliases
    {
        [JsonPropertyName("br")]
        public ImmutableSortedDictionary<string, OciArtifactExtensionAlias> OciArtifactExtensionAliases { get; init; } = ImmutableSortedDictionary<string, OciArtifactExtensionAlias>.Empty;
    }

    public record OciArtifactExtensionAlias
    {
        public string? Registry { get; init; }

        public string? ExtensionPath { get; init; }

        public override string ToString() => this.ExtensionPath is not null
            ? $"{Registry}/{this.ExtensionPath}"
            : $"{Registry}";
    }

    public partial class ExtensionAliasesConfiguration : ConfigurationSection<ExtensionAliases>
    {
        private readonly Uri? configFileUri;

        private ExtensionAliasesConfiguration(ExtensionAliases data, Uri? configFileUri)
            : base(data)
        {
            this.configFileUri = configFileUri;
        }
        public static ExtensionAliasesConfiguration Bind(JsonElement element, Uri? configFileUri) => new(element.ToNonNullObject<ExtensionAliases>(), configFileUri);

        public ImmutableSortedDictionary<string, OciArtifactExtensionAlias> OciArtifactExtensionAliases => this.Data.OciArtifactExtensionAliases;

        public ResultWithDiagnosticBuilder<OciArtifactExtensionAlias> TryGetOciArtifactExtensionAlias(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactExtensionAliases.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.OciArtifactExtensionAliasNameDoesNotExistInConfiguration(aliasName, configFileUri));
            }

            if (alias.Registry is null)
            {
                return new(x => x.InvalidOciArtifactExtensionAliasRegistryNullOrUndefined(aliasName, configFileUri));
            }

            return new(alias);
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out DiagnosticBuilderDelegate? errorBuilder)
        {
            if (!ExtensionAliasNameRegex().IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidExtensionAliasName(aliasName);
                return false;
            }

            errorBuilder = null;
            return true;
        }

        [GeneratedRegex("^[a-zA-Z0-9-_]+$", RegexOptions.CultureInvariant)]
        private static partial Regex ExtensionAliasNameRegex();


    }
}
