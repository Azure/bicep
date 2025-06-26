// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.IO.Abstraction;
using Bicep.IO.InMemory;
using FluentAssertions;

namespace Bicep.IO.UnitTests.InMemory
{
    [TestClass]
    public class InMemoryDirectoryHandleTests
    {
        [TestMethod]
        public void EnsureExists_WithNewDirectory_CreatesDirectory()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/mydir");
            sut.Exists().Should().BeFalse("directory does not exist yet");

            // Act
            sut.EnsureExists();

            // Assert
            sut.Exists().Should().BeTrue("because EnsureExists should create the directory if it does not exist");
        }

        [TestMethod]
        public void Delete_WhenCalled_RemovesDirectory()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/mydir");
            sut.EnsureExists();

            // Act
            sut.Delete();

            // Assert
            sut.Exists().Should().BeFalse("because Delete removes the directory");
        }

        [TestMethod]
        public void EnumerateDirectories_WithSubdirectories_ReturnsExpectedResults()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");
            sut.EnsureExists();

            // Create subdirectories.
            var childA = sut.GetDirectory("childA").EnsureExists();
            var childB = sut.GetDirectory("childB").EnsureExists();

            // Act
            var subDirs = sut.EnumerateDirectories();

            // Assert
            subDirs.Select(d => d.Uri.Path).Should().BeEquivalentTo(
            [
                "/parent/childA/",
                "/parent/childB/",
            ]);
        }

        [TestMethod]
        public void EnumerateDirectories_WithSearchPattern_FiltersCorrectly()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");
            sut.EnsureExists();

            // Create subdirectories with different names.
            sut.GetDirectory("abc").EnsureExists();
            sut.GetDirectory("abd").EnsureExists();
            sut.GetDirectory("xyz").EnsureExists();

            // Act
            var filteredDirs = sut.EnumerateDirectories("ab*");

            // Assert
            filteredDirs.Select(d => d.Uri.Path).Should().BeEquivalentTo(
            [
                "/parent/abc/",
                "/parent/abd/",
            ]);
        }

        [TestMethod]
        public void EnumerateFiles_NoSearchPattern_ReturnsAllFiles()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");
            sut.EnsureExists();

            // Create files.
            var fileA = sut.GetFile("fileA.txt").EnsureExists();
            var fileB = sut.GetFile("fileB.txt").EnsureExists();

            // Act
            var files = sut.EnumerateFiles();

            // Assert
            files.Select(f => f.Uri.Path).Should().BeEquivalentTo(
            [
                "/parent/fileA.txt",
                "/parent/fileB.txt",
            ]);
        }

        [TestMethod]
        public void EnumerateFiles_WithSearchPattern_FiltersCorrectly()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");
            sut.EnsureExists();

            // Create files with different names.
            var fileFoo = sut.GetFile("foo.txt").EnsureExists();
            var fileBar = sut.GetFile("bar.txt").EnsureExists();
            var fileFoobar = sut.GetFile("foobar.txt").EnsureExists();

            // Act
            var filteredFiles = sut.EnumerateFiles("foo*");

            // Assert
            filteredFiles.Select(f => f.Uri.Path).Should().BeEquivalentTo(
            [
                "/parent/foo.txt",
                "/parent/foobar.txt",
            ]);
        }

        [TestMethod]
        public void GetDirectory_WithRelativePath_ReturnsChildDirectory()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");

            // Act
            var child = sut.GetDirectory("child");

            // Assert
            child.Uri.Path.Should().Be("/parent/child/");
        }

        [TestMethod]
        public void GetFile_WithRelativePath_ReturnsChildFile()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");

            // Act
            var file = sut.GetFile("file.txt");

            // Assert
            file.Uri.Path.Should().Be("/parent/file.txt");
        }

        [TestMethod]
        public void GetParent_WithRootPath_ReturnsNull()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/");

            // Act
            var parent = sut.GetParent();

            // Assert
            parent.Should().BeNull();
        }

        [TestMethod]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");

            // Act
            var result = sut.Equals(sut);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_DifferentInstancesSameUri_ReturnsTrue()
        {
            // Arrange
            var sut = CreateInMemoryDirectoryHandle("/parent");
            var other = CreateInMemoryDirectoryHandle("/parent");

            // Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeTrue();
        }

        /// <summary>
        /// Creates a directory handle for the specified path (using the "inmemory" scheme),
        /// and returns the handle plus the file store so we can manage test data.
        /// </summary>
        private static InMemoryDirectoryHandle CreateInMemoryDirectoryHandle(string path)
        {
            var store = new InMemoryFileExplorer.FileStore();
            var uri = new IOUri("file", "", path);
            var directoryHandle = new InMemoryDirectoryHandle(store, uri);

            return directoryHandle;
        }
    }
}
