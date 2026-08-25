// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.Text.Json;
using Bicep.Core.Extensions;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bicep.Core.Configuration;

/// <summary>
/// Configures module documentation generation.
/// </summary>
public sealed record Documentation
{
    /// <summary>
    /// Gets output settings.
    /// </summary>
    public DocumentationOutput Output { get; init; } = new();

    /// <summary>
    /// Gets template settings.
    /// </summary>
    public DocumentationTemplate Template { get; init; } = new();

    /// <summary>
    /// Gets usage-example settings.
    /// </summary>
    public DocumentationExamples Examples { get; init; } = new();
}

/// <summary>
/// Configures generated documentation output.
/// </summary>
public sealed record DocumentationOutput
{
    /// <summary>
    /// Gets the generated file name.
    /// </summary>
    public string File { get; init; } = "README.md";
}

/// <summary>
/// Configures documentation templates.
/// </summary>
public sealed record DocumentationTemplate
{
    /// <summary>
    /// Gets an optional Scriban template file.
    /// </summary>
    public string? File { get; init; }

    /// <summary>
    /// Gets an optional root directory for template includes.
    /// </summary>
    public string? IncludeRoot { get; init; }

    /// <summary>
    /// Gets baseline custom template values.
    /// </summary>
    public ImmutableSortedDictionary<string, string> Values { get; init; } =
        ImmutableSortedDictionary<string, string>.Empty.WithComparers(StringComparer.Ordinal);
}

/// <summary>
/// Configures usage-example discovery.
/// </summary>
public sealed record DocumentationExamples
{
    /// <summary>
    /// Gets discovery sources relative to each module root.
    /// </summary>
    public ImmutableArray<DocumentationExampleSource> Sources { get; init; } =
    [
        new()
        {
            Path = "examples",
            Include = ["*.bicep", "**/main.bicep"],
            Exclude = ["**/dependencies*.bicep"],
        },
        new()
        {
            Path = "tests",
            Include = ["**/*.test.bicep"],
            Exclude = ["**/dependencies*.bicep"],
        },
    ];

    /// <summary>
    /// Gets conditional parent-to-child example reassignments.
    /// </summary>
    public ImmutableArray<DocumentationExampleReassignment> Reassignments { get; init; } = [];
}

/// <summary>
/// Defines one usage-example discovery source.
/// </summary>
public sealed record DocumentationExampleSource
{
    /// <summary>
    /// Gets the source path relative to a module root.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// Gets included example globs.
    /// </summary>
    public ImmutableArray<string> Include { get; init; } = [];

    /// <summary>
    /// Gets excluded example globs.
    /// </summary>
    public ImmutableArray<string> Exclude { get; init; } = [];
}

/// <summary>
/// Reassigns matching parent examples to one child module directory.
/// </summary>
public sealed record DocumentationExampleReassignment
{
    /// <summary>
    /// Gets patterns selecting examples from a parent module.
    /// </summary>
    public DocumentationPatternSet From { get; init; } = new();

    /// <summary>
    /// Gets the destination directory relative to the parent module root.
    /// </summary>
    public required string To { get; init; }
}

/// <summary>
/// Includes and excludes paths using glob patterns.
/// </summary>
public sealed record DocumentationPatternSet
{
    /// <summary>
    /// Gets included path globs.
    /// </summary>
    public ImmutableArray<string> Include { get; init; } = [];

    /// <summary>
    /// Gets excluded path globs.
    /// </summary>
    public ImmutableArray<string> Exclude { get; init; } = [];
}

/// <summary>
/// Provides the documentation section of a Bicep configuration.
/// </summary>
public sealed class DocumentationConfiguration : ConfigurationSection<Documentation>, IBicepDocumentationConfiguration
{
    public DocumentationConfiguration(Documentation data)
        : base(data)
    {
    }

    public static DocumentationConfiguration Bind(JsonElement element)
    {
        var data = Normalize(element.ToNonNullObject<Documentation>());
        Validate(data);

        return new(data);
    }

    private static Documentation Normalize(Documentation data)
    {
        if (data.Output is null || data.Template is null || data.Examples is null)
        {
            throw new ConfigurationException("The documentation output, template, and examples properties cannot be null.");
        }

        if (data.Template.Values is null)
        {
            throw new ConfigurationException("The documentation template.values property cannot be null.");
        }

        return data with
        {
            Template = data.Template with
            {
                Values = data.Template.Values.ToImmutableSortedDictionary(StringComparer.Ordinal),
            },
            Examples = data.Examples with
            {
                Sources = NormalizeSources(data.Examples.Sources),
                Reassignments = NormalizeReassignments(data.Examples.Reassignments),
            },
        };
    }

