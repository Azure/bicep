// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.Core.SourceLink
{

    [JsonSerializable(typeof(SourceArchiveMetadata))]
    [JsonSourceGenerationOptions(
        PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
        UseStringEnumConverter = true)]
    public partial class SourceArchiveSerializationContext : JsonSerializerContext { }

    /// <summary>
    /// Metadata for the entire archive (stored in __metadata.json file in archive)
    /// </summary>
    /// <param name="MetadataVersion">Version of the metadata schema.</param>
    /// <param name="BicepVersion">Version of Bicep used to generate the metadata.</param>
    /// <param name="EntryPoint">Relative path to entry point source file.</param>
    /// <param name="SourceFiles">Source file metadata.</param>
    /// <param name="DocumentLinks"></param>
    public record SourceArchiveMetadata(
        int MetadataVersion,
        string? BicepVersion,
        string EntryPoint, // Path of the entrypoint file
        IEnumerable<LinkedSourceFile> SourceFiles,
        IReadOnlyDictionary<string, SourceCodeDocumentPathLink[]>? DocumentLinks = null) // Maps source file relative path -> array of document links inside that file
    {
        /* Example __metadata.json
        {
            "metadataVersion": 0,
            "entryPoint": "my entrypoint.bicep",
            "bicepVersion": "0.18.19",
            "sourceFiles": [
                {
                    "sourcePath": "my entrypoint.bicep",
                    "archivePath": "files/my entrypoint.bicep",
                    "kind": "bicep"
                },
                {
                    "sourcePath": "modules/main.bicep",
                    "archivePath": "files/modules/main.bicep",
                    "kind": "bicep"
                },
                {
                  "path": "\u003Ccache\u003E/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
                  "archivePath": "files/_cache_/br/mcr.microsoft.com/bicep$app$app-configuration/1.0.1$/main.json",
                  "kind": "armTemplate",
                  "sourceArtifactId": "br:contoso.io/test/module1:v1"
                },
            ],
            "DocumentLinks": { ... }
        }
        */
    }
}
