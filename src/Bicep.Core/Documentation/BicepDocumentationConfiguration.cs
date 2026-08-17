// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Core.Documentation;

/// <summary>
/// Configures module documentation generation.
/// </summary>
public sealed record BicepDocumentationConfiguration
{
    /// <summary>
    /// Gets the entrypoint used when the input path is a directory.
    /// </summary>
    public string EntryPoint { get; init; } = "main.bicep";

    /// <summary>
    /// Gets output settings.
    /// </summary>
    public BicepDocumentationOutputConfiguration Output { get; init; } = new();

    /// <summary>
    /// Gets template settings.
    /// </summary>
    public BicepDocumentationTemplateConfiguration Template { get; init; } = new();

    /// <summary>
    /// Gets usage-example settings.
    /// </summary>
    public BicepDocumentationExamplesConfiguration Examples { get; init; } = new();
}

/// <summary>
/// Configures generated documentation output.
/// </summary>
public sealed record BicepDocumentationOutputConfiguration
{
    /// <summary>
    /// Gets the generated file name.
    /// </summary>
    public string File { get; init; } = "README.md";
}

/// <summary>
/// Configures documentation templates.
/// </summary>
public sealed record BicepDocumentationTemplateConfiguration
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
public sealed record BicepDocumentationExamplesConfiguration
{
    /// <summary>
    /// Gets discovery sources relative to each module root.
    /// </summary>
    public ImmutableArray<BicepDocumentationExampleSource> Sources { get; init; } =
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
    public ImmutableArray<BicepDocumentationExampleReassignment> Reassignments { get; init; } = [];
}

/// <summary>
/// Defines one usage-example discovery source.
/// </summary>
public sealed record BicepDocumentationExampleSource
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
public sealed record BicepDocumentationExampleReassignment
{
    /// <summary>
    /// Gets patterns selecting examples from a parent module.
    /// </summary>
    public BicepDocumentationPatternSet From { get; init; } = new();

    /// <summary>
    /// Gets the destination directory relative to the parent module root.
    /// </summary>
    public required string To { get; init; }
}

/// <summary>
/// Includes and excludes paths using glob patterns.
/// </summary>
public sealed record BicepDocumentationPatternSet
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
