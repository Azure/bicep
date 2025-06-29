// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.IO.Abstractions.TestingHelpers;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;

namespace Bicep.TextFixtures.IO
{
    public class MockFileSystemTestFileSet : TestFileSet
    {
        public MockFileSystemTestFileSet()
            : this(new MockFileSystem())
        {
        }

        public MockFileSystemTestFileSet(MockFileSystem fileSystem)
            : base(new FileSystemFileExplorer(fileSystem))
        {
            this.FileSystem = fileSystem;
        }

        public MockFileSystem FileSystem { get; }

        public static MockFileSystemTestFileSet Create(params (string, TestFileData)[] files)
        {
            var testFileSet = new MockFileSystemTestFileSet();

            testFileSet.AddFiles(files);

            return testFileSet;
        }

        public override IOUri GetUri(string path) => TestFileUri.FromMockFileSystemPath(path);
    }
}
