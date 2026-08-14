// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Documentation;

namespace Bicep.Cli.Arguments;

public record DocsGenerateArguments(
    string? InputFile,
    string? FilePattern,
    BicepDocumentationPreset Preset,
    string? TemplateFile,
    string? TemplateRoot,
    ImmutableArray<string> CustomValues,
    string OutputFile,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputArguments;
