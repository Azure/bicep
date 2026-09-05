// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Immutable;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO.Abstractions;
using System.Text.Json;
using Bicep.Cli.Arguments;
using Bicep.Cli.Helpers;
using Bicep.Cli.Logging;
using Bicep.Cli.Services;
using Bicep.Core.Exceptions;
using Bicep.Core.Extensions;
using Bicep.Core.SourceGraph;
using Bicep.IO.Abstraction;
using Option = Bicep.Cli.Constants.Option;

namespace Bicep.Cli.Commands;

public class DocsGenerateCommand(
    IOContext io,
    InputOutputArgumentsResolver argumentsResolver,
    DocsCommandRunner runner,
    OutputWriter writer,
    IFileSystem fileSystem) : ICommand
{
    public async Task<int> RunAsync(DocsGenerateArguments arguments, CancellationToken cancellationToken = default)
    {
        var (inputRoot, inputUris) = ResolveInputs(arguments);
        var workspace = new ActiveSourceFileSet();
        var successes = new Dictionary<IOUri, DocsRenderResult.Succeeded>();
        var experimentalWarningLogged = false;
        var hasErrors = false;

        foreach (var module in inputUris)
        {
            ArgumentHelper.ValidateBicepFile(module);
            var result = await runner.RenderAsync(
                module,
                arguments.CustomValues,
                arguments.NoRestore,
                arguments.DiagnosticsFormat,
                workspace,
                logExperimentalWarning: !experimentalWarningLogged,
                cancellationToken: cancellationToken);

            if (result is not DocsRenderResult.Succeeded success)
            {
                hasErrors = true;
                continue;
            }

            experimentalWarningLogged = true;
            successes.Add(module, success);
        }

        if (arguments.OutputToStdOut)
        {
            if (successes.Values.SingleOrDefault() is { } success)
            {
                await io.Output.Writer.WriteAsync(success.Contents.AsMemory(), cancellationToken);
            }
        }
        else if (successes.Count > 0)
        {
            var inputOutputPairs = argumentsResolver.ResolveFileSetInputOutputArguments(
                arguments,
                inputRoot,
                successes.Keys.ToArray(),
                (_, inputUri) => successes[inputUri].Configuration.Documentation.Data.Output.File);
            ValidateOutputPaths(inputOutputPairs);

            foreach (var (inputUri, outputUri) in inputOutputPairs)
            {
                var success = successes[inputUri];
                await writer.WriteToFileAsync(outputUri, success.Contents);
            }
        }

        return hasErrors ? 1 : 0;
    }

    private (IOUri RootUri, IReadOnlyList<IOUri> InputUris) ResolveInputs(
        DocsGenerateArguments arguments)
    {
        if (arguments.InputFile is not null)
        {
            var inputUri = argumentsResolver.ResolveInputArguments(arguments);
            return (inputUri.Resolve("."), [inputUri]);
        }

        if (arguments.FilePattern is not null)
        {
            var (rootUri, relativePaths) = argumentsResolver.ResolveFilePattern(arguments.FilePattern);
            return (rootUri, relativePaths.Select(rootUri.Resolve).ToArray());
        }

        throw new CommandLineException("Either the input file path or the --pattern parameter must be specified");
    }

    private ImmutableSortedDictionary<string, string> ParseCustomValues(
        ParseResult result,
        Option<string[]> customTemplateValueOption,
        Option<string[]> customTemplateValueFilePathOption)
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
                    GetOptionValue(tokens, ref index, Option.CustomTemplateValueFilePath));
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

    private void LoadValuesFile(
        ImmutableSortedDictionary<string, string>.Builder customValues,
        string path)
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
        catch (Exception exception) when (exception.IsFileSystemException())
        {
            throw new CommandLineException(
                $"Unable to read custom template value file \"{fullPath}\": {exception.Message}",
                exception);
        }
    }

    internal static Command CreateCommand(CommandLineBuilderContext context)
    {
        var command = new Command(
            Constants.Command.Docs,
            "[Experimental] Generates documentation for Bicep modules.");
        command.Add(CreateGenerateCommand(context));

        return command;
    }

    private static Command CreateGenerateCommand(CommandLineBuilderContext context)
    {
        var command = new Command(Constants.Command.DocsGenerate, "[Experimental] Generates documentation files for Bicep modules.")
        {
            TreatUnmatchedTokensAsErrors = true,
        };
        var inputFileArgument = new Argument<string?>(Constants.Argument.InputFile)
        {
            Description = "The path to an input .bicep file.",
            Arity = ArgumentArity.ZeroOrOne,
        };
        var stdoutOption = new Option<bool>(Option.Stdout)
        {
            Description = "Prints the generated documentation to stdout.",
        };
        var customTemplateValueOption = new Option<string[]>(Option.CustomTemplateValue)
        {
            Description = "Supplies a custom template value in key=value form. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = false,
        };
        var customTemplateValueFilePathOption = new Option<string[]>(Option.CustomTemplateValueFilePath)
        {
            Description = "Loads custom template string values from a JSON object file. May be repeated.",
            Arity = ArgumentArity.ZeroOrMore,
            AllowMultipleArgumentsPerToken = false,
        };
        var outDirOption = new Option<string?>(Option.OutDir)
        {
            Description = "Saves the generated README.md files beneath the specified directory.",
        };
        var outFileOption = new Option<string?>(Option.OutFile)
        {
            Description = "Saves the generated documentation as the specified file path.",
        };
        var patternOption = new Option<string?>(Option.Pattern)
        {
            Description = "Generates documentation for all files matching the glob pattern. Cannot be used with the input path.",
        };
        var noRestoreOption = new Option<bool>(Option.NoRestore)
        {
            Description = "Skips restoring external modules.",
        };
        var diagnosticsFormatOption = new Option<DiagnosticsFormat?>(Option.DiagnosticsFormat)
        {
            Description = "Sets the diagnostics format. Valid values are (Default, SARIF).",
        };

        command.Add(inputFileArgument);
        command.Add(stdoutOption);
        command.Add(customTemplateValueOption);
        command.Add(customTemplateValueFilePathOption);
        command.Add(outDirOption);
        command.Add(outFileOption);
        command.Add(patternOption);
        command.Add(noRestoreOption);
        command.Add(diagnosticsFormatOption);
        command.Validators.Add(result =>
        {
            CommandLineBuilderContext.ValidatePositionalArgument(result, inputFileArgument);
            if (result.GetValue(inputFileArgument) is not null && result.GetValue(patternOption) is not null)
            {
                result.AddError("The input path and --pattern parameter cannot both be specified.");
            }
        });

        command.SetAction((result, ct) => context.RunCommandAsync(async () =>
        {
            var handler = context.GetCommand<DocsGenerateCommand>();
            var customValues = handler.ParseCustomValues(
                result,
                customTemplateValueOption,
                customTemplateValueFilePathOption);
            var outputDir = result.GetValue(outDirOption);
            var outputFile = result.GetValue(outFileOption);
            var filePattern = result.GetValue(patternOption);
            var outputToStdOut = result.GetValue(stdoutOption);
            ArgumentHelper.ValidateOutputOptions(outputToStdOut, outputDir, outputFile, filePattern);
            var arguments = new DocsGenerateArguments(
                result.GetValue(inputFileArgument),
                filePattern,
                customValues,
                outputToStdOut,
                outputDir,
                outputFile,
                result.GetValue(noRestoreOption),
                result.GetValue(diagnosticsFormatOption));

            return await handler.RunAsync(arguments, ct);
        }));

        return command;
    }

    private static void ValidateOutputPaths(IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> paths)
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
