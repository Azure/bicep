// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record BuildArguments(
    string? InputFile,
    bool OutputToStdOut,
    bool NoRestore,
    string? OutputDir,
    string? OutputFile,
    string? FilePattern,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputOutputArguments<BuildArguments>
{
    public static Func<BuildArguments, IOUri, string> OutputFileExtensionResolver => (_, _) => LanguageConstants.JsonFileExtension;
}
