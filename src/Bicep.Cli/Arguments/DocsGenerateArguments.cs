// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Cli.Arguments;

public record DocsGenerateArguments(
    string? InputFile,
    string? FilePattern,
    string? TemplateFile,
    string? TemplateRoot,
    ImmutableSortedDictionary<string, string> CustomValues,
    string OutputFile,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputArguments;
