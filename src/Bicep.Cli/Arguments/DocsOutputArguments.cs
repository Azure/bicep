// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;

namespace Bicep.Cli.Arguments;

public record DocsOutputArguments(
    string? InputFile,
    string? TemplateFile,
    string? TemplateRoot,
    ImmutableSortedDictionary<string, string> CustomValues,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IInputArguments;
