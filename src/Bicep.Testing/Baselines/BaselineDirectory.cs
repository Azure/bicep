// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Testing.Assertions;
using Bicep.Testing.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Testing.Baselines;

public record BaselineDirectory(
    string OutputDirectoryPath,
    string StreamDirectoryPath,
    ImmutableDictionary<string, BaselineFile> Files,
    BaselineFile EntryFile)
{
    internal static BaselineDirectory Materialize(TestContext testContext, EmbeddedFile embeddedFile)
    {
        var outputDirectory = testContext.GetUniqueOutputPath();
        var baselines = embeddedFile.GetDirectoryFiles().ToImmutableDictionary(
            file => file.GetPathRelativeToDirectory(embeddedFile.StreamDirectoryPath),
            file => new BaselineFile(
                testContext,
                file,
                testContext.SaveResultFile(file.GetPathRelativeToDirectory(embeddedFile.StreamDirectoryPath), file.Contents, outputDirectory)));

        return new(
            outputDirectory,
            embeddedFile.StreamDirectoryPath,
            baselines,
            baselines[embeddedFile.GetPathRelativeToDirectory(embeddedFile.StreamDirectoryPath)]);
    }

    public BaselineFile? TryGetFile(string relativePath)
        => Files.TryGetValue(relativePath);

    public BaselineFile GetFileForPath(string filePath) => GetFile(GetBaselineStreamRelativePath(filePath));

    public BaselineFile GetFile(string relativePath)
    {
        if (TryGetFile(relativePath) is { } baseline)
        {
            return baseline;
        }

        var embeddedFile = new EmbeddedFile(
            EntryFile.EmbeddedFile.Assembly,
            $"{StreamDirectoryPath}/{relativePath}");

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