// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.Core.UnitTests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using System.Collections.Immutable;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;

namespace Bicep.Core.UnitTests.Baselines
{
    public record BaselineFolder(
        string OutputFolderPath,
        string StreamFolderPath,
        ImmutableDictionary<string, BaselineFile> Files,
        BaselineFile EntryFile)
    {
        public static BaselineFolder BuildOutputFolder(TestContext testContext, EmbeddedFile embeddedFile)
        {
            var outputDirectory = FileHelper.GetUniqueTestOutputPath(testContext);
            var parentStream = Path.GetDirectoryName(embeddedFile.StreamPath)!.Replace('\\', '/');
            var entryFileRelativePath = embeddedFile.StreamPath.Substring(parentStream.Length).TrimStart('/');

            var baselineFiles = new Dictionary<string, BaselineFile>();
            foreach (var streamPath in embeddedFile.Assembly.GetManifestResourceNames()
                .Where(file => file.StartsWith(parentStream, StringComparison.Ordinal)))
            {
                var relativePath = streamPath.Substring(parentStream.Length).TrimStart('/');
                var filePath = Path.Combine(outputDirectory, relativePath);

                baselineFiles[relativePath] = new(
                    testContext,
                    new EmbeddedFile(embeddedFile.Assembly, streamPath),
                    filePath);
            }

            foreach (var baselineFile in baselineFiles.Values)
            {
                var directoryPath = Path.GetDirectoryName(baselineFile.OutputFilePath)!;
                Directory.CreateDirectory(directoryPath);

                File.WriteAllText(baselineFile.OutputFilePath, baselineFile.EmbeddedFile.Contents);
                testContext.AddResultFile(baselineFile.OutputFilePath);
            }

            return new(
                outputDirectory,
                parentStream,
                baselineFiles.ToImmutableDictionary(),
                baselineFiles[entryFileRelativePath]);
        }

        public BaselineFile? TryGetFile(string relativePath)
            => Files.TryGetValue(relativePath);

        private string GetBaselineStreamRelativePath(string filePath)
            => filePath.StartsWith(OutputFolderPath) ?
            filePath.Substring(OutputFolderPath.Length).Replace('\\', '/').TrimStart('/') :
            throw new InvalidOperationException($"FilePath {filePath} is not a sub-path of {OutputFolderPath}");

        public BaselineFile GetFileOrEnsureCheckedIn(Uri fileUri)
            => GetFileOrEnsureCheckedIn(GetBaselineStreamRelativePath(fileUri.LocalPath));

        public BaselineFile GetFileOrEnsureCheckedIn(string relativePath)
        {
            if (TryGetFile(relativePath) is {} baselineFile)
            {
                return baselineFile;
            }

            var embeddedFile = new EmbeddedFile(
                this.EntryFile.EmbeddedFile.Assembly,
                $"{this.StreamFolderPath}/{relativePath}");

            var outputFile = Path.Combine(this.OutputFolderPath, relativePath);
            File.WriteAllText(outputFile, "");

            "".Should().EqualWithLineByLineDiffOutput(
                this.EntryFile.TestContext,
                "<missing>",
                expectedLocation: embeddedFile.RelativeSourcePath,
                actualLocation: outputFile);
            throw new NotImplementedException("Code cannot reach this point as the previous line will always throw");
        }
    }
}
