// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using LanguageConstants = Bicep.Core.LanguageConstants;

namespace Bicep.Cli.Arguments;

public record DecompileParamsArguments(
    string InputFile,
    bool OutputToStdOut,
    bool AllowOverwrite,
    string? OutputDir,
    string? OutputFile,
    string? BicepFilePath) : IInputOutputArguments<DecompileParamsArguments>
{
    public static Func<DecompileParamsArguments, IOUri, string> OutputFileExtensionResolver { get; } = (_, _) => LanguageConstants.ParamsFileExtension;
}
