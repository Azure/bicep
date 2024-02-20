// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Core.FileSystem;

namespace Bicep.Cli.Helpers;

public class ArgumentHelper
{
    public static DiagnosticsFormat ToDiagnosticsFormat(string? format)
    {
        if (format is null || (format is not null && format.Equals("default", StringComparison.OrdinalIgnoreCase)))
        {
            return Arguments.DiagnosticsFormat.Default;
        }
        else if (format is not null && format.Equals("sarif", StringComparison.OrdinalIgnoreCase))
        {
            return Arguments.DiagnosticsFormat.Sarif;
        }

        throw new ArgumentException($"Unrecognized diagnostics format {format}");
    }

    [return: NotNullIfNotNull(nameof(filePath))]
    public static Uri? GetFileUri(string? filePath, IFileSystem? fileSystem = null)
        => filePath is { } ? PathHelper.FilePathToFileUrl(PathHelper.ResolvePath(filePath, fileSystem: fileSystem)) : null;

    public static void ValidateBicepFile(Uri fileUri)
    {
        if (!PathHelper.HasBicepExtension(fileUri))
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepFileExtensionMessage, fileUri.LocalPath));
        }
    }

    public static void ValidateBicepParamFile(Uri fileUri)
    {
        if (!PathHelper.HasBicepparamsExtension(fileUri))
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepparamsFileExtensionMessage, fileUri.LocalPath));
        }
    }

    public static void ValidateBicepOrBicepParamFile(Uri fileUri)
    {
        if (!PathHelper.HasBicepExtension(fileUri) &&
            !PathHelper.HasBicepparamsExtension(fileUri))
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepOrBicepparamsFileExtensionMessage, fileUri.LocalPath));
        }
    }
}
