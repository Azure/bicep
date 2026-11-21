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
            parentDirectory.Uri.GetFilePath().Should().Be(fileSystem.Path.GetFullPath("/dir/"));
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

        [TestMethod]
        public void TryGetVersion_FileDoesNotExist_ReturnsNull()
        {
            var fileSystem = new MockFileSystem();
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/file");

            fileHandle.TryGetVersion().Should().BeNull();
        }

        [TestMethod]
        public void TryGetVersion_FileRewritten_ReturnsDifferentVersion()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/file", new MockFileData("content") { LastWriteTime = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc) });
            var fileHandle = CreateFileSystemFileHandle(fileSystem, "/file");
            var version = fileHandle.TryGetVersion();

            fileHandle.WriteAllText("content");

            version.Should().Be(new FileVersion(new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks, "content".Length));
            fileHandle.TryGetVersion().Should().NotBe(version);
        }

        [TestMethod]
        public void TryGetVersion_SymbolicLinkTargetChanged_ReturnsDifferentVersion()
        {
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile("/target.json", new MockFileData("{}"));
            fileSystem.File.CreateSymbolicLink(fileSystem.Path.GetFullPath("/link.json"), fileSystem.Path.GetFullPath("/target.json"));
            var link = CreateFileSystemFileHandle(fileSystem, "/link.json");
            var version = link.TryGetVersion();

            // Reading the link reads the target, so editing the target changes the link's version.
            fileSystem.File.WriteAllText(fileSystem.Path.GetFullPath("/target.json"), """{ "changed": true }""");

            version.Should().NotBeNull();
            link.TryGetVersion().Should().NotBe(version);
        }

        private static FileSystemFileHandle CreateFileSystemFileHandle(MockFileSystem fileSystem, string path)
        {
            path = fileSystem.Path.GetFullPath(path);

            return new FileSystemFileHandle(fileSystem, IOUri.FromFilePath(path));
        }
    }
}
