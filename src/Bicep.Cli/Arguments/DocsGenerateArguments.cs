// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record DocsGenerateArguments(
    string? InputFile,
    string? FilePattern,
    string? ConfigFilePath,
    string? TemplateFile,
    string? TemplateRoot,
    ImmutableSortedDictionary<string, string> CustomValues,
    string? OutputDir,
    string? OutputFile,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputOutputArguments<DocsGenerateArguments>
{
    public static Func<DocsGenerateArguments, IOUri, string> OutputFileExtensionResolver => (_, _) => ".md";
}
