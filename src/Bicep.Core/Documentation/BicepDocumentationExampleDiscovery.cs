// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Parsing;
using Bicep.Core.Syntax;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bicep.Core.Documentation;

internal static class BicepDocumentationExampleDiscovery
{
    private const int MaxDirectoryDepth = 100;

    public static ImmutableArray<BicepDocumentationUsageExample> Discover(
        IDirectoryHandle moduleRoot,
        Func<IOUri, bool>? shouldSkip = null) =>
        Discover(moduleRoot, new(), shouldSkip);

    public static ImmutableArray<BicepDocumentationUsageExample> Discover(
        IDirectoryHandle moduleRoot,
        BicepDocumentationExamplesConfiguration configuration,
        Func<IOUri, bool>? shouldSkip = null)
    {
        try
        {
            var sources = configuration.Sources.IsDefault
                ? new BicepDocumentationExamplesConfiguration().Sources
                : configuration.Sources;
            var discovered = DiscoverLocalFiles(moduleRoot, sources, shouldSkip);
            ApplyParentReassignments(moduleRoot, configuration, discovered);
            ApplyChildReassignments(moduleRoot, configuration, sources, discovered, shouldSkip);

            return BicepDocumentationOrdering.SortByName(
                discovered.Values.Select(item => BuildExample(moduleRoot, item)).ToImmutableArray(),
                example => example.RelativePath);
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

    private static Dictionary<IOUri, DiscoveredFile> DiscoverLocalFiles(
        IDirectoryHandle moduleRoot,
        ImmutableArray<BicepDocumentationExampleSource> sources,
        Func<IOUri, bool>? shouldSkip)
    {
        var discovered = new Dictionary<IOUri, DiscoveredFile>();
        foreach (var source in sources)
        {
            if (source is null || string.IsNullOrWhiteSpace(source.Path))
            {
                throw new BicepDocumentationException("Usage-example source paths cannot be empty.");
            }

            var sourceRoot = moduleRoot.GetDirectory(source.Path);
            if (!sourceRoot.Exists())
            {
                continue;
            }

            var matcher = CreateMatcher(source.Include, source.Exclude);
            foreach (var file in EnumerateFiles(sourceRoot, shouldSkip))
            {
                var sourceRelativePath = file.Uri.GetPathRelativeTo(sourceRoot.Uri);
                if (matcher.Match(sourceRelativePath).HasMatches)
                {
                    discovered.TryAdd(file.Uri, new(file, sourceRoot.Uri, sourceRelativePath));
                }
            }
        }

        return discovered;
    }

    private static Matcher CreateMatcher(ImmutableArray<string> includes, ImmutableArray<string> excludes)
    {
        try
        {
            var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
            foreach (var include in includes.IsDefault ? [] : includes)
            {
                matcher.AddInclude(include);
            }

            foreach (var exclude in excludes.IsDefault ? [] : excludes)
            {
                matcher.AddExclude(exclude);
            }

            return matcher;
        }
        catch (ArgumentException exception)
        {
            throw new BicepDocumentationException($"Invalid usage-example glob: {exception.Message}", exception);
        }
    }

    private static void ApplyParentReassignments(
        IDirectoryHandle moduleRoot,
        BicepDocumentationExamplesConfiguration configuration,
        Dictionary<IOUri, DiscoveredFile> discovered)
    {
        if (configuration.Reassignments.IsDefaultOrEmpty)
        {
            return;
        }

        foreach (var reassignment in configuration.Reassignments)
        {
            ValidateReassignment(reassignment);
            if (!moduleRoot.GetDirectory(reassignment.To).Exists())
            {
                continue;
            }

            var matcher = CreateMatcher(reassignment.From.Include, reassignment.From.Exclude);
            foreach (var fileUri in discovered
                .Where(item => matcher.Match(item.Value.SourceRelativePath).HasMatches)
                .Select(item => item.Key)
                .ToArray())
            {
                discovered.Remove(fileUri);
            }
        }
    }

    private static void ApplyChildReassignments(
        IDirectoryHandle moduleRoot,
        BicepDocumentationExamplesConfiguration configuration,
        ImmutableArray<BicepDocumentationExampleSource> sources,
        Dictionary<IOUri, DiscoveredFile> discovered,
        Func<IOUri, bool>? shouldSkip)
    {
        if (configuration.Reassignments.IsDefaultOrEmpty)
        {
            return;
        }

        if (moduleRoot.GetParent() is not { } parentRoot)
        {
            return;
        }

        ImmutableDictionary<IOUri, DiscoveredFile>? parentFiles = null;
        foreach (var reassignment in configuration.Reassignments)
        {
            ValidateReassignment(reassignment);
            if (!parentRoot.GetDirectory(reassignment.To).Uri.Equals(moduleRoot.Uri))
            {
                continue;
            }

            parentFiles ??= DiscoverLocalFiles(parentRoot, sources, shouldSkip).ToImmutableDictionary();
            var matcher = CreateMatcher(reassignment.From.Include, reassignment.From.Exclude);
            foreach (var item in parentFiles.Where(item => matcher.Match(item.Value.SourceRelativePath).HasMatches))
            {
                discovered.TryAdd(item.Key, item.Value);
            }
        }
    }

    private static void ValidateReassignment(BicepDocumentationExampleReassignment reassignment)
    {
        if (reassignment is null ||
            reassignment.From is null ||
            string.IsNullOrWhiteSpace(reassignment.To) ||
            reassignment.To.IndexOfAny(['/', '\\']) >= 0 ||
            reassignment.To is "." or "..")
        {
            throw new BicepDocumentationException("Usage-example reassignments must identify one child module directory.");
        }
    }

    private static BicepDocumentationUsageExample BuildExample(
        IDirectoryHandle moduleRoot,
        DiscoveredFile discovered)
    {
        string contents;
        try
        {
            contents = discovered.File.ReadAllText();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new BicepDocumentationException(
                $"Unable to read usage example '{discovered.File.Uri}': {exception.Message}",
                exception);
        }

        var metadata = GetStringMetadata(contents);
        var name = metadata.GetValueOrDefault("name") ?? GetExampleName(discovered.SourceRoot, discovered.File.Uri);

        return new(
            name,
            discovered.File.Uri.GetPathRelativeTo(moduleRoot.Uri),
            metadata.GetValueOrDefault("description") ?? TryGetLeadingComment(contents),
            contents.TrimEnd());
    }

    private static IEnumerable<IFileHandle> EnumerateFiles(
        IDirectoryHandle directory,
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

            foreach (var file in current.Directory.EnumerateFiles("*"))
            {
                if (shouldSkip?.Invoke(file.Uri) != true)
                {
                    yield return file;
                }
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

    private static string GetExampleName(IOUri sourceRoot, IOUri file)
    {
        var relativeToSource = file.GetPathRelativeTo(sourceRoot);
        var segments = relativeToSource.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length > 1)
        {
            return segments[^2];
        }

        var fileName = segments[^1];
        return fileName.EndsWith(".bicep", StringComparison.OrdinalIgnoreCase)
            ? fileName[..^".bicep".Length]
            : fileName;
    }

    private static ImmutableDictionary<string, string> GetStringMetadata(string contents)
    {
        var metadataValues = ImmutableDictionary.CreateBuilder<string, string>(LanguageConstants.IdentifierComparer);
        foreach (var metadata in new Parser(contents).Program().Declarations.OfType<MetadataDeclarationSyntax>())
        {
            if (metadata.Value is StringSyntax stringSyntax &&
                stringSyntax.TryGetLiteralValue() is { } value)
            {
                metadataValues[metadata.Name.IdentifierName] = value;
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

    private sealed record DiscoveredFile(
        IFileHandle File,
        IOUri SourceRoot,
        string SourceRelativePath);
}
