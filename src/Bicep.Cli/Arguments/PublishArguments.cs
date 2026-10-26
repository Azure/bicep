// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record PublishArguments(
    string InputFile,
    string TargetModuleReference,
    string? DocumentationUri,
    bool NoRestore,
    bool Force,
    bool WithSource) : IInputArguments;
