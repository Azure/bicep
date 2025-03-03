// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Logging;
using Bicep.Core.FileSystem;
using Bicep.Core.Semantics;
using Bicep.Core.Utils;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

namespace Bicep.Cli.Helpers;

public static class CommandHelper
{
    public static int GetExitCode(DiagnosticSummary summary)
        // return non-zero exit code on errors
        => summary.HasErrors ? 1 : 0;

    public static (string rootPath, string remainingPath) SplitFilePatternOnWildcard(string filePattern, char osPathSeparator)
    {
        var wildcardIndex = filePattern.IndexOf('*');
        if (wildcardIndex == -1)
        {
            wildcardIndex = filePattern.Length;
        }

        // We intentionally want different behavior on different OSes.
        // Linux treats \ as a regular character, while Windows treats it as a path separator.
        var prevDirIndex = filePattern[..wildcardIndex].LastIndexOfAny(['/', osPathSeparator]);
        var rootPath = prevDirIndex != -1 ? filePattern[..prevDirIndex] : "";
        var remainingPath = prevDirIndex != -1 ? filePattern[(prevDirIndex + 1)..] : filePattern;

        return (rootPath, remainingPath);
    }

    public static IEnumerable<Uri> GetFilesMatchingPattern(IEnvironment environment, string? filePattern)
    {
        if (filePattern is null)
        {
            yield break;
        }

        var (rootPath, remainingPath) = SplitFilePatternOnWildcard(filePattern, Path.DirectorySeparatorChar);
        rootPath = PathHelper.ResolvePath(rootPath, baseDirectory: environment.CurrentDirectory);

        Matcher matcher = new();
        matcher.AddInclude(remainingPath);

        foreach (var filePath in matcher.GetResultsInFullPath(rootPath))
        {
            yield return ArgumentHelper.GetFileUri(filePath);
        }
    }

    public static Uri GetJsonOutputUri(Uri inputUri, string? outputDir, string? outputFile)
    {
        var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, outputDir, outputFile, PathHelper.GetDefaultBuildOutputPath);
        return PathHelper.FilePathToFileUrl(outputPath);
    }

    public static void LogExperimentalWarning(ILogger logger, Compilation compilation)
    {
        if (ExperimentalFeatureWarningProvider.TryGetEnabledExperimentalFeatureWarningMessage(compilation.SourceFileGrouping.SourceFiles) is { } warningMessage)
        {
            logger.LogWarning(warningMessage);
        }
    }
}
