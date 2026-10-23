// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Testing.IO;

public static class EmbeddedFileExtensions
{
    public static string GetContents(this ITestFile file)
        => file.Contents ?? throw new InvalidOperationException($"Test file '{file.Path}' was not found in assembly '{file.Assembly.FullName}'.");

    public static BinaryData GetBinaryData(this EmbeddedFile file)
        => BinaryData.FromStream(file.Assembly.GetManifestResourceStream(file.StreamPath) ?? throw new InvalidOperationException($"Embedded resource '{file.StreamPath}' was not found in assembly '{file.Assembly.FullName}'."));

    internal static string GetStreamDirectoryPath(this EmbeddedFile file)
        => (Path.GetDirectoryName(file.StreamPath) ?? "").Replace('\\', '/');

    internal static string GetPathRelativeToDirectory(this EmbeddedFile file, string streamDirectoryPath)
        => file.StreamPath[streamDirectoryPath.Length..].TrimStart('/');

    internal static IEnumerable<EmbeddedFile> GetDirectoryFiles(this EmbeddedFile file)
    {
        var directoryPrefix = $"{file.GetStreamDirectoryPath()}/";

        return EmbeddedFile.LoadAll(file.Assembly, streamPath => streamPath.StartsWith(directoryPrefix, StringComparison.Ordinal));
    }
}
