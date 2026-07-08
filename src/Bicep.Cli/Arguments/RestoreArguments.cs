// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record RestoreArguments(
    string? InputFile,
    string? FilePattern,
    bool ForceModulesRestore) : IFilePatternInputArguments;
