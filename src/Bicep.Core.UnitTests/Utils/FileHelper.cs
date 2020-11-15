// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Utils
{
    public static class FileHelper
    {
        public static string GetResultFilePath(TestContext testContext, string fileName)
        {
            string filePath = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath) ?? throw new AssertFailedException($"There is no directory path for file '{filePath}'."));
            testContext.AddResultFile(filePath);

            return filePath;
        }

        public static string SaveResultFile(TestContext testContext, string fileName, string contents)
        {
            var filePath = GetResultFilePath(testContext, fileName);
            File.WriteAllText(filePath, contents);

            return filePath;
        }

        public static string SaveEmbeddedResourcesWithPathPrefix(TestContext testContext, Assembly containingAssembly, string outputDirName, string manifestFilePrefix)
        {
            string outputDirectory = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName, outputDirName);

            var filesSaved = false;
            foreach (var embeddedResourceName in containingAssembly.GetManifestResourceNames().Where(file => file.StartsWith(manifestFilePrefix,  StringComparison.Ordinal)))
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
                fileStream.Close();

                filesSaved = true;
            }

            if (!filesSaved)
            {
                throw new InvalidOperationException($"Failed to find any manifest files to save in assembly {containingAssembly}");
            }

            return outputDirectory;
        }
    }
}