// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using Bicep.Core.Documentation;

namespace Bicep.Cli.Arguments;

public record DocsOutputArguments(
    string? InputFile,
    BicepDocumentationPreset Preset,
    string? TemplateFile,
    string? TemplateRoot,
    ImmutableArray<string> CustomValues,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IInputArguments;
