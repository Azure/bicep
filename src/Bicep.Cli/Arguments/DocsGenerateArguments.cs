// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record DocsGenerateArguments(
    string? InputFile,
    string? FilePattern,
    bool OutputToStdOut,
    string? OutputDir,
    string? OutputFile,
    bool NoRestore,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputOutputArguments<DocsGenerateArguments>
{
    public static Func<DocsGenerateArguments, IOUri, string> OutputFileExtensionResolver => (_, _) => ".md";
}
