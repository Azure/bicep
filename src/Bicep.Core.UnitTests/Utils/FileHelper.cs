// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.Collections.Immutable;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Text;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class FileHelper
    {
        public static string GetUniqueTestOutputPath(TestContext testContext)
            => Path.Combine(testContext.ResultsDirectory!, Guid.NewGuid().ToString());

        public static string GetResultFilePath(TestContext testContext, string fileName, string? testOutputPath = null)
        {
            string filePath = Path.Combine(testOutputPath ?? GetUniqueTestOutputPath(testContext), fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new AssertFailedException($"There is no directory path for file '{filePath}'."));
            testContext.AddResultFile(filePath);

            return filePath;
        }

        public static string SaveResultFile(TestContext testContext, string fileName, string contents, string? testOutputPath = null, Encoding? encoding = null)
        {
            var outputPath = SaveResultFiles(testContext, [new ResultFile(fileName, contents, encoding)], testOutputPath);

            return Path.Combine(outputPath, fileName);
        }

        public record ResultFile(string FileName, string Contents, Encoding? Encoding = null);

        public static string SaveResultFiles(TestContext testContext, ResultFile[] files, string? testOutputPath = null)
        {
            var outputPath = testOutputPath ?? GetUniqueTestOutputPath(testContext);

            foreach (var (fileName, contents, encoding) in files)
            {
                var filePath = GetResultFilePath(testContext, fileName, outputPath);

                if (encoding is not null)
                {
                    File.WriteAllText(filePath, contents, encoding);
                }
                else
                {
                    File.WriteAllText(filePath, contents);
                }
            }

            return outputPath;
        }

        public static string SaveEmbeddedResourcesWithPathPrefix(TestContext testContext, Assembly containingAssembly, string manifestFilePrefix)
        {
            var outputDirectory = GetUniqueTestOutputPath(testContext);

            var filesSaved = false;
            foreach (var embeddedResourceName in containingAssembly.GetManifestResourceNames().Where(file => file.StartsWith(manifestFilePrefix, StringComparison.Ordinal)))
            {
                var relativePath = embeddedResourceName.Substring(manifestFilePrefix.Length).TrimStart('/');
                var manifestStream = containingAssembly.GetManifestResourceStream(embeddedResourceName);
                if (manifestStream == null)
                {
                    throw new ArgumentException($"Failed to load stream for manifest resource {embeddedResourceName}");
                }

                var filePath = Path.Combine(outputDirectory, relativePath);
                var directoryPath = Path.GetDirectoryName(filePath) ?? throw new AssertFailedException($"There is no directory path for file '{filePath}'.");
                Directory.CreateDirectory(directoryPath);

                var fileStream = File.Create(filePath);
                manifestStream.Seek(0, SeekOrigin.Begin);
                manifestStream.CopyTo(fileStream);
                testContext.WriteLine($"Bytes written to {filePath}: {fileStream.Position}");

                fileStream.Close();

                testContext.AddResultFile(filePath);
                filesSaved = true;
            }

            if (!filesSaved)
            {
                throw new InvalidOperationException($"Failed to find any manifest files to save in assembly {containingAssembly}");
            }

            return outputDirectory;
        }

        public static IDirectoryHandle GetCacheRootDirectory(TestContext testContext)
        {
            var path = GetUniqueTestOutputPath(testContext);
            var uri = IOUri.FromFilePath(path);

            return BicepTestConstants.FileExplorer.GetDirectory(uri);
        }

        public static ImmutableDictionary<string, string> BuildEmbeddedFileDictionary(Assembly containingAssembly, string streamNamePrefix)
        {
            var matches = containingAssembly
                .GetManifestResourceNames()
                .Where(streamName => streamName.StartsWith(streamNamePrefix, StringComparison.Ordinal))
                .Select(streamName => (streamName, key: streamName.Substring(streamNamePrefix.Length)));

            var builder = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var (streamName, key) in matches)
            {
                builder.Add(key, ReadEmbeddedFile(containingAssembly, streamName));
            }

            return builder.ToImmutable();
        }

        public static string ReadEmbeddedFile(Assembly containingAssembly, string streamName)
        {
            using var stream = containingAssembly.GetManifestResourceStream(streamName);
            stream.Should().NotBeNull($"because stream '{streamName}' should exist");

            using var reader = new StreamReader(stream!);

            return reader.ReadToEnd();
        }

        public static IFileSystem CreateMockFileSystemForEmbeddedFiles(Assembly containingAssembly, string streamNamePrefix)
        {
            var files = BuildEmbeddedFileDictionary(containingAssembly, streamNamePrefix);

            return new MockFileSystem(files.ToDictionary(
                x => x.Key,
                x => new MockFileData(x.Value)));
        }
    }
}
