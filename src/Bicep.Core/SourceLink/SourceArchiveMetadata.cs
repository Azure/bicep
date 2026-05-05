// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;

namespace Bicep.Core.SourceLink
{

    [JsonSerializable(typeof(SourceArchiveMetadata))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        UseStringEnumConverter = true)]
    public partial class SourceArchiveMetadataSerializationContext : JsonSerializerContext { }

    /// <summary>
    /// Metadata for the entire archive (stored in __metadata.json file in archive)
    /// </summary>
    /// <param name="MetadataVersion">The version of the archive metadata schema.</param>
    /// <param name="BicepVersion">The version of Bicep used to generate the archive metadata.</param>
    /// <param name="EntryPoint">The relative path to the entry point source file in the archive.</param>
    /// <param name="SourceFiles">Metadata for each source file included in the archive..</param>
    /// <param name="DocumentLinks">A mapping of source file relative paths to their associated document links.</param>
    /// <example>
    /// Example of the `__metadata.json` content:
    /// {
    ///   "metadataVersion": 0,
    ///   "entryPoint": "my-entrypoint.bicep",
    ///   "bicepVersion": "0.18.19",
    ///   "sourceFiles": [
    ///     {
    ///         "path": "my entrypoint.bicep",
    ///         "archivePath": "files/my entrypoint.bicep",
    ///         "kind": "bicep"
    ///     },
    ///     {
    ///         "path": "modules/main.bicep",
    ///         "archivePath": "files/modules/main.bicep",
    ///         "kind": "bicep"
    ///     },
    ///     {
    ///       "path": "\u003Ccache\u003E/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
    ///       "archivePath": "files/_cache_/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
    ///       "kind": "armTemplate",
    ///       "sourceArtifactId": "br:contoso.io/test/module1:v1"
    ///     }
    ///   ],
    ///   "documentLinks": { ... }
    /// }
    /// </example>
    public record SourceArchiveMetadata(
        int MetadataVersion,
        string? BicepVersion,
        string EntryPoint,
        ImmutableArray<ArchivedSourceFileMetadata> SourceFiles,
        ImmutableDictionary<string, ImmutableArray<ArchivedSourceFileLink>>? DocumentLinks)
    {
        public string BicepVersion { get; } = BicepVersion ?? "unknown";

        public ImmutableArray<ArchivedSourceFileMetadata> SourceFiles { get; } = [.. SourceFiles.OrderBy(x => x.Path).ThenBy(x => x.ArchivePath)];

        public ImmutableDictionary<string, ImmutableArray<ArchivedSourceFileLink>> DocumentLinks { get; } = DocumentLinks ?? [];
    }
}
