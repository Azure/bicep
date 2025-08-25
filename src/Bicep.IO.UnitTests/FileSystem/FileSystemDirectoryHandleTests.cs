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
    public class FileSystemDirectoryHandleTests
    {
        [TestMethod]
        public void Exists_DirectoryDoesNotExist_ReturnsFalse()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();

            var sut = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act & Assert.
            sut.Exists().Should().BeFalse();
        }

        [TestMethod]
        public void Exists_DirectoryExists_ReturnsTrue()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir");

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act & Assert.
            directoryHandle.Exists().Should().BeTrue();
        }

        [TestMethod]
        public void GetDirectory_RelativePath_ReturnsChildDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var childDirectory = directoryHandle.GetDirectory("child");

            // Assert.
            childDirectory.Uri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/dir/child/"));
        }

        [TestMethod]
        public void GetFile_RelativePath_ReturnsChildFile()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var childFile = directoryHandle.GetFile("child.txt");

            // Assert.
            childFile.Uri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/dir/child.txt"));
        }

        [TestMethod]
        public void GetParent_ParentDirectoryDoesNotExists_ReturnsNull()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/foo");

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
            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir/subdir");

            // Act.
            var parentDirectory = directoryHandle.GetParent();

            // Assert.
            parentDirectory.Should().NotBeNull();
            parentDirectory!.Uri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/dir/"));
        }

        [TestMethod]
        public void EnsureExists_DirectoryDoesNotExist_CreatesDirectory()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/newdir");

            // Act.
            directoryHandle.EnsureExists();

            // Assert.
            fileSystem.Directory.Exists("/newdir").Should().BeTrue();
        }

        [TestMethod]
        public void EnumerateDirectories_NoSubdirectories_ReturnsEmpty()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir");

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var subdirectories = directoryHandle.EnumerateDirectories();

            // Assert.
            subdirectories.Should().BeEmpty();
        }

        [TestMethod]
        public void EnumerateDirectories_WithSubdirectories_ReturnsSubdirectories()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir/subdir1");
            fileSystem.AddDirectory("/dir/subdir2");

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var subdirectories = directoryHandle.EnumerateDirectories().ToList();

            // Assert.
#if WINDOWS_BUILD
            subdirectories.Should().HaveCount(2);
            subdirectories.Select(d => d.Uri.GetFilePath()).Should().Contain(new[] { @"C:\dir\subdir1\", @"C:\dir\subdir2\" });
#else
            subdirectories.Should().HaveCount(2);
            subdirectories.Select(d => d.Uri.GetLocalFilePath()).Should().Contain(new[] { "/dir/subdir1/", "/dir/subdir2/" });
#endif
        }

        [TestMethod]
        public void EnumerateDirectories_WithSearchPattern_ReturnsFilteredSubdirectories()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir/subdir1");
            fileSystem.AddDirectory("/dir/subdir2");
            fileSystem.AddDirectory("/dir/otherdir");

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var subdirectories = directoryHandle.EnumerateDirectories("sub*").ToList();

            // Assert.
#if WINDOWS_BUILD
            subdirectories.Should().HaveCount(2);
            subdirectories.Select(d => d.Uri.GetFilePath()).Should().Contain(new[] { @"C:\dir\subdir1\", @"C:\dir\subdir2\" });
#else
            subdirectories.Should().HaveCount(2);
            subdirectories.Select(d => d.Uri.GetLocalFilePath()).Should().Contain(new[] { "/dir/subdir1/", "/dir/subdir2/" });
#endif
        }

        [TestMethod]
        public void EnumerateFiles_NoFiles_ReturnsEmpty()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("/dir");

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var files = directoryHandle.EnumerateFiles();

            // Assert.
            files.Should().BeEmpty();
        }

        [TestMethod]
        public void EnumerateFiles_WithFiles_ReturnsFiles()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/dir/file1.txt", new MockFileData("Content1"));
            fileSystem.AddFile("/dir/file2.txt", new MockFileData("Content2"));

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var files = directoryHandle.EnumerateFiles().ToList();

            // Assert.
#if WINDOWS_BUILD
            files.Should().HaveCount(2);
            files.Select(d => d.Uri.GetFilePath()).Should().Contain(new[] { @"C:\dir\file1.txt", @"C:\dir\file2.txt" });
#else
            files.Should().HaveCount(2);
            files.Select(d => d.Uri.GetLocalFilePath()).Should().Contain(new[] { "/dir/file1.txt", "/dir/file2.txt" });
#endif
        }

        [TestMethod]
        public void EnumerateFiles_WithSearchPattern_ReturnsFilteredFiles()
        {
            // Arrange.
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/dir/file1.txt", new MockFileData("Content1"));
            fileSystem.AddFile("/dir/file2.log", new MockFileData("Content2"));
            fileSystem.AddFile("/dir/file3.txt", new MockFileData("Content3"));

            var directoryHandle = CreateFileSystemDirectoryHandle(fileSystem, "/dir");

            // Act.
            var files = directoryHandle.EnumerateFiles("*.txt").ToList();

            // Assert.
#if WINDOWS_BUILD
            files.Should().HaveCount(2);
            files.Select(d => d.Uri.GetFilePath()).Should().Contain(new[] { @"C:\dir\file1.txt", @"C:\dir\file3.txt" });
#else
            files.Should().HaveCount(2);
            files.Select(d => d.Uri.GetLocalFilePath()).Should().Contain(new[] { "/dir/file1.txt", "/dir/file3.txt" });
#endif
        }

        private static FileSystemDirectoryHandle CreateFileSystemDirectoryHandle(MockFileSystem fileSystem, string path)
        {
            path = fileSystem.Path.GetFullPath(path);

            return new(fileSystem, IOUri.FromFilePath(path));
        }
    }
}
