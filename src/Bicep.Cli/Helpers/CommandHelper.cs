// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Cli.Logging;
using Bicep.Core.FileSystem;
using Bicep.Core.Utils;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bicep.Cli.Helpers;

public static class CommandHelper
{
    public static int GetExitCode(DiagnosticSummary summary)
        // return non-zero exit code on errors
        => summary.HasErrors ? 1 : 0;

    public static IEnumerable<Uri> GetFilesMatchingPattern(IEnvironment environment, string? filePatternRoot, IEnumerable<string> filePatterns)
    {
        Matcher matcher = new();
        matcher.AddIncludePatterns(filePatterns);
        
        var fullRootPath = filePatternRoot is {} ? PathHelper.ResolvePath(filePatternRoot) : environment.CurrentDirectory;
        foreach (var filePath in matcher.GetResultsInFullPath(fullRootPath))
        {
            yield return ArgumentHelper.GetFileUri(filePath);
        }
    }

    public static Uri GetJsonOutputUri(Uri inputUri, string? outputDir, string? outputFile)
    {
        var outputPath = PathHelper.ResolveDefaultOutputPath(inputUri.LocalPath, outputDir, outputFile, PathHelper.GetDefaultBuildOutputPath);
        return PathHelper.FilePathToFileUrl(outputPath);
    }
}