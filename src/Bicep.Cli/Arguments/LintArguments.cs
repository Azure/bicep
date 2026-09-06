// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record LintArguments(
    string? InputFile,
    string? FilePattern,
    DiagnosticsFormat? DiagnosticsFormat,
    bool NoRestore) : IFilePatternInputArguments;
