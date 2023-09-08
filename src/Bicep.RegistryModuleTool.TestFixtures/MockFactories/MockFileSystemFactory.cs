// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using System.Reflection;

namespace Bicep.RegistryModuleTool.TestFixtures.MockFactories
{
    public static partial class MockFileSystemFactory
    {
        private const string CurrentDirectory = "/modules/test/testmodule";

        public static MockFileSystem CreateForSample(Sample sample)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileSystem = new MockFileSystem();

            fileSystem.AddDirectory(CurrentDirectory);
            fileSystem.Directory.SetCurrentDirectory(fileSystem.Path.GetFullPath(CurrentDirectory));

            foreach (var (path, resourceName) in sample.EnumerateResources())
            {
                fileSystem.AddFileFromEmbeddedResource(path, assembly, resourceName);
            }

            return fileSystem;
        }
    }
}
