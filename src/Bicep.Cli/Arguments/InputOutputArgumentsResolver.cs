// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions;
using Azure.Deployments.Core.Extensions;
using Bicep.IO.Abstraction;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Bicep.Cli.Arguments
{
    public class InputOutputArgumentsResolver
    {
        private readonly IFileSystem fileSystem;

        public InputOutputArgumentsResolver(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        public IOUri PathToUri(string path)
        {
            try
            {
                return IOUri.FromFilePath(GetFullPath(path));
            }
            catch (Exception exception) when (exception.IsPathException())
            {
                throw new CommandLineException(exception.Message, exception);
            }
        }

        public string GetFullPath(string path)
        {
            if (!OperatingSystem.IsWindows() && path.Contains('\\'))
            {
                throw new CommandLineException(string.Format(CliResources.FilePathContainsBackslash, path));
            }

            try
            {
                return this.fileSystem.Path.GetFullPath(path);
            }
            catch (Exception exception) when (exception.IsPathException())
            {
                throw new CommandLineException(exception.Message, exception);
            }
        }

        public IOUri ResolveInputArguments(IInputArguments arguments)
        {
            ArgumentNullException.ThrowIfNull(arguments.InputFile);

            return this.PathToUri(arguments.InputFile);
        }

        public (IOUri inputUri, IOUri outputUri) ResolveInputOutputArguments<T>(T arguments)
            where T : IInputOutputArguments<T>
        {
            ArgumentNullException.ThrowIfNull(arguments.InputFile);

            var inputUri = this.PathToUri(arguments.InputFile);
            var outputUri = this.ResolveOutputUri(inputUri, arguments.OutputDir, arguments.OutputFile, T.OutputFileExtensionResolver.Invoke(arguments, inputUri));

            return (inputUri, outputUri);
        }

        public IReadOnlyList<IOUri> ResolveFilePatternInputArguments(IFilePatternInputArguments arguments)
        {
            if (arguments.InputFile is not null)
            {
                return [this.ResolveInputArguments(arguments)];
            }

            if (arguments.FilePattern is not null)
            {
                var result = new List<IOUri>();
                var (rootUri, inputRelativePaths) = this.ResolveFilePattern(arguments.FilePattern);

                foreach (var inputRelativePath in inputRelativePaths)
                {
                    var inputUri = rootUri.Resolve(inputRelativePath);
                    result.Add(inputUri);
                }
                return result;
            }

            throw new CommandLineException("Either the input file path or the --pattern parameter must be specified");
        }

        public IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> ResolveFilePatternInputOutputArguments<T>(
            T arguments,
            Func<T, IOUri, string>? outputFileNameResolver = null)
            where T : IFilePatternInputOutputArguments<T>
        {
            if (arguments.InputFile is not null)
            {
                var inputUri = this.ResolveInputArguments(arguments);
                var outputUri = this.ResolveOutputUri(
                    inputUri,
                    arguments.OutputDir,
                    arguments.OutputFile,
                    T.OutputFileExtensionResolver.Invoke(arguments, inputUri),
                    outputFileNameResolver?.Invoke(arguments, inputUri));
                return [(inputUri, outputUri)];
            }

            if (arguments.FilePattern is not null)
            {
                var (rootUri, inputRelativePaths) = this.ResolveFilePattern(arguments.FilePattern);
                return ResolveFileSetInputOutputArguments(
                    arguments,
                    rootUri,
                    inputRelativePaths.Select(rootUri.Resolve).ToArray(),
                    outputFileNameResolver);
            }

            throw new CommandLineException("Either the input file path or the --pattern parameter must be specified");
        }

        internal IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> ResolveFileSetInputOutputArguments<T>(
            T arguments,
            IOUri rootUri,
            IReadOnlyList<IOUri> inputUris,
            Func<T, IOUri, string>? outputFileNameResolver = null)
            where T : IFilePatternInputOutputArguments<T>
        {
            if (arguments.OutputFile is not null)
            {
                if (inputUris.Count != 1)
                {
                    throw new CommandLineException("The --outfile parameter can only be used when exactly one input file is selected.");
                }

                var inputUri = inputUris[0];
                return
                [
                    (
                        inputUri,
                        ResolveOutputUri(
                            inputUri,
                            arguments.OutputDir,
                            arguments.OutputFile,
                            T.OutputFileExtensionResolver.Invoke(arguments, inputUri),
                            outputFileNameResolver?.Invoke(arguments, inputUri)))
                ];
            }

            var result = new List<(IOUri InputUri, IOUri OutputUri)>();
            foreach (var inputUri in inputUris)
            {
                if (arguments.OutputDir is null)
                {
                    result.Add((
                        inputUri,
                        ResolveOutputUri(
                            inputUri,
                            null,
                            null,
                            T.OutputFileExtensionResolver.Invoke(arguments, inputUri),
                            outputFileNameResolver?.Invoke(arguments, inputUri))));
                    continue;
                }

                var inputRelativePath = inputUri.GetPathRelativeTo(rootUri);
                var outputRelativePath = outputFileNameResolver is null
                    ? this.fileSystem.Path.ChangeExtension(inputRelativePath, T.OutputFileExtensionResolver.Invoke(arguments, inputUri))
                    : this.fileSystem.Path.Combine(
                        inputRelativePath[..^this.fileSystem.Path.GetFileName(inputRelativePath).Length],
                        outputFileNameResolver(arguments, inputUri));
                var outputPath = this.fileSystem.Path.Combine(
                    GetFullPath(arguments.OutputDir),
                    outputRelativePath);
                result.Add((inputUri, PathToUri(outputPath)));
            }

            return result;
        }

        private IOUri ResolveOutputUri(
            IOUri inputUri,
            string? outputDir,
            string? outputFile,
            string outputFileExtension,
            string? outputFileName = null)
        {
            if (outputDir is not null)
            {
                outputDir = this.GetFullPath(outputDir);
                var resolvedOutputFileName = outputFileName ?? inputUri.GetFileNameWithoutExtension().ToString() + outputFileExtension;
                var outputPath = this.fileSystem.Path.Combine(outputDir, resolvedOutputFileName);

                return this.PathToUri(outputPath);
            }

            if (outputFile is not null)
            {
                return this.PathToUri(outputFile);
            }

            return outputFileName is null
                ? inputUri.WithExtension(outputFileExtension)
                : inputUri.Resolve(outputFileName);
        }

        internal (IOUri rootUri, IReadOnlyList<string> relativePaths) ResolveFilePattern(string filePattern)
        {
            var (rootPath, relativePattern) = SplitFilePatternOnWildcard(filePattern);
            var rootUri = IOUri.FromFilePath(rootPath);

            Matcher matcher = new();
            matcher.AddInclude(relativePattern);

            var relativePaths = new List<string>();
            foreach (var filePath in matcher.GetResultsInFullPath(rootPath))
            {
                var fileUri = IOUri.FromFilePath(filePath);
                var relativePath = fileUri.GetPathRelativeTo(rootUri);
                relativePaths.Add(relativePath);
            }

            return (rootUri, relativePaths);
        }

        public (string rootPath, string relativePattern) SplitFilePatternOnWildcard(string filePattern)
        {
            if (!OperatingSystem.IsWindows() && filePattern.Contains('\\'))
            {
                throw new CommandLineException(string.Format(CliResources.FilePatternContainsBackslash, filePattern));
            }

            var wildcardIndex = filePattern.IndexOf('*');
            if (wildcardIndex == -1)
            {
                wildcardIndex = filePattern.Length;
            }

            var directorySeparatorChar = this.fileSystem.Path.DirectorySeparatorChar;
            var altDirectorySeparatorChar = this.fileSystem.Path.AltDirectorySeparatorChar;
            var prevDirIndex = filePattern[..wildcardIndex].LastIndexOfAny([directorySeparatorChar, altDirectorySeparatorChar]);
            var rootPath = prevDirIndex != -1 ? filePattern[..prevDirIndex] : "";
            var relativePattern = prevDirIndex != -1 ? filePattern[(prevDirIndex + 1)..] : filePattern;

            if (rootPath.IsNullOrEmpty())
            {
                rootPath = this.fileSystem.Directory.GetCurrentDirectory();
            }

            // Normalize root dir path so it always ends with a directory separator.
            // This ensures IOUri.Resolve() works correctly.
            rootPath = rootPath.EndsWith(directorySeparatorChar) || rootPath.EndsWith(altDirectorySeparatorChar) ? rootPath : rootPath + directorySeparatorChar;
            rootPath = this.GetFullPath(rootPath);

            return (rootPath, relativePattern);
        }
    }
}
