// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.PrettyPrintV2;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record FormatArguments(
    bool OutputToStdOut,
    string? InputFile,
    string? OutputDir,
    string? OutputFile,
    string? FilePattern,
    NewlineKind? NewlineKind,
    IndentKind? IndentKind,
    int? IndentSize,
    bool? InsertFinalNewline) : IFilePatternInputOutputArguments<FormatArguments>
{
    public static Func<FormatArguments, IOUri, string> OutputFileExtensionResolver { get; } =
        (_, inputUri) => inputUri.GetExtension().ToString();
}
