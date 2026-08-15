// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Bicep.Cli.Arguments;
using Bicep.IO.Abstraction;

namespace Bicep.Cli.Services;

public class DocsModuleScanner(IFileSystem fileSystem, InputOutputArgumentsResolver argumentsResolver)
{
    public IReadOnlyList<IOUri> ResolveModules(DocsGenerateArguments arguments)
    {
        if (arguments.InputFile is not null && arguments.FilePattern is not null)
        {
            throw new CommandLineException("The input path and --pattern parameter cannot both be specified.");
        }

        if (arguments.FilePattern is not null)
        {
            return ExecutePathOperation(() =>
                argumentsResolver.ResolveFilePatternInputArguments(arguments)
                    .OrderBy(uri => uri.ToString(), StringComparer.OrdinalIgnoreCase)
                    .ThenBy(uri => uri.ToString(), StringComparer.Ordinal)
                    .ToArray());
        }

        return [ResolveModule(arguments.InputFile)];
    }

    public IOUri ResolveModule(string? path)
    {
        return ExecutePathOperation(() =>
        {
            path ??= fileSystem.Directory.GetCurrentDirectory();
            var fullPath = fileSystem.Path.GetFullPath(path);

            if (fileSystem.Directory.Exists(fullPath))
            {
                fullPath = fileSystem.Path.Combine(fullPath, "main.bicep");
            }
            else if (!fileSystem.File.Exists(fullPath) &&
                !string.Equals(fileSystem.Path.GetExtension(fullPath), ".bicep", StringComparison.OrdinalIgnoreCase))
            {
                throw new CommandLineException($"The path \"{fullPath}\" does not exist.");
            }

            return argumentsResolver.PathToUri(fullPath);
        });
    }

    public IOUri? ResolveOptionalFile(string? path) =>
        path is null ? null : ExecutePathOperation(() => argumentsResolver.PathToUri(path));

    public IOUri? ResolveOptionalDirectory(string? path)
    {
        if (path is null)
        {
            return null;
        }

        return ExecutePathOperation(() =>
        {
            var fullPath = fileSystem.Path.GetFullPath(path);
            if (!fileSystem.Directory.Exists(fullPath))
            {
                throw new CommandLineException($"The template root directory \"{fullPath}\" does not exist.");
            }

            var normalizedPath = fileSystem.Path.EndsInDirectorySeparator(fullPath)
                ? fullPath
                : fullPath + fileSystem.Path.DirectorySeparatorChar;

            return argumentsResolver.PathToUri(normalizedPath);
        });
    }

    public IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> ResolveOutputFiles(
        IReadOnlyList<IOUri> modules,
        string outputFile)
    {
        ValidateOutputFileName(outputFile);

        var results = modules
            .Select(input => (InputUri: input, OutputUri: input.Resolve(outputFile)))
            .ToArray();

        if (results.Select(result => result.OutputUri).Distinct().Count() != results.Length)
        {
            throw new CommandLineException("Multiple input modules resolve to the same output file.");
        }

        return results;
    }

    public void ValidateOutputFileName(string outputFile)
    {
        var extension = fileSystem.Path.GetExtension(outputFile);
        if (string.IsNullOrWhiteSpace(outputFile) ||
            outputFile is "." or ".." ||
            outputFile.Contains('/') ||
            outputFile.Contains('\\') ||
            outputFile.IndexOfAny(fileSystem.Path.GetInvalidFileNameChars()) >= 0 ||
            outputFile.EndsWith(' ') ||
            outputFile.EndsWith('.') ||
            IsReservedWindowsFileName(outputFile))
        {
            throw new CommandLineException("The --output-file parameter must be a file name without a directory path.");
        }

        if (extension.Equals(".bicep", StringComparison.OrdinalIgnoreCase) ||
            extension.Equals(".bicepparam", StringComparison.OrdinalIgnoreCase))
        {
            throw new CommandLineException("The --output-file parameter cannot use a Bicep source file extension.");
        }
    }

    private bool IsReservedWindowsFileName(string outputFile)
    {
        var name = fileSystem.Path.GetFileNameWithoutExtension(outputFile);
        return name.Equals("CON", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("CONIN$", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("CONOUT$", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("PRN", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("AUX", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("NUL", StringComparison.OrdinalIgnoreCase) ||
            Enumerable.Range(1, 9).Any(number =>
                name.Equals($"COM{number}", StringComparison.OrdinalIgnoreCase) ||
                name.Equals($"LPT{number}", StringComparison.OrdinalIgnoreCase));
    }

    private static T ExecutePathOperation<T>(Func<T> operation)
    {
        try
        {
            return operation();
        }
        catch (IOException exception)
        {
            throw new CommandLineException(exception.Message, exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            throw new CommandLineException(exception.Message, exception);
        }
        catch (ArgumentException exception)
        {
            throw new CommandLineException(exception.Message, exception);
        }
        catch (NotSupportedException exception)
        {
            throw new CommandLineException(exception.Message, exception);
        }
    }
}
