// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Text.Json;
using Bicep.Cli.Arguments;
using Bicep.Core.Diagnostics;
using Bicep.Core.Documentation;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.Semantics;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public static class DocsCommand
{
    internal const string InputFailureCode = "DOCS001";
    internal const string WriteFailureCode = "DOCS002";
    internal const string RenderFailureCode = "DOCS003";

    internal static System.CommandLine.Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new System.CommandLine.Command(
            Constants.Command.Docs,
            "[Experimental] Generates documentation for Bicep modules.");
        command.Add(DocsGenerateCommand.CreateCommand(context));
        command.Add(DocsOutputCommand.CreateCommand(context));

        return command;
    }

    internal static ImmutableSortedDictionary<string, string> ParseCustomValues(
        System.CommandLine.ParseResult result,
        System.CommandLine.Option<string[]> customTemplateValueOption,
        System.CommandLine.Option<string[]> customTemplateValueFilePathOption,
        IFileSystem fileSystem)
    {
        var customValues = ImmutableSortedDictionary.CreateBuilder<string, string>(StringComparer.Ordinal);
        var tokens = result.Tokens;
        for (var index = 0; index < tokens.Count; index++)
        {
            var token = tokens[index];
            if (token.Type != TokenType.Option)
            {
                continue;
            }

            if (token.Value.Equals(customTemplateValueOption.Name, StringComparison.Ordinal))
            {
                SetValue(customValues, GetOptionValue(tokens, ref index, Option.CustomTemplateValue));
            }
            else if (token.Value.Equals(customTemplateValueFilePathOption.Name, StringComparison.Ordinal))
            {
                LoadValuesFile(
                    customValues,
                    GetOptionValue(tokens, ref index, Option.CustomTemplateValueFilePath),
                    fileSystem);
            }
        }

        return customValues.ToImmutable();
    }

    private static string GetOptionValue(
        IReadOnlyList<Token> tokens,
        ref int optionIndex,
        string optionName)
    {
        if (optionIndex + 1 >= tokens.Count || tokens[optionIndex + 1].Type != TokenType.Argument)
        {
            throw new CommandLineException($"The {optionName} parameter expects an argument.");
        }

        return tokens[++optionIndex].Value;
    }

    private static void SetValue(
        ImmutableSortedDictionary<string, string>.Builder customValues,
        string value)
    {
        var separatorIndex = value.IndexOf('=');
        if (separatorIndex <= 0)
        {
            throw new CommandLineException(
                $"The {Option.CustomTemplateValue} value \"{value}\" must use the format key=value.");
        }

        customValues[value[..separatorIndex]] = value[(separatorIndex + 1)..];
    }

    private static void LoadValuesFile(
        ImmutableSortedDictionary<string, string>.Builder customValues,
        string path,
        IFileSystem fileSystem)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new CommandLineException(
                $"The {Option.CustomTemplateValueFilePath} parameter expects a nonempty path.");
        }

        string fullPath;
        try
        {
            fullPath = fileSystem.Path.GetFullPath(path);
        }
        catch (Exception exception) when (exception is ArgumentException or NotSupportedException)
        {
            throw new CommandLineException(
                $"The custom template value file path \"{path}\" is invalid: {exception.Message}",
                exception);
        }

        if (!fileSystem.File.Exists(fullPath))
        {
            throw new CommandLineException($"The custom template value file \"{fullPath}\" does not exist.");
        }

        try
        {
            using var document = JsonDocument.Parse(fileSystem.File.ReadAllText(fullPath));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new CommandLineException(
                    $"The custom template value file \"{fullPath}\" must contain a JSON object.");
            }

            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (var property in document.RootElement.EnumerateObject())
            {
                if (property.Name.Length == 0)
                {
                    throw new CommandLineException(
                        $"The custom template value file \"{fullPath}\" contains an empty key.");
                }

                if (!keys.Add(property.Name))
                {
                    throw new CommandLineException(
                        $"The custom template value file \"{fullPath}\" contains the duplicate key \"{property.Name}\".");
                }

                if (property.Value.ValueKind != JsonValueKind.String)
                {
                    throw new CommandLineException(
                        $"The custom template value file \"{fullPath}\" value for \"{property.Name}\" must be a string.");
                }

                customValues[property.Name] = property.Value.ToString();
            }
        }
        catch (JsonException exception)
        {
            throw new CommandLineException(
                $"The custom template value file \"{fullPath}\" is not valid JSON: {exception.Message}",
                exception);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            throw new CommandLineException(
                $"Unable to read custom template value file \"{fullPath}\": {exception.Message}",
                exception);
        }
    }

    internal static IDiagnostic CreateDiagnostic(string code, string message) =>
        new Diagnostic(new(0, 0), DiagnosticLevel.Error, DiagnosticSource.Compiler, code, message);

    internal static DocsDiagnostics MergeDiagnostics(
        IEnumerable<(IOUri SourceUri, Compilation? Compilation, IDiagnostic? DocumentationDiagnostic)> results)
    {
        var byUri = new Dictionary<IOUri, (BicepSourceFile File, ImmutableArray<IDiagnostic>.Builder Diagnostics)>();
        var additionalDiagnostics = ImmutableArray.CreateBuilder<(IOUri SourceUri, IDiagnostic Diagnostic)>();
        foreach (var (sourceUri, compilation, documentationDiagnostic) in results)
        {
            if (compilation is not null)
            {
                foreach (var (file, diagnostics) in compilation.GetAllDiagnosticsByBicepFile())
                {
                    if (!byUri.ContainsKey(file.FileHandle.Uri))
                    {
                        byUri[file.FileHandle.Uri] = (file, diagnostics.ToBuilder());
                    }
                }
            }

            if (documentationDiagnostic is not null)
            {
                if (compilation is null)
                {
                    additionalDiagnostics.Add((sourceUri, documentationDiagnostic));
                }
                else
                {
                    var entryFile = compilation.GetEntrypointSemanticModel().SourceFile;
                    byUri[entryFile.FileHandle.Uri].Diagnostics.Add(documentationDiagnostic);
                }
            }
        }

        return new(
            byUri.Values.ToImmutableDictionary(item => item.File, item => item.Diagnostics.ToImmutable()),
            additionalDiagnostics.ToImmutable());
    }

    internal record DocsDiagnostics(
        ImmutableDictionary<BicepSourceFile, ImmutableArray<IDiagnostic>> ByFile,
        ImmutableArray<(IOUri SourceUri, IDiagnostic Diagnostic)> Additional);

    internal static void ValidateOutputPaths(IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> paths)
    {
        var outputUris = new HashSet<IOUri>();
        foreach (var (inputUri, outputUri) in paths)
        {
            if (inputUri.Equals(outputUri))
            {
                throw new CommandLineException("The documentation output path cannot overwrite the input Bicep file.");
            }

            if (outputUri.HasBicepExtension() || outputUri.HasBicepParamExtension())
            {
                throw new CommandLineException("Documentation output cannot use a Bicep source file extension.");
            }

            if (!outputUris.Add(outputUri))
            {
                throw new CommandLineException($"Multiple input files resolve to the output file \"{outputUri}\".");
            }
        }
    }

}
