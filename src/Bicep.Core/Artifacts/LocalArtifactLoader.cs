// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts;

/// <summary>
/// Loads local artifacts, which are never restored. A local module is loaded from its file; a local extension package is
/// decoded and extracted into the artifact cache on first use.
/// </summary>
public static class LocalArtifactLoader
{
    /// <summary>
    /// Gets the file the compiler loads for a local artifact: the module file, or the extracted extension's types.
    /// </summary>
    /// <param name="cacheRootDirectory">The root of the artifact cache of the referencing file.</param>
    public static ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPoint(LocalArtifactReference reference, IDirectoryHandle cacheRootDirectory) => reference.Type switch
    {
        ArtifactType.Module => new(reference.File),
        ArtifactType.Extension => TryExtractExtension(reference, cacheRootDirectory).Transform(x => x.EntryPoint),
        _ => throw new InvalidOperationException($"Unexpected artifact type '{reference.Type}'."),
    };

    private static ResultWithDiagnosticBuilder<LocalExtensionArtifactCacheEntry> TryExtractExtension(LocalArtifactReference reference, IDirectoryHandle cacheRootDirectory)
    {
        if (!reference.File.TryReadBinaryData().IsSuccess(out var packageData, out var readErrorBuilder))
        {
            return new(readErrorBuilder);
        }

        // The cache is content-addressed, so a package that has already been extracted is reused.
        var cacheEntry = new LocalExtensionArtifactCacheEntry(packageData, cacheRootDirectory);
        if (cacheEntry.IsComplete)
        {
            return new(cacheEntry);
        }

        ExtensionArtifactPackage package;
        try
        {
            package = LocalExtensionArtifactCodec.Decode(packageData);
        }
        catch (InvalidDataException exception)
        {
            // The gzip/tar decompression failed - the referenced file is not a valid archive (see https://github.com/Azure/bicep/issues/19165).
            return new(x => x.ArtifactRestoreFailedWithMessage(
                reference.FullyQualifiedReference,
                $"The file is not a valid extension package. Expected a gzip-compressed tar archive containing extension types. Error: {exception.Message}"));
        }
        catch (Exception exception)
        {
            // Any other failure reading the package (e.g. missing types.tgz entry, truncated content).
            return new(x => x.ArtifactRestoreFailedWithMessage(
                reference.FullyQualifiedReference,
                $"The file could not be read as a valid extension package. Error: {exception.Message}"));
        }

        try
        {
            cacheEntry.Write(package, reference);
        }
        catch (Exception exception) when (exception is ArtifactException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return new(x => x.ArtifactRestoreFailedWithMessage(reference.FullyQualifiedReference, exception.Message));
        }

        return new(cacheEntry);
    }
}
