// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record TestArguments(
    string InputFile,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IInputArguments;
