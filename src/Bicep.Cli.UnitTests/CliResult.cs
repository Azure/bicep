// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.UnitTests;

public record CliResult(
    string Stdout,
    string Stderr,
    int ExitCode);
