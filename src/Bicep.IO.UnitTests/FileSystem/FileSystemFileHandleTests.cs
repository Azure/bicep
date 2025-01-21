// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.FileSystem;
using FluentAssertions;

namespace Bicep.IO.UnitTests.FileSystem
{
    [TestClass]
    public class FileSystemFileHandleTests
    {
        [TestMethod]
        public void Exists_FileDoesNotExist_ReturnsFalse()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/file");

            // Act & Assert.
            fileHandle.Exists().Should().BeFalse();
        }

        [TestMethod]
        public void Exists_FileExists_ReturnsTrue()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/file", new MockFileData("content"));
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/file");

            // Act & Assert.
            fileHandle.Exists().Should().BeTrue();
        }

        [TestMethod]
        public void GetParent_ByDefault_ReturnsParentDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/dir/file");

            // Act.
            var parentDirectory = fileHandle.GetParent();

            // Assert.
            parentDirectory.Uri.GetLocalFilePath().Should().Be(fileSystem.Path.GetFullPath("/dir/"));
        }

        [TestMethod]
        public void OpenWrite_ParentDirectoriesDoNotExist_CreatesParentDirectories()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/dir/subdir/file.txt");

            // Act.
            using (var stream = fileHandle.OpenWrite())
            {
                var content = Encoding.UTF8.GetBytes("Hello, World!");
                stream.Write(content, 0, content.Length);
            }

            // Assert.
            fileSystem.Directory.Exists("/dir/subdir").Should().BeTrue();
            fileSystem.File.Exists("/dir/subdir/file.txt").Should().BeTrue();
            fileSystem.File.ReadAllText("/dir/subdir/file.txt").Should().Be("Hello, World!");
        }

        private static FileSystemFileHandle CreateFileSystemFileHandle(MockFileSystem fileSystem, string path)
        {
            path = fileSystem.Path.GetFullPath(path);

            return new FileSystemFileHandle(fileSystem, IOUri.FromLocalFilePath(path));
        }
    }
}
