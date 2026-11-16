// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.Artifacts.Codecs;
using Bicep.Core.Artifacts.Packages;
using Bicep.Core.Artifacts.References;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Artifacts.Cache;

/// <summary>
/// The layout of an extension in its cache entry, shared by OCI and local extension entries: <c>types.tgz</c> and, if the
/// extension has a binary for this machine, <c>extension.bin</c>.
/// </summary>
public sealed class ExtensionArtifactCacheLayout
{
    public ExtensionArtifactCacheLayout(IDirectoryHandle directory)
    {
        this.TypesTgzFile = directory.GetFile("types.tgz");
        this.BinaryFile = directory.GetFile("extension.bin");
    }

    public IFileHandle TypesTgzFile { get; }

    public IFileHandle BinaryFile { get; }

    /// <summary>
    /// Writes the extension's types, and this machine's binary if the extension has binaries.
    /// </summary>
    /// <param name="reference">The extension's reference, for error messages.</param>
    /// <exception cref="InvalidOperationException">The extension has binaries, but none for this machine's OS and architecture.</exception>
    public void Write(ExtensionArtifactPackage package, ArtifactReference reference)
    {
        // Pick the binary before writing anything, so that an unsupported machine leaves no extension files behind.
        if (SelectBinary(package, reference) is { } binary)
        {
            this.BinaryFile.Write(binary.Data);
            this.BinaryFile.MakeExecutable();
        }

        // types.tgz is written last: a local extension's entry is complete once it exists.
        this.TypesTgzFile.Write(package.Types);
    }

    public ExtensionArtifact Open() =>
        new(this.TypesTgzFile, new(() => this.BinaryFile.Exists() ? this.BinaryFile : null));

    private static ExtensionBinary? SelectBinary(ExtensionArtifactPackage package, ArtifactReference reference)
    {
        if (!package.Binaries.Any())
        {
            return null;
        }

        if (SupportedArchitectures.TryGetCurrent() is not { } architecture)
        {
            throw new InvalidOperationException($"Failed to determine the system OS or architecture to execute extension \"{reference.FullyQualifiedReference}\".");
        }

        return package.Binaries.FirstOrDefault(x => x.Architecture.Name == architecture.Name) ??
            throw new InvalidOperationException($"The extension \"{reference.FullyQualifiedReference}\" does not support architecture {architecture.Name}.");
    }
}
