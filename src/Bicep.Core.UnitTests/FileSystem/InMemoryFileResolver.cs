// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using Bicep.Core.FileSystem;
using System.IO;
using FluentAssertions;

namespace Bicep.Core.UnitTests.FileSystem
{
    public class InMemoryFileResolver : FileResolver
    {
        public MockFileSystem MockFileSystem { get; }

        public InMemoryFileResolver(MockFileSystem fileSystem)
            : base(fileSystem)
        {
            MockFileSystem = fileSystem;
        }

        public InMemoryFileResolver(IReadOnlyDictionary<Uri, string> fileLookup)
            : this(new MockFileSystem(fileLookup.ToDictionary(x => x.Key.LocalPath, x => new MockFileData(x.Value))))
        {
        }

        public InMemoryFileResolver(IReadOnlyDictionary<Uri, MockFileData> fileLookup)
            : this(new MockFileSystem(fileLookup.ToDictionary(x => x.Key.LocalPath, x => x.Value)))
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
