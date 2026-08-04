// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Testing.Assertions;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing.Baselines;

public record BaselineFileSet(
    string OutputDirectoryPath,
    string StreamFolderPath,
    ImmutableDictionary<string, BaselineFile> Files,
    BaselineFile EntryFile)
{
    internal static BaselineFileSet Materialize(TestContext testContext, TestEmbeddedFile embeddedFile)
    {
        var outputDirectory = testContext.GetUniqueOutputPath();
        var parentStream = Path.GetDirectoryName(embeddedFile.StreamPath)!.Replace('\\', '/');
        var entryFileRelativePath = embeddedFile.StreamPath.Substring(parentStream.Length).TrimStart('/');

        var baselineFiles = new Dictionary<string, BaselineFile>();
        foreach (var streamPath in embeddedFile.Assembly.GetManifestResourceNames()
            .Where(file => file.StartsWith(parentStream, StringComparison.Ordinal)))
        {
            var relativePath = streamPath.Substring(parentStream.Length).TrimStart('/');
            var filePath = Path.Combine(outputDirectory, relativePath);

            baselineFiles[relativePath] = new(
                testContext,
                new TestEmbeddedFile(embeddedFile.Assembly, streamPath),
                filePath);
        }

        foreach (var baselineFile in baselineFiles.Values)
        {
            var directoryPath = Path.GetDirectoryName(baselineFile.OutputFilePath)!;
            Directory.CreateDirectory(directoryPath);

            File.WriteAllText(baselineFile.OutputFilePath, baselineFile.EmbeddedFile.Contents);
            testContext.AddResultFile(baselineFile.OutputFilePath);
        }

        return new(
            outputDirectory,
            parentStream,
            baselineFiles.ToImmutableDictionary(),
            baselineFiles[entryFileRelativePath]);
    }

    public BaselineFile? TryGetFile(string relativePath)
        => Files.TryGetValue(relativePath);

    public BaselineFile GetFileForPath(string filePath) => GetFile(GetBaselineStreamRelativePath(filePath));

    public BaselineFile GetFile(string relativePath)
    {
        if (TryGetFile(relativePath) is { } baselineFile)
        {
            return baselineFile;
        }

        var embeddedFile = new TestEmbeddedFile(
            EntryFile.EmbeddedFile.Assembly,
            $"{StreamFolderPath}/{relativePath}");

        var outputFile = Path.Combine(OutputDirectoryPath, relativePath);
        File.WriteAllText(outputFile, "");

        "".Should().MatchTextBaseline(
            EntryFile.TestContext,
            "<missing>",
            expectedPath: embeddedFile.RelativeSourcePath,
            actualPath: outputFile);
        throw new NotImplementedException("Code cannot reach this point as the previous line will always throw");
    }

    private string GetBaselineStreamRelativePath(string filePath)
        => filePath.StartsWith(OutputDirectoryPath) ?
        filePath.Substring(OutputDirectoryPath.Length).Replace('\\', '/').TrimStart('/') :
        throw new InvalidOperationException($"FilePath {filePath} is not a sub-path of {OutputDirectoryPath}");
}
