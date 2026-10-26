// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record PublishExtensionArguments(
    string? IndexFile,
    string? TargetExtensionReference,
    IReadOnlyDictionary<string, string> Binaries,
    bool Force);
