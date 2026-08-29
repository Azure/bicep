// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Core.Registry.Oci;

/// <summary>
/// Layout of the individual layers of a layered module artifact within the module cache.
/// </summary>
public static class OciLayerCache
{
    public const string LayersDirectoryName = "layers";

    /// <summary>
    /// Digests contain a colon, which is not a valid file name character on Windows.
    /// </summary>
    public static string GetLayerFileName(string digest) => $"{digest.Replace(':', '$')}.json";

    public static IDirectoryHandle GetLayersDirectory(IDirectoryHandle artifactDirectory) =>
        artifactDirectory.GetDirectory(LayersDirectoryName);

    public static IFileHandle? TryGetLayerFile(IDirectoryHandle artifactDirectory, string digest)
    {
        var file = GetLayersDirectory(artifactDirectory).GetFile(GetLayerFileName(digest));

        return file.Exists() ? file : null;
    }
}
