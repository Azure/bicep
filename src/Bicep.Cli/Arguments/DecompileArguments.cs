// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.IO.Abstraction;
using LanguageConstants = Bicep.Core.LanguageConstants;

namespace Bicep.Cli.Arguments;

public record DecompileArguments(
    string InputFile,
    bool OutputToStdOut,
    bool AllowOverwrite,
    string? OutputDir,
    string? OutputFile) : IInputOutputArguments<DecompileArguments>
{
    public static Func<DecompileArguments, IOUri, string> OutputFileExtensionResolver { get; } = (_, _) => LanguageConstants.LanguageFileExtension;
}
