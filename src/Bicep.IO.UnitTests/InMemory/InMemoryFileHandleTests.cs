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
    public class InMemoryFileHandleTests
    {
        [TestMethod]
        public void OpenRead_WithExistingContent_ReturnsCorrectContent()
        {
            // Arrange
            var (sut, store) = CreateFileHandleWithStore("/test.txt");
            store.WriteFile(sut, "Hello, World!");

            // Act
            using var stream = sut.OpenRead();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var content = reader.ReadToEnd();

            // Assert
            content.Should().Be("Hello, World!");
        }

        [TestMethod]
        public void OpenWrite_WithNewContent_UpdatesContent()
        {
            // Arrange
            var (sut, store) = CreateFileHandleWithStore("/test.txt");
            store.WriteFile(sut, "Initial content");

            // Act
            using (var stream = sut.OpenWrite())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write("Updated content");
            }

            // Assert
            var updatedContent = store.ReadFile(sut);
            updatedContent.Should().Be("Updated content");
        }

        [TestMethod]
        public void Delete_WhenCalled_FileNoLongerExists()
        {
            // Arrange
            var (sut, store) = CreateFileHandleWithStore("/test.txt");
            store.WriteFile(sut, "Some content");
            sut.Exists().Should().BeTrue("because the file was just created");

            // Act
            sut.Delete();

            // Assert
            sut.Exists().Should().BeFalse("because the file was deleted");
        }

        [TestMethod]
        public void EnsureExists_AfterDelete_FileIsRecreated()
        {
            // Arrange
            var (sut, store) = CreateFileHandleWithStore("/test.txt");
            store.WriteFile(sut, "Some content");

            sut.Delete();
            sut.Exists().Should().BeFalse("because the file was deleted");

            // Act
            sut.EnsureExists();

            // Assert
            sut.Exists().Should().BeTrue("because EnsureExists recreated the file");
        }

        [TestMethod]
        public void GetParent_WithNestedPath_ReturnsDirectoryHandle()
        {
            // Arrange
            var (sut, _) = CreateFileHandleWithStore("/folder/file.txt");

            // Act
            var parent = sut.GetParent();

            // Assert
            parent.Uri.Path.Should().Be("/folder/");
        }

        [TestMethod]
        public void MakeExecutable_WhenCalled_ThrowsNotSupportedException()
        {
            // Arrange
            var (sut, _) = CreateFileHandleWithStore("/test.txt");

            // Act
            Action act = () => sut.MakeExecutable();

            // Assert
            act.Should().Throw<NotSupportedException>();
        }

        [TestMethod]
        public void Equals_SameInstance_ReturnsTrue()
        {
            // Arrange
            var (sut, _) = CreateFileHandleWithStore("/test.txt");

            // Act
            var result = sut.Equals(sut);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public void Equals_DifferentInstancesSameUri_ReturnsTrue()
        {
            // Arrange
            var (sut, _) = CreateFileHandleWithStore("/test.txt");
            var (other, _) = CreateFileHandleWithStore("/test.txt");

            // Act
            var result = sut.Equals(other);

            // Assert
            result.Should().BeTrue();
        }

        private static (InMemoryFileHandle Handle, InMemoryFileExplorer.FileStore Store) CreateFileHandleWithStore(string filePath)
        {
            var store = new InMemoryFileExplorer.FileStore();
            var uri = new IOUri("file", string.Empty, filePath);
            var fileHandle = new InMemoryFileHandle(store, uri);

            return (fileHandle, store);
        }
    }
}
