// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

namespace Bicep.Cli.Arguments;

public record JsonRpcArguments
{
    public string? Pipe { get; init; }

    public int? Socket { get; init; }

    public bool? Stdio { get; init; }
}