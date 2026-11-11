// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Cache;
using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph.ArtifactReferences;
using Bicep.Core.SourceGraph.Artifacts;
using Bicep.Core.Utils;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.References;

/// <summary>
/// A reference to a file on disk, such as a relative path or a <c>moduleAliasesMock</c> hit. Local references are never restored.
/// </summary>
/// <remarks>
/// Two references are equal if they name the same file as the same artifact type, however they were written.
/// </remarks>
public sealed class LocalArtifactReference : ArtifactReference, IExtensionArtifactReference
{
    private readonly string displayReference;
    private readonly IDirectoryHandle cacheRootDirectory;
    private readonly Lazy<ResultWithDiagnosticBuilder<LocalExtensionArtifactCacheEntry>> lazyExtensionArtifact;

    /// <param name="cacheRootDirectory">The root of the local artifact cache, which extension packages are unpacked into.</param>
    /// <param name="displayReference">How the reference is shown to the user, for example in diagnostics.</param>
    public LocalArtifactReference(IDirectoryHandle cacheRootDirectory, ArtifactType type, IFileHandle file, string displayReference)
        : base(ArtifactSchemes.Local, type)
    {
        this.cacheRootDirectory = cacheRootDirectory;
        this.File = file;
        this.displayReference = displayReference;
        this.lazyExtensionArtifact = new(this.TryUnpackExtension);
    }

    public IFileHandle File { get; }

    public override string FullyQualifiedReference => this.displayReference;

    public override string UnqualifiedReference => this.displayReference;

    public ResultWithDiagnosticBuilder<IFileHandle> TryGetEntryPointFileHandle() => this.Type switch
    {
        ArtifactType.Module => new(this.File),
        // An extension package is unpacked into the cache on first use. Its entry point is the unpacked types.tgz.
        ArtifactType.Extension => this.lazyExtensionArtifact.Value.Transform(x => x.TypesTgzFile),
        _ => throw new InvalidOperationException($"Unexpected artifact type '{this.Type}'."),
    };

    public IExtensionArtifact ResolveExtensionArtifact() => this.Type == ArtifactType.Extension
        ? this.lazyExtensionArtifact.Value.Unwrap()
        : throw new InvalidOperationException("Cannot resolve extension artifact for local module reference.");

    public override bool Equals(object? obj) =>
        obj is LocalArtifactReference other &&
        this.Type == other.Type &&
        this.File.Uri.Equals(other.File.Uri);

    public override int GetHashCode() => HashCode.Combine(this.Type, this.File.Uri);

    private ResultWithDiagnosticBuilder<LocalExtensionArtifactCacheEntry> TryUnpackExtension()
    {
        if (!this.File.TryReadBinaryData().IsSuccess(out var binaryData, out var readErrorBuilder))
        {
            return new(readErrorBuilder);
        }

        // The cache is content-addressed, so a package that has already been unpacked is reused.
        var artifact = new LocalExtensionArtifactCacheEntry(binaryData, this.cacheRootDirectory);
        if (artifact.IsComplete)
        {
            return new(artifact);
        }

        ExtensionArtifactPackage package;
        try
        {
            package = LocalExtensionArtifactCodec.Decode(binaryData);
        }
        catch (InvalidDataException exception)
        {
            // The gzip/tar decompression failed - the referenced file is not a valid archive (see https://github.com/Azure/bicep/issues/19165).
            return new(x => x.ArtifactRestoreFailedWithMessage(
                this.FullyQualifiedReference,
                $"The file is not a valid extension package. Expected a gzip-compressed tar archive containing extension types. Error: {exception.Message}"));
        }
        catch (Exception exception)
        {
            // Any other failure reading the package (e.g. missing types.tgz entry, truncated content).
            return new(x => x.ArtifactRestoreFailedWithMessage(
                this.FullyQualifiedReference,
                $"The file could not be read as a valid extension package. Error: {exception.Message}"));
        }

        try
        {
            artifact.Unpack(package, this.FullyQualifiedReference);
        }
        catch (Exception exception) when (exception is ArtifactException or InvalidOperationException or IOException or UnauthorizedAccessException)
        {
            return new(x => x.ArtifactRestoreFailedWithMessage(this.FullyQualifiedReference, exception.Message));
        }

        return new(artifact);
    }
}
