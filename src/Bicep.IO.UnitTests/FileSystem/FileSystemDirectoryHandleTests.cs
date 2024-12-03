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
    public class FileSystemDirectoryHandleTests
    {
        [TestMethod]
        public void Exists_DirectoryDoesNotExist_ReturnsFalse()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act & Assert.
            directoryHandle.Exists().Should().BeFalse();
        }

        [TestMethod]
        public void Exists_DirectoryExists_ReturnsTrue()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir");

            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act & Assert.
            directoryHandle.Exists().Should().BeTrue();
        }

        [TestMethod]
        public void GetDirectory_NonRelativePath_Throws()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            Action act = () => directoryHandle.GetDirectory("/non-relative-path");

            // Assert.
            act.Should().Throw<FileSystemPathException>().WithMessage("Path must be relative.");
        }

        [TestMethod]
        public void GetDirectory_RelativePath_ReturnsChildDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var childDirectory = directoryHandle.GetDirectory("child");

            // Assert.
            childDirectory.Uri.GetFileSystemPath().Should().Be(fileSystem.Path.GetFullPath("/dir/child/"));
        }

        [TestMethod]
        public void GetFile_NonRelativePath_Throws()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            Action act = () => directoryHandle.GetFile("/non-relative-path");

            // Assert.
            act.Should().Throw<FileSystemPathException>().WithMessage("Path must be relative.");
        }

        [TestMethod]
        public void GetFile_RelativePath_ReturnsChildFile()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var childFile = directoryHandle.GetFile("child.txt");

            // Assert.
            childFile.Uri.GetFileSystemPath().Should().Be(fileSystem.Path.GetFullPath("/dir/child.txt"));
        }

        [TestMethod]
        public void GetParent_ParentDirectoryDoesNotExists_ReturnsNull()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/foo");

            // Act.
            var parentDirectory = directoryHandle.GetParent()?.GetParent();

            // Assert.
            parentDirectory.Should().BeNull();
        }

        [TestMethod]
        public void GetParent_ParentDirectoryExists_ReturnsParentDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/dir/subdir");

            // Act.
            var parentDirectory = directoryHandle.GetParent();

            // Assert.
            parentDirectory.Should().NotBeNull();
            parentDirectory!.Uri.GetFileSystemPath().Should().Be(fileSystem.Path.GetFullPath("/dir/"));
        }

        [TestMethod]
        public void EnsureExists_DirectoryDoesNotExist_CreatesDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = new FileSystemDirectoryHandle(fileSystem, "/newdir");

            // Act.
            directoryHandle.EnsureExists();

            // Assert.
            fileSystem.Directory.Exists("/newdir").Should().BeTrue();
        }
    }
}
