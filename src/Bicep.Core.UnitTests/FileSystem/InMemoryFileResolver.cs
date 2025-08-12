// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.FileSystem;
using FluentAssertions;

namespace Bicep.Core.UnitTests.FileSystem
{
    public class InMemoryFileResolver
    {
        public MockFileSystem MockFileSystem { get; }

        public InMemoryFileResolver(MockFileSystem fileSystem)
        {
            MockFileSystem = fileSystem;
        }

        public InMemoryFileResolver(IReadOnlyDictionary<Uri, string> fileLookup)
            : this(new MockFileSystem(fileLookup.ToDictionary(x => x.Key.LocalPath, x => new MockFileData(x.Value))))
        {
        }

        public static Uri GetFileUri(string unixPath)
        {
            unixPath.Should().NotContain("\\");
            unixPath.Should().StartWith("/");

            if (Path.DirectorySeparatorChar == '\\')
            {
                unixPath = $"/C:{unixPath}";
            }

            return PathHelper.FilePathToFileUrl(unixPath);
        }
    }
}
