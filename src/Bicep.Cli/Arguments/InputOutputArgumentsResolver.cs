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

        public IOUri PathToUri(string path) => IOUri.FromLocalFilePath(GetFullPath(path));

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

            throw new InvalidOperationException("Either InputFile or FilePattern must be specified.");
        }

        public IReadOnlyList<(IOUri InputUri, IOUri OutputUri)> ResolveFilePatternInputOutputArguments<T>(T arguments)
            where T : IFilePatternInputOutputArguments<T>
        {
            if (arguments.InputFile is not null)
            {
                return [this.ResolveInputOutputArguments(arguments)];
            }

            if (arguments.FilePattern is not null)
            {
                var result = new List<(IOUri InputUri, IOUri OutputUri)>();
                var (rootUri, inputRelativePaths) = this.ResolveFilePattern(arguments.FilePattern);

                foreach (var inputRelativePath in inputRelativePaths)
                {
                    var inputUri = rootUri.Resolve(inputRelativePath);
                    var outputRootPath = arguments.OutputDir ?? rootUri.GetLocalFilePath();
                    var outputRelativePath = this.fileSystem.Path.ChangeExtension(inputRelativePath, T.OutputFileExtensionResolver.Invoke(arguments, inputUri));
                    var outputPath = this.fileSystem.Path.Combine(outputRootPath, outputRelativePath);
                    var outputUri = this.PathToUri(outputPath);

                    result.Add((inputUri, outputUri));
                }

                return result;
            }

            throw new InvalidOperationException("Either InputFile or FilePattern must be specified.");
        }

        private IOUri ResolveOutputUri(IOUri inputUri, string? outputDir, string? outputFile, string outputFileExtension)
        {
            if (outputDir is not null)
            {
                outputDir = this.fileSystem.Path.GetFullPath(outputDir);
                var outputFileName = inputUri.GetFileNameWithoutExtension().ToString() + outputFileExtension;
                var outputPath = this.fileSystem.Path.Combine(outputDir, outputFileName);

                return this.PathToUri(outputPath);
            }

            if (outputFile is not null)
            {
                return this.PathToUri(this.fileSystem.Path.GetFullPath(outputFile));
            }

            return inputUri.WithExtension(outputFileExtension);
        }

        private string GetFullPath(string path) => this.fileSystem.Path.GetFullPath(path);

        private (IOUri rootUri, IReadOnlyList<string> relativePaths) ResolveFilePattern(string filePattern)
        {
            var (rootPath, relativePattern) = SplitFilePatternOnWildcard(filePattern);
            var rootUri = IOUri.FromLocalFilePath(rootPath);

            Matcher matcher = new();
            matcher.AddInclude(relativePattern);

            var relativePaths = new List<string>();
            foreach (var filePath in matcher.GetResultsInFullPath(rootPath))
            {
                var fileUri = IOUri.FromLocalFilePath(filePath);
                var relativePath = fileUri.GetPathRelativeTo(rootUri);
                relativePaths.Add(relativePath);
            }

            return (rootUri, relativePaths);
        }

        public (string rootPath, string relativePattern) SplitFilePatternOnWildcard(string filePattern)
        {
            var directorySeparatorChar = this.fileSystem.Path.DirectorySeparatorChar;
            var altDirectorySeparatorChar = this.fileSystem.Path.AltDirectorySeparatorChar;

            var wildcardIndex = filePattern.IndexOf('*');
            if (wildcardIndex == -1)
            {
                wildcardIndex = filePattern.Length;
            }

            // We intentionally want different behavior on different OSes.
            // Linux treats \ as a regular character, while Windows treats it as a path separator.
            var prevDirIndex = filePattern[..wildcardIndex].LastIndexOfAny([directorySeparatorChar, altDirectorySeparatorChar]);
            var rootPath = prevDirIndex != -1 ? filePattern[..prevDirIndex] : "";
            var relativePattern = prevDirIndex != -1 ? filePattern[(prevDirIndex + 1)..] : filePattern;

            if (rootPath.IsNullOrEmpty())
            {
                rootPath = this.fileSystem.Directory.GetCurrentDirectory();
            }

            rootPath = rootPath.EndsWith(directorySeparatorChar) || rootPath.EndsWith(altDirectorySeparatorChar) ? rootPath : rootPath + directorySeparatorChar;
            rootPath = this.GetFullPath(rootPath);

            return (rootPath, relativePattern);
        }
    }
}
