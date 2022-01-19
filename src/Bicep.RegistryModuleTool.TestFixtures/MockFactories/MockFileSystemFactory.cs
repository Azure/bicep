// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public static class MockFileSystemFactory
    {
        private const string CurrentDirectory = "modules/microsoft.test/testmodule/1.1";

        public static MockFileSystem CreateFileSystemWithEmptyFolder() => CreateFileSystem(Enumerable.Empty<(string, string)>());

        public static MockFileSystem CreateFileSystemWithNewlyGeneratedFiles() => CreateFileSystem(SampleFiles.NewlyGenerated);

        public static MockFileSystem CreateFileSystemWithModifiedFiles() => CreateFileSystem(SampleFiles.Modified);

        public static MockFileSystem CreateFileSystemWithValidFiles() => CreateFileSystem(SampleFiles.Valid);

        public static MockFileSystem CreateFileSystemWithInvalidFiles() => CreateFileSystem(SampleFiles.Invalid);

        private static MockFileSystem CreateFileSystem(IEnumerable<(string, string)> sampleFiles)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileSystem = new MockFileSystem();

            fileSystem.AddFile("/bin/bicep", "");
            fileSystem.AddFile("/bin/bicep.exe", "");

            fileSystem.AddDirectory(CurrentDirectory);
            fileSystem.Directory.SetCurrentDirectory(fileSystem.Path.GetFullPath(CurrentDirectory));

            foreach (var (path, resourceName) in sampleFiles)
            {
                fileSystem.AddFileFromEmbeddedResource(path, assembly, resourceName);
            }

            return fileSystem;
        }

        private static class SampleFiles
        {
            private const string SampleResourcePrefix = "Bicep.RegistryModuleTool.TestFixtures.SampleFiles";

            public static IEnumerable<(string, string)> NewlyGenerated { get; } = LoadSampleFiles();

            public static IEnumerable<(string, string)> Modified { get; } = LoadSampleFiles();

            public static IEnumerable<(string, string)> Valid { get; } = LoadSampleFiles();
            
            public static IEnumerable<(string, string)> Invalid { get; } = LoadSampleFiles();

            private static IEnumerable<(string, string)> LoadSampleFiles([CallerMemberName] string? category = null)
            {
                var prefix = $"{SampleResourcePrefix}.{category}.";
                var sampleResourceNames = Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceNames()
                    .Where(x => x.StartsWith(prefix));

                if (!sampleResourceNames.Any())
                {
                    throw new InvalidOperationException("Could not find any embeded sample files.");
                }

                var testPrefix = $"{prefix}test.";

                return sampleResourceNames.Select(resourceName =>
                {
                    string path = resourceName.StartsWith(testPrefix)
                        ? $"test/{resourceName[testPrefix.Length..]}"
                        : resourceName[prefix.Length..];

                    return (path, resourceName);
                });
            }
        }
    }
}
