// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.Cli.Logging;
using Bicep.Core.Extensions;
using Bicep.Core.FileSystem;
using Bicep.IO.Abstraction;
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

    public static void ValidateBicepFile(IOUri fileUri)
    {
        if (!fileUri.HasBicepExtension())
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepFileExtensionMessage, fileUri.ToString()));
        }
    }

    public static void ValidateBicepParamFile(IOUri fileUri)
    {
        if (!fileUri.HasBicepParamExtension())
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepparamsFileExtensionMessage, fileUri.ToString()));
        }
    }

    public static void ValidateBicepOrBicepParamFile(IOUri fileUri)
    {
        if (!fileUri.HasBicepExtension() && !fileUri.HasBicepParamExtension())
        {
            throw new CommandLineException(string.Format(CliResources.UnrecognizedBicepOrBicepparamsFileExtensionMessage, fileUri.ToString()));
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

    public static void ValidateNotAlreadySet<T>(string argName, T? value)
    {
        if (value is not null)
        {
            throw new CommandLineException($"The {argName} parameter cannot be specified twice");
        }
    }
}
