// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record BuildParamsArguments(
    string? InputFile,
    bool OutputToStdOut,
    bool NoRestore,
    string? OutputDir,
    string? OutputFile,
    string? FilePattern,
    string? BicepFile,
    DiagnosticsFormat? DiagnosticsFormat) : IFilePatternInputOutputArguments<BuildParamsArguments>
{
    public static Func<BuildParamsArguments, IOUri, string> OutputFileExtensionResolver => (_, _) => LanguageConstants.JsonFileExtension;
}
