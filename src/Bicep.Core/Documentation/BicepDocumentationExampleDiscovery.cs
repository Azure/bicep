// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;

namespace Bicep.Core.Documentation;

internal static class BicepDocumentationExampleDiscovery
{
    private static readonly ImmutableArray<string> CategoryFolderNames = ["examples", "tests"];
    private const int MaxDirectoryDepth = 100;

    public static ImmutableArray<BicepDocumentationUsageExample> Discover(
        IDirectoryHandle moduleRoot,
        Func<IOUri, bool>? shouldSkip = null)
    {
        try
        {
            var examples = ImmutableArray.CreateBuilder<BicepDocumentationUsageExample>();

            foreach (var categoryFolderName in CategoryFolderNames)
            {
                var categoryRoot = moduleRoot.GetDirectory(categoryFolderName);
                if (!categoryRoot.Exists())
                {
                    continue;
                }

                foreach (var file in EnumerateBicepFiles(categoryRoot, categoryFolderName, shouldSkip))
                {
                    var relativePath = file.Uri.GetPathRelativeTo(moduleRoot.Uri);
                    string contents;
                    try
                    {
                        contents = file.ReadAllText();
                    }
                    catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
                    {
                        throw new BicepDocumentationException($"Unable to read usage example '{file.Uri}': {exception.Message}", exception);
                    }

                    var metadata = GetStringMetadata(contents);
                    var name = metadata.GetValueOrDefault("name") ?? GetExampleName(categoryRoot.Uri, file.Uri);

                    examples.Add(new BicepDocumentationUsageExample(
                        name,
                        relativePath,
                        metadata.GetValueOrDefault("description") ?? TryGetLeadingComment(contents),
                        contents.TrimEnd()));
                }
            }

            return BicepDocumentationOrdering.SortByName(examples.ToImmutable(), e => e.RelativePath);
        }
        catch (BicepDocumentationException)
        {
            throw;
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new BicepDocumentationException(
                $"Unable to discover usage examples under '{moduleRoot.Uri}': {exception.Message}",
                exception);
        }
    }

    private static IEnumerable<IFileHandle> EnumerateBicepFiles(
        IDirectoryHandle directory,
        string categoryFolderName,
        Func<IOUri, bool>? shouldSkip)
    {
        var pending = new Stack<(IDirectoryHandle Directory, int Depth)>();
        pending.Push((directory, 0));

        while (pending.TryPop(out var current))
        {
            if (current.Depth > MaxDirectoryDepth)
            {
                throw new BicepDocumentationException(
                    $"Usage example discovery exceeded the maximum directory depth of {MaxDirectoryDepth} under '{directory.Uri}'.");
            }

            foreach (var file in current.Directory.EnumerateFiles("*")
                .Where(file =>
                    shouldSkip?.Invoke(file.Uri) != true &&
                    IsExampleEntrypoint(file, categoryFolderName, current.Depth)))
            {
                yield return file;
            }

            foreach (var subdirectory in current.Directory.EnumerateDirectories("*"))
            {
                if (shouldSkip?.Invoke(subdirectory.Uri) != true)
                {
                    pending.Push((subdirectory, current.Depth + 1));
                }
            }
        }
    }

    private static bool IsExampleEntrypoint(IFileHandle file, string categoryFolderName, int depth)
    {
        var fileName = file.Uri.GetFileName();
        if (!fileName.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase) ||
            fileName.StartsWith("dependencies", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return categoryFolderName.Equals("tests", StringComparison.OrdinalIgnoreCase)
            ? fileName.EndsWith(".test.bicep", StringComparison.OrdinalIgnoreCase)
            : depth == 0 || fileName.Equals("main.bicep", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetExampleName(IOUri categoryRoot, IOUri file)
    {
        var relativeToCategory = file.GetPathRelativeTo(categoryRoot);
        var segments = relativeToCategory.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 1)
        {
            return segments[^2];
        }

        var fileName = segments[^1];

        return fileName[..^".bicep".Length];
    }

    private static ImmutableDictionary<string, string> GetStringMetadata(string contents)
    {
        var metadataValues = ImmutableDictionary.CreateBuilder<string, string>(LanguageConstants.IdentifierComparer);
        foreach (var metadata in new Parser(contents).Program().Declarations.OfType<MetadataDeclarationSyntax>())
        {
            if (metadata.Value is not StringSyntax stringSyntax ||
                stringSyntax.TryGetLiteralValue() is not { } value)
            {
                continue;
            }

            if (!metadataValues.TryAdd(metadata.Name.IdentifierName, value))
            {
                throw new BicepDocumentationException(
                    $"Usage example metadata '{metadata.Name.IdentifierName}' is declared more than once.");
            }
        }

        return metadataValues.ToImmutable();
    }

    private static string? TryGetLeadingComment(string contents)
    {
        var leadingComment = contents
            .ReplaceLineEndings("\n")
            .Split('\n')
            .TakeWhile(line => line.TrimStart().StartsWith("//", StringComparison.Ordinal) || line.Trim().Length == 0)
            .Select(line => line.TrimStart().TrimStart('/').Trim())
            .Where(line => line.Length > 0)
            .ToArray();

        return leadingComment.Length > 0 ? string.Join(' ', leadingComment) : null;
    }
}
