// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var fileHandle = new FileSystemFileHandle(fileSystem, "/file");

            // Act & Assert.
            fileHandle.Exists().Should().BeFalse();
        }

        [TestMethod]
        public void Exists_FileExists_ReturnsTrue()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/file", new MockFileData("content"));
            var fileHandle = new FileSystemFileHandle(fileSystem, "/file");

            // Act & Assert.
            fileHandle.Exists().Should().BeTrue();
        }

        [TestMethod]
        public void GetParent_ByDefault_ReturnsParentDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var fileHandle = new FileSystemFileHandle(fileSystem, "/dir/file");

            // Act.
            var parentDirectory = fileHandle.GetParent();

            // Assert.
            parentDirectory.Identifier.GetFileSystemPath().Should().Be(fileSystem.Path.GetFullPath("/dir/"));
        }
    }
}