    private static ImmutableArray<DocumentationExampleSource> NormalizeSources(
        ImmutableArray<DocumentationExampleSource> sources)
    {
        var normalized = ImmutableArray.CreateBuilder<DocumentationExampleSource>(sources.Length);
        foreach (var source in sources)
        {
            if (source is null)
            {
                throw new ConfigurationException("The documentation examples.sources property cannot contain null values.");
            }
            normalized.Add(source);
        }

        return normalized.ToImmutable();
    }

    private static ImmutableArray<DocumentationExampleReassignment> NormalizeReassignments(
        ImmutableArray<DocumentationExampleReassignment> reassignments)
    {
        var normalized = ImmutableArray.CreateBuilder<DocumentationExampleReassignment>(reassignments.Length);
        foreach (var reassignment in reassignments)
        {
            if (reassignment is null || reassignment.From is null)
            {
                throw new ConfigurationException("The documentation examples.reassignments property cannot contain null values.");
            }
            normalized.Add(reassignment);
        }

        return normalized.ToImmutable();
    }

    private static void Validate(Documentation data)
    {
        ValidateFileName(data.Output.File, "output.file");

        if (data.Template.File is not null)
        {
            ValidateNonempty(data.Template.File, "template.file");
        }

        if (data.Template.IncludeRoot is not null)
        {
            ValidateNonempty(data.Template.IncludeRoot, "template.includeRoot");
        }

        foreach (var key in data.Template.Values.Keys)
        {
            ValidateNonempty(key, "template.values key");
        }

        foreach (var source in data.Examples.Sources)
        {
            if (source.Path != ".")
            {
                ValidateRelativePath(source.Path, "examples.sources[].path", allowNested: true);
            }

            ValidatePatterns(source.Include, source.Exclude, "examples.sources[]");
        }

        foreach (var reassignment in data.Examples.Reassignments)
        {
            ValidatePatterns(reassignment.From.Include, reassignment.From.Exclude, "examples.reassignments[].from");
            if (reassignment.From.Include.IsDefaultOrEmpty)
            {
                throw new ConfigurationException("The documentation examples.reassignments[].from.include property must contain at least one pattern.");
            }

            ValidateRelativePath(reassignment.To, "examples.reassignments[].to", allowNested: false);
        }
    }

    private static void ValidatePatterns(
        ImmutableArray<string> includes,
        ImmutableArray<string> excludes,
        string path)
    {
        var matcher = new Matcher(StringComparison.OrdinalIgnoreCase);
        foreach (var pattern in includes)
        {
            ValidateNonempty(pattern, $"{path}.include[]");
            matcher.AddInclude(pattern);
        }

        foreach (var pattern in excludes)
        {
            ValidateNonempty(pattern, $"{path}.exclude[]");
            matcher.AddExclude(pattern);
        }
    }

    private static void ValidateRelativePath(string value, string path, bool allowNested)
    {
        ValidateNonempty(value, path);
        if (value.StartsWith('/') ||
            value.StartsWith('\\') ||
            (value.Length > 1 && value[1] == ':') ||
            FilePathFacts.IsWindowsDosDevicePath(value))
        {
            throw new ConfigurationException($"The documentation {path} property must be a relative path.");
        }

        var segments = value.Split(['/', '\\'], StringSplitOptions.RemoveEmptyEntries);
        if (segments.Any(segment => segment is "." or "..") ||
            (!allowNested && segments.Length != 1))
        {
            throw new ConfigurationException($"The documentation {path} property cannot traverse directories.");
        }
    }

    private static void ValidateFileName(string value, string path)
    {
        ValidateRelativePath(value, path, allowNested: false);
        if (value.Any(FilePathFacts.IsForbiddenPathCharacter) ||
            FilePathFacts.IsForbiddenPathTerminatorCharacter(value[^1]) ||
            FilePathFacts.ContainsWindowsReservedFileName(value))
        {
            throw new ConfigurationException($"The documentation {path} property must be a portable file name.");
        }
    }

    private static void ValidateNonempty(string value, string path)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ConfigurationException($"The documentation {path} property cannot be empty.");
        }
    }
}
