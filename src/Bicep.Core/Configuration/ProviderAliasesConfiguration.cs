// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
        private readonly string? configurationPath;

        private ProviderAliasesConfiguration(ProviderAliases data, string? configurationPath)
            : base(data)
        {
            this.configurationPath = configurationPath;
        }
        public static ProviderAliasesConfiguration Bind(JsonElement element, string? configurationPath) => new(element.ToNonNullObject<ProviderAliases>(), configurationPath);

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
                return new(x => x.OciArtifactProviderAliasNameDoesNotExistInConfiguration(aliasName, this.configurationPath));
            }

            if (alias.Registry is null)
            {
                return new(x => x.InvalidOciArtifactProviderAliasRegistryNullOrUndefined(aliasName, this.configurationPath));
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
