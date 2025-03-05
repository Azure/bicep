// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bicep.Core.Diagnostics;
using Bicep.Core.Extensions;
using Bicep.Core.UnitTests.Assertions;
using Bicep.Core.UnitTests.Mock;
using Bicep.IO.Abstraction;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bicep.Core.UnitTests.Extensions
{
    [TestClass]
    public class IFileHandleExtensionsTests
    {
        [DataTestMethod]
        [DataRow("/main.json", true)]
        [DataRow("/main.jsonc", true)]
        [DataRow("/main.arm", true)]
        [DataRow("/main.txt", false)]
        public void IsArmTemplateLikeFile_VariousExtensions_ReturnsExpectedResult(string filePath, bool expectedResult)
        {
            // Arrange
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.Uri).Returns(new IOUri("file", "", filePath));

            // Act
            var result = sut.Object.IsArmTemplateLikeFile();

            // Assert
            result.Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow("/main.bicep", true)]
        [DataRow("/main.txt", false)]
        public void IsBicepFile_VariousExtensions_ReturnsExpectedResult(string filePath, bool expectedResult)
        {
            // Arrange
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.Uri).Returns(new IOUri("file", "", filePath));

            // Act
            var result = sut.Object.IsBicepFile();

            // Assert
            result.Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow("/main.bicepparam", true)]
        [DataRow("/main.txt", false)]
        public void IsBicepParamsFile_VariousExtensions_ReturnsExpectedResult(string filePath, bool expectedResult)
        {
            // Arrange
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.Uri).Returns(new IOUri("file", "", filePath));

            // Act
            var result = sut.Object.IsBicepParamFile();

            // Assert
            result.Should().Be(expectedResult);
        }

        [DataTestMethod]
        [DataRow("Peek at this contents", 5, "Peek ")]
        [DataRow("Short", 10, "Short")]
        public void TryPeek_PositiveLength_ReturnsContentsUpToLength(string contents, int length, string expectedContents)
        {
            // Arrange
            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes(contents));
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.OpenRead()).Returns(mockStream);

            // Act
            var result = sut.Object.TryPeekText(length);

            // Assert
            result.IsSuccess(out var actualContents).Should().BeTrue();
            actualContents.Should().Be(expectedContents);
        }

        [TestMethod]
        public void TryReadAllText_ByDefault_ReturnsAllContents()
        {
            // Arrange
            var expectedContents = "file contents";
            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContents));
            var sut = StrictMock.Of<IFileHandle>();

            sut.Setup(fh => fh.OpenRead()).Returns(mockStream);

            // Act
            var result = sut.Object.TryReadAllText();

            // Assert
            result.IsSuccess(out var contents).Should().BeTrue();
            contents.Should().Be(expectedContents);
        }

        [TestMethod]
        public void TryReadAllText_IOExceptionOccurs_ReturnsError()
        {
            // Arrange
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.OpenRead()).Throws(new IOException("IO error"));

            // Act
            var result = sut.Object.TryReadAllText();

            // Assert
            result.IsSuccess(out _, out var error).Should().BeFalse();
            error!.Should().HaveCode("BCP091");
        }

        [TestMethod]
        public void TryReadBinaryData_ByDefault_ReturnsBinaryData()
        {
            // Arrange
            var expectedContents = "file contents";
            var mockStream = new MemoryStream(Encoding.UTF8.GetBytes(expectedContents));
            var sut = StrictMock.Of<IFileHandle>();
            sut.Setup(fh => fh.OpenRead()).Returns(mockStream);

            // Act
            var result = sut.Object.TryReadBinaryData();

            // Assert
            result.IsSuccess(out var binaryData, out _).Should().BeTrue();
            binaryData!.ToArray().Should().Equal(Encoding.UTF8.GetBytes(expectedContents));
        }

        [TestMethod]
        public void Write_WithBinaryData_WritesToFile()
        {
            // Arrange
            var data = new BinaryData(Encoding.UTF8.GetBytes("file contents"));
            var mockFileSystem = new MockFileSystem();
            var mockStream = mockFileSystem.FileStream.New("test.txt", FileMode.Create);
            var mockFileHandle = StrictMock.Of<IFileHandle>();
            mockFileHandle.Setup(fh => fh.OpenWrite()).Returns(mockStream);

            // Act
            mockFileHandle.Object.Write(data);

            // Assert
            mockFileSystem.File.ReadAllText("test.txt").Should().Be("file contents");
        }

        [TestMethod]
        public void Write_WithStream_WritesToFile()
        {
            // Arrange
            var inputStream = new MemoryStream(Encoding.UTF8.GetBytes("file contents"));
            var mockFileSystem = new MockFileSystem();
            var mockStream = mockFileSystem.FileStream.New("test.txt", FileMode.Create);
            var mockFileHandle = StrictMock.Of<IFileHandle>();
            mockFileHandle.Setup(fh => fh.OpenWrite()).Returns(mockStream);

            // Act
            mockFileHandle.Object.Write(inputStream);

            // Assert
            mockFileSystem.File.ReadAllText("test.txt").Should().Be("file contents");
        }

        [TestMethod]
        public void Write_WithString_WritesToFile()
        {
            // Arrange
            var text = "file contents";
            var mockFileSystem = new MockFileSystem();
            var mockStream = mockFileSystem.FileStream.New("test.txt", FileMode.Create);
            var mockFileHandle = StrictMock.Of<IFileHandle>();
            mockFileHandle.Setup(fh => fh.OpenWrite()).Returns(mockStream);

            // Act
            mockFileHandle.Object.Write(text);

            // Assert
            mockFileSystem.File.ReadAllText("test.txt").Should().Be("file contents");
        }
    }
}
