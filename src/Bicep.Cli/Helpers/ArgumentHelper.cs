// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.FileSystem;
using Json.Patch;

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

    public static TEnum GetEnumValueWithValidation<TEnum>(string argName, string[] args, int argPosition)
        where TEnum : struct, Enum
    {
        var value = GetValueWithValidation(argName, args, argPosition);

        if (Enum.TryParse<TEnum>(value, ignoreCase: true, out var result))
        {
            return result;
        }

        throw new CommandLineException($"Unrecognized value {value} for parameter {argName}");
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
        if (!File.Exists(fileUri.LocalPath))
        {
            throw new CommandLineException(string.Format(CliResources.FileDoesNotExistFormat, fileUri.LocalPath));
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

    public static DiagnosticOptions GetDiagnosticOptions(DiagnosticsFormat? diagnosticsFormat)
        => new(
            Format: diagnosticsFormat ?? DiagnosticsFormat.Default,
            SarifToStdout: false);

    public static string GetValueWithValidation(string argName, string[] args, int argPosition)
    {
        if (args.Length == argPosition + 1)
        {
            throw new CommandLineException($"The {argName} parameter expects an argument");
        }

        return args[argPosition + 1];
    }

    public static string GetDirectoryPathValueWithValidation(string argName, string[] args, int argPosition)
    {
        var value = GetValueWithValidation(argName, args, argPosition);

        var resolvedPath = PathHelper.ResolvePath(value);
        if (!Directory.Exists(resolvedPath))
        {
            throw new CommandLineException($"The {argName} directory does not exist: {resolvedPath}");
        }

        return value;
    }

    public static void ValidateNotAlreadySet<T>(string argName, T? value)
    {
        if (value is not null)
        {
            throw new CommandLineException($"The {argName} parameter cannot be specified twice");
        }
    }
}
