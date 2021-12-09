// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Bicep.RegistryModuleTool.ModuleFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace Bicep.RegistryModuleTool.UnitTests.TestFixtures.Factories
{
    public static class MockFileSystemFactory
    {
        private const string SampleResourcePrefix = "Bicep.RegistryModuleTool.UnitTests.TestFixtures.SampleFiles";

        private readonly static string MainBicepFileContent = ReadContent($"{SampleResourcePrefix}.{MainBicepFile.FileName}");

        private readonly static string MainArmTemplateFileContent = ReadContent($"{SampleResourcePrefix}.{MainArmTemplateFile.FileName}");

        private readonly static string MainArmTemplateParametersFileContent = ReadContent($"{SampleResourcePrefix}.{MainArmTemplateParametersFile.FileName}");

        private readonly static string MetadataFileContent = ReadContent($"{SampleResourcePrefix}.{MetadataFile.FileName}");

        private readonly static string ReadmeFileContent = ReadContent($"{SampleResourcePrefix}.{ReadmeFile.FileName}");

        private readonly static string VersionFileContent = ReadContent($"{SampleResourcePrefix}.{VersionFile.FileName}");

        public static MockFileSystem CreateFileSystemWithAllValidFiles()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [$"/1.1/{MainBicepFile.FileName}"] = MainBicepFileContent,
                [$"/1.1/{MainArmTemplateFile.FileName}"] = MainArmTemplateFileContent,
                [$"/1.1/{MainArmTemplateParametersFile.FileName}"] = MainArmTemplateParametersFileContent,
                [$"/1.1/{MetadataFile.FileName}"] = MetadataFileContent,
                [$"/1.1/{ReadmeFile.FileName}"] = ReadmeFileContent,
                [$"/1.1/{VersionFile.FileName}"] = VersionFileContent,
            });

            fileSystem.Directory.SetCurrentDirectory(fileSystem.Path.GetFullPath("1.1"));

            return fileSystem;
        }

        private static string ReadContent(string resourceName)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream is null)
            {
                throw new InvalidOperationException($"Could not get resource stream for \"{resourceName}\".");
            }

            using var streamReader = new StreamReader(stream);

            return streamReader.ReadToEnd();
        }
    }
}
