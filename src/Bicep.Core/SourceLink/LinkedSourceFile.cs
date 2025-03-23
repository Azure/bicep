// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Text.Json.Serialization;

namespace Bicep.Core.SourceLink
{
    /// <summary>
    /// A single SourceFiles entry in the metadata.json file.
    /// </summary>
    /// <param name="Path">The path relative to the main.bicep file's folder, for the file that will be shown to the end user (required in all Bicep versions).</param>
    /// <param name="ArchivePath">The path relative to root of where the file is stored in the archive.</param>
    /// <param name="Kind">Kind of linked source file.</param>
    /// <param name="SourceArtifactId">Points to an external artifact that contains the source for this module (e.g. "br:contoso.io/test/module1:v1"), appears in v0.26 and higher.</param>
    public record LinkedSourceFile(
        // IF ADDING TO THIS: Remember both forwards and backwards compatibility.
        // E.g., previous versions must be able to deal with unrecognized source kinds.
        // (but see CurrentMetadataVersion for breaking changes)
        string Path,
        string ArchivePath,
        LinkedSourceFileKind Kind,
        string? SourceArtifactId = null);
}
