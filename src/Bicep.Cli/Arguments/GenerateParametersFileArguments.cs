// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core;
using Bicep.Core.Emit.Options;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Arguments;

public record GenerateParametersFileArguments(
    string InputFile,
    bool OutputToStdOut,
    bool NoRestore,
    string? OutputDir,
    string? OutputFile,
    OutputFormatOption OutputFormat,
    IncludeParamsOption IncludeParams) : IInputOutputArguments<GenerateParametersFileArguments>
{
    public static Func<GenerateParametersFileArguments, IOUri, string> OutputFileExtensionResolver { get; } = (args, _) => args.OutputFormat switch
    {
        OutputFormatOption.Json => $".parameters{LanguageConstants.JsonFileExtension}",
        OutputFormatOption.BicepParam => LanguageConstants.ParamsFileExtension,
        _ => throw new ArgumentOutOfRangeException(nameof(args.OutputFormat), $"Unsupported output format: {args.OutputFormat}")
    };
}
