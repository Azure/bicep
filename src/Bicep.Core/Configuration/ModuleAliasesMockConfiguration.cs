// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;
using static Bicep.Core.Diagnostics.DiagnosticBuilder;

namespace Bicep.Core.Configuration
{
    public record ModuleAliasesMock
    {
        [JsonPropertyName("br")]
        public ImmutableSortedDictionary<string, OciArtifactModuleAliasMock> OciArtifactModuleAliasesMock { get; init; } = ImmutableSortedDictionary<string, OciArtifactModuleAliasMock>.Empty;
    }

    public record OciArtifactModuleAliasMock
    {
        public string? MapToFilePath { get; init; }

        public override string ToString() => $"{MapToFilePath}";
    }

    public partial class ModuleAliasesMockConfiguration : ConfigurationSection<ModuleAliasesMock>
    {
        private readonly IOUri? configFileUri;

        private ModuleAliasesMockConfiguration(ModuleAliasesMock data, IOUri? configFileUri)
            : base(data)
        {
            this.configFileUri = configFileUri;
        }

        public static ModuleAliasesMockConfiguration Bind(JsonElement element, IOUri? configFileUri) => new(element.ToNonNullObject<ModuleAliasesMock>(), configFileUri);

        public ImmutableSortedDictionary<string, OciArtifactModuleAliasMock> GetOciArtifactModuleAliasesMock()
        {
            return this.Data.OciArtifactModuleAliasesMock;
        }

        public ResultWithDiagnosticBuilder<OciArtifactModuleAliasMock> TryGetOciArtifactModuleAliasMock(string aliasName)
        {
            if (!ValidateAliasName(aliasName, out var errorBuilder))
            {
                return new(errorBuilder);
            }

            if (!this.Data.OciArtifactModuleAliasesMock.TryGetValue(aliasName, out var alias))
            {
                return new(x => x.OciArtifactModuleAliasNameDoesNotExistInConfiguration(aliasName, configFileUri));
            }

            if (alias.MapToFilePath is null)
            {
                return new(x => x.InvalidOciArtifactModuleAliasRegistryNullOrUndefined(aliasName, configFileUri));
            }

            return new(alias);
        }

        private static bool ValidateAliasName(string aliasName, [NotNullWhen(false)] out DiagnosticBuilderDelegate? errorBuilder)
        {
            // To ensure consistency with module alias, we're referring to the same regex for validating alias names
            if (!ModuleAliasesConfiguration.ModuleAliasNameRegex().IsMatch(aliasName))
            {
                errorBuilder = x => x.InvalidModuleAliasName(aliasName);
                return false;
            }

            errorBuilder = null;
            return true;
        }
    }
}
