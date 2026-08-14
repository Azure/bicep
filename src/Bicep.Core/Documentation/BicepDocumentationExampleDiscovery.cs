// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Documentation;

internal static partial class BicepDocumentationExampleDiscovery
{
    private static readonly ImmutableArray<string> CategoryFolderNames = ["examples", "tests"];

    public static ImmutableArray<BicepDocumentationUsageExample> Discover(IDirectoryHandle moduleRoot)
    {
        var examples = ImmutableArray.CreateBuilder<BicepDocumentationUsageExample>();

        foreach (var categoryFolderName in CategoryFolderNames)
        {
            var categoryRoot = moduleRoot.GetDirectory(categoryFolderName);
            if (!categoryRoot.Exists())
            {
                continue;
            }

            foreach (var file in EnumerateBicepFiles(categoryRoot))
            {
                var relativePath = file.Uri.GetPathRelativeTo(moduleRoot.Uri);
                var name = GetExampleName(categoryRoot.Uri, file.Uri);
                var contents = file.ReadAllText();

                examples.Add(new BicepDocumentationUsageExample(name, relativePath, TryGetDescription(contents), contents));
            }
        }

        return BicepDocumentationOrdering.SortByName(examples.ToImmutable(), e => e.RelativePath);
    }

    private static IEnumerable<IFileHandle> EnumerateBicepFiles(IDirectoryHandle directory)
    {
        foreach (var file in directory.EnumerateFiles("*")
            .Where(file => file.Uri.Path.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase)))
        {
            yield return file;
        }

        foreach (var subdirectory in directory.EnumerateDirectories("*"))
        {
            foreach (var file in EnumerateBicepFiles(subdirectory))
            {
                yield return file;
            }
        }
    }

    private static string GetExampleName(IOUri categoryRoot, IOUri file)
    {
        var relativeToCategory = file.GetPathRelativeTo(categoryRoot);
        var segments = relativeToCategory.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 1)
        {
            return segments[0];
        }

        var fileName = segments[^1];

        return fileName[..^".bicep".Length];
    }

    // Avoids a full compile: uses a literal `metadata description = '...'` if present, else leading `//` comments.
    private static string? TryGetDescription(string contents)
    {
        var match = MetadataDescriptionPattern().Match(contents);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        var leadingComment = contents
            .ReplaceLineEndings("\n")
            .Split('\n')
            .TakeWhile(line => line.TrimStart().StartsWith("//", StringComparison.Ordinal) || line.Trim().Length == 0)
            .Select(line => line.TrimStart().TrimStart('/').Trim())
            .Where(line => line.Length > 0)
            .ToArray();

        return leadingComment.Length > 0 ? string.Join(' ', leadingComment) : null;
    }

    [GeneratedRegex("""metadata\s+description\s*=\s*'((?:[^'\\]|\\.)*)'""")]
    private static partial Regex MetadataDescriptionPattern();
}
