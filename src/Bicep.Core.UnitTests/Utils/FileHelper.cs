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

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
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
            => SaveEmbeddedResourcesToDisk(testContext, containingAssembly, outputDirName, file => file.StartsWith(manifestFilePrefix));

        private static string SaveEmbeddedResourcesToDisk(TestContext testContext, Assembly containingAssembly, string outputDirName, Func<string, bool> manifestFileMatchFunc)
        {
            string outputDirectory = Path.Combine(testContext.TestRunResultsDirectory, testContext.TestName, outputDirName);
            Directory.CreateDirectory(outputDirectory);

            var filesSaved = false;
            var uniqueNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var embeddedResourceName in containingAssembly.GetManifestResourceNames().Where(manifestFileMatchFunc))
            {
                var fileName = Path.GetFileName(embeddedResourceName);
                if (uniqueNames.Contains(fileName))
                {
                    throw new ArgumentException($"Embedded resource {embeddedResourceName} must have a unique file name");
                }
                uniqueNames.Add(fileName);
                
                var manifestStream = containingAssembly.GetManifestResourceStream(embeddedResourceName);
                if (manifestStream == null)
                {
                    throw new ArgumentException($"Failed to load stream for manifest resource {embeddedResourceName}");
                }

                var fileStream = File.Create(Path.Combine(outputDirectory, fileName));
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