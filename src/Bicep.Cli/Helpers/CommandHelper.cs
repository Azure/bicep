// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
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

    public static IEnumerable<(Uri inputUri, Uri outputUri)> GetInputAndOutputFilesForPattern(IEnvironment environment, string? filePattern, string? outputDir, Func<string, string> pathReplacementFunc)
    {
        if (filePattern is null)
        {
            yield break;
        }

        var (inputBasePath, relativeInputPaths) = GetFilesMatchingPattern(environment, filePattern);
        foreach (var relativeInputPath in relativeInputPaths)
        {
            var relativeOutputPath = pathReplacementFunc(relativeInputPath);
            var inputUri = PathHelper.FilePathToFileUrl(Path.Combine(inputBasePath, relativeInputPath));
            var outputUri = PathHelper.FilePathToFileUrl(Path.Combine(outputDir ?? inputBasePath, relativeOutputPath));

            yield return (inputUri, outputUri);
        }
    }

    public static IEnumerable<Uri> GetInputFilesForPattern(IEnvironment environment, string? filePattern)
    {
        if (filePattern is null)
        {
            yield break;
        }

        var (inputBasePath, relativeInputPaths) = GetFilesMatchingPattern(environment, filePattern);
        foreach (var relativeInputPath in relativeInputPaths)
        {
            var inputUri = PathHelper.FilePathToFileUrl(Path.Combine(inputBasePath, relativeInputPath));

            yield return inputUri;
        }
    }

    public static (string rootPath, ImmutableArray<string> relativePaths) GetFilesMatchingPattern(IEnvironment environment, string filePattern)
    {
        var (rootPath, remainingPath) = SplitFilePatternOnWildcard(filePattern, Path.DirectorySeparatorChar);
        rootPath = PathHelper.ResolvePath(rootPath, baseDirectory: environment.CurrentDirectory);
        var rootUri = PathHelper.FilePathToFileUrl(rootPath + Path.DirectorySeparatorChar);

        Matcher matcher = new();
        matcher.AddInclude(remainingPath);

        var relativePaths = ImmutableArray.CreateBuilder<string>();
        foreach (var filePath in matcher.GetResultsInFullPath(rootPath))
        {
            var relativePath = PathHelper.GetRelativePath(rootUri, ArgumentHelper.GetFileUri(filePath));
            relativePaths.Add(relativePath);
        }

        return (rootPath, relativePaths.ToImmutable());
    }

    public static Uri GetJsonOutputUri(Uri inputUri, string? outputDir, string? outputFile)
    {
        var outputPath = PathHelper.ResolveOutputPath(inputUri.LocalPath, outputDir, outputFile, PathHelper.GetJsonOutputPath, null);
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
